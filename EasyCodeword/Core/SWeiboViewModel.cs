using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;
using EasyCodeword.Utilities;
using EasyCodeword.Views;
using WeiboSDK.Sina;

namespace EasyCodeword.Core
{
    public class SWeiboViewModel : EntityObject
    {
        #region 变量

        private static SWeiboViewModel _instance = new SWeiboViewModel();

        private ILogger _logger = LoggerFactory.GetLogger(typeof(SWeiboViewModel).FullName);

        private const string ACCESS_TOKEN_URL = "https://api.weibo.com/oauth2/access_token";

        private const string AUTHORIZE_URL = "https://api.weibo.com/oauth2/authorize";

        private const string UNAUTHORIZED = "用户未授权";

        private readonly string _appKey = "858273299";

        private readonly string _appSecret = "3fae75eb725ca4e5a356352d73904536";

        private OAuth _oAuth = null;

        //private readonly string AppKey = "858273299";

        //private readonly string AppSecret = "3fae75eb725ca4e5a356352d73904536";

        //private string _tokenKey = null;

        //private string _tokenSecret = null;

        private string _accessKey = RWReg.GetValue(Constants.SubName, "SAccessKey", string.Empty).ToString();

        private string _accessSecret = RWReg.GetValue(Constants.SubName, "SAccessSecret", string.Empty).ToString();

        private string _nickname;

        #endregion

        #region 属性

        public static SWeiboViewModel Instance { get { return _instance; } }

        public bool IsAuthorized { get { return !string.IsNullOrEmpty(_accessKey) && !string.IsNullOrEmpty(_accessSecret); } }

        public bool HasChanged { get; private set; }

        /// <summary>
        /// 用户授权 Key
        /// </summary>
        public string AccessKey
        {
            get { return _accessKey; }
            set { _accessKey = value; }
        }

        /// <summary>
        /// 用户授权密钥
        /// </summary>
        public string AccessSecret
        {
            get { return _accessSecret; }
            set { _accessSecret = value; }
        }

        /// <summary>
        /// 微博昵称
        /// </summary>
        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; RaisePropertyChanged("Nickname"); }
        }

        #endregion

        private SWeiboViewModel()
        {
            _oAuth = new OAuth(_appKey, _appSecret);

            GetNickname();
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        private void GetNickname()
        {
            if (IsAuthorized)
            {
                _nickname = "加载中...";

                ThreadPool.QueueUserWorkItem(delegate
                {
                    GetNicknameCallback();
                });
            }
            else
            {
                _nickname = UNAUTHORIZED;
            }
        }

        private void GetNicknameCallback()
        {
            try
            {
                if (string.IsNullOrEmpty(_oAuth.AccessToken))
                {
                    if (!_oAuth.Login(_accessKey, _accessSecret))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Nickname = "获取昵称失败";
                            MainWindow.Instance.ShowMessage("获取新浪微博用户信息失败！");
                        }));
                        return;
                    }
                }
                var user = new User(_oAuth);
                var info = user.Info();
                var counts = user.Counts();
                var nameMatch = Regex.Match(info, @"""screen_name"":""(?<name>[^""]{0,32})""");
                var countsMath = Regex.Match(counts, @"""followers_count"":(?<fansnum>[0-9]+).*""friends_count"":(?<favnum>[0-9]+)");
                if (nameMatch.Success && countsMath.Success)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Nickname = string.Format("{0}[收听{1};听众:{2}]",
                            nameMatch.Groups["name"].Value,
                            countsMath.Groups["favnum"].Value,
                            countsMath.Groups["fansnum"].Value);
                    }));
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Nickname = "获取昵称失败";
                    MainWindow.Instance.ShowMessage("获取新浪微博用户信息失败！");
                }));
                _logger.Error("[GetNicknameCallback] : Exception ： {0}", ex.Message);
            }
        }

        private bool ParseToken(string response)
        {
            return true;
        }

        public void Save()
        {
            RWReg.SetValue(Constants.SubName, "SAccessKey", _accessKey);
            RWReg.SetValue(Constants.SubName, "SAccessSecret", _accessSecret);
        }

        public void Cancel()
        {
            _accessKey = RWReg.GetValue(Constants.SubName, "SAccessKey", string.Empty).ToString();
            _accessSecret = RWReg.GetValue(Constants.SubName, "SAccessSecret", string.Empty).ToString();
            RaisePropertyChanged("IsAuthorized");
            GetNickname();
        }

        public void Reset()
        {
            HasChanged = false;
        }

        /// <summary>
        /// 用户授权
        /// </summary>
        public void Authorize()
        {
            var inputAccountWindow = new InputAccoutWindow();
            inputAccountWindow.Owner = SettingWindow.Instance;
            inputAccountWindow.Title = "请输入新浪微博的账号和密码";
            if (inputAccountWindow.ShowDialog() == true)
            {
                var username = inputAccountWindow.UsernameTextBox.Text;
                var password = inputAccountWindow.PassowrdTextBox.Password;
                Login(username, password);
            }
        }

        private void Login(string username, string password)
        {
            //好吧，上一版的SDK在网络环境差的情况下经常会卡着不动，这次的我们来把操作界面线程和操作线程分开吧
            Thread thLogin = new Thread(new ThreadStart(delegate()
            {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    try
                    {
                        if (_oAuth.Login(username, password))
                        {
                            _accessKey = username;

                            _accessSecret = password;

                            UpdateHasChanged();

                            RaisePropertyChanged("IsAuthorized");

                            SettingViewModel.Instance.SaveCommand.RaiseCanExecuteChanged();

                            GetNickname();

                            MainWindow.Instance.ShowMessage("恭喜你的新浪微博已成功授权本应用！");

                        }
                        else
                        {
                            MainWindow.Instance.ShowMessage("获取新浪微博应用授权失败！");
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.Error("[Authorize] : 获取 token key 失败!");
                        MainWindow.Instance.ShowMessage("获取新浪微博应用授权出现异常：{0}", ex.Message);
                    }
                }));
            }));

            thLogin.Start();

        }

        private void UpdateHasChanged()
        {
            HasChanged = !string.Equals(RWReg.GetValue(Constants.SubName, "SAccessKey", string.Empty).ToString(),
                    _accessKey)
                || !string.Equals(RWReg.GetValue(Constants.SubName, "SAccessSecret", string.Empty).ToString(),
                    _accessSecret);
        }

        /// <summary>
        /// 取消用户授权
        /// </summary>
        internal void Deauthorize()
        {
            _accessKey = string.Empty;

            _accessSecret = string.Empty;

            UpdateHasChanged();

            RaisePropertyChanged("IsAuthorized");

            SettingViewModel.Instance.SaveCommand.RaiseCanExecuteChanged();

            Nickname = UNAUTHORIZED;
        }

        /// <summary>
        /// 发送一条文字微博
        /// </summary>
        /// <param name="weibo">微博信息</param>
        /// <returns></returns>
        public void Add(string weibo)
        {
            try
            {
                if (string.IsNullOrEmpty(_oAuth.AccessToken))
                {
                    if (!_oAuth.Login(_accessKey, _accessSecret))
                    {
                        _accessKey = string.Empty;
                        _accessSecret = string.Empty;
                    }
                }
                var t = new T(_oAuth);
                t.Update(string.Concat(weibo, "  ", SettingViewModel.Instance.TenderLockMessage));
            }
            catch (Exception ex)
            {
                _logger.Error("[Add] Exception : {0}" , ex.Message);
            }
        }
    }
}
