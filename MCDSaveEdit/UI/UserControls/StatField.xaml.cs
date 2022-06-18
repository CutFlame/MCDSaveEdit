using System;
using System.Windows;
using System.Windows.Controls;
#nullable enable

namespace MCDSaveEdit.UI
{
    /// <summary>
    /// Interaction logic for StatField.xaml
    /// </summary>
    public partial class StatField : UserControl
    {
        public event TextChangedEventHandler ValueChanged {
            add { textBox.TextChanged += value; }
            remove { textBox.TextChanged -= value; }
        }

        public string Title {
            get {
                return textBlock.Text;
            }
            set {
                textBlock.Text = value;
            }
        }

        public string Value {
            get {
                return textBox.Text;
            }
            set {
                textBox.Text = value;
            }
        }

        public long MaxValue { get; set; } = long.MaxValue;
        public long MinValue { get; set; } = long.MinValue;

        public long? numberValue {
            get {
                if (long.TryParse(this.Value, out long currentValue))
                {
                    return currentValue;
                }
                return null;
            }
        }

        public StatField()
        {
            InitializeComponent();
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            var tempValue = this.numberValue;
            if (tempValue != null)
            {
                textBox.Text = Math.Min(tempValue.Value + 1, this.MaxValue).ToString();
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            var tempValue = this.numberValue;
            if (tempValue != null)
            {
                textBox.Text = Math.Max(tempValue.Value - 1, this.MinValue).ToString();
            }
        }

    }
}
