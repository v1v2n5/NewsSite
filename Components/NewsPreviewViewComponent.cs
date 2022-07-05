using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NewsSite.Models;
using NewsSite.Data;

namespace NewsSite.Components
{
    public class NewsPreviewViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(News news)
        {
            return View(news);
        }
    }
}
