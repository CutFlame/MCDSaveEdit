using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Mapping;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class ProfileSaveFile
    {
        [JsonPropertyName("bonus_prerequisites")]
        public string[] BonusPrerequisites { get; set; }

        [JsonPropertyName("clone")]
        public bool Clone { get; set; }

        [JsonPropertyName("cosmetics")]
        public Cosmetic[] Cosmetics { get; set; }

        [JsonPropertyName("cosmeticsEverEquipped")]
        public string[] CosmeticsEverEquipped { get; set; }

        [JsonPropertyName("creationDate")]
        public string CreationDate { get; set; }

        [JsonPropertyName("currency")]
        public Currency[] Currency { get; set; }

        [JsonPropertyName("customized")]
        public bool Customized { get; set; }

        [JsonPropertyName("difficulties")]
        public Difficulties Difficulties { get; set; }

        //[JsonPropertyName("finishedObjectiveTags")]
        //public FinishedObjectiveTags FinishedObjectiveTags { get; set; }

        [JsonPropertyName("items")]
        public Item[] Items { get; set; }

        [JsonPropertyName("itemsFound")]
        public string[] ItemsFound { get; set; }

        [JsonPropertyName("lobbychest_progress")]
        public Dictionary<string, LobbychestProgress> LobbychestProgress { get; set; }

        //[JsonPropertyName("mapUIState")]
        //public MapUiState MapUiState { get; set; }

        [JsonPropertyName("mob_kills")]
        public Dictionary<string, long> MobKills { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        //[JsonPropertyName("pendingRewardItem")]
        //public object PendingRewardItem { get; set; }

        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }

        [JsonPropertyName("progress")]
        public Progress Progress { get; set; }

        [JsonPropertyName("progressionKeys")]
        public string[] ProgressionKeys { get; set; }

        [JsonPropertyName("skin")]
        public string Skin { get; set; }

        [JsonPropertyName("threatLevels")]
        public ThreatLevels ThreatLevels { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("totalGearPower")]
        public long TotalGearPower { get; set; }

        [JsonPropertyName("trialsCompleted")]
        public object[] TrialsCompleted { get; set; }

        //[JsonPropertyName("uiHintsExpired")]
        //public UiHintsExpired[] UiHintsExpired { get; set; }

        [JsonPropertyName("version")]
        public long Version { get; set; }

        [JsonPropertyName("xp")]
        public long Xp { get; set; }
    }
}
