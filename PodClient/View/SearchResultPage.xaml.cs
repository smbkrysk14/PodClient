using PodClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PodClient.View
{
    /// <summary>
    /// Search.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchResultPage : Page
    {
        public SearchResultPage()
        {
            InitializeComponent();
        }

        public SearchResultPage(NavigationService navi, NavigationService navi2, string keyWord)
        {
            InitializeComponent();
            this.DataContext = new SearchResultViewModel(navi, navi2, keyWord);
        }
    }
}
