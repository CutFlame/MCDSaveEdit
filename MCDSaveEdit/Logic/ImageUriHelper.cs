using DungeonTools.Save.Models.Enums;
using DungeonTools.Save.Models.Profiles;
using FModel;
using PakReader.Pak;
using PakReader.Parsers.Class;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Cache;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public interface IImageResolver
    {
        BitmapImage? imageSourceForItem(Item item);
        BitmapImage? imageSourceForRarity(Rarity rarity);
        BitmapImage? imageSourceForEnchantment(EnchantmentType enchantmentType);
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
            return imageSourceForItemType(item.type());
        }

        public BitmapImage? imageSourceForItemType(ItemTypeEnum itemType)
        {
            var itemTypeStr = folderNameForItemType(itemType);
            var filename = string.Format("{0}.png", itemTypeStr);
            var path = Path.Combine(IMAGES_URI, filename);
            var uri = new Uri(path);
            return tryBitmapImageForUri(uri);
        }

        private string folderNameForItemType(ItemTypeEnum type)
        {
            if (type.isArtifact())
            {
                return "Artifacts";
            }
            if (type.isArmor())
            {
                return "Armor";
            }
            if (type.isMeleeWeapon())
            {
                return "MeleeWeapons";
            }
            if (type.isRangedWeapon())
            {
                return "RangedWeapons";
            }

            return "Unknown";
        }

        private string imageNameForItem(Item item)
        {
            var itemName = item.type().ToString();
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

        public BitmapImage? imageSourceForEnchantment(EnchantmentType enchantmentType)
        {
            if (enchantmentType == EnchantmentType.Unset)
            {
                return null;
            }
            //var filename = imageNameFromEnchantment(enchantmentType);
            var filename = "Enchantments.png";
            var path = Path.Combine(IMAGES_URI, filename);
            var uri = new Uri(path);
            return tryBitmapImageForUri(uri);
        }

        private string imageNameFromEnchantment(EnchantmentType enchantment)
        {
            if (enchantment == EnchantmentType.Unset)
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
            return imageSourceForItemType(item.type());
        }

        public BitmapImage? imageSourceForItemType(ItemTypeEnum itemType)
        {
            var name = itemType.ToString();
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

        private string? folderPathForItemType(ItemTypeEnum type)
        {
            //handle exceptions first
            if (type == ItemTypeEnum.IronHideAmulet)
            {
                return Path.Combine(BasePathToItemImages, "Items", "Amulets");
            }

            var basePath = type.isInPatch1() ? Patch1PathToItemImages : BasePathToItemImages;
            if (type.isArtifact())
            {
                return Path.Combine(basePath, "Items");
            }
            if (type.isArmor())
            {
                return Path.Combine(basePath, "Equipment", "Armor");
            }
            if (type.isMeleeWeapon())
            {
                return Path.Combine(basePath, "Equipment", "MeleeWeapons");
            }
            if (type.isRangedWeapon())
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
        public BitmapImage? imageSourceForEnchantment(EnchantmentType enchantmentType)
        {
            if(enchantmentType == EnchantmentType.Unset)
            {
                return imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/locked_node");
            }
            
            var name = enchantmentType.ToString();
            var filename = string.Format("T_{0}_Icon", name);
            var path = Path.Combine(PathToEnchantmentImages, name, filename).Replace('\\', '/');
            var fullPath = "/" + path;
            return imageSource(fullPath) ?? _backupResolver.imageSourceForEnchantment(enchantmentType);
        }
    }

    public static class ImageUriHelper
    {
        public static IImageResolver instance = new LocalImageResolver();

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
