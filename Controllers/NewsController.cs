using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using xiaotasi.Models;
using xiaotasi.Pojo;
using xiaotasi.Service;
using xiaotasi.Vo;

namespace xiaotasi.Controllers
{

    public class NewsController : Controller
    {
        private readonly ILogger<NewsController> _logger;
        private readonly IConfiguration _config;
        private readonly NewsService _newsService;

        public NewsController(ILogger<NewsController> logger, IConfiguration config, NewsService newsService)
        {
            _logger = logger;
            _config = config;
            _newsService = newsService;
        }

        [HttpPost]
        public async Task<IActionResult> GetLatestNewsList(int page, int limit)
        {
            LatestNewsListVo latestNewsListVo = new LatestNewsListVo();

            List<NewsPojo> newList = await _newsService.getLatestNewsList();

            PageControl<NewsPojo> pageControl = new PageControl<NewsPojo>();

            List<NewsPojo> listNew = pageControl.pageControl(page, limit, newList);
            latestNewsListVo.success = 1;
            latestNewsListVo.count = pageControl.size;
            latestNewsListVo.page = page;
            latestNewsListVo.limit = limit;
            latestNewsListVo.lateNewsList = listNew;
            return Json(latestNewsListVo);
        }

        [HttpPost]
        public async Task<IActionResult> GetMediaNewsList(int page, int limit)
        {
            MediaNewsListVo mediaNewsListVo = new MediaNewsListVo();

            List<MediaNewsPojo> newList = await _newsService.getMediaNewsList();

            PageControl<MediaNewsPojo> pageControl = new PageControl<MediaNewsPojo>();

            List<MediaNewsPojo> listNew = pageControl.pageControl(page, limit, newList);
            mediaNewsListVo.success = 1;
            mediaNewsListVo.count = pageControl.size;
            mediaNewsListVo.page = page;
            mediaNewsListVo.limit = limit;
            mediaNewsListVo.mediaNewsList = listNew;
            return Json(mediaNewsListVo);
        }
    }
}
