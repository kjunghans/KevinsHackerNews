using System;
using System.Collections.Generic;
using System.Text;

namespace HackerNewsService
{
    public interface IHackerNewsService
    {
        List<HackerNewsItem> GetLatestNews(int startIndex, int numItems);
        HackerNewsItem GetNewsItem(int id);
    }
}
