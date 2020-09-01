using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class TextDoubleJsonConverter : JsonConverter<double>
    {
        /// <inheritdoc />
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
