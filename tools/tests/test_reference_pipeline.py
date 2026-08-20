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


if __name__ == "__main__":
    unittest.main()
