﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using EasyCodeword.Utilities;
using System.Windows;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using System.ComponentModel;
using EasyCodeword.Views;

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

       

        private bool _boot;

        private SolidColorBrush _foreground = Converter.ToBrush(RWReg.GetValue(Constants.SubName, "Foreground", "#FF00FF00").ToString());

        private SolidColorBrush _background = Converter.ToBrush(RWReg.GetValue(Constants.SubName, "Background", "#FF000000").ToString());

        private _FontFamily _fontFamily = new _FontFamily(new FontFamily(RWReg.GetValue(Constants.SubName, "FontFamily", MainWindow.Instance.FontFamily.ToString()).ToString()) ?? MainWindow.Instance.FontFamily);

        private _FontStyle _fontStyle = new _FontStyle(Converter.ToInt(RWReg.GetValue(Constants.SubName, "FontStyle", 0)));

        private _FontSize _fontSize = _fontSizes.FirstOrDefault(f => f.FontSize == Converter.ToDouble(RWReg.GetValue(Constants.SubName, "FontSize", 14d)))
            ?? new _FontSize() { DisplayName = "14", FontSize = 14d };

        private bool _autoSave = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSave", 1)) != 0; // 退出时否自动保存

        private int _autoSaveInterval = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSaveInterval", 3)); // 分钟

        private bool _autoPlayMusic = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoPlayMusic", 1)) != 0; // 播放背景音乐

        private bool _isShowNowPlaying = Converter.ToInt(RWReg.GetValue(Constants.SubName, "IsShowNowPlaying", 1)) != 0; // 播放背景音乐

        private string _musicFolder = RWReg.GetValue(Constants.SubName, "MusicFolder", string.Empty).ToString();

        private bool _isViolenceLock = Converter.ToInt(RWReg.GetValue(Constants.SubName, "LockType", 0)) == 0; // 暴力锁

        private bool _isTenderLock = Converter.ToInt(RWReg.GetValue(Constants.SubName, "LockType", 0)) != 0; // 暴力锁

        const string TENDERLOCK_MESSAGE = "我在用码字宝强制码字软件，真心太狠了，今天任务又没完成！";// 温柔锁强制发送的内容

        private string _tenderLockMessage = RWReg.GetValue(Constants.SubName, "TenderLockMessage", TENDERLOCK_MESSAGE).ToString(); // 温柔锁强制发送的内容

        private bool _remeberRTF = Converter.ToInt(RWReg.GetValue(Constants.SubName, "RemeberRTF", 0)) != 0; //  打开 RTF 文档不再提示

        private bool _autoSaveExit = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSaveExit", 1)) != 0; //  退出时自动保存

        private bool _autoSaveReturn = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSaveReturn", 1)) != 0; //  回车时自动保存

        private readonly DelegateCommand _saveCommand;

        private readonly DelegateCommand<object> _authorizeCommand;

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

        private bool _hasIsShowNowPlayingChanged = false;

        private bool _hasMusicFolderChanged = false;

        private bool _hasLockChanged = false;

        private bool _hasLockTypeChanged = false;

        private bool _hasTenderLockMessageChanged = false;

        private bool _hasRemeberRTFChanged = false;

        private bool _hasAutoSaveExitChanged = false;

        private bool _hasAutoSaveReturnChanged = false;

        #endregion

        #endregion

        #region 属性

        public static SettingViewModel Instance { get { return _instance; } }

        public LockViewModel Lock { get { return LockViewModel.Insatance; } }

        public QWeiboViewModel QWeibo { get { return QWeiboViewModel.Instance; } }

        public SWeiboViewModel SWeibo { get { return SWeiboViewModel.Instance; } }

        public EmailViewModel Email { get { return EmailViewModel.Instance; } }

        public string RecentFile
        {
            get { return RWReg.GetValue(Constants.SubName, "RecentFile", string.Empty).ToString(); }
            set { RWReg.SetValue(Constants.SubName, "RecentFile", value); }
        }

        public Rect ReferenceLocation
        {
            get 
            {
                var rectString = RWReg.GetValue(Constants.SubName, "ReferenceLocation", string.Empty).ToString();
                if (Regex.IsMatch(rectString, @"^-?(\d+(\.\d*)?|\.\d+),-?(\d+(\.\d*)?|\.\d+),(\d+(\.\d*)?|\.\d+),(\d+(\.\d*)?|\.\d+)$"))
                {
                    return Rect.Parse(rectString);
                }
                return new Rect(SystemParameters.FullPrimaryScreenWidth * 0.5d, 35d, SystemParameters.FullPrimaryScreenWidth * 0.5d - 35d, SystemParameters.FullPrimaryScreenHeight - 35d);
            }
            set { RWReg.SetValue(Constants.SubName, "ReferenceLocation", value.ToString()); }
        }

        /// <summary>
        /// 上次打开的资料窗口
        /// </summary>
        public string ReferenceFile
        {
            get { return RWReg.GetValue(Constants.SubName, "ReferenceFile", string.Empty).ToString(); }
            set { RWReg.SetValue(Constants.SubName, "ReferenceFile", value); }
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

        public bool IsShowNowPlaying
        {
            get
            {
                return _isShowNowPlaying;
            }
            set
            {
                _isShowNowPlaying = value;
                RaisePropertyChanged("IsShowNowPlaying");
                _hasIsShowNowPlayingChanged = HasIsShowNowPlayingChanged();
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
                    _isViolenceLock = value;
                    RaisePropertyChanged("IsViolenceLock");
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
                    _isTenderLock = value;
                    RaisePropertyChanged("IsTenderLock");
                    _hasLockTypeChanged = HasLockTypeChanged();
                    _saveCommand.RaiseCanExecuteChanged();
                    IsViolenceLock = !value;
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

        public bool RemeberRTF
        {
            get { return _remeberRTF; }
            set
            {
                if (_remeberRTF != value)
                {
                    _remeberRTF = value;
                    RaisePropertyChanged("RemeberRTF");
                    _hasRemeberRTFChanged = HasRemeberRTFChanged();
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool AutoSaveExit
        {
            get { return _autoSaveExit; }
            set
            {
                if (_autoSaveExit != value)
                {
                    _autoSaveExit = value;
                    RaisePropertyChanged("AutoSaveExit");
                    _hasAutoSaveExitChanged = HasAutoSaveExitChanged();
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool AutoSaveReturn
        {
            get { return _autoSaveReturn; }
            set
            {
                if (_autoSaveReturn != value)
                {
                    _autoSaveReturn = value;
                    RaisePropertyChanged("AutoSaveReturn");
                    _hasAutoSaveReturnChanged = HasAutoSaveReturnChanged();
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand SaveCommand { get { return _saveCommand; } }

        public DelegateCommand<object> AuthorizeCommand { get { return _authorizeCommand; } }

        #endregion

        #region 构造方法

        private SettingViewModel()
        {
            var boot = (string)RWReg.GetValue(Constants.BootName, "EasyCodeword", string.Empty);
            _boot = !string.IsNullOrEmpty(boot) &&
                string.Compare(System.Windows.Forms.Application.ExecutablePath,
                    boot, true) == 0;

            _saveCommand = new DelegateCommand(Save, CanSave);
            _authorizeCommand = new DelegateCommand<object>(Authorize);

            Lock.PropertyChanged += Lock_PropertyChanged;
        }

        #endregion

        #region 私有方法

        private void Save()
        {
            if (Verify())
            {
                if (_boot)
                {
                    RWReg.SetValue(Constants.BootName, "EasyCodeword", System.Windows.Forms.Application.ExecutablePath);
                }
                else
                {
                    RWReg.RemoveKey(Constants.BootName, "EasyCodeword");
                }

                if (_hasBackgroundChanged)
                {
                    MainWindow.Instance.SetBackground(_background);
                    RWReg.SetValue(Constants.SubName, "Background", _background.ToString());
                }

                if (_hasForegroundChanged)
                {
                    MainWindow.Instance.SetForeground(_foreground);
                    RWReg.SetValue(Constants.SubName, "Foreground", _foreground.ToString());
                }

                if (_hasFontFamilyChanged)
                {
                    MainWindow.Instance.SetFontFamily(_fontFamily.FontFamily);
                    RWReg.SetValue(Constants.SubName, "FontFamily", _fontFamily.ToString());
                }

                if (_hasFontStyleChanged)
                {
                    MainWindow.Instance.SetFontStyle(_fontStyle);
                    RWReg.SetValue(Constants.SubName, "FontStyle", _fontStyle.ID);
                }

                if (_hasFontSizeChanged)
                {
                    MainWindow.Instance.SetFontSize(_fontSize.FontSize);
                    RWReg.SetValue(Constants.SubName, "FontSize", _fontSize.FontSize);
                }

                if (_hasAutoSaveIntervalChanged)
                {
                    RWReg.SetValue(Constants.SubName, "AutoSaveInterval", _autoSaveInterval);
                }

                if (_hasAutoSaveChanged)
                {
                    RWReg.SetValue(Constants.SubName, "AutoSave", _autoSave ? 1 : 0);
                }

                if (_hasAutoPlayMusicChanged)
                {
                    RWReg.SetValue(Constants.SubName, "AutoPlayMusic", _autoPlayMusic ? 1 : 0);

                }
                
                if (_hasIsShowNowPlayingChanged)
                {
                    RWReg.SetValue(Constants.SubName, "IsShowNowPlaying", _isShowNowPlaying ? 1 : 0);
                    SoundPlayerViewModel.Instance.RaisePropertyChanged("IsShowNowPlaying");
                }

                if (_hasMusicFolderChanged)
                {
                    RWReg.SetValue(Constants.SubName, "MusicFolder", _musicFolder);
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
                    RWReg.SetValue(Constants.SubName, "LockType", _isTenderLock ? 1 : 0);
                }

                if (_hasTenderLockMessageChanged)
                {
                    RWReg.SetValue(Constants.SubName, "TenderLockMessage", _tenderLockMessage);
                }

                if (QWeibo.HasChanged)
                {
                    QWeibo.Save();
                }

                if (SWeibo.HasChanged)
                {
                    SWeibo.Save();
                }

                if (Email.HasChanged)
                {
                    Email.Save();
                }

                if (_hasRemeberRTFChanged)
                {
                    SaveRememberRTF();
                }

                if (_hasAutoSaveExitChanged)
                {
                    SaveAutoSaveExit();
                }

                if (_hasAutoSaveReturnChanged)
                {
                    RWReg.SetValue(Constants.SubName, "AutoSaveReturn", _autoSaveReturn ? 1 : 0);
                }

                Reset();
            }
        }

        public bool Verify()
        {
            if (_hasLockTypeChanged
                && _isTenderLock && !SWeibo.IsAuthorized
                && !QWeibo.IsAuthorized)
            {
                AlertWindow.ShowAlert("如果选择温柔锁，至少需要授权一个微博账号!", "锁定级别");
                return false;
            }
            else if (!Lock.Verify())
            {
                return false;
            }

            return true;
        }

        public void SaveAutoSaveExit()
        {
            RWReg.SetValue(Constants.SubName, "AutoSaveExit", _autoSaveExit ? 1 : 0);
            _hasAutoSaveExitChanged = false;
        }

        public void SaveRememberRTF()
        {
            RWReg.SetValue(Constants.SubName, "RemeberRTF", _remeberRTF ? 1 : 0);
            _hasRemeberRTFChanged = false;
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
                || _hasIsShowNowPlayingChanged
                || _hasMusicFolderChanged
                || _hasLockChanged
                || _hasLockTypeChanged
                || _hasTenderLockMessageChanged
                || QWeibo.HasChanged
                || SWeibo.HasChanged
                || Email.HasChanged
                || _hasRemeberRTFChanged
                || _hasAutoSaveExitChanged
                || _hasAutoSaveReturnChanged;
        }

        private void Reset()
        {
            _hasBootChanged = false;
            _hasForegroundChanged = false;
            _hasBackgroundChanged = false;
            _hasFontFamilyChanged = false;
            _hasFontStyleChanged = false;
            _hasFontSizeChanged = false;
            _hasAutoSaveIntervalChanged = false;
            _hasAutoSaveChanged = false;
            _hasAutoPlayMusicChanged = false;
            _hasMusicFolderChanged = false;
            _hasIsShowNowPlayingChanged = false;
            _hasLockChanged = false;
            _hasLockTypeChanged = false;
            _hasTenderLockMessageChanged = false;
            QWeibo.Reset();
            SWeibo.Reset();
            Email.Reset();
            _hasRemeberRTFChanged = false;
            _hasAutoSaveExitChanged = false;
            _hasAutoSaveReturnChanged = false;
        }

        public void Cancel()
        {
            var boot = (string)RWReg.GetValue(Constants.BootName, "EasyCodeword", string.Empty);
            _boot = !string.IsNullOrEmpty(boot) &&
                string.Compare(System.Windows.Forms.Application.ExecutablePath,
                    boot, true) == 0;
            RaisePropertyChanged("Boot");

            if (_hasBackgroundChanged)
            {
                _background = Converter.ToBrush(RWReg.GetValue(Constants.SubName, "Background", "#FF000000").ToString());
                RaisePropertyChanged("Background");
            }

            if (_hasForegroundChanged)
            {
                _foreground = Converter.ToBrush(RWReg.GetValue(Constants.SubName, "Foreground", "#FF00FF00").ToString());
                RaisePropertyChanged("Foreground");
            }

            if (_hasFontFamilyChanged)
            {
                _fontFamily = new _FontFamily(new FontFamily(RWReg.GetValue(Constants.SubName, "FontFamily", MainWindow.Instance.FontFamily.ToString()).ToString()) ?? MainWindow.Instance.FontFamily);
                RaisePropertyChanged("__FontFamily");
            }

            if (_hasFontStyleChanged)
            {
                _fontStyle = new _FontStyle(Converter.ToInt(RWReg.GetValue(Constants.SubName, "FontStyle", 0)));
                RaisePropertyChanged("__FontStyle");
            }

            if (_hasFontSizeChanged)
            {
                _fontSize = _fontSizes.FirstOrDefault(f => f.FontSize == Converter.ToDouble(RWReg.GetValue(Constants.SubName, "FontSize", 14d)))
                    ?? new _FontSize() { DisplayName = "14", FontSize = 14d };
                RaisePropertyChanged("__FontSize");
            }

            if (_hasAutoSaveIntervalChanged)
            {
                _autoSaveInterval = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSaveInterval", 3)); // 分钟
                RaisePropertyChanged("AutoSaveInterval");
            }

            if (_hasAutoSaveChanged)
            {
                _autoSave = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSave", 1)) != 0; // 退出时否自动保存
                RaisePropertyChanged("AutoSave");
            }

            if (_hasAutoPlayMusicChanged)
            {
                _autoPlayMusic = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoPlayMusic", 1)) != 0; // 播放背景音乐
                RaisePropertyChanged("AutoPlayMusic");
            }
            
            if (_hasIsShowNowPlayingChanged)
            {
                _isShowNowPlaying = Converter.ToInt(RWReg.GetValue(Constants.SubName, "IsShowNowPlaying", 1)) != 0; // 主界面是否显示正在播放的歌曲
                RaisePropertyChanged("IsShowNowPlaying");
            }

            if (_hasMusicFolderChanged)
            {
                _musicFolder = RWReg.GetValue(Constants.SubName, "MusicFolder", string.Empty).ToString();
                RaisePropertyChanged("MusicFolder");

            }

            if (_hasLockChanged)
            {
                Lock.Cancel();
            }

            if (_hasLockTypeChanged)
            {
                _isViolenceLock = Converter.ToInt(RWReg.GetValue(Constants.SubName, "LockType", 0)) == 0; // 暴力锁
                _isTenderLock = Converter.ToInt(RWReg.GetValue(Constants.SubName, "LockType", 0)) != 0; // 暴力锁
                RaisePropertyChanged("IsViolenceLock");
                RaisePropertyChanged("IsTenderLock");
            }

            if (_hasTenderLockMessageChanged)
            {
                _tenderLockMessage = RWReg.GetValue(Constants.SubName, "TenderLockMessage", TENDERLOCK_MESSAGE).ToString(); // 温柔锁强制发送的内容
                RaisePropertyChanged("TenderLockMessage");
            }

            if (QWeibo.HasChanged)
            {
                QWeibo.Cancel();
            }

            if (SWeibo.HasChanged)
            {
                SWeibo.Cancel();
            }

            if (Email.HasChanged)
            {
                Email.Cancel();
            }

            if (_hasRemeberRTFChanged)
            {
                _remeberRTF = Converter.ToInt(RWReg.GetValue(Constants.SubName, "RemeberRTF", 0)) != 0; //  打开 RTF 文档不再提示
                RaisePropertyChanged("RemeberRTF");
            }

            if (_hasAutoSaveExitChanged)
            {
                _autoSaveExit = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSaveExit", 1)) != 0; //  退出时自动保存
                RaisePropertyChanged("AutoSaveExit");
            }

            if (_hasAutoSaveReturnChanged)
            {
                _autoSaveReturn = Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSaveReturn", 1)) != 0; //  回车时自动保存
                RaisePropertyChanged("AutoSaveReturn");
            }
            Reset();
        }

        /// <summary>
        /// 微博授权 0: 新浪 1: 腾讯
        /// </summary>
        private void Authorize(object state)
        {
            try
            {
                var type = Converter.ToInt(state);
                if (type == 0)
                {
                    SWeibo.Authorize();
                }
                else if (type == 1)
                {
                    // 用户授权
                    QWeibo.Authorize();
                }
                else if (type == -1)
                {
                    // 取消授权
                    QWeibo.Deauthorize();
                }
                else if (type == -2)
                {
                    // 取消授权
                    SWeibo.Deauthorize();

                    MainWindow.Instance.ShowMessage("你已成功取消新浪微博对本应用的授权！");
                }
            }
            catch
            {
                MainWindow.Instance.ShowMessage("网络异常！");
            }
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
            var boot = (string)RWReg.GetValue(Constants.BootName, "EasyCodeword", string.Empty);
            return (!string.IsNullOrEmpty(boot) &&
                string.Compare(System.Windows.Forms.Application.ExecutablePath,
                    boot, true) == 0) != _boot;
        }

        private bool HasForegroundChanged()
        {
            return !string.Equals(
                RWReg.GetValue(Constants.SubName, "Foreground", "#FF00FF00").ToString(),
                _foreground.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
        }

        private bool HasBackgroundChanged()
        {
            return !string.Equals(
                RWReg.GetValue(Constants.SubName, "Background", "#FF000000").ToString(),
                _background.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
        }

        private bool HasFontFamilyChanged()
        {
            return null != _fontFamily && !string.Equals(
                RWReg.GetValue(Constants.SubName, "FontFamily", MainWindow.Instance.FontFamily.Source).ToString(),
                _fontFamily.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
        }

        private bool HasFontStyleChanged()
        {
            return null != _fontStyle && !int.Equals(
                Converter.ToInt(RWReg.GetValue(Constants.SubName, "FontStyle", 0)),
                _fontStyle.ID);
        }

        private bool HasFontSizeChanged()
        {
            return null != _fontSize && !double.Equals(
                Converter.ToDouble(RWReg.GetValue(Constants.SubName, "FontSize", 14d)),
                _fontSize.FontSize);
        }

        private bool HasAutoSaveChanged()
        {
            return !int.Equals(
                Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSave", 1)),
                _autoSave ? 1 : 0);
        }

        private bool HasAutoSaveIntervalChanged()
        {
            return !double.Equals(
                Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSaveInterval", 2)),
                _autoSaveInterval);
        }

        private bool HasAutoPlayMusicChanged()
        {
            return !int.Equals(
                Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoPlayMusic", 1)),
                _autoPlayMusic ? 1 : 0);
        }

        private bool HasIsShowNowPlayingChanged()
        {
            return !int.Equals(
                Converter.ToInt(RWReg.GetValue(Constants.SubName, "IsShowNowPlaying", 1)),
                _isShowNowPlaying ? 1 : 0);
        }

        private bool HasMusicFolderChanged()
        {
            return !string.Equals(
                RWReg.GetValue(Constants.SubName, "MusicFolder", string.Empty).ToString(),
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
                Converter.ToInt(RWReg.GetValue(Constants.SubName, "LockType", 0)),
                _isTenderLock ? 1 : 0);
        }

        private bool HasTenderLockMessageChanged()
        {
            return !string.Equals(
                RWReg.GetValue(Constants.SubName, "TenderLockMessage", TENDERLOCK_MESSAGE).ToString(),
                _tenderLockMessage);
        }

        private bool HasRemeberRTFChanged()
        {
            return !int.Equals(
                Converter.ToInt(RWReg.GetValue(Constants.SubName, "RemeberRTF", 0)),
                _remeberRTF ? 1 : 0);
        }
        
        private bool HasAutoSaveExitChanged()
        {
            return !int.Equals(
                Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSaveExit", 1)),
                _autoSaveExit ? 1 : 0);
        }

        private bool HasAutoSaveReturnChanged()
        {
            return !int.Equals(
                Converter.ToInt(RWReg.GetValue(Constants.SubName, "AutoSaveReturn", 1)),
                _autoSaveReturn ? 1 : 0);
        }

        #endregion
    }
}
