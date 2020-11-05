using System;
using System.IO;

namespace MCDSaveEdit
{
    public static class Constants
    {
        public const string LATEST_RELEASE_URL = "https://github.com/CutFlame/MCDSaveEdit/releases/latest";
        public const string CURRENT_RELEASE_TAG_NAME = "1.2.1";

        public const int MINIMUM_ENCHANTMENT_TIER = 0;
        public const int MAXIMUM_ENCHANTMENT_TIER = 3;

        public const int MINIMUM_CHARACTER_LEVEL = 0;
        public const int MAXIMUM_CHARACTER_LEVEL = 1000000000;

        public const int MINIMUM_ITEM_LEVEL = 0;
        public const int MAXIMUM_ITEM_LEVEL = 1000000000;

        public const string PAKS_FILTER_STRING = "/Dungeons/Content";

        public static string PAKS_FOLDER
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(appData, "Mojang", "products", "dungeons", "dungeons", "Dungeons", "Content", "Paks");
            }
        }

        public const string FILE_DIALOG_INITIAL_DIRECTORY = @"%USER%\Saved Games\Mojang Studios\Dungeons\";
        public const string ENCRYPTED_FILE_EXTENSION = ".dat";
        public const string DECRYPTED_FILE_EXTENSION = ".json";

    }
}
