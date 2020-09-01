using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class LevelProgress
    {
        [JsonPropertyName("completedDifficulty")]
        public Difficulty Difficulty { get; set; }
        [JsonPropertyName("completedThreatLevel")]
        public Threat ThreatLevel { get; set; }
    }
}
