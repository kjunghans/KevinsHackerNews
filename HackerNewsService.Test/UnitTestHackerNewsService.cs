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
            for (int i = 0; i <= numStories; i++)
                storyList.Add(i);
            HackerNewsItem newsItem = new HackerNewsItem() { Id = 1, Text = "My story", Type = "story" };
            var client = MockRestClient(HttpStatusCode.OK, storyList, newsItem);

            IHackerNewsService newsService = new HackerNewsService(client);
            int startIndex = 0;
            int numItems = 10;
            List<HackerNewsItem> newsItems = newsService.GetLatestNews(startIndex, numItems);
            Assert.Equal(numItems, newsItems.Count);
        }
    }
}
