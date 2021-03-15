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

    public class MerchantPricing
    {
        [JsonPropertyName("timesRestocked")]
        public long TimesRestocked { get; set; }

    }

    public class MerchantQuest
    {
        [JsonPropertyName("dynamicQuestState")]
        public object DynamicQuestState { get; set; }

        [JsonPropertyName("questState")]
        public object QuestState { get; set; }

    }

    public class MerchantSlot
    {
        [JsonPropertyName("item")]
        public Item Item { get; set; }

        [JsonPropertyName("priceMultiplier")]
        public double PriceMultiplier { get; set; }

        [JsonPropertyName("rebateFraction")]
        public double RebateFraction { get; set; }

        [JsonPropertyName("reserved")]
        public bool Reserved { get; set; }

    }
}
