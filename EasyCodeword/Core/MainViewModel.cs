using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace EasyCodeword.Core
{
    public class MainViewModel : EntityObject, IDisposable
    {
        #region 变量

        private static MainViewModel _instance = new MainViewModel();

        private readonly Timer _timer;

        private string _fileName = string.Empty;

        private bool _isDisposed;

        #endregion

        #region 属性

        public static MainViewModel Instance { get { return _instance; } }

        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; RaisePropertyChanged("FileName"); }
        }

        #endregion

        #region 构造函数

        private MainViewModel()
        {
            _timer = new Timer(TimerCallback, null, 0, 1000);
        }

        ~MainViewModel()
        {
            Dispose(false);
        }

        #endregion

        #region 私有方法

        private void TimerCallback(object state)
        {
            if (null != Application.Current)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { RaisePropertyChanged("Now"); }));
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 计算字数
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <param name="startIndex">开始索引</param>
        /// <returns></returns>
        public int CountWords(string text, int startIndex = 0)
        {

            if (startIndex > 0)
            {
                if (startIndex < text.Length)
                {
                    return Regex.Matches(text.Substring(startIndex), @"\w").Count;
                }
                return 0;
            }
            return Regex.Matches(text, @"\w").Count;
        }

        #endregion

        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _timer.Dispose();
                }

                // Release unmanaged resources

                _isDisposed = true;
            }
        }
    }
}
