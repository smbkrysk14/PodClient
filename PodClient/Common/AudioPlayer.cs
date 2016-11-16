using NAudio.Wave;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;

namespace PodClient.Common
{
    /// <summary>
    /// オーディオ再生を行います。
    /// </summary>
    sealed class AudioPlayer : IDisposable
    {
        private WaveStream _audioStream;
        private WaveChannel32 _volumeStream;
        private IWavePlayer _waveOut;
        private Stream ms = new MemoryStream();
        
        public void Play(string url)
        {
            if (_waveOut == null)
            {
                new Thread(delegate (object o)
                {
                    var response = WebRequest.Create(url).GetResponse();
                    using (var stream = response.GetResponseStream())
                    {

                        byte[] buffer = new byte[65536]; // 64KB chunks

                        int read;

                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            var pos = ms.Position;
                            ms.Position = ms.Length;
                            ms.Write(buffer, 0, read);
                            ms.Position = pos;
                        }

                    }
                }).Start();
            }


            // Pre-buffering some data to allow NAudio to start playing
            while (ms.Length < 65536 * 50)
                Thread.Sleep(1000);


            using (Mp3FileReader reader = new Mp3FileReader(ms))
            using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(reader))
            using (WaveStream blockAlignedStream = new BlockAlignReductionStream(pcm))
            {
                using (_waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    _waveOut.Init(blockAlignedStream);

                    _waveOut.Play();
                    while (_waveOut != null && _waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }



        /// <summary>
        /// 再生を一時停止します。
        /// </summary>
        public void Pause()
        {
            this._waveOut.Pause();
        }

        /// <summary>
        /// 再生を開始します。
        /// </summary>
        public void Play()
        {
            switch (this._waveOut.PlaybackState)
            {
                case PlaybackState.Playing:
                    break;

                case PlaybackState.Paused:
                case PlaybackState.Stopped:
                    this._waveOut.Play();
                    break;
            }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop()
        {
            this._waveOut.Stop();
            this.ms.Position = 0;
        }

        /// <summary>
        /// ボリュームを取得または設定します。
        /// </summary>
        public float Volume
        {
            get
            {
                return this._volumeStream.Volume;
            }
            set
            {
                this._volumeStream.Volume = value;
            }
        }

        /// <summary>
        /// リソースの解放を行います。
        /// </summary>
        public void Dispose()
        {
            if (this._waveOut != null)
            {
                this._waveOut.Stop();
            }

            if (this._audioStream != null)
            {
                this._volumeStream.Close();
                this._volumeStream = null;

                this._audioStream.Close();
                this._audioStream = null;
            }

            if (this._waveOut != null)
            {
                this._waveOut.Dispose();
                this._waveOut = null;
            }
        }

        public Stream GetStream()
        {
            return this._audioStream;
        }
    }
}
