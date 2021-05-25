using System;
using System.IO;
using Windows.Management.Deployment;
#nullable enable

namespace MCDSaveEdit
{
    public static partial class Constants
    {
        public const string PAKS_FILTER_STRING = "/Dungeons/Content";

        public const string FIRST_PAK_FILENAME = "pakchunk0-WindowsNoEditor.pak";

        //NOTE: default location of files for Launcher version: %localappdata%\Mojang\products\dungeons\dungeons\Dungeons\Content\Paks
        public static string PAKS_FOLDER_PATH {
            get {
                var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(appDataFolderPath, "Mojang", "products", "dungeons", "dungeons", "Dungeons", "Content", "Paks");
            }
        }

        //NOTE: location of files for WinStore version is where ever the storepatcher script dumped them
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

        //NOTE: default location of save game files: %userprofile%\Saved Games\Mojang Studios\Dungeons\2533274911688652\Characters
        public static string FILE_DIALOG_INITIAL_DIRECTORY {
            get {
                var userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(userFolderPath, "Saved Games", "Mojang Studios", "Dungeons");
            }
        }
        public const string ENCRYPTED_FILE_EXTENSION = ".dat";
        public const string DECRYPTED_FILE_EXTENSION = ".json";
    }
}
