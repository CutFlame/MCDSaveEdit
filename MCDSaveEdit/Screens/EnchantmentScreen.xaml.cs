using DungeonTools.Save.Models.Profiles;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for EnchantmentScreen.xaml
    /// </summary>
    public partial class EnchantmentScreen : UserControl
    {
        public EnchantmentScreen()
        {
            InitializeComponent();
            enchantmentBackgroundImage.Source = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/Inspector2/lv0_frame");
            enchantmentPointsSymbolImage.Source = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/enchant_counter");
            tierLabel.Content = R.TIER;

            updateUI();
        }

        private Enchantment? _enchantment;
        public Enchantment? enchantment
        {
            get { return _enchantment; }
            set { _enchantment = value; updateUI(); }
        }

        private void updateUI()
        {
            if(_enchantment == null)
            {
                powerfulImage.Source = null;
                powerfulLabel.Content = string.Empty;
                enchantmentImage.Source = null;
                enchantmentLabel.Content = string.Empty;
                updateTierUI();
                return;
            }

            if (_enchantment.Type.isPowerful())
            {
                powerfulImage.Source = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/Inspector/element_powerful");
                powerfulLabel.Content = R.POWERFUL;
            }
            else
            {
                powerfulImage.Source = null;
                powerfulLabel.Content = R.COMMON;
            }

            enchantmentImage.Source = ImageUriHelper.instance.imageSourceForEnchantment(_enchantment.Type);
            enchantmentLabel.Content = _enchantment.Type;

            updateTierUI();
        }

        private void updateTierUI()
        {
            if(_enchantment != null)
            {
                var enchantmentLevelImage = imageForEnchantmentLevel(_enchantment!.Level);
                var croppedImage = new CroppedBitmap(enchantmentLevelImage, new Int32Rect(0, 0, 976, 959));
                enchantmentBackgroundImage.Source = croppedImage;
                tierTextBox.Text = _enchantment!.Level.ToString();
                pointsCostLabel.Content = _enchantment!.pointsCost().ToString();
            }
            else
            {
                enchantmentBackgroundImage.Source = null;
                tierTextBox.Text = string.Empty;
                pointsCostLabel.Content = string.Empty;
            }
        }

        private BitmapImage? imageForEnchantmentLevel(int level)
        {
            switch(level)
            {
                case 1: return ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/Inspector2/lv1_frame");
                case 2: return ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/Inspector2/lv2_frame");
                case 3: return ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/Inspector2/lv3_frame");
            }
            return ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/Inspector2/lv0_frame");
        }

        public ICommand? saveChanges { get; set; }
        public ICommand? close { get; set; }

        private void tierTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_enchantment == null) { return; }
            if (int.TryParse(tierTextBox.Text, out int level))
            {
                _enchantment.Level = level;
                this.saveChanges?.Execute(_enchantment);
                updateTierUI();
            }
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            if (_enchantment == null) { return; }
            if (int.TryParse(tierTextBox.Text, out int level) && level < Constants.MAXIMUM_ENCHANTMENT_TIER)
            {
                int newLevel = level + 1;
                tierTextBox.Text = newLevel.ToString();
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (_enchantment == null) { return; }
            if (int.TryParse(tierTextBox.Text, out int level) && level > Constants.MINIMUM_ENCHANTMENT_TIER)
            {
                int newLevel = level - 1;
                tierTextBox.Text = newLevel.ToString();
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            close?.Execute(null);
        }
    }
}
