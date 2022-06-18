using System.Text.Json.Serialization;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Cosmetic
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}
