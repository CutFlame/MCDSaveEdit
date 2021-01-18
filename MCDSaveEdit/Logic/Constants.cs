using MCDSaveEdit.Save.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Windows.Management.Deployment;
#nullable enable

namespace MCDSaveEdit
{
    public static class Constants
    {
        public const string LATEST_RELEASE_URL = "https://github.com/CutFlame/MCDSaveEdit/releases/latest";
        public const string CURRENT_RELEASE_TAG_NAME = "1.3.0";

        public const int MAXIMUM_INVENTORY_ITEM_COUNT = 300;

        public const int MINIMUM_ENCHANTMENT_TIER = 0;
        public const int MAXIMUM_ENCHANTMENT_TIER = 3;

        public const int MINIMUM_CHARACTER_LEVEL = 0;
        public const int MAXIMUM_CHARACTER_LEVEL = 1000000000;

        public const int MINIMUM_ITEM_LEVEL = 0;
        public const int MAXIMUM_ITEM_LEVEL = 1000000000;

        public const string PAKS_FILTER_STRING = "/Dungeons/Content";

        public const string FIRST_PAK_FILENAME = "pakchunk0-WindowsNoEditor.pak";

        public static string PAKS_FOLDER_PATH {
            get {
                var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(appDataFolderPath, "Mojang", "products", "dungeons", "dungeons", "Dungeons", "Content", "Paks");
            }
        }

        public static string? WINSTORE_PAKS_FOLDER_IF_EXISTS {
            get {
                var pm = new PackageManager();
                foreach (var pkg in pm.FindPackagesForUser(string.Empty, "Microsoft.Lovika_8wekyb3d8bbwe"))
                {
                    if (pkg.IsDevelopmentMode) // Only true if run through the script
                    {
                        return Path.Combine(pkg.InstalledLocation.Path, "Dungeons", "Content", "Paks");
                    }
                }
                // Game has not been run through the script, meaning the files are not accessible
                return null;
            }
        }

        public static string FILE_DIALOG_INITIAL_DIRECTORY {
            get {
                var userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(userFolderPath, "Saved Games", "Mojang Studios", "Dungeons");
            }
        }
        public const string ENCRYPTED_FILE_EXTENSION = ".dat";
        public const string DECRYPTED_FILE_EXTENSION = ".json";

        public readonly static StaticLevelData[] DEBUG_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData(".00", new Point(0.00, 0.00), LevelTypeEnum.mission),
            new StaticLevelData(".05", new Point(0.05, 0.05), LevelTypeEnum.mission),
            new StaticLevelData(".10", new Point(0.10, 0.10), LevelTypeEnum.mission),
            new StaticLevelData(".15", new Point(0.15, 0.15), LevelTypeEnum.mission),
            new StaticLevelData(".20", new Point(0.20, 0.20), LevelTypeEnum.mission),
            new StaticLevelData(".25", new Point(0.25, 0.25), LevelTypeEnum.mission),
            new StaticLevelData(".30", new Point(0.30, 0.30), LevelTypeEnum.mission),
            new StaticLevelData(".35", new Point(0.35, 0.35), LevelTypeEnum.mission),
            new StaticLevelData(".40", new Point(0.40, 0.40), LevelTypeEnum.mission),
            new StaticLevelData(".45", new Point(0.45, 0.45), LevelTypeEnum.mission),
            new StaticLevelData(".50", new Point(0.50, 0.50), LevelTypeEnum.mission),
            new StaticLevelData(".55", new Point(0.55, 0.55), LevelTypeEnum.mission),
            new StaticLevelData(".60", new Point(0.60, 0.60), LevelTypeEnum.mission),
            new StaticLevelData(".65", new Point(0.65, 0.65), LevelTypeEnum.mission),
            new StaticLevelData(".70", new Point(0.70, 0.70), LevelTypeEnum.mission),
            new StaticLevelData(".75", new Point(0.75, 0.75), LevelTypeEnum.mission),
            new StaticLevelData(".80", new Point(0.80, 0.80), LevelTypeEnum.mission),
            new StaticLevelData(".85", new Point(0.85, 0.85), LevelTypeEnum.mission),
            new StaticLevelData(".90", new Point(0.90, 0.90), LevelTypeEnum.mission),
            new StaticLevelData(".95", new Point(0.95, 0.95), LevelTypeEnum.mission),
            new StaticLevelData("1.0", new Point(1.00, 1.00), LevelTypeEnum.mission),
        };

        public readonly static StaticLevelData[] MAINLAND_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("creepycrypt", new Point(.19, .37), LevelTypeEnum.dungeon),
            new StaticLevelData("mooshroomisland", new Point(.26, .24), LevelTypeEnum.dungeon),
            new StaticLevelData("creeperwoods", new Point(.28, .40), LevelTypeEnum.mission),
            new StaticLevelData("soggycave", new Point(.27, .65), LevelTypeEnum.dungeon),
            new StaticLevelData("soggyswamp", new Point(.29, .74), LevelTypeEnum.mission),
            new StaticLevelData("mooncorecaverns", new Point(.44, .35), LevelTypeEnum.mission),
            new StaticLevelData("cacticanyon", new Point(.42, .56), LevelTypeEnum.mission),
            new StaticLevelData("pumpkinpastures", new Point(.46, .70), LevelTypeEnum.mission),
            new StaticLevelData("archhaven", new Point(.57, .73), LevelTypeEnum.dungeon),
            new StaticLevelData("deserttemple", new Point(.59, .45), LevelTypeEnum.mission),
            new StaticLevelData("fieryforge", new Point(.63, .20), LevelTypeEnum.mission),
            new StaticLevelData("lowertemple", new Point(.67, .59), LevelTypeEnum.dungeon),
            new StaticLevelData("highblockhalls", new Point(.75, .40), LevelTypeEnum.mission),
            new StaticLevelData("obsidianpinnacle", new Point(.86, .25), LevelTypeEnum.mission),
            new StaticLevelData("underhalls", new Point(.87, .49), LevelTypeEnum.dungeon),
            new StaticLevelData("squidcoast", new Point(.15, .64), LevelTypeEnum.mission),
        };

        public readonly static StaticLevelData[] JUNGLE_AWAKENS_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("dingyjungle", new Point(.24, .44), LevelTypeEnum.mission),
            new StaticLevelData("overgrowntemple", new Point(.62, .15), LevelTypeEnum.mission),
            new StaticLevelData("bamboobluff", new Point(.65, .55), LevelTypeEnum.dungeon),
        };

        public readonly static StaticLevelData[] CREEPING_WINTER_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("frozenfjord", new Point(.24, .44), LevelTypeEnum.mission),
            new StaticLevelData("lonelyfortress", new Point(.62, .15), LevelTypeEnum.mission),
            new StaticLevelData("lostsettlement", new Point(.65, .55), LevelTypeEnum.dungeon),
        };

        public readonly static StaticLevelData[] HOWLING_PEAKS_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("windsweptpeaks", new Point(.24, .44), LevelTypeEnum.mission),
            new StaticLevelData("galesanctum", new Point(.62, .15), LevelTypeEnum.mission),
            new StaticLevelData("endlessrampart", new Point(.65, .55), LevelTypeEnum.dungeon),
        };

        public readonly static Dictionary<string, StaticLevelData> LEVEL_DATA_LOOKUP =
            MAINLAND_LEVEL_DATA
            .Concat(JUNGLE_AWAKENS_LEVEL_DATA)
            .Concat(CREEPING_WINTER_LEVEL_DATA)
            .Concat(HOWLING_PEAKS_LEVEL_DATA)
            .ToDictionary(data=>data.key);
    }
}
