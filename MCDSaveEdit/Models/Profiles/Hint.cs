using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class Hint
    {
        [JsonPropertyName("hintType")]
        public HintType Type { get; set; }
    }
}
