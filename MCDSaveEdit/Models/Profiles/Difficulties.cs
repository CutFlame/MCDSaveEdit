using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Difficulties
    {
        [JsonPropertyName("announced")]
        public string Announced { get; set; }

        [JsonPropertyName("selected")]
        public string Selected { get; set; }

        [JsonPropertyName("unlocked")]
        public string Unlocked { get; set; }
    }
}
