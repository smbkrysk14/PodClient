using HtmlAgilityPack;
using Newtonsoft.Json;
using PodClient.Common;
using PodClient.Model;
using PodClient.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace PodClient.ViewModel
{
    public class SearchResultViewModel:ViewModelBase
    {
        private NavigationService _navi;
        private NavigationService _navi2;

        public SearchResultViewModel(NavigationService navi, NavigationService navi2, string keyWord)
        {
            this._navi = navi;
            this._navi2 = navi2;
            init(keyWord);
        }

        private void init(string keyWord)
        {
            CallWebApi(keyWord);
        }

        private async void CallWebApi(string keyWord)
        {
            var client = new HttpClient();
            var uri1 = "https://itunes.apple.com/search?term=";
            var uri2 = "&media=podcast&entity=podcast&country=jp&lang=ja_jp";
            var uri = uri1 + keyWord + uri2;
            var result = await client.GetStringAsync(uri);

            string json = JsonConvert.SerializeObject(result);

            Content podcast = JsonConvert.DeserializeObject<Content>(result);

            Contents = podcast.results;
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

        private void AddMyList(int collectionId)
        {
            List<Content.Result> result = this.Contents.Where(r => r.collectionId == collectionId).ToList();

            MyList.myList.Add(result[0]);
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

        ICommand numberPressedCommand;
        public ICommand NumberPressedCommand
        {
            get
            {
                return numberPressedCommand ?? (numberPressedCommand = new DelegateCommand<int>(HandleNumberButtonPressed));
            }
        }

        private ICommand addMyListCommad;
        public ICommand AddMyListCommand
        {
            get
            {
                return addMyListCommad ?? (addMyListCommad = new DelegateCommand<int>(AddMyList));
            }
        }
    }
}
