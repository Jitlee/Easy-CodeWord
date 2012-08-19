using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace EasyCodeword.Core
{
    public class MainViewModel : EntityObject
    {
        #region 变量

        private static MainViewModel _instance = new MainViewModel();

        private readonly Timer _timer;

        private string _fileName = string.Empty;

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

        #endregion

        #region 私有方法

        private void TimerCallback(object state)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { RaisePropertyChanged("Now"); }));
        }

        #endregion
    }
}
