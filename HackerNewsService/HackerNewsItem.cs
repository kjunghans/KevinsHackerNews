using System;
using System.Collections.Generic;
using System.Text;

namespace HackerNewsService
{
    public class HackerNewsItem
    { 
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public bool Dead { get; set; }
        public int Parent { get; set; }
        public string Poll { get; set; }
        public List<int> Kids { get; set; }
        public string Url { get; set; }
        public int Score { get; set; }
        public string Title { get; set; }
        public List<int> Parts { get; set; }
        public int Descendants { get; set; }

    }

    
}
