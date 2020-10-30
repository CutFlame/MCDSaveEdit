using FModel;
using PakReader.Parsers.Objects;
using System.Windows;

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Window _busyWindow = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Globals.Game = new FGame(EGame.MinecraftDungeons, EPakVersion.FNAME_BASED_COMPRESSION_METHOD);

            loadAsync();
        }

        private async void loadAsync()
        {
            if (ImageUriHelper.canUseGameContent())
            {
                showBusyIndicator();
                await ImageUriHelper.loadGameContentAsync();
            }

            showMainWindow();
        }

        private void showMainWindow()
        {
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
