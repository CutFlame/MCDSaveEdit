using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for ItemScreen.xaml
    /// </summary>
    public partial class ItemScreen : UserControl
    {
        public ItemScreen()
        {
            InitializeComponent();

            enchantment1Control.command = new RelayCommand<Enchantment>(relaySelectEnchantment);
            enchantment2Control.command = new RelayCommand<Enchantment>(relaySelectEnchantment);
            enchantment3Control.command = new RelayCommand<Enchantment>(relaySelectEnchantment);

            //Clear out design/testing values
            rarityComboBox.Items.Clear();
            rarityComboBox.Items.Add(R.getString("rarity_common") ?? R.COMMON);
            rarityComboBox.Items.Add(R.getString("rarity_rare") ?? R.RARE);
            rarityComboBox.Items.Add(R.getString("rarity_unique") ?? R.UNIQUE);

            gildedButtonCheckBox.Content = R.getString("iteminspector_gilded") ?? R.GILDED;
            upgradedButtonCheckBox.Content = R.getString("item_diamond_dust_upgraded") ?? R.UPGRADED;
            giftedButtonCheckBox.Content = R.getString("item_gifted") ?? R.GIFTED;

            updateUI();
        }

        private Item? _item;
        public Item? item
        {
            get { return _item; }
            set { _item = value; updateUI(); }
        }

        public void updateUI()
        {
            if (_item == null)
            {
                inventoryIconImage.Source = null;
                powerTextBox.IsEnabled = false;
                powerTextBox.Text = string.Empty;
                rarityComboBox.IsEnabled = false;
                rarityComboBox.SelectedIndex = -1;
                nameLabel.Content = string.Empty;
                descLabel.Text = string.Empty;
                inventoryItemButton.IsEnabled = false;
            }
            else
            {
                inventoryIconImage.Source = ImageUriHelper.instance.imageSourceForItem(_item);
                powerTextBox.IsEnabled = false;
                powerTextBox.Text = _item.level().ToString();
                powerTextBox.IsEnabled = true;
                rarityComboBox.IsEnabled = false;
                rarityComboBox.SelectedIndex = indexForRarity(_item.Rarity);
                rarityComboBox.IsEnabled = true;
                nameLabel.Content = R.itemName(_item.Type);
                descLabel.Text = R.itemDesc(_item.Type);
                inventoryItemButton.IsEnabled = ImageUriHelper.gameContentLoaded;
            }

            updateCheckBoxes();
            updateArmorPropertiesUI();
            updateEnchantmentsUI();
        }

        public void updateCheckBoxes()
        {
            if (_item == null)
            {
                gildedButton.IsEnabled = false;
                gildedButtonCheckBox.IsChecked = false;
                upgradedButton.IsEnabled = false;
                upgradedButtonCheckBox.IsChecked = false;
                giftedButton.IsEnabled = false;
                giftedButtonCheckBox.IsChecked = false;
            }
            else
            {
                gildedButtonCheckBox.IsChecked = _item.NetheriteEnchant != null;
                upgradedButtonCheckBox.IsChecked = _item.Upgraded;
                upgradedButton.IsEnabled = true;
                giftedButtonCheckBox.IsChecked = _item.Gifted == true;
                giftedButton.IsEnabled = true;
            }
        }

        public void updateArmorPropertiesUI()
        {
            armorPropertiesStack.Children.Clear();
            if (_item?.Armorproperties != null)
            {
                var bulletImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Inspector/regular_bullit");

                foreach (var armorProperty in _item.Armorproperties)
                {
                    var bulletImage = new Image();
                    bulletImage.Height = 20;
                    bulletImage.Width = 20;
                    bulletImage.Source = bulletImageSource;
                    bulletImage.Margin = new Thickness(5);

                    var label = new Label();
                    label.FontSize = 16;
                    label.Content = string.Format("{0}: {1}", R.armorProperty(armorProperty.Id), R.armorPropertyDescription(armorProperty.Id));
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.Padding = new Thickness(0);

                    var armorPropertyStack = new StackPanel();
                    armorPropertyStack.Orientation = Orientation.Horizontal;
                    armorPropertyStack.Children.Add(bulletImage);
                    armorPropertyStack.Children.Add(label);

                    var button = new Button();
                    button.IsEnabled = ImageUriHelper.gameContentLoaded;
                    button.Tag = armorProperty;
                    button.HorizontalContentAlignment = HorizontalAlignment.Left;
                    button.VerticalContentAlignment = VerticalAlignment.Center;
                    button.Height = 32;
                    button.Background = null;
                    button.Padding = new Thickness(0);
                    button.Content = armorPropertyStack;
                    button.CommandParameter = armorProperty;
                    button.Command = new RelayCommand<Armorproperty>(armorPropertyButton_Click);
                    armorPropertiesStack.Children.Add(button);
                }
            }
        }

        private void armorPropertyButton_Click(Armorproperty armorProperty)
        {
            if (!ImageUriHelper.gameContentLoaded) { return; }
            EventLogger.logEvent("armorPropertyButton_Click", new Dictionary<string, object>() { { "armorProperty", armorProperty.Id } });
            var selectionWindow = new SelectionWindow();
            selectionWindow.Owner = Application.Current.MainWindow;
            selectionWindow.loadArmorProperties(armorProperty.Id);
            selectionWindow.onSelection = newArmorPropertyId => {
                this.replaceArmorProperty(armorProperty.Id, newArmorPropertyId);
            };
            //selectionWindow.onSelection = selectedArmorPropertyId;
            selectionWindow.Show();
        }

        private void replaceArmorProperty(string oldArmorPropertyId, string? newArmorPropertyId)
        {
            if (_item == null) { return; }
            if (newArmorPropertyId == null) { return; }
            var index = _item!.Armorproperties.ToList().FindIndex(prop => prop.Id == oldArmorPropertyId);
            _item!.Armorproperties[index].Id = newArmorPropertyId;
            //var newProperty = new Armorproperty() { Id = newArmorPropertyId, Rarity = Rarity.Common };
            //_item!.Armorproperties[index] = newProperty;
            updateArmorPropertiesUI();
        }

        public void updateEnchantmentsUI()
        {
            if (_item == null || _item.Enchantments == null || _item.isArtifact())
            {
                enchantment1Control.enchantments = null;
                enchantment1Control.Visibility = Visibility.Hidden;
                enchantment2Control.enchantments = null;
                enchantment2Control.Visibility = Visibility.Hidden;
                enchantment3Control.enchantments = null;
                enchantment3Control.Visibility = Visibility.Hidden;
            }
            else
            {
                enchantment1Control.enchantments = _item.Enchantments.Skip(0).Take(3);
                enchantment1Control.Visibility = Visibility.Visible;
                enchantment2Control.enchantments = _item.Enchantments.Skip(3).Take(3);
                enchantment2Control.Visibility = Visibility.Visible;
                enchantment3Control.enchantments = _item.Enchantments.Skip(6).Take(3);
                enchantment3Control.Visibility = Visibility.Visible;
            }
        }

        private void relaySelectEnchantment(Enchantment enchantment)
        {
            selectEnchantment?.Execute(enchantment);
        }

        private int indexForRarity(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return 0;
                case Rarity.Rare: return 1;
                case Rarity.Unique: return 2;
            }
            throw new NotImplementedException();
        }
        private Rarity rarityForIndex(int index)
        {
            switch (index)
            {
                case 0: return Rarity.Common;
                case 1: return Rarity.Rare;
                case 2: return Rarity.Unique;
            }
            throw new NotImplementedException();
        }

        public ICommand? saveChanges { get; set; }
        public ICommand? selectEnchantment { get; set; }

        private void gildedButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Not Implemented Yet
            throw new NotImplementedException();
        }

        private void upgradedButton_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null) return;
            _item.Upgraded = !_item.Upgraded;
            this.saveChanges?.Execute(_item);
        }

        private void giftedButton_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null) return;
            _item.Gifted = (_item.Gifted == null || _item.Gifted == false) ? true : false;
            this.saveChanges?.Execute(_item);
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null || !powerTextBox.IsEnabled) { return; }
            if (int.TryParse(powerTextBox.Text, out int level) && level < Constants.MAXIMUM_ITEM_LEVEL)
            {
                int newLevel = level + 1;
                EventLogger.logEvent("powerTextBox_upButton_Click", new Dictionary<string, object>() { { "newLevel", newLevel } });
                powerTextBox.Text = newLevel.ToString();
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null || !powerTextBox.IsEnabled) { return; }
            if (int.TryParse(powerTextBox.Text, out int level) && level > Constants.MINIMUM_ITEM_LEVEL)
            {
                int newLevel = level - 1;
                EventLogger.logEvent("powerTextBox_downButton_Click", new Dictionary<string, object>() { { "newLevel", newLevel } });
                powerTextBox.Text = newLevel.ToString();
            }
        }

        private void powerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_item == null || !powerTextBox.IsEnabled) { return; }
            if (int.TryParse(powerTextBox.Text, out int level))
            {
                EventLogger.logEvent("powerTextBox_TextChanged", new Dictionary<string, object>() { { "level", level } });
                powerTextBox.BorderBrush = Brushes.Gray;
                _item.Power = GameCalculator.powerFromLevel(level);
                this.saveChanges?.Execute(_item);
            }
            else
            {
                powerTextBox.BorderBrush = Brushes.Red;
            }
        }

        private void rarityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_item == null || !rarityComboBox.IsEnabled) { return; }
            Rarity rarity = rarityForIndex(rarityComboBox.SelectedIndex);
            EventLogger.logEvent("rarityComboBox_SelectionChanged", new Dictionary<string, object>() { { "rarity", rarity.ToString() } });
            _item.Rarity = rarity;
            this.saveChanges?.Execute(_item);
        }

        private void inventoryItemButton_Click(object sender, RoutedEventArgs e)
        {
            if(_item == null) { return; }
            if (!ImageUriHelper.gameContentLoaded) { return; }
            EventLogger.logEvent("inventoryItemButton_Click", new Dictionary<string, object>() { { "item", _item!.Type } });
            var selectionWindow = new SelectionWindow();
            selectionWindow.Owner = Application.Current.MainWindow;
            var filter = getFilter(_item);
            if(filter != ItemFilterEnum.All)
            {
                selectionWindow.loadFilteredItems(filter, _item?.Type);
            }
            else
            {
                selectionWindow.loadItems(_item?.Type);
            }
            selectionWindow.onSelection = selectedItemType;
            selectionWindow.Show();
        }

        private ItemFilterEnum getFilter(Item? item)
        {
            if (item == null) return ItemFilterEnum.All;

            if (item!.isArmor())
            {
                return ItemFilterEnum.Armor;
            }
            else if (item!.isArtifact())
            {
                return ItemFilterEnum.Artifacts;
            }
            else if (item!.isMeleeWeapon())
            {
                return ItemFilterEnum.MeleeWeapons;
            }
            else if (item!.isRangedWeapon())
            {
                return ItemFilterEnum.RangedWeapons;
            }
            else
            {
                return ItemFilterEnum.All;
            }
        }

        private void selectedItemType(string? itemType)
        {
            if (itemType == null)
            {
                _item = null;
            }
            else
            {
                if (_item == null)
                {
                    Debug.WriteLine("This should not happen");
                    EventLogger.logError("_item was null when itemType was not null");
                    throw new Exception();
                }
                else
                {
                    _item.Type = itemType!;
                }
                this.saveChanges?.Execute(_item);
            }
            updateUI();
        }

    }
}