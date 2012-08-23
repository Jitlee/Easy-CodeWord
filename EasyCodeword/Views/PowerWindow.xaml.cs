using System.Windows;
using System.Windows.Interop;
using EasyCodeword.Core;
using EasyCodeword.Utilities;

namespace EasyCodeword.Views
{
    /// <summary>
    /// PowerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PowerWindow : Window
    {
        public PowerWindow()
        {
            InitializeComponent();
            this.DataContext = PowerViewModel.Instance;
            this.HideIcon();
        }
    }
}
