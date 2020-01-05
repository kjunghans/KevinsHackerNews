using RestSharp;
using System;
using System.Collections.Generic;

namespace HackerNewsService
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IRestClient client;

        public HackerNewsService(IRestClient restClient)
        {
            client = restClient;
        }


        public List<HackerNewsItem> GetLatestNews(int startIndex, int numItems)
        {
            List<HackerNewsItem> newsItems = new List<HackerNewsItem>();
            var request = new RestRequest("topstories.json?print=pretty}", Method.GET);
            var response = client.Execute<List<int>>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new HackerNewsApiException($"The Hacker News API returned a status code of {response.StatusCode}");

            if (response.Data == null || response.Data.Count == 0)
                return newsItems;
            for(int i = startIndex; i < startIndex + numItems; i++)
            {
                var newsItem = GetNewsItem(response.Data[i]);
                if (newsItem != null)
                    newsItems.Add(newsItem);
            }

            return newsItems;
        }

        public HackerNewsItem GetNewsItem(int id)
        {
            HackerNewsItem newsItem = null;
            var request = new RestRequest("item/{id}.json?print=pretty}", Method.GET);
            request.AddUrlSegment("id", id);
            var response = client.Execute<HackerNewsItem>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new HackerNewsApiException($"The Hacker News API returned a status code of {response.StatusCode}");

            if (response.Data == null)
                return newsItem;

            newsItem = response.Data;
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
