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
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public interface IImageResolver
    {
        BitmapImage? imageSourceForItem(Item item);
        BitmapImage? imageSourceForRarity(Rarity rarity);
        BitmapImage? imageSourceForEnchantment(Enchantment enchantment);
        BitmapImage? imageSourceForEnchantment(string enchantmentType);
        BitmapImage? imageSource(string path);
    }

    public class LocalImageResolver: IImageResolver
    {
        //private const string ASSETS_URI = @"https://img.rankedboost.com/wp-content/plugins/minecraft-dungeons/assets/";
        private const string IMAGES_URI = @"pack://application:,,/Images";
        private readonly RequestCachePolicy REQUEST_CACHE_POLICY = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);

        public BitmapImage? imageSource(string path)
        {
            return null;
        }

        private BitmapImage? tryBitmapImageForUri(Uri uri)
        {
            try
            {
                //Console.WriteLine("Requesting Uri: {0}", uri);
                var image = new BitmapImage(uri, REQUEST_CACHE_POLICY);
                return image;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating bitmap for {0}", uri);
                Console.Write(e);
                //Debug.Assert(false);
                return null;
            }
        }

        #region Items

        public BitmapImage? imageSourceForItem(Item item)
        {
            return imageSourceForItemType(item.Type);
        }

        public BitmapImage? imageSourceForItemType(string itemType)
        {
            var itemTypeStr = folderNameForItemType(itemType);
            var filename = string.Format("{0}.png", itemTypeStr);
            var path = Path.Combine(IMAGES_URI, filename);
            var uri = new Uri(path);
            return tryBitmapImageForUri(uri);
        }

        private string folderNameForItemType(string type)
        {
            if (ItemExtensions.artifacts.Contains(type))
            {
                return "Artifacts";
            }
            if (ItemExtensions.armor.Contains(type))
            {
                return "Armor";
            }
            if (ItemExtensions.meleeWeapons.Contains(type))
            {
                return "MeleeWeapons";
            }
            if (ItemExtensions.rangedWeapons.Contains(type))
            {
                return "RangedWeapons";
            }

            return "Unknown";
        }

        private string imageNameForItem(Item item)
        {
            var itemName = item.Type;
            //var encodedString = Uri.EscapeDataString(stringFromItemName(itemName));
            return string.Format("T_{0}_Icon_inventory.png", itemName);
        }

        #endregion

        #region Raritys

        public BitmapImage? imageSourceForRarity(Rarity rarity)
        {
            return null;
            //var filename = imageNameFromRarity(rarity);
            //if(filename == null) { return null; }
            //var path = Path.Combine(IMAGES_URI, "UI", "ItemRarity", filename);
            //var uri = new Uri(path);
            //return tryBitmapImageForUri(uri);
        }

        private string? imageNameFromRarity(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return "drops_items_frame.png";
                case Rarity.Rare: return "drops_rare_frame.png";
                case Rarity.Unique: return "drops_unique_frame.png";
            }
            throw new NotImplementedException();
        }

        #endregion

        #region Enchantments

        public BitmapImage? imageSourceForEnchantment(Enchantment enchantment)
        {
            return imageSourceForEnchantment(enchantment.Id);
        }

        public BitmapImage? imageSourceForEnchantment(string enchantment)
        {
            var enchantmentId = enchantment.ToLowerInvariant();
            if (enchantmentId == "unset")
            {
                return null;
            }
            var filename = "Enchantments.png";
            var path = Path.Combine(IMAGES_URI, filename);
            var uri = new Uri(path);
            return tryBitmapImageForUri(uri);
        }

        #endregion

    }

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

                if (!fullPath.Contains("_icon"))
                {
                    //Debug.WriteLine($"Package is not an icon {fullPath}");
                    continue;
                }
                var filename = Path.GetFileName(fullPath);
                if (!filename.StartsWith("t"))
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

                if (fullPath.Contains("enchantments") && fullPath.EndsWith("_icon"))
                {
                    var enchantmentName = string.Join("", filename.Skip(2).Take(filename.Length - 7));
                    if(!_enchantments.ContainsKey(enchantmentName))
                    {
                        _enchantments.Add(enchantmentName, fullPath);
                        //Debug.WriteLine($"{enchantmentName} - {fullPath}");
                    }
                    continue;
                }

                if (fullPath.EndsWith("_icon_inventory"))
                {
                    var itemName = string.Join("", filename.Skip(2).Take(filename.Length - 17));
                    if(!_equipment.ContainsKey(itemName))
                    {
                        _equipment.Add(itemName, fullPath);
                        //Debug.WriteLine($"{itemName} - {fullPath}");
                    }

                    if (fullPath.Contains("equipment") && fullPath.Contains("meleeweapons"))
                    {
                        //Handle exceptions
                        if (itemName == "sword_steel")
                        {
                            _equipment.Add("sword", fullPath);
                            ItemExtensions.meleeWeapons.Add("sword");
                        }
                        if (itemName == "pickaxe_steel")
                        {
                            _equipment.Add("pickaxe", fullPath);
                            ItemExtensions.meleeWeapons.Add("pickaxe");
                        }
                        if (itemName == "pickaxe_unique1_steel")
                        {
                            _equipment.Add("pickaxe_unique1", fullPath);
                            ItemExtensions.meleeWeapons.Add("pickaxe_unique1");
                        }
                        ItemExtensions.meleeWeapons.Add(itemName);
                    }
                    if (fullPath.Contains("equipment") && fullPath.Contains("rangedweapons"))
                    {
                        ItemExtensions.rangedWeapons.Add(itemName);
                    }
                    if (fullPath.Contains("equipment") && fullPath.Contains("armor"))
                    {
                        ItemExtensions.armor.Add(itemName);
                    }
                    if (fullPath.Contains("items"))
                    {
                        ItemExtensions.artifacts.Add(itemName);
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
            var path = pathWithoutExtension.ToLowerInvariant();
            if (!_bitmaps.ContainsKey(path))
            {
                var bitmap = extractBitmap(path);
                if(bitmap == null) return null;
                _bitmaps[path] = bitmap!;
            }
            return _bitmaps[path];
        }

        private BitmapImage? extractBitmap(string fullPath)
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
            var texture = package.GetExport<UTexture2D>();
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
            var itemType = item.Type.ToLowerInvariant();
            if (_equipment.TryGetValue(itemType, out string fullPath))
            {
                var image = imageSource(fullPath);
                if (image != null)
                {
                    return image;
                }
            }
            Debug.WriteLine($"Could not find full path for item {itemType}");
            return _backupResolver.imageSourceForItemType(itemType);
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
            var enchantmentId = enchantment.ToLowerInvariant();
            if (enchantmentId.ToLowerInvariant() == "unset")
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

    public static class ImageUriHelper
    {
        public static IImageResolver instance = new LocalImageResolver();

        public static bool canUseGameContent()
        {
            return Directory.Exists(Constants.PAKS_FOLDER);
        }

        public static bool gameContentLoaded { get; private set; } = false;
        public static async Task loadGameContentAsync()
        {
            var pakIndex = await loadPakIndex();
            if (pakIndex != null)
            {
                var pakImageResolver = new PakImageResolver(pakIndex);
                await pakImageResolver.loadPakFilesAsync(preloadBitmaps: false);
                instance = pakImageResolver;
                gameContentLoaded = true;
            }
        }

        private static Task<PakIndex?> loadPakIndex()
        {
            var tcs = new TaskCompletionSource<PakIndex?>();
            Task.Run(() =>
            {
                try
                {
                    var filter = new PakFilter(new[] { Constants.PAKS_FILTER_STRING }, false);
                    var pakIndex = new PakIndex(path: Constants.PAKS_FOLDER, cacheFiles: true, caseSensitive: false, filter: filter);
                    pakIndex.UseKey(BinaryHelper.ToBytesKey(Secrets.PAKS_AES_KEY_STRING));
                    tcs.SetResult(pakIndex);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not load Minecraft Dungeons Paks: {e}");
                    tcs.SetResult(null);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Load a resource WPF-BitmapImage (png, bmp, ...) from embedded resource defined as 'Resource' not as 'Embedded resource'.
        /// </summary>
        /// <param name="pathInApplication">Path without starting slash</param>
        private static BitmapImage loadBitmapFromResource(string pathInApplication)
        {
            if (pathInApplication[0] == '/')
            {
                pathInApplication = pathInApplication.Substring(1);
            }
            var rootPath = @"pack://application:,,/";
            var fullPath = Path.Combine(rootPath, pathInApplication);
            var uri = new Uri(fullPath, UriKind.RelativeOrAbsolute);
            Console.WriteLine(uri);
            return new BitmapImage(uri);
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
