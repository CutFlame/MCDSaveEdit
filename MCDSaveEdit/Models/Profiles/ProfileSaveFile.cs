using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Mapping;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class ProfileSaveFile
    {
        [JsonPropertyName("version")]
        public int Version { get; set; }
        [JsonPropertyName("timestamp")]
        [JsonConverter(typeof(EpochDateTimeJsonConverter))]
        public DateTime TimeStamp { get; set; } // Format as Unix timestamp
        [JsonPropertyName("creationDate")]
        [JsonConverter(typeof(PrettyDateTimeJsonConverter))]
        public DateTime CreationDate { get; set; } // Format as "MMM d, yyyy", serialize null as empty string

        [JsonPropertyName("playerId")]
        public Guid PlayerGuid { get; set; }
        [JsonPropertyName("name")]
        public string ProfileName { get; set; } // Can be empty string
        [JsonPropertyName("skin")]
        public Skin Skin { get; set; }

        [JsonPropertyName("clone")]
        public bool IsCloned { get; set; }
        [JsonPropertyName("customized")]
        public bool IsCustomized { get; set; }

        [JsonPropertyName("totalGearPower")]
        public int GearPower { get; set; }
        [JsonPropertyName("xp")]
        public int Experience { get; set; }
        [JsonPropertyName("currency")]
        public IEnumerable<Currency> Currencies { get; set; } // Can be empty

        [JsonPropertyName("difficulties")]
        public Difficulties? Difficulties { get; set; }
        [JsonPropertyName("threatLevels")]
        public Threats? ThreatLevels { get; set; }


        [JsonPropertyName("progressionKeys")]
        public IEnumerable<string> Milestones { get; set; } // Can be empty
        [JsonPropertyName("finishedObjectiveTags")]
        public IDictionary<string, int>? CompletedObjectives { get; set; }
        [JsonPropertyName("progress")]
        public IDictionary<string, LevelProgress>? CompletedLevels { get; set; }
        [JsonPropertyName("bonus_prerequisites")]
        public IEnumerable<Level> BonusLevels { get; set; } // Can be empty
        [JsonPropertyName("trialsCompleted")]
        public IEnumerable CompletedTrials { get; set; } // TODO: Data structure unavailable in-game.

        [JsonPropertyName("cosmetics")]
        public IEnumerable<Cosmetic> Cosmetics { get; set; } // Can be empty
        [JsonPropertyName("cosmeticsEverEquipped")]
        public IEnumerable<string> CosmeticsHistory { get; set; } // Can be empty
        [JsonPropertyName("pendingRewardItem")]
        public Reward? PendingReward { get; set; }

        [JsonPropertyName("items")]
        public IEnumerable<Item> Inventory { get; set; } // Can be empty
        [JsonPropertyName("itemsFound")]
        public IEnumerable<string> ItemsHistory { get; set; } // Can be empty

        [JsonPropertyName("lobbychest_progress")]
        public IDictionary<string, LobbyChest>? LobbyChest { get; set; }

        [JsonPropertyName("mob_kills")]
        public IDictionary<string, int>? MobKills { get; set; }
        [JsonPropertyName("mapUIState")]
        public MapSettings? MapSettings { get; set; }
        [JsonPropertyName("uiHintsExpired")]
        public IEnumerable<Hint> HintsShown { get; set; } // Can be empty
    }
}
