using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodClient.Model
{
    public class Channel
    {
        public string title { get; set; }
        public string description { get; set; }
        public string thumbnail { get; set; }
        public string author { get; set; }
        public List<Track> tracks { get; set; }
    }

    public class Track
    {
        public string title { get; set; }
        public string description { get; set; }
        public string pubDate { get; set; }
        public string duration { get; set; }
        public string enclosureUrl { get; set; }
    }
}
