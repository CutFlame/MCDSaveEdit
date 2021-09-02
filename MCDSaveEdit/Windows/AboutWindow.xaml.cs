using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            Title = R.ABOUT_WINDOW_TITLE;
            versionLabel.Content = R.formatVERSION(Config.instance.versionLabel(), Constants.CURRENT_VERSION.ToString());
        }

        private void patreonButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("patreonButton_Click");
            Process.Start("https://www.patreon.com/cutflame");
            this.Close();
        }
        private void buyMeACoffeeButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("buyMeACoffeeButton_Click");
            Process.Start("https://www.buymeacoffee.com/cutflame");
            this.Close();
        }
        private void koFiButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("koFiButton_Click");
            Process.Start("https://ko-fi.com/cutflame");
            this.Close();
        }

    }
}
