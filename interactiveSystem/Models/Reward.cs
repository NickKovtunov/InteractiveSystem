using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class Reward
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Photo { get; set; }
        public int? IdRew { get; set; }
        public int Type { get; set; }
        public int Order { get; set; }
    }
}