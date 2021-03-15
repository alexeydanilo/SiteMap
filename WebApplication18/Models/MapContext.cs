using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication18.Models
{
    public class MapContext : DbContext
    {
        public DbSet<UrlMap> UrlMaps { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<Time> Times { get; set; }
        public DbSet<Quality> Qualities { get; set; }
    }

}
