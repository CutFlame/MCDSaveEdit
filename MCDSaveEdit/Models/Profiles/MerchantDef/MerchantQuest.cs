using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class MerchantQuest
    {
        [JsonPropertyName("dynamicQuestState")]
        public object DynamicQuestState { get; set; }

        [JsonPropertyName("questState")]
        public object QuestState { get; set; }

    }
}
