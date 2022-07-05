using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NewsSite.Services
{
    
    //Сервис для создания общей навигационной цепочки на основе MVC
    
    public class BreadcrumbService : IViewContextAware
    {
        IList<Breadcrumb> breadcrumbs;

        public void Contextualize(ViewContext viewContext)
        {
            breadcrumbs = new List<Breadcrumb>();

            string area = $"{viewContext.RouteData.Values["area"]}";
            string controller = $"{viewContext.RouteData.Values["controller"]}";
            string action = $"{viewContext.RouteData.Values["action"]}";
            object id = viewContext.RouteData.Values["id"];
            string title = $"{viewContext.ViewData["Title"]}";

            breadcrumbs.Add(new Breadcrumb(area, controller, action, title, id));

            if (!string.Equals(action, "index", StringComparison.OrdinalIgnoreCase))
            {
                breadcrumbs.Insert(0, new Breadcrumb(area, controller, "index", title));
            }
        }

        public IList<Breadcrumb> GetBreadcrumbs()
        {
            return breadcrumbs;
        }
    }

    public class Breadcrumb
    {
        public Breadcrumb(string area, string controller, string action, string title, object id) : this(area, controller, action, title)
        {
            Id = id;
        }

        public Breadcrumb(string area, string controller, string action, string title)
        {
            Area = area;
            Controller = controller;
            Action = action;

            if (string.IsNullOrWhiteSpace(title))
            {
                Title = Regex.Replace(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase) ? controller : action), "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
            }
            else
            {
                Title = title;
            }
        }

        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public object Id { get; set; }
        public string Title { get; set; }
    }
}