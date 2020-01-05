using System;
using Xunit;
using HackerNewsService;
using System.Collections.Generic;
using RestSharp;

namespace HackerNewsService.Test
{
    public class IntegrationTestHackerNewsService
    {
        [Fact]
        public void TestGetNewStories()
        {
            var client = new RestClient("https://hacker-news.firebaseio.com/v0");

            IHackerNewsService newsService = new HackerNewsService(client);
            int startIndex = 0;
            int numItems = 10;
            List<HackerNewsItem> newsItems = newsService.GetLatestNews(startIndex, numItems);
            Assert.Equal(numItems, newsItems.Count);
        }
    }
}
