using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;

namespace MCDSaveEdit.Logic
{
    public static class ItemExtensions
    {
        public static int level(this Item item)
        {
            return GameCalculator.levelFromPower(item.Power);
        }

        public static int enchantmentPoints(this Item item)
        {
            if (item.Enchantments == null) { return 0; }
            int points = 0;
            foreach (var enchantment in item.Enchantments)
            {
                if(item.NetheriteEnchant != null)
                {
                    points += enchantment.gildedPointsCost();
                }
                else
                {
                    points += enchantment.pointsCost();
                }
            }
            return points;
        }

        public static bool isArtifact(this Item item)
        {
            return ItemDatabase.artifacts.Contains(item.Type);
        }

        public static bool isArmor(this Item item)
        {
            return ItemDatabase.armor.Contains(item.Type);
        }

        public static bool isMeleeWeapon(this Item item)
        {
            return ItemDatabase.meleeWeapons.Contains(item.Type);
        }

        public static bool isRangedWeapon(this Item item)
        {
            return ItemDatabase.rangedWeapons.Contains(item.Type);
        }

        public static bool isHotbarItem(this Item item)
        {
            var equipmentSlot = item.EquipmentSlot;
            return equipmentSlot == EquipmentSlotEnum.HotbarSlot1.ToString()
                || equipmentSlot == EquipmentSlotEnum.HotbarSlot2.ToString()
                || equipmentSlot == EquipmentSlotEnum.HotbarSlot3.ToString();
        }

        public static bool isGearItem(this Item item)
        {
            var equipmentSlot = item.EquipmentSlot;
            return equipmentSlot == EquipmentSlotEnum.MeleeGear.ToString()
                || equipmentSlot == EquipmentSlotEnum.ArmorGear.ToString()
                || equipmentSlot == EquipmentSlotEnum.RangedGear.ToString();
        }


    }
}
