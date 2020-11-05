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

                //disabling armor swapping for now until armorProperties can be swapped too
                inventoryItemButton.IsEnabled = !_item.isArmor();
            }

            updateArmorPropertiesUI();
            updateEnchantmentsUI();
        }

        public void updateArmorPropertiesUI()
        {
            armorPropertiesStack.Children.Clear();
            if (_item?.Armorproperties != null)
            {
                foreach (var armorProperty in _item.Armorproperties)
                {
                    var label = new Label();
                    label.FontSize = 16;
                    label.Content = string.Format("{0}: {1}", R.armorProperty(armorProperty.Id), R.armorPropertyDescription(armorProperty.Id));
                    armorPropertiesStack.Children.Add(label);
                }
            }
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
            EventLogger.logEvent("inventoryItemButton_Click", new Dictionary<string, object>() { { "item", _item!.Type } });
            var selectionWindow = new SelectionWindow();
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