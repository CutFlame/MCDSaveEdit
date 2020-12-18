using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable


namespace MCDSaveEdit
{
    public class MainlandMapScreen : MapScreen
    {
        private static readonly BitmapImage? _mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/missionselect_map_center_xbox");
        public static void preload() { }

        public MainlandMapScreen() : base(Constants.MAINLAND_LEVEL_DATA)
        {
            mapLabel.Content = R.getString("ArchIllagerRealm_name") ?? R.MAINLAND;

            if(_mapImageSource!= null)
            {
                var croppedImageSource = new CroppedBitmap(_mapImageSource, new Int32Rect(10, 83, 6136, 2975));
                var background = new ImageBrush(croppedImageSource);
                this.canvas.Background = background;
            }
        }

        
    }
}
