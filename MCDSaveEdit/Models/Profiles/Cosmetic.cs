using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class Cosmetic
    {
        [JsonPropertyName("id")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public CosmeticType Type { get; set; }
    }
}
