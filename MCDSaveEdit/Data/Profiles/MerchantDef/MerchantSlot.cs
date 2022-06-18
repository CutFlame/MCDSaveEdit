using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
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
