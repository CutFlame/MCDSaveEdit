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

namespace MCDSaveEdit
{
    public class PakImageResolver : IImageResolver
    {
        private static readonly Dictionary<string, string> _mismatches = new Dictionary<string, string>() {
            //{"Imagefile foldername","Savefile reference"},
            { "TrickBow","Trickbow" },
            { "TrickBow_Unique1","Trickbow_Unique1" },
            { "TrickBow_Unique2","Trickbow_Unique2" },
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
            { "Huntingbow_Unique1","HuntingBow_Unique1" },
            { "TwistingVineBow_UNique1","TwistingVineBow_Unique1" },

            { "Battlerobe_unique1","BattleRobe_Unique1" },

            { "Sword_Steel","Sword" },
            { "Pickaxe_Steel","Pickaxe" },
            { "Pickaxe_Unique1_Steel","Pickaxe_Unique1" },
            { "Daggers_unique2","Daggers_Unique2" },

            { "Beenest","BeeNest" },

            { "Firetrail","FireTrail" },
        };

        private readonly LocalImageResolver _backupResolver;
        private readonly PakIndex _pakIndex;
        private readonly Dictionary<string, string> _enchantments = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _equipment = new Dictionary<string, string>();
        private readonly Dictionary<string, BitmapImage> _bitmaps = new Dictionary<string, BitmapImage>();
        private readonly List<string> _levels = new List<string>();
        public string? path { get; private set; }

        public PakImageResolver(PakIndex pakIndex, string? path)
        {
            this.path = path;
            _pakIndex = pakIndex;
            _backupResolver = new LocalImageResolver();
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
                    //Handle english language strings
                    if (lang == "en")
                    {
                        var stringLibrary = _pakIndex.extractLocResFile(fullPath);
                        if (stringLibrary != null)
                        {
                            R.loadExternalStrings(stringLibrary);
                            long totalStringCount = stringLibrary.Sum(pair => pair.Value.LongCount());
                            Debug.WriteLine($"Loaded {totalStringCount} {lang} LocRes");
                        }
                    }
                    continue;
                }

                if (fullPath.Contains("ArmorProperties") && !fullPath.Contains("Cues"))
                {
                    //Get the folder name because it will be the name of the armor property
                    string armorProperty = Path.GetDirectoryName(fullPath).Split(Path.DirectorySeparatorChar).Last();
                    if(armorProperty == "ArmorProperties" || armorProperty == "ReviveChance") { continue; } //Exception
                    ItemExtensions.armorProperties.Add(armorProperty);
                    //Debug.WriteLine($"Found ArmorProperty {armorProperty} in: {fullPath}");
                    continue;
                }

                var filename = Path.GetFileName(fullPath);

                if (fullPath.Contains("data") && fullPath.Contains("levels"))
                {
                    _levels.Add(filename);
                    //Debug.WriteLine($"Found level {filename}");
                    continue;
                }

                if (!filename.StartsWith("T") || !fullPath.Contains("_Icon"))
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
                if (fullPath.Contains("Enchantments") && fullPath.EndsWith("_Icon"))
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
                            EnchantmentExtensions.allEnchantments.Add(correctedEnchantmentName);
                        }
                        else
                        {
                            EnchantmentExtensions.allEnchantments.Add(enchantmentName);
                        }
                        //Debug.WriteLine($"{enchantmentName} - {fullPath}");
                    }
                    continue;
                }

                if (fullPath.EndsWith("_Icon_inventory") || fullPath.EndsWith("_Icon_Inventory"))
                {
                    var itemName = foldername;
                    if (!_equipment.ContainsKey(itemName))
                    {
                        _equipment.Add(itemName, fullPath);
                        if (!itemName.StartsWith("MysteryBox"))
                        {
                            ItemExtensions.all.Add(itemName);
                        }
                        //Debug.WriteLine($"{itemName} - {fullPath}");
                    }

                    if (fullPath.Contains("Equipment") && fullPath.Contains("MeleeWeapons"))
                    {
                        //Handle exceptions
                        if (_mismatches.ContainsKey(itemName))
                        {
                            var correctedItemName = _mismatches[itemName];
                            _equipment.Add(correctedItemName, fullPath);
                            ItemExtensions.meleeWeapons.Add(correctedItemName);
                        } else
                        {
                            ItemExtensions.meleeWeapons.Add(itemName);
                        }
                    }
                    if (fullPath.Contains("Equipment") && fullPath.Contains("RangedWeapons"))
                    {
                        //Handle exceptions
                        if (_mismatches.ContainsKey(itemName))
                        {
                            var correctedItemName = _mismatches[itemName];
                            _equipment.Add(correctedItemName, fullPath);
                            ItemExtensions.rangedWeapons.Add(correctedItemName);
                        } else
                        {
                            ItemExtensions.rangedWeapons.Add(itemName);
                        }
                    }
                    if (fullPath.Contains("Equipment") && fullPath.Contains("Armor"))
                    {
                        //Handle exceptions
                        if (_mismatches.ContainsKey(itemName))
                        {
                            var correctedItemName = _mismatches[itemName];
                            _equipment.Add(correctedItemName, fullPath);
                            ItemExtensions.armor.Add(correctedItemName);
                        } else
                        {
                            ItemExtensions.armor.Add(itemName);
                        }
                    }
                    if (fullPath.Contains("Items"))
                    {
                        if (!itemName.StartsWith("MysteryBox"))
                        {
                            if (_mismatches.ContainsKey(itemName))
                            {
                                var correctedItemName = _mismatches[itemName];
                                _equipment.Add(correctedItemName, fullPath);
                                ItemExtensions.artifacts.Add(correctedItemName);
                            }
                            else
                            {
                                ItemExtensions.artifacts.Add(itemName);
                            }
                        }
                    }
                }
            }

            Debug.WriteLine($"Found {_levels.Count()} levels");
            Debug.WriteLine($"Found {ItemExtensions.armorProperties.Count()} armor properties");
            Debug.WriteLine($"Loaded {_equipment.Count()} equipment images");
            Debug.WriteLine($"Loaded {_enchantments.Count()} enchantment images");
            if (preloadBitmaps)
            {
                Debug.WriteLine($"Preloaded {_bitmaps.Count()} bitmaps");
            }
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
