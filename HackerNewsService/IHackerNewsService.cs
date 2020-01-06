using System.Collections.Generic;

namespace HackerNewsService
{
    /// <summary>
    /// Provides an abstraction of a service for getting Hacker News
    /// </summary>
    public interface IHackerNewsService
    {
        /// <summary>
        /// Retrieves the latest news.  Uses a paging mechanism to retrieve a subset of the news.
        /// </summary>
        /// <param name="startIndex">A zero based index to start retrieving the news from.</param>
        /// <param name="numItems">The number of items to retrieve.</param>
        /// <returns>A list of news items<see cref="HackerNewsItem"/>. </returns>
        List<HackerNewsItem> GetLatestNews(int startIndex, int numItems);
        /// <summary>
        /// Retrieves a single news item <see cref="HackerNewsItem"/> based on the id of news item.
        /// </summary>
        /// <param name="id">The id of the news item.</param>
        /// <returns>Returns a single news item <see cref="HackerNewsItem"/> </returns>
        HackerNewsItem GetNewsItem(int id);
    }
}
