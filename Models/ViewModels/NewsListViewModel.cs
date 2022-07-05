using NewsSite.Models;
using System.Collections.Generic;

namespace NewsSite.Models.ViewModels
{
    public class NewsListViewModel
    {
        public IEnumerable<News> News { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public string Category { get; set; }

        public string Tag { get; set; }
    }
}
