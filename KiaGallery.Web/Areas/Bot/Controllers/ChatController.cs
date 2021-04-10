using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Bot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using KiaGallery.Model;

namespace KiaGallery.Web.Areas.Bot.Controllers
{
    public class ChatController : Controller
    {
        [Authorize(Roles = "admin, botMain")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetChats(string fromDate, string toDate, bool unknown)
        {
            Response response;
            try
            {
                DateTime? From = DateUtility.GetDateTime(fromDate);
                DateTime? To = DateUtility.GetDateTime(toDate);
                if (To != null) To = To.GetValueOrDefault().AddDays(1);

                using (var db = new KiaGalleryContext())
                {
                    var query = db.BotMessage.Select(x => x);
                    if (unknown)
                        query = query.Where(x => x.Unknown == true);

                    if (From != null)
                        query = query.Where(x => x.CreatedDate >= From);

                    if (To != null)
                        query = query.Where(x => x.CreatedDate <= To);

                    query = query.OrderByDescending(x => x.Id);
                    List<ResultUserList> result = query.Select(x => new ResultUserList()
                    {
                        chatId = x.ChatId,
                        firstName = db.UserData.FirstOrDefault(y => y.ChatId == x.ChatId).FirstName,
                        lastName = db.UserData.FirstOrDefault(y => y.ChatId == x.ChatId).LastName,
                        username = db.UserData.FirstOrDefault(y => y.ChatId == x.ChatId).Username,
                        lastMessage = x.Text
                    }).ToList();

                    List<ResultUserList> finalResult = new List<ResultUserList>();
                    for (int i = 0; i < result.Count; i++)
                    {
                        try
                        {
                            var item = result[i];
                            var res = finalResult.FirstOrDefault(x => x.chatId == item.chatId);
                            if (res == null) finalResult.Add(item);
                        }
                        catch (Exception) { }
                    }


                    //group user by new { ChatId = user.ChatId, FirstName = user.FirstName, LastName = user.LastName, Username = user.Username } into resultUser
                    //select new
                    //{
                    //    chatId = resultUser.Key.ChatId,
                    //    firstName = resultUser.Key.FirstName,
                    //    lastName = resultUser.Key.LastName,
                    //    userName = resultUser.Key.Username,
                    //    lastMessage = db.Message.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ChatId == resultUser.Key.ChatId && x.Unknown == true).Text
                    //}).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = finalResult
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

        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetUserChat(int chatId, int lastId)
        {
            Response response;
            try
            {
                List<BotMessage> result;
                using (var db = new KiaGalleryContext())
                {
                    result = db.BotMessage.Where(x=> x.ChatId == chatId && x.Id > lastId && x.Unknown == true ).OrderBy(x=> x.Id).Take(100).ToList();
                }
                var data = result.Select(x => new
                {
                    id = x.Id,
                    messageId = x.MessageId,
                    text = x.Text,
                    createdDate = DateUtility.GetPersianDateTime(x.CreatedDate.GetValueOrDefault()),
                    unknown = x.Unknown,
                    replyId = x.ReplayMessageId
                }).OrderBy(x => x.id).ToList();

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = data
                    }
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
        public JsonResult ResponseMessage(int chatId, int? messageId, string text)
        {
            Response responseObject = null;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    List<BotSettings> Settings = null;
                    BotMessage item = null;
                    Settings = db.BotSettings.ToList();
                    BotUserData user;

                    user = db.BotUserData.First(x => x.ChatId == chatId);

                    if (messageId != null)
                        item = db.BotMessage.FirstOrDefault(x => x.MessageId == messageId);

                    TelegramBotClient Bot = new TelegramBotClient(Settings.First(x => x.Key == "BotApi").Value);

                    if (user.Language == 0)
                    {
                        var PersianMainKeyBoard = new ReplyKeyboardMarkup();
                        PersianMainKeyBoard.Keyboard = new KeyboardButton[][] {
                            new KeyboardButton[] { Settings.First(x => x.Key == "ContactUs").ValueFa, Settings.First(x => x.Key == "Branch").ValueFa },
                            new KeyboardButton[] { Settings.First(x => x.Key == "Collection").ValueFa, Settings.First(x => x.Key == "News").ValueFa },
                            new KeyboardButton[] { Settings.First(x => x.Key == "Persian").Value, Settings.First(x => x.Key == "English").Value }
                        };
                        PersianMainKeyBoard.ResizeKeyboard = true;
                        Bot.SendTextMessageAsync(chatId, text,Telegram.Bot.Types.Enums.ParseMode.Default, false, false, messageId.GetValueOrDefault(), PersianMainKeyBoard);
                    }
                    else
                    {
                        var EnglishMainKeyBoard = new ReplyKeyboardMarkup();
                        EnglishMainKeyBoard.Keyboard = new KeyboardButton[][] {
                            new KeyboardButton[] { Settings.First(x => x.Key == "ContactUs").Value, Settings.First(x => x.Key == "Branch").Value },
                            new KeyboardButton[] { Settings.First(x => x.Key == "Collection").Value, Settings.First(x => x.Key == "News").Value },
                            new KeyboardButton[] { Settings.First(x => x.Key == "Persian").Value, Settings.First(x => x.Key == "English").Value }
                        };
                        EnglishMainKeyBoard.ResizeKeyboard = true;

                        Bot.SendTextMessageAsync(chatId, text,Telegram.Bot.Types.Enums.ParseMode.Default, false, false, messageId.GetValueOrDefault(), EnglishMainKeyBoard);
                    }

                    BotMessage message = new BotMessage()
                    {
                        ChatId = chatId,
                        Text = text,
                        CreatedDate = DateTime.Now,
                        Unknown = true,
                        ReplayMessageId = (item != null ? item.Id : (int?)null)

                    };
                    db.BotMessage.Add(message);
                    db.SaveChanges();
                }

                responseObject = new Response()
                {
                    status = 200,
                    message = "Your response sent successfully."
                };
            }
            catch (Exception ex)
            {
                responseObject = Core.GetExceptionResponse(ex);
            }

            return Json(responseObject, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult SendData(int chatId, string data)
        {
            Response response;
            try
            {
                var ids = data.Split(',').Select(int.Parse).ToList();
                using (var db = new KiaGalleryContext())
                {
                    var SelectedData = db.Product.Include(x=> x.ProductFileList).Where(x => ids.Contains(x.Id)).ToList();
                    List<BotSettings> settings = db.BotSettings.ToList();

                    var user = db.BotUserData.First(x => x.ChatId == chatId);

                    TelegramBotClient Bot = new TelegramBotClient(settings.First(x => x.Key == "BotApi").Value);

                    for (int i = 0; i < SelectedData.Count; i++)
                    {
                        var item = SelectedData[i];
                        double Price = (((item.Wage + int.Parse(settings.First(x => x.Key == "GoldPrice").Value)) * item.Weight) + (item.LeatherPrice + item.StonePrice)).GetValueOrDefault();
                        var TotalPrice = Core.Roundup(Price + (Price * 0.07) + ((Price + (Price * 0.07)) / 100 * 9), 4);
                        string Message;
                        if (user.Language == 0)
                        {
                            Message = settings.First(x => x.Key == "DataCaption").ValueFa.Replace("{Code}", item.Code).Replace("{Weight}", item.Weight.ToString()).Replace("{Price}", TotalPrice.ToString());
                        }
                        else
                        {
                            Message = settings.First(x => x.Key == "DataCaption").Value.Replace("{Code}", item.Code).Replace("{Weight}", item.Weight.ToString()).Replace("{Price}", TotalPrice.ToString());
                        }


                        if (string.IsNullOrEmpty(item.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileId))
                        {
                            string filePath = "C:\\Files\\KiaGalleryBot\\Data\\" + item.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileName;
                            Stream stream = System.IO.File.OpenRead(filePath);
                            FileToSend file = new FileToSend(Path.GetFileName(filePath), stream);
                            var msg = Bot.SendPhotoAsync(chatId, file, Message, false, 0);
                            string FileId = msg.Result.Photo[0].FileId;

                            var dataItem = db.Product.Include(x => x.ProductFileList).FirstOrDefault(x => x.Id == item.Id);
                            dataItem.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileId = FileId;
                        }
                        else
                        {
                            //Bot.SendPhotoAsync(chatId, item.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileId, Message, false, 0);
                        }
                    }

                    BotMessage message = new BotMessage()
                    {
                        ChatId = chatId,
                        Text = string.Join("", SelectedData.Select(x => x.Code + "\n").ToList()),
                        CreatedDate = DateTime.Now,
                        Unknown = true,
                        ReplayMessageId = null

                    };
                    db.BotMessage.Add(message);
                    db.SaveChanges();

                }
                response = new Response()
                {
                    status = 200,
                    message = "Your response sent successfully."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "admin, botMain")]
        public JsonResult DeleteMessage(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.BotMessage.First(x => x.Id == id);
                    db.BotMessage.Remove(item);
                    response = new Response()
                    {
                        status = 200,
                        message = "Your record successfully deleted."
                    };
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }

    public class ResultUserList
    {
        public long? chatId { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string lastMessage { get; set; }
    }

}
