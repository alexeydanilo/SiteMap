using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication18.Models
{
    public class Quality
    {
        public int id { get; set; }
        public bool Assessment { get; set; }
        public int UrlMapId { get; set; }
        public UrlMap UrlMap { get; set; }


    }
}