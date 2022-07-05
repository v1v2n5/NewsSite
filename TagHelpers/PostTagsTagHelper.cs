using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using NewsSite.Models;
using System.Text;
using System;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Policy;

namespace NewsSite.TagHelpers
{
    [HtmlTargetElement("post-tags")]
    public class PostTagsTagHelper : TagHelper
    {
        public IEnumerable<NewsTags> Elements { get; set; }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var Current = ViewContext.HttpContext;

            var BaseUrl = $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";


            output.TagName = "ul";
            output.Attributes.SetAttribute("class", "post-tags");
            StringBuilder listContent = new();
            listContent.Append($"<li><i class=\"fa fa-tags\"></i></li>");
            var enumerator = Elements.GetEnumerator();
            enumerator.MoveNext();
            while (true)
            {
                var tag = enumerator.Current.Tag.Name;
                listContent.Append($"<li><a href=\"{BaseUrl}/News/All?tag={tag}\">{tag}</a>");
                if (enumerator.MoveNext())
                    listContent.Append(",&nbsp;</li>");
                else
                {
                    listContent.Append("</li>");
                    break;
                }
            }
            output.Content.SetHtmlContent(listContent.ToString());
        }
    }
}
