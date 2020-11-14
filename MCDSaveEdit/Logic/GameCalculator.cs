using System;

namespace MCDSaveEdit
{
    public static class GameCalculator
    {
        public static readonly int[] enchantmentCostForLevel = new int[] { 0, 1, 3, 6, 10, 15, 21 };
        public static readonly int[] powerfulEnchantmentCostForLevel = new int[] { 0, 2, 5, 9, 14, 20, 27 };

        public static int levelFromPower(double power)
        {
            return (int)Math.Floor((power - 1 + 0.00001) * 10) + 1;
        }

        public static double powerFromLevel(int level)
        {
            return (((level - 1) / 10.0) + 1) + 0.00001; //not sure if this last part is needed
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
    }
}
