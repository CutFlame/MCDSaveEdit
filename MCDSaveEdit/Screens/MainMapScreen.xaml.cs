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
    /// Interaction logic for MainMapScreen.xaml
    /// </summary>
    public partial class MainMapScreen : UserControl
    {
        private ProfileViewModel? _model;
        public ProfileViewModel? model {
            get { return _model; }
            set {
                _model = value;
                //setupCommands();
                //updateUI();
            }
        }


        private Dictionary<string, MissionControl> _missionElements = new Dictionary<string, MissionControl>();

        public MainMapScreen()
        {
            InitializeComponent();
            mapLabel.Content = R.getString("ArchIllagerRealm_name") ?? R.MAINLAND;

            var mapImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/background/missionselect_map_center_xbox");
            if(mapImageSource!= null)
            {
                var croppedImageSource = new CroppedBitmap(mapImageSource, new Int32Rect(10, 83, 6136, 2975));
                var background = new ImageBrush(croppedImageSource);
                this.Background = background;
            }

            foreach (var staticLevelData in Constants._staticLevelData)
            {
                var panel = new MissionControl(staticLevelData.levelType);
                panel.text = R.getMissionName(staticLevelData.key);

                canvas.Children.Add(panel);
                _missionElements.Add(staticLevelData.key, panel);
            }

            positionLevels();
            updateUI();
        }

        public void updateUI()
        {
            if(_model?.profile.value == null)
            {
                foreach(var panel in _missionElements.Values)
                {
                    panel.Visibility = Visibility.Collapsed;
                }
                return;
            }
            var prerequisites = _model!.profile.value!.BonusPrerequisites;
            var progress = _model!.profile.value!.Progress;
            foreach (var level in Constants._staticLevelData.Select(level=>level.key))
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
                Canvas.SetTop(element, staticLevelData.mapPosition.Y * mapHeight - (element.ActualHeight / 2));
            }

        }

    }
}
