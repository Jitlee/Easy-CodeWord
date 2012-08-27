using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    public class TypingSpeedViewModel : EntityObject
    {
        #region 变量

        private readonly TextBox _textBox = null;

        private readonly Timer _timer;

        private const int INTERVAL = 5 * 1000;

        private ILogger _logger = LoggerFactory.GetLogger(typeof(TypingSpeedViewModel).FullName);

        private bool _isTesting;

        private int _totalCurrentWords = 0;   // 当前

        private int _beginWords = 0;

        private DateTime _appStartDateTime = DateTime.Now;  // 软件启动时间

        private int _appStartWords = 0;  // 软件启动时间

        private TimeSpan _lastTotalDateTime = new TimeSpan(); // 上一次计算时间

        private int _lastTotalWords = 0; // 上一次统计的字数

        private DateTime _beginTime = DateTime.Now;

        /// <summary>
        /// 当前时长
        /// </summary>
        private TimeSpan _currentHours;

        /// <summary>
        /// 当前字数
        /// </summary>
        private int _currentWords;

        /// <summary>
        /// 今日时长
        /// </summary>
        private TimeSpan _todayHours;
        
        /// <summary>
        /// 今日字数
        /// </summary>
        private int _todayWords = 0;

        /// <summary>
        /// 最高日产量
        /// </summary>
        private int _maximumDailyWords = Converter.ToInt(RWReg.GetValue(SettingViewModel.SUB_NAME, "MaximumDailyWords", 0));

        /// <summary>
        /// 最高日用时
        /// </summary>
        private TimeSpan _maximumDailyHours = TimeSpan.FromMilliseconds(Converter.ToDouble(RWReg.GetValue(SettingViewModel.SUB_NAME, "MaximumDailyHours", 0)));

        /// <summary>
        /// 最快速度
        /// </summary>
        private int _maximumTypingSpeed = Converter.ToInt(RWReg.GetValue(SettingViewModel.SUB_NAME, "MaximumTypingSpeed", 0));

        /// <summary>
        /// 总用时
        /// </summary>
        private TimeSpan _totalHours = TimeSpan.FromMilliseconds(Converter.ToDouble(RWReg.GetValue(SettingViewModel.SUB_NAME, "TotalHours", 0)));

        /// <summary>
        /// 总字数
        /// </summary>
        private int _totalWords = Converter.ToInt(RWReg.GetValue(SettingViewModel.SUB_NAME, "TotalWords", 0));

        #endregion

        #region 属性

        public static TypingSpeedViewModel Instance { get; private set; }

        public TimeSpan CurrentHours
        {
            get { return _currentHours; }
            set { _currentHours = value; }
        }

        public int CurrentWords
        {
            get { return _currentWords; }
            set { _currentWords = value; }
        }

        public TimeSpan TodayHours
        {
            get { return _todayHours; }
            set { _todayHours = value; }
        }

        public int TodayWords
        {
            get { return _todayWords; }
            set { _todayWords = value; }
        }

        public int MaximumDailyWords
        {
            get { return _maximumDailyWords; }
            set { _maximumDailyWords = value; }
        }

        public TimeSpan MaximumDailyHours
        {
            get { return _maximumDailyHours; }
            set { _maximumDailyHours = value; }
        }

        public int MaximumTypingSpeed
        {
            get { return _maximumTypingSpeed; }
            set { _maximumTypingSpeed = value; }
        }

        public TimeSpan TotalHours
        {
            get { return _totalHours; }
            set { _totalHours = value; }
        }

        public int TotalWords
        {
            get { return _totalWords; }
            set { _totalWords = value; }
        }

        #endregion

        public static void Init(TextBox textBox)
        {
            Instance = new TypingSpeedViewModel(textBox);
        }

        public void Refresh()
        {
            _appStartWords = MainViewModel.Instance.CountWords(_textBox.Text);
        }

        ~TypingSpeedViewModel()
        {
            _timer.Dispose();
        }

        public TypingSpeedViewModel(TextBox textBox)
        {
            _timer = new Timer(TypingSpeedTestCallback);
            _textBox = textBox;
            _textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            _textBox.PreviewKeyUp += TextBox_PreviewKeyUp;

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            // 判断是否今天
            if (string.Equals(RWReg.GetValue(SettingViewModel.SUB_NAME, "Today", string.Empty),
                today))
            {
                _todayHours = TimeSpan.FromMilliseconds(Converter.ToDouble(RWReg.GetValue(SettingViewModel.SUB_NAME, "TodayHours", 0)));
                _todayWords = Converter.ToInt(RWReg.GetValue(SettingViewModel.SUB_NAME, "TodayWords", 0));
            }
            else
            {
                RWReg.SetValue(SettingViewModel.SUB_NAME, "Today", today);
                _todayHours = new TimeSpan();
                _todayWords = 0;
            }

            Refresh();
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift
                || Keyboard.Modifiers == ModifierKeys.None
                || (e.Key >= Key.D0 && e.Key <= Key.Z)
                || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                if (!_isTesting)
                {
                    _isTesting = true;
                    _beginWords = _textBox.Text.Length;
                    _beginTime = DateTime.Now;
                }
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void TextBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _timer.Change(INTERVAL, INTERVAL);
        }

        private void TypingSpeedTestCallback(object state)
        {
            Testing(INTERVAL);
        }

        private void Testing(double offset)
        {
            if (_isTesting)
            {
                _isTesting = false;
                var len = 0;
                var time = DateTime.Now;
                _textBox.Dispatcher.Invoke(new Action(() =>
                {
                    len = MainViewModel.Instance.CountWords(_textBox.Text) - _beginWords;
                }));
                var span = ((time - _beginTime).TotalMilliseconds - offset);
                _totalCurrentWords += len;

                if (len > 5
                    && span > 30d * 1000d)
                {
                    var speed = len * 60d * 1000d / span;
                    if (speed > MaximumTypingSpeed)
                    {
                        _maximumTypingSpeed = (int)speed;
                    }
                }
            }
        }

        public void Total()
        {
            Testing(0d);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            // 当前时长
            _currentHours = DateTime.Now - _appStartDateTime;

            //当前字数
            _currentWords = MainViewModel.Instance.CountWords(_textBox.Text) - _appStartWords;

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            // 判断是否今天
            if (string.Equals(RWReg.GetValue(SettingViewModel.SUB_NAME, "Today", string.Empty),
                today))
            {
                _todayHours += _currentHours - _lastTotalDateTime;
                _todayWords += _currentWords - _lastTotalWords;
            }
            else
            {
                RWReg.SetValue(SettingViewModel.SUB_NAME, "Today", today);

                // 检查昨天是否超过记录
                CheckMaximumDaily();

                _todayHours = _currentHours - _lastTotalDateTime;
                _todayWords = _currentWords - _lastTotalWords;
            }

            CheckMaximumDaily();

            _totalHours += _currentHours - _lastTotalDateTime;
            _totalWords += _currentWords - _lastTotalWords;

            // 保存当前值
            Save();

            _lastTotalDateTime = _currentHours;
            _lastTotalWords = _currentWords;
        }

        private void Save()
        {
            RWReg.SetValue(SettingViewModel.SUB_NAME, "TodayHours", _todayHours.TotalMilliseconds);
            RWReg.SetValue(SettingViewModel.SUB_NAME, "TodayWords", _todayWords);
            RWReg.SetValue(SettingViewModel.SUB_NAME, "MaximumDailyHours", _maximumDailyHours.TotalMilliseconds);
            RWReg.SetValue(SettingViewModel.SUB_NAME, "MaximumDailyWords", _maximumDailyWords);
            RWReg.SetValue(SettingViewModel.SUB_NAME, "MaximumTypingSpeed", _maximumTypingSpeed);
            RWReg.SetValue(SettingViewModel.SUB_NAME, "TotalHours", _totalHours.Milliseconds);
            RWReg.SetValue(SettingViewModel.SUB_NAME, "TotalWords", _totalWords);
        }

        private void CheckMaximumDaily()
        {
            if (_todayHours > _maximumDailyHours)
            {
                _maximumDailyHours = _todayHours;
            }

            if (_todayWords > _maximumDailyWords)
            {
                _maximumDailyWords = _todayWords;
            }
        }
    }
}
