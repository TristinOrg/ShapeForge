import sys
import tempfile
import unittest
from pathlib import Path

from tools.shapeforge_external_export import export_external


class ShapeForgeExternalExportTests(unittest.TestCase):
    def test_converter_receives_literal_paths_and_output_is_verified(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            source = root / "input model.glb"
            output = root / "output model.fbx"
            source.write_bytes(b"glb")
            command = [
                sys.executable,
                "-c",
                "import pathlib,sys; pathlib.Path(sys.argv[2]).write_bytes(pathlib.Path(sys.argv[1]).read_bytes())",
                "{input}",
                "{output}",
            ]

            result = export_external(source, output, command)

            self.assertEqual(result["format"], "fbx")
            self.assertEqual(output.read_bytes(), b"glb")

    def test_rejects_missing_placeholders_and_unknown_formats(self):
        with tempfile.TemporaryDirectory() as folder:
            source = Path(folder) / "model.glb"
            source.write_bytes(b"glb")
            with self.assertRaisesRegex(ValueError, "Unsupported"):
                export_external(source, Path(folder) / "model.obj", ["tool", "{input}", "{output}"])
            with self.assertRaisesRegex(ValueError, "placeholders"):
                export_external(source, Path(folder) / "model.usd", ["tool", str(source)])


if __name__ == "__main__":
    unittest.main()
