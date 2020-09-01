using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class Threats
    {
        [JsonPropertyName("unlocked")]
        public Threat Unlocked { get; set; }
    }
}
