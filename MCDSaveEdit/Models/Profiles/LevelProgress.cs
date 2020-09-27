using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Progress
    {
        [JsonPropertyName("cacticanyon")]
        public LevelProgress Cacticanyon { get; set; }

        [JsonPropertyName("creeperwoods")]
        public LevelProgress Creeperwoods { get; set; }

        [JsonPropertyName("deserttemple")]
        public LevelProgress Deserttemple { get; set; }

        [JsonPropertyName("fieryforge")]
        public LevelProgress Fieryforge { get; set; }

        [JsonPropertyName("highblockhalls")]
        public LevelProgress Highblockhalls { get; set; }

        [JsonPropertyName("mooncorecaverns")]
        public LevelProgress Mooncorecaverns { get; set; }

        [JsonPropertyName("obsidianpinnacle")]
        public LevelProgress Obsidianpinnacle { get; set; }

        [JsonPropertyName("squidcoast")]
        public LevelProgress Squidcoast { get; set; }
    }

    public partial class LevelProgress
    {
        [JsonPropertyName("completedDifficulty")]
        public string CompletedDifficulty { get; set; }

        [JsonPropertyName("completedThreatLevel")]
        public string CompletedThreatLevel { get; set; }
    }
}
