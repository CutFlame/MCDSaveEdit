using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for ItemControl.xaml
    /// </summary>
    public partial class ItemControl : UserControl
    {
        private static readonly BitmapImage? enchantmentPointsImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Mobs/enchant_common_icon");
        private static readonly BitmapImage? gildedImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/Content_DLC4/UI/Materials/Inventory/Inventory_slot_gilded_plate");
        private static readonly BitmapImage? markedNewImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/HotBar2/Icons/inventoryslot_newitem");

        public static void preload()
        {
            ImageUriHelper.instance.imageSourceForRarity(Rarity.Common);
            ImageUriHelper.instance.imageSourceForRarity(Rarity.Rare);
            ImageUriHelper.instance.imageSourceForRarity(Rarity.Unique);
        }

        public ItemControl()
        {
            InitializeComponent();
            if (ImageUriHelper.gameContentLoaded)
            {
                useGameContentImages();
            }

            //Clear out design/testing values
            updateUI();
        }

        private void useGameContentImages()
        {
            enchantmentPointsImage.Source = enchantmentPointsImageSource;
            gildedImage.Source = gildedImageSource;
            markedNewImage.Source = markedNewImageSource;
        }

        private Item? _item;
        public Item? item
        {
            get { return _item; }
            set { _item = value; updateUI(); }
        }

        public void clearAll()
        {
            image.Source = null;
            backImage.Source = null;
            gildedImage.Visibility = Visibility.Hidden;
            markedNewImage.Visibility = Visibility.Hidden;
            inventoryIndexLabel.Content = null;
            powerLabel.Content = null;
            titleLabel.Content = null;
            titleLabel.Visibility = Visibility.Hidden;
            enchantmentPointsImage.Visibility = Visibility.Hidden;
            enchantmentPointsLabel.Visibility = Visibility.Hidden;
        }

        public void updateUI()
        {
            if(_item == null)
            {
                clearAll();
                return;
            }

            powerLabel.Content = _item.level();
            titleLabel.Content = R.itemName(_item.Type);
            image.Source = ImageUriHelper.instance.imageSourceForItem(_item);
            if (image.Source == null || !ImageUriHelper.gameContentLoaded)
            {
                titleLabel.Visibility = Visibility.Visible;
            }
            else
            {
                titleLabel.Visibility = Visibility.Hidden;
            }

            backImage.Source = ImageUriHelper.instance.imageSourceForRarity(_item.Rarity);

            if(_item.NetheriteEnchant != null)
            {
                gildedImage.Visibility = Visibility.Visible;
            }
            else
            {
                gildedImage.Visibility = Visibility.Hidden;
            }

            markedNewImage.Visibility = _item.MarkedNew == true ? Visibility.Visible : Visibility.Hidden;

            if (Config.instance.showInventoryIndexOrEquipmentSlot)
            {
                inventoryIndexLabel.Content = _item.InventoryIndex?.ToString() ?? _item.EquipmentSlot;
            }

            var enchantmentPoints = _item.enchantmentPoints();
            if (enchantmentPoints > 0)
            {
                enchantmentPointsImage.Visibility = Visibility.Visible;
                enchantmentPointsLabel.Visibility = Visibility.Visible;
                enchantmentPointsLabel.Content = enchantmentPoints;
            }
            else
            {
                enchantmentPointsImage.Visibility = Visibility.Hidden;
                enchantmentPointsLabel.Visibility = Visibility.Hidden;
            }
        }
    }
}
