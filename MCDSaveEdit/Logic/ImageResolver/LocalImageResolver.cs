using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Cache;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public class LocalImageResolver: IImageResolver
    {
        //private const string ASSETS_URI = @"https://img.rankedboost.com/wp-content/plugins/minecraft-dungeons/assets/";
        private const string IMAGES_URI = @"pack://application:,,/Images";
        private readonly RequestCachePolicy REQUEST_CACHE_POLICY = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);

        public string? path { get { return null; } }
        public IReadOnlyCollection<string> localizationOptions { get { return new List<string>(); } }

        public Dictionary<string, Dictionary<string, string>>? loadLanguageStrings(string langSpecifier) { return null; }

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
                EventLogger.logError($"Error creating bitmap for {uri}");
                Console.Write(e);
                //Debug.Assert(false);
                return null;
            }
        }

        #region Items

        public BitmapImage? imageSourceForItem(Item item)
        {
            return imageSourceForItem(item.Type);
        }

        public BitmapImage? imageSourceForItem(string itemType)
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
            var enchantmentId = enchantment;
            if (enchantmentId == Constants.DEFAULT_ENCHANTMENT_ID)
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
}
