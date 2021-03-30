using MCDSaveEdit.Save.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>
    public class SelectionWindow : Window
    {
        private static readonly BitmapImage? powerfulImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/Inspector/element_powerful");
        private static readonly BitmapImage? bulletImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Inspector/regular_bullit");

        public static void preload()
        {
            foreach (var item in ItemExtensions.all)
            {
                AppModel.instance.imageSourceForItem(item);
            }
            foreach (var enchantment in EnchantmentExtensions.allEnchantments)
            {
                AppModel.instance.imageSourceForEnchantment(enchantment);
            }
        }

        private ListBox _listBox = new ListBox();
        private bool _isProcessing = true;

        public Action<string?>? onSelection;

        public string? selectedItem {
            get {
                if(_listBox.SelectedItem is ListBoxItem listItem)
                {
                    return listItem.Tag as string;
                }
                return _listBox.SelectedItem as string;
            }
        }

        public SelectionWindow()
        {
            WindowStyle = WindowStyle.ToolWindow;
            ResizeMode = ResizeMode.NoResize;
            Height = 600;
            Width = 300;
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

            foreach (var armorProperty in ItemExtensions.armorProperties.OrderBy(str => str))
            {
                var image = new Image {
                    Height = 25,
                    Width = 25,
                    VerticalAlignment = VerticalAlignment.Center,
                    Source = bulletImageSource,
                };
                var label = new Label {
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                    Content = R.armorProperty(armorProperty),
                };

                var stackPanel = new StackPanel {
                    Height = 45,
                    Orientation = Orientation.Horizontal,
                };

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(label);

                var listItem = new ListBoxItem { Content = stackPanel, Tag = armorProperty };
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
            Title = R.SELECT_ENCHANTMENT;
            Content = _listBox;
            _isProcessing = true;
            _listBox.Items.Clear();

            foreach (var enchantment in EnchantmentExtensions.allEnchantments.OrderBy(str => str).Concat(new[] { Constants.DEFAULT_ENCHANTMENT_ID }))
            {
                var imageSource = AppModel.instance.imageSourceForEnchantment(enchantment);
                if (imageSource == null)
                {
                    continue;
                }

                var image = new Image {
                    Height = 50,
                    Width = 50,
                    VerticalAlignment = VerticalAlignment.Center,
                    Source = imageSource,
                };
                var label = new Label {
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                    Content = R.enchantmentName(enchantment),
                };

                var stackPanel = new StackPanel {
                    Height = 45,
                    Orientation = Orientation.Horizontal,
                };

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(label);
                if (EnchantmentExtensions.powerful.Contains(enchantment))
                {
                    var powerfulImage = new Image {
                        Height = 25,
                        Width = 25,
                        Source = powerfulImageSource,
                    };
                    stackPanel.Children.Add(powerfulImage);
                }

                var listItem = new ListBoxItem { Content = stackPanel, Tag = enchantment };
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
                case ItemFilterEnum.Armor: return R.SELECT_ARMOR;
                case ItemFilterEnum.Artifacts: return R.SELECT_ARTIFACT;
                case ItemFilterEnum.MeleeWeapons: return R.SELECT_MELEE_WEAPON;
                case ItemFilterEnum.RangedWeapons: return R.SELECT_RANGED_WEAPON;
                default: return R.SELECT_ITEM;
            }
        }

        public void loadItems(string? selectedItem = null)
        {
            Title = R.SELECT_ITEM;

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
            return AppModel.instance.imageSourceForItem(mysteryBoxStringForFilter(filter));
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
            throw new Exception();
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
                var imageSource = AppModel.instance.imageSourceForItem(item);
                if (imageSource == null)
                {
                    continue;
                }

                var stackPanel = createStackPanel(imageSource, R.itemName(item));
                var listItem = new ListBoxItem { Content = stackPanel, Tag = item };
                if (item.ToLowerInvariant().Contains("unique"))
                {
                    var backgroundImage = AppModel.instance.imageSourceForRarity(Rarity.Unique);
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
                case ItemFilterEnum.Artifacts: return ItemExtensions.artifacts;
                case ItemFilterEnum.Armor: return ItemExtensions.armor;
                case ItemFilterEnum.MeleeWeapons: return ItemExtensions.meleeWeapons;
                case ItemFilterEnum.RangedWeapons: return ItemExtensions.rangedWeapons;
                case ItemFilterEnum.All: return ItemExtensions.all;
            }
            return new string[0];
        }

        private StackPanel createStackPanel(BitmapImage? imageSource, string labelText)
        {
            var image = new Image {
                Height = 50,
                Width = 50,
                VerticalAlignment = VerticalAlignment.Center,
                Source = imageSource,
            };
            var label = new Label {
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14,
                Content = labelText,
            };

            var stackPanel = new StackPanel {
                Height = 50,
                Orientation = Orientation.Horizontal,
            };

            stackPanel.Children.Add(image);
            stackPanel.Children.Add(label);

            return stackPanel;
        }
    }
}
