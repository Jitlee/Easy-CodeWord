using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using EasyCodeword.Core;
using EasyCodeword.Utilities;

namespace EasyCodeword
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public class EntryPoint
        {
            [STAThread]
            public static void Main(string[] args)
            {
                var serialNumber = RWReg.GetValue(Constants.SubName, "Cache", string.Empty).ToString();
                if (!string.IsNullOrEmpty(serialNumber)
                    && Common.IsAdmin())
                {
                    try
                    {
                        RWReg.RemoveKey(Constants.SubName, "Cache");
                        // 软件注册
                        RWReg.SetValue(
                            Microsoft.Win32.Registry.LocalMachine,
                            Constants.SubName,
                            "Cache", serialNumber);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        System.Environment.Exit(System.Environment.ExitCode);
                    }
                }
                else
                {
                    var app = new App();
                    app.InitializeComponent();
                    app.Run();
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            LicenseProvider.Verify();
            SplashScreen splashScreen = new SplashScreen("Images/splash_screen.png");
            splashScreen.Show(true);
            base.OnStartup(e);
        }
    }
}
