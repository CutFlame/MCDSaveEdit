using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class TowerFloorTypeObject
    {
        [JsonPropertyName("towerFloorType")]
        public TowerFloorTypeEnum FloorType { get; set; }

    }

}
