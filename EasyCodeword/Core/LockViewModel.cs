﻿using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using EasyCodeword.Utilities;
using EasyCodeword.Views;

namespace EasyCodeword.Core
{
    public class LockViewModel : EntityObject, IDisposable
    {
        private static LockViewModel _instance = new LockViewModel();

        private bool _isDisposed;

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
            if (IsLock)
            {
                if (SettingViewModel.Instance.IsTenderLock)
                {
                    try
                    {
                        var addWeiboWindow = new AddWeiboWindow();
                        addWeiboWindow.Owner = MainWindow.Instance;
                        if (addWeiboWindow.ShowDialog() == true)
                        {
                            var weibo = addWeiboWindow.Weibo;
                            QWeiboViewModel.Instance.Add(weibo);
                            SWeiboViewModel.Instance.Add(weibo);
                            //MessageBox.Show(ret);
                            e.Cancel = false;
                            return;
                        }
                    }
                    catch
                    {
                        
                    }
                }
                e.Cancel = true;
            }
        }

        private void UnLock()
        {
            LockWords = string.Empty;
            LockMinutes = string.Empty;

            App.Current.MainWindow.Topmost = false;
            MainWindow.Instance.Closing -= MainWindow_Closing;
            MainWindow.Instance.MainTextBox.TextChanged -= MainTextBox_TextChanged;

            //RWReg.RemoveKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableChangePassword");
            //RWReg.RemoveKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableLockWorkstation");
            //RWReg.RemoveKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr");
            //RWReg.RemoveKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoLogoff");
            IsUnlocked = true;
            if (null != _hook)
            {
                _hook.KeyMaskStop();
                _hook = null;
            }

            MainWindow.Instance.ShowMessage("锁定功能已解除。");
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        internal void Lock()
        {
            // 软件尚未注册
            if (!Verify())
            {
                return;
            }

            var lockWords = Converter.ToInt(_lockWords);
            var lockMinutes = Converter.ToInt(_lockMinutes);
            if (lockWords > 0 || lockMinutes > 0)
            {
                _lockOriginLength = MainWindow.Instance.MainTextBox.Text.Length;
                _lockOriginTime = DateTime.Now;

                MainWindow.Instance.Closing -= MainWindow_Closing;
                MainWindow.Instance.Closing += MainWindow_Closing;

                IsUnlocked = false;

                if (SettingViewModel.Instance.IsViolenceLock)
                {
#if !DEBUG
                    App.Current.MainWindow.Topmost = true;
#endif
                    if (_hook == null)
                    {
                        _hook = new KeyboardHook();
                        _hook.KeyMaskStart(LockCallback);
                    }

                    //RWReg.SetValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableChangePassword", 1, true);
                    //RWReg.SetValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableLockWorkstation", 1, true);
                    //RWReg.SetValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", 1, true);
                    //RWReg.SetValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoLogoff", 1, true);
                }
                else if (SettingViewModel.Instance.IsTenderLock)
                {
                    if (null != _hook)
                    {
                        _hook.KeyMaskStop();
                        _hook = null;
                    }
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

        public bool Verify()
        {
            var lockWords = Converter.ToInt(_lockWords);
            var lockMinutes = Converter.ToInt(_lockMinutes);

            if ((lockWords > 0 ||
                lockMinutes > 0) &&
                !LicenseProvider.IsRegistered)
            {
                AlertWindow.ShowAlert("软件尚未注册，不能使用锁定功能，如需注册请从帮助窗口(F1键)点击注册注册按钮", "软件注册");
                return false;
            }
            return true;
        }

        public void Cancel()
        {
            LockWords = string.Empty;
            LockMinutes = string.Empty;
        }

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
                    if (null != _timer)
                    {
                        _timer.Dispose();
                    }
                }

                // Release unmanaged resources

                _isDisposed = true;
            }
        }

        ~LockViewModel()
        {
            Dispose(false);
        }
    }
}
