import json
import tempfile
import unittest
from pathlib import Path

from tools.shapeforge_reconstruction_benchmark import run_benchmark


class ShapeForgeReconstructionBenchmarkTests(unittest.TestCase):
    def test_aggregates_thresholds_and_failure_modes(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            manifest = root / "corpus.json"
            for name in ("model.json", "reference.json", "capture.json"):
                (root / name).write_text("{}", encoding="utf-8")
            manifest.write_text(json.dumps({
                "schema": "shapeforge.reconstruction-corpus/1.0", "id": "smoke",
                "defaults": {"minimumScore": 0.8},
                "cases": [
                    {"id": "pass", "source": "model.json", "reference": "reference.json", "capture": "capture.json"},
                    {"id": "fail", "source": "model.json", "reference": "reference.json", "capture": "capture.json"},
                ],
            }), encoding="utf-8")

            def reconstruct(model, reference, capture, output, work, *arguments):
                score = 0.9 if work.name == "pass" else 0.6
                work.mkdir(parents=True, exist_ok=True)
                output.write_text("{}", encoding="utf-8")
                report = {"bestScore": score, "status": "targetReached" if score > 0.8 else "stalled", "iterations": [{}, {}]}
                (work / "report.json").write_text(json.dumps(report), encoding="utf-8")
                return report

            result = run_benchmark(manifest, root / "report.json", root / "work", reconstruct)

            self.assertFalse(result["passed"])
            self.assertEqual(result["summary"]["passed"], 1)
            self.assertEqual(result["summary"]["failureModes"], {"stalled": 1})
            self.assertEqual(result["summary"]["meanScore"], 0.75)

    def test_rejects_duplicate_case_ids(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            manifest = root / "corpus.json"
            manifest.write_text(json.dumps({
                "schema": "shapeforge.reconstruction-corpus/1.0",
                "cases": [{"id": "same"}, {"id": "same"}],
            }), encoding="utf-8")
            with self.assertRaisesRegex(ValueError, "unique"):
                run_benchmark(manifest, root / "report.json", root / "work", lambda *args: {})


if __name__ == "__main__":
    unittest.main()
