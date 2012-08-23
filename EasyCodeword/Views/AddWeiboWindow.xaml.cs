using System;
using System.Windows;

namespace EasyCodeword.Views
{
    /// <summary>
    /// AddWeiboWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddWeiboWindow : Window
    {
        public string Weibo { get { return WeiboTextBox.Text; } }

        public AddWeiboWindow()
        {
            InitializeComponent();
            this.HideButton();
            this.Loaded += SettingWindow_Loaded;
        }

        private void SettingWindow_Loaded(object sender, EventArgs e)
        {
            WeiboTextBox.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
