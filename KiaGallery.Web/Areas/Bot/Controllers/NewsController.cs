using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Bot;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Areas.Bot.Controllers
{
    public class NewsController : Controller
    {
        [Authorize(Roles = "admin, botMain")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetData(int page, int count)
        {
            Response response;
            try
            {
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var query = db.BotNews.Select(x => new
                    {
                        x.Id,
                        x.Type,
                        x.Text,
                        x.TextFa,
                        x.FileName
                    });

                    var data = query.OrderByDescending(x => x.Id).Skip(page * count).Take(count).ToList();
                    int dataCount = query.Count();

                    var listData = data.Select(x => new
                    {
                        id = x.Id,
                        type = Enums.GetTitle(x.Type.GetValueOrDefault()),
                        text = x.Text,
                        textFa = x.TextFa,
                        fileName = x.FileName
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = listData,
                            pageCount = Math.Ceiling((double)dataCount / count),
                            count = dataCount,
                            pageNo = page + 1
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult Save(int? id, BotType type, string text, string textFa, string fileName, string fileId)
        {
            Response response;
            try
            {
                var message = "";
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    if (id != null && id > 0) // Edit News
                    {
                        var item = db.BotNews.First(x => x.Id == id);

                        if (!string.IsNullOrEmpty(item.FileName) && item.FileName != fileName)
                            System.IO.File.Delete(Path.Combine( "\\News\\", item.FileName));

                        item.Type = type;
                        item.Text = text;
                        item.TextFa = textFa;
                        item.FileName = fileName;
                        item.FileId = fileId;
                        message = "Your record has been updated.";
                    }
                    else // New News
                    {
                        var item = new BotNews()
                        {
                            Type = type,
                            Text = text,
                            TextFa = textFa,
                            FileName = fileName,
                            FileId = fileId,
                            ExpiredDate = DateTime.Now,
                            CreatedDate = DateTime.Now
                        };
                        db.BotNews.Add(item);
                        message = "Your record has been inserted.";
                    }
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200,
                    message = message
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex); ;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var item = db.BotNews.First(x => x.Id == id);
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            id = item.Id,
                            type = item.Type,
                            text = item.Text,
                            textFa = item.TextFa,
                            fileId = item.FileId,
                            fileName = item.FileName
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var item = db.BotNews.First(x => x.Id == id);

                    if (!string.IsNullOrEmpty(item.FileName))
                    {
                        System.IO.File.Delete(Path.Combine("\\News\\", item.FileName));
                    }

                    db.BotNews.Remove(item);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "Your record successfully removed."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}