using MCDSaveEdit.Save.Models.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    public class MissionControl: DockPanel
    {
        private const double IMAGE_RADIUS = 18;

        private static readonly BitmapImage? _missionImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/mission_marker_front");
        private static readonly BitmapImage? _dungeonImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/shield_dungeon");

        private static readonly BitmapImage? _lockedDungeonImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/locked_dungeons");
        private static readonly BitmapImage? _incompleteMissionImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/mission_marker_iconSword_A");
        private static readonly BitmapImage? _unlockedDungeonImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/icon_dungeon");

        private static readonly BitmapImage? _difficulty1ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level1");
        private static readonly BitmapImage? _difficulty2ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level2");
        private static readonly BitmapImage? _difficulty3ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level3");
        private static readonly BitmapImage? _difficulty4ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level4");
        private static readonly BitmapImage? _difficulty5ImageSource = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level5");

        private readonly Image _levelTypeImage = new Image();
        private readonly Image _levelDifficultyImage = new Image();
        private readonly Grid _levelImagePanel = new Grid();
        private readonly Button _button = new Button();
        private readonly Label _label = new Label();

        private readonly LevelTypeEnum _levelType;

        public MissionControl(LevelTypeEnum levelType)
        {
            _levelType = levelType;
            _levelImagePanel.Children.Add(_levelTypeImage);
            _levelImagePanel.Children.Add(_levelDifficultyImage);

            _button.Height = IMAGE_RADIUS * 2;
            _button.Width = IMAGE_RADIUS * 2;
            _button.Background = null;
            _button.Content = _levelImagePanel;

            _label.Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            _label.Foreground = new SolidColorBrush(Colors.White);
            _label.FontWeight = FontWeights.ExtraBold;
            _label.HorizontalContentAlignment = HorizontalAlignment.Center;

            this.Children.Add(_button);
            this.Children.Add(_label);
            DockPanel.SetDock(_button, Dock.Top);
            DockPanel.SetDock(_label, Dock.Bottom);

            updateUI();
        }

        public string? text {
            get { return _label.Content as string; }
            set { _label.Content = value; }
        }

        private bool _locked;
        public bool locked {
            get { return _locked; }
            set { _locked = value; updateLockedUI(); }
        }
        private void updateLockedUI()
        {
            if (_locked)
            {
                if(_levelType == LevelTypeEnum.dungeon)
                {
                    _levelDifficultyImage.Source = _lockedDungeonImageSource;
                    _label.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _levelDifficultyImage.Source = _incompleteMissionImageSource;
                    _label.Visibility = Visibility.Visible;
                }
            }
            else
            {
                _levelDifficultyImage.Source = _unlockedDungeonImageSource;
                _label.Visibility = Visibility.Visible;
            }
            _levelDifficultyImage.Width = IMAGE_RADIUS;
            _levelDifficultyImage.Height = IMAGE_RADIUS;
        }

        private uint _difficultyLevel;
        public uint difficultyLevel {
            get { return _difficultyLevel; }
            set { _difficultyLevel = value; updateDifficultyLevelUI(); }
        }
        private void updateDifficultyLevelUI()
        {
            _levelDifficultyImage.Source = imageForDifficulty(_difficultyLevel);
            _levelDifficultyImage.Width = IMAGE_RADIUS * .95;
            _levelDifficultyImage.Height = IMAGE_RADIUS * .95;
        }

        private BitmapImage? imageForDifficulty(uint difficulty)
        {
            switch (difficulty)
            {
                case 1: return _difficulty1ImageSource;
                case 2: return _difficulty2ImageSource;
                case 3: return _difficulty3ImageSource;
                case 4: return _difficulty4ImageSource;
                default: return _difficulty5ImageSource;
            }
        }

        private void updateUI()
        {
            _levelDifficultyImage.HorizontalAlignment = HorizontalAlignment.Center;
            _levelDifficultyImage.VerticalAlignment = VerticalAlignment.Center;

            switch (_levelType)
            {
                case LevelTypeEnum.mission:
                    _levelTypeImage.Source = _missionImageSource;
                    updateDifficultyLevelUI();
                    break;
                case LevelTypeEnum.dungeon:
                    _levelTypeImage.Source = _dungeonImageSource;
                    updateLockedUI();
                    break;
            }
        }

    }
}
