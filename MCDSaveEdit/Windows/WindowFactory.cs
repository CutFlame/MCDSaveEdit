using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#nullable enable

namespace MCDSaveEdit
{
    public class SplashWindow : Window
    {
        public Label label = new Label();
        public TextBox textbox = new TextBox();
        public StackPanel stack = new StackPanel();

        public SplashWindow() : base()
        {
            label.FontSize = 40;
            label.FontWeight = FontWeights.ExtraBold;
            label.Content = R.APPLICATION_TITLE;

            textbox.Width = 700;
            textbox.Height = 700;
            textbox.TextWrapping = TextWrapping.NoWrap;
            if (Constants.IS_DEBUG)
            {
                textbox.Visibility = Visibility.Visible;
            }
            else
            {
                textbox.Visibility = Visibility.Collapsed;
            }

            stack.Orientation = Orientation.Vertical;
            stack.VerticalAlignment = VerticalAlignment.Stretch;
            stack.Children.Add(label);
            stack.Children.Add(textbox);

            this.SizeToContent = SizeToContent.Width;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Content = stack;
        }
    }


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
