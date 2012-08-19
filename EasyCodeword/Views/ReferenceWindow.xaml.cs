using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using EasyCodeword.Utilities;
using System.Windows.Media.Animation;
using System.IO;
using Microsoft.Win32;
using EasyCodeword.Core;

namespace EasyCodeword.Views
{
    /// <summary>
    /// ReferenceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReferenceWindow : Window
    {
        private readonly Storyboard _showMessageStoryboard = null;

        public ReferenceWindow()
        {
            InitializeComponent();
            _showMessageStoryboard = Resources["ShowMessageStoryboard"] as Storyboard;
            this.Loaded += ReferenceWindow_Loaded;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            SettingViewModel.Instance.ReferenceLocation = new Rect(this.Left, this.Top, this.ActualWidth, this.ActualHeight);
        }

        private void ReferenceWindow_Loaded(object sender, EventArgs e)
        {
            var hWnd = new WindowInteropHelper(this).Handle;
            Common.DisableMinmize(hWnd);

            var rect = SettingViewModel.Instance.ReferenceLocation;
            this.Left = rect.Left;
            this.Top = rect.Top;
            this.Width = rect.Width;
            this.Height = rect.Height;

            if (File.Exists(SettingViewModel.Instance.ReferenceFile))
            {
                OpenFile(SettingViewModel.Instance.ReferenceFile);
            }
            else
            {
                Open();
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Search(SearchWindowTextBox.Text, false);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void Open()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文件(*.txt;*.rtf)|*.txt;*.rtf|文本文件(*.txt)|*.txt|RTF文件(*.rtf)|*.rtf";
            if (openFileDialog.ShowDialog() == true)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        private void Search(string searchWords, bool caseSensiteive)
        {
            StringComparison stringComparsion =
                caseSensiteive ?
                StringComparison.CurrentCulture
                : StringComparison.CurrentCultureIgnoreCase;
            var start = ReferenceTextBox.SelectionStart + ReferenceTextBox.SelectionLength;
            var index = ReferenceTextBox.Text.IndexOf(searchWords, start, stringComparsion);
            if (index > -1)
            {
                ReferenceTextBox.Select(index, searchWords.Length);
                ReferenceTextBox.Focus();
            }
            else
            {
                ShowMessage("已超过文档结尾。");
                start = 0;
                index = ReferenceTextBox.Text.IndexOf(searchWords, start, stringComparsion);
                if (index > -1)
                {
                    ReferenceTextBox.Select(index, searchWords.Length);
                    ReferenceTextBox.Focus();
                }
            }
        }

        private void ShowMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                _showMessageStoryboard.Stop();
            }
            else
            {
                MessageTextBlock.Text = message;
                _showMessageStoryboard.Begin();
            }
        }

        private void ShowMessage(string message, object arg0)
        {
            ShowMessage(string.Format(message, arg0));
        }

        private void ShowMessage(string message, params object[] args)
        {
            ShowMessage(string.Format(message, args));
        }

        private void OpenFile(string fileName)
        {
            this.ReferenceTextBox.Text = File.ReadAllText(fileName, Encoding.Default);
            this.ReferenceTextBox.Focus();
            SettingViewModel.Instance.ReferenceFile = fileName;
        }
    }
}
