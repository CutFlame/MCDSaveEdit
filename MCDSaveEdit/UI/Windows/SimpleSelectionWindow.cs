using MCDSaveEdit.Data;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#nullable enable

namespace MCDSaveEdit.UI
{
    /// <summary>
    /// A selection window for armor properties, enchantments, and items
    /// </summary>
    public partial class SimpleSelectionWindow : Window, ISelectionWindow
    {
        private ListBox _listBox = new ListBox();
        private bool _isProcessing = true;

        public Action<string?>? onSelection { get; set; }

        public string? selectedItem {
            get {
                if(_listBox.SelectedItem is ListBoxItem listItem)
                {
                    return listItem.Tag as string;
                }
                return _listBox.SelectedItem as string;
            }
        }

        public SimpleSelectionWindow()
        {
            WindowStyle = WindowStyle.ToolWindow;
            ResizeMode = ResizeMode.NoResize;
            Height = 600;
            Width = 300;
            ScrollViewer.SetCanContentScroll(_listBox, false);
            ScrollViewer.SetVerticalScrollBarVisibility(_listBox, ScrollBarVisibility.Visible);
            _listBox.SelectionChanged += listBox_SelectionChanged;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isProcessing) return;
            EventLogger.logEvent("listBox_SelectionChanged", new Dictionary<string, object>() { { "item", selectedItem ?? "null" } });
            _listBox.SelectionChanged -= listBox_SelectionChanged;
            onSelection?.Invoke(selectedItem);
            this.Close();
        }

        public void loadArmorProperties(string? selectedArmorProperty = null)
        {
            Title = R.SELECT_ARMOR_PROPERTY;
            Content = _listBox;
            _isProcessing = true;
            _listBox.Items.Clear();

            foreach (var armorProperty in ItemDatabase.armorProperties.OrderBy(str => str))
            {
                var itemView = new BaseSelectionWindow.ArmorPropertyView { titleContent = R.armorProperty(armorProperty) };
                if (Config.instance.showIDsInSelectionWindow)
                {
                    itemView.subtitleContent = armorProperty;
                }

                var listItem = new ListBoxItem { Content = itemView, Tag = armorProperty };
                _listBox.Items.Add(listItem);

                if (selectedArmorProperty == armorProperty)
                {
                    _listBox.SelectedItem = listItem;
                }
            }

            _isProcessing = false;
        }

        public void loadEnchantments(string? selectedEnchantment = null)
        {
            Title = R.getString("UIHints_SelectEnchantmentTitle") ?? R.SELECT_ENCHANTMENT;
            Content = _listBox;
            _isProcessing = true;
            _listBox.Items.Clear();

            foreach (var enchantment in EnchantmentDatabase.allEnchantments.OrderBy(str => str).Concat(new[] { Constants.DEFAULT_ENCHANTMENT_ID }))
            {
                var imageSource = ImageResolver.instance.imageSourceForEnchantment(enchantment);
                if (imageSource == null)
                {
                    continue;
                }

                var itemView = new BaseSelectionWindow.EnchantmentView { imageSource = imageSource, titleContent = R.enchantmentName(enchantment) };
                itemView.powerful = Constants.powerful.Contains(enchantment);
                if(Config.instance.showIDsInSelectionWindow)
                {
                    itemView.subtitleContent = enchantment;
                }

                var listItem = new ListBoxItem { Content = itemView, Tag = enchantment };
                _listBox.Items.Add(listItem);

                if (selectedEnchantment == enchantment)
                {
                    _listBox.SelectedItem = listItem;
                }
            }

            _isProcessing = false;
        }

        public void loadFilteredItems(ItemFilterEnum filter, string? selectedItem = null)
        {
            Title = getTitleForFilter(filter);
            Content = _listBox;

            buildItemList(filter, selectedItem: selectedItem);

            _isProcessing = false;
        }

        private string getTitleForFilter(ItemFilterEnum filter)
        {
            switch(filter)
            {
                case ItemFilterEnum.Armor: return R.getString("merchant_slot_select_item") ?? R.SELECT_ARMOR;
                case ItemFilterEnum.Artifacts: return R.getString("merchant_slot_select_item") ?? R.SELECT_ARTIFACT;
                case ItemFilterEnum.MeleeWeapons: return R.getString("merchant_slot_select_item") ?? R.SELECT_MELEE_WEAPON;
                case ItemFilterEnum.RangedWeapons: return R.getString("merchant_slot_select_item") ?? R.SELECT_RANGED_WEAPON;
                default: return R.getString("merchant_slot_select_item") ?? R.SELECT_ITEM;
            }
        }

