using PodClient.Model;
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
    /// Channel.xaml の相互作用ロジック
    /// </summary>
    public partial class ChannelPage : Page
    {
        public ChannelPage(NavigationService navi, NavigationService navi2, Channel ch)
        {
            InitializeComponent();

            this.DataContext = new ChannelPageViewModel(navi, navi2, ch);

        }

        private void Tracks_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
