using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;

namespace MCDSaveEdit.Save.Models.Enums
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
                points += enchantment.pointsCost();
            }
            return points;
        }

        public static bool isArtifact(this Item item)
        {
            return artifacts.Contains(item.Type.ToLowerInvariant());
        }

        public static bool isArmor(this Item item)
        {
            return armor.Contains(item.Type.ToLowerInvariant());
        }

        public static bool isMeleeWeapon(this Item item)
        {
            return meleeWeapons.Contains(item.Type.ToLowerInvariant());
        }

        public static bool isRangedWeapon(this Item item)
        {
            return rangedWeapons.Contains(item.Type.ToLowerInvariant());
        }

        public static HashSet<string> artifacts = new HashSet<string>();
        public static HashSet<string> armor = new HashSet<string>();
        public static HashSet<string> meleeWeapons = new HashSet<string>();
        public static HashSet<string> rangedWeapons = new HashSet<string>();
    }
}
