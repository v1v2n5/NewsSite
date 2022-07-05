using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NewsSite.Models;
using NewsSite.Data;

namespace NewsSite.Components
{
    public class RecentNewsListViewComponent : ViewComponent
    {
        private ApplicationDbContext _context;
        public RecentNewsListViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.News.
                OrderByDescending(x => x.Time).
                Take(3));
        }
    }
}
