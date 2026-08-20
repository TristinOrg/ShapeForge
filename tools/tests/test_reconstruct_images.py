import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

from tools.shapeforge_reconstruct_images import optimize_images, reconstruct_images, score_comparison


class ShapeForgeImageReconstructionTests(unittest.TestCase):
    def test_weighted_score_prioritizes_silhouette_and_view_confidence(self):
        comparison = {"views": [
            {"weight": 2, "confidence": 1, "scores": {"silhouette": 1, "proportion": 0.5, "detail": 0, "color": 0}},
            {"weight": 1, "confidence": 0.5, "scores": {"silhouette": 0, "proportion": 0.5, "detail": 1, "color": 1}},
        ]}

        score, metrics = score_comparison(comparison)

        self.assertGreater(metrics["silhouette"], 0.7)
        self.assertGreater(score, 0.5)

    def test_loop_improves_global_proportion_and_preserves_artifacts(self):
        with tempfile.TemporaryDirectory() as directory:
            folder = Path(directory)
            reference_image = folder / "reference.png"
            self._rectangle(reference_image, 100, 100)
            reference = folder / "reference.json"
            reference.write_text(json.dumps({
                "schema": "shapeforge.reference-images/1.0",
                "id": "reference",
                "images": [{"viewId": "front", "imagePath": str(reference_image), "weight": 1}],
            }), encoding="utf-8")
            model = folder / "model.json"
            model.write_text(json.dumps({
                "schema": "shapeforge.shape/1.0",
                "name": "Model",
                "style": "",
                "root": {
                    "id": "model", "name": "Model", "type": "core/group",
                    "transform": {
                        "position": {"x": 0, "y": 0, "z": 0},
                        "eulerAngles": {"x": 0, "y": 0, "z": 0},
                        "scale": {"x": 1.6, "y": 1, "z": 1},
                    },
                    "appearance": {"colorRole": "", "hasColorOverride": False,
                                   "color": {"r": 1, "g": 1, "b": 1, "a": 1}},
                    "children": [],
                },
            }), encoding="utf-8")
            capture = folder / "capture.json"
            capture.write_text(json.dumps({
                "schema": "shapeforge.render-capture/1.0", "id": "capture", "candidateId": "candidate",
                "width": 256, "height": 256,
                "views": [{"id": "front", "azimuth": 0, "elevation": 0, "framingScale": 1.1}],
            }), encoding="utf-8")
            output = folder / "best.json"
            work = folder / "work"

            result = reconstruct_images(
                model, reference, capture, output, work, 6, 0.85, 0.001, self._invoke)

            final_model = json.loads(output.read_text(encoding="utf-8"))
            self.assertLess(final_model["root"]["transform"]["scale"]["x"], 1.6)
            self.assertGreater(result["bestScore"], 0.6)
            self.assertGreaterEqual(len(result["iterations"]), 2)
            self.assertTrue((work / "report.json").is_file())

    def test_offline_optimizer_renders_candidates_and_preserves_best(self):
        with tempfile.TemporaryDirectory() as directory:
            folder = Path(directory)
            reference_image = folder / "reference.png"
            self._rectangle(reference_image, 100, 100)
            reference = folder / "reference.json"
            reference.write_text(json.dumps({
                "schema": "shapeforge.reference-images/1.0", "id": "reference",
                "images": [{"viewId": "front", "imagePath": str(reference_image), "weight": 1}],
            }), encoding="utf-8")
            model = folder / "model.json"
            model.write_text(json.dumps({"schema": "shapeforge.shape/1.0", "root": {
                "id": "model", "transform": {"position": {"x": 0, "y": 0, "z": 0},
                "scale": {"x": 1.6, "y": 1, "z": 1}}, "children": []}}), encoding="utf-8")
            capture = folder / "capture.json"
            capture.write_text(json.dumps({"schema": "shapeforge.render-capture/1.0", "id": "capture",
                                            "views": [{"id": "front"}]}), encoding="utf-8")
            output = folder / "best.json"

            result = optimize_images(model, reference, capture, output, folder / "work", 8, 0.0001,
                                     self._invoke, include_profiles=False)

            optimized = json.loads(output.read_text(encoding="utf-8"))
            self.assertLess(optimized["root"]["transform"]["scale"]["x"], 1.6)
            self.assertGreater(result["acceptedChanges"], 0)
            self.assertEqual(result["evaluations"], 8)

    def _invoke(self, arguments):
        command = arguments[0]
        if command == "render":
            model = json.loads(Path(arguments[1]).read_text(encoding="utf-8"))
            images = Path(arguments[arguments.index("--images") + 1])
            manifest = Path(arguments[arguments.index("-o") + 1])
            image = images / "front.png"
            scale = model["root"]["transform"]["scale"]
            self._rectangle(image, round(100 * scale["x"]), round(100 * scale["y"]))
            manifest.parent.mkdir(parents=True, exist_ok=True)
            manifest.write_text(json.dumps({
                "captureId": "capture", "candidateId": "candidate",
                "images": [{"viewId": "front", "imagePath": str(image.resolve())}],
            }), encoding="utf-8")
        elif command == "compare":
            comparison = json.loads(Path(arguments[1]).read_text(encoding="utf-8"))
            scores = comparison["views"][0]["scores"]
            overall = sum(scores.values()) / len(scores)
            Path(arguments[arguments.index("-o") + 1]).write_text(
                json.dumps({"overallScore": overall}), encoding="utf-8")
        elif command == "patch":
            model = json.loads(Path(arguments[1]).read_text(encoding="utf-8"))
            patch = json.loads(Path(arguments[2]).read_text(encoding="utf-8"))
            model["root"]["transform"] = patch["operations"][0]["update"]["transform"]
            Path(arguments[arguments.index("-o") + 1]).write_text(json.dumps(model), encoding="utf-8")
        else:
            raise AssertionError(command)

    @staticmethod
    def _rectangle(path, width, height):
        path.parent.mkdir(parents=True, exist_ok=True)
        image = Image.new("RGBA", (256, 256), (0, 0, 0, 0))
        left = (256 - width) // 2
        top = (256 - height) // 2
        ImageDraw.Draw(image).rectangle((left, top, left + width - 1, top + height - 1), fill=(180, 120, 60, 255))
        image.save(path)


if __name__ == "__main__":
    unittest.main()
