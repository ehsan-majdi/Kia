using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Bot;
using KiaGallery.Web.Areas.Bot.Models;
using KiaGallery.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KiaGallery.Web.Areas.Bot.Controllers
{
    public class ConversionController : Controller
    {
        public object FoodService { get; private set; }

        [Authorize(Roles = "admin, botMain")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin, botMain")]
        public ActionResult View(int id)
        {
            ViewBag.ChatId = id;
            return View();
        }

        [Authorize(Roles = "admin, botMain")]
        public ActionResult Messages()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetUserList(long? chatId, string firstName, string lastName, string username, int page, int count)
        {
            List<BotUserData> listUsers = new List<BotUserData>();
            int UserCount = 0;

            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var query = db.BotUserData.Select(x => x);

                if (chatId != null && chatId > 0)
                    query = query.Where(x => x.ChatId == chatId);

                if (!string.IsNullOrEmpty(firstName))
                    query = query.Where(x => x.FirstName.Contains(firstName));

                if (!string.IsNullOrEmpty(lastName))
                    query = query.Where(x => x.LastName.Contains(lastName));

                if (!string.IsNullOrEmpty(username))
                    query = query.Where(x => x.Username.Contains(username));

                listUsers = query.OrderByDescending(x => x.Id).Skip(page * count).Take(count).ToList();
                UserCount = query.Count();
            }

            var response = new Response()
            {
                status = 200,
                data = new
                {
                    list = listUsers,
                    userCount = UserCount,
                    pageCount = Math.Ceiling((double)UserCount / count)
                }
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetUserUpdates(long chatId, int firstId)
        {
            List<BotMessage> listMessages = new List<BotMessage>();
            int messageCount = 0;

            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var query = db.BotMessage.Include("Replay").Where(x => x.Id > firstId && x.ChatId == chatId);

                listMessages = query.OrderByDescending(x => x.Id).ToList();
                messageCount = query.Count();
            }

            var data = listMessages.Select(x => new MessageView()
            {
                Id = x.Id,
                ChatId = x.ChatId,
                MessageId = x.MessageId,
                Text = x.Text,
                Date = Common.DateUtility.GetPersianDateTime(x.CreatedDate.GetValueOrDefault()),
                Unknown = x.Unknown.GetValueOrDefault(),
                ReplayId = x.ReplayId,
                ReplayText = x.ReplayId == null ? null : x.Replay.Text,
                ReplayDate = x.ReplayId == null ? null : Common.DateUtility.GetPersianDateTime(x.Replay.CreatedDate.GetValueOrDefault())

            }).ToList();

            var response = new Response()
            {
                status = 200,
                data = new
                {
                    list = data,
                    messageCount = messageCount
                }
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetUserChat(int chatId, int lastId, int count, bool unknown)
        {
            List<BotMessage> data = new List<BotMessage>();
            BotUserData user = null;
            var MessageCount = 0;

            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                user = db.BotUserData.SingleOrDefault(x => x.ChatId == chatId);
                var query = db.BotMessage.Include("Replay").Where(x => x.ChatId == chatId);
                if (lastId > 0)
                    query = query.Where(x => x.Id < lastId);

                data = query.OrderByDescending(x => x.Id).Take(count).ToList();
                MessageCount = query.Count();
            }

            var ResponseData = data.Select(x => new MessageView()
            {
                Id = x.Id,
                ChatId = x.ChatId,
                MessageId = x.MessageId,
                Text = x.Text,
                Date = Common.DateUtility.GetPersianDateTime(x.CreatedDate.GetValueOrDefault()),
                Unknown = x.Unknown.GetValueOrDefault(),
                ReplayId = x.ReplayId,
                ReplayText = x.ReplayId == null ? null : x.Replay.Text,
                ReplayDate = x.ReplayId == null ? null : Common.DateUtility.GetPersianDateTime(x.Replay.CreatedDate.GetValueOrDefault())
            }).ToList();


            var response = new Response()
            {
                status = 200,
                data = new
                {
                    list = ResponseData,
                    user = user,
                    messageCount = MessageCount
                }
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetMessages(long? chatId, string text, string fromDate, string toDate, int page, int count, bool unknown)
        {
            List<BotMessage> data = new List<BotMessage>();
            int MessageCount = 0;

            using (KiaGalleryContext db = new KiaGalleryContext())
            {

                var query = db.BotMessage.Include("Replay").Select(x => x);

                if (chatId != null && chatId > 0)
                    query = query.Where(x => x.ChatId == chatId);

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

                if (unknown)
                    query = query.Where(x => x.Unknown == true);

                MessageCount = query.Count();
                data = query.OrderByDescending(x => x.Id).Skip(page * count).Take(count).ToList();
            }

            var MessageList = data.Select(x => new MessageView()
            {
                Id = x.Id,
                ChatId = x.ChatId,
                MessageId = x.MessageId,
                Text = x.Text,
                Date = Common.DateUtility.GetPersianDateTime(x.CreatedDate.GetValueOrDefault()),
                Unknown = x.Unknown.GetValueOrDefault(),
                ReplayId = x.ReplayId,
                ReplayText = x.ReplayId == null ? null : x.Replay.Text,
                ReplayDate = x.ReplayId == null ? null : Common.DateUtility.GetPersianDateTime(x.Replay.CreatedDate.GetValueOrDefault())

            }).ToList();

            var response = new Response()
            {
                status = 200,
                data = new
                {
                    list = MessageList,
                    messageCount = MessageCount,
                    pageCount = Math.Ceiling((double)MessageCount / count)
                }
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult ResponseMessage(int id, string response)
        {
            Response responseObject = null;
            try
            {
                List<BotSettings> _Settings = null;
                BotMessage UserMessage = null;
                BotUserData user;

                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    _Settings = db.BotSettings.ToList();
                    UserMessage = db.BotMessage.FirstOrDefault(x => x.Id == id);
                    user = db.BotUserData.First(x => x.ChatId == UserMessage.ChatId);
                }

                TelegramBotClient Bot = new TelegramBotClient(_Settings.First(x => x.Key == "BotApi").Value);

                if (user.Language == 0)
                {
                    var PersianMainKeyBoard = new ReplyKeyboardMarkup();
                    PersianMainKeyBoard.Keyboard = new KeyboardButton[][] {
                            new KeyboardButton[] { _Settings.First(x => x.Key == "ContactUs").ValueFa, _Settings.First(x => x.Key == "Branch").ValueFa },
                            new KeyboardButton[] { _Settings.First(x => x.Key == "Collection").ValueFa, _Settings.First(x => x.Key == "News").ValueFa },
                            new KeyboardButton[] { _Settings.First(x => x.Key == "Persian").Value, _Settings.First(x => x.Key == "English").Value }
                        };
                    PersianMainKeyBoard.ResizeKeyboard = true;
                    Bot.SendTextMessageAsync(UserMessage.ChatId.GetValueOrDefault(), response,Telegram.Bot.Types.Enums.ParseMode.Html, false, false, UserMessage.MessageId, PersianMainKeyBoard);
                }
                else
                {
                    var EnglishMainKeyBoard = new ReplyKeyboardMarkup();
                    EnglishMainKeyBoard.Keyboard = new KeyboardButton[][] {
                            new KeyboardButton[] { _Settings.First(x => x.Key == "ContactUs").Value, _Settings.First(x => x.Key == "Branch").Value },
                            new KeyboardButton[] { _Settings.First(x => x.Key == "Collection").Value, _Settings.First(x => x.Key == "News").Value },
                            new KeyboardButton[] { _Settings.First(x => x.Key == "Persian").Value, _Settings.First(x => x.Key == "English").Value }
                        };
                    EnglishMainKeyBoard.ResizeKeyboard = true;

                    Bot.SendTextMessageAsync(UserMessage.ChatId.GetValueOrDefault(), response, Telegram.Bot.Types.Enums.ParseMode.Html, false, false, UserMessage.MessageId, EnglishMainKeyBoard);
                }

                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    UserMessage = db.BotMessage.FirstOrDefault(x => x.Id == id);
                    var Replay = new Replay()
                    {
                        Text = response,
                        CreatedDate = DateTime.Now
                    };
                    db.BotReplay.Add(Replay);
                    UserMessage.Replay = Replay;
                    db.SaveChanges();
                }

                responseObject = new Response()
                {
                    status = 200,
                    message = "Your response sent successfully."
                };
            }
            catch (Exception)
            {
                responseObject = new Response()
                {
                    status = 200,
                    message = "Your response not sent successfully, try again later."
                };
            }

            return Json(responseObject, JsonRequestBehavior.AllowGet);
        }
    }
}