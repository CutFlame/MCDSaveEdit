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

        public BitmapImage? imageSourceForEnchantment(string enchantmentType)
        {
            if (enchantmentType == "Unset")
            {
                return null;
            }
            //var filename = imageNameFromEnchantment(enchantmentType);
            var filename = "Enchantments.png";
            var path = Path.Combine(IMAGES_URI, filename);
            var uri = new Uri(path);
            return tryBitmapImageForUri(uri);
        }

        private string imageNameFromEnchantment(string enchantment)
        {
            if (enchantment == "Unset")
            {
                return string.Empty;
            }
            //var encodedString = Uri.EscapeDataString(stringFromEnchantment(enchantment));
            //return string.Format("{0}.png", encodedString);
            return string.Format("T_{0}_Icon.png", enchantment.ToString());
        }

        //public string stringFromEnchantment(EnchantmentType enchantment)
        //{
        //    return spaceOutWords(enchantment.ToString());
        //}

        #endregion

    }

    public class PakImageResolver: IImageResolver
    {
        private LocalImageResolver _backupResolver;
        private PakIndex _pakIndex;
        private Dictionary<string, string> _enchantments = new Dictionary<string, string>();
        private Dictionary<string, string> _artifacts = new Dictionary<string, string>();
        private Dictionary<string, string> _meleeweapons = new Dictionary<string, string>();
        private Dictionary<string, string> _rangedweapons = new Dictionary<string, string>();
        private Dictionary<string, string> _armor = new Dictionary<string, string>();
        private Dictionary<string, string> _equipment = new Dictionary<string, string>();

        public PakImageResolver(PakIndex pakIndex)
        {
            _pakIndex = pakIndex;
            _backupResolver = new LocalImageResolver();

            foreach (var item in _pakIndex)
            {
                if (item == null) continue;
                var fullPath = item!.Replace('\\', '/').Replace("//", "/");
                if (fullPath.Contains("enchantments") && fullPath.EndsWith("_icon"))
                {
                    var filename = Path.GetFileName(fullPath);
                    if (!filename.StartsWith("t")) continue;
                    var enchantmentName = string.Join("", filename.Skip(2).Take(filename.Length - 7));
                    if(!_enchantments.ContainsKey(enchantmentName))
                    {
                        _enchantments.Add(enchantmentName, fullPath);
                    }
                    continue;
                }
                if (fullPath.EndsWith("_icon_inventory"))
                {
                    var filename = Path.GetFileName(fullPath);
                    if (!filename.StartsWith("t")) continue;
                    var itemName = string.Join("", filename.Skip(2).Take(filename.Length - 17));
                    if(!_equipment.ContainsKey(itemName))
                    {
                        _equipment.Add(itemName, fullPath);
                    }

                    if (fullPath.Contains("equipment") && fullPath.Contains("meleeweapons"))
                    {
                        if (itemName == "sword_steel")
                        {
                            _equipment.Add("sword", fullPath);
                            _meleeweapons.Add("sword", fullPath);
                        }
                        if (itemName == "pickaxe_steel")
                        {
                            _equipment.Add("pickaxe", fullPath);
                            _meleeweapons.Add("pickaxe", fullPath);
                        }
                        if (itemName == "pickaxe_unique1_steel")
                        {
                            _equipment.Add("pickaxe_unique1", fullPath);
                            _meleeweapons.Add("pickaxe_unique1", fullPath);
                        }
                        _meleeweapons.Add(itemName, fullPath);
                    }
                    if (fullPath.Contains("equipment") && fullPath.Contains("rangedweapons"))
                    {
                        _rangedweapons.Add(itemName, fullPath);
                    }
                    if (fullPath.Contains("equipment") && fullPath.Contains("armor"))
                    {
                        _armor.Add(itemName, fullPath);
                    }
                    if (fullPath.Contains("items"))
                    {
                        _artifacts.Add(itemName, fullPath);
                    }
                }
            }
        }

        private readonly Dictionary<string, BitmapImage> bitmaps = new Dictionary<string, BitmapImage>();
        public BitmapImage? imageSource(string pathWithoutExtension)
        {
            var path = pathWithoutExtension.ToLowerInvariant();
            if (bitmaps.ContainsKey(path))
            {
                return bitmaps[path];
            }
            if (_pakIndex.TryGetPackage(path, out var package))
            {
                var texture = package.GetExport<UTexture2D>();
                return bitmaps[path] = BitmapImageFromSKImage(texture.Image);
            }
            return null;
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
            if (_equipment.TryGetValue(item.Type.ToLowerInvariant(), out string fullPath))
            {
                var image = imageSource(fullPath);
                if (image != null)
                {
                    return image;
                }
            }
            Debug.WriteLine($"Could not find full path for item {item.Type}");
            return _backupResolver.imageSourceForItemType(item.Type);
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
            if (enchantment == "Unset")
            {
                return imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/locked_node");
            }

            if (_enchantments.TryGetValue(enchantment.ToLowerInvariant(), out string fullPath))
            {
                var image = imageSource(fullPath);
                if (image != null)
                {
                    return image;
                }
            }
            Debug.WriteLine($"Could not find full path for item {enchantment}");
            return _backupResolver.imageSourceForEnchantment(enchantment);
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
                instance = new PakImageResolver(pakIndex);
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
                    var filter = new PakFilter(new[] { "/dungeons/content" });
                    var pakIndex = new PakIndex(path: Constants.PAKS_FOLDER, cacheFiles: true, caseSensitive: false, filter: filter);
                    pakIndex.UseKey(BinaryHelper.ToBytesKey(Constants.PAKS_AES_KEY_STRING));
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
