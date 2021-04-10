using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Bot;
using KiaGallery.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KiaGallery.Web.Areas.Bot.Controllers
{
    public class BroadcastController : BaseController
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
                using (var db = new KiaGalleryContext())
                {
                    var query = db.BotBroadcast.Select(x => new
                    {
                        x.Id,
                        x.BroadcastType,
                        x.Text,
                        x.FileName,
                        x.CreatedDate
                    });

                    var data = query.OrderByDescending(x => x.Id).Skip(page * count).Take(count).ToList();
                    int dataCount = query.Count();

                    var listData = data.Select(x => new
                    {
                        id = x.Id,
                        type = Enums.GetTitle(x.BroadcastType.GetValueOrDefault()),
                        text = x.Text,
                        fileName = x.FileName,
                        persianCreatedDate = DateUtility.GetPersianDateTime(x.CreatedDate.GetValueOrDefault())
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
        public JsonResult Save(BotType type, string text, string textFa, string fileName, string dataCode, int? count)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var item = new Broadcast()
                    {
                        BroadcastType = type,
                        Text = text,
                        TextFa = textFa,
                        FileName = fileName,
                        FileId = null,
                        CreatedDate = DateTime.Now,
                        Sended = false,
                        SubmitedUser = user.Id
                    };
                    db.BotBroadcast.Add(item);
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200,
                    message = "پیام عمومی ثبت شد."
                };


            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult SendSample(int userId, string text, string fileName, int type)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.UserData.First(x => x.Id == userId);
                    List<BotSettings> settings = db.BotSettings.ToList();
                    TelegramBotClient Bot = new TelegramBotClient(settings.First(x => x.Key == "BotApi").Value);

                    switch (type)
                    {
                        case 0:
                            Bot.SendTextMessageAsync(user.ChatId, text,Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, null);
                            break;
                        case 1:
                            string RootPath = "C:/Files/KiaGalleryBot/";

                            if (string.IsNullOrEmpty(fileName)) break;
                            var filePath = RootPath + "Broadcast/" + fileName;

                            Stream stream = System.IO.File.OpenRead(filePath);
                            FileToSend file = new FileToSend(System.IO.Path.GetFileName(filePath), stream);

                            if (text.Length <= 199)
                            {
                                Bot.SendPhotoAsync(user.ChatId, file, text, false, 0, null);
                            }
                            else
                            {
                                Bot.SendPhotoAsync(user.ChatId, file, "-");
                                Bot.SendTextMessageAsync(user.ChatId, text, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, null);
                            }
                            break;
                    }
                }

                response = new Response()
                {
                    status = 200,
                    message = "Your record has been Sent."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}