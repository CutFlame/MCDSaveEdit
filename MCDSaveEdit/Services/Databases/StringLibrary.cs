using MCDSaveEdit.Data;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace MCDSaveEdit.Services
{
    public class R : Properties.Resources
    {
        private static Dictionary<string, string> _itemType = new Dictionary<string, string>();
        private static Dictionary<string, string> _enchantment = new Dictionary<string, string>();
        private static Dictionary<string, string> _armorProperties = new Dictionary<string, string>();
        private static Dictionary<string, string> _mission = new Dictionary<string, string>();
        private static Dictionary<string, string> _clickys = new Dictionary<string, string>();

        public static bool isStringsLoaded { get; private set; } = false;

        public static int totalStringCount {
            get {
                return _itemType.Count
                    + _enchantment.Count
                    + _armorProperties.Count
                    + _mission.Count
                    + _clickys.Count;
            }
        }

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
                && stringLibrary.TryGetValue("Content_Season1/Decor/Text/TowerUILabels.csv", out var towerDict)
                && stringLibrary.TryGetValue("Content_Season2/Decor/Text/Season2MerchantLabels.csv", out var s2MerchantLabelsDict)
                && stringLibrary.TryGetValue("ItemPowerEffect", out var itemEffectDict)
                )
            {
                _clickys = clickysDict
                    .Concat(realmsDict)
                    .Concat(dlcDict)
                    .Concat(ancientLabelsDict)
                    .Concat(merchantLabelsDict)
                    .Concat(missionInterestDict)
                    .Concat(diffDict)
                    .Concat(towerDict)
                    .Concat(s2MerchantLabelsDict)
                    .Concat(itemEffectDict)
                    .ToDictionary(pair => pair.Key.Trim(), pair => pair.Value);
            }
            isStringsLoaded = true;
        }

        public static void unloadExternalStrings()
        {
            _itemType.Clear();
            _enchantment.Clear();
            _armorProperties.Clear();
            _mission.Clear();
            _clickys.Clear();
            isStringsLoaded = false;
        }

        internal static string formatFILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE(string filename) { return string.Format(FILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE, filename); }
        internal static string formatFILE_DECRYPT_ERROR_MESSAGE(string filename) { return string.Format(FILE_DECRYPT_ERROR_MESSAGE, filename); }
        internal static string formatITEMS_COUNT_LABEL(int items, int max) { return string.Format(ITEMS_COUNT_LABEL, items, max); }
        internal static string formatVERSION(string versionLabel, string versionString) { return string.Format(VERSION_FORMAT, versionLabel, versionString); }
        internal static string formatNEXT_LEVEL_LABEL(int nextFloor, int totalFloors, string floorType) { return string.Format(NEXT_LEVEL_LABEL, nextFloor, totalFloors, floorType); }

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
            if (Constants.stringMismatches.ContainsKey(key))
            {
                key = Constants.stringMismatches[key];
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
            if (Constants.stringMismatches.ContainsKey(key))
            {
                key = Constants.stringMismatches[key];
            }
            if (_enchantment.TryGetValue(key, out string value))
            {
                if(enchantment.ToLowerInvariant().Contains("ranged"))
                {
                    var classification = R.getString("ItemTag_Ranged") ?? R.RANGED_ITEMS_FILTER;
                    return $"{value} ({classification})";
                }
                else if (enchantment.ToLowerInvariant().Contains("melee"))
                {
                    var classification = R.getString("ItemTag_Melee") ?? R.MELEE_ITEMS_FILTER;
                    return $"{value} ({classification})";
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
            if (Constants.stringMismatches.ContainsKey(key))
            {
                key = Constants.stringMismatches[key];
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
            if (Constants.stringMismatches.ContainsKey(key))
            {
                key = Constants.stringMismatches[key];
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
            if (Constants.stringMismatches.ContainsKey(key))
            {
                key = Constants.stringMismatches[key];
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
            if (Constants.stringMismatches.ContainsKey(key))
            {
                key = Constants.stringMismatches[key];
            }
            if (_clickys.TryGetValue(key, out string value))
            {
                return value;
            }
            EventLogger.logError($"Could not find string for key -{key}-");
            return null;
        }

    }
}
