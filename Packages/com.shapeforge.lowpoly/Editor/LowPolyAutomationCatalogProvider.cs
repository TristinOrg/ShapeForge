using ShapeForge.Unity.Editor;
using UnityEditor;

namespace ShapeForge.LowPoly.Editor
{
    /// <summary>
    /// Publishes Low Poly discovery documents through the style-neutral Unity automation registry.
    /// </summary>
    [InitializeOnLoad]
    public sealed class LowPolyAutomationCatalogProvider : IShapeAutomationCatalogProvider
    {
        static LowPolyAutomationCatalogProvider()
        {
            ShapeAutomationCatalogRegistry.Register("lowpoly/official", new LowPolyAutomationCatalogProvider());
        }

        /// <inheritdoc />
        public ShapeCapabilityCatalogDocument CreateCapabilities() =>
            LowPolyShapeCapabilityCatalog.Instance.CreateDocument();

        /// <inheritdoc />
        public ShapeTemplateCatalogDocument CreateTemplates() =>
            LowPolyShapeTemplateCatalog.Instance.CreateDocument("lowpoly/official");
    }
}
