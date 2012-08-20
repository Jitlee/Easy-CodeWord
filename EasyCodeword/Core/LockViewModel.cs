using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyCodeword.Core
{
    public class LockViewModel : EntityObject
    {
        private static LockViewModel _instance = new LockViewModel();

        private bool _isViolenceLock = false;

        private bool _isTenderLock = false;

        private int _lockMinute;

        private int _lockLength;

        private readonly DelegateCommand _lockMinuteCommand;

        private readonly DelegateCommand _lockLengthCommand;

        public bool IsLock { get { return _lockMinute > 0 || _lockMinute > 0; } }

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


        public int LockMinute
        {
            get { return _lockMinute; }
            set { _lockMinute = value; RaisePropertyChanged("LockMinute"); }
        }

        public int LockLength
        {
            get { return _lockLength; }
            set { _lockLength = value; RaisePropertyChanged("LockLength"); }
        }

        public DelegateCommand LockMinuteCommand { get { return _lockMinuteCommand; } }

        public DelegateCommand LockLengthCommand { get { return _lockLengthCommand; } }

        private LockViewModel()
        {
            _lockMinuteCommand = new DelegateCommand(LockMinuteHandler);
            _lockLengthCommand = new DelegateCommand(LockLengthHandler);
        }

        public void LockMinuteHandler()
        {

        }

        public void LockLengthHandler()
        {

        }
    }
}
