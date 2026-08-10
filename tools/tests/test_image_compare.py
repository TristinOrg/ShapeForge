import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

from tools.shapeforge_image_compare import compare_images, compare_manifests


class ShapeForgeImageCompareTests(unittest.TestCase):
    def test_identical_transparent_shapes_score_near_one(self):
        image = self._rectangle((60, 40, 196, 216), (220, 80, 40, 255))

        scores, _ = compare_images(image, image.copy())

        for score in scores.values():
            self.assertGreater(score, 0.98)

    def test_aspect_and_color_changes_are_detected(self):
        reference = self._rectangle((70, 30, 186, 226), (220, 80, 40, 255))
        candidate = self._rectangle((30, 70, 226, 186), (40, 80, 220, 255))

        scores, observations = compare_images(reference, candidate)

        self.assertLess(scores["proportion"], 0.7)
        self.assertLess(scores["color"], 0.2)
        self.assertNotEqual(observations["referenceAspect"], observations["candidateAspect"])

    def test_manifests_emit_valid_render_compare_shape(self):
        with tempfile.TemporaryDirectory() as directory:
            folder = Path(directory)
            self._rectangle((50, 40, 206, 216), (180, 120, 60, 255)).save(folder / "reference.png")
            self._rectangle((60, 40, 196, 216), (180, 120, 60, 255)).save(folder / "candidate.png")
            reference = {
                "schema": "shapeforge.reference-images/1.0",
                "id": "reference",
                "images": [{"viewId": "front", "imagePath": "reference.png", "weight": 2}],
            }
            candidate = {
                "captureId": "capture",
                "candidateId": "candidate",
                "images": [{"viewId": "front", "imagePath": "candidate.png"}],
            }
            (folder / "reference.json").write_text(json.dumps(reference), encoding="utf-8")
            (folder / "candidate.json").write_text(json.dumps(candidate), encoding="utf-8")

            result = compare_manifests(folder / "reference.json", folder / "candidate.json")

            self.assertEqual(result["schema"], "shapeforge.render-compare/1.0")
            self.assertEqual(result["views"][0]["viewId"], "front")
            self.assertEqual(result["views"][0]["weight"], 2.0)

    @staticmethod
    def _rectangle(box, color):
        image = Image.new("RGBA", (256, 256), (0, 0, 0, 0))
        ImageDraw.Draw(image).rectangle(box, fill=color)
        return image


if __name__ == "__main__":
    unittest.main()
