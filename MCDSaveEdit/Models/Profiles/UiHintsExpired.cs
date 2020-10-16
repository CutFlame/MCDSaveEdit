using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class UiHintsExpired
    {
        [JsonPropertyName("hintType")]
        public string HintType { get; set; }
    }
}
