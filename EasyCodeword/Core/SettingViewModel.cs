using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    public class SettingViewModel : EntityObject
    {
        #region 变量

        static SettingViewModel _instance = new SettingViewModel();

        const string SUB_NAME = "Software\\EasyCodeword";

        const string BOOT_NAME = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private bool _boot;

        private SolidColorBrush _foreground = Converter.ToBrush(RWReg.GetValue(SUB_NAME, "Foreground", "#FF00FF00").ToString());

        private SolidColorBrush _background = Converter.ToBrush(RWReg.GetValue(SUB_NAME, "Background", "#FF000000").ToString());

        private readonly DelegateCommand _saveCommand;

        #endregion

        #region 属性

        public static SettingViewModel Instance { get { return _instance; } }

        public bool Boot
        {
            get
            {
                return _boot;
            }
            set
            {
                _boot = value;
                RaisePropertyChanged("Boot");
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public SolidColorBrush Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                _foreground = value;
                RaisePropertyChanged("Foreground");
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public SolidColorBrush Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                RaisePropertyChanged("Background");
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand { get { return _saveCommand; } }

        #endregion

        #region 构造方法

        private SettingViewModel()
        {
            var boot = (string)RWReg.GetValue(BOOT_NAME, "EasyCodeword", string.Empty);
            _boot = !string.IsNullOrEmpty(boot) &&
                string.Compare(System.Windows.Forms.Application.ExecutablePath,
                    boot, true) == 0;

            _saveCommand = new DelegateCommand(Save, CanSave);
        }

        #endregion

        #region 私有方法

        private void Save()
        {
            if (_boot)
            {
                RWReg.SetValue(BOOT_NAME, "EasyCodeword", System.Windows.Forms.Application.ExecutablePath);
            }
            else
            {
                RWReg.RemoveKey(BOOT_NAME, "EasyCodeword");
            }


            RWReg.SetValue(SUB_NAME, "Foreground", Converter.ToArgbString(_foreground));

            RWReg.SetValue(SUB_NAME, "Background", Converter.ToArgbString(_background));

        }

        private bool CanSave()
        {
            return HasBootChanged() || HashForegroundChanged() || HasBackgroundChanged();
        }

        private bool HasBootChanged()
        {
            var boot = (string)RWReg.GetValue(BOOT_NAME, "EasyCodeword", string.Empty);
            return (!string.IsNullOrEmpty(boot) &&
                string.Compare(System.Windows.Forms.Application.ExecutablePath,
                    boot, true) == 0) != _boot;
        }

        private bool HashForegroundChanged()
        {
            return string.Equals(
                RWReg.GetValue(SUB_NAME, "Foreground", "#FF00FF00").ToString(),
                Converter.ToArgbString(_foreground),
                StringComparison.CurrentCultureIgnoreCase);
        }

        private bool HasBackgroundChanged()
        {
            return string.Equals(
                RWReg.GetValue(SUB_NAME, "Background", "#FF000000").ToString(),
                Converter.ToArgbString(_background),
                StringComparison.CurrentCultureIgnoreCase);
        }

        //private bool HasAllowControlChanged()
        //{
        //    var allowControl = Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowControl", 1)) != 0;
        //    return allowControl != _allowControl;
        //}

        //private bool HasAllowBroadcastChanged()
        //{
        //    var allowBroadcast = Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowBroadcast", 1)) != 0;
        //    return _allowBroadcast != allowBroadcast;
        //}

        //private bool HasServerAddressChanged()
        //{
        //    var serverAddress = (string)RWReg.GetValue(SUB_NAME, "Server_Address", string.Empty);
        //    return string.Compare(serverAddress, _serverAddress, true) != 0;
        //}

        #endregion

    }
}
