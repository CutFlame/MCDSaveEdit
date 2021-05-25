using MCDSaveEdit.Save.Models.Enums;
using System.Collections.Generic;
using System.Linq;


namespace MCDSaveEdit.Save.Models.Profiles
{
    public static class ProfileExtensions
    {
        public static bool isValid(this ProfileSaveFile profile)
        {
            return profile.Items != null;
        }
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
            return profile.Items.FirstOrDefault(x => x.EquipmentSlot == equipmentSlot.ToString());
        }
        public static IEnumerable<Item> unequippedItems(this ProfileSaveFile profile)
        {
            return profile.Items.Where(x => x.EquipmentSlot == null);
        }
        public static IEnumerable<Item> equippedItems(this ProfileSaveFile profile)
        {
            return profile.Items.Where(x => x.EquipmentSlot != null);
        }
        public static int level(this ProfileSaveFile profile)
        {
            return GameCalculator.levelForExperience(profile.Xp);
        }
        public static int computeCharacterPower(this ProfileSaveFile profile)
        {
            var melee = profile.meleeGearItem()?.Power ?? 0;
            var armor = profile.armorGearItem()?.Power ?? 0;
            var ranged = profile.rangedGearItem()?.Power ?? 0;
            var slot1 = profile.hotbarSlot1Item()?.Power ?? 0;
            var slot2 = profile.hotbarSlot2Item()?.Power ?? 0;
            var slot3 = profile.hotbarSlot3Item()?.Power ?? 0;
            var characterPower = GameCalculator.characterPowerFromEquippedItemPowers(melee, armor, ranged, slot1, slot2, slot3);
            var chacarterDisplayPower = GameCalculator.levelFromPower(characterPower);
            return chacarterDisplayPower;
        }
        public static int remainingEnchantmentPoints(this ProfileSaveFile profile)
        {
            int totalEnchantmentPointsUsed = 1;
            foreach(var item in profile.Items)
            {
                totalEnchantmentPointsUsed += item.enchantmentPoints();
            }
            return profile.level() - totalEnchantmentPointsUsed;
        }
    }
}
