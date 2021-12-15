using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class TowerInfo
    {
        [JsonPropertyName("towerArrowsAmount")]
        public int ArrowsAmount { get; set; }

        [JsonPropertyName("towerConfig")]
        public TowerConfig Config { get; set; }

        [JsonPropertyName("towerCurrentFloorWasCompleted")]
        public bool CurrentFloorWasCompleted { get; set; }

        [JsonPropertyName("towerEnchantmentPointsGranted")]
        public int EnchantmentPointsGranted { get; set; }

        [JsonPropertyName("towerFinalRewards")]
        public object FinalRewards { get; set; }

        [JsonPropertyName("towerInfo")]
        public NestedTowerInfo Info { get; set; }

        [JsonPropertyName("towerItems")]
        public Item[] Items { get; set; }

        [JsonPropertyName("towerWasCompleted")]
        public bool WasCompleted { get; set; }
    }

}
