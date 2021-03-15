using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication18.Models
{
    public class PageViewModel
    {
        public IEnumerable<Map> Maps { get; set; }

        public IEnumerable<UrlMap> UrlMaps { get; set; }

        public IEnumerable<Time> Times { get; set; }

        public IEnumerable<Quality> Qualities { get; set; }
        public string url { get; set; }
    }
}