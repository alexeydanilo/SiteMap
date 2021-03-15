using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml.Linq;
using WebApplication18.Models;

namespace WebApplication18.Controllers
{
    public class HomeController : Controller
    {

        MapContext db = new MapContext();



        public ActionResult Index()
        {

            db.Database.Delete();
            db.SaveChanges();

            PageViewModel vm = new PageViewModel
            {
                UrlMaps = db.UrlMaps.ToList(),
                Maps = db.Maps.ToList(),
                Times = db.Times.ToList(),
                Qualities = db.Qualities.ToList()
            };

            return View(vm);

        }

        [HttpPost]
        public ActionResult Index(PageViewModel vm)
        {
            List<int> alltime = new List<int>();
            UrlMap urlmap = new UrlMap
            {
                Url = vm.url
            };

            db.UrlMaps.Add(urlmap);
            db.SaveChanges();

            Uri uri = new Uri(vm.url);
            string urlxml = uri.Scheme + "://" + uri.Host + "/sitemap.xml";

            IEnumerable<string> XmlMap = XmlReadMap(urlxml);
            if (XmlMap == null)

            {

                string data = GetHtmlPageText(vm.url);

                string pattern = "<a href=\"(.*?)\">";

                Regex regex = new Regex(pattern);
                MatchCollection matches = regex.Matches(data);

                foreach (Match matche in matches)
                {


                    string[] words = matche.Value.Split('"');
                    string[] check = vm.url.Split('/');
                    if (check.Length > 1)
                    {
                        if ((words[1].StartsWith("http") && (words[1].Contains(check[3]))) || (words[1].StartsWith("http") && (words[1].Contains(check[2]))) || (words[1].StartsWith("/") && !(words[1].EndsWith("."))))
                        {
                            Map smap = new Map()
                            {
                                SiteMap = words[1],

                                UrlMapId = urlmap.id
                            };

                            if (words[1].StartsWith("http"))
                            {
                                int time = PageResponce(words[1]);
                                smap.Time = time;
                            }
                            if (words[1].StartsWith("/"))
                            {
                                int time = PageResponce(vm.url + words[1]);
                                smap.Time = time;
                            }
                            alltime.Add(smap.Time);



                            db.Maps.Add(smap);
                            db.SaveChanges();


                        }


                    }
                }
            }
            else
            {
                foreach (var item in XmlMap)
                {
                    Map smap = new Map()
                    {
                        SiteMap = item,

                        UrlMapId = urlmap.id
                    };
                    int time = PageResponce(item);
                    smap.Time = time;
                    alltime.Add(smap.Time);
                    db.Maps.Add(smap);
                    db.SaveChanges();
                }

            }


            Time seconds = new Time
            {
                MaxTime = alltime.Max(),
                MinTime = alltime.Min(),
                UrlMapId = urlmap.id
            };
            Quality quality = new Quality
            {
                Assessment = true,
                UrlMapId = urlmap.id
            };
            db.Times.Add(seconds);
            db.Qualities.Add(quality);
            db.SaveChanges();

            PageViewModel pv = new PageViewModel
            {
                UrlMaps = db.UrlMaps.ToList(),
                Maps = db.Maps.ToList(),
                Times = db.Times.ToList(),
                Qualities = db.Qualities.ToList()

            };

            pv.Maps = db.Maps.OrderByDescending(x => x.Time);


            return View(pv);
        }


        [HttpPost]
        public ActionResult ChangeUserState(int id, string value)
        {


            Quality quality = db.Qualities.FirstOrDefault(x => x.id == id);

            if (value == "Хорошо")
            {
                quality.Assessment = true;
            }
            else
            {
                quality.Assessment = false;
            }
            quality.id = id;
            db.SaveChanges();

            return Json(new { result = "OK" });



        }
        [HttpPost]
        public JsonResult NewChart()
        {
            List<object> iData = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Maps", System.Type.GetType("System.String"));
            dt.Columns.Add("Time", System.Type.GetType("System.Int32"));
            PageViewModel pv = new PageViewModel
            {
                UrlMaps = db.UrlMaps.ToList(),
                Maps = db.Maps.ToList()
            };

            DataRow dr;

            for (int i = 0; i < pv.Maps.Count(); i++)
            {
                if (i == 0) { dt.Clear(); }

                if (pv.UrlMaps.Last().id == pv.Maps.ElementAt(i).UrlMapId)
                {
                    dr = dt.NewRow();
                    dr["Maps"] = pv.Maps.ElementAt(i).SiteMap;
                    dr["Time"] = pv.Maps.ElementAt(i).Time;
                    dt.Rows.Add(dr);
                }
            }


            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }

            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        public static string GetHtmlPageText(string url)
        {
            var Webget = new HtmlWeb();
            var doc = Webget.Load(url);

            string website = doc.Text;

            return website;

        }

        public IEnumerable<string> XmlReadMap(string url)
        {
            try
            {

                XDocument xDoc = XDocument.Load(url);
                XName xName = XName.Get("loc", xDoc.Root.Name.Namespace.NamespaceName);
                IEnumerable<string> urls = xDoc.Root.Elements().Select(x => x.Element(xName).Value).ToList();
                return urls;
            }
            catch
            {
                return null;
            }
        }




        public static int PageResponce(string url)
        {
            Stopwatch time = new Stopwatch();
            if (!(url.StartsWith("https://vk")))
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36";
                time.Start();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                time.Stop();
            }

            TimeSpan timeSpan = time.Elapsed;

            return timeSpan.Milliseconds;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
