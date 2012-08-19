using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyCodeword.Views
{
    /// <summary>
    /// SearchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchWindow : Window
    {
        public static bool CaseSensiteive { get; private set; }

        public static string SerachWords { get; private set; }

        public SearchWindow()
        {
            InitializeComponent();
            this.Loaded += SerachWindow_Loaded;
            this.Activated += SerachWindow_Activated;
        }

        private void SerachWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButtonState();
            SearchTextBox.Focus();
        }

        private void SerachWindow_Activated(object sender, EventArgs e)
        {
            UpdateButtonState();
            SearchTextBox.Focus();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Search(SearchTextBox.Text, CaseSensitiveCheckBox.IsChecked == true);
        }

        public static void Search()
        {
            Search(SerachWords, CaseSensiteive);
        }

        public static void Search(string searchWords, bool caseSensiteive)
        {
            SerachWords = searchWords;
            CaseSensiteive = caseSensiteive;
            StringComparison stringComparsion =
                caseSensiteive ?
                StringComparison.CurrentCulture
                : StringComparison.CurrentCultureIgnoreCase;
            var start = MainWindow.Instance.MainTextBox.SelectionStart + MainWindow.Instance.MainTextBox.SelectionLength;
            var index = MainWindow.Instance.MainTextBox.Text.IndexOf(searchWords, start, stringComparsion);
            if (index > -1)
            {
                MainWindow.Instance.MainTextBox.Select(index, searchWords.Length);
                MainWindow.Instance.MainTextBox.Focus();
            }
            else
            {
                MainWindow.Instance.ShowMessage("已超过文档结尾。");
                start = 0;
                index = MainWindow.Instance.MainTextBox.Text.IndexOf(searchWords, start, stringComparsion);
                if (index > -1)
                {
                    MainWindow.Instance.MainTextBox.Select(index, searchWords.Length);
                    MainWindow.Instance.MainTextBox.Focus();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            this.SearchNextButton.IsEnabled = SearchTextBox.Text.Length > 0;
        }
    }
}
