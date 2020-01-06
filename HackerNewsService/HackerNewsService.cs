using RestSharp;
using System;
using System.Collections.Generic;

namespace HackerNewsService
{
    /// <summary>
    /// A concrete implementation of the <see cref="IHackerNewsService"/> that uses the 
    /// Hacker News API [https://github.com/HackerNews/API].
    /// </summary>
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IRestClient _client;
        private readonly IHackerNewsCache _cache;

        /// <summary>
        /// Constructs a new instance of <see cref="HackerNewsService"/>.
        /// </summary>
        /// <param name="restClient">A concrete implementation of <see cref="IRestClient"/> used to communicate with Hacker News API.</param>
        /// <param name="cache">A concrete implementation of <see cref="IHackerNewsCache"/> used for caching news items.</param>
        public HackerNewsService(IRestClient restClient, IHackerNewsCache cache)
        {
            _client = restClient;
            _cache = cache;
        }

        /// <summary>
        /// Retrieves a list of id's for the latest stories from the Hacker News API.
        /// </summary>
        /// <returns>Returns a list of the latest story id's.</returns>
        private List<int> GetNewsListIds()
        {
            List<int> idList = _cache.GetLatestStoryIds();
            if (idList != null)
                return idList;
            var request = new RestRequest("topstories.json?print=pretty}", Method.GET);
            var response = _client.Execute<List<int>>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new HackerNewsApiException($"The Hacker News API returned a status code of {response.StatusCode}");

            if (response.Data == null || response.Data.Count == 0)
                return null;
            _cache.SetLatestStoryIds(response.Data);

            return response.Data;
        }

        /// <summary>
        /// Retrieves the latest news stories from the Hacker News API.
        /// </summary>
        /// <param name="startIndex">A zero based index on where to start retrieving the news items from.</param>
        /// <param name="numItems">The number of items to retrieve.</param>
        /// <returns>A list of news items <see cref="HackerNewsItem"/></returns>
        public List<HackerNewsItem> GetLatestNews(int startIndex, int numItems)
        {
            List<HackerNewsItem> newsItems = new List<HackerNewsItem>();
            List<int> idList = GetNewsListIds();
            if (idList == null)
                return newsItems;
            int lastItem = startIndex + numItems;
            if (lastItem > idList.Count)
                lastItem = idList.Count;
            for(int i = startIndex; i < lastItem; i++)
            {
                var newsItem = GetNewsItem(idList[i]);
                if (newsItem != null)
                    newsItems.Add(newsItem);
            }

            return newsItems;
        }

        /// <summary>
        /// Retrieves a single news item from the Hacker News API.
        /// </summary>
        /// <param name="id">The id of the story.</param>
        /// <returns>Returns a news item <see cref="HackerNewsItem"/> that matches the id.</returns>
        public HackerNewsItem GetNewsItem(int id)
        {
            HackerNewsItem newsItem = _cache.GetNewsItem(id);
            if (newsItem != null)
                return newsItem;
            var request = new RestRequest("item/{id}.json?print=pretty}", Method.GET);
            request.AddUrlSegment("id", id);
            var response = _client.Execute<HackerNewsItem>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new HackerNewsApiException($"The Hacker News API returned a status code of {response.StatusCode}");

            if (response.Data == null)
                return newsItem;

            newsItem = response.Data;
            _cache.SetNewsItem(newsItem);
            return newsItem;

        }
    }

    /// <summary>
    /// Exception thrown if there is a problem reaching the Hacker News API.
    /// </summary>
    public class HackerNewsApiException : Exception
    {
        /// <summary>
        /// Constructor for <see cref="HackerNewsApiException"/>
        /// </summary>
        public HackerNewsApiException()
        {
        }

        /// <summary>
        /// Constructor for <see cref="HackerNewsApiException"/>
        /// </summary>
        /// <param name="message">Message for exception</param>
        public HackerNewsApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor for <see cref="HackerNewsApiException"/>
        /// </summary>
        /// <param name="message">Message for exception</param>
        /// <param name="inner">Inner exception</param>
        public HackerNewsApiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
