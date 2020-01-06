using System;
using System.Collections.Generic;
using System.Text;

namespace HackerNewsService
{
    public interface IHackerNewsCache
    {
        HackerNewsItem GetNewsItem(int id);
        void SetNewsItem(HackerNewsItem newsItem);
        List<int> GetLatestStoryIds();
        void SetLatestStoryIds(List<int> ids);

    }
}
