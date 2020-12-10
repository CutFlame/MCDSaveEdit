using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public class HowlingPeaksMapScreen : MapScreen
    {
        public HowlingPeaksMapScreen() : base(Constants.HOWLING_PEAKS_LEVEL_DATA)
        {
            mapLabel.Content = R.getString("TheHowlingPeaks_name") ?? R.HOWLING_PEAKS;

            this.Background = new SolidColorBrush(Colors.LightCyan);

            var mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/Mountain_base_NOTPOTWO");
            if (mapImageSource != null)
            {
                var croppedImageSource = new CroppedBitmap(mapImageSource, new Int32Rect(0, 0, 2466, 2414));
                var background = new ImageBrush(croppedImageSource);
                this.grid.Background = background;
            }
        }
    }
}
