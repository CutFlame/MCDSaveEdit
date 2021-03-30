using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;
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
        private static readonly BitmapImage? _levelFrameImageSource = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Character/STATS_LV_frame");
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
            levelLabelImage.Source = _levelFrameImageSource;

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
            //"UnlockRequirements_level"
            //"HUD_Level"
            levelLabel.Content = R.getString("HUD_Level") ?? R.LEVEL;
            allItemsButton.Content = R.ALL_ITEMS_FILTER;
            meleeItemsButton.Content = R.MELEE_ITEMS_FILTER;
            rangedItemsButton.Content = R.RANGED_ITEMS_FILTER;
            armorItemsButton.Content = R.ARMOR_ITEMS_FILTER;
            artifactItemsButton.Content = R.ARTIFACT_ITEMS_FILTER;
            enchantedItemsButton.Content = R.ENCHANTED_ITEMS_FILTER;
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

            model.profile.subscribe(_ => this.updateUI());
            model.level.subscribe(updateLevelUI);
            model.filteredItemList.subscribe(updateGridItemsUI);
            model.equippedItemList.subscribe(_ => this.updateEquippedItemsUI());
        }

        public void updateUI()
        {
            updateLevelUI(_model?.profile.value?.level());
            updateEquippedItemsUI();
            updateGridItemsUI(model?.filteredItemList.value);
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
                armorGearSlotItemControl.item = null;
                armorGearSlotButton.CommandParameter = null;
                rangedGearSlotItemControl.item = null;
                rangedGearSlotButton.CommandParameter = null;
                hotbarSlot1ItemControl.item = null;
                hotbarSlot1Button.CommandParameter = null;
                hotbarSlot2ItemControl.item = null;
                hotbarSlot2Button.CommandParameter = null;
                hotbarSlot3ItemControl.item = null;
                hotbarSlot3Button.CommandParameter = null;
                return;
            }
            var profile = _model!.profile.value!;

            meleeGearSlotItemControl.item = profile.meleeGearItem();
            meleeGearSlotButton.CommandParameter = meleeGearSlotItemControl.item;
            armorGearSlotItemControl.item = profile.armorGearItem();
            armorGearSlotButton.CommandParameter = armorGearSlotItemControl.item;
            rangedGearSlotItemControl.item = profile.rangedGearItem();
            rangedGearSlotButton.CommandParameter = rangedGearSlotItemControl.item;
            hotbarSlot1ItemControl.item = profile.hotbarSlot1Item();
            hotbarSlot1Button.CommandParameter = hotbarSlot1ItemControl.item;
            hotbarSlot2ItemControl.item = profile.hotbarSlot2Item();
            hotbarSlot2Button.CommandParameter = hotbarSlot2ItemControl.item;
            hotbarSlot3ItemControl.item = profile.hotbarSlot3Item();
            hotbarSlot3Button.CommandParameter = hotbarSlot3ItemControl.item;
        }

        private const int ITEMS_PER_ROW = 3;

        private void updateGridItemsUI(IEnumerable<Item>? items)
        {
            itemsGrid.RowDefinitions.Clear();
            itemsGrid.Children.Clear();

            if (_model == null || items == null)
            {
                inventoryCountLabel.Content = string.Empty;
                return;
            }

            int itemCount = 0;
            foreach (var item in items!)
            {
                var itemControl = new ItemControl();
                itemControl.item = item;
                itemControl.Height = 100;
                itemControl.Width = 100;

                var itemButton = new Button();
                itemButton.Background = null;
                itemButton.HorizontalAlignment = HorizontalAlignment.Center;
                itemButton.VerticalAlignment = VerticalAlignment.Center;
                itemButton.Height = 100;
                itemButton.Width = 100;
                itemButton.Margin = new Thickness(0);
                itemButton.Content = itemControl;
                itemButton.Command = new RelayCommand<Item>(_model!.selectItem);
                itemButton.CommandParameter = item;

                if (itemCount % ITEMS_PER_ROW == 0)
                {
                    var rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(100);
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
                newItemButton.Height = 100;
                newItemButton.Width = 100;
                newItemButton.Margin = new Thickness(0);
                newItemButton.Content = "+";
                newItemButton.Command = new RelayCommand<object>(_ => { this.addNewItemButton_Click(_model?.filter.value); });

                if (itemCount % ITEMS_PER_ROW == 0)
                {
                    var rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(100);
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
            model?.addItem(item);
            model?.selectItem(item);
        }

        private Item createDefaultItemForFilter(ItemFilterEnum filter)
        {
            var itemID = defaultItemIDForFilter(filter);
            return new Item() {
                MarkedNew = true,
                Upgraded = false,
                Power = 1,
                Rarity = Rarity.Common,
                Type = itemID,
            };
        }

        private string defaultItemIDForFilter(ItemFilterEnum filter)
        {
            switch (filter)
            {
                case ItemFilterEnum.MeleeWeapons: return Constants.DEFAULT_MELEE_WEAPON_ID;
                case ItemFilterEnum.Armor: return Constants.DEFAULT_ARMOR_ID;
                case ItemFilterEnum.RangedWeapons: return Constants.DEFAULT_RANGED_WEAPON_ID;
                case ItemFilterEnum.Artifacts: return Constants.DEFAULT_ARTIFACT_ID;
            }
            throw new ArgumentException($"Invalid filter value {filter}", "filter");
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
