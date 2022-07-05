using System.Collections.Generic;
using NewsSite.Models;

namespace NewsSite.Models.ViewModels
{
    public class DetailNewsViewModel
    {
        public IEnumerable<News> RelatedNews { get; set; }
        public News CurrentNews { get; set; }
    }
}
