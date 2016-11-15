using HtmlAgilityPack;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using PodClient.Common;
using PodClient.Model;
using PodClient.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Linq;

namespace PodClient.ViewModel
{
    class MyPodcastViewModel: ViewModelBase
    {
        private NavigationService _navi;
        private NavigationService _navi2;

        public MyPodcastViewModel(NavigationService navi, NavigationService navi2)
        {
            this._navi = navi;
            this._navi2 = navi2;
            init();
        }

        public MyPodcastViewModel()
        {
            init();
        }
        
        private void init()
        {
            CallWebApi();
        }

        private async void CallWebApi()
        {
            var client = new HttpClient();
            var uri = "https://itunes.apple.com/search?term=podcast&media=podcast&entity=podcast&country=jp&lang=ja_jp";
            var result = await client.GetStringAsync(uri);

            string json = JsonConvert.SerializeObject(result);

            Content podcast = JsonConvert.DeserializeObject<Content>(result);

            Contents = podcast.results;
        }

        private bool CanPlayStopExecute()
        {
            return true;
        }

        private void selectNameExecute2()
        {

        }

        private DelegateCommand _selectNameCommand2;

        public DelegateCommand SelectNameCommand2

        {

            get

            {

                if (_selectNameCommand2 == null)

                    _selectNameCommand2 = new DelegateCommand(selectNameExecute2, CanPlayStopExecute);

                return _selectNameCommand2;

            }

        }


        private List<Content.Result> _contents = new List<Content.Result>();
        public List<Content.Result> Contents
        {
            get
            {
                return this._contents;
            }
            set
            {
                this._contents = value;
                RaisePropertyChanged("Contents");
            }
        }

        private Content.Result _selectedSample;

        public Content.Result SelectedSample

        {

            get { return _selectedSample; }

            set

            {

                _selectedSample = value;

                base.RaisePropertyChanged("SelectedSample");

            }

        }

        private string _id;
        public string Id
        {
            set
            {
                this._id = value;
            }
            get
            {
                return this._id;
                base.RaisePropertyChanged("Id");
            }
        }



        private void HandleNumberButtonPressed(int collectionId)
        {
            List<Content.Result> result = this.Contents.Where(r => r.collectionId == collectionId).ToList();

            string feedUrl = result[0].feedUrl;


            XNamespace atom = @"http://www.w3.org/2005/Atom";
            XNamespace itunes = @"http://www.itunes.com/dtds/podcast-1.0.dtd";
            XNamespace media = @"http://search.yahoo.com/mrss/";
            XNamespace dc = @"http://purl.org/dc/elements/1.1/";


            try
            {
                //GetFeed(feedUrl);

                //XElement spx = XElement.Load("http://feeds.rebuild.fm/rebuildfm");
                XElement spx = XElement.Load(feedUrl);

                Console.WriteLine("完了!");

                // チャンネル情報を取得します。
                XElement spxChannel = spx.Element("channel");

                Channel ch = new Channel();
                //ch.title = spxChannel.Element("title").Value;
                ch.title = result[0].trackName;
                //ch.description = spxChannel.Element("description").Value;
                

                //ch.thumbnail = spxChannel.Element(media + "thumbnail").Attribute("url").Value;
                ch.thumbnail = result[0].artworkUrl600;
                //ch.author = spxChannel.Element(itunes + "author").Value;
                ch.author = result[0].artistName;


                // 各話のデータを取得します。
                IEnumerable<XElement> spxItems = spxChannel.Elements("item");
                Console.WriteLine("\n各話の情報");

                ch.tracks = new List<Track>();

                int i = 0;
                foreach (var item in spxItems)
                {
                    i++;
                    Track track = new Track();

                    track.title = item.Element("title").Value;

                    Console.WriteLine(i);
                    Console.WriteLine(track.title);

                    string encodedContent = item.Element("description").Value;
                    var decodedContent = System.Net.WebUtility.HtmlDecode(encodedContent);
                    var html = new HtmlDocument();
                    html.LoadHtml(decodedContent);
                    string innerTxt = html.DocumentNode.InnerText;
                    StringReader inTxtStb = new StringReader(html.DocumentNode.InnerText);
                    track.description = inTxtStb.ReadLine();
                    
                    string pubDateStr = GetXmlValue(item, "pubDate", null);
                    CultureInfo cl = new CultureInfo("ja-JP");
                    DateTime pubDateDateTime = Convert.ToDateTime(pubDateStr, cl);
                    track.pubDate = pubDateDateTime.ToString("yyyy/MM/dd");
                    
                    track.duration = GetXmlValue(item, "duration", itunes);

                    track.enclosureUrl = item.Element("enclosure").Attribute("url").Value;


                    ch.tracks.Add(track);
                }

                this._navi.Navigate(new ChannelPage(_navi, _navi2, ch));
         


            }
            catch (Exception ex)
            {
                MessageBox.Show("RSSが読み取れません");
            };
          
        }

        // attributeが指定できないのでできるようにする
        private string GetXmlValue(XElement item, string elementName, XNamespace ns)
        {
            try
            {
                if (ns != null)
                {
                    return item.Element(ns + elementName).Value;
                }
                else
                {
                    return item.Element(elementName).Value;
                }
            }
            catch
            {
                // 取得できなかった場合からEmptyを返す
                return string.Empty;
            }
        }

        private void GetFeed(string feedUrl)
        {
            using (XmlReader rdr = XmlReader.Create(feedUrl))
            {
                SyndicationFeed feed = SyndicationFeed.Load(rdr);

                Console.WriteLine("title: " + feed.Title.Text);
            }
        }

        ICommand numberPressedCommand;
        public ICommand NumberPressedCommand
        {
            get
            {
                return numberPressedCommand ?? (numberPressedCommand = new DelegateCommand<int>(HandleNumberButtonPressed));
            }
        }
    }

}
