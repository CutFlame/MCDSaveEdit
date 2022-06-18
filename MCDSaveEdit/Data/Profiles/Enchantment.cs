using System;
using System.Text.Json.Serialization;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Enchantment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("investedPoints")]
        public long? InvestedPoints { get; set; } = null;

        [JsonPropertyName("level")]
        public long Level { get; set; } = 0;
    }

    public partial class Enchantment : ICloneable
    {
        public object Clone()
        {
            return Copy();
        }

        public Enchantment Copy()
        {
            var copy = new Enchantment();
            copy.Id = this.Id;
            copy.Level = this.Level;
            return copy;
        }
    }
}
