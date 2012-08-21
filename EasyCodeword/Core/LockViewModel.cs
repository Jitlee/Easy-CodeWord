using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    public class LockViewModel : EntityObject
    {
        private static LockViewModel _instance = new LockViewModel();

        private KeyboardHook _hook; // 锁屏钩子

        private DateTime _lockOriginTime= DateTime.Now;   //  锁定时的时间

        private int _lockOriginLength = 0;    // 锁定时字数。

        private bool _isViolenceLock = false;

        private bool _isTenderLock = false;

        private string _lockLength;

        private string _lockMinute;

        private readonly DelegateCommand _lockLengthCommand;

        private readonly DelegateCommand _lockMinuteCommand;

        public static LockViewModel Insatance { get { return _instance; } }

        public bool IsLock
        {
            get
            {
                var lockLength = Converter.ToInt(_lockLength);
                var lockMinute = Converter.ToInt(_lockMinute);

                if (_lockOriginTime.AddMinutes(lockMinute) < DateTime.Now
                    || lockLength < MainViewModel.Instance.CountWords(MainWindow.Instance.MainTextBox.Text, _lockOriginLength))
                {
                    return true;
                }

                UnLock();

                return false;
            }
        }

        /// <summary>
        /// 暴力锁
        /// </summary>
        public bool IsViolenceLock
        {
            get { return _isViolenceLock; }
            set { _isViolenceLock = value; RaisePropertyChanged("IsViolenceLock"); }
        }

        /// <summary>
        /// 温柔锁
        /// </summary>
        public bool IsTenderLock
        {
            get { return _isTenderLock; }
            set { _isTenderLock = value; RaisePropertyChanged("IsTenderLock"); }
        }

        public string LockLength
        {
            get { return _lockLength; }
            set { _lockLength = value; RaisePropertyChanged("LockLength"); }
        }

        public string LockMinute
        {
            get { return _lockMinute; }
            set { _lockMinute = value; RaisePropertyChanged("LockMinute"); }
        }

        public DelegateCommand LockLengthCommand { get { return _lockLengthCommand; } }

        public DelegateCommand LockMinuteCommand { get { return _lockMinuteCommand; } }

        private LockViewModel()
        {
            _lockLengthCommand = new DelegateCommand(LockLengthHandler);
            _lockMinuteCommand = new DelegateCommand(LockMinuteHandler);
        }

        ~LockViewModel()
        {
            if(null != _hook)
            {
                _hook.KeyMaskStop();
            }
        }

        //private void Current_Deactivated(object sender, EventArgs e)
        //{
        //    if (!App.Current.MainWindow.IsActive)
        //    {
        //        App.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
        //        //App.Current.MainWindow.Activate();
        //        App.Current.MainWindow.Focus();
        //    }
        //}

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !IsLock;
        }

        private void Lock()
        {
            //App.Current.Deactivated -= Current_Deactivated;
            //App.Current.Deactivated -= Current_Deactivated;
            //App.Current.Deactivated += Current_Deactivated;
            //App.Current.MainWindow.Topmost = true;
            MainWindow.Instance.Closing -= MainWindow_Closing;
            MainWindow.Instance.Closing -= MainWindow_Closing;
            MainWindow.Instance.Closing += MainWindow_Closing;
            _hook = new KeyboardHook();
            _hook.KeyMaskStart(LockCallback);
        }

        private void UnLock()
        {
            //App.Current.Deactivated -= Current_Deactivated;
            //App.Current.Deactivated -= Current_Deactivated;
            //App.Current.Deactivated += Current_Deactivated;
            //App.Current.MainWindow.Topmost = false;
            MainWindow.Instance.Closing -= MainWindow_Closing;
            MainWindow.Instance.Closing -= MainWindow_Closing;
            if (null != _hook)
            {
                _hook.KeyMaskStop();
                _hook = null;
            }
        }

        private bool LockCallback()
        {
            return IsLock;
        }

        public void LockLengthHandler()
        {
            var lockLength = Converter.ToInt(_lockLength);
            if (lockLength > 0)
            {
                _lockOriginLength = MainWindow.Instance.MainTextBox.MaxLength;
                Lock();
            }
        }

        public void LockMinuteHandler()
        {
            var lockMinute = Converter.ToInt(_lockMinute);
            if (lockMinute > 0)
            {
                _lockOriginTime = DateTime.Now;
                Lock();
            }
        }
    }
}
