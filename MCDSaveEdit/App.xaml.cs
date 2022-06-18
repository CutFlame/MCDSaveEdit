using MCDSaveEdit.Data;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using MCDSaveEdit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private readonly AppModel _model = new AppModel();
        private readonly MultiTextWriter _outputWriter = new MultiTextWriter();

        private ControlWriter? _controlWriter = null;
        private SplashWindow? _splashWindow = null;
        private Window? _busyWindow = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _outputWriter.addWriter(Console.Out);
            Console.SetOut(_outputWriter);

            EventLogger.init();

            showSplashWindowReplacingOldWindow();

            startAsync(e.Args);
        }

        private async void startAsync(string[] args)
        {
            string? fileName = args.LastOrDefault();
            if(!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
            {
                string extension = Path.GetExtension(fileName!);
                EventLogger.logEvent("handleFileOpenAsync", new Dictionary<string, object>() { { "extension", extension } });
                await _model.mainModel.handleFileOpenAsync(fileName!);
            }

            bool skipGameContent = args.Contains("SKIP_GAME_CONTENT");
            if (skipGameContent)
            {
                showMainWindow();
            }
            else
            {
                bool askForGameContentLocation = args.Contains("ASK_FOR_GAME_CONTENT_LOCATION");
                await loadAsync(askForGameContentLocation);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            EventLogger.dispose();
            _outputWriter.Dispose();
            base.OnExit(e);
        }

        private async Task loadAsync(bool askForGameContentLocation)
        {
            bool canContinue = true;
            //check default install locations
            string? paksFolderPath = _model.usableGameContentIfExists();
            if (askForGameContentLocation || string.IsNullOrWhiteSpace(paksFolderPath))
            {
                //show dialog asking for install location
                canContinue = showGameFilesWindow(ref paksFolderPath);
            }

            if (!string.IsNullOrWhiteSpace(paksFolderPath))
            {
                try
                {
                    await loadGameContentAsync(paksFolderPath!);
                }
                catch (Exception e)
                {
                    //Clear the path saved in the registry because it might be the cause of the exception
                    _model.unloadGameContent();

                    var title = $"{Constants.APPLICATION_NAME} {Constants.CURRENT_VERSION} - {R.ERROR}";
                    var message = $"{R.FAILED_TO_LOAD_GAME_CONTENT_ERROR_TITLE}\n\n{e.Message}\n\n{R.PLEASE_HAVE_LATEST_VERSION}\n\n{R.LAUNCH_WITH_LIMITED_FEATURES_QUESTION}";
                    var result = MessageBox.Show(message, title, MessageBoxButton.YesNo);
                    canContinue = result == MessageBoxResult.Yes || result == MessageBoxResult.OK;
                }
            }
            else
            {
                //Clear the path saved in the registry
                _model.unloadGameContent();
            }

            if (!canContinue)
            {
                //User opted to Exit
                _splashWindow?.Close();
                closeBusyIndicator();
                this.MainWindow?.Close();
                this.Shutdown();
                return;
            }

            showMainWindow();
        }

        private bool showGameFilesWindow(ref string? selectedPath)
        {
            EventLogger.logEvent("showGameFilesWindow");
            var gameFilesWindow = WindowFactory.createGameFilesWindow(selectedPath, allowNoContent: true);
            gameFilesWindow.ShowDialog();
            var gameFilesWindowResult = gameFilesWindow.result;
            switch (gameFilesWindowResult)
            {
                case GameFilesWindow.GameFilesWindowResult.exit:
                    selectedPath = null;
                    return false;
                case GameFilesWindow.GameFilesWindowResult.useSelectedPath:
                    selectedPath = gameFilesWindow.selectedPath!;
                    return true;
                case GameFilesWindow.GameFilesWindowResult.noContent:
                    selectedPath = null;
                    return true;
            }
            throw new NotImplementedException();
        }

        private async Task<bool> loadGameContentAsync(string paksFolderPath)
        {
            showBusyIndicator();
            _model.initPakReader();
            await _model.loadGameContentAsync(paksFolderPath);
            await preloadImages();
            return true;
        }

        private Task<bool> preloadImages()
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() => {
#if !HIDE_MAP_SCREENS
                LevelImagePanel.preload();
                MapScreen.preload(); //This takes a while
#endif
                InventoryTab.preload();

#if !HIDE_CHEST_TAB
                ChestTab.preload();
#endif
                EquipmentScreen.preload();
                ItemListScreen.preload();
                SelectionWindow.preload();
                ItemControl.preload();
                tcs.SetResult(true);
            });
            return tcs.Task;
        }

        private void showMainWindow()
        {
            EventLogger.logEvent("showMainWindow", new Dictionary<string, object>() { { "gameContentLoaded", AppModel.gameContentLoaded.ToString() } });
            var mainWindow = WindowFactory.createMainWindow(_model.mainModel);
            mainWindow.onRelaunch = onRelaunch;
            mainWindow.onReload = onReload;
            this.MainWindow = mainWindow;

            _splashWindow?.Close();
            closeBusyIndicator();

            this.MainWindow.Show();
        }

        private void onRelaunch()
        {
            showSplashWindowReplacingOldWindow();
            _ = loadAsync(askForGameContentLocation: true);
        }

        private void onReload(string? autoReloadFilename, ProfileSaveFile? profile)
        {
            var oldMainWindow = this.MainWindow;
            var mainWindow = WindowFactory.createMainWindow(_model.mainModel);
            mainWindow.onRelaunch = onRelaunch;
            mainWindow.onReload = onReload;
            this.MainWindow = mainWindow;
            oldMainWindow?.Close();
            this.MainWindow.Show();

            if (!string.IsNullOrWhiteSpace(autoReloadFilename))
            {
                if(profile != null)
                {
                    _model.mainModel.setProfile(autoReloadFilename!, profile);
                }
                else
                {
                    mainWindow.handleFileOpenAsync(autoReloadFilename!);
                }
            }
        }

        private void showSplashWindowReplacingOldWindow()
        {
            var oldMainWindow = this.MainWindow;
            _splashWindow = WindowFactory.createSplashWindow();
            if (Constants.IS_DEBUG)
            {
                //Turning this off for now
                //stopped working for some reason
                //_controlWriter = new ControlWriter(_splashWindow.textbox);
                //_outputWriter.addWriter(_controlWriter);
            }

            MainWindow = _splashWindow;
            oldMainWindow?.Close();
            this.MainWindow.Show();
        }

        private void showBusyIndicator()
        {
            closeBusyIndicator();

            _busyWindow = WindowFactory.createBusyWindow();
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

#region Dispose
        //Implemented as described here: https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

        private bool _disposed = false;

        ~App()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose managed state (managed objects).
                _outputWriter.Dispose();
                _controlWriter?.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            _disposed = true;
        }

#endregion
    }
}
