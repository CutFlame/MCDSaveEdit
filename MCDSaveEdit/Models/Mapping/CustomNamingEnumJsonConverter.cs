using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class CustomNamingEnumJsonConverter<TE, TC> : JsonConverter<TE> where TE : Enum where TC : INamingPolicy<TE>
    {
        private readonly TC _converter;

        public CustomNamingEnumJsonConverter()
        {
            _converter = Activator.CreateInstance<TC>();
        }

        /// <inheritdoc />
        public override TE Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _converter.ConvertName(reader.GetString());
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TE value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(_converter.ConvertValue(value));
        }
    }
}
