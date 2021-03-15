using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication18.Models
{
    public class Time
    {
        public int id { get; set; }
        public int MaxTime { get; set; }
        public int MinTime { get; set; }
        public int UrlMapId { get; set; }
        public UrlMap UrlMap { get; set; }
    }
}