using PodClient.Common;
using PodClient.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace PodClient.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private NavigationService _navi;
        private NavigationService _navi2;


        public MainWindowViewModel(NavigationService navi, NavigationService navi2)
        {
            this._navi = navi;
            this._navi2 = navi2;
            //this.MainFrame = new Frame();
        }

        private void Init()
        {
            this._navi.Navigate(new MyPodcast(_navi, _navi2));
        }

        private void Search(string searchWord)
        {
            this._navi.Navigate(new SearchResultPage(_navi, _navi2, searchWord));

        }

        private void OnButtonMyPodcast()
        {
            this._navi.Navigate(new MyPodcast(_navi, _navi2));
        }

        ICommand onButtonMyPodcastCommand;
        public ICommand OnButtonMyPodcastCommand
        {
            get
            {
                return onButtonMyPodcastCommand ?? (onButtonMyPodcastCommand = new DelegateCommand(OnButtonMyPodcast));
            }
        }

        ICommand searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                return searchCommand ?? (searchCommand = new DelegateCommand<string>(Search));
            }
        }

        ICommand initializeViewModelCommand;
        public ICommand InitializeViewModelCommand
        {
            get
            {
                return initializeViewModelCommand ?? (initializeViewModelCommand = new DelegateCommand(Init));
            }
        }



        private Frame _mainFrame;
        public Frame MainFrame
        {
            get
            {
                return this._mainFrame;
            }
            set
            {
                this._mainFrame = value;
                base.RaisePropertyChanged("MainFrame");
            }
        }

        private string _searchWordTxt;
        public string SearchWordTxt
        {
            get
            {
                return this._searchWordTxt;
            }
            set
            {
                this._searchWordTxt = value;
                base.RaisePropertyChanged("SearchWordTxt");
            }
        }
    }
}
