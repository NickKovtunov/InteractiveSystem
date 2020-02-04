using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class Man_Reward
    {
        public int Id { get; set; }
        public int ManId { get; set; }
        public int RewardId { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}