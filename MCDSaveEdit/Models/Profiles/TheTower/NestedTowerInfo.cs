using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class NestedTowerInfo
    {
        [JsonPropertyName("towerInfoBossesKilled")]
        public int BossesKilled { get; set; }

        [JsonPropertyName("towerInfoCurrentFloor")]
        public int CurrentFloor { get; set; }

        [JsonPropertyName("towerInfoFloors")]
        public TowerFloorTypeObject[] Floors { get; set; }
    }

}
