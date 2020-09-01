using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Mapping;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class MapPosition
    {
        [JsonPropertyName("x")]
        [JsonConverter(typeof(TextDoubleJsonConverter))]
        public double X { get; set; } // Serialize as string
        [JsonPropertyName("y")]
        [JsonConverter(typeof(TextDoubleJsonConverter))]
        public double Y { get; set; } // Serialize as string
    }
}
