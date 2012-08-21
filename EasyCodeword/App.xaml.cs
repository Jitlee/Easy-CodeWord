using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using EasyCodeword.Core;

namespace EasyCodeword
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            SoundPlayerViewModel.Instance.Stop();
            base.OnExit(e);
        }
    }
}
