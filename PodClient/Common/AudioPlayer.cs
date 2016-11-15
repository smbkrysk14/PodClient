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

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="fileName">ファイルへのパス。</param>
        /// <exception cref="FileNotFoundException">ファイルが存在しない。</exception>
        /// <exception cref="Exception">ストリームの生成に失敗した。</exception>
        public AudioPlayer(string fileName)
        {
            
            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                using (System.IO.Stream st = wc.OpenRead(fileName))
                {
                    try
                    {
                        if (st.CanRead)
                        {
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(st))
                            {
                                try
                                {
                                    Console.WriteLine(sr.ReadToEnd());
                                }
                                finally
                                {
                                    if (sr != null) sr.Close();
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (st != null)
                        {
                            st.Close();
                        }
                    }
                }
            }
            catch (System.Net.WebException we)
            {
                Console.WriteLine("System.Net.WebException が投げられました");
                throw we;
            }
            finally
            {
                if (wc != null) wc.Dispose();
            }
        }

        public AudioPlayer()
        {

        }
        /// <summary>
        /// ファイルへのストリームを生成します。
        /// </summary>
        /// <param name="fileName">ファイルへのパス。</param>
        /// <exception cref="InvalidOperationException">ストリームの生成に失敗した。</exception>
        private void InitializeStream(string fileName)
        {
            WaveChannel32 stream;
            if (fileName.EndsWith(".wav"))
            {
                WaveStream reader = new WaveFileReader(fileName);
                if (reader.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
                {
                    reader = WaveFormatConversionStream.CreatePcmStream(reader);
                    reader = new BlockAlignReductionStream(reader);
                }

                if (reader.WaveFormat.BitsPerSample != 16)
                {
                    var format = new WaveFormat(reader.WaveFormat.SampleRate, 16, reader.WaveFormat.Channels);
                    reader = new WaveFormatConversionStream(format, reader);
                }

                stream = new WaveChannel32(reader);
            }
            else if (fileName.EndsWith(".mp3"))
            {
                var reader = new Mp3FileReader(fileName);
                var pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);
                var blockAlignedStream = new BlockAlignReductionStream(pcmStream);

                stream = new WaveChannel32(blockAlignedStream);
            }
            else
            {
                MessageBox.Show("サポートしていないファイル形式です");
                return;
            }

            this._volumeStream = stream;
            this._audioStream = new MeteringStream(stream, stream.WaveFormat.SampleRate / 10);

            this._waveOut = new WaveOut() { DesiredLatency = 200 };
            this._waveOut.Init(this._audioStream);
        }

        public void PlayRadio(string url)
        {
            using (Stream ms = new MemoryStream())
            {
                using (Stream stream = WebRequest.Create(url)
                    .GetResponse().GetResponseStream())
                {
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                }

                ms.Position = 0;
                using (WaveStream blockAlignedStream =
                    new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(
                            new Mp3FileReader(ms))))
                {
                    using (_waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        _waveOut.Init(blockAlignedStream);
                        _waveOut.Play();
                        while (_waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }
        }

        //public void PlayRadio2(string url)
        //{
        //    using (Stream ms = new MemoryStream())
        //    {
        //        using (Stream stream = WebRequest.Create(url)
        //            .GetResponse().GetResponseStream())
        //        {
        //            byte[] buffer = new byte[32768];
        //            int read;
        //            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                ms.Write(buffer, 0, read);
        //            }
        //        }

        //        ms.Position = 0;
        //        using (WaveStream blockAlignedStream =
        //            new BlockAlignReductionStream(
        //                WaveFormatConversionStream.CreatePcmStream(
        //                    new Mp3FileReader(ms))))
        //        {
        //            using (_waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
        //            {
        //                _waveOut.Init(blockAlignedStream);
        //                _waveOut.Play();
        //                while (_waveOut.PlaybackState == PlaybackState.Playing)
        //                {
        //                    System.Threading.Thread.Sleep(100);
        //                }
        //            }
        //        }
        //    }
        //}

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
