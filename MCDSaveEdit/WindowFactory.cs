using System.Windows;
using System.Windows.Controls;
#nullable enable

namespace MCDSaveEdit
{
    public static class WindowFactory
    {
        public static Window createSplashWindow()
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

        public static Window createBusyWindow()
        {
            var busyWindow = new Window();
            busyWindow.Owner = Application.Current.MainWindow;
            busyWindow.Height = 200;
            busyWindow.Width = 200;
            busyWindow.ResizeMode = ResizeMode.NoResize;
            busyWindow.WindowStyle = WindowStyle.None;
            busyWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
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

        public static MainWindow createMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Width = 1200;
            mainWindow.Height = 675;
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return mainWindow;
        }

        public static SelectionWindow createSelectionWindow()
        {
            var selectionWindow = new SelectionWindow();
            selectionWindow.Left = Application.Current.MainWindow.Left + Application.Current.MainWindow.Width;
            selectionWindow.Top = Application.Current.MainWindow.Top;
            selectionWindow.Owner = Application.Current.MainWindow;
            return selectionWindow;
        }

        public static AboutWindow createAboutWindow()
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = Application.Current.MainWindow;
            return aboutWindow;
        }
    }
}
