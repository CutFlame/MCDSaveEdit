using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class TowerFloor
    {
        [JsonPropertyName("challenges")]
        public string[]? Challenges { get; set; }

        [JsonPropertyName("rewards")]
        public string[] Rewards { get; set; }

        [JsonPropertyName("tile")]
        public string Tile { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

}
