using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public class JungleAwakensMapScreen : MapScreen
    {
        public JungleAwakensMapScreen() : base(Constants.JUNGLE_AWAKENS_LEVEL_DATA)
        {
            mapLabel.Content = R.getString("TheJungleAwakens_name") ?? R.JUNGLE_AWAKENS;

            var mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/DLC_Jungle_Island");
            if (mapImageSource != null)
            {
                var croppedImageSource = new CroppedBitmap(mapImageSource, new Int32Rect(0, 0, 2166, 1455));
                var background = new ImageBrush(croppedImageSource);
                this.Background = background;
            }
        }
    }
}
