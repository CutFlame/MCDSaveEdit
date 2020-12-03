using MCDSaveEdit.Save.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for MainlandMapScreen.xaml
    /// </summary>
    public partial class MainlandMapScreen : UserControl
    {
        public ProfileViewModel? model { get; set; }


        private Dictionary<string, MissionControl> _missionElements = new Dictionary<string, MissionControl>();
        private readonly IEnumerable<StaticLevelData> _levelData;

        public MainlandMapScreen()
        {
            InitializeComponent();
            _levelData = Constants.MAINLAND_LEVEL_DATA;
            mapLabel.Content = R.getString("ArchIllagerRealm_name") ?? R.MAINLAND;

            var mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/missionselect_map_center_xbox");
            if(mapImageSource!= null)
            {
                var croppedImageSource = new CroppedBitmap(mapImageSource, new Int32Rect(10, 83, 6136, 2975));
                var background = new ImageBrush(croppedImageSource);
                this.Background = background;
            }

            foreach (var staticLevelData in _levelData)
            {
                var panel = new MissionControl(staticLevelData.levelType);
                panel.text = R.getMissionName(staticLevelData.key);

                canvas.Children.Add(panel);
                _missionElements.Add(staticLevelData.key, panel);
            }

            updateUI();
        }

        public void updateUI()
        {
            if(model?.profile.value == null)
            {
                foreach(var panel in _missionElements.Values)
                {
                    panel.Visibility = Visibility.Collapsed;
                }
                return;
            }
            var prerequisites = model!.profile.value!.BonusPrerequisites;
            var progress = model!.profile.value!.Progress;
            foreach (var level in _levelData.Select(level=>level.key))
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
                uint difficulty = 1;
                if (levelProgress.CompletedDifficulty == DifficultyEnum.Difficulty_1)
                {
                    difficulty = 2;
                }
                else if (levelProgress.CompletedDifficulty == DifficultyEnum.Difficulty_2)
                {
                    difficulty = 3;
                }
                else if (levelProgress.CompletedDifficulty == DifficultyEnum.Difficulty_3 && levelProgress.CompletedEndlessStruggle == 0)
                {
                    difficulty = 4;
                }
                else if (levelProgress.CompletedDifficulty == DifficultyEnum.Difficulty_3 && levelProgress.CompletedEndlessStruggle > 0)
                {
                    difficulty = 5;
                }
                panel.difficultyLevel = difficulty;
            }
            positionLevels();
        }

        private void screen_SizeChanged(object sender, SizeChangedEventArgs e)
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
                Canvas.SetTop(element, (staticLevelData.mapPosition.Y * mapHeight) - (MissionControl.IMAGE_RADIUS));
            }
        }

    }
}
