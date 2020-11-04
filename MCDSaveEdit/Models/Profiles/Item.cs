using System.Collections.Generic;
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

        [JsonPropertyName("inventoryIndex")]
        public long? InventoryIndex { get; set; }

        [JsonPropertyName("power")]
        public double Power { get; set; }

        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("upgraded")]
        public bool Upgraded { get; set; }

        [JsonPropertyName("markedNew")]
        public bool? MarkedNew { get; set; }
    }
}
