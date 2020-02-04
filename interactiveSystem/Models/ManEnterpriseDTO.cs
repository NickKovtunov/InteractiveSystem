using System;

namespace interactiveSystem.Models
{
    public class ManEnterpriseDTO
    {
        public int Id { get; set; }
        public int ManId { get; set; }
        public string Man { get; set; }
        public string Enterprise { get; set; }
        public string Description { get; set; }
        public bool IsLeader { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}