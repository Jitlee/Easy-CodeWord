using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace Keygen
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.CodeTextBox.Focus();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            IDataObject obj = Clipboard.GetDataObject();
            if (obj.GetDataPresent(DataFormats.Text))
            {
                var code = (obj.GetData(DataFormats.Text)).ToString();
                if (code.Length > 19)
                {
                    this.CodeTextBox.Text = code.Substring(0, 19);
                }
                else
                {
                    this.CodeTextBox.Text = code;
                }
            }
            this.CodeTextBox.Focus();
            this.CodeTextBox.SelectAll();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(this.SerialNumberTextBox.Text);
        }

        private void CodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CodeTextBox.Text.Length == 19)
            {
                try
                {
                    SerialNumberTextBox.Text = FormatSerialNumber(GetGenerateSerialNumber(CodeTextBox.Text));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, string.Format("生成序列号发生异常：{0}", ex.Message), "异常", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                SerialNumberTextBox.Text = string.Empty;
            }
        }

        private void SerialNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CopyButton.IsEnabled = SerialNumberTextBox.Text.Length > 0;
        }

        private string GetGenerateSerialNumber(string code)
        {
            return MD5(AES.Encrypt(code, "861D636D970F4E859354C90D9F7EABE2"));
        }

        private string FormatSerialNumber(string serialNumber)
        {
            serialNumber = serialNumber.Replace("-", "");
            if (serialNumber.Length == 16)
            {
                serialNumber = serialNumber.Insert(12, "-");
                serialNumber = serialNumber.Insert(8, "-");
                serialNumber = serialNumber.Insert(4, "-");
            }
            return serialNumber;
        }

        // MD5 加密数据
        /// <summary>
        /// MD5 加密数据
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        private static string MD5(string plainText)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(plainText)), 4, 8);
                t2 = t2.Replace("-", "");
                return t2;
            }
        }
    }
}
