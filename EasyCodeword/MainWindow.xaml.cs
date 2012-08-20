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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasyCodeword.Views;
using EasyCodeword.Core;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Animation;
using System.Threading;
using EasyCodeword.Utilities;
using System.Text.RegularExpressions;

namespace EasyCodeword
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 变量

        private SearchWindow _searchWindow = null;

        private ReplaceWindow _replaceWindow = null;

        private ReferenceWindow _referenceWindow = null;

        private readonly Storyboard _showMessageStoryboard = null;

        private readonly Timer _autoSaveTimer;
        
        #endregion

        #region 属性
        
        public static MainWindow Instance { get; private set; }
        
        #endregion
        
        #region 构造方法

        public MainWindow()
        {
            InitializeComponent();
            _showMessageStoryboard = Resources["ShowMessageStoryboard"] as Storyboard;
            _autoSaveTimer = new Timer(AutoSaveCallback);
            this.DataContext = MainViewModel.Instance;
            this.Loaded += MainWindow_Loaded;
            Instance = this;
        }

        #endregion

        #region 事件

        #region Menu 事件

        private void Top_MouseEnter(object sender, MouseEventArgs e)
        {
            MenuPopup.Width = SystemParameters.FullPrimaryScreenWidth;
            MenuPopup.IsOpen = true;
        }

        private void MenuPopup_Opened(object sender, EventArgs e)
        {
            MainTextBoxBorder.SetValue(MarginProperty, new Thickness(0d, 150d, 0d, 0d));
        }

        private void MenuPopup_Closed(object sender, EventArgs e)
        {
            MainTextBoxBorder.ClearValue(MarginProperty);
            SetFocus();
        }

        private void MenuGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuPopup.IsOpen = false;
        }

        #endregion

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingViewModel.Instance.AutoSave)
            {
                var interval = SettingViewModel.Instance.AutoSaveInterval * 60 * 1000;
                if (interval < 60 * 1000
                    || interval > 40 * 24 * 60 * 1000)
                {
                    interval = 2 * 60 * 1000;
                }

                _autoSaveTimer.Change(interval, interval);
            }

            var recentFile = SettingViewModel.Instance.RecentFile;
            if (File.Exists(recentFile))
            {
                OpenFile(recentFile);
                
                if (File.Exists(Common.TempFile))
                {
                    LoadAccidentFile();
                }
            }
            else if (File.Exists(Common.TempFile))
            {
                LoadAccidentFile();
            }
            else
            {
                New();
            }

            SetBackground(SettingViewModel.Instance.Background);
            SetForeground(SettingViewModel.Instance.Foreground);
            SetFontFamily(SettingViewModel.Instance.__FontFamily.FontFamily);
            SetFontStyle(SettingViewModel.Instance.__FontStyle);
            SetFontSize(SettingViewModel.Instance.__FontSize.FontSize);

            // 播放背景音乐
            if (SettingViewModel.Instance.AutoPlayMusic)
            {
                SoundPlayerViewModel.Instance.Play(SettingViewModel.Instance.MusicFolder);
            }
        }

        private void LoadAccidentFile()
        {
            var fileInfo = new FileInfo(Common.TempFile);
            if (MessageBox.Show(this,
                string.Format("上次[{0:yyyy-MM-dd HH:mm:ss}]程序意外退出，是否加载系统自动保存的内容？",
                fileInfo.LastWriteTime),
                "提示",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                OpenFile(Common.TempFile);
                File.Delete(Common.TempFile);
            }
            else
            {
                New();
            }
        }

        private void New()
        {
            MainTextBox.AppendText("　　");
            SetFocus(true);
        }

        public void SetFocus(bool flag = false)
        {
            if ((null != _searchWindow && _searchWindow.IsActive)
                && (null != _replaceWindow && _replaceWindow.IsActive))
            {
                return;
            }

            if (flag)
            {
                MainTextBox.Select(MainTextBox.Text.Length, 0);
            }
            MainTextBox.Focus();
        }

        private void MainTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainTextBox.AppendText("\r\n\r\n　　");
                MainTextBox.Select(MainTextBox.Text.Length, 0);
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control
                && e.Key == Key.F)
            {
                Search();
            }
            if (Keyboard.Modifiers == ModifierKeys.Control
                && e.Key == Key.H)
            {
                Replace();
            }
            else if (e.Key == Key.F3)
            {
                SearchWindow.Search();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SettingViewModel.Instance.RecentFile = MainViewModel.Instance.FileName;

            try
            {
                if (string.IsNullOrEmpty(MainViewModel.Instance.FileName))
                {
                    SaveFile();
                }
                else
                {
                    File.WriteAllText(MainViewModel.Instance.FileName, this.MainTextBox.Text, Encoding.Default);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "文件保存异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            base.OnClosed(e);
        }

        #region 文件

        private void New_Click(object sender, RoutedEventArgs e)
        {
            this.MainTextBox.Text = string.Empty;
            MenuPopup.IsOpen = false;
            MainViewModel.Instance.FileName = string.Empty;
            New();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文件(*.txt;*.rtf)|*.txt;*.rtf|文本文件(*.txt)|*.txt|RTF文件(*.rtf)|*.rtf";
            if (openFileDialog.ShowDialog() == true)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        private void OpenFile(string fileName)
        {
            this.MainTextBox.Text = File.ReadAllText(fileName, Encoding.Default);
            MainViewModel.Instance.FileName = fileName;
            SetFocus(true);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(MainViewModel.Instance.FileName))
                {
                    SaveFile();
                }
                else
                {
                    File.WriteAllText(MainViewModel.Instance.FileName, this.MainTextBox.Text, Encoding.Default);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "文件保存异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "文件另存为异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveFile()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|RFT文件(*.rft)|*.rft";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, this.MainTextBox.Text, Encoding.Default);
                MainViewModel.Instance.FileName = saveFileDialog.FileName;
                try
                {
                    File.Delete(Common.TempFile);
                }
                catch{}
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 操作

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            if (MainTextBox.Text.Length > 0)
            {
                if (null != _replaceWindow)
                {
                    _replaceWindow.Close();
                }

                if (null == _searchWindow)
                {
                    _searchWindow = new SearchWindow();
                    _searchWindow.Owner = this;
                    _searchWindow.Closed += SearchWindow_Closed;
                    _searchWindow.Show();
                }
                else
                {
                    _searchWindow.Activate();
                }
            }
        }

        private void SearchWindow_Closed(object sender, EventArgs e)
        {
            _searchWindow = null;
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            Replace();
        }

        private void Replace()
        {
            if (MainTextBox.Text.Length > 0)
            {
                if (null != _searchWindow)
                {
                    _searchWindow.Close();
                }

                if (null == _replaceWindow)
                {
                    _replaceWindow = new ReplaceWindow();
                    _replaceWindow.Owner = this;
                    _replaceWindow.Closed += ReplaceWindow_Closed;
                    _replaceWindow.Show();
                }
                else
                {
                    _replaceWindow.Activate();
                }
            }
        }

        private void ReplaceWindow_Closed(object sender, EventArgs e)
        {
            _replaceWindow = null;
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            var settingWindow = new SettingWindow();
            settingWindow.Owner = this;
            settingWindow.ShowDialog();
            SetFocus();
        }

        #endregion

        private void AutoSaveCallback(object state)
        {
            this.MainTextBox.Dispatcher.Invoke(new Action(() =>
            {
                File.WriteAllText(Common.TempFile, this.MainTextBox.Text, Encoding.Default);
            }));
        }

        #endregion

        #region 公共方法

        public void SetBackground(Brush brush)
        {
            this.Background = brush;
            this.MainTextBox.Background = brush;
            this.MenuGrid.Background = brush;
        }

        public void SetForeground(Brush brush)
        {
            this.Foreground = brush;
            this.MainTextBox.Foreground = brush;
        }

        public void SetFontFamily(FontFamily fontFamily)
        {
            this.MainTextBox.FontFamily = fontFamily;
        }

        public void SetFontStyle(EasyCodeword.Core.SettingViewModel._FontStyle fontStyle)
        {
            this.MainTextBox.FontStyle = fontStyle.FontStyle;
            this.MainTextBox.FontWeight = fontStyle.FontWeight;
        }

        public void SetFontSize(double fontSize)
        {
            this.MainTextBox.FontSize = fontSize;
        }

        public void ShowMessage(string message)
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

        public void ShowMessage(string message, object arg0)
        {
            ShowMessage(string.Format(message, arg0));
        }

        public void ShowMessage(string message, params object[] args)
        {
            ShowMessage(string.Format(message, args));
        }

        #endregion

        private void Refrence_Click(object sender, RoutedEventArgs e)
        {
            if (null == _referenceWindow)
            {
                _referenceWindow = new ReferenceWindow();
                _referenceWindow.Closed += ReferenceWindow_Closed;
                _referenceWindow.Owner = this;
                _referenceWindow.Show();
            }
            else
            {
                _referenceWindow.Activate();
            }
            SetFocus();
        }

        private void ReferenceWindow_Closed(object sender, EventArgs e)
        {
            _referenceWindow = null;
        }

        private void AutoTypeSetting_Click(object sender, RoutedEventArgs e)
        {
            this.MainTextBox.Text =
                Regex.Replace(this.MainTextBox.Text,
                    @"([\s]*[\r\n]+[\s]*)|([\s]{5,})",
                    "\r\n\r\n　　");
        }

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainTextBox.Text.Length > 0)
            {
                CharacterCountTextBlock.Text = string.Format("{0}\\{1}",
                    Regex.Matches(MainTextBox.Text, "[\u4e00-\u9fff]").Count
                    , MainTextBox.Text.Length);
            }
            else
            {
                CharacterCountTextBlock.Text = string.Empty;
            }
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            var powerWindow = new PowerWindow();
            powerWindow.Owner = this;
            powerWindow.ShowDialog();
        }
    }
}
