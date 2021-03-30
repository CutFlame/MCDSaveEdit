using MCDSaveEdit.Save.Models.Profiles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly BitmapImage? _emeraldImage = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Character/STATS_emerald");
        private static readonly BitmapImage? _goldImage = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Currency/GoldIndicator");
        private static readonly BitmapImage? _enchantmentImage = AppModel.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/enchantscore_background");

        public static void init() { }

        public Action? onRelaunch;

        private readonly MainViewModel _mainModel = new MainViewModel();

        private ProfileViewModel? _model;
        public ProfileViewModel? model
        {
            get { return _model; }
            set
            {
                _model = value;
                inventoryScreen.model = _model;
                _mapScreens.ForEach(mapScreen => mapScreen.model = _model);
                setupCommands();
                updateUI();
            }
        }

        private readonly List<MapScreen> _mapScreens = new List<MapScreen>();
        private Window? _busyWindow = null;

        public MainWindow()
        {
            InitializeComponent();
            translateStaticStrings();

            _mainModel.showError = showError;
            gameFilesLocationMenuItem.Header = AppModel.instance.path;

            refreshRecentFilesList();
            if (AppModel.gameContentLoaded)
            {
                useGameContentImages();
            }

#if !HIDE_MAP_SCREENS
            createMapScreenTabItems();
#endif

            //Clear out design/testing values
            updateUI();

            checkForNewVersionAsync();
        }

        private void refreshRecentFilesList()
        {
            recentFilesMenuItem.Items.Clear();
            foreach(var menuItem in _mainModel.recentFilesInfos.Select(createRecentFileMenuItem))
            {
                recentFilesMenuItem.Items.Add(menuItem);
            }
            recentFilesMenuItem.IsEnabled = recentFilesMenuItem.Items.Count > 0;
        }

        private MenuItem createRecentFileMenuItem(FileInfo fileInfo)
        {
            var menuItem = new MenuItem();
            menuItem.Header = fileInfo.Name;
            menuItem.CommandParameter = fileInfo;
            menuItem.Command = new RelayCommand<FileInfo>(openRecentFileCommandBinding_Executed);
            return menuItem;
        }

        private void useGameContentImages()
        {
            emeraldsLabelImage.Source = _emeraldImage;
            goldLabelImage.Source = _goldImage;
            remainingEnchantmentPointsLabelImage.Source = _enchantmentImage;
        }

        private void translateStaticStrings()
        {
            inventoryTabItem.Header = R.getString("Quickaction_inventory") ?? R.INVENTORY;
            statsTabItem.Header = R.STATS_COUNTERS;
            statsLabel.Content = R.PROGRESS_STAT_COUNTERS;
            mobKillsLabel.Content = R.MOB_KILLS;
        }

        private void createMapScreenTabItems()
        {
            foreach(var mapImageData in Constants.ALL_MAP_IMAGE_DATA)
            {
                var mapScreen = new MapScreen(mapImageData);
                var mapScreenTabItem = new TabItem() {
                    Content = mapScreen,
                    Header = mapImageData.title(),
                };
                _mapScreens.Add(mapScreen);
                mainTabControl.Items.Add(mapScreenTabItem);
            }
        }

        private void fillStatsStack()
        {
            statsStack.Children.Clear();
            if (_model == null || _model?.profile.value == null) { return; }

            foreach (var pair in _model!.profile.value!.ProgressStatCounters)
            {
                var field = createStatField(pair.Key, pair.Value);
                statsStack.Children.Add(field);
            }
        }

        private void fillMobKillsStack()
        {
            mobKillsStack.Children.Clear();
            if (_model?.profile.value == null) { return; }

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

        private void statStepper_UpButtonClick(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            var stepper = sender as Stepper;
            if (stepper == null) { return; }
            var textBox = stepper.Tag as TextBox;
            if (textBox == null) { return; }
            if(long.TryParse(textBox.Text, out long currentValue))
            {
                textBox.Text = Math.Min(currentValue + 1, long.MaxValue).ToString();
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

        private void setupCommands()
        {
            if (_model == null) { return; }
            var model = _model!;

            selectedItemScreen.selectEnchantment = new RelayCommand<Enchantment>(model.selectEnchantment);
            selectedItemScreen.saveChanges = new RelayCommand<Item>(model.saveItem);
            selectedItemScreen.addEnchantmentSlot = new RelayCommand<object>(model.addEnchantmentSlot);
            selectedEnchantmentScreen.close = new RelayCommand<Enchantment>(model.selectEnchantment);
            selectedEnchantmentScreen.saveChanges = new RelayCommand<Enchantment>(model.saveEnchantment);

            model.level.subscribe(_ => this.updateEnchantmentPointsUI());
            model.emeralds.subscribe(updateEmeraldsUI);
            model.gold.subscribe(updateGoldUI);
            model.selectedItem.subscribe(item => this.selectedItemScreen.item = item);
            model.selectedEnchantment.subscribe(updateEnchantmentScreenUI);
            model.profile.subscribe(_ => this.updateUI());
            model.equippedItemList.subscribe(_ => this.updateEnchantmentPointsUI());
            model.filteredItemList.subscribe(_ => this.updateEnchantmentPointsUI());
        }

        #region Version Check

        private async void checkForNewVersionAsync()
        {
            bool isNewVersionAvailable = await Config.instance.isNewVersionAvailable();
            if (isNewVersionAvailable)
            {
                updateMenuItem.Visibility = Visibility.Collapsed;
            }
            else
            {
                updateMenuItem.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region User Input Methods

        private void exitCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EventLogger.logEvent("exitCommandBinding_Executed");
            Application.Current.Shutdown();
        }

        private void relaunchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("relaunchMenuItem_Click");
            onRelaunch?.Invoke();
        }

        private void openCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EventLogger.logEvent("openCommandBinding_Executed");
            var openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = constructOpenFileDialogFilterString(ProfileViewModel.supportedFileTypesDict);
            openFileDialog.FilterIndex = 0;
            if(!string.IsNullOrWhiteSpace(_model?.filePath))
            {
                var directory = Path.GetDirectoryName(_model!.filePath);
                openFileDialog.InitialDirectory = directory;
            }
            else
            {
                openFileDialog.InitialDirectory = Constants.FILE_DIALOG_INITIAL_DIRECTORY;
            }
            if (openFileDialog.ShowDialog() == true)
            {
                handleFileOpenAsync(openFileDialog.FileName);
            }
        }

        private void saveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EventLogger.logEvent("saveAsCommandBinding_Executed");
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = constructOpenFileDialogFilterString(ProfileViewModel.supportedFileTypesDict);
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(_model!.filePath); //Constants.FILE_DIALOG_INITIAL_DIRECTORY;
            if (saveFileDialog.ShowDialog() == true)
            {
                handleFileSaveAsync(saveFileDialog.FileName);
            }
        }

        private void saveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EventLogger.logEvent("saveCommandBinding_Executed");
            handleFileSaveAsync(_model?.filePath);
        }

        private void openRecentFileCommandBinding_Executed(FileInfo fileInfo)
        {
            handleFileOpenAsync(fileInfo.FullName);
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("aboutMenuItem_Click");
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void updateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("updateMenuItem_Click");
            Process.Start(Config.instance.newVersionDownloadURL());
        }
        
        private void window_File_Drop(object sender, DragEventArgs e)
        {
            EventLogger.logEvent("window_File_Drop");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                handleFileOpenAsync(files[0]);
            }
            else
            {
                showError(R.FILE_DROP_ERROR_MESSAGE);
            }
        }

        private void emeraldsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_model?.profile.value == null || !emeraldsTextBox.IsEnabled) { return; }
            if (uint.TryParse(emeraldsTextBox.Text, out uint emeralds))
            {
                EventLogger.logEvent("emeraldsTextBox_TextChanged");
                emeraldsTextBox.BorderBrush = Brushes.Gray;
                _model!.emeralds.setValue = emeralds;
            }
            else
            {
                emeraldsTextBox.BorderBrush = Brushes.Red;
            }
        }

        private void goldTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_model?.profile.value == null || !goldTextBox.IsEnabled) { return; }
            if (uint.TryParse(goldTextBox.Text, out uint gold))
            {
                EventLogger.logEvent("goldTextBox_TextChanged");
                goldTextBox.BorderBrush = Brushes.Gray;
                _model!.gold.setValue = gold;
            }
            else
            {
                goldTextBox.BorderBrush = Brushes.Red;
            }
        }

        #endregion

        #region Helper Functions

        private string constructOpenFileDialogFilterString(Dictionary<string, string> dict)
        {
            return string.Join("|", dict.Select(x => string.Join("|", string.Format("{0} ({1})", x.Value, x.Key), x.Key)));
        }

        private async void handleFileOpenAsync(string? fileName)
        {
            if(string.IsNullOrWhiteSpace(fileName)) { return; }
            if (!File.Exists(fileName))
            {
                showError(R.FILE_DOESNT_EXIST_ERROR_MESSAGE);
                return;
            }
            showBusyIndicator();
            string extension = Path.GetExtension(fileName!);
            EventLogger.logEvent("handleFileOpenAsync", new Dictionary<string, object>() { { "extension", extension } });
            var profile = await _mainModel.handleFileOpenAsync(fileName!);
            _mainModel.addRecentFile(fileName!);
            refreshRecentFilesList();
            if (this.model == null) { this.model = new ProfileViewModel(); }
            this.model!.filePath = fileName;
            this.model!.profile.setValue = profile;
            closeBusyIndicator();
        }

        private async void handleFileSaveAsync(string? fileName)
        {
            if (_model == null || _model!.profile.value == null || string.IsNullOrWhiteSpace(fileName)) { return; }
            showBusyIndicator();
            string extension = Path.GetExtension(fileName!);
            EventLogger.logEvent("handleFileSaveAsync", new Dictionary<string, object>() { { "extension", extension } });
            await _mainModel.handleFileSaveAsync(fileName!, _model!.profile.value);
            _mainModel.addRecentFile(fileName!);
            this.model!.filePath = fileName;
            updateTitleUI();
            refreshRecentFilesList();
            closeBusyIndicator();
        }
        
        private void showBusyIndicator()
        {
            closeBusyIndicator();

            _busyWindow = new Window();
            _busyWindow.Owner = this;
            _busyWindow.Height = 200;
            _busyWindow.Width = 200;
            _busyWindow.ResizeMode = ResizeMode.NoResize;
            _busyWindow.WindowStyle = WindowStyle.None;
            _busyWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _busyWindow.Content = new BusyIndicator();
            _busyWindow.Show();
        }

        private void closeBusyIndicator()
        {
            if (_busyWindow != null)
            {
                _busyWindow!.Close();
                _busyWindow = null;
            }
        }

        private void showError(string message)
        {
            EventLogger.logEvent("showError", new Dictionary<string, object>() { { "message", message } });
            MessageBox.Show(message, R.ERROR);
            closeBusyIndicator();
        }

        #endregion

        #region UI

        private void updateUI()
        {
            updateTitleUI();
            updateEmeraldsUI(_model?.emeralds.value);
            updateGoldUI(_model?.gold.value);
            fillStatsStack();
            fillMobKillsStack();
            updateMapScreensUI();
            updateEnchantmentPointsUI();
            selectedItemScreen.item = _model?.selectedItem.value;
            closeBusyIndicator();
        }

        private void updateMapScreensUI()
        {
            inventoryScreen.updateUI();
            _mapScreens.ForEach(mapScreen => mapScreen.updateUI());
        }

        private void updateTitleUI()
        {
            if (_model?.profile.value != null)
            {
                Title = string.Format("{0} - {1}", R.APPLICATION_TITLE, Path.GetFileName(_model!.filePath));
                saveMenuItem.IsEnabled = saveAsMenuItem.IsEnabled = true;
            }
            else
            {
                Title = R.APPLICATION_TITLE;
                saveMenuItem.IsEnabled = saveAsMenuItem.IsEnabled = false;
            }
        }

        private void updateEmeraldsUI(ulong? emeralds)
        {
            if (emeralds != null)
            {
                emeraldsTextBox.IsEnabled = false;
                emeraldsTextBox.Text = emeralds!.ToString();
                emeraldsTextBox.IsEnabled = true;
                emeraldsTextBox.Visibility = Visibility.Visible;
                emeraldsAddButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                emeraldsTextBox.IsEnabled = false;
                emeraldsTextBox.Text = string.Empty;
                emeraldsTextBox.Visibility = Visibility.Collapsed;
                emeraldsAddButton.Visibility = Visibility.Visible;
            }
        }

        private void updateGoldUI(ulong? gold)
        {
            if (gold != null)
            {
                goldTextBox.IsEnabled = false;
                goldTextBox.Text = gold!.ToString();
                goldTextBox.IsEnabled = true;
                goldTextBox.Visibility = Visibility.Visible;
                goldAddButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                goldTextBox.IsEnabled = false;
                goldTextBox.Text = string.Empty;
                goldTextBox.Visibility = Visibility.Collapsed;
                goldAddButton.Visibility = Visibility.Visible;
            }
        }

        private void updateEnchantmentPointsUI()
        {
            if (_model?.profile.value != null)
            {
                remainingEnchantmentPointsLabel.Content = _model!.profile.value!.remainingEnchantmentPoints().ToString();
            }
            else
            {
                remainingEnchantmentPointsLabel.Content = string.Empty;
            }
        }

        private void updateEnchantmentScreenUI(Enchantment? enchantment)
        {
            if(enchantment == null)
            {
                selectedEnchantmentScreen.Visibility = Visibility.Collapsed;
                selectedEnchantmentScreenBackShadowRectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                selectedEnchantmentScreen.Visibility = Visibility.Visible;
                selectedEnchantmentScreenBackShadowRectangle.Visibility = Visibility.Visible;
                selectedEnchantmentScreen.enchantment = enchantment;
                selectedEnchantmentScreen.isGilded = this.selectedItemScreen.item?.NetheriteEnchant != null;
                selectedItemScreen.updateEnchantmentsUI();
            }
        }

        #endregion

        private void emeraldsAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            _model.emeralds.setValue = 0;
        }

        private void goldAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model?.profile.value == null) { return; }
            _model.gold.setValue = 0;
        }
    }
}
