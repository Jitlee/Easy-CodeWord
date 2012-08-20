using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyCodeword.Core
{
    public class PowerViewModel : EntityObject
    {
        private static PowerViewModel _instance = new PowerViewModel();
        private string _description;

        private int _operation = 0;

        private string[] _descriptions = new[]
        {
            "关闭所有打开的程序，关闭 Windows，然后关闭计算机。",
            "关闭所有打开程序，注销系统。",
            "关闭所有打开程序，关闭Windows，然后重新启动系统。",
            "保存会话并关闭计算机。打开计算机时，Windows 会还原会话。"
        };

        private DelegateCommand _okCommand;

        public static PowerViewModel Instance { get { return _instance; } }

        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
        }

        public int Operation
        {
            get { return _operation; }
            set
            {
                _operation = value;
                RaisePropertyChanged("Operation");
                if (value > -1 && value < 4)
                {
                    Description = _descriptions[value];
                }
                else
                {
                    Description = string.Empty;
                }
            }
        }

        public DelegateCommand OKCommand { get { return _okCommand; } }

        private PowerViewModel()
        {
            _description = _descriptions[_operation];
            _okCommand = new DelegateCommand(OK);
        }

        private void OK()
        {
            switch (_operation)
            {
                case 0:
                    PowerHelper.Shutdown();
                    break;
                case 1:
                    PowerHelper.Logoff();
                    break;
                case 2:
                    PowerHelper.Reboot();
                    break;
                case 3:
                    PowerHelper.Poweroff();
                    break;
            }
        }
    }
}
