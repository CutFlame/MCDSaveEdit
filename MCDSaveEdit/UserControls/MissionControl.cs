using MCDSaveEdit.Save.Models.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
#nullable enable

namespace MCDSaveEdit
{
    public class MissionControl: DockPanel
    {
        private readonly LevelImagePanel _levelImagePanel = new LevelImagePanel();
        private readonly Button _button = new Button();
        private readonly Label _label = new Label();

        private readonly LevelTypeEnum _levelType;

        public ICommand Command { get => _button.Command; set => _button.Command = value; }
        public object CommandParameter { get => _button.CommandParameter; set => _button.CommandParameter = value; }

        public MissionControl(LevelTypeEnum levelType)
        {
            _levelType = levelType;

            _levelImagePanel.levelType = _levelType;

            _button.Height = LevelImagePanel.IMAGE_RADIUS * 2;
            _button.Width = LevelImagePanel.IMAGE_RADIUS * 2;
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
                    _label.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _label.Visibility = Visibility.Visible;
                }
            }
            else
            {
                _label.Visibility = Visibility.Visible;
            }
            _levelImagePanel.updateLockedUI(_locked);
        }

        private uint _difficultyLevel;
        public uint difficultyLevel {
            get { return _difficultyLevel; }
            set { _difficultyLevel = value; updateDifficultyLevelUI(); }
        }

        private void updateDifficultyLevelUI()
        {
            _levelImagePanel.updateDifficultyLevelUI(_difficultyLevel);
        }

        private void updateUI()
        {
            switch (_levelType)
            {
                case LevelTypeEnum.mission:
                    updateDifficultyLevelUI();
                    break;
                case LevelTypeEnum.dungeon:
                    updateLockedUI();
                    break;
            }
        }

    }
}
