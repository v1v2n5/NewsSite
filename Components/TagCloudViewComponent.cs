using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NewsSite.Models;
using NewsSite.Data;

namespace NewsSite.Components
{
    public class TagCloudViewComponent : ViewComponent
    {
        private ApplicationDbContext _context;
        public TagCloudViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.Tags
                .Distinct()
                .OrderBy(x => x.Name));
        }
    }
}
