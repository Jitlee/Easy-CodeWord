using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    public class EmailViewModel : EntityObject
    {
        #region 变量

        private static EmailViewModel _instance = new EmailViewModel();

        private ILogger _logger = LoggerFactory.GetLogger(typeof(EmailViewModel).FullName);

        private string _emailFrom = RWReg.GetValue(Constants.SubName, "EmailF", string.Empty).ToString();

        private string _emailPassword = RWReg.GetValue(Constants.SubName, "EmailP", string.Empty).ToString();

        private string _emailTo = RWReg.GetValue(Constants.SubName, "EmailTo", string.Empty).ToString();

        private string _smtp = RWReg.GetValue(Constants.SubName, "SMTP", string.Empty).ToString();

        private const string HOST = "smtp.163.com";
        private const string FROM = "icodeword@163.com";
        private const string ACCOUNT = "icodeword";
        private const string PASSWORD = "icOdewoRd8156";

        #endregion

        #region 属性

        public static EmailViewModel Instance { get { return _instance; } }

        public bool HasChanged { get; private set; }

        /// <summary>
        /// 邮件发件人地址
        /// </summary>
        public string EmailFrom
        {
            get { return _emailFrom; }
            set
            {
                _emailFrom = value.Trim(); RaisePropertyChanged("EmailFrom");
                var emailHost = EmailProvider.GetEmailHost(_emailFrom);
                if (null != emailHost)
                {
                    _smtp = emailHost.SMTP;
                    RaisePropertyChanged("SMTP");
                }
                UpdateHasChanged();
                SettingViewModel.Instance.SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 邮件发件人地址
        /// </summary>
        public string EmailPassword
        {
            get { return _emailPassword; }
            set
            {
                _emailPassword = value;
                UpdateHasChanged();
                SettingViewModel.Instance.SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 邮件收件人地址
        /// </summary>
        public string EmailTo
        {
            get { return _emailTo; }
            set
            {
                _emailTo = value.Trim(); RaisePropertyChanged("EmailTo");
                UpdateHasChanged();
                SettingViewModel.Instance.SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string SMTP
        {
            get { return _smtp; }
            set
            {
                _smtp = value.Trim(); RaisePropertyChanged("SMTP");
                UpdateHasChanged();
                SettingViewModel.Instance.SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region 构造方法

        private EmailViewModel()
        {

        }

        #endregion

        #region 共方法

        public void Save(bool saveAll = true)
        {
            RWReg.SetValue(Constants.SubName, "EmailF", _emailFrom);
            RWReg.SetValue(Constants.SubName, "EmailP", _emailPassword);
            RWReg.SetValue(Constants.SubName, "SMTP", _smtp);
            if (saveAll)
            {
                RWReg.SetValue(Constants.SubName, "EmailTo", _emailTo);
            }
        }

        public void Reset()
        {
            HasChanged = false;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        public bool Send(string subject, string content)
        {
            try
            {
                var emailHost = EmailProvider.GetEmailHost(_emailFrom);
                //var accountMatch = Regex.Match(_emailFrom, "^[_.-A-Za-z0-9]+(?=@)");
                if (null != emailHost
                    && !string.IsNullOrEmpty(emailHost.SMTP))
                {
                    var account = _emailFrom;
                    if (!emailHost.SameName)
                    {
                        account = Regex.Match(_emailFrom, "^[_.-A-Za-z0-9]+(?=@)").Value;
                    }
                    if (!string.IsNullOrEmpty(emailHost.SMTP))
                    {
                        var smtp = new SmtpClient();
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Host = emailHost.SMTP;
                        smtp.Credentials = new System.Net.NetworkCredential(account, _emailPassword);
                        if (emailHost.Port > 0)
                        {
                            smtp.Port = emailHost.Port;
                        }

                        var Mail = new MailMessage(_emailFrom, _emailTo);

                        Mail.Subject = subject;
                        Mail.Body = content;
                        Mail.BodyEncoding = System.Text.Encoding.UTF8;
                        Mail.IsBodyHtml = false;
                        Mail.Priority = MailPriority.Normal;
                        smtp.SendAsync(Mail, null);

                        Save();
                        Reset();
                        SettingViewModel.Instance.SaveCommand.RaiseCanExecuteChanged();
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("[Send] Exception : {0}", ex.Message);
                return true;
            }
            return false;
        }

        public string GetSMTP(string email)
        {
            var emailHost = EmailProvider.GetEmailHost(email);
            if (null != emailHost)
            {
                return emailHost.SMTP;
            }
            return null;
        }

        #endregion

        #region 私有方法

        private void UpdateHasChanged()
        {
            HasChanged = !string.Equals(RWReg.GetValue(Constants.SubName, "EmailF", string.Empty).ToString(),
                    _emailFrom)
                || !string.Equals(RWReg.GetValue(Constants.SubName, "EmailTo", string.Empty).ToString(),
                    _emailTo)
                || !string.Equals(RWReg.GetValue(Constants.SubName, "EmailP", string.Empty).ToString(),
                    _emailPassword)
                || !string.Equals(RWReg.GetValue(Constants.SubName, "SMTP", string.Empty).ToString(),
                    _smtp);
        }

        #endregion
    }

    class EmailHost
    {
        public string Server { get; set; }
        public string POP3 { get; set; }
        public string SMTP { get; set; }
        public string IMAP { get; set; }
        public int Port { get; set; }
        public bool SameName { get; set; } // 账号和邮箱同名
    }

    static class EmailProvider
    {

        static ILogger _logger = LoggerFactory.GetLogger(typeof(EmailProvider).FullName);

        static XElement _xElement = null;

        public static EmailHost GetEmailHost(string email)
        {
            try
            {
                var match = Regex.Match(email, "(?<=@)([A-Za-z0-9]+[.])+[a-zA-Z]+$");
                if (match.Success)
                {
                    var host = match.Value;
                    if (null == _xElement)
                    {
                        _xElement = XElement.Parse(Properties.Resources.email);
                    }
                    if (null != _xElement)
                    {
                        var emailHostElement = _xElement.XPathSelectElement(string.Format("EmailHost/Server[translate(., 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '{0}']", host));
                        if (null != emailHostElement)
                        {
                            emailHostElement = emailHostElement.Parent;
                            var emailHost = new EmailHost();
                            emailHost.Server = emailHostElement.ElementValue("Server");
                            emailHost.POP3 = emailHostElement.ElementValue("POP3");
                            emailHost.SMTP = emailHostElement.ElementValue("SMTP");
                            emailHost.IMAP = emailHostElement.ElementValue("IMAP");
                            emailHost.Port = Converter.ToInt(emailHostElement.ElementValue("Port"));
                            emailHost.SameName = Converter.ToInt(emailHostElement.ElementValue("SameName")) != 0;
                            return emailHost;
                        }
                    }
                    else
                    {
                        _logger.Debug("[GetEmailHost] XElement.Parse failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("[GetEmailHost] Exception : {0}", ex.Message);
            }
            return null;
        }

        private static string ElementValue(this XElement root, XName xName)
        {
            if (null != root.Element(xName))
            {
                return root.Element(xName).Value;
            }
            return string.Empty;
        }
    }
}
