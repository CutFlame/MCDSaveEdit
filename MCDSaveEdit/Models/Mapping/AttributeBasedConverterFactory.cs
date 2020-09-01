using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class AttributeBasedConverterFactory : JsonConverterFactory
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.GetCustomAttribute<JsonConverterAttribute>() != null;
        }

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            JsonConverterAttribute attribute = typeToConvert.GetCustomAttribute<JsonConverterAttribute>()!;
            return (Activator.CreateInstance(attribute.ConverterType) as JsonConverter)!;
        }
    }
}
