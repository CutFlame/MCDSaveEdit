using DungeonTools.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for InventoryScreen.xaml
    /// </summary>
    public partial class InventoryScreen : UserControl
    {
        private ProfileViewModel? _model;
        public ProfileViewModel? model
        {
            get { return _model; }
            set
            {
                _model = value;
                setupCommands();
                updateUI();
            }
        }

        public InventoryScreen()
        {
            InitializeComponent();
            translateStaticStrings();
            if (ImageUriHelper.gameContentLoaded)
            {
                useGameContentImages();
            }
        }

        private void useGameContentImages()
        {
            levelLabelImage.Source = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Character/STATS_LV_frame");

            tryLoadAndSetImage(allItemsButton, "/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_all_default");
            tryLoadAndSetImage(meleeItemsButton, "/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_melee_default");
            tryLoadAndSetImage(rangedItemsButton, "/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_ranged_default");
            tryLoadAndSetImage(armorItemsButton, "/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_armour_default");
            tryLoadAndSetImage(artifactItemsButton, "/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_consume_default");
            tryLoadAndSetImage(enchantedItemsButton, "/Dungeons/Content/UI/Materials/Inventory2/Filter/filter_enchant_default");
        }

        private void tryLoadAndSetImage(Button button, string imagePath)
        {
            var imageSource = ImageUriHelper.instance.imageSource(imagePath);
            if (imageSource != null)
            {
                var image = new Image();
                image.Source = imageSource;
                button.Content = image;
            }
        }

        private void translateStaticStrings()
        {
            levelLabel.Content = R.LEVEL;
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

                if (itemCount % 3 == 0)
                {
                    var rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(100);
                    itemsGrid.RowDefinitions.Add(rowDef);
                }

                itemsGrid.Children.Add(itemButton);
                Grid.SetRow(itemButton, itemCount / 3);
                Grid.SetColumn(itemButton, itemCount % 3);

                itemCount++;
            }

            inventoryCountLabel.Content = R.formatITEMS_COUNT_LABEL(itemCount);
        }


        private void levelTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_model?.profile.value == null || !levelTextBox.IsEnabled) { return; }
            if (int.TryParse(levelTextBox.Text, out int level))
            {
                _model!.level.setValue = level;
            }
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null || !levelTextBox.IsEnabled) { return; }
            if (int.TryParse(levelTextBox.Text, out int level) && level < Constants.MAXIMUM_CHARACTER_LEVEL)
            {
                int newLevel = level + 1;
                _model!.level.setValue = newLevel;
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null || !levelTextBox.IsEnabled) { return; }
            if (int.TryParse(levelTextBox.Text, out int level) && level > Constants.MINIMUM_CHARACTER_LEVEL)
            {
                int newLevel = level - 1;
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
            setItemFilter(ItemFilterEnum.All);
        }
        private void allMeleeItemsButton_Click(object sender, RoutedEventArgs e)
        {
            setItemFilter(ItemFilterEnum.MeleeWeapons);
        }
        private void allRangedItemsButton_Click(object sender, RoutedEventArgs e)
        {
            setItemFilter(ItemFilterEnum.RangedWeapons);
        }
        private void allArmorItemsButton_Click(object sender, RoutedEventArgs e)
        {
            setItemFilter(ItemFilterEnum.Armor);
        }
        private void allArtifactItemsButton_Click(object sender, RoutedEventArgs e)
        {
            setItemFilter(ItemFilterEnum.Artifacts);
        }
        private void allEnchantedItemsButton_Click(object sender, RoutedEventArgs e)
        {
            setItemFilter(ItemFilterEnum.Enchanted);
        }

    }
}
