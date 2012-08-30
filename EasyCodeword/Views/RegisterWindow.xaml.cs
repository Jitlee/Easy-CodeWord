using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EasyCodeword.Core;
using EasyCodeword.Utilities;

namespace EasyCodeword.Views
{
    /// <summary>
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private ILogger _logger = LoggerFactory.GetLogger(typeof(RegisterWindow).FullName);
        public RegisterWindow()
        {
            InitializeComponent();
            this.HideIconAndButton();
            this.CodeTextBox.Text = LicenseProvider.Code;
            this.Loaded += RegisterWindow_Loaded;
            this.HelpTextBlock.Text = @"
注册方法：
    复制特征码，通过注册联系方式，交与软件开发者。付费成功后，软件开发者将会返回一组注册码，然后请将注册码粘贴到注册码编辑框中回车即可注册。";

            Init();
        }

        private void Init()
        {
            if (LicenseProvider.IsRegistered)
            {
                SerialNumberTextBox.Text = LicenseProvider.FormatSerialNumber(LicenseProvider.SerialNumber);
                SerialNumberTextBox.IsReadOnly = true;
                SerialNumberTextBox.MouseMove += CodeTextBox_MouseMove;
                RegisterButton.Visibility = Visibility.Collapsed;
                HelpTextBlock.Visibility = Visibility.Collapsed;
                return;
            }

            // 当应用程序没有以管理员权限运行时按钮显示盾牌
            if (!Common.IsAdmin())
            {
                // 由于在 WPF 中控件没有Handle，所以需要绘图实现
                // 先取出盾牌图标
                using (Icon icon = new Icon(SystemIcons.Shield, 12, 12))
                {
                    // 绘图
                    using (Bitmap bitmap = Bitmap.FromHicon(icon.Handle))
                    {
                        // 使之显示到界面
                        MemoryStream stream = new MemoryStream();
                        {
                            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            ImageBrush imageBrush = new ImageBrush();
                            ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                            this.UACImage.Visibility = Visibility.Visible;
                            this.UACImage.Source = (ImageSource)imageSourceConverter.ConvertFrom(stream);
                        }
                    }
                }
            }
        }

        private void RegisterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SerialNumberTextBox.Focus();
            CodeTextBox.SelectAll();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Register();
        }

        private void Register()
        {
            try
            {
                var serialNumber = this.SerialNumberTextBox.Text.Replace(" ", "").Replace("-", "");
                if (serialNumber.Length != 16)
                {
                    MainWindow.Instance.ShowMessage("请输入正确格式的序列号: XXXX-XXXX-XXXX-XXXX！");
                    return;
                }

                RWReg.SetValue(Constants.SubName, "Cache", serialNumber);
                ProcessStartInfo start = new ProcessStartInfo();
                start.WorkingDirectory = Environment.CurrentDirectory;
                start.FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                start.Verb = "runas";   // 这个动词将弹出 UAC 提示

                using (var p = Process.Start(start))
                {
                    p.WaitForExit();
                    if (LicenseProvider.Verify())
                    {
                        MainWindow.Instance.ShowMessage("恭喜你，注册成功！");
                        this.Close();
                    }
                    else
                    {
                        MainWindow.Instance.ShowMessage("注册失败！");
                    }
                }
            }
            catch (System.ComponentModel.Win32Exception win32Exception)
            {
                _logger.Trance("[Register] Win32Exception : {0}", win32Exception.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("[Register] Exception : {0}", ex.Message);
                MainWindow.Instance.ShowMessage("注册失败！");
            }
        }

        private void CodeTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.SelectionLength == 0)
            {
                textBox.SelectAll();
            }
        }
    }
}
