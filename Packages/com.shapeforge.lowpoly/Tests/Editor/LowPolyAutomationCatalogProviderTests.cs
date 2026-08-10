using NUnit.Framework;
using ShapeForge.Unity.Editor;

namespace ShapeForge.LowPoly.Tests
{
    /// <summary>
    /// Verifies style-neutral agent discovery includes registered Low Poly catalogs.
    /// </summary>
    public sealed class LowPolyAutomationCatalogProviderTests
    {
        [Test]
        public void ProviderPublishesCapabilitiesAndTemplates()
        {
            ShapeForge.LowPoly.Editor.LowPolyAutomationCatalogProvider provider = new();

            Assert.That(provider.CreateCapabilities().Shapes, Is.Not.Empty);
            Assert.That(provider.CreateTemplates().Templates, Has.Count.GreaterThanOrEqualTo(6));
            Assert.That(ShapeAutomationCatalogRegistry.GetProviders(), Has.Count.GreaterThanOrEqualTo(2));
        }
    }
}
