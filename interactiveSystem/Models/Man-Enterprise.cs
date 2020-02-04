using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class Man_Enterprise
    {
        public int Id { get; set; }
        public int ManId { get; set; }
        public int EnterpriseId { get; set; }
        public string Description { get; set; }
        public bool IsLeader { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
    }
}