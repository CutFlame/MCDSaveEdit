using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
#nullable enable

namespace MCDSaveEdit
{
    public class ControlWriter : TextWriter
    {
        private TextBox textbox;
        private StringBuilder textBuilder = new StringBuilder();
        public ControlWriter(TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textBuilder.Append(value);
            if (value == '\n')
            {
                updateUI();
            }
        }

        public override void Write(string value)
        {
            textBuilder.Append(value);
            updateUI();
        }

        public override void Flush()
        {
            base.Flush();
            updateUI();
        }

        private void updateUI()
        {
            this.ExecuteOnMainThreadWithNonnullThis(nonnullThis => {
                if (!nonnullThis._isDisposed)
                {
                    //Update UI here
                    nonnullThis.textbox.Text = nonnullThis.textBuilder.ToString();
                    nonnullThis.textbox.ScrollToEnd();
                }
            });
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }

        private bool _isDisposed = false;
        protected override void Dispose(bool disposing)
        {
            _isDisposed = true;
            base.Dispose(disposing);
        }
    }

    public static class MyExtensions
    {
        public static void ExecuteOnMainThreadWithNonnullThis<T>(this T lhs, Action<T> funcToRun) where T : class
        {
            lhs.ExecuteOnMainThreadWithWeakThis(weakThis => {
                if (weakThis.TryGetTarget(out T nonnullThis))
                {
                    funcToRun(nonnullThis);
                }
            });
        }
        public static void ExecuteOnMainThreadWithWeakThis<T>(this T lhs, Action<WeakReference<T>> funcToRun) where T : class
        {
            var weakThis = new WeakReference<T>(lhs);
            _ = Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate {
                funcToRun(weakThis);
            }));
        }
    }

    public class MultiTextWriter : TextWriter
    {
        private List<TextWriter> _writers;
        public MultiTextWriter(IEnumerable<TextWriter> writers)
        {
            this._writers = writers.ToList();
        }

        public MultiTextWriter(params TextWriter[] writers)
        {
            this._writers = writers.ToList();
        }

        public void addWriter(TextWriter writer)
        {
            _writers.Add(writer);
        }

        public void removeWriter(TextWriter writer)
        {
            _writers.Remove(writer);
        }

        public override void Write(char value)
        {
            foreach (var writer in _writers)
                writer.Write(value);
        }

        public override void Write(string value)
        {
            foreach (var writer in _writers)
                writer.Write(value);
        }

        public override void Flush()
        {
            foreach (var writer in _writers)
                writer.Flush();
        }

        public override void Close()
        {
            foreach (var writer in _writers)
                writer.Close();
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
