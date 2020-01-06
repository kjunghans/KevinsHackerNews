using System;
using System.Collections.Generic;
using System.Text;

namespace HackerNewsService
{
    /// <summary>
    /// Provides an abstraction of a caching mechanism for the hacker news.
    /// </summary>
    public interface IHackerNewsCache
    {
        /// <summary>
        /// Retrieves a single news item from the cache.
        /// </summary>
        /// <param name="id">The id of the news item to retrieve.</param>
        /// <returns>Returns a single news item <see cref="HackerNewsItem"/></returns>
        HackerNewsItem GetNewsItem(int id);

        /// <summary>
        /// Adds a news item to the cache
        /// </summary>
        /// <param name="newsItem">The news item to add to the cache <see cref="HackerNewsItem"/>. </param>
        void SetNewsItem(HackerNewsItem newsItem);

        /// <summary>
        /// Retrieves a list of story id's from the cache.
        /// </summary>
        /// <returns>Returns a list of story id's.</returns>
        List<int> GetLatestStoryIds();

        /// <summary>
        /// Adds a list of story id's to the cache.
        /// </summary>
        /// <param name="ids">A list of story id's.</param>
        void SetLatestStoryIds(List<int> ids);

    }
}
