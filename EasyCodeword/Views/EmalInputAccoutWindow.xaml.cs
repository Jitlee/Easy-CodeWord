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
    /// EmailInputAccoutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EmailInputAccoutWindow : Window
    {
        public EmailInputAccoutWindow()
        {
            InitializeComponent();
            this.HideButton();
            this.Loaded += InputAccoutWindow_Loaded;
        }

        private void InputAccoutWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void UsernameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PassowrdTextBox.Focus();
                PassowrdTextBox.SelectAll();
                e.Handled = true;
            }
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var smtp = EmailViewModel.Instance.GetSMTP(UsernameTextBox.Text.Trim());
            if (!string.IsNullOrEmpty(smtp))
            {
                SMTPTextBox.Text = smtp;
            }
        }
    }
}
