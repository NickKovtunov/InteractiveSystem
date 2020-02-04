using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class NewsGallery
    {
        [Key]
        public int Id { get; set; }
        public string Preview { get; set; }
        public string PublishingDate { get; set; }
        public string ImagePreview { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}