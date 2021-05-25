using System;

namespace MCDSaveEdit
{
    public static class GameCalculator
    {
        public static int enchantmentCostForLevel(int level)
        {
            if (level == 0) return 0;
            return (level * (level + 1)) / 2;
        }

        public static int powerfulEnchantmentCostForLevel(int level)
        {
            if (level == 0) return 0;
            return (level * (level + 3)) / 2;
        }

        public static int levelFromPower(double power)
        {
            if (power <= 0) return 0;
            return (int)Math.Floor((Math.Max(1, power) - 1 + 0.00001) * 10) + 1;
        }

        public static double powerFromLevel(int level)
        {
            if (level <= 0) return 0;
            return ((Math.Max(1, level) - 1) / 10.0) + 1 + 0.00001; //not sure if this last part is needed
        }

        public static int levelForExperience(long xp)
        {
            double result = (1.0 / 30.0) * (Math.Sqrt(3 * xp + 100.0) + 20);
            return (int)Math.Floor(result);
        }

        public static long experienceForLevel(int level)
        {
            return 100 * (level - 1) * (3 * level - 1);
        }

        public static double characterPowerFromEquippedItemPowers(double melee, double armor, double ranged, double slot1, double slot2, double slot3)
        {
            var weightedAverage
                = (melee + armor + ranged) / 4
                + (slot1 + slot2 + slot3) / 12;
            return weightedAverage;
        }
    }
}
