using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace NewsSite.Models.ViewModels
{
    public class EditNewsViewModel
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Дата статьи")]
        public DateTime Time { get; set; }

        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        [Display(Name ="Описание")]
        public string PreviewContent { get; set; }

        [Required]
        [Display(Name ="Содержание")]
        public string Content { get; set; }

        [Required]
        [Display(Name ="Категория")]
        public string Category { get; set; }

        [Display(Name ="Теги")]
        public string Tags { get; set; }
        [Display(Name ="Основная картинка")]
        public IFormFile PreviewImage { get; set; }

        public EditNewsViewModel()
        {

        }
        public EditNewsViewModel(News news)
        {
            Time = news.Time;
            Title = news.Title;
            PreviewContent = news.PreviewContent;
            Content = news.Content;
            Category = news.Category?.Name ?? String.Empty;
            Tags = string.Join(',',news.Tags.Select(x => x.Tag.Name));

        }
    }
}
