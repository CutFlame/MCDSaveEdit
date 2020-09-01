using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Mapping
{
    public class PrettyDateTimeJsonConverter : JsonConverter<DateTime>
    {
        /// <inheritdoc />
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string formattedDate = reader.GetString();
            return string.IsNullOrWhiteSpace(formattedDate) ? DateTime.MinValue : DateTime.ParseExact(formattedDate, "MMM d, yyyy", CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value == null || value == DateTime.MinValue)
            {
                writer.WriteStringValue("");
                return;
            }

            writer.WriteStringValue(value.ToString("MMM d, yyyy"));
        }
    }
}
