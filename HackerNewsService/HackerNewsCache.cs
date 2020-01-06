using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsService
{
    /// <summary>
    /// A concrete implementation of <see cref="IHackerNewsCache"/>.  This implementation
    /// uses a simple <see cref="MemoryCache"/> as the caching mechanism because it does
    /// not require a distributed cache.
    /// </summary>
    public class HackerNewsCache : IHackerNewsCache
    {
        private readonly IMemoryCache _cache;
        private readonly string storyIdsKey = "HackerNews.StoryIds";

        /// <summary>
        /// Constructor for <see cref="HackerNewsCache"/>.
        /// </summary>
        public HackerNewsCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// Gets a list of story id's from cache.
        /// </summary>
        /// <returns></returns>
        public List<int> GetLatestStoryIds()
        {
            return _cache.Get<List<int>>(storyIdsKey);
        }

        /// <summary>
        /// Gets a <see cref="HackerNewsItem"/> from the cache.
        /// </summary>
        /// <param name="id">Id of the news item to retrieve.</param>
        /// <returns></returns>
        public HackerNewsItem GetNewsItem(int id)
        {
            return _cache.Get<HackerNewsItem>(id);
        }

        /// <summary>
        /// Add a list of story id's to the cache.
        /// </summary>
        /// <param name="ids"></param>
        public void SetLatestStoryIds(List<int> ids)
        {
            _cache.Set<List<int>>(storyIdsKey, ids);
        }

        /// <summary>
        /// Adds a <see cref="HackerNewsItem"/> to the cache.
        /// </summary>
        /// <param name="newsItem"></param>
        public void SetNewsItem(HackerNewsItem newsItem)
        {
            _cache.Set<HackerNewsItem>(newsItem.Id, newsItem);
        }
    }
}
