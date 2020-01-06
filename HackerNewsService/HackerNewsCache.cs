using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsService
{
    public class HackerNewsCache : IHackerNewsCache
    {
        private readonly IMemoryCache _cache;
        private readonly string storyIdsKey = "HackerNews.StoryIds";

        public HackerNewsCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public List<int> GetLatestStoryIds()
        {
            return _cache.Get<List<int>>(storyIdsKey);
        }

        public HackerNewsItem GetNewsItem(int id)
        {
            return _cache.Get<HackerNewsItem>(id);
        }

        public void SetLatestStoryIds(List<int> ids)
        {
            _cache.Set<List<int>>(storyIdsKey, ids);
        }

        public void SetNewsItem(HackerNewsItem newsItem)
        {
            _cache.Set<HackerNewsItem>(newsItem.Id, newsItem);
        }
    }
}
