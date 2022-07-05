using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NewsSite.Models;
using NewsSite.Data;

namespace NewsSite.Components
{
    public class NewsCardViewComponent : ViewComponent
    {
        private ApplicationDbContext _context;
        public NewsCardViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke(News news)
        {
            return View(news);
        }
    }
}