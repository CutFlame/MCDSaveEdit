using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class MerchantDef
    {
        [JsonPropertyName("everInteracted")]
        public bool EverInteracted { get; set; }

        [JsonPropertyName("pricing")]
        public MerchantPricing Pricing { get; set; }

        [JsonPropertyName("quests")]
        public Dictionary<string, MerchantQuest> Quests { get; set; }

        [JsonPropertyName("slots")]
        public Dictionary<string, MerchantSlot> Slots { get; set; }
    }
}
