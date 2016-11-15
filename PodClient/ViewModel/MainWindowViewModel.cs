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
    class MainWindowViewModel : ViewModelBase
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
            //this._navi2.Navigate(new AudioControlPage(_navi2));
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
    }
}
