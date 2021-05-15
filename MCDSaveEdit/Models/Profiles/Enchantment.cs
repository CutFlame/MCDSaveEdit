using System;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Enchantment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("level")]
        public long Level { get; set; }
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
