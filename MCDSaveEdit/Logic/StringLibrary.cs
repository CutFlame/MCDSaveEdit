using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace MCDSaveEdit
{
    public class R : Properties.Resources
    {
        private static readonly Dictionary<string, string> _mismatches = new Dictionary<string, string>() {
            //{"Imagefile reference","Game.json reference"},
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
            { "Firetrail","FireTrail" },
            { "Firetrail_desc","FireTrail_desc" },
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
            { "JunglePoisonMelee_desc","JunglePoisonMelee" },
            { "JunglePoisonRanged_desc","JunglePoison" },
            { "EnigmaResonatorMelee_desc","Enigma_desc" },
            { "EnigmaResonatorRanged_desc","EnigmaRanged_desc" },
            { "SoulSiphon_desc","Soul_desc" },
            { "EmeraldDivination_desc","EmeraldDivination_effect_desc" },
            { "DeathBarter_desc","DeathBarter_effect_desc" },

            { "Powerbow","PowerBow" },
            { "Powerbow_Unique1","PowerBow_Unique1" },
            { "Powerbow_Unique2","PowerBow_Unique2" },
            { "Flavour_Sword","Desc_Sword" },
            { "Flavour_Sword_Steel","Desc_Sword" },
            { "Flavour_Sword_Spooky1", "Desc_Sword_Spooky1" },
            { "Flavour_ShortBow_Unique3", "Flavour_Shortbow_Unique3" },
            { "CorruptedSeeds","CorruptedSeeds_Unique1" },
            { "Flavour_CorruptedSeeds","Flavour_CorruptedSeeds_Unique1" },
            { "Beenest","BeeNest" },
            { "Flavour_Beenest", "Flavour_BeeNest" },
            { "Daggers_unique2","Daggers_Unique2" },
            { "Flavour_Daggers_unique2","Flavour_Daggers_Unique2" },
            { "HighlanderLongSword","Sword" }, //Missing
            { "Flavour_HighlanderLongSword","Desc_Sword" }, //Missing
            { "LongBow","Longbow" },
            { "Flavour_LongBow","Flavour_Longbow" },
            { "LongBow_Unique1","Longbow_Unique1" },
            { "LongBow_Unique2","Longbow_Unique2" },
            { "TwistingVineBow_UNique1","TwistingVineBow_Unique1" },
            { "Flavour_TwistingVineBow_UNique1","Flavour_TwistingVineBow_Unique1" },
            { "OakWoodBrew","OakwoodBrew" },
            { "Pickaxe_Steel","Pickaxe" },
            { "Flavour_Pickaxe_Steel","Flavour_Pickaxe" },
            { "Pickaxe_Unique1_Steel","Pickaxe_Unique1" },
            { "Flavour_Pickaxe_Unique1_Steel","Flavour_Pickaxe_Unique1" },
            { "ShortBow_Unique1","Shortbow_Unique1" },
            { "ShortBow_Unique2","Shortbow_Unique1" },
            { "ShortBow_Unique3","Shortbow_Unique1" },
            { "Slowbow_Unique1","SlowBow_Unique1" },
            { "Sword_Steel","Sword" },
            { "Flavour_Sword_Unique2", "Desc_Sword_Unique2" },
            { "Flavour_Sword_Unique1", "Desc_Sword_Unique1" },
            { "TrickBow","Trickbow" },
            { "TrickBow_Unique1","Trickbow_Unique1" },
            { "TrickBow_Unique2","Trickbow_Unique2" },
            { "TrickBow_Year1","Trickbow_Year1" },
            { "Flavour_TrickBow_Year1","Flavour_Trickbow_Year1" },
            { "Flavour_LongBow_Unique1","Flavour_Longbow_Unique1" },
            { "Flavour_LongBow_Unique2","Flavour_Longbow_Unique2" },
            { "Flavour_PowerBow","Flavour_Powerbow" },
            { "Flavour_PowerBow_Unique1","Flavour_Powerbow_Unique1" },
            { "Flavour_PowerBow_Unique2","Flavour_Powerbow_Unique2" },
            { "Flavour_ShortBow_Unique1","Flavour_Shortbow_Unique1" },
            { "Flavour_ShortBow_Unique2","Flavour_Shortbow_Unique2" },
            { "Flavour_Slowbow_Unique1","Flavour_SlowBow_Unique1" },
            { "Flavour_TrickBow","Flavour_Trickbow" },
            { "Flavour_TrickBow_Unique1","Flavour_Trickbow_Unique1" },
            { "Flavour_TrickBow_Unique2","Flavour_Trickbow_Unique2" },
            { "Flavour_BackstabbersBrew","Desc_BackstabbersBrew" },
            { "Flavour_HealthPotion","Desc_HealthPotion" },
            { "Flavour_OakWoodBrew","OakwoodBrew" },
            { "Flavour_StrengthPotion","Desc_StrengthPotion" },
            { "Flavour_SwiftnessPotion","Desc_SwiftnessPotion" },
            { "Flavour_TNTBox","Desc_TNTBox"},
            { "EmeraldArmor_Unique1","Emerald_Armor_Unique1" },
            { "EmeraldArmor_Unique2","Emerald_Armor_Unique2" },
            { "Flavour_ClimbingGear_Unique1","Flavour_ClimbingGearSolid" },
            { "Flavour_ThunderingQuiver","Flavour_ThunderQuiver" },
            { "Flavour_Anchor","Flavour_Anchor_Unique1" },
            { "Flavour_CoralBlade","Flavour_CoralBlade_Unique1" },
            { "Flavour_HarpoonCrossbow","Flavour_HarpoonCrossbow_Unique1" },
            { "Flavour_WaterBreathingPotion","Desc_WaterBreathingPotion" },
            { "SatchelofNeed","SatchelOfNeed" },
            { "Flavour_SatchelofNeed","Flavour_SatchelOfNeed" },
            { "SatchelofNourishment","Desc_SatchelOfNourishment" },
            { "Flavour_SatchelofNourishment","Flavour_SatchelOfNourishment" },

            { "ReviveChance","Unset" }, //missing
            { "ReviveChance_description","Unset" }, //missing
            { "ItemCooldownDecrease","ArtifactCooldownDecrease" },
            { "ItemCooldownDecrease_description","ArtifactCooldownDecrease_description" },
            { "ItemDamageBoost","ArtifactDamageBoost" },
            { "ItemDamageBoost_description","ArtifactDamageBoost_description" },
            { "SlowResistance","FreezingResistance" },
            { "SlowResistance_description","FreezingResistance_description" },
            { "Reckless_desc","ShardArmor_desc" },
            { "Flee_desc","Flee_effect_desc" },
            { "ReliableRicochet_desc","Ricochet_desc" },

            { "underhalls_name","highblockhallsbonus_name" },
            { "lowertemple_name","deserttemplebonus_name" },
            { "soggycave_name","soggyswampbonus_name" },
            { "creepycrypt_name","creeperwoodsbonus_name" },
            { "archhaven_name","pumpkinpasturesbonus_name" },
        };

        private static Dictionary<string, string> _itemType = new Dictionary<string, string>();
        private static Dictionary<string, string> _enchantment = new Dictionary<string, string>();
        private static Dictionary<string, string> _armorProperties = new Dictionary<string, string>();
        private static Dictionary<string, string> _mission = new Dictionary<string, string>();
        private static Dictionary<string, string> _clickys = new Dictionary<string, string>();

        public static bool isStringsLoaded { get; private set; } = false;

        public static void loadExternalStrings(Dictionary<string, Dictionary<string, string>> stringLibrary)
        {
            if (stringLibrary.TryGetValue("ItemType", out var itemDict))
            {
                _itemType = itemDict.ToDictionary(pair => pair.Key.Trim(), pair => pair.Value);
            }
            if (stringLibrary.TryGetValue("Enchantment", out var enchantmentDict))
            {
                _enchantment = enchantmentDict.ToDictionary(pair => pair.Key.Trim(), pair => pair.Value);
            }
            if (stringLibrary.TryGetValue("ArmorProperties", out var armorPropertyDict))
            {
                _armorProperties = armorPropertyDict.ToDictionary(pair => pair.Key.Trim(), pair => pair.Value);
            }
            if (stringLibrary.TryGetValue("Mission", out var missionDict))
            {
                _mission = missionDict.ToDictionary(pair => pair.Key.Trim(), pair => pair.Value);
            }
            if (stringLibrary.TryGetValue("", out var clickysDict)
                && stringLibrary.TryGetValue("Realms", out var realmsDict)
                && stringLibrary.TryGetValue("DLC", out var dlcDict)
                && stringLibrary.TryGetValue("AncientLabels", out var ancientLabelsDict)
                && stringLibrary.TryGetValue("MerchantLabels", out var merchantLabelsDict)
                && stringLibrary.TryGetValue("MissionInterest", out var missionInterestDict)
                && stringLibrary.TryGetValue("Difficulty", out var diffDict)
                )
            {
                _clickys = clickysDict
                    .Concat(realmsDict)
                    .Concat(dlcDict)
                    .Concat(ancientLabelsDict)
                    .Concat(merchantLabelsDict)
                    .Concat(missionInterestDict)
                    .Concat(diffDict)
                    .ToDictionary(pair => pair.Key.Trim(), pair => pair.Value);
            }
            isStringsLoaded = true;
        }

        internal static string formatFILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE(string filename) { return string.Format(FILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE, filename); }

        internal static string formatFILE_DECRYPT_ERROR_MESSAGE(string filename) { return string.Format(FILE_DECRYPT_ERROR_MESSAGE, filename); }

        internal static string formatITEMS_COUNT_LABEL(int items, int max) { return string.Format(ITEMS_COUNT_LABEL, items, max); }

        internal static string formatVERSION(string versionString) { return string.Format(VERSION_FORMAT, versionString); }

        public static string itemName(string type)
        {
            return getItemString(type) ?? type;
        }

        public static string itemDesc(string type)
        {
            var key = "Flavour_" + type;
            return getItemString(key) ?? type;
        }

        private static string? getItemString(string key)
        {
            if (!isStringsLoaded) { return key; }
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_itemType.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for item {key}");
            return null;
        }


        public static string enchantmentName(string enchantment)
        {
            var key = enchantment;
            if (!isStringsLoaded) { return key; }
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
            EventLogger.logError($"Could not find string for enchantment {key}");
            return enchantment;
        }

        public static string enchantmentDescription(string enchantment)
        {
            var key = enchantment + "_desc";
            return getEnchantmentString(key) ?? enchantment;
        }

        public static string enchantmentEffect(string enchantment)
        {
            var key = enchantment + "_effect";
            return getEnchantmentString(key) ?? enchantment;
        }

        private static string? getEnchantmentString(string key)
        {
            if (!isStringsLoaded) { return key; }
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_enchantment.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for enchantment {key}");
            return null;
        }

        public static string armorProperty(string armorPropertyId)
        {
            return getArmorPropertyString(armorPropertyId) ?? armorPropertyId;
        }

        public static string armorPropertyDescription(string armorPropertyId)
        {
            var key = armorPropertyId + "_description";
            return getArmorPropertyString(key) ?? armorPropertyId;
        }

        private static string? getArmorPropertyString(string key)
        {
            if (!isStringsLoaded) { return key; }
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_armorProperties.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for armor {key}");
            return null;
        }

        public static string? getMissionName(string missionId)
        {
            var key = missionId + "_name";
            return getMissionString(key) ?? missionId;
        }

        private static string? getMissionString(string key)
        {
            if (!isStringsLoaded) { return key; }
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_mission.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for mission {key}");
            return null;
        }

        public static string? getString(string key)
        {
            if (!isStringsLoaded) { return null; }
            if (_mismatches.ContainsKey(key))
            {
                key = _mismatches[key];
            }
            if (_clickys.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for mission {key}");
            return null;
        }

    }
}
