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
                            Debug.WriteLine($"Loaded {lang} LocRes");
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

                if (fullPath.Contains("Enchantments") && fullPath.EndsWith("_Icon"))
                {
                    var enchantmentName = string.Join("", filename.Skip(2).Take(filename.Length - 7));
                    if (enchantmentName.EndsWith("Shine")) continue;
                    if(!_enchantments.ContainsKey(enchantmentName))
                    {
                        _enchantments.Add(enchantmentName, fullPath);
                        EnchantmentExtensions.allEnchantments.Add(enchantmentName);
                        //Debug.WriteLine($"{enchantmentName} - {fullPath}");
                    }
                    continue;
                }

                if (fullPath.EndsWith("_Icon_inventory"))
                {
                    var itemName = string.Join("", filename.Skip(2).Take(filename.Length - 17));
                    if(!_equipment.ContainsKey(itemName))
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
                        if (itemName == "Sword_Steel")
                        {
                            _equipment.Add("Sword", fullPath);
                            ItemExtensions.meleeWeapons.Add("Sword");
                        }
                        if (itemName == "PickAxe_Steel")
                        {
                            _equipment.Add("Pickaxe", fullPath);
                            ItemExtensions.meleeWeapons.Add("Pickaxe");
                        }
                        if (itemName == "Pickaxe_Unique1_Steel")
                        {
                            _equipment.Add("Pickaxe_Unique1", fullPath);
                            ItemExtensions.meleeWeapons.Add("Pickaxe_Unique1");
                        }
                        ItemExtensions.meleeWeapons.Add(itemName);
                    }
                    if (fullPath.Contains("Equipment") && fullPath.Contains("RangedWeapons"))
                    {
                        //Handle exceptions
                        if (itemName == "TrickBow")
                        {
                            _equipment.Add("Trickbow", fullPath);
                            ItemExtensions.rangedWeapons.Add("TrickBow");
                        }
                        if (itemName == "LongBow")
                        {
                            _equipment.Add("Longbow", fullPath);
                            ItemExtensions.rangedWeapons.Add("Longbow");
                        }
                        if (itemName == "PowerBow")
                        {
                            _equipment.Add("Powerbow", fullPath);
                            ItemExtensions.rangedWeapons.Add("Powerbow");
                        }
                        if (itemName == "ShortBow")
                        {
                            _equipment.Add("Shortbow", fullPath);
                            ItemExtensions.rangedWeapons.Add("Shortbow");
                        }
                        if (itemName == "LongBow_Unique2")
                        {
                            _equipment.Add("Longbow_Unique2", fullPath);
                            ItemExtensions.rangedWeapons.Add("Longbow_Unique2");
                        }
                        if (itemName == "ShortBow_Unique2")
                        {
                            _equipment.Add("Shortbow_Unique2", fullPath);
                            ItemExtensions.rangedWeapons.Add("Shortbow_Unique2");
                        }
                        if (itemName == "Huntingbow_Unique1")
                        {
                            _equipment.Add("HuntingBow_Unique1", fullPath);
                            ItemExtensions.rangedWeapons.Add("HuntingBow_Unique1");
                        }
                        ItemExtensions.rangedWeapons.Add(itemName);
                    }
                    if (fullPath.Contains("Equipment") && fullPath.Contains("Armor"))
                    {
                        //Handle exceptions
                        if (itemName == "Battlerobe_unique1")
                        {
                            _equipment.Add("BattleRobe_Unique1", fullPath);
                            ItemExtensions.armor.Add("BattleRobe_Unique1");
                        }
                        ItemExtensions.armor.Add(itemName);
                    }
                    if (fullPath.Contains("Items"))
                    {
                        if (!itemName.StartsWith("MysteryBox"))
                        {
                            ItemExtensions.artifacts.Add(itemName);
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
                Debug.WriteLine($"Could not get package from {fullPath}");
                return null;
            }
            if (!package.HasExport())
            {
                Debug.WriteLine($"Package does not have export {fullPath}");
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
                Debug.WriteLine($"Could not get texture from package {fullPath}");
                return null;
            }
            var bitmap = BitmapImageFromSKImage(texture.Image);
            if (bitmap == null)
            {
                Debug.WriteLine($"Could not get bitmap from texture {fullPath}");
                return null;
            }
            return bitmap;
        }

        private Dictionary<string, Dictionary<string, string>>? extractLocResFile(string fullPath)
        {
            if (!_pakIndex.TryGetFile(fullPath, out var byteArray) || byteArray == null)
            {
                Debug.WriteLine($"Could not get anything from {fullPath}");
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
            Debug.WriteLine($"Could not find full path for item {itemType}");
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
            Debug.WriteLine($"Could not find full path for enchantment {enchantmentId}");
            return _backupResolver.imageSourceForEnchantment(enchantmentId);
        }
    }
}
