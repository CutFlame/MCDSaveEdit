using FModel;
using MCDSaveEdit.Data;
using MCDSaveEdit.Services;
using MCDSaveEdit.ViewModels;
using PakReader;
using PakReader.Pak;
using PakReader.Parsers.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEdit
{
    public class AppModel
    {
        public MainViewModel mainModel = new MainViewModel();

        public void initPakReader()
        {
            Globals.Game = new FGame(EGame.MinecraftDungeons, EPakVersion.FNAME_BASED_COMPRESSION_METHOD);
        }

        public string? usableGameContentIfExists()
        {
#if DEBUG
            string? result = null;
            foreach (var path in contentPathsToCheck())
            {
                if (string.IsNullOrWhiteSpace(path) == false)
                {
                    if (Directory.Exists(path))
                    {
                        Console.WriteLine($"{path} exists");
                        result = result ?? path;
                    }
                    else
                    {
                        Console.WriteLine($"{path} does not exist");
                    }
                }
            }
            return result;
#else
            foreach(var path in contentPathsToCheck())
            {
                if (string.IsNullOrWhiteSpace(path) == false && Directory.Exists(path))
                {
                    return path;
                }
            }

            return null;
#endif
        }

        private IEnumerable<string?> contentPathsToCheck()
        {
            yield return RegistryTools.GetSetting(Constants.APPLICATION_NAME, Constants.PAK_FILE_LOCATION_REGISTRY_KEY, string.Empty);
            yield return Constants.STEAM_PAKS_FOLDER_PATH;
            yield return Constants.LAUNCHER_PAKS_FOLDER_PATH;
            yield return Constants.XBOX_PC_GAMES_PAKS_FOLDER_PATH;
            //yield return Constants.WINSTORE_PAKS_FOLDER_PATH_IF_EXISTS;
        }

        public static bool gameContentLoaded { get; private set; } = false;

        public async Task loadGameContentAsync(string paksFolderPath)
        {
            var pakIndex = await loadPakIndex(paksFolderPath);
            if (pakIndex == null)
            {
                throw new NullReferenceException($"PakIndex is null. Cannot Continue.");
            }

            var pakContentResolver = new PakContentResolver(pakIndex!, paksFolderPath);
            await pakContentResolver.loadPakFilesAsync(preloadBitmaps: false);
            ImageResolver.instance = pakContentResolver;
            LanguageResolver.instance = pakContentResolver;
            gameContentLoaded = true;

            RegistryTools.SaveSetting(Constants.APPLICATION_NAME, Constants.PAK_FILE_LOCATION_REGISTRY_KEY, paksFolderPath!);

            //Load language strings
            var lang = RegistryTools.GetSetting(Constants.APPLICATION_NAME, Constants.LANG_SPECIFIER_REGISTRY_KEY, Constants.DEFAULT_LANG_SPECIFIER);
            loadLanguageStrings(lang);
        }

        public static string currentLangSpecifier { get; private set; } = string.Empty;

        public static void loadLanguageStrings(string lang)
        {
            if(lang == currentLangSpecifier)
            {
                return;
            }

            R.unloadExternalStrings();

            Console.WriteLine($"Extracting '{lang}' language strings...");
            var stringLibrary = LanguageResolver.instance.loadLanguageStrings(lang);
            if (stringLibrary != null)
            {
                Console.WriteLine($"Loading '{lang}' language strings...");
                R.loadExternalStrings(stringLibrary);
            }

            currentLangSpecifier = lang;

            RegistryTools.SaveSetting(Constants.APPLICATION_NAME, Constants.LANG_SPECIFIER_REGISTRY_KEY, lang);
        }

        private Task<PakIndex?> loadPakIndex(string paksFolderPath)
        {
            var tcs = new TaskCompletionSource<PakIndex?>();
            Task.Run(() =>
            {
                try
                {
                    var filter = new PakFilter(new[] { Constants.PAKS_FILTER_STRING }, false);
                    var pakIndex = new PakIndex(path: paksFolderPath, cacheFiles: true, caseSensitive: true, filter: filter);
                    if (pakIndex.PakFileCount == 0)
                    {
                        throw new FileNotFoundException($"No files were found at {paksFolderPath}");
                    }
                    var success = unlockPakIndex(pakIndex);
                    if (success == false)
                    {
                        throw new InvalidOperationException($"Could not decrypt pak files at {paksFolderPath}");
                    }
                    var versionStr = R.formatMCD_VERSION(mainModel.detectedGameVersion ?? R.ERROR);
                    Console.WriteLine($"Detected Pak Files for {versionStr}");

                    tcs.SetResult(pakIndex);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not load Minecraft Dungeons Paks: {e}");
                    tcs.SetException(e);
                }
            });
            return tcs.Task;
        }

        private bool unlockPakIndex(PakIndex pakIndex)
        {
            foreach (var key in Secrets.PAKS_AES_KEYS)
            {
                var keyStr = key.key;
                byte[] keyBytes;
                if (keyStr.StartsWith("0x"))
                {
                    keyBytes = keyStr.Substring(2).ToBytesKey();
                }
                else
                {
                    keyBytes = keyStr.ToBytesKey();
                }

                if (keyBytes.Length != 32)
                {
                    throw new InvalidOperationException($"AES Key ({keyStr.Substring(0, 3)}...) is {keyBytes.Length} bytes instead of the expected 32");
                }
                var count = pakIndex.UseKey(FGuid.Zero, keyBytes);
                if (count > 0)
                {
                    mainModel.detectedGameVersion = key.versions;
                    return true;
                }
            }
            mainModel.detectedGameVersion = null;
            return false;
        }

        public void unloadGameContent()
        {
            //Clear the path saved in the registry
            RegistryTools.DeleteSetting(Constants.APPLICATION_NAME, Constants.PAK_FILE_LOCATION_REGISTRY_KEY);

            if (gameContentLoaded)
            {
                gameContentLoaded = false;
                var localContentResolver = new LocalContentResolver();
                ImageResolver.instance = localContentResolver;
                LanguageResolver.instance = localContentResolver;
                mainModel.detectedGameVersion = null;
            }

            if (!string.IsNullOrWhiteSpace(currentLangSpecifier))
            {
                R.unloadExternalStrings();
            }
        }

        private static string spaceOutWords(string input)
        {
            var output = new StringBuilder();
            for (int ii = 0; ii < input.Length; ii++)
            {
                var letter = input[ii];
                if (ii > 0 && char.IsUpper(letter) && output[output.Length] != ' ')
                {
                    output.Append(' ');
                }
                output.Append(letter);
            }
            return output.ToString();
        }

    }
}
