using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ShapeForge.Unity
{
    /// <summary>
    /// Provides the reference JSON implementation for ShapeForge documents.
    /// </summary>
    public sealed class ShapeJsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver  = new CamelCasePropertyNamesContractResolver(),
            Formatting        = Formatting.None,
            TypeNameHandling  = TypeNameHandling.None,
            Converters        = { new StringEnumConverter(new CamelCaseNamingStrategy()) }
        };
        private readonly ShapeDefinitionValidator      shapeValidator = new ShapeDefinitionValidator();
        private readonly ShapeStyleDefinitionValidator styleValidator = new ShapeStyleDefinitionValidator();

        /// <summary>
        /// Serializes a shape definition using the versioned ShapeForge JSON contract.
        /// </summary>
        public string Serialize(ShapeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return JsonConvert.SerializeObject(definition, Settings);
        }

        /// <summary>
        /// Deserializes a versioned ShapeForge shape document.
        /// </summary>
        public ShapeDefinition DeserializeShape(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("Shape JSON cannot be empty.", nameof(json));

            ShapeDefinition definition = JsonConvert.DeserializeObject<ShapeDefinition>(json, Settings) ??
                                         throw new JsonSerializationException("Shape JSON produced no definition.");
            shapeValidator.Validate(definition);
            return definition;
        }

        /// <summary>
        /// Serializes a style definition using the versioned ShapeForge JSON contract.
        /// </summary>
        public string Serialize(ShapeStyleDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return JsonConvert.SerializeObject(definition, Settings);
        }

        /// <summary>
        /// Serializes a versioned shape-capability catalog for external authoring tools.
        /// </summary>
        public string Serialize(ShapeCapabilityCatalogDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return JsonConvert.SerializeObject(document, Settings);
        }

        /// <summary>
        /// Serializes versioned semantic-template discovery data for external authoring tools.
        /// </summary>
        public string Serialize(ShapeTemplateCatalogDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return JsonConvert.SerializeObject(document, Settings);
        }

        /// <summary>
        /// Deserializes a versioned ShapeForge style document.
        /// </summary>
        public ShapeStyleDefinition DeserializeStyle(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("Style JSON cannot be empty.", nameof(json));

            ShapeStyleDefinition definition = JsonConvert.DeserializeObject<ShapeStyleDefinition>(json, Settings) ??
                                              throw new JsonSerializationException("Style JSON produced no definition.");
            styleValidator.Validate(definition);
            return definition;
        }
    }
}
