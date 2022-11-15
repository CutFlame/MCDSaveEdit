using MCDSaveEdit.Data;
using MCDSaveEdit.Logic;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using MCDSaveEdit.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#nullable enable

namespace MCDSaveEdit.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Action? onRelaunch;
        public Action<string?, ProfileSaveFile?>? onReload;

        private readonly MainViewModel _model;

        private Window? _busyWindow = null;

        public MainWindow(MainViewModel model)
        {
            _model = model;
            InitializeComponent();
            translateStaticStrings();

            _model.showError = showError;
            gameFilesLocationMenuItem.Header = ImageResolver.instance.path ?? R.GAME_FILES_WINDOW_NO_CONTENT_BUTTON;
            var detectedGameVersion = _model.detectedGameVersion;
            if (detectedGameVersion == null)
            {
                gameFilesVersionMenuItem.Header = R.NO_GAME_VERSION_DETECTED;
            }
            else
            {
                gameFilesVersionMenuItem.Header = R.formatMCD_VERSION(detectedGameVersion);
            }

            refreshRecentFilesList();

            createLangMenuItems();


#if HIDE_CHEST_TAB
            chestTab.Visibility = Visibility.Collapsed;
#else
            chestTab.model = _model.profileModel;
#endif

            inventoryTab.model = _model.profileModel;
            statsTab.model = _model.profileModel;
            _model.profileModel.profile.subscribe(_ => this.updateUI());

            //Clear out design/testing values
            updateUI();

            checkForNewVersionAsync();
        }

        #region UI

        public void updateUI()
        {
            updateTitleUI();
            statsTab.updateUI();
            inventoryTab.updateUI();
            chestTab.updateUI();
            closeBusyIndicator();
        }

        private void updateTitleUI()
        {
            if (_model.profileModel.filePath != null)
            {
                Title = string.Format("{0} - {1}", R.APPLICATION_TITLE, Path.GetFileName(_model.profileModel.filePath));
                saveMenuItem.IsEnabled = saveAsMenuItem.IsEnabled = true;
            }
            else
            {
                Title = R.APPLICATION_TITLE;
                saveMenuItem.IsEnabled = saveAsMenuItem.IsEnabled = false;
            }
        }

        private void OnTabSelected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab != null)
            {
                // this tab is selected!
                this._model.profileModel.mainEquipmentModel.updateEnchantmentPoints();
                this._model.profileModel.storageChestEquipmentModel.updateEnchantmentPoints();
            }
        }

        #endregion

        #region Setup

        private void refreshRecentFilesList()
        {
            recentFilesMenuItem.Items.Clear();
            foreach(var menuItem in _model.recentFilesInfos.Select(createRecentFileMenuItem))
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

        private void translateStaticStrings()
        {
            inventoryTabItem.Header = R.getString("Quickaction_inventory") ?? R.INVENTORY;
            statsTabItem.Header = R.STATS_COUNTERS;
            chestTabItem.Header = R.getString("StorageChest") ?? R.CHEST;
        }

        private void createLangMenuItems()
        {
            langMenuItem.Items.Clear();
            var noneMenuItem = createLangMenuItem(R.getString("rebind_none") ?? R.NONE);
            langMenuItem.Items.Add(noneMenuItem);
            langMenuItem.Items.Add(new Separator());
            foreach(var menuItem in LanguageResolver.instance.localizationOptions.Select(createLangMenuItem))
            {
                langMenuItem.Items.Add(menuItem);
            }
        }

        private MenuItem createLangMenuItem(string lang)
        {
            var specificLangMenuItem = new MenuItem();
            string header;
            try
            {
                header = CultureInfo.GetCultureInfo(lang).NativeName;
            }
            catch
            {
                header = lang;
            }
            specificLangMenuItem.Header = header;
            specificLangMenuItem.IsChecked = AppModel.currentLangSpecifier == lang;
            specificLangMenuItem.CommandParameter = lang;
            specificLangMenuItem.Command = new RelayCommand<string>(languageSelectedMenuItem_Click);
            return specificLangMenuItem;
        }

#endregion

#region Version Check

        private async void checkForNewVersionAsync()
        {
            await Config.instance.downloadAsync();
            if (Config.instance.isNewBetaVersionAvailable())
            {
                updateMenuItem.Header = R.BETA_UPDATE_MENU_ITEM_HEADER;
                updateMenuItem.Visibility = Visibility.Visible;
            }
            else if (Config.instance.isNewStableVersionAvailable())
            {
                updateMenuItem.Header = R.STABLE_UPDATE_MENU_ITEM_HEADER;
                updateMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                updateMenuItem.Visibility = Visibility.Collapsed;
            }
        }

#endregion

#region User Input Methods

#region Keyboard Captures

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            //Capture the delete key
            if (e.Key == Key.Delete)
            {
                if (inventoryTab.IsVisible && !(Keyboard.FocusedElement is TextBox))
                {
                    inventoryTab.deleteCurrentSelectedItem();
                }
                else if (chestTab.IsVisible && !(Keyboard.FocusedElement is TextBox))
                {
                    chestTab.deleteCurrentSelectedItem();
                }
            }
        }

