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

        private ILogger _logger = LoggerFactory.GetLogger(typeof(MainWindow).FullName);

        private SearchWindow _searchWindow = null;

        private ReplaceWindow _replaceWindow = null;

        private ReferenceWindow _referenceWindow = null;

        private AbountWindow _abountWindow = null;

        private TotalWindow _totalWindow = null;

        private readonly Storyboard _showMessageStoryboard = null;

        private readonly Timer _autoSaveTimer;

        private Storyboard _showMenuStoryboard;

        private Storyboard _showLockStoryboard;

        //private readonly Storyboard _showMenuStorybaord;

        //private readonly Storyboard _hideMenuStorybaord;
        
        #endregion

        #region 属性
        
        public static MainWindow Instance { get; private set; }

        /// <summary>
        /// 原始字数
        /// </summary>
        public int OriginWords { get; private set; }

        /// <summary>
        /// 当时字数
        /// </summary>
        public int Words { get; private set; }
        
        #endregion
        
        #region 构造方法

        public MainWindow()
        {

#if DEBUG
            LoggerFactory.SetLoggerLevel(LoggerLevel.Trance);
#else
            LoggerFactory.SetLoggerInstance(typeof(WriteFileLogger));
#endif
            
            InitializeComponent();
            _showMessageStoryboard = Resources["ShowMessageStoryboard"] as Storyboard;
            _autoSaveTimer = new Timer(AutoSaveCallback);
            this.Loaded += MainWindow_Loaded;
            Instance = this;

            _showMenuStoryboard = Resources["ShowMenuStoryboard"] as Storyboard;
            _showLockStoryboard = Resources["ShowLockStoryboard"] as Storyboard;

            //_showMenuStorybaord = Resources["ShowMenuStoryboard"] as Storyboard;
            //_hideMenuStorybaord = Resources["HideMenuStoryboard"] as Storyboard;
        }

        #endregion

        #region 事件

        #region Menu 事件

        //private void Top_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    //MenuPopup.Width = SystemParameters.FullPrimaryScreenWidth;
        //    //MenuPopup.IsOpen = true;
        //    //_hideMenuStorybaord.Stop();
        //    //_showMenuStorybaord.Begin();
        //}

        //private void MenuPopup_Opened(object sender, EventArgs e)
        //{
        //    MainTextBoxBorder.SetValue(MarginProperty, new Thickness(0d, 150d, 0d, 0d));
        //}

        //private void MenuPopup_Closed(object sender, EventArgs e)
        //{
        //    MainTextBoxBorder.ClearValue(MarginProperty);
        //    SetFocus();
        //}

        //private void MenuGrid_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    MenuPopup.IsOpen = false;
        //    //_showMenuStorybaord.Stop();
        //    //_hideMenuStorybaord.Begin();
        //}

        #endregion

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TypingSpeedViewModel.Init(MainTextBox);
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
                OpenFile(recentFile, false);
                
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

            if (!LicenseProvider.IsRegistered)
            {
                //AlertWindow.ShowAlert("软件尚未注册，如需注册请从帮助窗口(F1键)点击注册注册按钮", "软件注册");
                ShowMessage("软件尚未注册，如需注册请从帮助窗口(F1键)点击注册注册按钮");
            }


            this.MainMenuSubPopup.DataContext = MenuViewModel.FileMenu;
            this.LockSubBorder.DataContext = LockViewModel.Insatance;
            this.LockInfoTextBlock.DataContext = LockViewModel.Insatance;
            this.TopGrid.DataContext = MainViewModel.Instance;
            this.MusicStackPanel.DataContext = SoundPlayerViewModel.Instance;
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
                OpenFile(Common.TempFile, false);
            }
            else
            {
                New();
            }
            File.Delete(Common.TempFile);
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
            if (e.Key == Key.Return)
            {
                if (SettingViewModel.Instance.AutoSaveReturn)
                {
                    SaveCommand();
                }
                var index = MainTextBox.SelectionStart;
                MainTextBox.Text = MainTextBox.Text.Insert(MainTextBox.SelectionStart, "\r\n\r\n　　");
                MainTextBox.SelectionStart = index + "\r\n\r\n　　".Length;
                e.Handled = true;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control
                && e.Key == Key.F)
            {
                SearchCommand();
            }
            if (Keyboard.Modifiers == ModifierKeys.Control
                && e.Key == Key.H)
            {
                ReplaceCommand();
            }
            else if (e.Key == Key.F3)
            {
                SearchWindow.Search();
            }
            else if (e.Key == Key.F1)
            {
                AboutCommand();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control
                && e.Key == Key.N)
            {
                NewCommand();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control
               && e.Key == Key.O)
            {
                OpenCommand();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control
               && e.Key == Key.S)
            {
                SaveCommand();
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift)
               && e.Key == Key.S)
            {
                SaveAsCommand();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control
               && e.Key == Key.M)
            {
                this.WindowState = WindowState.Minimized;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Alt
                && e.Key == Key.G)
            {
                PowerCommand();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control
                && e.Key == Key.Tab)
            {
                RefrenceCommand();
            }
            else if (e.Key == Key.F2)
            {
                TotalCommand();
            }
            else if (e.Key == Key.F4)
            {
                if (LockViewModel.Insatance.IsUnlocked)
                {
                    Topmost = !Topmost;
                    if (Topmost)
                    {
                        ShowMessage("窗口已取消置顶，可以按快捷键F4取消或设置本窗口置顶");
                    }
                    else
                    {
                        ShowMessage("窗口已取消置顶！");
                    }
                }
            }
            else if (e.Key == Key.F5)
            {
                if (!SoundPlayerViewModel.Instance.IsPlaying)
                {
                    SoundPlayerViewModel.Instance.Play(SettingViewModel.Instance.MusicFolder);
                }
            }
            else if (e.Key == Key.F6)
            {
                SoundPlayerViewModel.Instance.Stop();
            }
            else if (e.Key == Key.F9)
            {
                AutoTypingSettingCommand();
            }
            else if (e.Key == Key.F12)
            {
                SettingCommand();
            }
            //else if (Keyboard.Modifiers == ModifierKeys.Alt
            //    && e.Key == Key.Q)
            //{
            //    ExitCommand();
            //}
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SettingViewModel.Instance.RecentFile = MainViewModel.Instance.FileName;
            try
            {
                if (string.IsNullOrEmpty(MainViewModel.Instance.FileName))
                {
                    if (Words > 0)
                    {
                        SaveFileAs();
                    }
                }
                else
                {
                    if (!SettingViewModel.Instance.AutoSaveExit)
                    {
                        var confirmWindow = new ConfirmWindow();
                        confirmWindow.Title = "提示";
                        confirmWindow.MessageTextBlock.Text = string.Concat(GetSaveCaption(), "\r\n\r\n是否保存当前文档？");
                        confirmWindow.RemeberCheckedBox.Content = "以后退出时自动保存当前文件";
                        confirmWindow.Topmost = true;
                        var result = confirmWindow.ShowDialog();
                        if (result != true)
                        {
                            base.OnClosing(e);
                            return;
                        }
                        SettingViewModel.Instance.AutoSaveExit = confirmWindow.RemeberCheckedBox.IsChecked == true;
                        SettingViewModel.Instance.SaveAutoSaveExit();
                    }
                    SaveFileTo(MainViewModel.Instance.FileName);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("保存文件出现异常：", ex.Message);
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            TypingSpeedViewModel.Instance.Total();
            _autoSaveTimer.Dispose();
            base.OnClosed(e);

            System.Environment.Exit(System.Environment.ExitCode); 
        }

        #region 文件

        private void New_Click(object sender, RoutedEventArgs e)
        {
            NewCommand();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenCommand();
        }

        private void OpenFile(string fileName, bool confirm = true)
        {
            try
            {
                var text = string.Empty;
                if (Regex.IsMatch(fileName, "\\.txt$", RegexOptions.IgnoreCase))
                {
                    text = File.ReadAllText(fileName, Encoding.Default);
                }
                else if (Regex.IsMatch(fileName, "\\.rtf$", RegexOptions.IgnoreCase))
                {
                    if (confirm && !SettingViewModel.Instance.RemeberRTF)
                    {
                        var confirmWindow = new ConfirmWindow();
                        confirmWindow.Title = "提示";
                        confirmWindow.MessageTextBlock.Text = "打开 RTF 格式的文件将丢失原有格式信息、和图片等对象，是否需要继续？";
                        confirmWindow.RemeberCheckedBox.Content = "下次不再提示";
                        confirmWindow.Owner = this;
                        var result = confirmWindow.ShowDialog();
                        SettingViewModel.Instance.RemeberRTF = confirmWindow.RemeberCheckedBox.IsChecked == true;
                        SettingViewModel.Instance.SaveRememberRTF();

                        if (result != true)
                        {
                            return;
                        }
                    }
                    text = RtfHelper.Read(fileName);
                }
                else // 其他
                {
                    text = File.ReadAllText(fileName, Encoding.Default);
                }
                TypingSpeedViewModel.Instance.Total();
                this.MainTextBox.Text = text;
                TypingSpeedViewModel.Instance.Refresh();
                if (string.Compare(fileName, Common.TempFile, true) != 0)
                {
                    MainViewModel.Instance.FileName = fileName;
                }
                OriginWords = MainViewModel.Instance.CountWords(MainTextBox.Text);
                SetFocus(true);
            }
            catch (Exception ex)
            {
                ShowMessage("打开文件({0})出现异常：{1}", fileName, ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveCommand();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAsCommand();
        }

        private void SaveFileAs()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = GetSaveCaption();
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|RTF文件(*.rtf)|*.rtf";
            if (saveFileDialog.ShowDialog() == true)
            {
                var fileName = saveFileDialog.FileName;
                SaveFileTo(fileName);
                MainViewModel.Instance.FileName = fileName;
                OriginWords = MainViewModel.Instance.CountWords(MainTextBox.Text);
                try
                {
                    File.Delete(Common.TempFile);
                }
                catch { }
            }
        }

        private string GetSaveCaption()
        {
            return string.Format("这次不计空格和符号共计码字{0}个，亲，一定记得要保存哦(*^_^*)",
                Words - OriginWords);
        }

        private void SaveFileTo(string fileName)
        {
            if (Regex.IsMatch(fileName, "\\.txt$", RegexOptions.IgnoreCase))
            {
                File.WriteAllText(fileName, this.MainTextBox.Text, Encoding.Default);
            }
            else if (Regex.IsMatch(fileName, "\\.rtf$", RegexOptions.IgnoreCase))
            {
                RtfHelper.Write(fileName, this.MainTextBox.Text);
            }
            OriginWords = Words;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitCommand();
        }

        #endregion

        #region 操作

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SearchCommand();
        }

        private void SearchWindow_Closed(object sender, EventArgs e)
        {
            _searchWindow = null;
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            ReplaceCommand();
        }

        private void ReplaceWindow_Closed(object sender, EventArgs e)
        {
            _replaceWindow = null;
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            SettingCommand();
        }

        #endregion

        private void AutoSaveCallback(object state)
        {
            this.MainTextBox.Dispatcher.Invoke(new Action(() =>
            {
                var text = this.MainTextBox.Text;
                if (string.IsNullOrEmpty(MainViewModel.Instance.FileName) && MainViewModel.Instance.CountWords(text) == 0)
                {
                    return;
                }
                File.WriteAllText(Common.TempFile, this.MainTextBox.Text, Encoding.Default);
            }));
        }

        #endregion

        #region 公共方法

        public void SetBackground(Brush brush)
        {
            this.Background = brush;
            this.MainTextBox.Background = brush;
            //this.MenuGrid.Background = brush;
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
            RefrenceCommand();
        }

        private void ReferenceWindow_Closed(object sender, EventArgs e)
        {
            _referenceWindow = null;
        }

        private void AutoTypeSetting_Click(object sender, RoutedEventArgs e)
        {
            AutoTypingSettingCommand();
        }

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainTextBox.Text.Length > 0)
            {
                Words = MainViewModel.Instance.CountWords(MainTextBox.Text);
                CharacterCountTextBlock.Text = string.Format("{0}\\{1}",
                    Words
                    , MainTextBox.Text.Length);
            }
            else
            {
                CharacterCountTextBlock.Text = string.Empty;
            }
        }

        //private void Operation_Click(object sender, RoutedEventArgs e)
        //{
        //    PowerCommand();
        //}

        private void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            SendEmailCommand();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutCommand();
        }

        private void MainTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (MainTextBox.SelectionLength > 0)
            {
                ImmediateTotalGroupBox.Visibility = Visibility.Visible;
                ImmediateWordsTextBlock.Text = MainViewModel.Instance.CountWords(MainTextBox.SelectedText).ToString();
                ImmediateLengthTextBlock.Text = MainTextBox.SelectionLength.ToString();
            }
            else
            {
                ImmediateTotalGroupBox.Visibility = Visibility.Collapsed;
            }
        }

        private void Total_Click(object sender, RoutedEventArgs e)
        {
            TotalCommand();
        }

        #region Command

        #region 文件操作

        public void NewCommand()
        {
            if (!(string.IsNullOrEmpty(MainViewModel.Instance.FileName) && MainViewModel.Instance.CountWords(MainTextBox.Text) == 0)
                 && MessageBox.Show(this, "当前文档还未保存，是否需要保存？", "询问", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
            {
                SaveCommand();
            }

            TypingSpeedViewModel.Instance.Total();
            this.MainTextBox.Text = string.Empty;
            //MenuPopup.IsOpen = false;
            MainViewModel.Instance.FileName = string.Empty;
            TypingSpeedViewModel.Instance.Refresh();
            OriginWords = 0;
            New();
            this.MainMenuSubPopup.IsOpen = false;
        }

        public void OpenCommand()
        {
            if (!(string.IsNullOrEmpty(MainViewModel.Instance.FileName) && MainViewModel.Instance.CountWords(MainTextBox.Text) == 0)
                 && MessageBox.Show(this, "当前文档还未保存，是否需要保存？", "询问", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
            {
                SaveCommand();
                this.MainMenuSubPopup.IsOpen = false;
            }

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文件(*.txt;*.rtf)|*.txt;*.rtf|文本文件(*.txt)|*.txt|RTF文件(*.rtf)|*.rtf";
            if (openFileDialog.ShowDialog() == true)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        public void SaveCommand()
        {
            try
            {
                if (string.IsNullOrEmpty(MainViewModel.Instance.FileName))
                {
                    SaveFileAs();
                    this.MainMenuSubPopup.IsOpen = false;
                }
                else
                {
                    SaveFileTo(MainViewModel.Instance.FileName);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("文件保存时出现异常：", ex.Message);
            }
        }

        public void SaveAsCommand()
        {
            try
            {
                SaveFileAs();
                this.MainMenuSubPopup.IsOpen = false;
            }
            catch (Exception ex)
            {
                ShowMessage("文件另存为时出现异常：", ex.Message);
            }
        }

        public void ExitCommand()
        {
            this.Close();
        }

        #endregion

        #region 操作

        public void SearchCommand()
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
                this.MainMenuSubPopup.IsOpen = false;
            }
        }

        public void ReplaceCommand()
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
                this.MainMenuSubPopup.IsOpen = false;
            }
        }

        public void SettingCommand()
        {
            var settingWindow = new SettingWindow();
            settingWindow.Owner = this;
            settingWindow.ShowDialog();
            SetFocus();
        }

        public void RefrenceCommand()
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

        #endregion

        #region 帮助

        public void SendEmailCommand()
        {
            var sendEmailWindow = new SendEmailWindow();
            sendEmailWindow.Owner = this;
            if (sendEmailWindow.ShowDialog() == true)
            {
                var result = EmailViewModel.Instance.Send(sendEmailWindow.SubjectTextBox.Text, this.MainTextBox.Text);
                if (result)
                {
                    ShowMessage("邮件发送成功");
                }
                else
                {
                    ShowMessage("邮件发送失败");
                }
            }
        }

        public void AutoTypingSettingCommand()
        {
            this.MainTextBox.Text =
                Regex.Replace(this.MainTextBox.Text,
                    @"([\s]*[\r\n]+[\s]*)|([\s]{5,})",
                    "\r\n\r\n　　").TrimStart('\r', '\n', ' ', '　').Insert(0, "　　");
            SetFocus(true);
            this.MainMenuSubPopup.IsOpen = false;
        }

        public void PowerCommand()
        {
            var powerWindow = new PowerWindow();
            powerWindow.Owner = this;
            powerWindow.ShowDialog();
        }

        public void TotalCommand()
        {
            if (null == _totalWindow)
            {
                _totalWindow = new TotalWindow();
                _totalWindow.Owner = this;
                _totalWindow.Closed += (o, e) => { _totalWindow = null; };
                _totalWindow.Show();
                this.MainMenuSubPopup.IsOpen = false;
            }
        }

        public void AboutCommand()
        {
            if (null == _abountWindow)
            {
                _abountWindow = new AbountWindow();
                _abountWindow.Owner = this;
                _abountWindow.Closed += (o, e) => { _abountWindow = null; };
                _abountWindow.Show();
                this.MainMenuSubPopup.IsOpen = false;
            }
        }

        #endregion


        #endregion

        private void OpenPopup()
        {
            this.MainMenuPopup.IsOpen = false;
            var top = (SystemParameters.PrimaryScreenHeight - MainMenuSubBorder.Height) / 2d;
            this.MainMenuSubPopup.VerticalOffset = top;
            MainMenuSubBorder.Width = SystemParameters.PrimaryScreenWidth - this.MainMenuSubPopup.HorizontalOffset;
            this.MainMenuSubPopup.IsOpen = true;
            _showMenuStoryboard.Begin();
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            
            OpenPopup();
            if (this.MainMenuSubPopup.DataContext != MenuViewModel.FileMenu)
            {
                this.MainMenuSubPopup.DataContext = MenuViewModel.FileMenu;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            OpenPopup();
            if (this.MainMenuSubPopup.DataContext != MenuViewModel.EditMenu)
            {
                this.MainMenuSubPopup.DataContext = MenuViewModel.EditMenu;
            }
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            OpenPopup();
            if (this.MainMenuSubPopup.DataContext != MenuViewModel.OperationMenu)
            {
                this.MainMenuSubPopup.DataContext = MenuViewModel.OperationMenu;
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            OpenPopup();
            if (this.MainMenuSubPopup.DataContext != MenuViewModel.HelpMenu)
            {
                this.MainMenuSubPopup.DataContext = MenuViewModel.HelpMenu;
            }
        }

        private void Lock_Click(object sender, RoutedEventArgs e)
        {
            this.MainMenuPopup.IsOpen = false;
            var top = (SystemParameters.PrimaryScreenHeight - MainMenuSubBorder.Height) / 2d;
            this.LockPopup.VerticalOffset = top;
            LockPopup.Width = SystemParameters.PrimaryScreenWidth - this.LockPopup.HorizontalOffset;
            this.LockPopup.IsOpen = true;
            _showLockStoryboard.Begin();
            LockWordsTextBox.Focus();
        }

        private void LockOK_Click(object sender, RoutedEventArgs e)
        {
            this.LockPopup.IsOpen = false;
        }

        private void LeftRectangle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            this.MainMenuPopup.IsOpen = true;
        }

        private void MainMenuBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            this.MainMenuPopup.IsOpen = false;
        }

        private void MainMenuPopup_Opened(object sender, EventArgs e)
        {
            MainTextBoxBorder.SetValue(MarginProperty, new Thickness(MainMenuBorder.ActualWidth, 0d, 0d, 0d));
        }

        private void MainMenuPopup_Closed(object sender, EventArgs e)
        {
            MainTextBoxBorder.ClearValue(MarginProperty);
        }
    }
}

