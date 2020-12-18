using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public class JungleAwakensMapScreen : MapScreen
    {
        private static readonly BitmapImage? _mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/DLC_Jungle_Island");
        public static void preload() { }

        public JungleAwakensMapScreen() : base(Constants.JUNGLE_AWAKENS_LEVEL_DATA)
        {
            mapLabel.Content = R.getString("TheJungleAwakens_name") ?? R.JUNGLE_AWAKENS;

            this.Background = new SolidColorBrush(Colors.LightCyan);

            if (_mapImageSource != null)
            {
                var croppedImageSource = new CroppedBitmap(_mapImageSource, new Int32Rect(0, 0, 2166, 1455));
                var background = new ImageBrush(croppedImageSource);
                this.canvas.Background = background;
            }
        }
    }
}
