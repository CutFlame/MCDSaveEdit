using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class Currency
    {
        [JsonPropertyName("count")]
        public ulong Count { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
