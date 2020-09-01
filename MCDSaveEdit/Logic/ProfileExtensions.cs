using MCDSaveEdit.Save.Models.Enums;
using System.Collections.Generic;
using System.Linq;


namespace MCDSaveEdit.Save.Models.Profiles
{
    public static class ProfileExtensions
    {
        public static Item meleeGearItem(this ProfileSaveFile profile)
        {
            return profile.equipmentSlot(EquipmentSlotEnum.MeleeGear);
        }
        public static Item armorGearItem(this ProfileSaveFile profile)
        {
            return profile.equipmentSlot(EquipmentSlotEnum.ArmorGear);
        }
        public static Item rangedGearItem(this ProfileSaveFile profile)
        {
            return profile.equipmentSlot(EquipmentSlotEnum.RangedGear);
        }
        public static Item hotbarSlot1Item(this ProfileSaveFile profile)
        {
            return profile.equipmentSlot(EquipmentSlotEnum.HotbarSlot1);
        }
        public static Item hotbarSlot2Item(this ProfileSaveFile profile)
        {
            return profile.equipmentSlot(EquipmentSlotEnum.HotbarSlot2);
        }
        public static Item hotbarSlot3Item(this ProfileSaveFile profile)
        {
            return profile.equipmentSlot(EquipmentSlotEnum.HotbarSlot3);
        }
        public static Item equipmentSlot(this ProfileSaveFile profile, EquipmentSlotEnum equipmentSlot)
        {
            return profile.Inventory.FirstOrDefault(x => x.EquipmentSlot == equipmentSlot.ToString());
        }
        public static IEnumerable<Item> unequippedItems(this ProfileSaveFile profile)
        {
            return profile.Inventory.Where(x => x.EquipmentSlot == null);
        }
        public static IEnumerable<Item> equippedItems(this ProfileSaveFile profile)
        {
            return profile.Inventory.Where(x => x.EquipmentSlot != null);
        }
        public static int level(this ProfileSaveFile profile)
        {
            return GameCalculator.levelForExperience(profile.Experience);
        }
        public static int remainingEnchantmentPoints(this ProfileSaveFile profile)
        {
            int totalEnchantmentPointsUsed = 1;
            foreach(var item in profile.Inventory)
            {
                totalEnchantmentPointsUsed += item.enchantmentPoints();
            }
            return profile.level() - totalEnchantmentPointsUsed;
        }
    }
}
