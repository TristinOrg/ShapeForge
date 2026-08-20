using System;
using System.Collections.Generic;

namespace ShapeForge
{
    /// <summary>
    /// Stores deterministic image measurements before an asset-specific compiler interprets them.
    /// </summary>
    [Serializable]
    public sealed class ShapeReferenceBlueprint
    {
        /// <summary>Identifies the current reference-blueprint schema.</summary>
        public const string CurrentSchema = "shapeforge.reference-blueprint/1.0";
        /// <summary>Gets or sets the schema identifier.</summary>
        public string Schema { get; set; } = CurrentSchema;
        /// <summary>Gets or sets the stable blueprint identifier.</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>Gets or sets the source image path for auditing.</summary>
        public string SourceImage { get; set; } = string.Empty;
        /// <summary>Gets or sets the normalized coordinate convention.</summary>
        public string CoordinateSystem { get; set; } = "image-normalized/top-left";
        /// <summary>Gets or sets the detected layout profile used to extract evidence.</summary>
        public string LayoutProfile { get; set; } = "single-or-turntable";
        /// <summary>Gets or sets measured views.</summary>
        public IList<ShapeReferenceBlueprintView> Views { get; set; } = new List<ShapeReferenceBlueprintView>();
        /// <summary>Gets or sets cross-view scalar measurements.</summary>
        public IDictionary<string, float> Measurements { get; set; } = new Dictionary<string, float>();
        /// <summary>Gets or sets dominant measured colors.</summary>
        public IList<ShapeReferencePaletteSample> Palette { get; set; } = new List<ShapeReferencePaletteSample>();
        /// <summary>Gets or sets auditable source regions such as details, diagrams, and annotations.</summary>
        public IList<ShapeReferenceEvidenceRegion> EvidenceRegions { get; set; } = new List<ShapeReferenceEvidenceRegion>();
        /// <summary>Gets or sets transcribed or OCR-produced reference annotations.</summary>
        public IList<ShapeReferenceAnnotation> Annotations { get; set; } = new List<ShapeReferenceAnnotation>();
        /// <summary>Gets or sets the optional asset classification.</summary>
        public ShapeReferenceClassification Classification { get; set; } = new();
        /// <summary>Gets or sets facts that deterministic analysis could not resolve.</summary>
        public IList<ShapeReferenceReviewItem> ReviewQueue { get; set; } = new List<ShapeReferenceReviewItem>();
    }

    /// <summary>Stores one isolated reference view and its measured silhouette.</summary>
    [Serializable]
    public sealed class ShapeReferenceBlueprintView
    {
        /// <summary>Gets or sets the stable view identifier.</summary>
        public string ViewId { get; set; } = string.Empty;
        /// <summary>Gets or sets the isolated image path.</summary>
        public string ImagePath { get; set; } = string.Empty;
        /// <summary>Gets or sets the normalized foreground bounds.</summary>
        public ShapeReferenceBounds ForegroundBounds { get; set; } = new();
        /// <summary>Gets or sets the ordered normalized silhouette.</summary>
        public IList<ForgeVector2> Silhouette { get; set; } = new List<ForgeVector2>();
        /// <summary>Gets or sets deterministic measurement confidence.</summary>
        public float Confidence { get; set; }
    }

    /// <summary>Stores a normalized rectangular image region.</summary>
    [Serializable]
    public sealed class ShapeReferenceBounds
    {
        /// <summary>Gets or sets the left edge.</summary>
        public float X { get; set; }
        /// <summary>Gets or sets the top edge.</summary>
        public float Y { get; set; }
        /// <summary>Gets or sets the width.</summary>
        public float Width { get; set; }
        /// <summary>Gets or sets the height.</summary>
        public float Height { get; set; }
    }

    /// <summary>Stores one dominant color measurement.</summary>
    [Serializable]
    public sealed class ShapeReferencePaletteSample
    {
        /// <summary>Gets or sets the optional stable swatch identifier.</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>Gets or sets the optional printed color label.</summary>
        public string Label { get; set; } = string.Empty;
        /// <summary>Gets or sets an RGB hexadecimal color.</summary>
        public string Hex { get; set; } = string.Empty;
        /// <summary>Gets or sets normalized pixel coverage.</summary>
        public float Coverage { get; set; }
        /// <summary>Gets or sets how the color was obtained.</summary>
        public string Source { get; set; } = "pixel-cluster";
        /// <summary>Gets or sets measurement confidence.</summary>
        public float Confidence { get; set; } = 1f;
    }

    /// <summary>Stores text evidence with its source region and language.</summary>
    [Serializable]
    public sealed class ShapeReferenceAnnotation
    {
        /// <summary>Gets or sets the evidence-region identifier.</summary>
        public string RegionId { get; set; } = string.Empty;
        /// <summary>Gets or sets transcribed text.</summary>
        public string Text { get; set; } = string.Empty;
        /// <summary>Gets or sets an optional BCP-47 language tag.</summary>
        public string Language { get; set; } = string.Empty;
    }

    /// <summary>Stores an auditable crop containing geometry, detail, color, measurement, or text evidence.</summary>
    [Serializable]
    public sealed class ShapeReferenceEvidenceRegion
    {
        /// <summary>Gets or sets the stable region identifier.</summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>Gets or sets the evidence kind.</summary>
        public string Kind { get; set; } = string.Empty;
        /// <summary>Gets or sets the isolated crop path.</summary>
        public string ImagePath { get; set; } = string.Empty;
        /// <summary>Gets or sets the region bounds in the original image.</summary>
        public ShapeReferenceBounds Bounds { get; set; } = new();
        /// <summary>Gets or sets extraction confidence.</summary>
        public float Confidence { get; set; }
    }

    /// <summary>Stores an optional category proposed outside deterministic measurement.</summary>
    [Serializable]
    public sealed class ShapeReferenceClassification
    {
        /// <summary>Gets or sets a category or unresolved.</summary>
        public string Category { get; set; } = "unresolved";
        /// <summary>Gets or sets classification confidence.</summary>
        public float Confidence { get; set; }
    }

    /// <summary>Describes one ambiguity requiring rules, metadata, or limited human/AI review.</summary>
    [Serializable]
    public sealed class ShapeReferenceReviewItem
    {
        /// <summary>Gets or sets the stable ambiguity kind.</summary>
        public string Kind { get; set; } = string.Empty;
        /// <summary>Gets or sets the reason deterministic analysis stopped.</summary>
        public string Reason { get; set; } = string.Empty;
        /// <summary>Gets or sets whether compilation must resolve this item.</summary>
        public bool Required { get; set; }
    }
}
