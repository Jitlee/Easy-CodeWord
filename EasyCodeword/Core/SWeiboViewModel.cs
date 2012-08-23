﻿using System;
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
using WeiboSDK.Sina;

namespace EasyCodeword.Core
{
    public class SWeiboViewModel : EntityObject
    {
        #region 变量

        private static SWeiboViewModel _instance = new SWeiboViewModel();

        private ILogger _logger = LoggerFactory.GetLogger(typeof(SWeiboViewModel).FullName);

        private const string UNAUTHORIZED = "用户未授权";

        private readonly string AppKey = "3905834229";

        private readonly string AppSecret = "d6dacf5f0dc2d3c298edea2aac3a51a0";

        private string _tokenKey = null;

        private string _tokenSecret = null;

        private string _accessKey = RWReg.GetValue(SettingViewModel.SUB_NAME, "SAccessKey", string.Empty).ToString();

        private string _accessSecret = RWReg.GetValue(SettingViewModel.SUB_NAME, "SAccessSecret", string.Empty).ToString();

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
                //OauthKey oauthKey = new OauthKey();
                //oauthKey.customKey = AppKey;
                //oauthKey.customSecret = AppSecret;
                //oauthKey.tokenKey = _accessKey;
                //oauthKey.tokenSecret = _accessSecret;

                //user user = new user(oauthKey, "xml");

                //var info = user.info();
                //var xElment = XElement.Parse(info);
                //if (null != xElment)
                //{
                //    var nick = xElment.XPathSelectElement("//nick");
                //    var favnum = xElment.XPathSelectElement("//favnum");
                //    var fansnum = xElment.XPathSelectElement("//fansnum");
                //    if (null != nick &&
                //        null != favnum &&
                //        null != fansnum)
                //    {
                //        Application.Current.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Nickname = string.Format("{0}[收听{1};听众:{2}]", nick.Value, favnum.Value, fansnum.Value);
                //        }));
                //    }
                //}
                //else
                //{
                //    _logger.Debug("try parse xelement faild");
                //}
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
            SWeiboRequest request = new SWeiboRequest();
            return ParseToken(request.SyncRequest(url, "GET", parameters, null));
        }

        private bool GetRequestToken(string customKey, string customSecret)
        {
            try
            {
                string url = "https://api.weibo.com/oauth2/access_token";
                List<Parameter> parameters = new List<Parameter>();
                parameters.Add(new Parameter("client_id", customKey));
                parameters.Add(new Parameter("client_secret", customSecret));
                parameters.Add(new Parameter("grant_type", "password"));
                parameters.Add(new Parameter("username", "www.wpj@163.com"));
                parameters.Add(new Parameter("password", "a123456"));
                var request = new SWeiboRequest();
                return ParseToken(request.SyncRequest(url, "POST", parameters, null));
            }
            catch 
            {
                return false;
            }
        }

        private bool ParseToken(string response)
        {
            return true;
        }

        public void Save()
        {
            RWReg.SetValue(SettingViewModel.SUB_NAME, "SAccessKey", _accessKey);
            RWReg.SetValue(SettingViewModel.SUB_NAME, "SAccessSecret", _accessSecret);
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
                MainWindow.Instance.ShowMessage("获取新浪微博应用授权失败！");
                return;
            }

            _accessKey = _tokenKey;
            _accessSecret = _tokenSecret;

            UpdateHasChanged();

            RaisePropertyChanged("IsAuthorized");

            SettingViewModel.Instance.SaveCommand.RaiseCanExecuteChanged();

            GetNickname();

            MainWindow.Instance.ShowMessage("恭喜你的新浪微博已成功授权本应用！");
        }

        private void UpdateHasChanged()
        {
            HasChanged = !string.Equals(RWReg.GetValue(SettingViewModel.SUB_NAME, "SAccessKey", string.Empty).ToString(),
                    _accessKey)
                || !string.Equals(RWReg.GetValue(SettingViewModel.SUB_NAME, "SAccessSecret", string.Empty).ToString(),
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

            MainWindow.Instance.ShowMessage("你已成功取消新浪微博对本应用的授权！");
        }

        /// <summary>
        /// 发送一条文字微博
        /// </summary>
        /// <param name="weibo">微博信息</param>
        /// <returns></returns>
        public string Add(string weibo)
        {
            return null;
        }
    }
}