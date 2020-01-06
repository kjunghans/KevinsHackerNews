using RestSharp;
using System;
using System.Collections.Generic;

namespace HackerNewsService
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IRestClient _client;
        private readonly IHackerNewsCache _cache;

        public HackerNewsService(IRestClient restClient, IHackerNewsCache cache)
        {
            _client = restClient;
            _cache = cache;
        }

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

    public class HackerNewsApiException : Exception
    {
        public HackerNewsApiException()
        {
        }

        public HackerNewsApiException(string message)
            : base(message)
        {
        }

        public HackerNewsApiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
