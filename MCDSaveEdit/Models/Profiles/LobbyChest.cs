using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class LobbyChest
    {
        [JsonPropertyName("unlockedTimes")]
        public int TimesUnlocked { get; set; }
    }
}
