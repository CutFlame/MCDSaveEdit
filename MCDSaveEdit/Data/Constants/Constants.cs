﻿#nullable enable

using System;

namespace MCDSaveEdit.Data
{
    public static partial class Constants
    {
        public const string CURRENT_VERSION_NUMBER = "1.5.8.0";
        public const string LATEST_RELEASE_GITHUB_URL = "https://github.com/CutFlame/MCDSaveEdit/releases/latest";
        public const string LATEST_RELEASE_MOD_URL_FORMAT = "https://www.nexusmods.com/minecraftdungeons/mods/{0}?tab=files";

        // The application's name used for identification in the registry.
        public const string APPLICATION_NAME = "MCDSaveEdit";
        public const string PAK_FILE_LOCATION_REGISTRY_KEY = "PakFilesPath";
        public const string LANG_SPECIFIER_REGISTRY_KEY = "LangSpecifier";
        public const int MAX_RECENT_FILES = 10;

        public const int MAXIMUM_INVENTORY_ITEM_COUNT = 300;

        public const int MINIMUM_ENCHANTMENT_TIER = 0;
        public const int MAXIMUM_ENCHANTMENT_TIER = 3;

        public const int MINIMUM_CHARACTER_LEVEL = 1;
        public const int MAXIMUM_CHARACTER_LEVEL = 1_000_000_000;

        public const int MINIMUM_ITEM_LEVEL = 0;
        public const int MAXIMUM_ITEM_LEVEL = 1_000_000_000;

        public const int MAXIMUM_ENCHANTMENT_OPTIONS_PER_ITEM = 9;
        public const string DEFAULT_ENCHANTMENT_ID = "Unset";

        //These are the names found in the character save files
        public const string EMERALD_CURRENCY_NAME = "Emerald";
        public const string GOLD_CURRENCY_NAME = "Gold";
        public const string EYE_OF_ENDER_CURRENCY_NAME = "EyeOfEnder";

        public const string DEFAULT_LANG_SPECIFIER = "en";

        public static readonly Version CURRENT_VERSION = new Version(CURRENT_VERSION_NUMBER);
        public static bool IS_DEBUG {
            get {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

    }
}
