using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Enchantment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("level")]
        public long Level { get; set; }
    }
}
