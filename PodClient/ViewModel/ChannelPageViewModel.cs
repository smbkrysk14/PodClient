using PodClient.Common;
using PodClient.Model;
using PodClient.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace PodClient.ViewModel
{
    public class ChannelPageViewModel:ViewModelBase
    {
        private NavigationService _navi;
        private NavigationService _navi2;

        public ChannelPageViewModel() { }

        public ChannelPageViewModel(NavigationService navi, NavigationService navi2, Channel ch)
        {
            this._navi = navi;
            this._navi2 = navi2;

            List<Channel> a = new List<Model.Channel>();
            a.Add(ch);
            this.Channel = a;;
            this.Tracks = ch.tracks;
        }

        private void Back()
        {
            this._navi.GoBack();
        }

        private void TrackSelected(string trackId)
        {
            this.AsyncTaskFunc(trackId);
        }

        public async void AsyncTaskFunc(string trackId)
        {

            this._navi2.Navigate(new AudioControlPage(_navi2, trackId));


            //await Task.Run(() => {
            //    audio.PlayRadio(trackId);
            //});
        }


        ICommand backCommand;
        public ICommand BackCommand
        {
            get
            {
                return backCommand ?? (backCommand = new DelegateCommand(Back));
            }
        }

        ICommand trackSelectedCommand;
        public ICommand TrackSelectedCommand
        {
            get
            {
                return trackSelectedCommand ?? (trackSelectedCommand = new DelegateCommand<string>(TrackSelected));
            }
        }


        private string _thumbnail;
        public string Thumbnail
        {
            set
            {
                this._thumbnail = value;
                base.RaisePropertyChanged("Thumbnail");
            }
            get
            {
                return this._thumbnail;
            }
        }

        private string _title;
        public string Title
        {
            set
            {
                this._title = value;
                base.RaisePropertyChanged("Title");
            }
            get
            {
                return this._title;
            }
        }

        private string _itunesAuthor;
        public string ItunesAuthor
        {
            set
            {
                this._itunesAuthor = value;
                base.RaisePropertyChanged("ItunesAuthor");
            }
            get
            {
                return this._itunesAuthor;
            }
        }

        private string _description;
        public string Description
        {
            set
            {
                this._description = value;
                base.RaisePropertyChanged("Description");
            }
            get
            {
                return this._description;
            }
        }



        private List<Channel> _channel;
        public List<Channel> Channel
        {
            set
            {
                this._channel = value;
                base.RaisePropertyChanged("Channel");
            }
            get
            {
                return this._channel;
            }
        }

        private List<Track> _tracks;
        public List<Track> Tracks
        {
            set
            {
                this._tracks = value;
                base.RaisePropertyChanged("Tracks");
    }
            get
            {
                return this._tracks;
            }
        }
    }
}
