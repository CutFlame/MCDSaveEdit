using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public enum LevelTypeEnum
        {
            mission,
            dungeon,
        }

        public struct StaticLevelData
        {
            public string key;
            public Point mapPosition;
            public LevelTypeEnum levelType;

            public StaticLevelData(string key, Point mapPosition, LevelTypeEnum levelType) : this()
            {
                this.key = key;
                this.mapPosition = mapPosition;
                this.levelType = levelType;
            }
        }

        public static StaticLevelData[] _staticLevelData = new StaticLevelData[] {
            //new StaticLevelData(".00", new Point(0.00, 0.00), LevelTypeEnum.mission),
            //new StaticLevelData(".05", new Point(0.05, 0.05), LevelTypeEnum.mission),
            //new StaticLevelData(".10", new Point(0.10, 0.10), LevelTypeEnum.mission),
            //new StaticLevelData(".15", new Point(0.15, 0.15), LevelTypeEnum.mission),
            //new StaticLevelData(".20", new Point(0.20, 0.20), LevelTypeEnum.mission),
            //new StaticLevelData(".25", new Point(0.25, 0.25), LevelTypeEnum.mission),
            //new StaticLevelData(".30", new Point(0.30, 0.30), LevelTypeEnum.mission),
            //new StaticLevelData(".35", new Point(0.35, 0.35), LevelTypeEnum.mission),
            //new StaticLevelData(".40", new Point(0.40, 0.40), LevelTypeEnum.mission),
            //new StaticLevelData(".45", new Point(0.45, 0.45), LevelTypeEnum.mission),
            //new StaticLevelData(".50", new Point(0.50, 0.50), LevelTypeEnum.mission),
            //new StaticLevelData(".55", new Point(0.55, 0.55), LevelTypeEnum.mission),
            //new StaticLevelData(".60", new Point(0.60, 0.60), LevelTypeEnum.mission),
            //new StaticLevelData(".65", new Point(0.65, 0.65), LevelTypeEnum.mission),
            //new StaticLevelData(".70", new Point(0.70, 0.70), LevelTypeEnum.mission),
            //new StaticLevelData(".75", new Point(0.75, 0.75), LevelTypeEnum.mission),
            //new StaticLevelData(".80", new Point(0.80, 0.80), LevelTypeEnum.mission),
            //new StaticLevelData(".85", new Point(0.85, 0.85), LevelTypeEnum.mission),
            //new StaticLevelData(".90", new Point(0.90, 0.90), LevelTypeEnum.mission),
            //new StaticLevelData(".95", new Point(0.95, 0.95), LevelTypeEnum.mission),
            //new StaticLevelData("1.0", new Point(1.00, 1.00), LevelTypeEnum.mission),

            new StaticLevelData("creepycrypt", new Point(.19, .37), LevelTypeEnum.dungeon),
            new StaticLevelData("mooshroomisland", new Point(.26, .24), LevelTypeEnum.dungeon),
            new StaticLevelData("creeperwoods", new Point(.28, .40), LevelTypeEnum.mission),
            new StaticLevelData("soggycave", new Point(.27, .65), LevelTypeEnum.dungeon),
            new StaticLevelData("soggyswamp", new Point(.29, .74), LevelTypeEnum.mission),
            new StaticLevelData("mooncorecaverns", new Point(.44, .35), LevelTypeEnum.mission),
            new StaticLevelData("cacticanyon", new Point(.42, .56), LevelTypeEnum.mission),
            new StaticLevelData("pumpkinpastures", new Point(.46, .70), LevelTypeEnum.mission),
            new StaticLevelData("archhaven", new Point(.57, .73), LevelTypeEnum.dungeon),
            new StaticLevelData("deserttemple", new Point(.59, .45), LevelTypeEnum.mission),
            new StaticLevelData("fieryforge", new Point(.63, .20), LevelTypeEnum.mission),
            new StaticLevelData("lowertemple", new Point(.67, .59), LevelTypeEnum.dungeon),
            new StaticLevelData("highblockhalls", new Point(.75, .40), LevelTypeEnum.mission),
            new StaticLevelData("obsidianpinnacle", new Point(.86, .25), LevelTypeEnum.mission),
            new StaticLevelData("underhalls", new Point(.87, .49), LevelTypeEnum.dungeon),
            new StaticLevelData("squidcoast", new Point(.15, .64), LevelTypeEnum.mission),
        };

        private const double imageRadius = 16;

        private Dictionary<string, Panel> _missionElements = new Dictionary<string, Panel>();
        public MapWindow()
        {
            InitializeComponent();

            var mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/missionselect_map_center_xbox");
            var croppedImageSource = new CroppedBitmap(mapImageSource, new Int32Rect(10, 83, 6136, 2975));
            var background = new ImageBrush(croppedImageSource);
            this.Background = background;

            var missionImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/mission_marker_front");
            var dungeonImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/shield_dungeon");
            var lockedDungeonImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/locked_dungeons");
            var unlockedDungeonImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/icon_dungeon");
            var difficulty1ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level1");
            var difficulty2ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level2");
            var difficulty3ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level3");
            var difficulty4ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level4");
            var difficulty5ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level5");

            foreach (var staticLevelData in _staticLevelData)
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
                button.Height = imageRadius*2;
                button.Width = imageRadius*2;
                button.Background = null;
                button.Content = levelImagePanel;

                var label = new Label();
                label.Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
                label.Foreground = new SolidColorBrush(Colors.White);
                label.FontWeight = FontWeights.ExtraBold;
                label.Content = R.getMissionName(staticLevelData.key);

                var panel = new DockPanel();
                panel.Children.Add(button);
                panel.Children.Add(label);
                DockPanel.SetDock(button, Dock.Top);
                DockPanel.SetDock(label, Dock.Bottom);


                canvas.Children.Add(panel);

                _missionElements.Add(staticLevelData.key, panel);

            }

            positionLevels();
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            positionLevels();
        }

        private void positionLevels()
        {
            var mapWidth = canvas.ActualWidth;
            var mapHeight = canvas.ActualHeight;
            foreach (var staticLevelData in _staticLevelData)
            {
                var element = _missionElements[staticLevelData.key];
                Canvas.SetLeft(element, staticLevelData.mapPosition.X * mapWidth - (element.ActualWidth / 2));
                Canvas.SetTop(element, staticLevelData.mapPosition.Y * mapHeight - imageRadius);
            }

        }
    }
}
