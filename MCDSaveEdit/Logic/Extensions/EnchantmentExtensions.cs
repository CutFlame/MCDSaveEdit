using MCDSaveEdit.Data;
using MCDSaveEdit.Save.Models.Profiles;

namespace MCDSaveEdit.Logic
{
    public static class EnchantmentExtensions
    {
        public static bool isPowerful(this Enchantment enchantment)
        {
            return Constants.powerful.Contains(enchantment.Id);
        }

        public static int pointsCost(this Enchantment enchantment)
        {
            int level = (int)enchantment.Level;
            int cost;
            if (enchantment.isPowerful())
            {
                cost = GameCalculator.powerfulEnchantmentCostForLevel(level);
            }
            else
            {
                cost = GameCalculator.enchantmentCostForLevel(level);
            }
            return cost;
        }

        public static int gildedPointsCost(this Enchantment enchantment)
        {
            int level = (int)enchantment.Level;
            int cost = pointsCost(enchantment);
            return cost + level;
        }
    }
}
