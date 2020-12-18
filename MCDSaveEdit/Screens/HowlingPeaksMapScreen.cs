using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public class HowlingPeaksMapScreen : MapScreen
    {
        private static readonly BitmapImage? _mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/Mountain_base_NOTPOTWO");
        public static void preload() { }

        public HowlingPeaksMapScreen() : base(Constants.HOWLING_PEAKS_LEVEL_DATA)
        {
            mapLabel.Content = R.getString("TheHowlingPeaks_name") ?? R.HOWLING_PEAKS;

            this.Background = new SolidColorBrush(Colors.LightCyan);

            if (_mapImageSource != null)
            {
                var croppedImageSource = new CroppedBitmap(_mapImageSource, new Int32Rect(0, 0, 2466, 2414));
                var background = new ImageBrush(croppedImageSource);
                this.canvas.Background = background;
            }
        }
    }
}
