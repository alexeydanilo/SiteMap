using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication18.Models
{
    public class Map
    {
        public int id { get; set; }

        public string SiteMap { get; set; }

        public int Time { get; set; }

        public int UrlMapId { get; set; }
        
        public UrlMap UrlMap { get; set; }

}

}