        public void loadItems(string? selectedItem = null)
        {
            Title = R.getString("merchant_slot_select_item") ?? R.SELECT_ITEM;

            var anyButton = createFilterButton(ItemFilterEnum.All);
            var meleeButton = createFilterButton(ItemFilterEnum.MeleeWeapons);
            var rangedButton = createFilterButton(ItemFilterEnum.RangedWeapons);
            var armorButton = createFilterButton(ItemFilterEnum.Armor);
            var artifactButton = createFilterButton(ItemFilterEnum.Artifacts);

            var toolStack = new StackPanel {
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal,
            };
            toolStack.Children.Add(anyButton);
            toolStack.Children.Add(meleeButton);
            toolStack.Children.Add(rangedButton);
            toolStack.Children.Add(armorButton);
            toolStack.Children.Add(artifactButton);

            var mainStack = new DockPanel();
            DockPanel.SetDock(toolStack, Dock.Top);
            mainStack.Children.Add(toolStack);
            DockPanel.SetDock(_listBox, Dock.Bottom);
            mainStack.Children.Add(_listBox);

            Content = mainStack;

            buildItemList(selectedItem: selectedItem);
        }

        private Button createFilterButton(ItemFilterEnum filter)
        {
            var button = new Button { Margin = new Thickness(5), };
            button.Content = new Image { Source = imageSourceForFilter(filter) };
            button.Command = new RelayCommand<object>(filterItems);
            button.CommandParameter = filter;
            return button;
        }

        private ImageSource? imageSourceForFilter(ItemFilterEnum filter)
        {
            return ImageResolver.instance.imageSourceForItem(mysteryBoxStringForFilter(filter));
        }

        private string mysteryBoxStringForFilter(ItemFilterEnum filter)
        {
            switch (filter)
            {
                case ItemFilterEnum.All: return "MysteryBoxAny";
                case ItemFilterEnum.Armor: return "MysteryBoxArmor";
                case ItemFilterEnum.Artifacts: return "MysteryBoxArtifact";
                case ItemFilterEnum.MeleeWeapons: return "MysteryBoxMelee";
                case ItemFilterEnum.RangedWeapons: return "MysteryBoxRanged";
            }
            throw new ArgumentException($"No MysteryBox string for {filter}", "filter");
        }

        private void filterItems(object filter)
        {
            if(filter is ItemFilterEnum filterEnum)
            {
                buildItemList(filterEnum);
            }
        }

        private void buildItemList(ItemFilterEnum filter = ItemFilterEnum.All, string? selectedItem = null)
        {
            _isProcessing = true;
            _listBox.Items.Clear();

            foreach (var item in itemsForFilter(filter).OrderBy(str => str))
            {
                var imageSource = ImageResolver.instance.imageSourceForItem(item);
                if (imageSource == null)
                {
                    continue;
                }

                var itemView = new BaseSelectionWindow.ItemView { imageSource = imageSource, titleContent = R.itemName(item) };
                if(Config.instance.showIDsInSelectionWindow)
                {
                    itemView.subtitleContent = item;
                }

                var listItem = new ListBoxItem { Content = itemView, Tag = item };
                if (item.ToLowerInvariant().Contains("unique"))
                {
                    var backgroundImage = ImageResolver.instance.imageSourceForRarity(Rarity.Unique);
                    var brush = new ImageBrush(backgroundImage);
                    listItem.Background = brush;
                }
                _listBox.Items.Add(listItem);

                if (selectedItem == item)
                {
                    _listBox.SelectedItem = listItem;
                }
            }
            _isProcessing = false;
        }

        private IEnumerable<string> itemsForFilter(ItemFilterEnum filter)
        {
            switch(filter)
            {
                case ItemFilterEnum.Artifacts: return ItemDatabase.artifacts;
                case ItemFilterEnum.Armor: return ItemDatabase.armor;
                case ItemFilterEnum.MeleeWeapons: return ItemDatabase.meleeWeapons;
                case ItemFilterEnum.RangedWeapons: return ItemDatabase.rangedWeapons;
                case ItemFilterEnum.All: return ItemDatabase.all;
                //case ItemFilterEnum.Enchanted: return new string[0];
            }
            throw new ArgumentException($"No item database for {filter}", "filter");
        }
    }
}
