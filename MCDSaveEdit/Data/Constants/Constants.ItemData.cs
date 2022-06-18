using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System;
#nullable enable

namespace MCDSaveEdit.Data
{
    public static partial class Constants
	{
        public const string DEFAULT_ARMOR_PROPERTY_ID = "AllyDamageBoost";
        public const string DEFAULT_MELEE_WEAPON_ID = "Sword";
        public const string DEFAULT_ARMOR_ID = "ArchersStrappings";
        public const string DEFAULT_RANGED_WEAPON_ID = "Bow";
        public const string DEFAULT_ARTIFACT_ID = "FireworksArrowItem";

        public static string defaultItemIDForEquipmentSlot(EquipmentSlotEnum equipmentSlot)
        {
            return defaultItemIDForFilter(itemFilterForEquipmentSlot(equipmentSlot));
        }

        public static string defaultItemIDForFilter(ItemFilterEnum filter)
        {
            switch (filter)
            {
                case ItemFilterEnum.MeleeWeapons: return Constants.DEFAULT_MELEE_WEAPON_ID;
                case ItemFilterEnum.Armor: return Constants.DEFAULT_ARMOR_ID;
                case ItemFilterEnum.RangedWeapons: return Constants.DEFAULT_RANGED_WEAPON_ID;
                case ItemFilterEnum.Artifacts: return Constants.DEFAULT_ARTIFACT_ID;
            }
            throw new ArgumentException($"Invalid filter value {filter}", "filter");
        }

        public static ItemFilterEnum itemFilterForEquipmentSlot(EquipmentSlotEnum equipmentSlot)
        {
            switch (equipmentSlot)
            {
                case EquipmentSlotEnum.MeleeGear: return ItemFilterEnum.MeleeWeapons;
                case EquipmentSlotEnum.ArmorGear: return ItemFilterEnum.Armor;
                case EquipmentSlotEnum.RangedGear: return ItemFilterEnum.RangedWeapons;
                case EquipmentSlotEnum.HotbarSlot1:
                case EquipmentSlotEnum.HotbarSlot2:
                case EquipmentSlotEnum.HotbarSlot3:
                    return ItemFilterEnum.Artifacts;
            }
            throw new ArgumentException($"Invalid equipmentSlot value {equipmentSlot}", "equipmentSlot");
        }

        public static Item createDefaultItemForFilter(ItemFilterEnum filter)
        {
            var itemID = Constants.defaultItemIDForFilter(filter);
            return new Item() {
                MarkedNew = true,
                Upgraded = false,
                Power = 1,
                Rarity = Rarity.Common,
                Type = itemID,
            };
        }

        public static Item createDefaultItemForEquipmentSlot(EquipmentSlotEnum equipmentSlot)
        {
            var itemID = Constants.defaultItemIDForEquipmentSlot(equipmentSlot);
            return new Item() {
                MarkedNew = true,
                Upgraded = false,
                Power = 1,
                Rarity = Rarity.Common,
                Type = itemID,
                EquipmentSlot = equipmentSlot.ToString(),
            };
        }


    }
}
