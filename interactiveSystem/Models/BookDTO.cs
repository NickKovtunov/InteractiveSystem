using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class BookDTO
    {
        public string FolderName { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public int FileNameWithoutExtension { get; set; }
    }
}