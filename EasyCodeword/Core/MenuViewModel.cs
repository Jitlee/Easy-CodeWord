using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace EasyCodeword.Core
{
    public class MenuViewModel
    {
        public readonly static MenuItem FileMenu = new MenuItem()
        {
            Title = "文件",
            Items = new MenuItem[]
            {
                new MenuItem(){ Title = "新建", Command=new DelegateCommand(MainWindow.Instance.NewCommand) },
                new MenuItem(){ Title = "打开", Command=new DelegateCommand(MainWindow.Instance.OpenCommand) },
                new MenuItem(){ Title = "保存", Command=new DelegateCommand(MainWindow.Instance.SaveCommand) },
                new MenuItem(){ Title = "另存为", Command=new DelegateCommand(MainWindow.Instance.SaveAsCommand) },
                new MenuItem(){ Title = "退出", Command=new DelegateCommand(MainWindow.Instance.ExitCommand) }
            }
        };

        public readonly static MenuItem EditMenu = new MenuItem()
        {
            Title = "编辑",
            Items = new MenuItem[]
            {
                new MenuItem(){ Title = "全选", Command=ApplicationCommands.SelectAll, CommandTarget = MainWindow.Instance.MainTextBox },
                new MenuItem(){ Title = "剪切", Command=ApplicationCommands.Cut, CommandTarget = MainWindow.Instance.MainTextBox },
                new MenuItem(){ Title = "复制", Command=ApplicationCommands.Copy, CommandTarget = MainWindow.Instance.MainTextBox },
                new MenuItem(){ Title = "粘贴", Command=ApplicationCommands.Paste, CommandTarget = MainWindow.Instance.MainTextBox },
                new MenuItem(){ Title = "撤销", Command=ApplicationCommands.Undo, CommandTarget = MainWindow.Instance.MainTextBox }
            }
        };

        public readonly static MenuItem OperationMenu = new MenuItem()
        {
            Title = "操作",
            Items = new MenuItem[]
            {
                new MenuItem(){ Title = "查找", Command=new DelegateCommand(MainWindow.Instance.SearchCommand) },
                new MenuItem(){ Title = "替换", Command=new DelegateCommand(MainWindow.Instance.ReplaceCommand) },
                new MenuItem(){ Title = "设置\n窗口",Command=new DelegateCommand(MainWindow.Instance.SettingCommand) },
                //new MenuItem(){ Title = "日记窗口", Command=ApplicationCommands.Paste, CommandTarget = MainWindow.Instance.MainTextBox },
                new MenuItem(){ Title = "资料\n窗口", Command=new DelegateCommand(MainWindow.Instance.RefrenceCommand) }
            }
        };

        public readonly static MenuItem HelpMenu = new MenuItem()
        {
            Title = "帮助",
            Items = new MenuItem[]
            {
                new MenuItem(){ Title = "一键\n排版", Command=new DelegateCommand(MainWindow.Instance.AutoTypingSettingCommand) },
                new MenuItem(){ Title = "统计\n窗口", Command=new DelegateCommand(MainWindow.Instance.TotalCommand) },
                new MenuItem(){ Title = "邮件\n窗口",Command=new DelegateCommand(MainWindow.Instance.SendEmailCommand) },
                new MenuItem(){ Title = "操作\n计算机", Command=new DelegateCommand(MainWindow.Instance.PowerCommand) },
                new MenuItem(){ Title = "帮助\n卡片", Command=new DelegateCommand(MainWindow.Instance.AboutCommand) }
            }
        };
    }

    public class MenuItem
    {
        public string Title { get; set; }
        public ICommand Command { get; set; }
        public object CommandTarget { get; set; }
        public MenuItem[] Items { get; set; }
    }
}
