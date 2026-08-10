using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ShapeForge.Unity.Editor
{
    /// <summary>
    /// Produces isolated transparent orthographic views for deterministic external image comparison.
    /// </summary>
    public static class UnityShapeReferenceRenderer
    {
        private const int CaptureLayer = 31;

        /// <summary>Renders every requested view and returns a portable image manifest.</summary>
        public static ShapeRenderCaptureManifest Render(
            GameObject                root,
            ShapeRenderCaptureRequest request,
            string                    outputFolder)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            ShapeDiagnosticReport report = new ShapeRenderCaptureRequestValidator().Analyze(request);
            if (!report.IsValid)
                throw new ShapeValidationException(report.Diagnostics[0].Message);
            if (string.IsNullOrWhiteSpace(outputFolder))
                throw new ArgumentException("A capture output folder is required.", nameof(outputFolder));

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(false);
            if (renderers.Length == 0)
                throw new ShapeValidationException("Reference rendering requires at least one active Renderer.");
            Directory.CreateDirectory(outputFolder);
            Bounds bounds = CombineBounds(renderers);
            List<LayerState> layers = SetLayer(root.transform, CaptureLayer);
            GameObject cameraObject = null;
            GameObject lightObject  = null;
            try
            {
                cameraObject = new("ShapeForge Capture Camera") { hideFlags = HideFlags.HideAndDontSave };
                lightObject  = new("ShapeForge Capture Light") { hideFlags = HideFlags.HideAndDontSave };
                Camera camera = ConfigureCamera(cameraObject);
                ConfigureLight(lightObject, cameraObject.transform);
                ShapeRenderCaptureManifest manifest = new()
                {
                    CaptureId  = request.Id,
                    CandidateId = request.CandidateId
                };
                foreach (ShapeRenderCaptureView view in request.Views)
                {
                    ConfigureView(camera, bounds, view, request.Width, request.Height);
                    string fileName = $"{Sanitize(view.Id)}.png";
                    string fullPath = Path.GetFullPath(Path.Combine(outputFolder, fileName));
                    RenderPng(camera, request.Width, request.Height, fullPath);
                    manifest.Images.Add(new() { ViewId = view.Id, ImagePath = fileName });
                }
                return manifest;
            }
            finally
            {
                RestoreLayers(layers);
                if (cameraObject != null)
                    UnityEngine.Object.DestroyImmediate(cameraObject);
                if (lightObject != null)
                    UnityEngine.Object.DestroyImmediate(lightObject);
            }
        }

        private static Bounds CombineBounds(Renderer[] renderers)
        {
            Bounds result = renderers[0].bounds;
            for (int index = 1; index < renderers.Length; index++)
                result.Encapsulate(renderers[index].bounds);
            return result;
        }

        private static Camera ConfigureCamera(GameObject owner)
        {
            Camera camera           = owner.AddComponent<Camera>();
            camera.clearFlags       = CameraClearFlags.SolidColor;
            camera.backgroundColor  = Color.clear;
            camera.orthographic     = true;
            camera.allowHDR         = false;
            camera.allowMSAA        = true;
            camera.cullingMask      = 1 << CaptureLayer;
            camera.nearClipPlane    = 0.01f;
            camera.farClipPlane     = 10000f;
            return camera;
        }

        private static void ConfigureLight(GameObject owner, Transform camera)
        {
            owner.transform.SetParent(camera, false);
            owner.transform.localRotation = Quaternion.Euler(35f, -30f, 0f);
            Light light        = owner.AddComponent<Light>();
            light.type         = LightType.Directional;
            light.intensity    = 1.15f;
            light.shadows      = LightShadows.None;
            light.cullingMask  = 1 << CaptureLayer;
        }

        private static void ConfigureView(
            Camera camera,
            Bounds bounds,
            ShapeRenderCaptureView view,
            int width,
            int height)
        {
            Quaternion orbit = Quaternion.Euler(view.Elevation, view.Azimuth, 0f);
            Vector3 offset   = orbit * Vector3.back;
            float distance   = Math.Max(bounds.extents.magnitude * 4f, 1f);
            camera.transform.position = bounds.center + offset * distance;
            camera.transform.LookAt(bounds.center, orbit * Vector3.up);

            Vector3 right = camera.transform.right;
            Vector3 up    = camera.transform.up;
            float halfWidth  = 0f;
            float halfHeight = 0f;
            foreach (Vector3 corner in Corners(bounds))
            {
                Vector3 relative = corner - bounds.center;
                halfWidth  = Math.Max(halfWidth, Math.Abs(Vector3.Dot(relative, right)));
                halfHeight = Math.Max(halfHeight, Math.Abs(Vector3.Dot(relative, up)));
            }
            float aspect = (float)width / height;
            camera.aspect           = aspect;
            camera.orthographicSize = Math.Max(halfHeight, halfWidth / aspect) * view.FramingScale;
        }

        private static IEnumerable<Vector3> Corners(Bounds bounds)
        {
            Vector3 center  = bounds.center;
            Vector3 extents = bounds.extents;
            for (int x = -1; x <= 1; x += 2)
            for (int y = -1; y <= 1; y += 2)
            for (int z = -1; z <= 1; z += 2)
                yield return center + Vector3.Scale(extents, new(x, y, z));
        }

        private static void RenderPng(Camera camera, int width, int height, string path)
        {
            RenderTexture previous = RenderTexture.active;
            RenderTexture target = RenderTexture.GetTemporary(
                width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 4);
            Texture2D image = new(width, height, TextureFormat.RGBA32, false);
            try
            {
                camera.targetTexture = target;
                RenderTexture.active = target;
                GL.Clear(true, true, Color.clear);
                camera.Render();
                image.ReadPixels(new(0, 0, width, height), 0, 0, false);
                image.Apply(false, false);
                File.WriteAllBytes(path, image.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = null;
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(target);
                UnityEngine.Object.DestroyImmediate(image);
            }
        }

        private static List<LayerState> SetLayer(Transform root, int layer)
        {
            List<LayerState> states = new();
            foreach (Transform target in root.GetComponentsInChildren<Transform>(true))
            {
                states.Add(new(target.gameObject, target.gameObject.layer));
                target.gameObject.layer = layer;
            }
            return states;
        }

        private static void RestoreLayers(IEnumerable<LayerState> states)
        {
            foreach (LayerState state in states)
            {
                if (state.Target != null)
                    state.Target.layer = state.Layer;
            }
        }

        private static string Sanitize(string value)
        {
            foreach (char invalid in Path.GetInvalidFileNameChars())
                value = value.Replace(invalid, '_');
            return value;
        }

        /// <summary>Stores one reversible temporary layer mutation.</summary>
        private readonly struct LayerState
        {
            public LayerState(GameObject target, int layer)
            {
                Target = target;
                Layer  = layer;
            }
            public GameObject Target { get; }
            public int Layer { get; }
        }
    }
}
