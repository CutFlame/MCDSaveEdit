using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class ArmorProperty
    {
        [JsonPropertyName("id")]
        public ArmorPropertyType Type { get; set; }
        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }
    }
}
