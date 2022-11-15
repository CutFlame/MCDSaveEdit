using MCDSaveEdit.Data;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#nullable enable

namespace MCDSaveEdit.UI
{
    /// <summary>
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>
    public partial class SelectionWindow : Window, INotifyPropertyChanged
    {
        private string _searchText;

        public string searchText {
            get { return _searchText; }
            set {
                _searchText = value;
                _isProcessing = true;
                OnPropertyChanged("searchText");
                OnPropertyChanged("filteredItems");
                _isProcessing = false;
            }
        }

        private List<ListBoxItem> items = new List<ListBoxItem>();

        public IEnumerable<ListBoxItem> filteredItems {
            get {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    return items;
                }
                var searchUpper = searchText.Trim().ToUpperInvariant();
                var itemsFirstStartWith = items.Where(i => firstFilterableContent(i).ToUpperInvariant().StartsWith(searchUpper));
                var itemsSecondStartWith = items.Where(i => secondFilterableContent(i).ToUpperInvariant().StartsWith(searchUpper));
                var itemsFirstContains = items.Where(i => firstFilterableContent(i).ToUpperInvariant().Contains(searchUpper));
                var itemsSecondContains = items.Where(i => secondFilterableContent(i).ToUpperInvariant().Contains(searchUpper));
                return itemsFirstStartWith.Union(itemsSecondStartWith).Union(itemsFirstContains).Union(itemsSecondContains);
            }
        }

        private string firstFilterableContent(ListBoxItem listBoxItem)
        {
            var view = listBoxItem.Content as BaseSelectionWindow.ItemView;
            return view?.filterableText ?? "";
        }

        private string secondFilterableContent(ListBoxItem listBoxItem)
        {
            return listBoxItem.Tag as string ?? "";
        }

        public SelectionWindow()
        {            
            InitializeComponent();

            ScrollViewer.SetCanContentScroll(listBox, false);
            ScrollViewer.SetVerticalScrollBarVisibility(listBox, ScrollBarVisibility.Visible);
            listBox.SelectionChanged += listBox_SelectionChanged;

            this.DataContext = this;
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            _isProcessing = true;
            // Set the default selected item
            listBox.SelectedItem = _selectedListBoxItem;
            // Put the cursor in the search box
            textBox.Focus();
            _isProcessing = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class SelectionWindow: ISelectionWindow
    {
        private bool _isProcessing = true;

        public Action<string?>? onSelection { get; set; }

        public string? selectedItem {
            get {
                if (listBox.SelectedItem is ListBoxItem listItem)
                {
                    return listItem.Tag as string;
                }
                return listBox.SelectedItem as string;
            }
        }

        private ListBoxItem? _selectedListBoxItem;

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isProcessing) return;
            EventLogger.logEvent("listBox_SelectionChanged", new Dictionary<string, object>() { { "item", selectedItem ?? "null" } });
            listBox.SelectionChanged -= listBox_SelectionChanged;
            onSelection?.Invoke(selectedItem);
            this.Close();
        }

        public void loadArmorProperties(string? selectedArmorProperty = null)
        {
            Title = R.SELECT_ARMOR_PROPERTY;
            _isProcessing = true;
            items.Clear();

            foreach (var armorProperty in ItemDatabase.armorProperties.OrderBy(str => str))
            {
                var title = R.armorProperty(armorProperty);
                var itemView = new BaseSelectionWindow.ArmorPropertyView { titleContent = title, filterableText = title };
                if (Config.instance.showIDsInSelectionWindow)
                {
                    itemView.subtitleContent = armorProperty;
                }

                var listItem = new ListBoxItem { Content = itemView, Tag = armorProperty };
                items.Add(listItem);

                if (selectedArmorProperty == armorProperty)
                {
                    _selectedListBoxItem = listItem;
                }
            }

            _isProcessing = false;
        }

        public void loadEnchantments(string? selectedEnchantment = null)
        {
            Title = R.getString("UIHints_SelectEnchantmentTitle") ?? R.SELECT_ENCHANTMENT;
            _isProcessing = true;
            items.Clear();

            foreach (var enchantment in EnchantmentDatabase.allEnchantments.OrderBy(str => str).Concat(new[] { Constants.DEFAULT_ENCHANTMENT_ID }))
            {
                var imageSource = ImageResolver.instance.imageSourceForEnchantment(enchantment);
                if (imageSource == null)
                {
                    continue;
                }

                var title = R.enchantmentName(enchantment);
                var itemView = new BaseSelectionWindow.EnchantmentView { imageSource = imageSource, titleContent = title, filterableText = title };
                itemView.powerful = Constants.powerful.Contains(enchantment);
                if (Config.instance.showIDsInSelectionWindow)
                {
                    itemView.subtitleContent = enchantment;
                }

                var listItem = new ListBoxItem { Content = itemView, Tag = enchantment };
                items.Add(listItem);

                if (selectedEnchantment == enchantment)
                {
                    _selectedListBoxItem = listItem;
                }
            }

            _isProcessing = false;
        }

        public void loadFilteredItems(ItemFilterEnum filter, string? selectedItem = null)
        {
            Title = getTitleForFilter(filter);

            buildItemList(filter, selectedItem: selectedItem);

            _isProcessing = false;
        }

        private string getTitleForFilter(ItemFilterEnum filter)
        {
            switch (filter)
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
            DockPanel.SetDock(listBox, Dock.Bottom);
            mainStack.Children.Add(listBox);

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
            if (filter is ItemFilterEnum filterEnum)
            {
                buildItemList(filterEnum);
            }
        }

        private void buildItemList(ItemFilterEnum filter = ItemFilterEnum.All, string? selectedItem = null)
        {
            _isProcessing = true;
            items.Clear();

            foreach (var item in itemsForFilter(filter).OrderBy(str => str))
            {
                var imageSource = ImageResolver.instance.imageSourceForItem(item);
                if (imageSource == null)
                {
                    continue;
                }

                var title = R.itemName(item);
                var itemView = new BaseSelectionWindow.ItemView { imageSource = imageSource, titleContent = title, filterableText = title };
                if (Config.instance.showIDsInSelectionWindow)
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
                items.Add(listItem);

                if (selectedItem == item)
                {
                    _selectedListBoxItem = listItem;
                }
            }
            _isProcessing = false;
        }

        private IEnumerable<string> itemsForFilter(ItemFilterEnum filter)
        {
            switch (filter)
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
