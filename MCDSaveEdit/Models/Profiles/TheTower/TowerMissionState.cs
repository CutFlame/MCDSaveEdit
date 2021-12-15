using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class TowerMissionState
    {
        [JsonPropertyName("completedOnce")]
        public bool CompletedOnce { get; set; }

        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        [JsonPropertyName("livesLost")]
        public int LivesLost { get; set; }

        [JsonPropertyName("missionDifficulty")]
        public MissionDifficulty MissionDifficulty { get; set; }

        [JsonPropertyName("offeredEnchantmentPoints")]
        public int OfferedEnchantmentPoints { get; set; }

        [JsonPropertyName("offeredItems")]
        public object OfferedItems { get; set; }

        [JsonPropertyName("ownedDLCs")]
        public object OwnedDLCs { get; set; }

        [JsonPropertyName("partsDiscovered")]
        public int PartsDiscovered { get; set; }

        [JsonPropertyName("seed")]
        public int Seed { get; set; }

        [JsonPropertyName("towerInfo")]
        public TowerInfo TowerInfo { get; set; }

    }

}
