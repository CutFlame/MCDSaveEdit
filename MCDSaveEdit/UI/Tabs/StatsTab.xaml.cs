using MCDSaveEdit.Data;
using MCDSaveEdit.Logic;
using MCDSaveEdit.Services;
using MCDSaveEdit.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#nullable enable

namespace MCDSaveEdit.UI
{
    /// <summary>
    /// Interaction logic for StatsTab.xaml
    /// </summary>
    public partial class StatsTab : UserControl
    {
        private ProfileViewModel? _model;
        public ProfileViewModel? model {
            get { return _model; }
            set { _model = value; }
        }

        public StatsTab()
        {
            InitializeComponent();
            translateStaticStrings();

            //Clear out design/testing values
            updateUI();
        }

        public void updateUI()
        {
            fillStatsStack();
            fillMobKillsStack();
        }

        private void translateStaticStrings()
        {
            statsLabel.Content = R.PROGRESS_STAT_COUNTERS;
            mobKillsLabel.Content = R.MOB_KILLS;
        }

        private void fillStatsStack()
        {
            statsStack.Children.Clear();
            if (_model?.profile.value?.ProgressStatCounters == null) { return; }

            foreach (var pair in _model!.profile.value!.ProgressStatCounters)
            {
                var field = createStatField(pair.Key, pair.Value);
                statsStack.Children.Add(field);
            }
        }

        private void fillMobKillsStack()
        {
            mobKillsStack.Children.Clear();
            if (_model?.profile.value?.MobKills == null) { return; }

            foreach (var pair in _model!.profile.value!.MobKills)
            {
                var field = createStatField(pair.Key, pair.Value);
                mobKillsStack.Children.Add(field);
            }
        }

        private Panel createStatField(string fieldName, long fieldValue)
        {
            var label = new TextBlock() {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0),
                Margin = new Thickness(5),
                FontSize = 14,
                Text = fieldName,
            };

            var textbox = new TextBox() {
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 5, 0, 5),
                Background = null,
                Width = 70,
                FontSize = 16,
                Text = fieldValue.ToString(),
                Tag = fieldName,
            };
            textbox.TextChanged += statTextbox_TextChanged;

            var stepper = new Stepper() {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 5, 0, 5),
                Width = 12,
                Tag = textbox,
            };
            stepper.UpButtonClick += statStepper_UpButtonClick;
            stepper.DownButtonClick += statStepper_DownButtonClick;

            var dockPanel = new DockPanel() { Height = 40, Margin = new Thickness(5, 0, 5, 0) };

            dockPanel.Children.Add(label);
            DockPanel.SetDock(label, Dock.Left);
            dockPanel.Children.Add(stepper);
            DockPanel.SetDock(stepper, Dock.Right);
            dockPanel.Children.Add(textbox);
            DockPanel.SetDock(textbox, Dock.Right);

            return dockPanel;
        }

        #region User Input Methods

        private void statStepper_UpButtonClick(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            var stepper = sender as Stepper;
            if (stepper == null) { return; }
            var textBox = stepper.Tag as TextBox;
            if (textBox == null) { return; }
            if (long.TryParse(textBox.Text, out long currentValue))
            {
                textBox.Text = Math.Min(currentValue + 1, long.MaxValue).ToString();
            }
        }

        private void statStepper_DownButtonClick(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            var stepper = sender as Stepper;
            if (stepper == null) { return; }
            var textBox = stepper.Tag as TextBox;
            if (textBox == null) { return; }
            if (long.TryParse(textBox.Text, out long currentValue))
            {
                textBox.Text = Math.Max(currentValue - 1, 0).ToString();
            }
        }

        private void statTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            var statTextBox = sender as TextBox;
            if (statTextBox == null) { return; }
            var fieldName = statTextBox.Tag as string;
            if (fieldName == null) { return; }

            if (long.TryParse(statTextBox.Text, out long newValue))
            {
                EventLogger.logEvent("statTextbox_TextChanged");
                statTextBox.BorderBrush = Brushes.Gray;
                if (_model!.profile.value.ProgressStatCounters.ContainsKey(fieldName))
                {
                    _model!.profile.value.ProgressStatCounters[fieldName] = newValue;
                }
                else
                {
                    _model!.profile.value.MobKills[fieldName] = newValue;
                }
            }
            else
            {
                statTextBox.BorderBrush = Brushes.Red;
            }
        }

        #endregion
    }
}
