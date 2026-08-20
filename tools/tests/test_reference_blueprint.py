import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

from tools.shapeforge_reference_blueprint import analyze_reference


class ShapeForgeReferenceBlueprintTests(unittest.TestCase):
    def test_analyzer_emits_five_measured_views_without_semantic_guessing(self):
        with tempfile.TemporaryDirectory() as directory:
            folder = Path(directory)
            source = folder / "sheet.png"
            image  = Image.new("RGB", (1000, 600), (172, 172, 172))
            draw   = ImageDraw.Draw(image)
            for center in (70, 200, 330, 460, 590):
                draw.ellipse((center - 38, 70, center + 38, 160), fill=(25, 45, 70))
                draw.rounded_rectangle((center - 28, 150, center + 28, 325), 14, fill=(20, 20, 24))
                draw.rectangle((center - 23, 315, center - 5, 500), fill=(25, 25, 28))
                draw.rectangle((center + 5, 315, center + 23, 500), fill=(25, 25, 28))
            image.save(source)

            result = analyze_reference(source, folder / "views")

            self.assertEqual(result["schema"], "shapeforge.reference-blueprint/1.0")
            self.assertEqual([view["viewId"] for view in result["views"]],
                             ["front", "front-three-quarter", "side", "back-three-quarter", "back"])
            self.assertGreater(len(result["views"][0]["silhouette"]), 20)
            self.assertEqual(result["classification"]["category"], "unresolved")
            self.assertEqual(len(result["reviewQueue"]), 4)

    def test_single_building_image_uses_same_category_neutral_contract(self):
        with tempfile.TemporaryDirectory() as directory:
            folder = Path(directory)
            source = folder / "building.png"
            image  = Image.new("RGB", (400, 500), (210, 210, 210))
            draw   = ImageDraw.Draw(image)
            draw.rectangle((70, 120, 330, 450), fill=(120, 70, 45))
            draw.polygon(((50, 130), (200, 30), (350, 130)), fill=(65, 45, 40))
            image.save(source)

            result = analyze_reference(source, folder / "views")

            self.assertEqual([view["viewId"] for view in result["views"]], ["source"])
            self.assertEqual(result["classification"]["category"], "unresolved")


if __name__ == "__main__":
    unittest.main()
