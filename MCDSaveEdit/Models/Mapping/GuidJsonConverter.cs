using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class GuidJsonConverter : JsonConverter<Guid>
    {
        /// <inheritdoc />
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Guid.Parse(reader.GetString());
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("N").ToUpper());
        }
    }
}
