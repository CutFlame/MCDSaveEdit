using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MCDSaveEdit
{
    internal class R : Properties.Resources
    {
        static Dictionary<string, Dictionary<string, string>> _stringLibrary;

        public static void loadExternalStrings(Dictionary<string, Dictionary<string, string>> stringLibrary)
        {
            _stringLibrary = stringLibrary;
        }

        internal static string formatFILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE(string filename) { return string.Format(FILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE, filename); }

        internal static string formatFILE_DECRYPT_ERROR_MESSAGE(string filename) { return string.Format(FILE_DECRYPT_ERROR_MESSAGE, filename); }

        internal static string formatITEMS_COUNT_LABEL(int items) { return string.Format(ITEMS_COUNT_LABEL, items); }

        internal static string formatVERSION(string versionString) { return string.Format(VERSION_FORMAT, versionString); }

        internal static string itemName(string type)
        {
            var main = _stringLibrary["ItemType"];
            if (main.TryGetValue(type, out string value))
            {
                return value;
            }
            Debug.WriteLine($"Could not find string for {type}");
            return type;
        }

        internal static string enchantment(string enchantment)
        {
            var main = _stringLibrary["Enchantment"];
            if (main.TryGetValue(enchantment, out string value))
            {
                return value;
            }
            Debug.WriteLine($"Could not find string for {enchantment}");
            return enchantment;
        }
    }
}
