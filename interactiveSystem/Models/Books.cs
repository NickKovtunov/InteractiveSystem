using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace interactiveSystem.Models
{
    public class Books
    {
        private Books()
        { }

        public static Books Create(string folderName)
        {
            var mth = Regex.Match(folderName, @"(?<date>\d{2}\-\d{2}\-\d{4})\s+(?<title>.+)");
            if (mth.Success)
            {
                return new Books
                {
                    Title = mth.Groups["title"].Value,
                    OccurredOn = DateTime.Parse(mth.Groups["date"].Value),
                    FolderName = Path.GetFileName(folderName),
                    FirstFileName = Path.GetFileName(Directory.GetFiles(folderName).FirstOrDefault()),
                    //OccurredOnForLink = DateTime.Parse(mth.Groups["date"].Value).ToString("dd-MM-yyyy"),
                    //OccurredOnForShow = DateTime.Parse(mth.Groups["date"].Value).ToString("D")
                };
            }
            return null;
        }
        
        public string Title { get; private set; }
        public DateTime OccurredOn { get; private set; }
        public string FolderName { get; private set; }
        //public string OccurredOnForLink { get; private set; }
        //public string OccurredOnForShow { get; private set; }
        public string FirstFileName { get; private set; }
        //public string LastFolderName { get; set; }
    }
}