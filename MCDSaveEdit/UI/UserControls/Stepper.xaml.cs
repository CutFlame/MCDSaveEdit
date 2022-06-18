using System.Windows;
using System.Windows.Controls;

namespace MCDSaveEdit.UI
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// https://stackoverflow.com/questions/841293/where-is-the-wpf-numeric-updown-control/5321605#5321605
    /// </summary>
    public partial class Stepper : UserControl
    {
        public static readonly RoutedEvent UpButtonClickEvent = EventManager.RegisterRoutedEvent("UpButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Stepper));
        public static readonly RoutedEvent DownButtonClickEvent = EventManager.RegisterRoutedEvent("DownButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Stepper));

        public event RoutedEventHandler UpButtonClick
        {
            add { AddHandler(UpButtonClickEvent, value); }
            remove { RemoveHandler(UpButtonClickEvent, value); }
        }

        public event RoutedEventHandler DownButtonClick
        {
            add { AddHandler(DownButtonClickEvent, value); }
            remove { RemoveHandler(DownButtonClickEvent, value); }
        }

        public Stepper()
        {
            InitializeComponent();
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(UpButtonClickEvent));
        }


        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(DownButtonClickEvent));
        }
    }
}
