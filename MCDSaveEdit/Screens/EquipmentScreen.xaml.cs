using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for EquipmentScreen.xaml
    /// </summary>
    public partial class EquipmentScreen : UserControl
    {
        private static readonly BitmapImage? _levelFrameImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Character/STATS_LV_frame");
        private static readonly BitmapImage? _powerFrameImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Power/gearstrenght_icon");

        public static void preload() { }

        private ProfileViewModel? _model;
        public ProfileViewModel? model {
            get { return _model; }
            set {
                _model = value;
                setupCommands();
                //updateUI();
            }
        }

        public EquipmentScreen()
        {
            InitializeComponent();
            translateStaticStrings();
            if (AppModel.gameContentLoaded)
            {
                useGameContentImages();
            }
        }

        private void useGameContentImages()
        {
            levelLabelImage.Source = _levelFrameImageSource;
            powerLabelImage.Source = _powerFrameImageSource;
        }

        private void translateStaticStrings()
        {
            levelTitleLabel.Content = R.getString("HUD_Level") ?? R.LEVEL;
            powerTitleLabel.Content = R.getString("gearpower_POWER") ?? R.POWER;
        }

        private void setupCommands()
        {
            if (_model == null) { return; }
            var model = _model!;

            meleeGearSlotButton.Command = new RelayCommand<Item>(model.selectItem);
            armorGearSlotButton.Command = new RelayCommand<Item>(model.selectItem);
            rangedGearSlotButton.Command = new RelayCommand<Item>(model.selectItem);
            hotbarSlot1Button.Command = new RelayCommand<Item>(model.selectItem);
            hotbarSlot2Button.Command = new RelayCommand<Item>(model.selectItem);
            hotbarSlot3Button.Command = new RelayCommand<Item>(model.selectItem);
            newItemForMeleeGearSlotButton.Command = new RelayCommandImmutable<EquipmentSlotEnum>(addNewEquippedItemButton_Click);
            newItemForMeleeGearSlotButton.CommandParameter = EquipmentSlotEnum.MeleeGear;
            newItemForArmorGearSlotButton.Command = new RelayCommandImmutable<EquipmentSlotEnum>(addNewEquippedItemButton_Click);
            newItemForArmorGearSlotButton.CommandParameter = EquipmentSlotEnum.ArmorGear;
            newItemForRangedGearSlotButton.Command = new RelayCommandImmutable<EquipmentSlotEnum>(addNewEquippedItemButton_Click);
            newItemForRangedGearSlotButton.CommandParameter = EquipmentSlotEnum.RangedGear;
            newItemForHotbarSlot1Button.Command = new RelayCommandImmutable<EquipmentSlotEnum>(addNewEquippedItemButton_Click);
            newItemForHotbarSlot1Button.CommandParameter = EquipmentSlotEnum.HotbarSlot1;
            newItemForHotbarSlot2Button.Command = new RelayCommandImmutable<EquipmentSlotEnum>(addNewEquippedItemButton_Click);
            newItemForHotbarSlot2Button.CommandParameter = EquipmentSlotEnum.HotbarSlot2;
            newItemForHotbarSlot3Button.Command = new RelayCommandImmutable<EquipmentSlotEnum>(addNewEquippedItemButton_Click);
            newItemForHotbarSlot3Button.CommandParameter = EquipmentSlotEnum.HotbarSlot3;

            model.profile.subscribe(_ => this.updateUI());
            model.level.subscribe(updateLevelUI);
            model.equippedItemList.subscribe(_ => this.updateEquippedItemsUI());
        }

        public void updateUI()
        {
            updateLevelUI(_model?.profile.value?.level());
            updateEquippedItemsUI();
        }

        private void updateLevelUI(int? level)
        {
            if (level != null)
            {
                levelTextBox.IsEnabled = false;
                levelTextBox.Text = level.ToString();
                levelTextBox.IsEnabled = true;
            }
            else
            {
                levelTextBox.IsEnabled = false;
                levelTextBox.Text = string.Empty;
            }
        }

        private void updateEquippedItemsUI()
        {
            if (_model?.profile.value == null)
            {
                meleeGearSlotItemControl.item = null;
                meleeGearSlotButton.CommandParameter = null;
                meleeGearSlotButton.Visibility = Visibility.Visible;
                newItemForMeleeGearSlotButton.Visibility = Visibility.Collapsed;
                armorGearSlotItemControl.item = null;
                armorGearSlotButton.CommandParameter = null;
                armorGearSlotButton.Visibility = Visibility.Visible;
                newItemForArmorGearSlotButton.Visibility = Visibility.Collapsed;
                rangedGearSlotItemControl.item = null;
                rangedGearSlotButton.CommandParameter = null;
                rangedGearSlotButton.Visibility = Visibility.Visible;
                newItemForRangedGearSlotButton.Visibility = Visibility.Collapsed;
                hotbarSlot1ItemControl.item = null;
                hotbarSlot1Button.CommandParameter = null;
                hotbarSlot1Button.Visibility = Visibility.Visible;
                newItemForHotbarSlot1Button.Visibility = Visibility.Collapsed;
                hotbarSlot2ItemControl.item = null;
                hotbarSlot2Button.CommandParameter = null;
                hotbarSlot2Button.Visibility = Visibility.Visible;
                newItemForHotbarSlot2Button.Visibility = Visibility.Collapsed;
                hotbarSlot3ItemControl.item = null;
                hotbarSlot3Button.CommandParameter = null;
                hotbarSlot3Button.Visibility = Visibility.Visible;
                newItemForHotbarSlot3Button.Visibility = Visibility.Collapsed;

                powerLabel.Content = string.Empty;
                return;
            }
            var profile = _model!.profile.value!;

            var meleeGearItem = profile.meleeGearItem();
            meleeGearSlotItemControl.item = meleeGearItem;
            meleeGearSlotButton.CommandParameter = meleeGearItem;
            if (meleeGearItem != null)
            {
                meleeGearSlotButton.Visibility = Visibility.Visible;
                newItemForMeleeGearSlotButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                meleeGearSlotButton.Visibility = Visibility.Collapsed;
                newItemForMeleeGearSlotButton.Visibility = Visibility.Visible;
            }
            var armorGearItem = profile.armorGearItem();
            armorGearSlotItemControl.item = armorGearItem;
            armorGearSlotButton.CommandParameter = armorGearItem;
            if (armorGearItem != null)
            {
                armorGearSlotButton.Visibility = Visibility.Visible;
                newItemForArmorGearSlotButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                armorGearSlotButton.Visibility = Visibility.Collapsed;
                newItemForArmorGearSlotButton.Visibility = Visibility.Visible;
            }
            var rangedGearItem = profile.rangedGearItem();
            rangedGearSlotItemControl.item = rangedGearItem;
            rangedGearSlotButton.CommandParameter = rangedGearItem;
            if (rangedGearItem != null)
            {
                rangedGearSlotButton.Visibility = Visibility.Visible;
                newItemForRangedGearSlotButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                rangedGearSlotButton.Visibility = Visibility.Collapsed;
                newItemForRangedGearSlotButton.Visibility = Visibility.Visible;
            }
            var hotbarSlot1GearItem = profile.hotbarSlot1Item();
            hotbarSlot1ItemControl.item = hotbarSlot1GearItem;
            hotbarSlot1Button.CommandParameter = hotbarSlot1GearItem;
            if (hotbarSlot1GearItem != null)
            {
                hotbarSlot1Button.Visibility = Visibility.Visible;
                newItemForHotbarSlot1Button.Visibility = Visibility.Collapsed;
            }
            else
            {
                hotbarSlot1Button.Visibility = Visibility.Collapsed;
                newItemForHotbarSlot1Button.Visibility = Visibility.Visible;
            }
            var hotbarSlot2GearItem = profile.hotbarSlot2Item();
            hotbarSlot2ItemControl.item = hotbarSlot2GearItem;
            hotbarSlot2Button.CommandParameter = hotbarSlot2GearItem;
            if (hotbarSlot2GearItem != null)
            {
                hotbarSlot2Button.Visibility = Visibility.Visible;
                newItemForHotbarSlot2Button.Visibility = Visibility.Collapsed;
            }
            else
            {
                hotbarSlot2Button.Visibility = Visibility.Collapsed;
                newItemForHotbarSlot2Button.Visibility = Visibility.Visible;
            }
            var hotbarSlot3GearItem = profile.hotbarSlot3Item();
            hotbarSlot3ItemControl.item = hotbarSlot3GearItem;
            hotbarSlot3Button.CommandParameter = hotbarSlot3GearItem;
            if (hotbarSlot3GearItem != null)
            {
                hotbarSlot3Button.Visibility = Visibility.Visible;
                newItemForHotbarSlot3Button.Visibility = Visibility.Collapsed;
            }
            else
            {
                hotbarSlot3Button.Visibility = Visibility.Collapsed;
                newItemForHotbarSlot3Button.Visibility = Visibility.Visible;
            }

            powerLabel.Content = profile.computeCharacterPower();
        }

        private void addNewEquippedItemButton_Click(EquipmentSlotEnum equipmentSlot)
        {
            EventLogger.logEvent("addNewEquippedItemButton_Click", new Dictionary<string, object>() { { "equipmentSlot", equipmentSlot.ToString() } });
            var item = createDefaultItemForEquipmentSlot(equipmentSlot);
            model?.addEquippedItem(item);
            model?.selectItem(item);
        }

        private Item createDefaultItemForEquipmentSlot(EquipmentSlotEnum equipmentSlot)
        {
            var itemID = Constants.defaultItemIDForEquipmentSlot(equipmentSlot);
            return new Item() {
                MarkedNew = true,
                Upgraded = false,
                Power = 1,
                Rarity = Rarity.Common,
                Type = itemID,
                EquipmentSlot = equipmentSlot.ToString(),
            };
        }

        private void levelTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_model?.profile.value == null || !levelTextBox.IsEnabled) { return; }
            if (int.TryParse(levelTextBox.Text, out int level))
            {
                EventLogger.logEvent("levelTextBox_TextChanged", new Dictionary<string, object>() { { "level", level } });
                _model!.level.setValue = level;
            }
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null || !levelTextBox.IsEnabled) { return; }
            if (int.TryParse(levelTextBox.Text, out int level) && level < Constants.MAXIMUM_CHARACTER_LEVEL)
            {
                int newLevel = level + 1;
                EventLogger.logEvent("levelTextBox_upButton_Click", new Dictionary<string, object>() { { "newLevel", newLevel } });
                _model!.level.setValue = newLevel;
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null || !levelTextBox.IsEnabled) { return; }
            if (int.TryParse(levelTextBox.Text, out int level) && level > Constants.MINIMUM_CHARACTER_LEVEL)
            {
                int newLevel = level - 1;
                EventLogger.logEvent("levelTextBox_downButton_Click", new Dictionary<string, object>() { { "newLevel", newLevel } });
                _model!.level.setValue = newLevel;
            }
        }
    }
}
