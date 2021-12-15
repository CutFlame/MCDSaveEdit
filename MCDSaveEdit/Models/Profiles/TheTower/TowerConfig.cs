using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class TowerConfig
    {
        [JsonPropertyName("floors")]
        public TowerFloor[] Floors { get; set; }

        [JsonPropertyName("seed")]
        public int Seed { get; set; }
    }

}
