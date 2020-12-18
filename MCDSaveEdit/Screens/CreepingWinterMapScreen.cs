using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public class CreepingWinterMapScreen : MapScreen
    {
        public CreepingWinterMapScreen() : base(Constants.CREEPING_WINTER_LEVEL_DATA)
        {
            mapLabel.Content = R.getString("TheCreepingWinter_name") ?? R.CREEPING_WINTER;

            this.Background = new SolidColorBrush(Colors.LightCyan);

            var mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/DLC_Snowy_Island");
            if (mapImageSource != null)
            {
                var croppedImageSource = new CroppedBitmap(mapImageSource, new Int32Rect(0, 0, 2211, 1437));
                var background = new ImageBrush(croppedImageSource);
                this.canvas.Background = background;
            }
        }
    }
}
