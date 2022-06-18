using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace MCDSaveEdit.Logic
{
    public static class ProfileExtensions
    {
        public static bool isValid(this ProfileSaveFile profile)
        {
            return profile.Items != null;
        }

        public static IEnumerable<Item> allItems(this ProfileSaveFile profile)
        {
            return (profile.Items ?? new Item[0]).Concat(profile.StorageChestItems ?? new Item[0]);
        }
        
        public static int level(this ProfileSaveFile profile)
        {
            return GameCalculator.levelForExperience(profile.Xp);
        }

        public static int remainingEnchantmentPoints(this ProfileSaveFile profile)
        {
            int totalEnchantmentPointsUsed = 1;
            foreach (var item in profile.allItems())
            {
                totalEnchantmentPointsUsed += item.enchantmentPoints();
            }
            return profile.level() - totalEnchantmentPointsUsed;
        }

        private static Item? equipmentSlot(this ProfileSaveFile profile, EquipmentSlotEnum equipmentSlot)
        {
            var equipmentSlotString = equipmentSlot.ToString();
            return profile.Items.FirstOrDefault(x => x.EquipmentSlot == equipmentSlotString);
        }
        public static int computeCharacterPower(this ProfileSaveFile profile)
        {
            var melee = profile.equipmentSlot(EquipmentSlotEnum.MeleeGear)?.Power ?? 0;
            var armor = profile.equipmentSlot(EquipmentSlotEnum.ArmorGear)?.Power ?? 0;
            var ranged = profile.equipmentSlot(EquipmentSlotEnum.RangedGear)?.Power ?? 0;
            var slot1 = profile.equipmentSlot(EquipmentSlotEnum.HotbarSlot1)?.Power ?? 0;
            var slot2 = profile.equipmentSlot(EquipmentSlotEnum.HotbarSlot2)?.Power ?? 0;
            var slot3 = profile.equipmentSlot(EquipmentSlotEnum.HotbarSlot3)?.Power ?? 0;
            var characterPower = GameCalculator.characterPowerFromEquippedItemPowers(melee, armor, ranged, slot1, slot2, slot3);
            var chacarterDisplayPower = GameCalculator.levelFromPower(characterPower);
            return chacarterDisplayPower;
        }
    }
}
