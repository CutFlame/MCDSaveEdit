using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for EnchantmentSetControl.xaml
    /// </summary>
    public partial class EnchantmentSetControl : UserControl
    {
        public EnchantmentSetControl()
        {
            InitializeComponent();
            if (AppModel.gameContentLoaded)
            {
                useGameContentImages();
            }

            updateUI();
        }

        private void useGameContentImages()
        {
            backgroundImage.Source = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/StatusEffect/Enchantment/EnchantmentsBackground");
            topEnchantmentSymbolImage.Source = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Mobs/enchant_common_icon");
        }

        private Enchantment[]? _enchantments;
        public IEnumerable<Enchantment>? enchantments
        {
            get { return _enchantments; }
            set { _enchantments = value?.ToArray(); updateUI(); }
        }

        public void clearAll()
        {
            enchantment1Image.Source = null;
            enchantment1Button.CommandParameter = null;
            enchantment2Image.Source = null;
            enchantment2Button.CommandParameter = null;
            enchantment3Image.Source = null;
            enchantment3Button.CommandParameter = null;
            upgradedEnchantmentButton.Visibility = Visibility.Visible;
            upgradedEnchantmentImage.Source = AppModel.instance.imageSourceForEnchantment(Constants.DEFAULT_ENCHANTMENT_ID);
            upgradedEnchantmentButton.CommandParameter = null;
        }
        public void updateUI()
        {
            if(_enchantments == null || _enchantments.Length == 0)
            {
                clearAll();
                return;
            }

            var upgradedEnchantment = _enchantments.FirstOrDefault(x => x.Level > 0);
            if(upgradedEnchantment != null)
            {
                enchantment1Image.Source = null;
                enchantment1Button.CommandParameter = null;
                enchantment2Image.Source = null;
                enchantment2Button.CommandParameter = null;
                enchantment3Image.Source = null;
                enchantment3Button.CommandParameter = null;
                upgradedEnchantmentButton.Visibility = Visibility.Visible;
                upgradedEnchantmentImage.Source = AppModel.instance.imageSourceForEnchantment(upgradedEnchantment);
                upgradedEnchantmentButton.CommandParameter = upgradedEnchantment;
            }
            else
            {
                enchantment1Image.Source = AppModel.instance.imageSourceForEnchantment(_enchantments[0]);
                enchantment1Button.CommandParameter = _enchantments[0];
                enchantment2Image.Source = AppModel.instance.imageSourceForEnchantment(_enchantments[1]);
                enchantment2Button.CommandParameter = _enchantments[1];
                enchantment3Image.Source = AppModel.instance.imageSourceForEnchantment(_enchantments[2]);
                enchantment3Button.CommandParameter = _enchantments[2];
                upgradedEnchantmentButton.Visibility = Visibility.Collapsed;
                upgradedEnchantmentImage.Source = null;
                upgradedEnchantmentButton.CommandParameter = null;
            }
        }

        private ICommand? _command;
        public ICommand? command
        {
            get { return _command; }
            set { _command = value; updateCommand(); }
        }

        public void updateCommand()
        {
            enchantment1Button.Command = _command;
            enchantment2Button.Command = _command;
            enchantment3Button.Command = _command;
            upgradedEnchantmentButton.Command = _command;
        }
    }
}
