using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using EasyCodeword.Utilities;
using System.Windows;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace EasyCodeword.Core
{
    public class SettingViewModel : EntityObject
    {
        #region 静态

        public class _FontFamily
        {
            static XmlLanguage _enus = XmlLanguage.GetLanguage("en-US");
            static XmlLanguage _zhcn = XmlLanguage.GetLanguage("zh-CN");
            static string _fontName = string.Empty;

            public string DisplayName { get; set; }
            public FontFamily FontFamily { get; set; }

            public _FontFamily() { }

            public _FontFamily(FontFamily fontFamily)
                : this()
            {
                if (fontFamily.FamilyNames.ContainsKey(_enus))
                {
                    fontFamily.FamilyNames.TryGetValue(_enus, out _fontName);
                }
                if (fontFamily.FamilyNames.ContainsKey(_zhcn))
                {
                    fontFamily.FamilyNames.TryGetValue(_zhcn, out _fontName);
                }
                DisplayName = _fontName;
                FontFamily = fontFamily;
            }

            public override string ToString()
            {
                return DisplayName;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is _FontFamily)
                {
                    return (obj as _FontFamily).FontFamily.Equals(FontFamily);
                }
                return base.Equals(obj);
            }
        }

        private static IEnumerable<_FontFamily> _fontFamilies = GetFontFamilies();

        private static IEnumerable<_FontFamily> GetFontFamilies()
        {
            foreach (var fontFamily in System.Windows.Media.Fonts.SystemFontFamilies)
            {
                yield return new _FontFamily(fontFamily);
            }
        }

        public class _FontStyle
        {
            public int ID { get; set; }
            public string DisplayName { get; set; }
            public FontStyle FontStyle { get; set; }
            public FontWeight FontWeight { get; set; }
            public override string ToString()
            {
                return DisplayName;
            }

            public _FontStyle() { }

            public _FontStyle(int style)
                :this()
            {
                switch (style)
                {
                    case 1:
                        ID = 1;
                        DisplayName = "斜体";
                        FontStyle = System.Windows.FontStyles.Italic;
                        FontWeight = FontWeights.Normal;
                        break;
                    case 2:
                        ID = 2;
                        DisplayName = "粗体";
                        FontStyle = System.Windows.FontStyles.Normal;
                        FontWeight = FontWeights.Bold;
                        break;
                    case 3:
                        ID = 3;
                        DisplayName = "粗体 斜体";
                        FontStyle = System.Windows.FontStyles.Italic;
                        FontWeight = FontWeights.Bold;
                        break;
                    default:
                        ID = 0;
                        DisplayName = "常规";
                        FontStyle = System.Windows.FontStyles.Normal;
                        FontWeight = FontWeights.Normal;
                        break;
                }
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is _FontStyle)
                {
                    return (obj as _FontStyle).ID.Equals(ID);
                }
                return base.Equals(obj);
            }
        }

        private readonly static IEnumerable<_FontStyle> _fontStyles = GetFontStyles();

        private static IEnumerable<_FontStyle> GetFontStyles()
        {
            yield return new _FontStyle(0);
            yield return new _FontStyle(1);
            yield return new _FontStyle(2);
            yield return new _FontStyle(3);
        }

        public class _FontSize
        {
            public string DisplayName { get; set; }
            public double FontSize { get; set; }
            public override string ToString()
            {
                return DisplayName;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is _FontSize)
                {
                    return (obj as _FontSize).FontSize.Equals(FontSize);
                }
                return base.Equals(obj);
            }
        }

        private readonly static IEnumerable<_FontSize> _fontSizes = GetFontSizes();

        private static IEnumerable<_FontSize> GetFontSizes()
        {
            yield return new _FontSize() { DisplayName = "8", FontSize = 8d };
            yield return new _FontSize() { DisplayName = "9", FontSize = 9d };
            yield return new _FontSize() { DisplayName = "10", FontSize = 10d };
            yield return new _FontSize() { DisplayName = "11", FontSize = 11d };
            yield return new _FontSize() { DisplayName = "12", FontSize = 12d };
            yield return new _FontSize() { DisplayName = "14", FontSize = 14d };
            yield return new _FontSize() { DisplayName = "16", FontSize = 16d };
            yield return new _FontSize() { DisplayName = "18", FontSize = 18d };
            yield return new _FontSize() { DisplayName = "20", FontSize = 20d };
            yield return new _FontSize() { DisplayName = "22", FontSize = 22d };
            yield return new _FontSize() { DisplayName = "24", FontSize = 24d };
            yield return new _FontSize() { DisplayName = "26", FontSize = 26d };
            yield return new _FontSize() { DisplayName = "28", FontSize = 28d };
            yield return new _FontSize() { DisplayName = "36", FontSize = 36d };
            yield return new _FontSize() { DisplayName = "48", FontSize = 48d };
            yield return new _FontSize() { DisplayName = "72", FontSize = 72d };
            yield return new _FontSize() { DisplayName = "初号", FontSize = 56d };
            yield return new _FontSize() { DisplayName = "小初", FontSize = 48d };
            yield return new _FontSize() { DisplayName = "一号", FontSize = 34.67d };
            yield return new _FontSize() { DisplayName = "小一", FontSize = 32d };
            yield return new _FontSize() { DisplayName = "二号", FontSize = 29.3d };
            yield return new _FontSize() { DisplayName = "小二", FontSize = 24d };
            yield return new _FontSize() { DisplayName = "三号", FontSize = 21.3d };
            yield return new _FontSize() { DisplayName = "小三", FontSize = 20d };
            yield return new _FontSize() { DisplayName = "四号", FontSize = 18d };
            yield return new _FontSize() { DisplayName = "小四", FontSize = 16d };
            yield return new _FontSize() { DisplayName = "五号", FontSize = 14d };
            yield return new _FontSize() { DisplayName = "小五", FontSize = 12d };
            yield return new _FontSize() { DisplayName = "六号", FontSize = 10d };
            yield return new _FontSize() { DisplayName = "小六", FontSize = 8.67d };
            yield return new _FontSize() { DisplayName = "七号", FontSize = 7.3d };
            yield return new _FontSize() { DisplayName = "八号", FontSize = 6.67d };
        }

        #endregion

        #region 变量

        static SettingViewModel _instance = new SettingViewModel();

        const string SUB_NAME = "Software\\EasyCodeword";

        const string BOOT_NAME = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private bool _boot;

        private SolidColorBrush _foreground = Converter.ToBrush(RWReg.GetValue(SUB_NAME, "Foreground", "#FF00FF00").ToString());

        private SolidColorBrush _background = Converter.ToBrush(RWReg.GetValue(SUB_NAME, "Background", "#FF000000").ToString());

        private _FontFamily _fontFamily = new _FontFamily(new FontFamily(RWReg.GetValue(SUB_NAME, "FontFamily", MainWindow.Instance.FontFamily.ToString()).ToString()) ?? MainWindow.Instance.FontFamily);

        private _FontStyle _fontStyle = new _FontStyle(Converter.ToInt(RWReg.GetValue(SUB_NAME, "FontStyle", 0)));

        private _FontSize _fontSize = _fontSizes.FirstOrDefault(f => f.FontSize == Converter.ToDouble(RWReg.GetValue(SUB_NAME, "FontSize", 14d)))
            ?? new _FontSize() { DisplayName = "14", FontSize = 14d };

        private bool _autoSave = Converter.ToInt(RWReg.GetValue(SUB_NAME, "AutoSave", 1)) != 0; // 退出时否自动保存

        private int _autoSaveInterval = Converter.ToInt(RWReg.GetValue(SUB_NAME, "AutoSaveInterval", 3)); // 分钟

        private bool _autoPlayMusic = Converter.ToInt(RWReg.GetValue(SUB_NAME, "AutoPlayMusic", 1)) != 0; // 播放背景音乐

        private string _musicFolder = RWReg.GetValue(SUB_NAME, "MusicFolder", string.Empty).ToString();

        private bool _isViolenceLock = Converter.ToInt(RWReg.GetValue(SUB_NAME, "LockType", 0)) == 0; // 暴力锁

        private bool _isTenderLock = Converter.ToInt(RWReg.GetValue(SUB_NAME, "LockType", 0)) != 0; // 暴力锁

        const string TENDERLOCK_MESSAGE = "今天又没能够有完成任务。";// 温柔锁发送的模板消息

        private string _tenderLockMessage = RWReg.GetValue(SUB_NAME, "TenderLockMessage", TENDERLOCK_MESSAGE).ToString(); // 温柔锁发送的模板消息

        private readonly DelegateCommand _saveCommand;

        #region 保存是否修改属性的变量值

        private bool _hasBootChanged = false;

        private bool _hasForegroundChanged = false;

        private bool _hasBackgroundChanged = false;

        private bool _hasFontFamilyChanged = false;

        private bool _hasFontStyleChanged = false;

        private bool _hasFontSizeChanged = false;

        private bool _hasAutoSaveChanged = false;

        private bool _hasAutoSaveIntervalChanged = false;

        private bool _hasAutoPlayMusicChanged = false;

        private bool _hasMusicFolderChanged = false;

        private bool _hasLockChanged = false;

        private bool _hasLockTypeChanged = false;

        private bool _hasTenderLockMessageChanged = false;

        #endregion

        #endregion

        #region 属性

        public static SettingViewModel Instance { get { return _instance; } }

        public LockViewModel Lock { get { return LockViewModel.Insatance; } } 

        public string RecentFile
        {
            get { return RWReg.GetValue(SUB_NAME, "RecentFile", string.Empty).ToString(); }
            set { RWReg.SetValue(SUB_NAME, "RecentFile", value); }
        }

        public Rect ReferenceLocation
        {
            get 
            {
                var rectString = RWReg.GetValue(SUB_NAME, "ReferenceLocation", string.Empty).ToString();
                if (Regex.IsMatch(rectString, @"^-?(\d+(\.\d*)?|\.\d+),-?(\d+(\.\d*)?|\.\d+),(\d+(\.\d*)?|\.\d+),(\d+(\.\d*)?|\.\d+)$"))
                {
                    return Rect.Parse(rectString);
                }
                return new Rect(SystemParameters.FullPrimaryScreenWidth * 0.5d, 35d, SystemParameters.FullPrimaryScreenWidth * 0.5d - 35d, SystemParameters.FullPrimaryScreenHeight - 35d);
            }
            set { RWReg.SetValue(SUB_NAME, "ReferenceLocation", value.ToString()); }
        }

        /// <summary>
        /// 上次打开的资料窗口
        /// </summary>
        public string ReferenceFile
        {
            get { return RWReg.GetValue(BOOT_NAME, "ReferenceFile", string.Empty).ToString(); }
            set { RWReg.SetValue(BOOT_NAME, "ReferenceFile", value); }
        }

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
                _hasBootChanged = HasBootChanged();
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
                _hasForegroundChanged = HasForegroundChanged();
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
                _hasBackgroundChanged = HasBackgroundChanged();
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<_FontFamily> Fonts
        {
            get { return _fontFamilies; }
        }

        public IEnumerable<_FontStyle> FontStyles
        {
            get { return _fontStyles; }
        }

        public IEnumerable<_FontSize> FontSizes
        {
            get { return _fontSizes; }
        }

        public _FontFamily __FontFamily
        {
            get { return _fontFamily; }
            set
            {
                _fontFamily = value; RaisePropertyChanged("__FontFamily");
                if (null != value)
                {
                    _hasBackgroundChanged = HasBackgroundChanged();
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public _FontStyle __FontStyle
        {
            get { return _fontStyle; }
            set
            {
                _fontStyle = value; RaisePropertyChanged("__FontStyle");
                if (null != value)
                {
                    _hasFontStyleChanged = HasFontStyleChanged();
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public _FontSize __FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value; RaisePropertyChanged("__FontSize");
                if (null != value)
                {
                    _hasFontSizeChanged = HasFontSizeChanged();
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool AutoSave
        {
            get
            {
                return _autoSave;
            }
            set
            {
                _autoSave = value;
                RaisePropertyChanged("AutoSave");
                _hasAutoSaveChanged = HasAutoSaveChanged();
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public int AutoSaveInterval
        {
            get { return _autoSaveInterval; }
            set
            {
                _autoSaveInterval = value; RaisePropertyChanged("AutoSaveInterval");
                _hasAutoSaveIntervalChanged = HasAutoSaveIntervalChanged();
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool AutoPlayMusic
        {
            get
            {
                return _autoPlayMusic;
            }
            set
            {
                _autoPlayMusic = value;
                RaisePropertyChanged("AutoPlayMusic");
                _hasAutoPlayMusicChanged = HasAutoPlayMusicChanged();
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public string MusicFolder
        {
            get { return _musicFolder; }
            set
            {
                _musicFolder = value; RaisePropertyChanged("MusicFolder");
                _hasMusicFolderChanged = HasMusicFolderChanged();
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 暴力锁
        /// </summary>
        public bool IsViolenceLock
        {
            get { return _isViolenceLock; }
            set
            {
                if (_isViolenceLock != value)
                {
                    _isViolenceLock = value; RaisePropertyChanged("IsViolenceLock");
                    IsTenderLock = !value;
                }
            }
        }

        /// <summary>
        /// 温柔锁
        /// </summary>
        public bool IsTenderLock
        {
            get { return _isTenderLock; }
            set
            {
                if (_isTenderLock != value)
                {
                    _isTenderLock = value; RaisePropertyChanged("IsTenderLock");
                    IsViolenceLock = !value;
                    _hasLockTypeChanged = HasLockTypeChanged();
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string TenderLockMessage
        {
            get { return _tenderLockMessage; }
            set
            {
                _tenderLockMessage = value; RaisePropertyChanged("TenderLockMessage");
                _hasTenderLockMessageChanged = HasTenderLockMessageChanged();
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

            Lock.PropertyChanged += Lock_PropertyChanged;
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

            if (_hasBackgroundChanged)
            {
                MainWindow.Instance.SetBackground(_background);
                RWReg.SetValue(SUB_NAME, "Background", _background.ToString());
            }

            if (_hasForegroundChanged)
            {
                MainWindow.Instance.SetForeground(_foreground);
                RWReg.SetValue(SUB_NAME, "Foreground", _foreground.ToString());
            }

            if (_hasFontFamilyChanged)
            {
                MainWindow.Instance.SetFontFamily(_fontFamily.FontFamily);
                RWReg.SetValue(SUB_NAME, "FontFamily", _fontFamily.ToString());
            }

            if (_hasFontStyleChanged)
            {
                MainWindow.Instance.SetFontStyle(_fontStyle);
                RWReg.SetValue(SUB_NAME, "FontStyle", _fontStyle.ID);
            }

            if (_hasFontSizeChanged)
            {
                MainWindow.Instance.SetFontSize(_fontSize.FontSize);
                RWReg.SetValue(SUB_NAME, "FontSize", _fontSize.FontSize);
            }

            if (_hasAutoSaveIntervalChanged)
            {
                RWReg.SetValue(SUB_NAME, "AutoSaveInterval", _autoSaveInterval);
            }

            if (_hasAutoSaveChanged)
            {
                RWReg.SetValue(SUB_NAME, "AutoSave", _autoSave ? 1 : 0);
            }

            if (_hasAutoPlayMusicChanged)
            {
                RWReg.SetValue(SUB_NAME, "AutoPlayMusic", _autoPlayMusic ? 1 : 0);
            }

            if (_hasMusicFolderChanged)
            {
                RWReg.SetValue(SUB_NAME, "MusicFolder", _musicFolder);
            }

            if (_hasAutoPlayMusicChanged)
            {
                if (_autoPlayMusic)
                {
                    SoundPlayerViewModel.Instance.Play(SettingViewModel.Instance.MusicFolder);
                }
                else
                {
                    SoundPlayerViewModel.Instance.Stop();
                }
            }
            else if (_hasMusicFolderChanged && _autoPlayMusic)
            {
                SoundPlayerViewModel.Instance.Play(SettingViewModel.Instance.MusicFolder);
            }

            if (_hasLockChanged)
            {
                Lock.Lock();
            }

            if (_hasLockTypeChanged)
            {
                RWReg.SetValue(SUB_NAME, "LockType", _isTenderLock ? 1 : 0);
            }

            if (_hasTenderLockMessageChanged)
            {
                RWReg.SetValue(SUB_NAME, "TenderLockMessage", _tenderLockMessage);
            }
        }

        private bool CanSave()
        {
            return _hasBootChanged
                || _hasForegroundChanged
                || _hasBackgroundChanged
                || _hasFontFamilyChanged
                || _hasFontStyleChanged
                || _hasFontSizeChanged
                || _hasAutoSaveIntervalChanged
                || _hasAutoSaveChanged
                || _hasAutoPlayMusicChanged
                || _hasMusicFolderChanged
                || _hasLockChanged
                || _hasLockTypeChanged
                || _hasTenderLockMessageChanged;
        }

        private void Lock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LockWords"
                || e.PropertyName == "LockMinutes"
                || e.PropertyName == "IsUnlocked")
            {
                _hasLockChanged = HasLockChanged();
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool HasBootChanged()
        {
            var boot = (string)RWReg.GetValue(BOOT_NAME, "EasyCodeword", string.Empty);
            return (!string.IsNullOrEmpty(boot) &&
                string.Compare(System.Windows.Forms.Application.ExecutablePath,
                    boot, true) == 0) != _boot;
        }

        private bool HasForegroundChanged()
        {
            return !string.Equals(
                RWReg.GetValue(SUB_NAME, "Foreground", "#FF00FF00").ToString(),
                _foreground.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
        }

        private bool HasBackgroundChanged()
        {
            return !string.Equals(
                RWReg.GetValue(SUB_NAME, "Background", "#FF000000").ToString(),
                _background.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
        }

        private bool HasFontFamilyChanged()
        {
            return null != _fontFamily && !string.Equals(
                RWReg.GetValue(SUB_NAME, "FontFamily", MainWindow.Instance.FontFamily.Source).ToString(),
                _fontFamily.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
        }

        private bool HasFontStyleChanged()
        {
            return null != _fontStyle && !int.Equals(
                Converter.ToInt(RWReg.GetValue(SUB_NAME, "FontStyle", 0)),
                _fontStyle.ID);
        }

        private bool HasFontSizeChanged()
        {
            return null != _fontSize && !double.Equals(
                Converter.ToDouble(RWReg.GetValue(SUB_NAME, "FontSize", 14d)),
                _fontSize.FontSize);
        }

        private bool HasAutoSaveChanged()
        {
            return !int.Equals(
                Converter.ToInt(RWReg.GetValue(SUB_NAME, "AutoSave", 1)),
                _autoSave ? 1 : 0);
        }

        private bool HasAutoSaveIntervalChanged()
        {
            return !double.Equals(
                Converter.ToInt(RWReg.GetValue(SUB_NAME, "AutoSaveInterval", 2)),
                _autoSaveInterval);
        }

        private bool HasAutoPlayMusicChanged()
        {
            return !int.Equals(
                Converter.ToInt(RWReg.GetValue(SUB_NAME, "AutoPlayMusic", 1)),
                _autoPlayMusic ? 1 : 0);
        }

        private bool HasMusicFolderChanged()
        {
            return !string.Equals(
                RWReg.GetValue(SUB_NAME, "MusicFolder", string.Empty).ToString(),
                _musicFolder, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool HasLockChanged()
        {
            var lockWords = Converter.ToInt(Lock.LockWords);
            var lockMinutes = Converter.ToInt(Lock.LockMinutes);
            return Lock.IsUnlocked && (lockWords > 0 || lockMinutes > 0);
        }

        private bool HasLockTypeChanged()
        {
            return !int.Equals(
                Converter.ToInt(RWReg.GetValue(SUB_NAME, "LockType", 0)),
                _isTenderLock ? 1 : 0);
        }

        private bool HasTenderLockMessageChanged()
        {
            return !string.Equals(
                RWReg.GetValue(SUB_NAME, "TenderLockMessage", TENDERLOCK_MESSAGE).ToString(),
                _tenderLockMessage);
        }

        #endregion
    }
}
