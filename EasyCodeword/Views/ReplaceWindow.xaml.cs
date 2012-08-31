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
using System.Text.RegularExpressions;

namespace EasyCodeword.Views
{
    /// <summary>
    /// ReplaceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReplaceWindow : Window
    {
        public ReplaceWindow()
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
            SearchWindow.Search(SearchTextBox.Text, CaseSensitiveCheckBox.IsChecked == true);
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            var igroneCase = CaseSensitiveCheckBox.IsChecked == true;
            StringComparison stringComparsion =
                igroneCase ?
                StringComparison.CurrentCulture
                : StringComparison.CurrentCultureIgnoreCase;

            var start = MainWindow.Instance.MainTextBox.SelectionStart + MainWindow.Instance.MainTextBox.SelectionLength;
            var index = MainWindow.Instance.MainTextBox.Text.IndexOf(SearchTextBox.Text, start, stringComparsion);
            if (index > -1)
            {
                MainWindow.Instance.MainTextBox.Text =
                    MainWindow.Instance.MainTextBox.Text.Remove(index, SearchTextBox.Text.Length)
                    .Insert(index, ReplaceTextBox.Text);

                MainWindow.Instance.MainTextBox.Select(index, ReplaceTextBox.Text.Length);
                MainWindow.Instance.MainTextBox.Focus();
            }
            else
            {
                MainWindow.Instance.ShowMessage("已超过文档结尾。");
                start = 0;
                index = MainWindow.Instance.MainTextBox.Text.IndexOf(SearchTextBox.Text, start, stringComparsion);
                if (index > -1)
                {
                    MainWindow.Instance.MainTextBox.Text =
                    MainWindow.Instance.MainTextBox.Text.Remove(index, SearchTextBox.Text.Length)
                    .Insert(index, ReplaceTextBox.Text);

                    MainWindow.Instance.MainTextBox.Select(index, ReplaceTextBox.Text.Length);
                    MainWindow.Instance.MainTextBox.Focus();
                }
            }
        }

        private void ReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            var caseSensitive = CaseSensitiveCheckBox.IsChecked == true;
            var regexOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

            var matches = Regex.Matches(MainWindow.Instance.MainTextBox.Text,
                Regex.Escape(SearchTextBox.Text),
                regexOptions);

            MainWindow.Instance.MainTextBox.Text =
                Regex.Replace(MainWindow.Instance.MainTextBox.Text,
                Regex.Escape(SearchTextBox.Text),
                Regex.Escape(ReplaceTextBox.Text),
                regexOptions);

            SearchWindow.Search(ReplaceTextBox.Text, CaseSensitiveCheckBox.IsChecked == true);

            MainWindow.Instance.ShowMessage("替换了 {0} 处搜索项。", matches.Count);
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
            this.SearchNextButton.IsEnabled =
                this.ReplaceButton.IsEnabled =
                this.ReplaceAllButton.IsEnabled = SearchTextBox.Text.Length > 0;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            MainWindow.Instance.Focus();
        }
    }
}
