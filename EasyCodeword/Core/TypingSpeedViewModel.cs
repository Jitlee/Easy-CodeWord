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

        //private readonly Timer _timer;

        //private const int INTERVAL = 5 * 1000;

        private ILogger _logger = LoggerFactory.GetLogger(typeof(TypingSpeedViewModel).FullName);

        //private bool _isTesting;

        //private int _totalCurrentWords = 0;   // 当前

        //private int _beginWords = 0;

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
        /// 今日速度
        /// </summary>
        private int _todayTypingSpeed = 0;

        /// <summary>
        /// 最高日产量
        /// </summary>
        private int _maximumDailyWords = Converter.ToInt(RWReg.GetValue(Constants.SubName, "MaximumDailyWords", 0));

        /// <summary>
        /// 最高日用时
        /// </summary>
        private TimeSpan _maximumDailyHours = TimeSpan.FromMilliseconds(Converter.ToDouble(RWReg.GetValue(Constants.SubName, "MaximumDailyHours", 0)));

        /// <summary>
        /// 最快速度
        /// </summary>
        private int _maximumTypingSpeed = Converter.ToInt(RWReg.GetValue(Constants.SubName, "MaximumTypingSpeed", 0));

        /// <summary>
        /// 总用时
        /// </summary>
        private TimeSpan _totalHours = TimeSpan.FromMilliseconds(Converter.ToDouble(RWReg.GetValue(Constants.SubName, "TotalHours", 0)));

        /// <summary>
        /// 总字数
        /// </summary>
        private int _totalWords = Converter.ToInt(RWReg.GetValue(Constants.SubName, "TotalWords", 0));

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

        public int TodayTypingSpeed
        {
            get { return _todayTypingSpeed; }
            set { _todayTypingSpeed = value; }
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

        //~TypingSpeedViewModel()
        //{
        //    _timer.Dispose();
        //}

        public TypingSpeedViewModel(TextBox textBox)
        {
            //_timer = new Timer(TypingSpeedTestCallback);
            _textBox = textBox;
            //_textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            //_textBox.PreviewKeyUp += TextBox_PreviewKeyUp;

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            // 判断是否今天
            if (string.Equals(RWReg.GetValue(Constants.SubName, "Today", string.Empty),
                today))
            {
                _todayHours = TimeSpan.FromMilliseconds(Converter.ToDouble(RWReg.GetValue(Constants.SubName, "TodayHours", 0)));
                _todayWords = Converter.ToInt(RWReg.GetValue(Constants.SubName, "TodayWords", 0));
            }
            else
            {
                RWReg.SetValue(Constants.SubName, "Today", today);
                _todayHours = new TimeSpan();
                _todayWords = 0;
            }

            Refresh();
        }

        //private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (Keyboard.Modifiers == ModifierKeys.Shift
        //        || Keyboard.Modifiers == ModifierKeys.None
        //        || (e.Key >= Key.D0 && e.Key <= Key.Z)
        //        || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
        //    {
        //        if (!_isTesting)
        //        {
        //            _isTesting = true;
        //            _beginWords = _textBox.Text.Length;
        //            _beginTime = DateTime.Now;
        //        }
        //        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        //    }
        //}

        //private void TextBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    _timer.Change(INTERVAL, INTERVAL);
        //}

        //private void TypingSpeedTestCallback(object state)
        //{
        //    Testing(INTERVAL);
        //}

        //private void Testing(double offset)
        //{
        //    if (_isTesting)
        //    {
        //        _isTesting = false;
        //        var len = 0;
        //        var time = DateTime.Now;
        //        _textBox.Dispatcher.Invoke(new Action(() =>
        //        {
        //            len = MainViewModel.Instance.CountWords(_textBox.Text) - _beginWords;
        //        }));
        //        var span = ((time - _beginTime).TotalMilliseconds - offset);
        //        _totalCurrentWords += len;

        //        if (len > 5
        //            && span > 30d * 1000d)
        //        {
        //            var speed = len * 60d * 1000d / span;
        //            if (speed > MaximumTypingSpeed)
        //            {
        //                _maximumTypingSpeed = (int)speed;
        //            }
        //        }
        //    }
        //}

        public void Total()
        {
            //Testing(0d);
            //_timer.Change(Timeout.Infinite, Timeout.Infinite);

            // 当前时长
            _currentHours = DateTime.Now - _appStartDateTime;

            //当前字数
            _currentWords = MainViewModel.Instance.CountWords(_textBox.Text) - _appStartWords;

            if (_currentWords < 0)
            {
                _currentWords = 0;
            }

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            // 判断是否今天
            if (string.Equals(RWReg.GetValue(Constants.SubName, "Today", string.Empty),
                today))
            {
                _todayHours += _currentHours - _lastTotalDateTime;
                _todayWords += _currentWords - _lastTotalWords;
            }
            else
            {
                RWReg.SetValue(Constants.SubName, "Today", today);

                // 检查昨天是否超过记录
                CheckMaximumDaily();

                _todayHours = _currentHours - _lastTotalDateTime;
                _todayWords = _currentWords - _lastTotalWords;
            }

            CheckMaximumDaily();

            _totalHours += _currentHours - _lastTotalDateTime;
            _totalWords += _currentWords - _lastTotalWords;

            _todayTypingSpeed = (int)(_todayWords * 60d * 1000d / _todayHours.TotalMilliseconds);

            // 保存当前值
            Save();

            _lastTotalDateTime = _currentHours;
            _lastTotalWords = _currentWords;
        }

        private void Save()
        {
            RWReg.SetValue(Constants.SubName, "TodayHours", _todayHours.TotalMilliseconds);
            RWReg.SetValue(Constants.SubName, "TodayWords", _todayWords);
            RWReg.SetValue(Constants.SubName, "MaximumDailyHours", _maximumDailyHours.TotalMilliseconds);
            RWReg.SetValue(Constants.SubName, "MaximumDailyWords", _maximumDailyWords);
            RWReg.SetValue(Constants.SubName, "MaximumTypingSpeed", _maximumTypingSpeed);
            RWReg.SetValue(Constants.SubName, "TotalHours", _totalHours.Milliseconds);
            RWReg.SetValue(Constants.SubName, "TotalWords", _totalWords);
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

            var typingSpeed = (int)(_todayWords * 60d * 1000d / _todayHours.TotalMilliseconds);
            if (typingSpeed > _maximumTypingSpeed)
            {
                _maximumTypingSpeed = typingSpeed;
            }
        }
    }
}
