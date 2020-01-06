using System;
using Xunit;
using HackerNewsService;
using System.Collections.Generic;
using RestSharp;
using Moq;
using System.Net;
using Newtonsoft.Json;

namespace HackerNewsService.Test
{
    public class UnitTestHackerNewsService
    {
        public static IRestClient MockRestClient(HttpStatusCode httpStatusCode, List<int> storyList, HackerNewsItem newsItem )
        {
            var responseList = new Mock<IRestResponse<List<int>>>();
            responseList.Setup(_ => _.StatusCode).Returns(httpStatusCode);
            responseList.Setup(_ => _.Data).Returns(storyList);

            var responseNewsItem = new Mock<IRestResponse<HackerNewsItem>>();
            responseNewsItem.Setup(_ => _.StatusCode).Returns(httpStatusCode);
            responseNewsItem.Setup(_ => _.Data).Returns(newsItem);



            var mockIRestClient = new Mock<IRestClient>();
            mockIRestClient
              .Setup(x => x.Execute<List<int>>(It.IsAny<IRestRequest>()))
              .Returns(responseList.Object);
            mockIRestClient
               .Setup(x => x.Execute<HackerNewsItem>(It.IsAny<IRestRequest>()))
               .Returns(responseNewsItem.Object);

            return mockIRestClient.Object;
        }

        [Fact]
        public void TestGetNewStories()
        {
            List<int> storyList = new List<int>();
            int numStories = 10;
            for (int i = 0; i < numStories; i++)
                storyList.Add(i);
            HackerNewsItem newsItem = new HackerNewsItem() { Id = 1, Text = "My story", Type = "story" };
            var client = MockRestClient(HttpStatusCode.OK, storyList, newsItem);
            var cache = new HackerNewsCache();

            IHackerNewsService newsService = new HackerNewsService(client, cache);
            int startIndex = 0;
            int numItems = 10;
            List<HackerNewsItem> newsItems = newsService.GetLatestNews(startIndex, numItems);
            Assert.Equal(numItems, newsItems.Count);
        }

        [Fact]
        public void TestApiFails()
        {
            List<int> storyList = new List<int>();
            int numStories = 10;
            for (int i = 0; i < numStories; i++)
                storyList.Add(i);
            HackerNewsItem newsItem = new HackerNewsItem() { Id = 1, Text = "My story", Type = "story" };
            var client = MockRestClient(HttpStatusCode.BadRequest, storyList, newsItem);
            var cache = new HackerNewsCache();

            IHackerNewsService newsService = new HackerNewsService(client, cache);
            int startIndex = 0;
            int numItems = 10;
            Assert.Throws<HackerNewsApiException>(() => newsService.GetLatestNews(startIndex, numItems));
  
        }

        [Fact]
        public void TestGetLastFewItems()
        {
            List<int> storyList = new List<int>();
            int numStories = 13;

            for (int i = 0; i < numStories; i++)
                storyList.Add(i);
            HackerNewsItem newsItem = new HackerNewsItem() { Id = 1, Text = "My story", Type = "story" };
            var client = MockRestClient(HttpStatusCode.OK, storyList, newsItem);
            var cache = new HackerNewsCache();

            IHackerNewsService newsService = new HackerNewsService(client, cache);
            int pageNumber = 1;
            int pageSize = 10;
            int startIndex = pageNumber * pageSize;
            int expectedNumItems = numStories - pageSize;
            List<HackerNewsItem> newsItems = newsService.GetLatestNews(startIndex, pageSize);
            Assert.Equal(expectedNumItems, newsItems.Count);
            //Make sure we handle if the index is past the end of the list
            startIndex = numStories;
            newsItems = newsService.GetLatestNews(startIndex, pageSize);
            Assert.Empty(newsItems);


        }
    }
}