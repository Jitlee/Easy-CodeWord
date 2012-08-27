using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;
using EasyCodeword.Utilities;
using EasyCodeword.Views;
using WeiboSDK;
using WeiboSDK.QQ;

namespace EasyCodeword.Core
{
    public class QWeiboViewModel : EntityObject
    {
        #region 变量

        private static QWeiboViewModel _instance = new QWeiboViewModel();

        private ILogger _logger = LoggerFactory.GetLogger(typeof(QWeiboViewModel).FullName);

        private const string UNAUTHORIZED = "用户未授权";

        private readonly string AppKey = "801222750";

        private readonly string AppSecret = "73779ac0cee5631fad2fcdad172baaab";

        private string _tokenKey = null;

        private string _tokenSecret = null;

        private string _accessKey = RWReg.GetValue(SettingViewModel.SUB_NAME, "QAccessKey", string.Empty).ToString();

        private string _accessSecret = RWReg.GetValue(SettingViewModel.SUB_NAME, "QAccessSecret", string.Empty).ToString();

        private string _nickname;

        #endregion

        #region 属性

        public static QWeiboViewModel Instance { get { return _instance; } }

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

        private QWeiboViewModel()
        {
            if (IsAuthorized)
            {
                _nickname = "加载中...";
                GetNickname();
            }
            else
            {
                _nickname = UNAUTHORIZED;
            }
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        private void GetNickname()
        {
            Thread thread = new Thread(GetNicknameCallback);
            thread.IsBackground = true;
            thread.Start();
        }

        private void GetNicknameCallback()
        {
            try
            {
                OauthKey oauthKey = new OauthKey();
                oauthKey.customKey = AppKey;
                oauthKey.customSecret = AppSecret;
                oauthKey.tokenKey = _accessKey;
                oauthKey.tokenSecret = _accessSecret;

                user user = new user(oauthKey, "xml");

                var info = user.info();
                var xElment = XElement.Parse(info);
                if (null != xElment)
                {
                    var nick = xElment.XPathSelectElement("//nick");
                    var favnum = xElment.XPathSelectElement("//favnum");
                    var fansnum = xElment.XPathSelectElement("//fansnum");
                    if (null != nick &&
                        null != favnum &&
                        null != fansnum)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Nickname = string.Format("{0}[收听{1};听众:{2}]", nick.Value, favnum.Value, fansnum.Value);
                        }));
                    }
                }
                else
                {
                    _logger.Debug("try parse xelement faild");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("[GetNicknameCallback] : Exception ： {0}", ex.Message);
            }
        }

        private bool GetAccessToken(string customKey, string customSecret, string requestToken, string requestTokenSecrect, string verify)
        {
            string url = "https://open.t.qq.com/cgi-bin/access_token";
            List<Parameter> parameters = new List<Parameter>();
            OauthKey oauthKey = new OauthKey();
            oauthKey.customKey = customKey;
            oauthKey.customSecret = customSecret;
            oauthKey.tokenKey = requestToken;
            oauthKey.tokenSecret = requestTokenSecrect;
            oauthKey.verify = verify;

            QWeiboRequest request = new QWeiboRequest();
            return ParseToken(request.SyncRequest(url, "GET", oauthKey, parameters, null));
        }

        private bool GetRequestToken(string customKey, string customSecret)
        {
            string url = "https://open.t.qq.com/cgi-bin/request_token";
            List<Parameter> parameters = new List<Parameter>();
            OauthKey oauthKey = new OauthKey();
            oauthKey.customKey = customKey;
            oauthKey.customSecret = customSecret;
            oauthKey.callbackUrl = "http://www.qq.com";

            QWeiboRequest request = new QWeiboRequest();
            return ParseToken(request.SyncRequest(url, "GET", oauthKey, parameters, null));
        }

        private bool ParseToken(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            string[] tokenArray = response.Split('&');

            if (tokenArray.Length < 2)
            {
                return false;
            }

            string strTokenKey = tokenArray[0];
            string strTokenSecrect = tokenArray[1];

            string[] token1 = strTokenKey.Split('=');
            if (token1.Length < 2)
            {
                return false;
            }
            _tokenKey = token1[1];

            string[] token2 = strTokenSecrect.Split('=');
            if (token2.Length < 2)
            {
                return false;
            }
            _tokenSecret = token2[1];

            return true;
        }

        public void Save()
        {
            RWReg.SetValue(SettingViewModel.SUB_NAME, "QAccessKey", _accessKey);
            RWReg.SetValue(SettingViewModel.SUB_NAME, "QAccessSecret", _accessSecret);
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
            if (GetRequestToken(AppKey, AppSecret) == false)
            {
                _logger.Error("[Authorize] : 获取 token key 失败!");
                return;
            }

            var qWeiboAuthorizeWindow = new QWeiboAuthorizeWindow("http://open.t.qq.com/cgi-bin/authorize?oauth_token=" + _tokenKey);

            qWeiboAuthorizeWindow.Owner = SettingWindow.Instance;
            if (qWeiboAuthorizeWindow.ShowDialog() == true)
            {
                var oauthVerify = qWeiboAuthorizeWindow.OauthVerifier;
                if (string.IsNullOrEmpty(oauthVerify))
                {
                    MainWindow.Instance.ShowMessage("你选择拒绝授权！");
                }
                else
                {
                    if (GetAccessToken(AppKey, AppSecret, _tokenKey, _tokenSecret, oauthVerify) == false)
                    {
                        _logger.Error("[Authorize] : 获取 acesskey 失败!");
                        return;
                    }

                    _accessKey = _tokenKey;
                    _accessSecret = _tokenSecret;

                    UpdateHasChanged();

                    RaisePropertyChanged("IsAuthorized");

                    SettingViewModel.Instance.SaveCommand.RaiseCanExecuteChanged();

                    GetNickname();

                    MainWindow.Instance.ShowMessage("恭喜你的腾讯微博已成功授权本应用！");
                }
            }
        }

        private void UpdateHasChanged()
        {
            HasChanged = !string.Equals(RWReg.GetValue(SettingViewModel.SUB_NAME, "QAccessKey", string.Empty).ToString(),
                    _accessKey)
                || !string.Equals(RWReg.GetValue(SettingViewModel.SUB_NAME, "QAccessSecret", string.Empty).ToString(),
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

            MainWindow.Instance.ShowMessage("你已成功取消腾讯微博对本应用的授权！");
        }

        /// <summary>
        /// 发送一条文字微博
        /// </summary>
        /// <param name="weibo">微博信息</param>
        /// <returns></returns>
        public string Add(string weibo)
        {
            try
            {
                OauthKey oauthKey = new OauthKey();
                oauthKey.customKey = AppKey;
                oauthKey.customSecret = AppSecret;
                oauthKey.tokenKey = _accessKey;
                oauthKey.tokenSecret = _accessSecret;

                t twit = new t(oauthKey, "json");

                var ret = twit.add(string.Concat(weibo, "  ", SettingViewModel.Instance.TenderLockMessage),
                              "",
                              "",
                              "");
                return ret;
            }
            catch (Exception ex)
            {
                _logger.Error("[Add] Exception : {0}", ex.Message);
            }
            return string.Empty;
        }
    }
}
