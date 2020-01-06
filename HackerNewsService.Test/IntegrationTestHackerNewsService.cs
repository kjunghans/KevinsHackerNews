using System;
using Xunit;
using HackerNewsService;
using System.Collections.Generic;
using RestSharp;
using System.Diagnostics;

namespace HackerNewsService.Test
{
    public class IntegrationTestHackerNewsService
    {
        [Fact]
        public void TestGetNewStories()
        {
            var client = new RestClient("https://hacker-news.firebaseio.com/v0");
            var cache = new HackerNewsCache();

            IHackerNewsService newsService = new HackerNewsService(client, cache);
            int startIndex = 0;
            int numItems = 10;
            Stopwatch stopWatchNoCache = new Stopwatch();
            stopWatchNoCache.Start();
            List<HackerNewsItem> newsItems = newsService.GetLatestNews(startIndex, numItems);
            stopWatchNoCache.Stop();
            Assert.Equal(numItems, newsItems.Count);
            TimeSpan tsWithoutCache = stopWatchNoCache.Elapsed;

            //Lets see if using the cache helps
            Stopwatch stopWatchWithCache = new Stopwatch();
            stopWatchWithCache.Start();
            newsItems = newsService.GetLatestNews(startIndex, numItems);
            stopWatchWithCache.Stop();
            Assert.Equal(numItems, newsItems.Count);
            TimeSpan tsWithCache = stopWatchWithCache.Elapsed;
            int lagTimeInMs = 500;  //Should at least be a half second faster
            Assert.True(tsWithCache.TotalMilliseconds < tsWithoutCache.TotalMilliseconds - lagTimeInMs);

        }
    }
}
