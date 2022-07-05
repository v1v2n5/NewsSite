using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsSite.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NewsSite.Data;
using Microsoft.EntityFrameworkCore;

namespace NewsSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var news = _context.News
                .OrderByDescending(x => x.Time)
                .ThenByDescending(x => x.Id)
                .Take(5)
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .ThenInclude(x => x.Tag);
            return View(news);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
