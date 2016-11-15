using MaterialDesignThemes.Wpf;
using PodClient.Common;
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
    public class AudioControlViewModel:ViewModelBase
    {
        private NavigationService _navi;
        AudioPlayer audio = new AudioPlayer();
        private bool isPlaying = false;

        public AudioControlViewModel(NavigationService navi, string trackId)
        {
            this._navi = navi;
            this.PreviewButton = PackIconKind.StepBackward;
            this.NextButton = PackIconKind.StepForward;
            this.PlayStopButton = PackIconKind.Play;

            this.PlayStop(trackId);
        }

        public async void PlayStop(string trackId)
        {
            if(!isPlaying)
            {

                this.PlayStopButton = PackIconKind.Stop;
                this.isPlaying = true;
                await Task.Run(() =>
                {
                    audio.Play(trackId);
                });
            }
            else
            {
                audio.Stop();
                this.PlayStopButton = PackIconKind.Play;
                this.isPlaying = false;
            }
        }

        ICommand playStopCommand;
        public ICommand PlayStopCommand
        {
            get
            {
                return playStopCommand ?? (playStopCommand = new DelegateCommand<string>(PlayStop));
            }
        }

        private PackIconKind _previewButton;
        public PackIconKind PreviewButton
        {
            get
            {
                return this._previewButton;
            }
            set
            {
                this._previewButton = value;
                RaisePropertyChanged("PreviewButton");
            }
        }
        private PackIconKind _nextButton;
        public PackIconKind NextButton
        {
            get
            {
                return this._nextButton;
            }
            set
            {
                this._nextButton = value;
                RaisePropertyChanged("NextButton");
            }
        }


        private PackIconKind _playStopButton;
        public PackIconKind PlayStopButton
        {
            get
            {
                return this._playStopButton;
            }
            set
            {
                this._playStopButton = value;
                RaisePropertyChanged("PlayStopButton");
            }
        }
    }
}
