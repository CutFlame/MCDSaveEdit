using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
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
    /// Interaction logic for InventoryScreen.xaml
    /// </summary>
    public partial class InventoryScreen : UserControl
    {
        private static readonly BitmapImage? _allItemsButtonImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_all_default");
        private static readonly BitmapImage? _meleeItemsButtonImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_melee_default");
        private static readonly BitmapImage? _rangedItemsButtonImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_ranged_default");
        private static readonly BitmapImage? _armorItemsButtonImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_armour_default");
        private static readonly BitmapImage? _artifactItemsButtonImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_consume_default");
        private static readonly BitmapImage? _enchantedItemsButtonImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_enchant_default");

        public static void preload() { }

        private ProfileViewModel? _model;
        public ProfileViewModel? model
        {
            get { return _model; }
            set
            {
                _model = value;
                equipmentScreen.model = _model;
                setupCommands();
                //updateUI();
            }
        }

        public InventoryScreen()
        {
            InitializeComponent();
            translateStaticStrings();
            if (AppModel.gameContentLoaded)
            {
                useGameContentImages();
            }
            meleeItemsButton.IsEnabled = AppModel.gameContentLoaded;
            rangedItemsButton.IsEnabled = AppModel.gameContentLoaded;
            armorItemsButton.IsEnabled = AppModel.gameContentLoaded;
            artifactItemsButton.IsEnabled = AppModel.gameContentLoaded;
        }

        private void useGameContentImages()
        {
            tryLoadAndSetImage(allItemsButton, _allItemsButtonImageSource);
            tryLoadAndSetImage(meleeItemsButton, _meleeItemsButtonImageSource);
            tryLoadAndSetImage(rangedItemsButton, _rangedItemsButtonImageSource);
            tryLoadAndSetImage(armorItemsButton, _armorItemsButtonImageSource);
            tryLoadAndSetImage(artifactItemsButton, _artifactItemsButtonImageSource);
            tryLoadAndSetImage(enchantedItemsButton, _enchantedItemsButtonImageSource);
        }

        private void tryLoadAndSetImage(Button button, ImageSource? imageSource)
        {
            if (imageSource != null)
            {
                var image = new Image();
                image.Source = imageSource;
                button.Content = image;
            }
        }

        private void translateStaticStrings()
        {
            allItemsButton.Content = R.getString("ItemTag_All") ?? R.ALL_ITEMS_FILTER;
            meleeItemsButton.Content = R.getString("ItemTag_Melee") ?? R.MELEE_ITEMS_FILTER;
            rangedItemsButton.Content = R.getString("ItemTag_Ranged") ?? R.RANGED_ITEMS_FILTER;
            armorItemsButton.Content = R.getString("ItemTag_Armor") ?? R.ARMOR_ITEMS_FILTER;
            artifactItemsButton.Content = R.getString("ItemTag_Items") ?? R.ARTIFACT_ITEMS_FILTER;
            enchantedItemsButton.Content = R.getString("ItemTag_Enchanted") ?? R.ENCHANTED_ITEMS_FILTER;
        }

        private void setupCommands()
        {
            if (_model == null) { return; }
            var model = _model!;

            model.filteredItemList.subscribe(updateGridItemsUIReloadingAll);
        }

        public void updateUI()
        {
            updateGridItemsUI(model?.filteredItemList.value);
            equipmentScreen.updateUI();
        }

        private const int ITEMS_PER_ROW = 3;
        private const double INVENTORY_ITEM_SIDE_LENGTH = 100;

        private void updateGridItemsUIReloadingAll(IEnumerable<Item>? items)
        {
            updateGridItemsUI(items, true);
        }

        private void updateGridItemsUI(IEnumerable<Item>? items, bool forceReloadAll = false)
        {
            if (_model == null || items == null)
            {
                inventoryCountLabel.Content = string.Empty;
                itemsGrid.RowDefinitions.Clear();
                itemsGrid.Children.Clear();
                return;
            }

            //Optimization. Only reload the grid if necessary or forced
            if(forceReloadAll == false && itemsGrid.Children.Count == items.Count())
            {
                return;
            }

            itemsGrid.RowDefinitions.Clear();
            itemsGrid.Children.Clear();

            int itemCount = 0;
            foreach (var item in items!)
            {
                var itemControl = new ItemControl();
                itemControl.item = item;
                itemControl.Height = INVENTORY_ITEM_SIDE_LENGTH;
                itemControl.Width = INVENTORY_ITEM_SIDE_LENGTH;

                var itemButton = new Button();
                itemButton.Background = null;
                itemButton.HorizontalAlignment = HorizontalAlignment.Center;
                itemButton.VerticalAlignment = VerticalAlignment.Center;
                itemButton.Height = INVENTORY_ITEM_SIDE_LENGTH;
                itemButton.Width = INVENTORY_ITEM_SIDE_LENGTH;
                itemButton.Margin = new Thickness(0);
                itemButton.Content = itemControl;
                itemButton.Command = new RelayCommand<Item>(_model!.selectItem);
                itemButton.CommandParameter = item;

                if (itemCount % ITEMS_PER_ROW == 0)
                {
                    var rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(INVENTORY_ITEM_SIDE_LENGTH);
                    itemsGrid.RowDefinitions.Add(rowDef);
                }

                itemsGrid.Children.Add(itemButton);
                Grid.SetRow(itemButton, itemCount / ITEMS_PER_ROW);
                Grid.SetColumn(itemButton, itemCount % ITEMS_PER_ROW);

                itemCount++;
            }

            var currentFilter = _model?.filter.value;
            if (currentFilter != null && currentFilter != ItemFilterEnum.Enchanted && currentFilter != ItemFilterEnum.All)
            {
                var newItemButton = new Button();
                newItemButton.HorizontalAlignment = HorizontalAlignment.Center;
                newItemButton.VerticalAlignment = VerticalAlignment.Center;
                newItemButton.Height = INVENTORY_ITEM_SIDE_LENGTH;
                newItemButton.Width = INVENTORY_ITEM_SIDE_LENGTH;
                newItemButton.Margin = new Thickness(0);
                newItemButton.Content = "+";
                newItemButton.Command = new RelayCommand<object>(_ => { this.addNewItemButton_Click(_model?.filter.value); });

                if (itemCount % ITEMS_PER_ROW == 0)
                {
                    var rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(INVENTORY_ITEM_SIDE_LENGTH);
                    itemsGrid.RowDefinitions.Add(rowDef);
                }

                itemsGrid.Children.Add(newItemButton);
                Grid.SetRow(newItemButton, itemCount / ITEMS_PER_ROW);
                Grid.SetColumn(newItemButton, itemCount % ITEMS_PER_ROW);
            }

            string? gameContentString = R.getString("inventory_count");
            if(gameContentString != null)
            {
                gameContentString = gameContentString
                    .Replace("{current}", itemCount.ToString())
                    .Replace("{max}", Constants.MAXIMUM_INVENTORY_ITEM_COUNT.ToString());
            }
            else
            {
                gameContentString = R.formatITEMS_COUNT_LABEL(itemCount, Constants.MAXIMUM_INVENTORY_ITEM_COUNT);
            }
            inventoryCountLabel.Content = gameContentString;
        }

        private void addNewItemButton_Click(ItemFilterEnum? currentFilter)
        {
            if (currentFilter == null || currentFilter == ItemFilterEnum.Enchanted || currentFilter == ItemFilterEnum.All) { return; }
            EventLogger.logEvent("addNewItemButton_Click", new Dictionary<string, object>() { { "currentFilter", currentFilter.ToString() } });
            var item = createDefaultItemForFilter(currentFilter!.Value);
            model?.addItemToInventory(item);
            model?.selectItem(item);
        }

        private Item createDefaultItemForFilter(ItemFilterEnum filter)
        {
            var itemID = Constants.defaultItemIDForFilter(filter);
            return new Item() {
                MarkedNew = true,
                Upgraded = false,
                Power = 1,
                Rarity = Rarity.Common,
                Type = itemID,
            };
        }

        private void setItemFilter(ItemFilterEnum filter)
        {
            if(_model == null) { return; }
            _model!.filter.setValue = filter;
        }

        private void allItemsButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("allItemsButton_Click");
            setItemFilter(ItemFilterEnum.All);
        }
        private void allMeleeItemsButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("allMeleeItemsButton_Click");
            setItemFilter(ItemFilterEnum.MeleeWeapons);
        }
        private void allRangedItemsButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("allRangedItemsButton_Click");
            setItemFilter(ItemFilterEnum.RangedWeapons);
        }
        private void allArmorItemsButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("allArmorItemsButton_Click");
            setItemFilter(ItemFilterEnum.Armor);
        }
        private void allArtifactItemsButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("allArtifactItemsButton_Click");
            setItemFilter(ItemFilterEnum.Artifacts);
        }
        private void allEnchantedItemsButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("allEnchantedItemsButton_Click");
            setItemFilter(ItemFilterEnum.Enchanted);
        }

    }
}
