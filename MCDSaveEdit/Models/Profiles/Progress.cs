using MCDSaveEdit.Save.Models.Enums;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Progress
    {
        [JsonPropertyName("completedDifficulty")]
        public DifficultyEnum CompletedDifficulty { get; set; }

        [JsonPropertyName("completedEndlessStruggle")]
        public int CompletedEndlessStruggle { get; set; }

        [JsonPropertyName("completedThreatLevel")]
        public string CompletedThreatLevel { get; set; }
    }
}
