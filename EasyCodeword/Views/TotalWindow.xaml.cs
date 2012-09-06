using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EasyCodeword.Core;

namespace EasyCodeword.Views
{
    /// <summary>
    /// TotalWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TotalWindow : Window
    {
       private bool _flag = false;
       public TotalWindow()
        {
            InitializeComponent();
            this.Loaded += TotalWindow_Loaded;
           if (LicenseProvider.IsRegistered)
            {
                RegisterStatusRun.Text = "软件已注册";
            }
            else
            {
                RegisterStatusRun.Text = "注册";
                RegisterHelperRun.Text = "\n本软件尚未注册，部分功能将限制使用";
            }
            VersionRun.Text = Regex.Match(Assembly.GetExecutingAssembly().GetName().Version.ToString(), @"^[0-9]+\.[0-9]+").Value;
        }

       private void TotalWindow_Loaded(object sender, RoutedEventArgs e)
       {
           TypingSpeedViewModel.Instance.Total();
           this.DataContext = TypingSpeedViewModel.Instance;
       }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            this.Close();
            base.OnMouseDown(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_flag != true)
            {
                var unLoadStoryboard = Resources["UnLoadStoryboard"] as Storyboard;
                if (null != unLoadStoryboard)
                {
                    e.Cancel = true;
                    unLoadStoryboard.Completed += UnLoadStoryboard_Completed;
                    unLoadStoryboard.Begin();
                }
            }
            base.OnClosing(e);
        }

        private void UnLoadStoryboard_Completed(object sender, EventArgs e)
        {
            _flag = true;
            this.Close();
        }


        private void Regeister_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            var registerWindow = new RegisterWindow();
            registerWindow.Owner = MainWindow.Instance;
            registerWindow.Show();
        }
    }
}
