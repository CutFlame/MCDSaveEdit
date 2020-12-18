using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public class MainlandMapScreen : MapScreen
    {
        public MainlandMapScreen() : base(Constants.MAINLAND_LEVEL_DATA)
        {
            mapLabel.Content = R.getString("ArchIllagerRealm_name") ?? R.MAINLAND;

            var mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/missionselect_map_center_xbox");
            if(mapImageSource!= null)
            {
                var croppedImageSource = new CroppedBitmap(mapImageSource, new Int32Rect(10, 83, 6136, 2975));
                var background = new ImageBrush(croppedImageSource);
                this.canvas.Background = background;
            }
        }
    }
}
