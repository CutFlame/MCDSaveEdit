using System.Text.Json.Serialization;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class Currency
    {
        [JsonPropertyName("count")]
        public ulong Count { get; set; } = 0;
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}
