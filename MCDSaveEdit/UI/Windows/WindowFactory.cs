using MCDSaveEdit.Logic;
using MCDSaveEdit.ViewModels;
using System.Windows;
using System.Windows.Media;
#nullable enable

namespace MCDSaveEdit.UI
{
    public static class WindowFactory
    {
        public static SplashWindow createSplashWindow()
        {
            return new SplashWindow();
        }

        public static Window createBusyWindow()
        {
            var busyWindow = new Window();
            busyWindow.Owner = Application.Current.MainWindow;
            busyWindow.Height = 200;
            busyWindow.Width = 200;
            busyWindow.ResizeMode = ResizeMode.NoResize;
            busyWindow.WindowStyle = WindowStyle.None;
            busyWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            busyWindow.AllowsTransparency = true;
            busyWindow.Background = new SolidColorBrush(Colors.Transparent);
            busyWindow.Content = new BusyIndicator();
            return busyWindow;
        }

        public static GameFilesWindow createGameFilesWindow(string? defaultPath, bool allowNoContent)
        {
            var gameFilesWindow = new GameFilesWindow(defaultPath, allowNoContent: allowNoContent);
            gameFilesWindow.Owner = Application.Current.MainWindow;
            gameFilesWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return gameFilesWindow;
        }

        public static MainWindow createMainWindow(MainViewModel model)
        {
            var mainWindow = new MainWindow(model);
            mainWindow.Width = 1200;
            mainWindow.Height = 675;
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return mainWindow;
        }

        public static ISelectionWindow createSelectionWindow()
        {
#if USE_SIMPLE_SELECTION
            var selectionWindow = new SimpleSelectionWindow();
#else
            var selectionWindow = new SelectionWindow();
#endif
            var mainWindow = Application.Current.MainWindow;
            selectionWindow.Owner = mainWindow;
            selectionWindow.Top = mainWindow.Top;
            selectionWindow.Height = mainWindow.Height;
            // Adjust the window placement so it doesn't show off the screen
            var rightSideSpace = SystemParameters.PrimaryScreenWidth - (mainWindow.Left + mainWindow.Width);
            if (rightSideSpace < selectionWindow.Width)
            {
                selectionWindow.Left = mainWindow.Left + mainWindow.Width - selectionWindow.Width;
            }
            else
            {
                selectionWindow.Left = mainWindow.Left + mainWindow.Width;
            }

            return selectionWindow;
        }

        public static AboutWindow createAboutWindow()
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = Application.Current.MainWindow;
            aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return aboutWindow;
        }
    }
}
