using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MCDSaveEdit
{
    internal class R : Properties.Resources
    {
        private static readonly Dictionary<string, string> _mismatches = new Dictionary<string, string>() {
            { "Unset_desc","Unset" },

            { "Accelerating","Accelerate" },
            { "AnimaConduitMelee","Anima" },
            { "AnimaConduitMelee_desc","Anima_desc" },
            { "AnimaConduitRanged","Anima" },
            { "AnimaConduitRanged_desc","AnimaRanged_desc" },
            { "Celerity","Cool Down" },
            { "ChainReaction_desc","Chain_desc" },
            { "CriticalHit","Critical" },
            { "CriticalHit_desc","Critical_desc" },
            { "Deflecting","Deflect" },
            { "EnigmaResonatorMelee","EnigmaMelee" },
            { "EnigmaResonatorRanged","EnigmaRanged" },
            { "FireAspect","Fire" },
            { "FireAspect_desc","Fire_desc" },
            { "Gravity","GravityMelee" },
            { "Gravity_desc","GravityMelee_desc" },
            { "JunglePoisonRanged","JunglePoison" },
            { "MultiShot","Multi" },
            { "MultiShot_desc","Multi_desc" },
            { "PoisonedMelee","Poisoned" },
            { "PoisonedRanged","Poisoned" },
            { "PoisonedMelee_desc","Poisoned_desc" },
            { "PoisonedRanged_desc","Poisoned_desc" },
            { "Shockwave","Shock" },
            { "Shockwave_desc","Shock_desc" },
            { "SoulSiphon","Soul" },
            { "TempoTheft","Tempo" },
            { "TempoTheft_desc","Tempo_desc" },

            { "Powerbow","PowerBow" },
            { "Powerbow_Unique2","PowerBow_Unique2" },
            { "Flavour_Sword","Desc_Sword" },
            { "CorruptedSeeds","CorruptedSeeds_Unique1" },
            { "Flavour_CorruptedSeeds","Flavour_CorruptedSeeds_Unique1" },
            { "Beenest","BeeNest" },
            { "Daggers_unique2","Daggers_Unique2" },
            { "HighlanderLongSword","HighlanderLongSword" },
            { "LongBow","Longbow" },
            { "LongBow_Unique1","Longbow_Unique1" },
            { "LongBow_Unique2","Longbow_Unique2" },
            { "OakWoodBrew","OakwoodBrew" },
            { "Pickaxe_Steel","Pickaxe" },
            { "Pickaxe_Unique1_Steel","Pickaxe_Unique1" },
            { "ShortBow_Unique1","Shortbow_Unique1" },
            { "ShortBow_Unique2","Shortbow_Unique1" },
            { "ShortBow_Unique3","Shortbow_Unique1" },
            { "Slowbow_Unique1","SlowBow_Unique1" },
            { "Sword_Steel","Sword" },
            { "TrickBow","Trickbow" },
            { "TrickBow_Unique1","Trickbow_Unique1" },
            { "TrickBow_Unique2","Trickbow_Unique1" },
        };

        private static Dictionary<string, string> _itemType;
        private static Dictionary<string, string> _enchantment;
        private static Dictionary<string, string> _armorProperties;

        public static void loadExternalStrings(Dictionary<string, Dictionary<string, string>> stringLibrary)
        {
            _itemType = stringLibrary["ItemType"].ToDictionary(pair => pair.Key.Trim(), pair => pair.Value);
            _enchantment = stringLibrary["Enchantment"].ToDictionary(pair => pair.Key.Trim(), pair => pair.Value);
            _armorProperties = stringLibrary["ArmorProperties"].ToDictionary(pair => pair.Key.Trim(), pair => pair.Value);
        }

        internal static string formatFILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE(string filename) { return string.Format(FILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE, filename); }

        internal static string formatFILE_DECRYPT_ERROR_MESSAGE(string filename) { return string.Format(FILE_DECRYPT_ERROR_MESSAGE, filename); }

        internal static string formatITEMS_COUNT_LABEL(int items) { return string.Format(ITEMS_COUNT_LABEL, items); }

        internal static string formatVERSION(string versionString) { return string.Format(VERSION_FORMAT, versionString); }

        internal static string itemName(string type)
        {
            var key = type;
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_itemType.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for item {type}");
            return type;
        }

        internal static string itemDesc(string type)
        {
            var key = "Flavour_" + type;
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_itemType.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for item {key}");
            return type;
        }

        internal static string enchantment(string enchantment)
        {
            var key = enchantment;
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_enchantment.TryGetValue(key, out string value))
            {
                if(enchantment.ToLowerInvariant().Contains("ranged"))
                {
                    return $"{value} ({R.RANGED_ITEMS_FILTER})";
                }
                else if (enchantment.ToLowerInvariant().Contains("melee"))
                {
                    return $"{value} ({R.MELEE_ITEMS_FILTER})";
                }
                else
                {
                    return value;
                }
            }
            EventLogger.logError($"Could not find string for enchantment {enchantment}");
            return enchantment;
        }

        internal static string enchantmentDescription(string enchantment)
        {
            var key = enchantment + "_desc";
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_enchantment.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for enchantment {key}");
            return enchantment;
        }

        internal static string enchantmentEffect(string enchantment)
        {
            var key = enchantment + "_effect";
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_enchantment.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for enchantment {key}");
            return enchantment;
        }

        internal static string armorProperty(string armorPropertyId)
        {
            if (_armorProperties.TryGetValue(armorPropertyId, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for armor {armorPropertyId}");
            return armorPropertyId;
        }
        internal static string armorPropertyDescription(string armorPropertyId)
        {
            var key = armorPropertyId + "_description";
            if (_armorProperties.TryGetValue(key, out string description))
            {
                return description;
            }
            return armorProperty(armorPropertyId);
        }
    }
}
