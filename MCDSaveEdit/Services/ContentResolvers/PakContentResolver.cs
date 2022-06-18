using MCDSaveEdit.Data;
using MCDSaveEdit.Interfaces;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using PakReader.Pak;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit.Services
{
    public class PakContentResolver : IImageResolver, ILanguageResolver
    {
        private static readonly Dictionary<string, string> _mismatches = new Dictionary<string, string>() {
            //{"Imagefile foldername","Savefile reference"},
            { "TrickBow","Trickbow" },
            { "TrickBow_Unique1","Trickbow_Unique1" },
            { "TrickBow_Unique2","Trickbow_Unique2" },
            { "TrickBow_Year1","Trickbow_Year1" },
            { "LongBow","Longbow" },
            { "LongBow_Unique1","Longbow_Unique1" },
            { "LongBow_Unique2","Longbow_Unique2" },
            { "PowerBow","Powerbow" },
            { "PowerBow_Unique1","Powerbow_Unique1" },
            { "PowerBow_Unique2","Powerbow_Unique2" },
            { "Slowbow_Unique1","SlowBow_Unique1" },
            { "ShortBow","Shortbow" },
            { "ShortBow_Unique1","Shortbow_Unique1" },
            { "ShortBow_Unique2","Shortbow_Unique2" },
            { "ShortBow_Unique3","Shortbow_Unique3" },
            { "Huntingbow_Unique1","HuntingBow_Unique1" },
            { "TwistingVineBow_UNique1","TwistingVineBow_Unique1" },

            { "Battlerobe_unique1","BattleRobe_Unique1" },

            { "Sword_Steel","Sword" },
            { "Pickaxe_Steel","Pickaxe" },
            { "Pickaxe_Unique1_Steel","Pickaxe_Unique1" },
            { "Daggers_unique2","Daggers_Unique2" },

            { "Beenest","BeeNest" },
            { "SatchelofNourishment","SatchelOfNourishment" },
            { "SatchelofNeed","SatchelOfNeed" },

            { "Firetrail","FireTrail" },
        };

        //These items should be hidden from any and all lists because they won't work in the game.
        private static readonly string[] _blockedItems = new[] {
            "MysteryBox",
            "Potions",
            "Food",
            "Elytra",
            "EyeOfEnder",
            "Arrow_Icon",
            "TNTBox",
            "Trident",
            "HighlanderLongSword",
        };

        private readonly LocalContentResolver _backupResolver;
        private readonly PakIndex _pakIndex;
        private readonly Dictionary<string, string> _enchantments = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _equipment = new Dictionary<string, string>();
        private readonly Dictionary<string, BitmapImage> _bitmaps = new Dictionary<string, BitmapImage>();
        private readonly Dictionary<string, string> _localizations = new Dictionary<string, string>();
        public IReadOnlyCollection<string> localizationOptions { get { return _localizations.Keys; } }
        private readonly List<string> _levels = new List<string>();
        public string? path { get; private set; }

        public PakContentResolver(PakIndex pakIndex, string? path)
        {
            this.path = path;
            _pakIndex = pakIndex;
            _backupResolver = new LocalContentResolver();
        }

        public Task loadPakFilesAsync(bool preloadBitmaps = false)
        {
            var tcs = new TaskCompletionSource<object?>();
            Task.Run(() => {
                this.loadPakFiles(preloadBitmaps);
                tcs.SetResult(null);
            });
            return tcs.Task;
        }

        public void loadPakFiles(bool preloadBitmaps = false)
        {
            Debug.WriteLine($"Loading Pak Files");
            foreach (var item in _pakIndex)
            {
                if (item == null) continue;
                //Drop the mount point prefix
                var startIndex = item!.IndexOf("//") + 1;
                var fullPath = item!.Substring(startIndex);

                if (fullPath.Contains("Localization") && fullPath.EndsWith("Game"))
                {
                    //Get the folder name because it will be the language specifier
                    string lang = Path.GetDirectoryName(fullPath).Split(Path.DirectorySeparatorChar).Last();
                    if (lang != "Game") //Exception for specific invalid language file
                    {
                        _localizations.Add(lang, fullPath);
                        //Debug.WriteLine($"LocRes {lang} - {fullPath}");
                    }
                    continue;
                }

                if (fullPath.Contains("ArmorProperties") && !fullPath.Contains("Cues"))
                {
                    //Get the folder name because it will be the name of the armor property
                    string armorProperty = Path.GetDirectoryName(fullPath).Split(Path.DirectorySeparatorChar).Last();
                    if(armorProperty == "ArmorProperties" || armorProperty == "ReviveChance") { continue; } //Exception
                    ItemDatabase.armorProperties.Add(armorProperty);
                    //Debug.WriteLine($"Found ArmorProperty {armorProperty} in: {fullPath}");
                    continue;
                }

                var filename = Path.GetFileName(fullPath);
                var fullPathLowercase = fullPath.ToLowerInvariant();

                if (fullPath.Contains("data") && fullPath.Contains("levels"))
                {
                    _levels.Add(filename);
                    //Debug.WriteLine($"Found level {filename}");
                    continue;
                }

                if (!filename.StartsWith("T") || !fullPathLowercase.Contains("_icon"))
                {
                    //Debug.WriteLine($"Package is not an icon {fullPath}");
                    continue;
                }

                if (preloadBitmaps)
                {
                    var bitmap = _pakIndex.extractBitmap(fullPath);
                    if (bitmap != null)
                    {
                        _bitmaps[fullPath] = bitmap;
                    }
                }

                var foldername = Path.GetDirectoryName(fullPath).Split(Path.DirectorySeparatorChar).Last();
                if (fullPath.Contains("Enchantments") && fullPathLowercase.EndsWith("_icon"))
                {
                    var enchantmentName = foldername;
                    if (enchantmentName.EndsWith("Shine")) continue;
                    if (!_enchantments.ContainsKey(enchantmentName))
                    {
                        _enchantments.Add(enchantmentName, fullPath);
                        //Handle exceptions
                        if (_mismatches.ContainsKey(enchantmentName))
                        {
                            var correctedEnchantmentName = _mismatches[enchantmentName];
                            _enchantments.Add(correctedEnchantmentName, fullPath);
                            EnchantmentDatabase.allEnchantments.Add(correctedEnchantmentName);
                        }
                        else
                        {
                            EnchantmentDatabase.allEnchantments.Add(enchantmentName);
                        }
                        //Debug.WriteLine($"{enchantmentName} - {fullPath}");
                    }
                    continue;
                }

                if (fullPathLowercase.EndsWith("_icon_inventory"))
                {
                    var itemName = foldername;
                    if (!_equipment.ContainsKey(itemName))
                    {
                        _equipment.Add(itemName, fullPath);
                        if (isItemValid(fullPath))
                        {
                            ItemDatabase.all.Add(itemName);
                        }
                        //Debug.WriteLine($"{itemName} - {fullPath}");
                    }

                    if (fullPath.Contains("Equipment") && isItemValid(fullPath))
                    {
                        //Handle exceptions
                        string? correctedItemName = null;
                        if (_mismatches.ContainsKey(itemName))
                        {
                            correctedItemName = _mismatches[itemName];
                            _equipment.Add(correctedItemName, fullPath);
                        }

                        if (fullPath.Contains("MeleeWeapons"))
                        {
                            ItemDatabase.meleeWeapons.Add(correctedItemName ?? itemName);
                        }
                        if (fullPath.Contains("RangedWeapons"))
                        {
                            ItemDatabase.rangedWeapons.Add(correctedItemName ?? itemName);
                        }
                        if (fullPath.Contains("Armor"))
                        {
                            ItemDatabase.armor.Add(correctedItemName ?? itemName);
                        }
                    }
                    
                    if (fullPath.Contains("Items") && isItemValid(fullPath))
                    {
                        //Handle exceptions
                        string? correctedItemName = null;
                        if (_mismatches.ContainsKey(itemName))
                        {
                            correctedItemName = _mismatches[itemName];
                            _equipment.Add(correctedItemName, fullPath);
                        }

                        ItemDatabase.artifacts.Add(correctedItemName ?? itemName);
                    }
                }
            }

            Debug.WriteLine($"Found {_localizations.Count()} localizations");
            Debug.WriteLine($"Found {_levels.Count()} levels");
            Debug.WriteLine($"Found {ItemDatabase.armorProperties.Count()} armor properties");
            Debug.WriteLine($"Loaded {_equipment.Count()} equipment images");
            Debug.WriteLine($"Loaded {_enchantments.Count()} enchantment images");
            if (preloadBitmaps)
            {
                Debug.WriteLine($"Preloaded {_bitmaps.Count()} bitmaps");
            }
        }

        private bool isItemValid(string fullPath)
        {
            return !_blockedItems.Any(fullPath.Contains);
        }

        public Dictionary<string, Dictionary<string, string>>? loadLanguageStrings(string langSpecifier)
        {
            if(!_localizations.TryGetValue(langSpecifier, out string fullPath))
            {
                return null;
            }

            var defaultStringLibrary = _pakIndex.extractLocResFile(_localizations[Constants.DEFAULT_LANG_SPECIFIER]);
            if(langSpecifier == Constants.DEFAULT_LANG_SPECIFIER)
            {
                return defaultStringLibrary;
            }

            var stringLibrary = _pakIndex.extractLocResFile(fullPath);
            if (stringLibrary != null)
            {
                long totalStringCount = stringLibrary.Sum(pair => pair.Value.LongCount());
                Debug.WriteLine($"Loaded {totalStringCount} strings from {langSpecifier} LocRes");

                //Fill in any missing strings using the defaultStringLibrary
                // i.e. fr-FR is missing "Glaive", "Claymore", and "Katana"
                foreach(var pair in stringLibrary.ToArray())
                {
                    var defaultValue = defaultStringLibrary![pair.Key];
                    var newValue = pair.Value.concatMissingFrom(defaultValue);
                    stringLibrary[pair.Key] = newValue;
                }
            }
            return stringLibrary;
        }

        public BitmapImage? imageSource(string pathWithoutExtension)
        {
            var path = pathWithoutExtension;
            if (!_bitmaps.ContainsKey(path))
            {
                var bitmap = _pakIndex.extractBitmap(path);
                if (bitmap == null) return null;
                _bitmaps[path] = bitmap!;
            }
            return _bitmaps[path];
        }

        public BitmapImage? imageSourceForItem(Item item)
        {
            return imageSourceForItem(item.Type);
        }

        public BitmapImage? imageSourceForItem(string itemType) {
            if (_equipment.TryGetValue(itemType, out string fullPath))
            {
                var image = imageSource(fullPath);
                if (image != null)
                {
                    return image;
                }
            }
            EventLogger.logError($"Could not find full path for item {itemType}");
            return _backupResolver.imageSourceForItem(itemType);
        }

        public BitmapImage? imageSourceForRarity(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/inspector/loot/drops_gear_frame");
                case Rarity.Rare: return imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/inspector/loot/drops_rare_frame");
                case Rarity.Unique: return imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/inspector/loot/drops_unique_frame");
            }
            return _backupResolver.imageSourceForRarity(rarity);
        }

        public BitmapImage? imageSourceForEnchantment(Enchantment enchantment)
        {
            return imageSourceForEnchantment(enchantment.Id);
        }

        public BitmapImage? imageSourceForEnchantment(string enchantment)
        {
            var enchantmentId = enchantment;
            if (enchantmentId == Constants.DEFAULT_ENCHANTMENT_ID)
            {
                return imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/locked_node");
            }

            if (_enchantments.TryGetValue(enchantmentId, out string fullPath))
            {
                var image = imageSource(fullPath);
                if (image != null)
                {
                    return image;
                }
            }
            EventLogger.logError($"Could not find full path for enchantment {enchantmentId}");
            return _backupResolver.imageSourceForEnchantment(enchantmentId);
        }
    }
}
