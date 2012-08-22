using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EasyCodeword.Utilities;

namespace EasyCodeword.Views
{
    /// <summary>
    /// QWeiboAuthorizeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QWeiboAuthorizeWindow : Window
    {
        public string OauthVerifier { get; private set; }
        private readonly string _url;
        public QWeiboAuthorizeWindow(string url)
        {
            InitializeComponent();
            _url = url;
            this.ContentRendered += QWeiboAuthorizeWindow_ContentRendered;
            this.Loaded += SettingWindow_Loaded;
        }

        private void SettingWindow_Loaded(object sender, EventArgs e)
        {
            var hWnd = new WindowInteropHelper(this).Handle;
            Common.DisableMinmize(hWnd);
            AuthorizeWebBrower.Focus();
        }

        private void QWeiboAuthorizeWindow_ContentRendered(object sender, EventArgs e)
        {
            Thread thread = new Thread(NavigateCallback);
            thread.IsBackground = true;
            thread.Start();
        }

        private void NavigateCallback()
        {
            Thread.Sleep(200);
            this.Dispatcher.Invoke(new Action(() =>
            {
                AuthorizeWebBrower.Navigate(new Uri(_url));
            }));
        }

        private void AuthorizeWebBrower_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var url = e.Uri.AbsoluteUri;

            var match = Regex.Match(url, @"(?<=(oauth_verifier\=))[0-9]+");
            if(match.Success)
            {
                // 授权成功
                OauthVerifier = match.Value;
                this.DialogResult = true;
                this.Close();
            }
            else if ((match = Regex.Match(url, "&checkType=error")).Success)
            {
                // 决绝授权
                this.Close();
            }
        }
    }
}
