using FModel;
using PakReader;
using PakReader.Pak;
using PakReader.Parsers.Objects;
using System;
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
            showBusyIndicator();

            loadPakIndex();

            doneLoading();
        }

        private void loadPakIndex()
        {
            try
            {
                Globals.Game = new FGame(EGame.MinecraftDungeons, EPakVersion.FNAME_BASED_COMPRESSION_METHOD);

                var filter = new PakFilter(new[] { "/dungeons/content" });
                var pakIndex = new PakIndex(path: Constants.PAKS_FOLDER, cacheFiles: true, caseSensitive: false, filter: filter);
                pakIndex.UseKey(BinaryHelper.ToBytesKey(Constants.PAKS_AES_KEY_STRING));
                ImageUriHelper.instance = new PakImageResolver(pakIndex);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Could not load Minecraft Dungeons Paks: {e}");
            }
        }

        private void doneLoading()
        {
            var mainWindow = new MainWindow();
            mainWindow.model = new ProfileViewModel();
            mainWindow.Show();

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
