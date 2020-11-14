using FModel;
using PakReader.Parsers.Objects;
using System.Collections.Generic;
using System.Windows;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Window? _busyWindow = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
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
            string? paksFolderPath = ImageUriHelper.usableGameContentIfExists();
            if (!string.IsNullOrWhiteSpace(paksFolderPath))
            {
                showBusyIndicator();
                await ImageUriHelper.loadGameContentAsync(paksFolderPath!);
            }

            showMainWindow();
        }

        private void showMainWindow()
        {
            EventLogger.logEvent("showMainWindow", new Dictionary<string, object>() { { "canUseGameContent", (!string.IsNullOrWhiteSpace(Constants.PAKS_FOLDER_PATH)).ToString() } });
            var mainWindow = new MainWindow();
            mainWindow.model = new ProfileViewModel();
            this.MainWindow = mainWindow;
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
