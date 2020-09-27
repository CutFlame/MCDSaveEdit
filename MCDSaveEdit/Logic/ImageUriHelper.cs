using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using PakReader;
using PakReader.Pak;
using PakReader.Parsers.Class;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
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
        public PakImageResolver(PakIndex pakIndex)
        {
            _pakIndex = pakIndex;
            _backupResolver = new LocalImageResolver();
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

        private static readonly string BasePathToItemImages = Path.Combine("Dungeons", "Content", "Actors");
        private static readonly string Patch1PathToItemImages = Path.Combine("Dungeons", "Content", "Patch1", "Actors");

        public BitmapImage? imageSourceForItem(Item item)
        {
            return imageSourceForItemType(item.Type);
        }

        public BitmapImage? imageSourceForItemType(string itemType)
        {
            var name = itemType;
            var folderPath = folderPathForItemType(itemType);
            if(folderPath == null)
            {
                return _backupResolver.imageSourceForItemType(itemType);
            }
            var filename = string.Format("T_{0}_Icon_inventory", name);
            var path = Path.Combine(folderPath!, name, filename).Replace('\\', '/');
            var fullPath = "/" + path;
            return imageSource(fullPath) ?? _backupResolver.imageSourceForItemType(itemType);
        }

        private string? folderPathForItemType(string type)
        {
            //handle exceptions first
            if (type == "IronHideAmulet")
            {
                return Path.Combine(BasePathToItemImages, "Items", "Amulets");
            }

            var basePath = ItemExtensions.patch1Items.Contains(type) ? Patch1PathToItemImages : BasePathToItemImages;
            if (ItemExtensions.artifacts.Contains(type))
            {
                return Path.Combine(basePath, "Items");
            }
            if (ItemExtensions.armor.Contains(type))
            {
                return Path.Combine(basePath, "Equipment", "Armor");
            }
            if (ItemExtensions.meleeWeapons.Contains(type))
            {
                return Path.Combine(basePath, "Equipment", "MeleeWeapons");
            }
            if (ItemExtensions.rangedWeapons.Contains(type))
            {
                return Path.Combine(basePath, "Equipment", "RangedWeapons");
            }

            return null;
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

        private static readonly string PathToEnchantmentImages = Path.Combine("Dungeons", "Content", "Components", "Enchantments");
        public BitmapImage? imageSourceForEnchantment(Enchantment enchantment)
        {
            return imageSourceForEnchantment(enchantment.Id);
        }

        public BitmapImage? imageSourceForEnchantment(string enchantment)
        {
            if(enchantment == "Unset")
            {
                return imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/locked_node");
            }
            
            var name = enchantment;
            var filename = string.Format("T_{0}_Icon", name);
            var path = Path.Combine(PathToEnchantmentImages, name, filename).Replace('\\', '/');
            var fullPath = "/" + path;
            return imageSource(fullPath) ?? _backupResolver.imageSourceForEnchantment(enchantment);
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
