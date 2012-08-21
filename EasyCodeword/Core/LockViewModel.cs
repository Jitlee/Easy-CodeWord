using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    public class LockViewModel : EntityObject
    {
        private static LockViewModel _instance = new LockViewModel();

        private Timer _timer;   // 自动解锁时钟

        private KeyboardHook _hook; // 锁屏钩子

        private DateTime _lockOriginTime= DateTime.Now;   //  锁定时的时间

        private int _lockOriginLength = 0;    // 锁定时字数。

        private bool _isUnlocked = true;     // 是否锁定, 用于 设置IsEnabled

        private string _lockWords;

        private string _lockMinutes;

        private int _surplusWords;

        private int _surplusMinutes;

        private readonly DelegateCommand _lockCommand;

        public static LockViewModel Insatance { get { return _instance; } }

        public bool IsLock
        {
            get
            {
                var lockWords = Converter.ToInt(_lockWords);
                var lockMinutes = Converter.ToInt(_lockMinutes);

                if ((lockMinutes < 1
                        && lockWords > 0
                        && lockWords > MainViewModel.Instance.CountWords(MainWindow.Instance.MainTextBox.Text, _lockOriginLength))
                    || (lockWords < 1
                        && lockMinutes > 0
                        && _lockOriginTime.AddMinutes(lockMinutes) > DateTime.Now)
                    || (lockMinutes > 0
                        && lockWords > 0
                        && lockWords > MainViewModel.Instance.CountWords(MainWindow.Instance.MainTextBox.Text, _lockOriginLength)
                        && _lockOriginTime.AddMinutes(lockMinutes) > DateTime.Now))
                {
                    return true;
                }

                UnLock();

                return false;
            }
        }

        public bool IsUnlocked
        {
            get { return _isUnlocked; }
            set { _isUnlocked = value; RaisePropertyChanged("IsUnlocked"); }
        }

        public string LockWords
        {
            get { return _lockWords; }
            set { _lockWords = value; RaisePropertyChanged("LockWords"); }
        }

        public string LockMinutes
        {
            get { return _lockMinutes; }
            set { _lockMinutes = value; RaisePropertyChanged("LockMinutes"); }
        }

        public int SurplusWords
        {
            get { return _surplusWords; }
            set { _surplusWords = value; RaisePropertyChanged("SurplusWords"); }
        }

        public int SurplusMinutes
        {
            get { return _surplusMinutes; }
            set { _surplusMinutes = value; RaisePropertyChanged("SurplusMinutes"); }
        }

        public DelegateCommand LockCommand { get { return _lockCommand; } }

        private LockViewModel()
        {
            _lockCommand = new DelegateCommand(Lock);
        }


        private bool LockCallback()
        {
            return IsLock;
        }

        private void AutoUnlock()
        {
            ReleaseTimer();
            _timer = new Timer(AutoUnlockCallback, null, 60000, 60000);
        }

        private void AutoUnlockCallback(object state)
        {
            var lockMinutes = Converter.ToInt(_lockMinutes);

            if (lockMinutes < 1
                || _lockOriginTime.AddMinutes(lockMinutes) > DateTime.Now)
            {
                ReleaseTimer();
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    UnLock();
                }));
            }
            else if (lockMinutes > 0)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                   {
                       SurplusMinutes = (int)(_lockOriginTime.AddMinutes(lockMinutes) - DateTime.Now).TotalMinutes;
                   }));
            }
        }

        private void ReleaseTimer()
        {
            if (null != _timer)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var lockWords = Converter.ToInt(_lockWords);
            var surplusWords = lockWords - MainViewModel.Instance.CountWords(MainWindow.Instance.MainTextBox.Text, _lockOriginLength);

            if (lockWords < 1
                 || surplusWords < 1)
            {
                ReleaseTimer();
                UnLock();
            }
            else if (surplusWords > 0)
            {
                SurplusWords = surplusWords;
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = IsLock;
        }

        private void UnLock()
        {
            LockWords = string.Empty;
            LockMinutes = string.Empty;

            App.Current.MainWindow.Topmost = false;
            MainWindow.Instance.Closing -= MainWindow_Closing;
            MainWindow.Instance.MainTextBox.TextChanged -= MainTextBox_TextChanged;
            IsUnlocked = true;
            if (null != _hook)
            {
                _hook.KeyMaskStop();
                _hook = null;
            }

            MainWindow.Instance.ShowMessage("锁定功能已解除。");
        }

        public void Lock()
        {
            var lockWords = Converter.ToInt(_lockWords);
            var lockMinutes = Converter.ToInt(_lockMinutes);
            if (lockWords > 0 || lockMinutes > 0)
            {
                _lockOriginLength = MainWindow.Instance.MainTextBox.Text.Length;
                _lockOriginTime = DateTime.Now;

                App.Current.MainWindow.Topmost = true;
                MainWindow.Instance.Closing -= MainWindow_Closing;
                MainWindow.Instance.Closing += MainWindow_Closing;

                IsUnlocked = false;
                if (_hook == null)
                {
                    _hook = new KeyboardHook();
                    _hook.KeyMaskStart(LockCallback);
                }

                if (lockMinutes > 0)
                {
                    SurplusMinutes = lockMinutes;
                    AutoUnlock();
                }

                if (lockWords > 0)
                {
                    SurplusWords = lockWords;
                    MainWindow.Instance.MainTextBox.TextChanged -= MainTextBox_TextChanged;
                    MainWindow.Instance.MainTextBox.TextChanged += MainTextBox_TextChanged;
                }
                if (lockMinutes > 0 && lockWords < 1)
                {
                    MainWindow.Instance.ShowMessage("已启用锁定功能：锁定 {0} 分钟。", lockMinutes);
                }
                else if (lockWords > 0 && lockMinutes < 1)
                {
                    MainWindow.Instance.ShowMessage("已启用锁定功能：锁定 {0} 字。", lockWords);
                }
                else
                {
                    MainWindow.Instance.ShowMessage("已启用锁定功能：锁定 {0} 字和 {1} 分钟。", lockWords, lockMinutes);
                }
            }
        }
    }
}
