using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Armorproperty
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }
    }
}
