using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace interactiveSystem.Models
{
    public class ISContext : DbContext
    {
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<Man> Men { get; set; }
        public DbSet<Man_Enterprise> Man_Enterprises { get; set; }
        public DbSet<Man_Reward> Man_Rewards { get; set; }
        public DbSet<Man_Photo> Man_Photos { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<Leadership> Leaderships { get; set; }
        public DbSet<NewsGallery> NewsGalleries { get; set; }
    }
}