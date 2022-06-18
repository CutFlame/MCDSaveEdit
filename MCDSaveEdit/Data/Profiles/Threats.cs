using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class ThreatLevels
    {
        [JsonPropertyName("unlocked")]
        public string? Unlocked { get; set; } = null;
    }
}
