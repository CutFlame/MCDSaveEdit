using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MCDSaveEdit
{
    internal class R : Properties.Resources
    {
        static Dictionary<string, string> _itemType;
        static Dictionary<string, string> _enchantment;
        static Dictionary<string, string> _armorProperties;

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
            if (_itemType.TryGetValue(type, out string value))
            {
                return value;
            }
            Debug.WriteLine($"Could not find string for {type}");
            return type;
        }

        internal static string enchantment(string enchantment)
        {
            if (_enchantment.TryGetValue(enchantment, out string value))
            {
                return value;
            }
            Debug.WriteLine($"Could not find string for {enchantment}");
            return enchantment;
        }

        internal static string enchantmentDescription(string enchantment)
        {
            var key = enchantment + "_desc";
            if (_enchantment.TryGetValue(key, out string value))
            {
                return value;
            }
            Debug.WriteLine($"Could not find string for {key}");
            return enchantment;
        }

        internal static string enchantmentEffect(string enchantment)
        {
            var key = enchantment + "_effect";
            if (_enchantment.TryGetValue(key, out string value))
            {
                return value;
            }
            Debug.WriteLine($"Could not find string for {key}");
            return enchantment;
        }

        internal static string armorProperty(string armorPropertyId)
        {
            if (_armorProperties.TryGetValue(armorPropertyId, out string value))
            {
                return value;
            }
            Debug.WriteLine($"Could not find string for {armorPropertyId}");
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
