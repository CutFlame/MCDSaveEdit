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
            _listBox.SelectionChanged += ListBox_SelectionChanged;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isProcessing) return;
            _listBox.SelectionChanged -= ListBox_SelectionChanged;
            onSelection?.Invoke(selectedItem);
            this.Close();
        }

        public void loadEnchantments(string? selectedEnchantment = null)
        {
            Title = R.SELECT_ENCHANTMENT;
            Content = _listBox;
            _isProcessing = true;
            _listBox.Items.Clear();

            var powerfulImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/Inspector/element_powerful");

            foreach (var enchantment in EnchantmentExtensions.allEnchantments.OrderBy(str => str).Concat(new[] { "Unset" }))
            {
                var imageSource = ImageUriHelper.instance.imageSourceForEnchantment(enchantment);
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
                    Content = R.enchantment(enchantment),
                };

                var stackPanel = new StackPanel {
                    Height = 45,
                    Orientation = Orientation.Horizontal,
                };

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(label);
                if (EnchantmentExtensions.powerful.Any(str => { return enchantment == str; }))
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

        public void loadItems(string? selectedItem = null)
        {
            Title = R.SELECT_ITEM;

            var anyButton = buildFilterButton(ItemFilterEnum.All);
            var meleeButton = buildFilterButton(ItemFilterEnum.MeleeWeapons);
            var rangedButton = buildFilterButton(ItemFilterEnum.RangedWeapons);
            var armorButton = buildFilterButton(ItemFilterEnum.Armor);
            var artifactButton = buildFilterButton(ItemFilterEnum.Artifacts);

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

        private Button buildFilterButton(ItemFilterEnum filter)
        {
            var button = new Button { Margin = new Thickness(5), };
            button.Content = new Image { Source = imageSourceForFilter(filter) };
            button.Command = new RelayCommand<object>(filterItems);
            button.CommandParameter = filter;
            return button;
        }

        private ImageSource? imageSourceForFilter(ItemFilterEnum filter)
        {
            return ImageUriHelper.instance.imageSourceForItem(mysteryBoxStringForFilter(filter));
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
                var imageSource = ImageUriHelper.instance.imageSourceForItem(item);
                if (imageSource == null)
                {
                    continue;
                }

                var stackPanel = createStackPanel(imageSource, R.itemName(item));
                var listItem = new ListBoxItem { Content = stackPanel, Tag = item };
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
