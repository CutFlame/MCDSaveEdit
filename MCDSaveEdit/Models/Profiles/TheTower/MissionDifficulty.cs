using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class MissionDifficulty
    {
        [JsonPropertyName("difficulty")]
        public DifficultyEnum Difficulty { get; set; }

        [JsonPropertyName("endlessStruggle")]
        public int endlessStruggle { get; set; }

        [JsonPropertyName("mission")]
        public string mission { get; set; }

        [JsonPropertyName("threatLevel")]
        public ThreatLevelEnum threatLevel { get; set; }
    }
}
