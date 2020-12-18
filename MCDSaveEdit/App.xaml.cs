using FModel;
using PakReader.Parsers.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Window? _splashWindow = null;
        private Window? _busyWindow = null;
        private bool _askForGameContentLocation = false;
        private bool _skipGameContent = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _askForGameContentLocation = e.Args.Contains("ASK_FOR_GAME_CONTENT_LOCATION");
            _skipGameContent = e.Args.Contains("SKIP_GAME_CONTENT");
            EventLogger.init();
            initPakReader();

            _splashWindow = buildSplashWindow();
            MainWindow = _splashWindow;
            this.MainWindow.Show();

            if (_skipGameContent)
            {
                showMainWindow();
            }
            else
            {
                load();
            }
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

        private void load()
        {
            //check default install locations
            string? paksFolderPath = ImageUriHelper.usableGameContentIfExists();
            if (_askForGameContentLocation || string.IsNullOrWhiteSpace(paksFolderPath))
            {
                //show dialog asking for install location
                EventLogger.logEvent("showGameFilesWindow");
                var gameFilesWindow = new GameFilesWindow();
                gameFilesWindow.Owner = this.MainWindow;
                gameFilesWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                gameFilesWindow.ShowDialog();
                var gameFilesWindowResult = gameFilesWindow.result;
                switch (gameFilesWindowResult)
                {
                    case GameFilesWindow.GameFilesWindowResult.exit:
                        this.Shutdown();
                        break;
                    case GameFilesWindow.GameFilesWindowResult.useSelectedPath:
                        loadGameContentAsync(gameFilesWindow.selectedPath!);
                        break;
                    case GameFilesWindow.GameFilesWindowResult.noContent:
                        showMainWindow();
                        break;
                }
            }
            else
            {
                loadGameContentAsync(paksFolderPath!);
            }
        }

        private async void loadGameContentAsync(string paksFolderPath)
        {
            showBusyIndicator();
            await ImageUriHelper.loadGameContentAsync(paksFolderPath);
            await preloadImages();
            showMainWindow();
        }

        private Task<bool> preloadImages()
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() => {
                LevelImagePanel.preload();
                MainlandMapScreen.preload();
                JungleAwakensMapScreen.preload();
                CreepingWinterMapScreen.preload();
                HowlingPeaksMapScreen.preload();
                InventoryScreen.preload();
                SelectionWindow.preload();
                ItemControl.preload();
                tcs.SetResult(true);
            });
            return tcs.Task;
        }

        private void showMainWindow()
        {
            EventLogger.logEvent("showMainWindow", new Dictionary<string, object>() { { "canUseGameContent", (!string.IsNullOrWhiteSpace(Constants.PAKS_FOLDER_PATH)).ToString() } });
            var mainWindow = new MainWindow();
            mainWindow.model = new ProfileViewModel();
            this.MainWindow = mainWindow;

            _splashWindow?.Close();
            closeBusyIndicator();

            this.MainWindow.Show();
        }

        private Window buildSplashWindow()
        {
            var label = new Label();
            label.FontSize = 40;
            label.FontWeight = FontWeights.ExtraBold;
            label.Content = R.APPLICATION_TITLE;

            var window = new Window();
            window.SizeToContent = SizeToContent.Width;
            window.ResizeMode = ResizeMode.NoResize;
            window.WindowStyle = WindowStyle.None;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Content = label;

            return window;
        }

        private void showBusyIndicator()
        {
            closeBusyIndicator();

            _busyWindow = new Window();
            _busyWindow.Owner = this.MainWindow;
            _busyWindow.Height = 200;
            _busyWindow.Width = 200;
            _busyWindow.ResizeMode = ResizeMode.NoResize;
            _busyWindow.WindowStyle = WindowStyle.None;
            _busyWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