#endregion

#region Menu Items

        private void exitCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EventLogger.logEvent("exitCommandBinding_Executed");
            Application.Current?.Shutdown();
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
            if(!string.IsNullOrWhiteSpace(_model.profileModel.filePath))
            {
                var directory = Path.GetDirectoryName(_model.profileModel.filePath!);
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
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(_model.profileModel.filePath!); //Constants.FILE_DIALOG_INITIAL_DIRECTORY;
            if (saveFileDialog.ShowDialog() == true)
            {
                handleFileSaveAsync(saveFileDialog.FileName);
            }
        }

        private void saveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EventLogger.logEvent("saveCommandBinding_Executed");
            handleFileSaveAsync(_model.profileModel.filePath);
        }

        private void openRecentFileCommandBinding_Executed(FileInfo fileInfo)
        {
            handleFileOpenAsync(fileInfo.FullName);
        }

        private void languageSelectedMenuItem_Click(string langSpecifier)
        {
            EventLogger.logEvent("languageSelectedMenuItem_Click", new Dictionary<string, object> { { "langSpecifier", langSpecifier } });
            AppModel.loadLanguageStrings(langSpecifier);
            onReload?.Invoke(_model.profileModel.filePath, _model.profileModel.profile.value);
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("aboutMenuItem_Click");
            var aboutWindow = WindowFactory.createAboutWindow();
            aboutWindow.ShowDialog();
        }

        private void updateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("updateMenuItem_Click");
            Process.Start(Config.instance.newVersionDownloadURL());
        }

#endregion

#region File Drop Capture

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

#endregion

#endregion

#region Helper Functions

        private string constructOpenFileDialogFilterString(Dictionary<string, string> dict)
        {
            return string.Join("|", dict.Select(x => string.Join("|", string.Format("{0} ({1})", x.Value, x.Key), x.Key)));
        }

        public async void handleFileOpenAsync(string? fileName)
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
            await _model.handleFileOpenAsync(fileName!);
            updateTitleUI();
            refreshRecentFilesList();
            closeBusyIndicator();
        }

        private async void handleFileSaveAsync(string? fileName)
        {
            if (_model.profileModel.profile.value == null || string.IsNullOrWhiteSpace(fileName)) { return; }
            showBusyIndicator();
            string extension = Path.GetExtension(fileName!);
            EventLogger.logEvent("handleFileSaveAsync", new Dictionary<string, object>() { { "extension", extension } });
            await _model.handleFileSaveAsync(fileName!, _model.profileModel.profile.value!);
            updateTitleUI();
            refreshRecentFilesList();
            closeBusyIndicator();
        }
        
        private void showBusyIndicator()
        {
            closeBusyIndicator();

            _busyWindow = WindowFactory.createBusyWindow();
            _busyWindow.Owner = this;
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
    }
}
