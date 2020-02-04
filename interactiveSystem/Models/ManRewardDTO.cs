using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class ManRewardDTO
    {
        public int Id { get; set; }
        public int ManId { get; set; }
        public string Man { get; set; }
        public string Reward { get; set; }
        public int RewardId { get; set; }
        public byte[] RewardPhoto { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}