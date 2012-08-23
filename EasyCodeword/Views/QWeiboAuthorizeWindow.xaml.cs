using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

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
            this.HideButton();
            this.ContentRendered += QWeiboAuthorizeWindow_ContentRendered;
        }

        private void SettingWindow_Loaded(object sender, EventArgs e)
        {
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
                OauthVerifier = string.Empty;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
