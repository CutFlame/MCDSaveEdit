using MCDSaveEdit.Data;
using MCDSaveEdit.Logic;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Services;
using MCDSaveEdit.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
#nullable enable

namespace MCDSaveEdit.UI
{
    /// <summary>
    /// Interaction logic for MissionScreen.xaml
    /// </summary>
    public partial class MissionScreen : UserControl
    {
        private ProfileViewModel? _model;

        public ProfileViewModel? model { get => _model; set { _model = value; setMissionInfo(null); } }

        public MissionScreen()
        {
            InitializeComponent();

            difficultyLabel.Content = R.COMPLETED_DIFFICULTY;
            threatLevelLabel.Content = R.COMPLETED_THREAT_LEVEL;
            endlessStruggleLabel.Content = R.COMPLETED_APOCALYPSE_PLUS;
            threatLevelTextBox.Text = string.Empty;
            endlessStruggleTextBox.Text = string.Empty;
            difficultyComboBox.IsEnabled = false;
            threatLevelSlider.IsEnabled = false;
            endlessStruggleSlider.IsEnabled = false;

            difficultyComboBox.Items.Clear();
            difficultyComboBox.Items.Add(R.getString("mission_locked"));
            difficultyComboBox.Items.Add(R.NOT_COMPLETED);
            difficultyComboBox.Items.Add(R.getString("Difficulty_1"));
            difficultyComboBox.Items.Add(R.getString("Difficulty_2"));
            difficultyComboBox.Items.Add(R.getString("Difficulty_3"));
        }

        public void setMissionInfo(string? key)
        {
            var profile = model?.profile.value;
            if (profile == null || key == null)
            {
                this.Visibility = Visibility.Hidden;
                return;
            }
            this.Visibility = Visibility.Visible;

            var levelData = Constants.LEVEL_DATA_LOOKUP[key];
            missionStatusImagePanel.levelType = levelData.levelType;

            var prerequisites = profile!.BonusPrerequisites;
            var progress = profile!.Progress;
            var locked = levelData.levelType == LevelTypeEnum.dungeon && !prerequisites.Contains(key);
            missionStatusImagePanel.updateDifficultyLevelUI(0);
            missionStatusImagePanel.updateLockedUI(locked);

            if (levelData.levelType == LevelTypeEnum.dungeon)
            {
                secretMissionLabel.Content = R.getString("missioninspector_secretmission");
            }
            else
            {
                secretMissionLabel.Content = string.Empty;
            }

            threatLevelSlider.Value = 0;
            endlessStruggleSlider.Value = 0;

            if (locked)
            {
                missionNameLabel.Content = R.getString("missioninspector_lockedlevel");
                difficultyComboBox.SelectedIndex = 0;
            }
            else if (progress != null && progress.ContainsKey(key))
            {
                var levelProgress = progress[key];
                var difficultyLevel = levelProgress.getDifficultyImageLevel();

                missionStatusImagePanel.updateDifficultyLevelUI(difficultyLevel);
                missionNameLabel.Content = R.getMissionName(key);
                difficultyComboBox.SelectedIndex = getDifficultyCompletedFromCompletedDifficulty(levelProgress.CompletedDifficulty);
                threatLevelSlider.Value = getThreatLevelCompletedFromCompletedThreatLevel(levelProgress.CompletedThreatLevel);
                endlessStruggleSlider.Value = levelProgress.CompletedEndlessStruggle;
            }
            else
            {
                missionStatusImagePanel.updateDifficultyLevelUI(0);
                missionNameLabel.Content = R.getMissionName(key);
                difficultyComboBox.SelectedIndex = 1;
            }
        }

        private int getThreatLevelCompletedFromCompletedThreatLevel(string completedThreatLevel)
        {
            //https://stackoverflow.com/questions/262448/replace-non-numeric-with-empty-string
            string numbersOnly = Regex.Replace(completedThreatLevel, "[^0-9]", "");
            if (int.TryParse(numbersOnly, out int result))
            {
                return result;
            }
            throw new NotImplementedException();
        }

        private int getThreatLevelCompletedFromCompletedThreatLevel(ThreatLevelEnum completedThreatLevel)
        {
            switch (completedThreatLevel)
            {
                case ThreatLevelEnum.Threat_1: return 1;
                case ThreatLevelEnum.Threat_2: return 2;
                case ThreatLevelEnum.Threat_3: return 3;
                case ThreatLevelEnum.Threat_4: return 4;
                case ThreatLevelEnum.Threat_5: return 5;
                case ThreatLevelEnum.Threat_6: return 6;
                case ThreatLevelEnum.Threat_7: return 7;
            }
            throw new NotImplementedException();
        }

        private int getDifficultyCompletedFromCompletedDifficulty(string difficultyEnum)
        {
            //https://stackoverflow.com/questions/262448/replace-non-numeric-with-empty-string
            string numbersOnly = Regex.Replace(difficultyEnum, "[^0-9]", "");
            if (int.TryParse(numbersOnly, out int result))
            {
                return result + 1;
            }
            throw new NotImplementedException();
        }

        private void endlessStruggleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            uint newValue = (uint)Math.Max(0, e.NewValue);
            if (endlessStruggleTextBox != null)
                endlessStruggleTextBox.Text = $"+{newValue}";
        }

        private void threatLevelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            uint newValue = (uint)Math.Max(0, e.NewValue);
            threatLevelTextBox.Text = romanNumerals(newValue);
        }

        //https://stackoverflow.com/questions/7040289/converting-integers-to-roman-numerals
        private string romanNumerals(uint number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + romanNumerals(number - 1000);
            if (number >= 900) return "CM" + romanNumerals(number - 900);
            if (number >= 500) return "D" + romanNumerals(number - 500);
            if (number >= 400) return "CD" + romanNumerals(number - 400);
            if (number >= 100) return "C" + romanNumerals(number - 100);
            if (number >= 90) return "XC" + romanNumerals(number - 90);
            if (number >= 50) return "L" + romanNumerals(number - 50);
            if (number >= 40) return "XL" + romanNumerals(number - 40);
            if (number >= 10) return "X" + romanNumerals(number - 10);
            if (number >= 9) return "IX" + romanNumerals(number - 9);
            if (number >= 5) return "V" + romanNumerals(number - 5);
            if (number >= 4) return "IV" + romanNumerals(number - 4);
            if (number >= 1) return "I" + romanNumerals(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }
    }
}
