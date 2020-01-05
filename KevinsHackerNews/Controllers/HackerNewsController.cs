using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HackerNewsService;

namespace KevinsHackerNews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNewsService _newsService;
        public HackerNewsController(IHackerNewsService newsService)
        {
            _newsService = newsService;
        }

        // GET: api/HackerNews
        [HttpGet("GetLatestStories/{startIndex}/{numItems}")]
        public IEnumerable<HackerNewsItem> GetLatestStories(int startIndex, int numItems)
        {
            return _newsService.GetLatestNews(startIndex, numItems);
        }


    }
}
