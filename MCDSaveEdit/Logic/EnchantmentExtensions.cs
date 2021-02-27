using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCDSaveEdit.Save.Models.Enums
{
    public static class EnchantmentExtensions
    {
        public static HashSet<string> allEnchantments = new HashSet<string>();

        public static HashSet<string> powerful = new HashSet<string>() {
            "FinalShout",
            "Chilling",
            "Protection",
            "GravityPulse",
            
            "CriticalHit",
            "Exploding",
            "RadianceMelee",
            "GravityMelee",
            "Shockwave",
            "Swirling",

            "Gravity",
            "TempoTheft",
            "ChainReaction",
            "RadianceRanged",
        };

        public static HashSet<string> forArmor = new HashSet<string>() {
            "Swiftfooted",
            "PotionFortification",
            "Snowing",
            "SurpriseGift",
            "Burning",
            "Cowardice",
            "Deflecting",
            "Electrified",
            "Thorns",
            "Explorer",
            "Frenzied",
            "Celerity",
            "Recycler",
            "FoodReserves",
            "FireTrail",
            "HealthSynergy",
            "SpeedSynergy",
            "SpiritSpeed",
            
            "FinalShout",
            "Chilling",
            "Protection",
            "GravityPulse",
        };

        public static HashSet<string> forMelee = new HashSet<string>() {
            "Weakening",
            "FireAspect",
            "Looting",
            "Chains",
            "Echo",
            "Stunning",
            "Rampaging",
            "Freezing",
            "Committed",
            "PoisonedMelee",
            "Prospector",
            "EnigmaResonatorMelee",
            "SoulSiphon",
            "Thundering",
            "Sharpness",
            "Leeching",
            
            "CriticalHit",
            "Exploding",
            "RadianceMelee",
            "GravityMelee",
            "Shockwave",
            "Swirling",
        };

        public static HashSet<string> forRanged = new HashSet<string>() {
            "Accelerating",
            "Growing",
            "AnimaConduitRanged",
            "RapidFire",
            "Infinity",
            "Unchanting",
            "Piercing",
            "Power",
            "WildRage",
            "Punch",
            "Ricochet",
            "Supercharge",
            "FuseShot",
            "BonusShot",
            "FireAspect",
            "MultiShot",
            
            "Gravity",
            "TempoTheft",
            "ChainReaction",
            "RadianceRanged",
        };

        public static bool isPowerful(this Enchantment enchantment)
        {
            return powerful.Contains(enchantment.Id);
        }

        public static int pointsCost(this Enchantment enchantment)
        {
            int cost = 0;
            if (enchantment.isPowerful())
            {
                int level = Math.Min((int)enchantment.Level, GameCalculator.powerfulEnchantmentCostForLevel.Length);
                cost = GameCalculator.powerfulEnchantmentCostForLevel[level];
            }
            else
            {
                int level = Math.Min((int)enchantment.Level, GameCalculator.enchantmentCostForLevel.Length);
                cost = GameCalculator.enchantmentCostForLevel[level];
            }
            return cost;
        }

        public static int gildedPointsCost(this Enchantment enchantment)
        {
            int cost = 0;
            int level = 0;
            if (enchantment.isPowerful())
            {
                level = Math.Min((int)enchantment.Level, GameCalculator.powerfulEnchantmentCostForLevel.Length);
                cost = GameCalculator.powerfulEnchantmentCostForLevel[level];
            }
            else
            {
                level = Math.Min((int)enchantment.Level, GameCalculator.enchantmentCostForLevel.Length);
                cost = GameCalculator.enchantmentCostForLevel[level];
            }
            return cost + level;
        }
    }
}
