using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for MainMapScreen.xaml
    /// </summary>
    public partial class MainMapScreen : UserControl
    {
        private const double imageRadius = 16;

        private Dictionary<string, Panel> _missionElements = new Dictionary<string, Panel>();

        public MainMapScreen()
        {
            InitializeComponent();

            var mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/missionselect_map_center_xbox");
            if(mapImageSource!= null)
            {
                var croppedImageSource = new CroppedBitmap(mapImageSource, new Int32Rect(10, 83, 6136, 2975));
                var background = new ImageBrush(croppedImageSource);
                this.Background = background;
            }

            var missionImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/mission_marker_front");
            var dungeonImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/shield_dungeon");
            var lockedDungeonImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/locked_dungeons");
            var unlockedDungeonImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/icon_dungeon");
            var difficulty1ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level1");
            var difficulty2ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level2");
            var difficulty3ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level3");
            var difficulty4ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level4");
            var difficulty5ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level5");

            foreach (var staticLevelData in Constants._staticLevelData)
            {
                var levelTypeImage = new Image();
                var levelDifficultyImage = new Image();
                switch (staticLevelData.levelType)
                {
                    case LevelTypeEnum.mission:
                        levelTypeImage.Source = missionImageSource;
                        levelDifficultyImage.Source = difficulty5ImageSource;
                        levelDifficultyImage.Width = imageRadius * .95;
                        levelDifficultyImage.Height = imageRadius * .95;
                        levelDifficultyImage.HorizontalAlignment = HorizontalAlignment.Center;
                        levelDifficultyImage.VerticalAlignment = VerticalAlignment.Center;
                        break;
                    case LevelTypeEnum.dungeon:
                        levelTypeImage.Source = dungeonImageSource;
                        levelDifficultyImage.Width = imageRadius;
                        levelDifficultyImage.Height = imageRadius;
                        levelDifficultyImage.HorizontalAlignment = HorizontalAlignment.Center;
                        levelDifficultyImage.VerticalAlignment = VerticalAlignment.Center;
                        if (true) //(levelUnlocked)
                        {
                            levelDifficultyImage.Source = unlockedDungeonImageSource;
                        }
                        else
                        {
                            levelDifficultyImage.Source = lockedDungeonImageSource;
                        }
                        break;
                }

                var levelImagePanel = new Grid();
                levelImagePanel.Children.Add(levelTypeImage);
                levelImagePanel.Children.Add(levelDifficultyImage);

                var button = new Button();
                button.Height = imageRadius * 2;
                button.Width = imageRadius * 2;
                button.Background = null;
                button.Content = levelImagePanel;

                var label = new Label();
                label.Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
                label.Foreground = new SolidColorBrush(Colors.White);
                label.FontWeight = FontWeights.ExtraBold;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.Content = R.getMissionName(staticLevelData.key);

                var panel = new DockPanel();
                panel.Children.Add(button);
                panel.Children.Add(label);
                DockPanel.SetDock(button, Dock.Top);
                DockPanel.SetDock(label, Dock.Bottom);
                panel.Visibility = Visibility.Collapsed;


                canvas.Children.Add(panel);

                _missionElements.Add(staticLevelData.key, panel);

            }

            positionLevels();
        }

        private void screen_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            positionLevels();
        }

        private void positionLevels()
        {
            var mapWidth = canvas.ActualWidth;
            var mapHeight = canvas.ActualHeight;
            foreach (var staticLevelData in Constants._staticLevelData)
            {
                var element = _missionElements[staticLevelData.key];
                Canvas.SetLeft(element, staticLevelData.mapPosition.X * mapWidth - (element.ActualWidth / 2));
                Canvas.SetTop(element, staticLevelData.mapPosition.Y * mapHeight - imageRadius);
            }

        }

    }
}
