using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class Enchantment
    {
        [JsonPropertyName("id")]
        public EnchantmentType Type { get; set; }
        [JsonPropertyName("level")]
        public int Level { get; set; } // Min: 0; Max: 3
    }
}
