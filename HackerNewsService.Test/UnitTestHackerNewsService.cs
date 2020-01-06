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
    /// <summary>
    /// A set of unit tests for the <see cref="HackerNewsService"/>
    /// </summary>
    public class UnitTestHackerNewsService
    {
        private static readonly string cacheIdentifier = "from-cache";

        /// <summary>
        /// Creates a mock rest client.
        /// </summary>
        /// <param name="httpStatusCode">The http status code to use for the test.</param>
        /// <param name="storyList">The story list to use for the test.</param>
        /// <param name="newsItem">The news item to use for the test.</param>
        /// <returns>Returns a mock rest client <see cref="IRestClient"/></returns>
        private static IRestClient MockRestClient(HttpStatusCode httpStatusCode, List<int> storyList, HackerNewsItem newsItem )
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

        /// <summary>
        /// Creates a mock cache.
        /// </summary>
        /// <param name="storyList">The story list to use for this test.</param>
        /// <returns>Returns a mock cache <see cref="IHackerNewsCache"/></returns>
        private static IHackerNewsCache MockCache(List<int> storyList)
        {
            var cache = new Mock<IHackerNewsCache>();
            List<HackerNewsItem> newsList = new List<HackerNewsItem>();
            for(int i = 0; i < storyList.Count; i++)
            {
                newsList.Add(new HackerNewsItem() { Id = i, By = cacheIdentifier });
            }

            cache
                .Setup(x => x.GetLatestStoryIds())
                .Returns(storyList);

            cache
                .Setup(x => x.GetNewsItem(It.IsAny<int>()))
                .Returns((int id) => { return newsList[id]; });

            return cache.Object;
        }


        /// <summary>
        /// Creates a story list with sequential id's starting from zero.
        /// </summary>
        /// <param name="numStories">The number of story id's to create.</param>
        /// <returns></returns>
        private List<int> GetStoryList(int numStories)
        {
            List<int> storyList = new List<int>();
            for (int i = 0; i < numStories; i++)
                storyList.Add(i);
            return storyList;
        }

        /// <summary>
        /// Validates we get a full list of news items from the beginning of the list 
        /// without using the cache.
        /// </summary>
        [Fact]
        public void TestGetNewStories()
        {
            int numStories = 10;
            List<int> storyList = GetStoryList(numStories);
            HackerNewsItem newsItem = new HackerNewsItem() { Id = 1, Text = "My story", Type = "story" };
            var client = MockRestClient(HttpStatusCode.OK, storyList, newsItem);
            var cache = new HackerNewsCache();

            IHackerNewsService newsService = new HackerNewsService(client, cache);
            int startIndex = 0;
            int numItems = 10;
            List<HackerNewsItem> newsItems = newsService.GetLatestNews(startIndex, numItems);
            Assert.Equal(numItems, newsItems.Count);
        }

        /// <summary>
        /// This test validates that and API error is handled correctly.
        /// </summary>
        [Fact]
        public void TestApiFails()
        {
            int numStories = 10;
            List<int> storyList = GetStoryList(numStories);
            HackerNewsItem newsItem = new HackerNewsItem() { Id = 1, Text = "My story", Type = "story" };
            var client = MockRestClient(HttpStatusCode.BadRequest, storyList, newsItem);
            var cache = new HackerNewsCache();

            IHackerNewsService newsService = new HackerNewsService(client, cache);
            int startIndex = 0;
            int numItems = 10;
            Assert.Throws<HackerNewsApiException>(() => newsService.GetLatestNews(startIndex, numItems));
  
        }

        /// <summary>
        /// This test validates that passing parameters that go past the end of the list work correctly.
        /// </summary>
        [Fact]
        public void TestGetLastFewItems()
        {
            int numStories = 13;
            List<int> storyList = GetStoryList(numStories);
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

        /// <summary>
        /// This test uses a mock cache that will alway return the item as a cached item.  This will bypass the 
        /// use of the mock RestClient to retrieve the items.
        /// </summary>
        [Fact]
        public void TestUsingCache()
        {
            int numStories = 13;
            List<int> storyList = GetStoryList(numStories);
            HackerNewsItem newsItem = new HackerNewsItem() { Id = 1, Text = "My story", Type = "story" };
            var client = MockRestClient(HttpStatusCode.OK, storyList, newsItem);
            var cache = MockCache(storyList);

            IHackerNewsService newsService = new HackerNewsService(client, cache);
            int pageNumber = 1;
            int pageSize = 10;
            int startIndex = pageNumber * pageSize;
            int expectedNumItems = numStories - pageSize;
            List<HackerNewsItem> newsItems = newsService.GetLatestNews(startIndex, pageSize);
            Assert.Equal(expectedNumItems, newsItems.Count);
            //Validate all of the items came from the cache
            foreach (var item in newsItems)
                Assert.Equal(cacheIdentifier, item.By);
            //Validate we got the correct items from the list
            for (int i = 0; i < expectedNumItems; i++)
                Assert.Equal(i + startIndex, newsItems[i].Id);
            //Validate we handle if the index is past the end of the list
            startIndex = numStories;
            newsItems = newsService.GetLatestNews(startIndex, pageSize);
            Assert.Empty(newsItems);
        }
    }
}
