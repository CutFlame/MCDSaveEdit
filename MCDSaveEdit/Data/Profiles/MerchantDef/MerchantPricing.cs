using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class MerchantPricing
    {
        [JsonPropertyName("timesRestocked")]
        public long TimesRestocked { get; set; }

    }
}
