using System;
using System.IO;

namespace MCDSaveEdit
{
    public static class Constants
    {
        public const string LATEST_RELEASE_URL = "https://github.com/CutFlame/MCDSaveEdit/releases/latest";
        public const string CURRENT_RELEASE_TAG_NAME = "1.1.1";

        public const int MINIMUM_ENCHANTMENT_TIER = 0;
        public const int MAXIMUM_ENCHANTMENT_TIER = 3;

        public const int MINIMUM_CHARACTER_LEVEL = 0;
        public const int MAXIMUM_CHARACTER_LEVEL = 999;

        public const int MINIMUM_ITEM_LEVEL = 0;
        public const int MAXIMUM_ITEM_LEVEL = 999;

        public const string PAKS_AES_KEY_STRING = "90F270A4EA6DE0E3BABDC4C8BDC04F46FA5B9087BE8FE76AA859C93C17D04661";

        public static string PAKS_FOLDER
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(appData, "Mojang", "products", "dungeons", "dungeons", "Dungeons", "Content", "Paks");
            }
        }
    }
}
