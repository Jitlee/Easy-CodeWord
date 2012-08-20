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
            this.Loaded += PowerWindow_Loaded;
        }

        private void PowerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hWnd = new WindowInteropHelper(this).Handle;
            Common.DisableMinmize(hWnd);
        }
    }
}
