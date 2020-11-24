using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapWindow()
        {
            InitializeComponent();

            var mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/missionselect_map_center_xbox");
            var croppedImageSource = new CroppedBitmap(mapImageSource, new Int32Rect(10, 83, 6136, 2975));
            var background = new ImageBrush(croppedImageSource);
            this.Background = background;
        }
    }
}
