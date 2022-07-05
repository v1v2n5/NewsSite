using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace NewsSite.Models.ViewModels
{
    public class ActivityDirectionsViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Display(Name = "Основная картинка")]
        public IFormFile PreviewImage { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string PreviewContent { get; set; }

        [Required]
        [Display(Name = "Содержание")]
        public string Content { get; set; }


        public ActivityDirectionsViewModel()
        {

        }

        public ActivityDirectionsViewModel(ActivityDirections activityDirections)
        {
            this.Id = activityDirections.Id;
            this.Title = activityDirections.Title;
            this.PreviewContent = activityDirections.PreviewContent;
            this.Content = activityDirections.Content;
        }
    }
}
