using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class MapSettings
    {
        [JsonPropertyName("selectedRealm")]
        public Realm Realm { get; set; }
        [JsonPropertyName("selectedMission")]
        public Level Level { get; set; }
        [JsonPropertyName("selectedDifficulty")]
        public Difficulty Difficulty { get; set; }
        [JsonPropertyName("selectedThreatLevel")]
        public Threat ThreatLevel { get; set; }

        [JsonPropertyName("panPosition")]
        public MapPosition VisualPanning { get; set; }
    }
}
