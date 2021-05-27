using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        [JsonPropertyName("currenciesFound")]
        public string[] CurrenciesFound { get; set; }

        [JsonPropertyName("currency")]
        public Currency[] Currency { get; set; }

        [JsonPropertyName("customized")]
        public bool Customized { get; set; }

        [JsonPropertyName("difficulties")]
        public Difficulties Difficulties { get; set; }

        [JsonPropertyName("endGameContentProgress")]
        public object EndGameContentProgress { get; set; }

        [JsonPropertyName("finishedObjectiveTags")]
        public Dictionary<string, long> FinishedObjectiveTags { get; set; }

        [JsonPropertyName("items")]
        public Item[] Items { get; set; }

        [JsonPropertyName("itemsFound")]
        public string[] ItemsFound { get; set; }

        [JsonPropertyName("legendaryStatus")]
        public double? LegendaryStatus { get; set; }

        [JsonPropertyName("lobbychest_progress")]
        public Dictionary<string, LobbychestProgress> LobbychestProgress { get; set; }

        [JsonPropertyName("mapUIState")]
        public object MapUiState { get; set; }
        //public MapUiState MapUiState { get; set; }

        [JsonPropertyName("merchantData")]
        public Dictionary<string, MerchantDef> MerchantData { get; set; }

        [JsonPropertyName("missionStatesMap")]
        public object MissionStatesMap { get; set; }

        [JsonPropertyName("mob_kills")]
        public Dictionary<string, long> MobKills { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pendingRewardItem")]
        public object PendingRewardItem { get; set; }
        
        [JsonPropertyName("pendingRewardItems")]
        public object PendingRewardItems { get; set; }

        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }

        [JsonPropertyName("progress")]
        public Dictionary<string, Progress> Progress { get; set; }

        [JsonPropertyName("progressStatCounters")]
        public Dictionary<string, long> ProgressStatCounters { get; set; }

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

        [JsonPropertyName("uiHintsExpired")]
        public object[] UiHintsExpired { get; set; }
        //public UiHintsExpired[] UiHintsExpired { get; set; }

        [JsonPropertyName("version")]
        public long Version { get; set; }

        [JsonPropertyName("xp")]
        public long Xp { get; set; }
    }
}
