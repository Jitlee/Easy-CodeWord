using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasyCodeword.Core;

namespace EasyCodeword.Views
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public static SettingWindow Instance { get; private set; }
        public SettingWindow()
        {
            InitializeComponent();
            this.DataContext = SettingViewModel.Instance;
            Instance = this;
            this.HideButton();
            EmailPasswordTextBox.Password = EmailViewModel.Instance.EmailPassword;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingViewModel.Instance.Verify())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Digital_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            //屏蔽非法按键
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                || e.Key == Key.Decimal || e.Key.ToString() == "Tab")
            {
                if (txt.Text.Contains(".") && e.Key == Key.Decimal)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) 
                || e.Key == Key.OemPeriod) 
                && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                if (txt.Text.Contains(".") && e.Key == Key.OemPeriod)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else if (e.Key == Key.Back
                || e.Key == Key.Delete
                || e.Key == Key.Left
                || e.Key == Key.Right)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void MusicBrowser_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SettingViewModel.Instance.MusicFolder = dlg.SelectedPath;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (SettingViewModel.Instance.Verify())
            {
                if (this.DialogResult != true
                    && SettingViewModel.Instance.SaveCommand.CanExecute(null))
                {
                    var result = MessageBox.Show(this,
                            "设置已更改，是否需要保存？",
                            "询问",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.None,
                            MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                    {
                        SettingViewModel.Instance.SaveCommand.Execute(null);
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void EmailPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            EmailViewModel.Instance.EmailPassword = EmailPasswordTextBox.Password;
        }
    }
}
