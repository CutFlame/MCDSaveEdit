using MCDSaveEdit.Save.Models.Profiles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProfileViewModel? _model;
        public ProfileViewModel? model
        {
            get { return _model; }
            set
            {
                _model = value;
                inventoryScreen.model = _model;
                //mainlandMapScreen.model = _model;
                //jungleAwakensMapScreen.model = _model;
                //creepingWinterMapScreen.model = _model;
                //howlingPeaksMapScreen.model = _model;
                setupCommands();
                updateUI();
            }
        }

        private Window? _busyWindow = null;

        public MainWindow()
        {
            InitializeComponent();
            translateStaticStrings();
            if (ImageUriHelper.gameContentLoaded)
            {
                useGameContentImages();
            }

            //Clear out design/testing values
            updateUI();

            checkForNewVersionAsync();
        }

        private void useGameContentImages()
        {
            emeraldsLabelImage.Source = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Character/STATS_emerald");
            remainingEnchantmentPointsLabelImage.Source = ImageUriHelper.instance.imageSource("/Dungeons/Content/UI/Materials/Inventory2/Enchantment/enchantscore_background");
        }

        private void translateStaticStrings()
        {
            inventoryTabItem.Header = R.getString("Quickaction_inventory") ?? R.INVENTORY;
            //mainlandTabItem.Header = R.getString("ArchIllagerRealm_name") ?? R.MAINLAND;
            //jungleAwakensTabItem.Header = R.getString("TheJungleAwakens_name") ?? R.JUNGLE_AWAKENS;
            //creepingWinterTabItem.Header = R.getString("TheCreepingWinter_name") ?? R.CREEPING_WINTER;
            //howlingPeaksTabItem.Header = R.getString("TheHowlingPeaks_name") ?? R.HOWLING_PEAKS;
        }

        private void setupCommands()
        {
            if (_model == null) { return; }
            var model = _model!;

            selectedItemScreen.selectEnchantment = new RelayCommand<Enchantment>(model.selectEnchantment);
            selectedItemScreen.saveChanges = new RelayCommand<Item>(model.saveItem);
            selectedEnchantmentScreen.close = new RelayCommand<Enchantment>(model.selectEnchantment);
            selectedEnchantmentScreen.saveChanges = new RelayCommand<Enchantment>(model.saveEnchantment);

            model.showError = showError;
            model.level.subscribe(_ => this.updateEnchantmentPointsUI());
            model.emeralds.subscribe(updateEmeraldsUI);
            model.selectedItem.subscribe(item => this.selectedItemScreen.item = item);
            model.selectedEnchantment.subscribe(updateEnchantmentScreenUI);
            model.profile.subscribe(_ => this.updateUI());
            model.equippedItemList.subscribe(_ => this.updateEnchantmentPointsUI());
            model.filteredItemList.subscribe(_ => this.updateEnchantmentPointsUI());
        }

        #region Version Check

        private async void checkForNewVersionAsync()
        {
            string latest = await latestReleaseVersionString(Constants.LATEST_RELEASE_URL);
            if (string.IsNullOrWhiteSpace(latest) || Constants.CURRENT_RELEASE_TAG_NAME.Equals(latest))
            {
                updateMenuItem.Visibility = Visibility.Collapsed;
            }
            else
            {
                updateMenuItem.Visibility = Visibility.Visible;
            }
        }

        private async Task<string> latestReleaseVersionString(string webAddress)
        {
            try
            {
                var request = WebRequest.Create(webAddress);
                using var response = await request.GetResponseAsync();
                return response.ResponseUri.Segments.Last();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return string.Empty;
        }
        #endregion

        #region User Input Methods

        private void exitCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EventLogger.logEvent("exitCommandBinding_Executed");
            Application.Current.Shutdown();
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
            Process.Start(Constants.LATEST_RELEASE_URL);
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
            EventLogger.logEvent("emeraldsTextBox_TextChanged");
            if (uint.TryParse(emeraldsTextBox.Text, out uint emeralds))
            {
                _model!.emeralds.setValue = emeralds;
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
            if(_model == null || fileName == null) { return; }
            showBusyIndicator();
            string extension = Path.GetExtension(fileName!);
            EventLogger.logEvent("handleFileOpenAsync", new Dictionary<string, object>() { { "extension", extension } });
            await _model!.handleFileOpenAsync(fileName);
            closeBusyIndicator();
        }

        private async void handleFileSaveAsync(string? fileName)
        {
            if (_model == null || fileName == null) { return; }
            showBusyIndicator();
            string extension = Path.GetExtension(fileName!);
            EventLogger.logEvent("handleFileSaveAsync", new Dictionary<string, object>() { { "extension", extension } });
            await _model!.handleFileSaveAsync(fileName);
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

        private void updateUI() {
            updateTitleUI();
            updateEmeraldsUI(_model?.emeralds.value);
            inventoryScreen.updateUI();
            //mainlandMapScreen.updateUI();
            //jungleAwakensMapScreen.updateUI();
            //creepingWinterMapScreen.updateUI();
            //howlingPeaksMapScreen.updateUI();
            updateEnchantmentPointsUI();
            selectedItemScreen.item = _model?.selectedItem.value;
            closeBusyIndicator();
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
            }
            else
            {
                emeraldsTextBox.IsEnabled = false;
                emeraldsTextBox.Text = string.Empty;
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
                selectedItemScreen.updateEnchantmentsUI();
            }
        }

        #endregion
        
    }
}
