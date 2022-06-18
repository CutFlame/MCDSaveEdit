using MCDSaveEdit.Save.Models.Enums;
using System.Text.Json.Serialization;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Difficulties
    {
        [JsonPropertyName("announced")]
        public DifficultyEnum? Announced { get; set; } = null;

        [JsonPropertyName("selected")]
        public DifficultyEnum? Selected { get; set; } = null;

        [JsonPropertyName("unlocked")]
        public DifficultyEnum Unlocked { get; set; } = DifficultyEnum.Difficulty_1;
    }
}
