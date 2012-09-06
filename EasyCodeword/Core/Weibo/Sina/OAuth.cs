using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using EasyCodeword.Utilities;

namespace WeiboSDK.Sina
{
    public class OAuth
    {
        private ILogger _logger = LoggerFactory.GetLogger(typeof(OAuth).FullName);
        /// <summary>
        /// 获取App Key
        /// </summary>
        public string AppKey
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取App Secret
        /// </summary>
        public string AppSecret
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Access Token
        /// </summary>
        public string AccessToken
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取 Uid
        /// </summary>
        public string Uid
        {
            get;
            private set;
        }

        public OAuth(string appKey, string appSecret)
        {
            AppKey = appKey;
            AppSecret = appSecret;
        }

        public bool Login(string username, string password)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            var cookieContainer = new CookieContainer();
            var postData = GetPostData(username, password);
            HttpWebRequest webRequest = WebRequest.Create(Api.AUTHORIZE_URL) as HttpWebRequest;
            webRequest.Referer = GetAuthorizeURL();
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.AllowAutoRedirect = true;
            webRequest.KeepAlive = true;
            webRequest.CookieContainer = cookieContainer;
            webRequest.ContentLength = postData.Length;

            try
            {
                using (var request = webRequest.GetRequestStream())
                {
                    request.Write(postData, 0, postData.Length);
                }
                using (var response = webRequest.GetResponse())
                {
                    if (response != null)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var html = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(html))
                            {
                                var match = Regex.Match(html, @"""access_token"":""(?<token>.{0,32})"".*""uid"":""(?<uid>.{0,32})""");
                                if (match.Success)
                                {
                                    AccessToken = match.Groups["token"].Value;
                                    Uid = match.Groups["uid"].Value;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (WebException webException)
            {
                _logger.Trance("[Login] WebException : {0}", webException.Message);
            }
            return false;
        }

        //private bool VerifierAccessToken()
        //{

        //}

        private string GetAuthorizeURL()
        {
            var parameters = new List<Parameter>();
            parameters.Add(new Parameter("client_id", AppKey));
            parameters.Add(new Parameter("redirect_uri", ""));
            parameters.Add(new Parameter("response_type", "code"));
            parameters.Add(new Parameter("state", ""));
            parameters.Add(new Parameter("display", "default"));
            UriBuilder builder = new UriBuilder(Api.AUTHORIZE_URL);
            builder.Query = HttpUtil.FormatQueryString(parameters);
            return builder.ToString();
        }

        private byte[] GetPostData(string account, string password)
        {
            var parameters = new List<Parameter>();
            parameters.Add(new Parameter("action", "submit"));
            parameters.Add(new Parameter("withOfficalFlag", "0"));
            parameters.Add(new Parameter("ticket", ""));
            parameters.Add(new Parameter("isLoginSina", ""));
            parameters.Add(new Parameter("response_type", "token"));
            parameters.Add(new Parameter("regCallback", ""));
            parameters.Add(new Parameter("redirect_uri", ""));
            parameters.Add(new Parameter("client_id", AppKey));
            parameters.Add(new Parameter("state", ""));
            parameters.Add(new Parameter("from", ""));
            parameters.Add(new Parameter("userId", account));
            parameters.Add(new Parameter("passwd", password));
            parameters.Add(new Parameter("display", "js"));
            var queryString = HttpUtil.FormatQueryString(parameters);
            return Encoding.Default.GetBytes(queryString);
        }
    }
}
