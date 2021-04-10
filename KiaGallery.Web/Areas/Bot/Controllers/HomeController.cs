using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Bot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web;

namespace KiaGallery.Web.Areas.Bot.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "admin, botMain")]
        public ActionResult Index()
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                ViewBag.UserCount = db.BotUserData.Count();
                ViewBag.TodayUserCount = db.BotUserData.Where(x => x.CreatedDate >= DateTime.Today).Count();
                ViewBag.MessageCount = db.BotMessage.Count();
                ViewBag.TodayMessageCount = db.BotMessage.Where(x => x.CreatedDate >= DateTime.Today).Count();
                ViewBag.TodayUnknownMessageCount = db.BotMessage.Where(x => x.CreatedDate >= DateTime.Today && x.Unknown == true).Count();

            }
            return View();
        }

        [Authorize(Roles = "admin, botMain")]
        public ActionResult Broadcast()
        {
            return View();
        }

        [Authorize(Roles = "admin, botMain")]
        public ActionResult News()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, botMain")]
        public ContentResult UploadFiles()
        {
            var result = new List<UploadFilesResult>();

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;
                string serverPath = "C:/Files/KiaGalleryBot/Broadcast";
                string savedFileName = Path.Combine(serverPath, Path.GetFileName(hpf.FileName));

                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
                hpf.SaveAs(savedFileName);

                result.Add(new UploadFilesResult()
                {
                    Name = hpf.FileName,
                    Length = hpf.ContentLength,
                    Type = hpf.ContentType
                });
            }

            return Content("{\"name\":\"" + result[0].Name + "\"}", "application/json");
        }

        [Authorize(Roles = "admin, botMain")]
        public ContentResult SaveBroadcast(string Message, string FileName)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                db.BotBroadcast.Add(new Broadcast()
                {
                    Text = Message,
                    FileName = string.IsNullOrEmpty(FileName) ? null : FileName,
                    Sended = false,
                    CreatedDate = DateTime.Now
                });
                db.SaveChanges();
            }

            return Content("{\"result\":" + true + "}", "application/json");
        }

        [Authorize(Roles = "admin, botMain")]
        public ContentResult SaveNews(string Message, string FileName, int Credit)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                db.BotNews.Add(new BotNews()
                {
                    Text = Message,
                    FileName = string.IsNullOrEmpty(FileName) ? null : FileName,
                    CreatedDate = DateTime.Now,
                    ExpiredDate = DateTime.Now.AddDays(Credit)
                });
                db.SaveChanges();
            }

            return Content("{\"result\":" + true + "}", "application/json");
        }

        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetNews(string text, string fromDate, string toDate, int page, int count)
        {
            List<BotNews> data = new List<BotNews>();
            int newsCount = 0;

            using (KiaGalleryContext db = new KiaGalleryContext())
            {

                var query = db.BotNews.Select(x => x);

                if (!string.IsNullOrEmpty(text))
                    query = query.Where(x => x.Text.Contains(text));

                DateTime? From = Common.DateUtility.GetDateTime(fromDate);
                if (From != null)
                    query = query.Where(x => x.CreatedDate >= From);

                DateTime? To = Common.DateUtility.GetDateTime(toDate);
                if (To != null)
                {
                    To = To.GetValueOrDefault().AddDays(1);
                    query = query.Where(x => x.CreatedDate <= To);
                }

                newsCount = query.Count();
                data = query.OrderByDescending(x => x.Id).Skip(page * count).Take(count).ToList();
            }


            var listData = data.Select(x => new
            {
                x.Id,
                x.Text,
                x.FileName,
                x.FileId,
                CreatedDate = Common.DateUtility.GetPersianDateTime(x.CreatedDate.GetValueOrDefault()),
                ExpiredDate = Common.DateUtility.GetPersianDateTime(x.ExpiredDate.GetValueOrDefault())
            });

            var response = new Response()
            {
                status = 200,
                data = new
                {
                    list = listData,
                    newsCount = newsCount,
                    pageCount = Math.Ceiling((double)newsCount / count)
                }
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetBroadcast(string text, string fromDate, string toDate, int page, int count)
        {
            List<Broadcast> data = new List<Broadcast>();
            int broadcastCount = 0;

            using (KiaGalleryContext db = new KiaGalleryContext())
            {

                var query = db.BotBroadcast.Select(x => x);

                if (!string.IsNullOrEmpty(text))
                    query = query.Where(x => x.Text.Contains(text));

                DateTime? From = Common.DateUtility.GetDateTime(fromDate);
                if (From != null)
                    query = query.Where(x => x.CreatedDate >= From);

                DateTime? To = Common.DateUtility.GetDateTime(toDate);
                if (To != null)
                {
                    To = To.GetValueOrDefault().AddDays(1);
                    query = query.Where(x => x.CreatedDate <= To);
                }

                broadcastCount = query.Count();
                data = query.OrderByDescending(x => x.Id).Skip(page * count).Take(count).ToList();
            }

            var listData = data.Select(x => new
            {
                x.Id,
                x.Text,
                x.FileName,
                x.FileId,
                CreatedDate = Common.DateUtility.GetPersianDateTime(x.CreatedDate.GetValueOrDefault())
            });

            var response = new Response()
            {
                status = 200,
                data = new
                {
                    list = listData,
                    broadcastCount = broadcastCount,
                    pageCount = Math.Ceiling((double)broadcastCount / count)
                }
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        

    }


    public class UploadFilesResult
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public string Type { get; set; }
    }
}