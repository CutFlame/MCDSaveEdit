using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Item
    {
        [JsonPropertyName("armorproperties")]
        public Armorproperty[] Armorproperties { get; set; }

        [JsonPropertyName("enchantments")]
        public Enchantment[] Enchantments { get; set; }

        [JsonPropertyName("equipmentSlot")]
        public string EquipmentSlot { get; set; }

        [JsonPropertyName("gifted")]
        public bool? Gifted { get; set; }

        [JsonPropertyName("inventoryIndex")]
        public long? InventoryIndex { get; set; }

        [JsonPropertyName("markedNew")]
        public bool? MarkedNew { get; set; }

        [JsonPropertyName("netheriteEnchant")]
        public Enchantment NetheriteEnchant { get; set; }

        [JsonPropertyName("power")]
        public double Power { get; set; }

        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("upgraded")]
        public bool Upgraded { get; set; }
    }

    public partial class Item : ICloneable
    {
        public object Clone()
        {
            return Copy();
        }

        public Item Copy()
        {
            var copy = new Item();
            //NOTE: deliberately skipping EquipmentSlot and InventoryIndex
            copy.Armorproperties = this.Armorproperties?.deepClone().ToArray();
            copy.Enchantments = this.Enchantments?.deepClone().ToArray();
            copy.Gifted = this.Gifted;
            copy.MarkedNew = this.MarkedNew;
            copy.NetheriteEnchant = this.NetheriteEnchant?.Copy();
            copy.Power = this.Power;
            copy.Rarity = this.Rarity;
            copy.Type = this.Type;
            copy.Upgraded = this.Upgraded;
            return copy;
        }
    }
}
