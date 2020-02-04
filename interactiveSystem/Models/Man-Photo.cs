using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class Man_Photo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string GalleryType { get; set; }
        public string FolderName { get; set; }
        public string OrgFolderName { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
    }
}