using System.Text.Json.Serialization;

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
