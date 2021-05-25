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
using System.Windows.Media.Imaging;
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

            netheriteEnchantmentControl.saveChanges = new RelayCommand<object>(relaySaveChanges);
            enchantment1Control.command = new RelayCommand<Enchantment>(relaySelectEnchantment);
            enchantment2Control.command = new RelayCommand<Enchantment>(relaySelectEnchantment);
            enchantment3Control.command = new RelayCommand<Enchantment>(relaySelectEnchantment);

            //Clear out design/testing values
            rarityComboBox.Items.Clear();
            rarityComboBox.Items.Add(R.getString("rarity_common") ?? R.COMMON);
            rarityComboBox.Items.Add(R.getString("rarity_rare") ?? R.RARE);
            rarityComboBox.Items.Add(R.getString("rarity_unique") ?? R.UNIQUE);

            markedNewButtonCheckBox.Content = R.getString("Interest_New") ?? R.NEW;
            upgradedButtonCheckBox.Content = R.getString("item_diamond_dust_upgraded") ?? R.UPGRADED;
            giftedButtonCheckBox.Content = R.getString("item_gifted") ?? R.GIFTED;

            duplicateItemButton.Content = R.DUPLICATE;
            deleteItemButton.Content = R.DELETE;

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
                inventoryIconImage.Source = AppModel.instance.imageSourceForItem(_item);
                powerTextBox.IsEnabled = false;
                powerTextBox.Text = _item.level().ToString();
                powerTextBox.IsEnabled = true;
                rarityComboBox.IsEnabled = false;
                rarityComboBox.SelectedIndex = indexForRarity(_item.Rarity);
                rarityComboBox.IsEnabled = true;
                nameLabel.Content = R.itemName(_item.Type);
                descLabel.Text = R.itemDesc(_item.Type);
                inventoryItemButton.IsEnabled = AppModel.gameContentLoaded;
            }

            updateCheckBoxes();
            updateArmorPropertiesUI();
            updateNetheriteEnchantmentUI();
            updateEnchantmentsUI();
        }

        public void updateCheckBoxes()
        {
            if (_item == null)
            {
                markedNewButton.IsEnabled = false;
                markedNewButtonCheckBox.IsChecked = false;
                upgradedButton.IsEnabled = false;
                upgradedButtonCheckBox.IsChecked = false;
                giftedButton.IsEnabled = false;
                giftedButtonCheckBox.IsChecked = false;
                duplicateItemButton.IsEnabled = false;
                deleteItemButton.IsEnabled = false;
            }
            else
            {
                markedNewButtonCheckBox.IsChecked = _item.MarkedNew == true;
                markedNewButton.IsEnabled = true;
                upgradedButtonCheckBox.IsChecked = _item.Upgraded;
                upgradedButton.IsEnabled = true;
                giftedButtonCheckBox.IsChecked = _item.Gifted == true;
                giftedButton.IsEnabled = true;
                duplicateItemButton.IsEnabled = true;
                deleteItemButton.IsEnabled = true;
            }
        }

        public class ArmorPropertyButton: Button
        {
            public static BitmapImage? bulletImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Inspector/regular_bullit");

            public ArmorPropertyButton(Armorproperty? armorProperty)
            {
                var bulletImage = new Image();
                bulletImage.Height = 20;
                bulletImage.Width = 20;
                bulletImage.Margin = new Thickness(5);
                bulletImage.Source = bulletImageSource;

                var label = new Label();
                label.FontSize = 16;
                if(armorProperty == null)
                {
                    label.Content = "<null>";
                }
                else
                {
                    label.Content = string.Format("{0}: {1}", R.armorProperty(armorProperty.Id), R.armorPropertyDescription(armorProperty.Id));
                }
                label.VerticalAlignment = VerticalAlignment.Center;
                label.Padding = new Thickness(0);

                var armorPropertyStack = new StackPanel();
                armorPropertyStack.Orientation = Orientation.Horizontal;
                armorPropertyStack.Children.Add(bulletImage);
                armorPropertyStack.Children.Add(label);

                this.IsEnabled = AppModel.gameContentLoaded;
                this.Tag = armorProperty;
                this.HorizontalContentAlignment = HorizontalAlignment.Left;
                this.VerticalContentAlignment = VerticalAlignment.Center;
                this.Height = 32;
                this.Background = null;
                this.Padding = new Thickness(0);
                this.Content = armorPropertyStack;
            }
        }

        public void updateArmorPropertiesUI()
        {
            armorPropertiesStack.Children.Clear();
            if (_item == null || !_item.isArmor()) {
                armorPropertiesScrollViewer.Visibility = Visibility.Collapsed;
                return;
            }

            armorPropertiesScrollViewer.Visibility = Visibility.Visible;
            if (_item?.Armorproperties != null)
            {
                foreach (var armorProperty in _item.Armorproperties)
                {
                    var stack = new DockPanel() {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };

                    var removeButton = new Button();
                    removeButton.Content = "X";
                    removeButton.Width = 20;
                    removeButton.CommandParameter = armorProperty;
                    removeButton.Command = new RelayCommand<Armorproperty>(armorPropertyRemoveButton_Click);
                    stack.Children.Add(removeButton);
                    DockPanel.SetDock(removeButton, Dock.Right);

                    var button = new ArmorPropertyButton(armorProperty);
                    button.ClipToBounds = true;
                    button.CommandParameter = armorProperty;
                    button.Command = new RelayCommand<Armorproperty>(armorPropertyButton_Click);
                    stack.Children.Add(button);
                    DockPanel.SetDock(button, Dock.Left);

                    armorPropertiesStack.Children.Add(stack);
                }
            }

            //Add Plus button
            var label = new Label();
            label.FontSize = 16;
            label.Content = "+";
            label.Padding = new Thickness(0);

            var plusButton = new Button();
            plusButton.IsEnabled = AppModel.gameContentLoaded;
            plusButton.HorizontalContentAlignment = HorizontalAlignment.Center;
            plusButton.VerticalContentAlignment = VerticalAlignment.Center;
            plusButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            plusButton.Height = 32;
            plusButton.Padding = new Thickness(0);
            plusButton.Content = label;
            plusButton.Command = new RelayCommand<object>(plusArmorPropertyButton_Click);

            armorPropertiesStack.Children.Add(plusButton);
        }

        private void plusArmorPropertyButton_Click(object sender)
        {
            if (_item == null || !_item.isArmor()) { return; }
            EventLogger.logEvent("plusArmorPropertyButton_Click");
            var armorProperties = _item.Armorproperties?.ToList() ?? new List<Armorproperty>();
            armorProperties.Add(new Armorproperty() { Id = Constants.DEFAULT_ARMOR_PROPERTY_ID, Rarity = Rarity.Common });
            _item.Armorproperties = armorProperties.ToArray();
            updateArmorPropertiesUI();
        }

        public void updateNetheriteEnchantmentUI()
        {
            if (_item == null || _item.isArtifact())
            {
                netheriteEnchantmentControl.Visibility = Visibility.Collapsed;
                netheriteEnchantmentControl.item = null;
            }
            else
            {
                netheriteEnchantmentControl.Visibility = Visibility.Visible;
                netheriteEnchantmentControl.item = _item;
            }
        }

        private void armorPropertyButton_Click(Armorproperty armorProperty)
        {
            if (!AppModel.gameContentLoaded) { return; }
            EventLogger.logEvent("armorPropertyButton_Click", new Dictionary<string, object>() { { "armorProperty", armorProperty.Id } });
            var selectionWindow = WindowFactory.createSelectionWindow();
            selectionWindow.loadArmorProperties(armorProperty.Id);
            selectionWindow.onSelection = newArmorPropertyId => {
                this.replaceArmorProperty(armorProperty.Id, newArmorPropertyId);
            };
            //selectionWindow.onSelection = selectedArmorPropertyId;
            selectionWindow.Show();
        }

        private void armorPropertyRemoveButton_Click(Armorproperty armorProperty)
        {
            if (_item == null) { return; }
            if (armorProperty == null) { return; }
            EventLogger.logEvent("armorPropertyRemoveButton_Click", new Dictionary<string, object>() { { "armorProperty", armorProperty.Id } });
            var list = _item!.Armorproperties.ToList();
            list.Remove(armorProperty);
            _item!.Armorproperties = list.ToArray();
            updateArmorPropertiesUI();
        }

        private void replaceArmorProperty(string oldArmorPropertyId, string? newArmorPropertyId)
        {
            if (_item == null) { return; }
            if (newArmorPropertyId == null) { return; }
            EventLogger.logEvent("replaceArmorProperty", new Dictionary<string, object>() { { "oldArmorPropertyId", oldArmorPropertyId }, { "newArmorPropertyId", newArmorPropertyId } });
            var index = _item!.Armorproperties.ToList().FindIndex(prop => prop.Id == oldArmorPropertyId);
            _item!.Armorproperties[index].Id = newArmorPropertyId;
            //var newProperty = new Armorproperty() { Id = newArmorPropertyId, Rarity = Rarity.Common };
            //_item!.Armorproperties[index] = newProperty;
            updateArmorPropertiesUI();
        }

        public void updateEnchantmentsUI()
        {
            if (_item == null || _item.isArtifact())
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
                if(_item.Enchantments != null)
                {
                    enchantment1Control.enchantments = _item.Enchantments.Skip(0).Take(3);
                    enchantment2Control.enchantments = _item.Enchantments.Skip(3).Take(3);
                    enchantment3Control.enchantments = _item.Enchantments.Skip(6).Take(3);
                }
                else
                {
                    enchantment1Control.enchantments = new List<Enchantment>();
                    enchantment2Control.enchantments = new List<Enchantment>();
                    enchantment3Control.enchantments = new List<Enchantment>();
                }
                enchantment1Control.Visibility = Visibility.Visible;
                enchantment2Control.Visibility = Visibility.Visible;
                enchantment3Control.Visibility = Visibility.Visible;
            }
        }

        private void relaySelectEnchantment(Enchantment enchantment)
        {
            if (enchantment == null)
            {
                addEnchantmentSlot?.Execute(this);
                updateEnchantmentsUI();
            }
            else
            {
                selectEnchantment?.Execute(enchantment);
            }

        }

        private void relaySaveChanges(object sender)
        {
            this.saveChanges?.Execute(_item);
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

        public ICommand? duplicateItem { get; set; }
        public ICommand? deleteItem { get; set; }
        public ICommand? saveChanges { get; set; }
        public ICommand? selectEnchantment { get; set; }
        public ICommand? addEnchantmentSlot { get; set; }

        private void markedNewButton_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null) return;
            _item.MarkedNew = (_item.MarkedNew == null || _item.MarkedNew == false) ? true : false;
            EventLogger.logEvent("markedNewButton_Click", new Dictionary<string, object>() { { "markedNew", _item.MarkedNew.ToString() } });
            this.saveChanges?.Execute(_item);
        }

        private void upgradedButton_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null) return;
            _item.Upgraded = !_item.Upgraded;
            EventLogger.logEvent("upgradedButton_Click", new Dictionary<string, object>() { { "upgraded", _item.Upgraded.ToString() } });
            this.saveChanges?.Execute(_item);
        }

        private void giftedButton_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null) return;
            _item.Gifted = (_item.Gifted == null || _item.Gifted == false) ? true : false;
            EventLogger.logEvent("giftedButton_Click", new Dictionary<string, object>() { { "gifted", _item.Gifted.ToString() } });
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
            if (!AppModel.gameContentLoaded) { return; }
            EventLogger.logEvent("inventoryItemButton_Click", new Dictionary<string, object>() { { "item", _item!.Type } });
            var selectionWindow = WindowFactory.createSelectionWindow();
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

        private void duplicateItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null) { return; }
            EventLogger.logEvent("duplicateItemButton_Click", new Dictionary<string, object>() { { "item", _item!.Type } });
            this.duplicateItem?.Execute(_item.Copy());
        }

        private void deleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null) { return; }
            EventLogger.logEvent("deleteItemButton_Click", new Dictionary<string, object>() { { "item", _item!.Type } });
            this.deleteItem?.Execute(_item);
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