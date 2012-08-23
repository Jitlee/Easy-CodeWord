using System.Windows;
using System.Windows.Media.Imaging;


namespace EasyCodeword.Views
{
    /// <summary>
    /// ConfirmWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public ConfirmWindow()
        {
            InitializeComponent();

            System.Drawing.Icon icon = System.Drawing.SystemIcons.Asterisk;

            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            IconImage.Source = bs;

            this.HideIconAndButton();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
