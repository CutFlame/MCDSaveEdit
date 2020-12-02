using FModel;
using PakReader.Parsers.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Window? _busyWindow = null;
        private bool _launchToMissionViewer = false;
        private bool _askForGameContentLocation = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _launchToMissionViewer = e.Args.Contains("LAUNCH_TO_MISSION_VIEWER");
            _askForGameContentLocation = e.Args.Contains("ASK_FOR_GAME_CONTENT_LOCATION");
            EventLogger.init();
            initPakReader();
            loadAsync();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            EventLogger.dispose();
            base.OnExit(e);
        }

        private void initPakReader()
        {
            Globals.Game = new FGame(EGame.MinecraftDungeons, EPakVersion.FNAME_BASED_COMPRESSION_METHOD);
        }

        private async void loadAsync()
        {
            showBusyIndicator();

            //check default install locations
            string? paksFolderPath = ImageUriHelper.usableGameContentIfExists();
            if (_askForGameContentLocation || string.IsNullOrWhiteSpace(paksFolderPath))
            {
                //show dialog asking for install location
                EventLogger.logEvent("showGameFilesWindow");
                var gameFilesWindow = new GameFilesWindow();
                gameFilesWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                gameFilesWindow.ShowDialog();
                var gameFilesWindowResult = gameFilesWindow.result;
                if (gameFilesWindowResult == GameFilesWindow.GameFilesWindowResult.exit)
                {
                    closeBusyIndicator();
                    this.Shutdown();
                    return;
                }
                if (gameFilesWindowResult == GameFilesWindow.GameFilesWindowResult.useSelectedPath)
                {
                    paksFolderPath = gameFilesWindow.selectedPath;
                }
            }

            if (!string.IsNullOrWhiteSpace(paksFolderPath))
            {
                await ImageUriHelper.loadGameContentAsync(paksFolderPath!);
            }

            showMainWindow();
        }

        private void showMainWindow()
        {
            EventLogger.logEvent("showMainWindow", new Dictionary<string, object>() { { "canUseGameContent", (!string.IsNullOrWhiteSpace(Constants.PAKS_FOLDER_PATH)).ToString() } });
            if (_launchToMissionViewer)
            {
                var mainWindow = new MapWindow();
                this.MainWindow = mainWindow;
            }
            else
            {
                var mainWindow = new MainWindow();
                mainWindow.model = new ProfileViewModel();
                this.MainWindow = mainWindow;
            }
            this.MainWindow.Show();

            closeBusyIndicator();
        }

        private void showBusyIndicator()
        {
            closeBusyIndicator();

            _busyWindow = new Window();
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

    }
}
