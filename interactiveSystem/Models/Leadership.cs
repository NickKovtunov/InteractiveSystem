using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class Leadership
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public byte[] Photo { get; set; }
        public int Order { get; set; }
    }
}