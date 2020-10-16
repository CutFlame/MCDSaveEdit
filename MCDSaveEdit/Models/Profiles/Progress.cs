using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Progress
    {
        [JsonPropertyName("completedDifficulty")]
        public string CompletedDifficulty { get; set; }

        [JsonPropertyName("completedThreatLevel")]
        public string CompletedThreatLevel { get; set; }
    }
}
