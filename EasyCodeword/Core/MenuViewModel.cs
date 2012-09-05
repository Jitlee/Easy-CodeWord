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
            Icon = "Images/file.png",
            Items = new MenuItem[]
            {
                new MenuItem(){ Title = "新建", Command=new DelegateCommand(MainWindow.Instance.NewCommand), Icon="Images/new.png" },
                new MenuItem(){ Title = "打开", Command=new DelegateCommand(MainWindow.Instance.OpenCommand), Icon="Images/open.png" },
                new MenuItem(){ Title = "保存", Command=new DelegateCommand(MainWindow.Instance.SaveCommand), Icon="Images/save.png" },
                new MenuItem(){ Title = "另存为", Command=new DelegateCommand(MainWindow.Instance.SaveAsCommand), Icon="Images/save_as.png" },
                new MenuItem(){ Title = "退出", Command=new DelegateCommand(MainWindow.Instance.ExitCommand), Icon="Images/exit.png" }
            }
        };

        public readonly static MenuItem EditMenu = new MenuItem()
        {
            Title = "编辑",
            Icon = "Images/edit.png",
            Items = new MenuItem[]
            {
                new MenuItem(){ Title = "全选", Command=ApplicationCommands.SelectAll, CommandTarget = MainWindow.Instance.MainTextBox, Icon="Images/select_all.png" },
                new MenuItem(){ Title = "剪切", Command=ApplicationCommands.Cut, CommandTarget = MainWindow.Instance.MainTextBox, Icon="Images/cut.png" },
                new MenuItem(){ Title = "复制", Command=ApplicationCommands.Copy, CommandTarget = MainWindow.Instance.MainTextBox, Icon="Images/copy.png" },
                new MenuItem(){ Title = "粘贴", Command=ApplicationCommands.Paste, CommandTarget = MainWindow.Instance.MainTextBox, Icon="Images/paste.png" },
                new MenuItem(){ Title = "撤销", Command=ApplicationCommands.Undo, CommandTarget = MainWindow.Instance.MainTextBox, Icon="Images/undo.png" }
            }
        };

        public readonly static MenuItem OperationMenu = new MenuItem()
        {
            Title = "操作",
            Icon = "Images/operation.png",
            Items = new MenuItem[]
            {
                new MenuItem(){ Title = "查找", Command=new DelegateCommand(MainWindow.Instance.SearchCommand), Icon="Images/find.png" },
                new MenuItem(){ Title = "替换", Command=new DelegateCommand(MainWindow.Instance.ReplaceCommand), Icon="Images/replace.png" },
                new MenuItem(){ Title = "设置窗口",Command=new DelegateCommand(MainWindow.Instance.SettingCommand), Icon="Images/setting.png" },
                //new MenuItem(){ Title = "日记窗口", Command=ApplicationCommands.Paste, CommandTarget = MainWindow.Instance.MainTextBox },
                new MenuItem(){ Title = "资料窗口", Command=new DelegateCommand(MainWindow.Instance.RefrenceCommand), Icon="Images/reference.png" }
            }
        };

        public readonly static MenuItem HelpMenu = new MenuItem()
        {
            Title = "帮助",
            Icon = "Images/help.png",
            Items = new MenuItem[]
            {
                new MenuItem(){ Title = "一键排版", Command=new DelegateCommand(MainWindow.Instance.AutoTypingSettingCommand), Icon="Images/auto_typing.png" },
                new MenuItem(){ Title = "统计窗口", Command=new DelegateCommand(MainWindow.Instance.TotalCommand), Icon="Images/total.png" },
                new MenuItem(){ Title = "邮件窗口",Command=new DelegateCommand(MainWindow.Instance.SendEmailCommand), Icon="Images/email.png" },
                new MenuItem(){ Title = "操作计算机", Command=new DelegateCommand(MainWindow.Instance.PowerCommand), Icon="Images/power.png" },
                new MenuItem(){ Title = "帮助卡片", Command=new DelegateCommand(MainWindow.Instance.AboutCommand), Icon="Images/about.png" }
            }
        };
    }

    public class MenuItem
    {
        public string Title { get; set; }
        public ICommand Command { get; set; }
        public object CommandTarget { get; set; }
        public MenuItem[] Items { get; set; }
        public string Icon { get; set; }
    }
}
