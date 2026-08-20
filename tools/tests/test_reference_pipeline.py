import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

from tools.shapeforge_reference_pipeline import run_pipeline


class ShapeForgeReferencePipelineTests(unittest.TestCase):
    def test_pipeline_pauses_without_interpretation_then_resumes_to_compiler_handoff(self):
        with tempfile.TemporaryDirectory() as directory:
            root   = Path(directory)
            source = root / "asset.png"
            image  = Image.new("RGB", (320, 420), (210, 210, 210))
            ImageDraw.Draw(image).rectangle((60, 60, 260, 380), fill=(40, 55, 70))
            image.save(source)

            waiting = run_pipeline(source, root / "work")
            self.assertEqual(waiting["status"], "awaiting-review")
            reference_images = json.loads(Path(waiting["artifacts"]["referenceImages"]).read_text(encoding="utf-8"))
            capture_template = json.loads(Path(waiting["artifacts"]["captureTemplate"]).read_text(encoding="utf-8"))
            self.assertEqual([item["viewId"] for item in reference_images["images"]],
                             [item["id"] for item in capture_template["views"]])
            template_path = Path(waiting["artifacts"]["reviewTemplate"])
            review = json.loads(template_path.read_text(encoding="utf-8"))
            for decision in review["decisions"]:
                decision["value"]      = "building" if decision["kind"] == "asset-category" else "reviewed"
                decision["confidence"] = 0.9
                decision["source"]     = "human"
            template_path.write_text(json.dumps(review), encoding="utf-8")

            ready = run_pipeline(source, root / "work", template_path)
            reviewed = json.loads(Path(ready["artifacts"]["reviewedBlueprint"]).read_text(encoding="utf-8"))

            self.assertEqual(ready["status"], "ready-for-compiler")
            self.assertEqual(ready["nextStage"], "category-compiler")
            self.assertEqual(reviewed["classification"]["category"], "building")
            self.assertEqual(reviewed["reviewQueue"], [])

    def test_printed_annotations_override_sampled_colors_without_ai(self):
        with tempfile.TemporaryDirectory() as directory:
            root   = Path(directory)
            source = root / "asset.png"
            image = Image.new("RGB", (300, 400), (210, 210, 210))
            ImageDraw.Draw(image).rectangle((80, 60, 220, 360), fill=(30, 40, 50))
            image.save(source)
            annotations = root / "annotations.json"
            annotations.write_text(json.dumps({
                "schema": "shapeforge.reference-annotations/1.0",
                "blueprintId": "asset",
                "palette": [{"id": "material-main", "hex": "#1C2D44", "label": "Main"}],
                "measurements": {"heightUnits": 3.5},
                "annotations": [{"regionId": "notes", "text": "stylized low-poly", "language": "en"}],
            }), encoding="utf-8")

            result = run_pipeline(source, root / "work", annotations_path=annotations)
            blueprint = json.loads(Path(result["artifacts"]["blueprint"]).read_text(encoding="utf-8"))

            self.assertEqual(blueprint["palette"][0]["hex"], "#1C2D44")
            self.assertEqual(blueprint["palette"][0]["source"], "printed-label")
            self.assertEqual(blueprint["measurements"]["heightUnits"], 3.5)


if __name__ == "__main__":
    unittest.main()
