using MCDSaveEdit.Save.Models.Profiles;
using System.Collections.Generic;

namespace MCDSaveEdit.Save.Models.Enums
{
    public static class EnchantmentExtensions
    {
        private static HashSet<EnchantmentType> powerful = new HashSet<EnchantmentType>() {
            EnchantmentType.FinalShout,
            EnchantmentType.Chilling,
            EnchantmentType.Protection,
            EnchantmentType.GravityPulse,

            EnchantmentType.CriticalHit,
            EnchantmentType.Exploding,
            EnchantmentType.RadianceMelee,
            EnchantmentType.GravityMelee,
            EnchantmentType.Shockwave,
            EnchantmentType.Swirling,

            EnchantmentType.Gravity,
            EnchantmentType.TempoTheft,
            EnchantmentType.ChainReaction,
            EnchantmentType.RadianceRanged,
        };

        public static HashSet<EnchantmentType> forArmor = new HashSet<EnchantmentType>() {
            EnchantmentType.Swiftfooted,
            EnchantmentType.PotionFortification,
            EnchantmentType.Snowing,
            EnchantmentType.SurpriseGift,
            EnchantmentType.Burning,
            EnchantmentType.Cowardice,
            EnchantmentType.Deflecting,
            EnchantmentType.Electrified,
            EnchantmentType.Thorns,
            EnchantmentType.Explorer,
            EnchantmentType.Frenzied,
            EnchantmentType.Celerity,
            EnchantmentType.Recycler,
            EnchantmentType.FoodReserves,
            EnchantmentType.FireTrail,
            EnchantmentType.HealthSynergy,
            EnchantmentType.SpeedSynergy,
            EnchantmentType.SpiritSpeed,

            EnchantmentType.FinalShout,
            EnchantmentType.Chilling,
            EnchantmentType.Protection,
            EnchantmentType.GravityPulse,
        };

        public static HashSet<EnchantmentType> forMelee = new HashSet<EnchantmentType>() {
            EnchantmentType.Weakening,
            EnchantmentType.FireAspect,
            EnchantmentType.Looting,
            EnchantmentType.Chains,
            EnchantmentType.Echo,
            EnchantmentType.Stunning,
            EnchantmentType.Rampaging,
            EnchantmentType.Freezing,
            EnchantmentType.Committed,
            EnchantmentType.PoisonedMelee,
            EnchantmentType.Prospector,
            EnchantmentType.EnigmaResonatorMelee,
            EnchantmentType.SoulSiphon,
            EnchantmentType.Thundering,
            EnchantmentType.Sharpness,
            EnchantmentType.Leeching,

            EnchantmentType.CriticalHit,
            EnchantmentType.Exploding,
            EnchantmentType.RadianceMelee,
            EnchantmentType.GravityMelee,
            EnchantmentType.Shockwave,
            EnchantmentType.Swirling,
        };

        public static HashSet<EnchantmentType> forRanged = new HashSet<EnchantmentType>() {
            EnchantmentType.Accelerating,
            EnchantmentType.Growing,
            EnchantmentType.AnimaConduitRanged,
            EnchantmentType.RapidFire,
            EnchantmentType.Infinity,
            EnchantmentType.Unchanting,
            EnchantmentType.Piercing,
            EnchantmentType.Power,
            EnchantmentType.WildRage,
            EnchantmentType.Punch,
            EnchantmentType.Ricochet,
            EnchantmentType.Supercharge,
            EnchantmentType.FuseShot,
            EnchantmentType.BonusShot,
            EnchantmentType.FireAspect,
            EnchantmentType.MultiShot,

            EnchantmentType.Gravity,
            EnchantmentType.TempoTheft,
            EnchantmentType.ChainReaction,
            EnchantmentType.RadianceRanged,
        };

        public static bool isPowerful(this EnchantmentType enchantmentType)
        {
            return powerful.Contains(enchantmentType);
        }

        public static int pointsCost(this Enchantment enchantment)
        {
            if (enchantment.Type.isPowerful())
            {
                return GameCalculator.powerfulEnchantmentCostForLevel[enchantment.Level];
            }
            else
            {
                return GameCalculator.enchantmentCostForLevel[enchantment.Level];
            }
        }
    }
}
