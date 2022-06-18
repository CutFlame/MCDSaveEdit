using MCDSaveEdit.Logic;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using MCDSaveEdit.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit.UI
{
    /// <summary>
    /// Interaction logic for InventoryTab.xaml
    /// </summary>
    public partial class InventoryTab : UserControl
    {
        private static readonly BitmapImage? _emeraldImage = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Character/STATS_emerald");
        private static readonly BitmapImage? _goldImage = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Currency/GoldIndicator");
        private static readonly BitmapImage? _eyeOfEnderImage = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Currency/T_EyeOfEnder_Currency");
        private static readonly BitmapImage? _enchantmentImage = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/enchantscore_background");

        public static void preload() { }

        private ProfileViewModel? _model;
        public ProfileViewModel? model {
            get { return _model; }
            set {
                _model = value;
                var mainEquipmentModel = _model?.mainEquipmentModel;
                equipmentScreen.model = mainEquipmentModel;
                itemListScreen.model = mainEquipmentModel;
                setupCommands();
                //updateUI(); //Not Needed
            }
        }

        public InventoryTab()
        {
            InitializeComponent();

            if (AppModel.gameContentLoaded)
            {
                useGameContentImages();
            }

            //Clear out design/testing values
            updateUI();
        }

        public void deleteCurrentSelectedItem()
        {
            if (selectedItemScreen.item != null)
            {
                removeItem(selectedItemScreen.item!);
            }
        }

        private void useGameContentImages()
        {
            emeraldsLabelImage.Source = _emeraldImage;
            goldLabelImage.Source = _goldImage;
            eyeOfEnderLabelImage.Source = _eyeOfEnderImage;
            remainingEnchantmentPointsLabelImage.Source = _enchantmentImage;
        }

        private void setupCommands()
        {
            if (_model == null) { return; }
            var model = _model!;

            model.profile.subscribe(_ => this.updateUI());
            model.emeralds.subscribe(updateEmeraldsUI);
            model.gold.subscribe(updateGoldUI);
            model.eyeOfEnder.subscribe(updateEyeOfEnderUI);
            model.unlockPortal.subscribe(updateUnlockPortalUI);

            var equipmentModel = model.mainEquipmentModel;

            selectedItemScreen.itemLocation = ItemLocationEnum.Inventory;
            selectedItemScreen.selectEnchantment = new RelayCommand<Enchantment>(equipmentModel.selectEnchantment);
            selectedItemScreen.saveChanges = new RelayCommand<Item>(equipmentModel.saveItem);
            selectedItemScreen.duplicateItem = new RelayCommand<Item>(duplicateItem);
            selectedItemScreen.deleteItem = new RelayCommand<Item>(removeItem);
            selectedItemScreen.moveItemToChest = new RelayCommand<Item>(moveItemToStorage);
            selectedItemScreen.addEnchantmentSlot = new RelayCommand<object>(equipmentModel.addEnchantmentSlot);
            selectedEnchantmentScreen.close = new RelayCommand<Enchantment>(equipmentModel.selectEnchantment);
            selectedEnchantmentScreen.saveChanges = new RelayCommand<Enchantment>(equipmentModel.saveEnchantment);

            equipmentModel.selectedItem.subscribe(item => this.selectedItemScreen.item = item);
            equipmentModel.selectedEnchantment.subscribe(updateEnchantmentScreenUI);
            equipmentModel.remainingEnchantmentPoints.subscribe(updateEnchantmentPointsUI);
        }

        private void duplicateItem(Item item)
        {
            model?.mainEquipmentModel?.selectEnchantment(null);
            model?.mainEquipmentModel?.addItemToList(item);
            model?.mainEquipmentModel?.selectItem(item);
        }

        private void removeItem(Item item)
        {
            model?.mainEquipmentModel?.selectEnchantment(null);
            model?.mainEquipmentModel?.selectItem(null);
            model?.mainEquipmentModel?.removeItem(item);
        }

        private void moveItemToStorage(Item item)
        {
            model?.storageChestEquipmentModel?.selectEnchantment(null);
            model?.storageChestEquipmentModel?.addItemToList(item);
            model?.storageChestEquipmentModel?.selectItem(item);
            
            removeItem(item);
        }

        private void emeraldsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_model?.profile.value == null || !emeraldsTextBox.IsEnabled) { return; }
            if (uint.TryParse(emeraldsTextBox.Text, out uint emeralds))
            {
                EventLogger.logEvent("emeraldsTextBox_TextChanged");
                emeraldsTextBox.BorderBrush = Brushes.Gray;
                _model!.emeralds.setValue = emeralds;
            }
            else
            {
                emeraldsTextBox.BorderBrush = Brushes.Red;
            }
        }

        private void goldTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_model?.profile.value == null || !goldTextBox.IsEnabled) { return; }
            if (uint.TryParse(goldTextBox.Text, out uint gold))
            {
                EventLogger.logEvent("goldTextBox_TextChanged");
                goldTextBox.BorderBrush = Brushes.Gray;
                _model!.gold.setValue = gold;
            }
            else
            {
                goldTextBox.BorderBrush = Brushes.Red;
            }
        }

        private void eyeOfEnderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_model?.profile.value == null || !eyeOfEnderTextBox.IsEnabled)
                return;
            if (uint.TryParse(eyeOfEnderTextBox.Text, out uint eyeOfEnder))
            {
                EventLogger.logEvent("eyeOfEnderTextBox_TextChanged");
                eyeOfEnderTextBox.BorderBrush = Brushes.Gray;
                _model!.eyeOfEnder.setValue = eyeOfEnder;
            }
            else
            {
                eyeOfEnderTextBox.BorderBrush = Brushes.Red;
            }
        }

        #region UI

        public void updateUI()
        {
            updateEmeraldsUI(_model?.emeralds.value);
            updateGoldUI(_model?.gold.value);
            updateEyeOfEnderUI(_model?.eyeOfEnder.value);
            updateUnlockPortalUI(_model?.unlockPortal.value);
            updateEnchantmentPointsUI(model?.mainEquipmentModel.remainingEnchantmentPoints.value);
            equipmentScreen.updateUI();
            itemListScreen.updateUI();
            selectedItemScreen.item = model?.mainEquipmentModel?.selectedItem.value;
        }

        private void updateEmeraldsUI(ulong? emeralds)
        {
            if (emeralds != null)
            {
                emeraldsTextBox.IsEnabled = false;
                emeraldsTextBox.Text = emeralds!.ToString();
                emeraldsTextBox.IsEnabled = true;
                emeraldsTextBox.Visibility = Visibility.Visible;
                emeraldsAddButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                emeraldsTextBox.IsEnabled = false;
                emeraldsTextBox.Text = string.Empty;
                emeraldsTextBox.Visibility = Visibility.Collapsed;
                emeraldsAddButton.Visibility = Visibility.Visible;
            }
        }

        private void updateGoldUI(ulong? gold)
        {
            if (gold != null)
            {
                goldTextBox.IsEnabled = false;
                goldTextBox.Text = gold!.ToString();
                goldTextBox.IsEnabled = true;
                goldTextBox.Visibility = Visibility.Visible;
                goldAddButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                goldTextBox.IsEnabled = false;
                goldTextBox.Text = string.Empty;
                goldTextBox.Visibility = Visibility.Collapsed;
                goldAddButton.Visibility = Visibility.Visible;
            }
        }

        private void updateEyeOfEnderUI(ulong? eyeOfEnder)
        {
            if (eyeOfEnder != null)
            {
                eyeOfEnderTextBox.IsEnabled = false;
                eyeOfEnderTextBox.Text = eyeOfEnder!.ToString();
                eyeOfEnderTextBox.IsEnabled = true;
                eyeOfEnderTextBox.Visibility = Visibility.Visible;
                eyeOfEnderAddButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                eyeOfEnderTextBox.IsEnabled = false;
                eyeOfEnderTextBox.Text = string.Empty;
                eyeOfEnderTextBox.Visibility = Visibility.Collapsed;
                eyeOfEnderAddButton.Visibility = Visibility.Visible;
            }
        }

        private void updateUnlockPortalUI(bool? value)
        {
            if (_model?.profile.value == null || !value.HasValue)
            {
                unlockPortalButton.Visibility = Visibility.Collapsed;
                return;
            }

            unlockPortalButton.Visibility = value.Value ? Visibility.Collapsed : Visibility.Visible;
        }

        private void updateEnchantmentPointsUI(int? value)
        {
            remainingEnchantmentPointsLabel.Content = value?.ToString() ?? string.Empty;
        }

        private void updateEnchantmentScreenUI(Enchantment? enchantment)
        {
            if (enchantment == null)
            {
                selectedEnchantmentScreen.Visibility = Visibility.Collapsed;
                selectedEnchantmentScreenBackShadowRectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                selectedEnchantmentScreen.Visibility = Visibility.Visible;
                selectedEnchantmentScreenBackShadowRectangle.Visibility = Visibility.Visible;
                selectedEnchantmentScreen.enchantment = enchantment;
                selectedEnchantmentScreen.isGilded = this.selectedItemScreen.item?.NetheriteEnchant != null;
                selectedItemScreen.updateEnchantmentsUI();
            }
        }

        #endregion

        private void emeraldsAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            _model.emeralds.setValue = 0;
        }

        private void goldAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            _model.gold.setValue = 0;
        }

        private void eyeOfEnderAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            _model.eyeOfEnder.setValue = 0;
        }

        private void unlockPortalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            _model.unlockPortal.setValue = true;
        }
    }
}
