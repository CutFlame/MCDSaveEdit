using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for NetheriteEnchantmentControl.xaml
    /// </summary>
    public partial class NetheriteEnchantmentControl : UserControl
    {
        public NetheriteEnchantmentControl()
        {
            InitializeComponent();

            gildedButtonCheckBox.Content = R.getString("iteminspector_gilded") ?? R.GILDED;
        }


        private Item? _item;
        public Item? item {
            get { return _item; }
            set { _item = value; updateUI(); }
        }

        public Enchantment? enchantment {
            get { return _item?.NetheriteEnchant; }
            set { _item!.NetheriteEnchant = value; updateUI(); }
        }

        public void updateUI()
        {
            if (_item == null)
            {
                gildedButton.IsEnabled = false;
                gildedButtonCheckBox.IsEnabled = false;
                gildedButton.Visibility = Visibility.Visible;
                netheriteEnchantmentStack.Visibility = Visibility.Collapsed;
                netheriteEnchantmentRemoveButton.Visibility = Visibility.Collapsed;
                return;
            }

            if(_item.NetheriteEnchant == null)
            {
                gildedButton.IsEnabled = ImageUriHelper.gameContentLoaded;
                gildedButton.Visibility = Visibility.Visible;
                netheriteEnchantmentStack.Visibility = Visibility.Collapsed;
                netheriteEnchantmentRemoveButton.Visibility = Visibility.Collapsed;
                return;
            }

            gildedButton.IsEnabled = false;
            gildedButton.Visibility = Visibility.Collapsed;
            netheriteEnchantmentStack.Visibility = Visibility.Visible;
            netheriteEnchantmentButton.IsEnabled = ImageUriHelper.gameContentLoaded;
            netheriteEnchantmentRemoveButton.Visibility = Visibility.Visible;
            netheriteEnchantmentRemoveButton.IsEnabled = ImageUriHelper.gameContentLoaded;
            
            netheriteEnchantmentImage.Source = ImageUriHelper.instance.imageSourceForEnchantment(enchantment!);
            
            netheriteEnchantmentTextBox.Text = enchantment!.Level.ToString();
            netheriteEnchantmentLabel.Content = R.enchantmentName(enchantment!.Id);
        }


        private void gildedButton_Click(object sender, RoutedEventArgs e)
        {
            selectedEnchantmentId("Unset");
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            if (enchantment == null) { return; }
            if (int.TryParse(netheriteEnchantmentTextBox.Text, out int level) && level < Constants.MAXIMUM_ENCHANTMENT_TIER)
            {
                int newLevel = level + 1;
                EventLogger.logEvent("netheriteEnchantmentStepper_UpButtonClick", new Dictionary<string, object>() { { "newLevel", newLevel } });
                netheriteEnchantmentTextBox.Text = newLevel.ToString();
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (enchantment == null) { return; }
            if (int.TryParse(netheriteEnchantmentTextBox.Text, out int level) && level > Constants.MINIMUM_ENCHANTMENT_TIER)
            {
                int newLevel = level - 1;
                EventLogger.logEvent("netheriteEnchantmentStepper_DownButtonClick", new Dictionary<string, object>() { { "newLevel", newLevel } });
                netheriteEnchantmentTextBox.Text = newLevel.ToString();
            }
        }

        private void tierTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (enchantment == null) { return; }
            if (int.TryParse(netheriteEnchantmentTextBox.Text, out int level))
            {
                EventLogger.logEvent("tierTextBox_TextChanged", new Dictionary<string, object>() { { "level", level } });
                enchantment.Level = level;
                this.saveChanges?.Execute(item);
                //updateTierUI();
            }
        }

        private void enchantmentImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ImageUriHelper.gameContentLoaded) { return; }
            EventLogger.logEvent("enchantmentImageButton_Click", new Dictionary<string, object>() { { "enchantment", enchantment?.Id ?? "null" } });
            var selectionWindow = new SelectionWindow();
            selectionWindow.Owner = Application.Current.MainWindow;
            selectionWindow.loadEnchantments(enchantment?.Id);
            selectionWindow.onSelection = selectedEnchantmentId;
            selectionWindow.Show();
        }

        public ICommand? saveChanges { get; set; }

        private void selectedEnchantmentId(string? enchantmentId)
        {
            if (enchantmentId == null)
            {
                enchantment = null;
            }
            else
            {
                if (enchantment == null)
                {
                    enchantment = new Enchantment() { Id = enchantmentId!, Level = 0, };
                }
                else
                {
                    enchantment.Id = enchantmentId!;
                }
            }
            this.saveChanges?.Execute(item);
            updateUI();
        }

        private void netheriteEnchantmentRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            selectedEnchantmentId(null);
        }
    }
}
