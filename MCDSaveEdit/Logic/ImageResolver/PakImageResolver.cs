using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using PakReader;
using PakReader.Pak;
using PakReader.Parsers.Class;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public class PakImageResolver: IImageResolver
    {
        private static readonly Dictionary<string, string> _mismatches = new Dictionary<string, string>() {
            { "TrickBow","Trickbow" },
            { "LongBow","Longbow" },
            { "LongBow_Unique1","Longbow_Unique1" },
            { "LongBow_Unique2","Longbow_Unique2" },
            { "PowerBow","Powerbow" },
            { "PowerBow_Unique2","Powerbow_Unique2" },
            { "Slowbow_Unique1","SlowBow_Unique1" },
            { "ShortBow","Shortbow" },
            { "ShortBow_Unique1","Shortbow_Unique1" },
            { "ShortBow_Unique2","Shortbow_Unique2" },
            { "Huntingbow_Unique1","HuntingBow_Unique1" },

            { "Battlerobe_unique1","BattleRobe_Unique1" },

            { "Sword_Steel","Sword" },
            { "Pickaxe_Steel","Pickaxe" },
            { "Pickaxe_Unique1_Steel","Pickaxe_Unique1" },
            { "Daggers_unique2","Daggers_Unique2" },

            { "Beenest","BeeNest" },
        };

        private readonly LocalImageResolver _backupResolver;
        private readonly PakIndex _pakIndex;
        private readonly Dictionary<string, string> _enchantments = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _equipment = new Dictionary<string, string>();
        private readonly Dictionary<string, BitmapImage> _bitmaps = new Dictionary<string, BitmapImage>();

        public PakImageResolver(PakIndex pakIndex)
        {
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
            foreach (var item in _pakIndex)
            {
                if (item == null) continue;
                //Drop the mount point prefix
                var startIndex = item!.IndexOf("//") + 1;
                var fullPath = item!.Substring(startIndex);

                if (fullPath.Contains("Localization") && fullPath.EndsWith("Game"))
                {
                    var splitPath = fullPath.Split(new[] { "Game" }, StringSplitOptions.RemoveEmptyEntries);
                    string lang = splitPath[splitPath.Length - 1].Trim('/');
                    if(lang == "en")
                    {
                        var stringLibrary = extractLocResFile(fullPath);
                        if(stringLibrary != null)
                        {
                            R.loadExternalStrings(stringLibrary);
                            long totalStringCount = stringLibrary.Sum(pair => pair.Value.LongCount());
                            Debug.WriteLine($"Loaded {totalStringCount} {lang} LocRes");
                        }
                    }
                    continue;
                }

                var filename = Path.GetFileName(fullPath);
                if (!filename.StartsWith("T") || !fullPath.Contains("_Icon"))
                {
                    //Debug.WriteLine($"Package is not an icon {fullPath}");
                    continue;
                }

                if(preloadBitmaps)
                {
                    var bitmap = extractBitmap(fullPath);
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
                    if(!_enchantments.ContainsKey(enchantmentName))
                    {
                        _enchantments.Add(enchantmentName, fullPath);
                        EnchantmentExtensions.allEnchantments.Add(enchantmentName);
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
                        if(_mismatches.ContainsKey(itemName))
                        {
                            var correctedItemName = _mismatches[itemName];
                            _equipment.Add(correctedItemName, fullPath);
                            ItemExtensions.meleeWeapons.Add(correctedItemName);
                        }
                        else
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
                        }
                        else
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
                        }
                        else
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
                var bitmap = extractBitmap(path);
                if(bitmap == null) return null;
                _bitmaps[path] = bitmap!;
            }
            return _bitmaps[path];
        }

        private PakPackage? extractPackage(string fullPath)
        {
            if (!_pakIndex.TryGetPackage(fullPath, out var package))
            {
                EventLogger.logError($"Could not get package from {fullPath}");
                return null;
            }
            if (!package.HasExport())
            {
                EventLogger.logError($"Package does not have export {fullPath}");
                return null;
            }
            return package;
        }

        private BitmapImage? extractBitmap(string fullPath)
        {
            var package = extractPackage(fullPath);
            var texture = package?.GetExport<UTexture2D>();
            if (texture == null)
            {
                EventLogger.logError($"Could not get texture from package {fullPath}");
                return null;
            }
            var bitmap = BitmapImageFromSKImage(texture.Image);
            if (bitmap == null)
            {
                EventLogger.logError($"Could not get bitmap from texture {fullPath}");
                return null;
            }
            return bitmap;
        }

        private Dictionary<string, Dictionary<string, string>>? extractLocResFile(string fullPath)
        {
            if (!_pakIndex.TryGetFile(fullPath, out var byteArray) || byteArray == null)
            {
                EventLogger.logError($"Could not get anything from {fullPath}");
                return null;
            }
            var stream = new MemoryStream(byteArray!.Value.Array, byteArray!.Value.Offset, byteArray!.Value.Count);
            Dictionary<string, Dictionary<string, string>>? entries = new LocResReader(stream).Entries;
            return entries;
        }

        private static BitmapImage BitmapImageFromSKBitmap(SKBitmap image) => BitmapImageFromSKImage(SKImage.FromBitmap(image));
        private static BitmapImage BitmapImageFromSKImage(SKImage image)
        {
            using var encoded = image.Encode();
            using var stream = encoded.AsStream();
            BitmapImage photo = new BitmapImage();
            photo.BeginInit();
            photo.CacheOption = BitmapCacheOption.OnLoad;
            photo.StreamSource = stream;
            photo.EndInit();
            photo.Freeze();
            return photo;
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
                case Rarity.Common: return ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/inspector/loot/drops_gear_frame");
                case Rarity.Rare: return ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/inspector/loot/drops_rare_frame");
                case Rarity.Unique: return ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/inspector/loot/drops_unique_frame");
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
            if (enchantmentId == "Unset")
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
