using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit.UI
{
    public interface ISelectionWindow
    {
        Action<string?>? onSelection { get; set; }

        void loadArmorProperties(string? id);
        void loadEnchantments(string? id);
        void loadFilteredItems(ItemFilterEnum filter, string? type);
        void loadItems(string? type);
        void Show();
    }

    public class BaseSelectionWindow
    {
        public static readonly BitmapImage? powerfulImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/Inspector/element_powerful");
        public static readonly BitmapImage? bulletImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Inspector/regular_bullit");

        public static void preload()
        {
            foreach (var item in ItemDatabase.all)
            {
                ImageResolver.instance.imageSourceForItem(item);
            }
            foreach (var enchantment in EnchantmentDatabase.allEnchantments)
            {
                ImageResolver.instance.imageSourceForEnchantment(enchantment);
            }
        }

        public class ArmorPropertyView : ItemView
        {
            public ArmorPropertyView()
            {
                image.Height = 25;
                image.Width = 25;
                image.Source = bulletImageSource;
            }
        }

        public class EnchantmentView : ItemView
        {
            public readonly Image powerfulImage;
            public bool powerful {
                get { return powerfulImage.Visibility == Visibility.Visible; }
                set { powerfulImage.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
            }

            public EnchantmentView()
            {
                powerfulImage = new Image {
                    Height = 25,
                    Width = 25,
                    Source = powerfulImageSource,
                    Visibility = Visibility.Collapsed,
                };
                Children.Add(powerfulImage);
            }
        }

        public class ItemView : StackPanel
        {
            public readonly Image image;
            public readonly Label titleLabel;
            public readonly Label subtitleLabel;

            public string filterableText { get; set; }

            public ImageSource? imageSource {
                get { return image.Source; }
                set { image.Source = value; }
            }

            public object titleContent {
                get { return titleLabel.Content; }
                set { titleLabel.Content = value; }
            }

            public object subtitleContent {
                get { return subtitleLabel.Content; }
                set { subtitleLabel.Content = value; updateSubtitleUI(); }
            }

            public ItemView()
            {
                image = new Image {
                    VerticalAlignment = VerticalAlignment.Center,
                };
                image.Height = 50;
                image.Width = 50;

                titleLabel = new Label {
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                };
                subtitleLabel = new Label {
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 10,
                };

                var labelStack = new StackPanel {
                    Height = 50,
                    //HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Orientation = Orientation.Vertical,
                };
                labelStack.Children.Add(titleLabel);
                labelStack.Children.Add(subtitleLabel);

                Orientation = Orientation.Horizontal;
                Children.Add(image);
                Children.Add(labelStack);
            }

            private void updateSubtitleUI()
            {
                if (subtitleLabel.Content == null)
                {
                    subtitleLabel.Visibility = Visibility.Collapsed;
                }
                else if (subtitleLabel.Content is string)
                {
                    subtitleLabel.Visibility = string.IsNullOrWhiteSpace(subtitleLabel.Content as string) ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    subtitleLabel.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
