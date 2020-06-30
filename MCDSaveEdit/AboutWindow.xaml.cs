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
            versionLabel.Content = R.formatVERSION(Constants.CURRENT_RELEASE_TAG_NAME);
        }

        private void supportMeButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.patreon.com/cutflame");
            this.Close();
        }
    }
}
