using System.Collections.Generic;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class Item
    {
        [JsonPropertyName("type")]
        public string Name { get; set; } // Should be Enum but possible values are mixed with other game assets
        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }

        [JsonPropertyName("power")]
        public double Power { get; set; }
        [JsonPropertyName("upgraded")]
        public bool IsUpgraded { get; set; }

        [JsonPropertyName("equipmentSlot")]
        public string? EquipmentSlot { get; set; }
        [JsonPropertyName("inventoryIndex")]
        public int? InventorySlot { get; set; }

        [JsonPropertyName("enchantments")]
        public IEnumerable<Enchantment>? Enchantments { get; set; }
        [JsonPropertyName("armorproperties")]
        public IEnumerable<ArmorProperty>? ArmorProperties { get; set; }

        [JsonPropertyName("markedNew")]
        public bool? IsNew { get; set; }
    }
}
