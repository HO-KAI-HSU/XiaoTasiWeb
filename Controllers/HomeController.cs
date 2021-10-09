using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public class HomeController : Controller
    {
        private readonly IndexService _indexService;
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config, IndexService indexService)
        {
            _config = config;
            _indexService = indexService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Brand()
        {
            return View();
        }

        public IActionResult News()
        {
            return View();
        }

        public IActionResult MediaNews()
        {
            return View();
        }

        public IActionResult MediaNewsInfo()
        {
            return View();
        }

        public IActionResult Download()
        {
            return View();
        }

        public IActionResult Rule()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetIndexBannerList(int page, int limit)
        {
            IndexBannerListVo indexBannerListVo = new IndexBannerListVo();

            List<IndexBannerPojo> list = _indexService.getIndexBannerList();

            PageControl<IndexBannerPojo> pageControl = new PageControl<IndexBannerPojo>();

            List<IndexBannerPojo> listNew = pageControl.pageControl(page, limit, list);
            indexBannerListVo.success = 1;
            indexBannerListVo.count = pageControl.size;
            indexBannerListVo.page = page;
            indexBannerListVo.limit = limit;
            indexBannerListVo.indexBannerList = listNew;
            return Json(indexBannerListVo);
        }
    }
}
