using System;

namespace MCDSaveEdit
{
    public static class GameCalculator
    {
        public static readonly int[] enchantmentCostForLevel = new int[] { 0, 1, 3, 6 };
        public static readonly int[] powerfulEnchantmentCostForLevel = new int[] { 0, 2, 5, 9 };

        public static int levelFromPower(double power)
        {
            return (int)Math.Floor((power - 1) * 10) + 1;
        }

        public static double powerFromLevel(int level)
        {
            return (((level - 1) / 10.0) + 1); // - 0.00000100; //not sure if this last part is needed
        }

        public static int levelForExperience(int xp)
        {
            double result = (1.0 / 30.0) * (Math.Sqrt(3 * xp + 100.0) + 20);
            return (int)Math.Floor(result);
        }

        public static int experienceForLevel(int level)
        {
            return 100 * (level - 1) * (3 * level - 1);
        }
    }
}
