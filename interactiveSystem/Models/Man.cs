using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class Man
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Photo { get; set; }
        public int? IdPersonal { get; set; }
        public string Position { get; set; }
        public DateTime? Birthday { get; set; }
    }
}