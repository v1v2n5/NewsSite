using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NewsSite.Models;
using NewsSite.Data;

namespace NewsSite.Components
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private ApplicationDbContext _context;
        public CategoryMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.Categories
                .Select(x => x)
                .Distinct()
                .OrderBy(x => x.Name));
        }
    }
}
