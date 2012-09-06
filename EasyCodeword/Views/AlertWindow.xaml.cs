using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EasyCodeword.Utilities;

namespace EasyCodeword.Views
{
    /// <summary>
    /// AlertWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AlertWindow : Window, IDisposable
    {
        private static AlertWindow _instnace;
        private readonly Timer _timer;
        private bool _flag = false;
        private bool _isDisposed;

        public AlertWindow()
        {
            InitializeComponent();
            _timer = new Timer(TimerCalback);
            this.Loaded += AlertWindow_Loaded;
        }

        private void AlertWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Change(1000, 1000);
        }

        private void TimerCalback(object state)
        {
            var count = Converter.ToInt(SecondRun.Text);
            count--;

            this.Dispatcher.Invoke(new Action(() =>
            {
                if (count < 1)
                {
                    this.Close();
                }
                else
                {
                    SecondRun.Text = count.ToString();
                }
            }));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            this.Close();
            base.OnMouseDown(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_flag != true)
            {
                var unLoadStoryboard = Resources["UnLoadStoryboard"] as Storyboard;
                if (null != unLoadStoryboard)
                {
                    e.Cancel = true;
                    unLoadStoryboard.Completed += UnLoadStoryboard_Completed;
                    unLoadStoryboard.Begin();
                }
            }
            base.OnClosing(e);
        }

        private void UnLoadStoryboard_Completed(object sender, EventArgs e)
        {
            _flag = true;
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instnace.Dispose();
            _instnace = null;

            MainWindow.Instance.Focus();
            MainWindow.Instance.MainTextBox.Focus();
        }

        public static void ShowAlert(string message, string title = "消息")
        {
            if (null != _instnace)
            {
                _instnace._flag = true;
                _instnace.Close();
            }

            _instnace = new AlertWindow();
            _instnace.Title = _instnace.TitleRun.Text = title;
            _instnace.MessageRun.Text = message;
            _instnace.Owner = MainWindow.Instance;
            _instnace.Show();

            MainWindow.Instance.Focus();
            MainWindow.Instance.MainTextBox.Focus();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _timer.Dispose();
                }

                // Release unmanaged resources

                _isDisposed = true;
            }
        }

        ~AlertWindow()
        {
            Dispose(false);
        }
    }
}
