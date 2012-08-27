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
using EasyCodeword.Core;

namespace EasyCodeword.Views
{
    /// <summary>
    /// SendEmailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SendEmailWindow : Window
    {
        public SendEmailWindow()
        {
            InitializeComponent();
            this.HideIconAndButton();
            this.Loaded += InputAccoutWindow_Loaded;
            this.DataContext = EmailViewModel.Instance;
        }

        private void InputAccoutWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var from = EmailViewModel.Instance.EmailFrom;
            var to = EmailViewModel.Instance.EmailTo;

            if (string.IsNullOrEmpty(from))
            {
                FromTextBox.Focus();
            }
            else if (string.IsNullOrEmpty(to))
            {
                ToTextBox.Focus();
            }
            else
            {
                SubjectTextBox.Focus();
            }
        }

        private void FromTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var from = FromTextBox.Text.Trim();
            var to = ToTextBox.Text.Trim();
            var subject = SubjectTextBox.Text.Trim();
            OKButton.IsEnabled = !string.IsNullOrEmpty(from)
                && !string.IsNullOrEmpty(to)
                && !string.IsNullOrEmpty(subject);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void ChangeFrom_Click(object sender, RoutedEventArgs e)
        {
            var inputAccoutWindow = new EmailInputAccoutWindow();
            inputAccoutWindow.Owner = this;
            inputAccoutWindow.Title = "输入发件人信息";
            inputAccoutWindow.UsernameTextBlock.Text = "发送人邮箱地址：";
            inputAccoutWindow.PasswordTextBlock.Text = "发送人邮箱密码：";
            if (inputAccoutWindow.ShowDialog() == true)
            {
                EmailViewModel.Instance.EmailFrom = inputAccoutWindow.UsernameTextBox.Text;
                EmailViewModel.Instance.EmailPassword = inputAccoutWindow.PassowrdTextBox.Password;
                EmailViewModel.Instance.Save(false);
            }
        }
    }
}
