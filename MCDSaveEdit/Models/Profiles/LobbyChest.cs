using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class LobbychestProgress
    {
        [JsonPropertyName("unlockedTimes")]
        public long UnlockedTimes { get; set; }
    }
}
