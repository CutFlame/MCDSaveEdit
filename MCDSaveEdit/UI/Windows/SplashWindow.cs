using MCDSaveEdit.Data;
using MCDSaveEdit.Services;
using System.Windows;
using System.Windows.Controls;
#nullable enable

namespace MCDSaveEdit.UI
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
}
