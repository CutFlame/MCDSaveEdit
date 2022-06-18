using MCDSaveEdit.Data;
using MCDSaveEdit.Logic;
using MCDSaveEdit.Services;
using MCDSaveEdit.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#nullable enable

namespace MCDSaveEdit.UI
{
    /// <summary>
    /// Interaction logic for MapScreen.xaml
    /// </summary>
    public partial class MapScreen : UserControl
    {
        public static void preload()
        {
            Constants.ALL_MAP_IMAGE_DATA.ForEach(mapImageData => mapImageData.preload());
        }

        private ProfileViewModel? _model;
        public ProfileViewModel? model { get => _model; set { _model = value; missionScreen.model = _model; } }

        protected Dictionary<string, MissionControl> _missionElements = new Dictionary<string, MissionControl>();
        protected IEnumerable<StaticLevelData> _levelData;

        public MapScreen(MapImageData mapImageData)
        {
            InitializeComponent();

            _levelData = mapImageData.levelData;
            foreach (var staticLevelData in _levelData)
            {
                var panel = new MissionControl(staticLevelData.levelType);
                panel.text = R.getMissionName(staticLevelData.key);
                panel.Command = new RelayCommand<string>(this.missionScreen.setMissionInfo);
                panel.CommandParameter = staticLevelData.key;

                canvas.Children.Add(panel);
                _missionElements.Add(staticLevelData.key, panel);
            }

            mapLabel.Content = mapImageData.title();

            var tuple = mapImageData.backgroundColor;
            var color = Color.FromArgb(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
            this.Background = new SolidColorBrush(color);

            var mapImageSource = mapImageData.usableImageSource();
            if (mapImageSource != null)
            {
                var background = new ImageBrush(mapImageSource);
                this.canvas.Background = background;
            }

            updateUI();
        }

        public void updateUI()
        {
            var profile = model?.profile.value;
            if (profile == null)
            {
                foreach (var panel in _missionElements.Values)
                {
                    panel.Visibility = Visibility.Collapsed;
                }
                this.missionScreen.setMissionInfo(null);
                return;
            }

            var prerequisites = profile!.BonusPrerequisites;
            var progress = profile!.Progress;
            foreach (var level in _levelData.Select(levelData => levelData.key))
            {
                var panel = _missionElements[level];
                panel.Visibility = Visibility.Visible;
                if (progress == null || !progress.ContainsKey(level))
                {
                    panel.locked = !prerequisites.Contains(level);
                    continue;
                }

                panel.locked = false;
                var levelProgress = progress[level];
                panel.difficultyLevel = levelProgress.getDifficultyImageLevel();
            }
            positionLevels();
        }

        protected void screen_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            positionLevels();
        }

        private void positionLevels()
        {
            canvas.UpdateLayout();
            var mapWidth = canvas.ActualWidth;
            var mapHeight = canvas.ActualHeight;
            foreach (var staticLevelData in _levelData)
            {
                var element = _missionElements[staticLevelData.key];
                Canvas.SetLeft(element, (staticLevelData.mapPosition.X * mapWidth) - (element.ActualWidth / 2));
                Canvas.SetTop(element, (staticLevelData.mapPosition.Y * mapHeight) - (LevelImagePanel.IMAGE_RADIUS));
            }
        }

    }
}
