using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using xiaotasi.Models;

namespace xiaotasi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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

        public IActionResult Download()
        {
            return View();
        }

        public IActionResult Rule()
        {
            return View();
        }
    }
}
