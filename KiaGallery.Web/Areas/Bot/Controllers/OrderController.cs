using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Bot;
using KiaGallery.Web.Controllers;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KiaGallery.Web.Areas.Bot.Controllers
{
    public class OrderController : BaseController
    {
        [Authorize(Roles = "admin, botMain")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin, botMain")]
        public ActionResult ViewOrder(int id)
        {
            BotOrder item;
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                item = db.BotOrder.Include(x=> x.BotOrderLogList).First(x => x.Id == id);
            }

            ViewBag.Id = id;
            ViewBag.Title = item.PhoneNumber + " (" + item.FirstName + " " + item.LastName + ")";
            ViewBag.PhoneNumber = item.PhoneNumber;
            ViewBag.FirstName = item.FirstName;
            ViewBag.LastName = item.LastName;
            ViewBag.Description = item.Description;
            ViewBag.Deposit = item.Deposit;
            ViewBag.CardDetails = item.CardDetails;
            ViewBag.Address = item.Address;
            ViewBag.PaymentType = item.PaymentType;
            ViewBag.Status = item.Status;
            ViewBag.StatusText = Enums.GetTitle(item.Status);
            ViewBag.DataId = item.ProductId;

            ViewBag.CreatedDate = DateUtility.GetPersianDateTime(item.CreatedDate);
            ViewBag.AnswerDate = DateUtility.GetPersianDateTime(item.BotOrderLogList.FirstOrDefault(x=> x.Status == BotOrderStatus.PendingPrepayment).CreateDate);
            ViewBag.FirstPaymentDate = DateUtility.GetPersianDateTime(item.BotOrderLogList.FirstOrDefault(x => x.Status == BotOrderStatus.UnderConstruction).CreateDate);
            ViewBag.BuildDate = DateUtility.GetPersianDateTime(item.BotOrderLogList.FirstOrDefault(x => x.Status == BotOrderStatus.PendingPayment).CreateDate);
            ViewBag.SendDate = DateUtility.GetPersianDateTime(item.BotOrderLogList.FirstOrDefault(x => x.Status == BotOrderStatus.Sent).CreateDate);

            return View();
        }

        [Authorize(Roles = "admin, botMain")]
        public ActionResult NewOrder()
        {
            return View("ViewOrder");
        }

        [HttpGet]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetData(int page, int count, string nameQuery, string orderNo, BotOrderStatus[] status, string fromDate, string toDate)
        {
            Response response;
            try
            {
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var query = db.BotOrder.Where(x => status.Any(y => y == x.Status));

                    if (!string.IsNullOrEmpty(orderNo))
                    {
                        query = query.Where(x => x.OrderSerial.Contains(orderNo));
                    }

                    if (!string.IsNullOrEmpty(nameQuery))
                    {
                        query = query.Where(x => (x.FirstName.Contains(nameQuery) || x.LastName.Contains(nameQuery) || x.PhoneNumber.Contains(nameQuery)));
                    }

                    if (string.IsNullOrEmpty(nameQuery) && string.IsNullOrEmpty(orderNo) && !string.IsNullOrEmpty(fromDate))
                    {
                        DateTime? _fromDate = DateUtility.GetDateTime(fromDate);
                        if (_fromDate != null)
                            query = query.Where(x => x.CreatedDate >= _fromDate);
                    }

                    if (string.IsNullOrEmpty(nameQuery) && string.IsNullOrEmpty(orderNo) && !string.IsNullOrEmpty(toDate))
                    {
                        DateTime? _toDate = DateUtility.GetDateTime(toDate);
                        if (_toDate != null)
                        {
                            _toDate = _toDate.Value.AddDays(1);
                            query = query.Where(x => x.CreatedDate <= _toDate);
                        }
                    }

                    query.Select(x => new
                    {
                        x.Id,
                        x.ChatId,
                        x.OrderSerial,
                        x.Status,
                        x.FirstName,
                        x.LastName,
                        x.PhoneNumber,
                        x.CreatedDate
                    });

                    var data = query.OrderByDescending(x => x.Id).Skip(page * count).Take(count).ToList();
                    int dataCount = query.Count();

                    var listData = data.Select(x => new
                    {
                        id = x.Id,
                        chatId = x.ChatId,
                        orderNo = x.OrderSerial,
                        status = x.Status,
                        statusText = Enums.GetTitle(x.Status),
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        phoneNumber = x.PhoneNumber,
                        createdDate = DateUtility.GetPersianDateTime(x.CreatedDate),
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
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var item = db.BotOrder.First(x => x.Id == id);

                    if (item.Status > 0)
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "Your cannot delete this record."
                        };
                    }
                    else
                    {
                        db.BotOrder.Remove(item);
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "Your record successfully removed."
                        };
                    }
                }
            }
            catch (Exception)
            {
                response = new Response()
                {
                    status = 500,
                    message = "Internal Server Error, Try again later."
                };
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "admin, botMain")]
        public JsonResult SaveMainInfo(int id, string firstName, string lastName, string description)
        {
            Response response = null;
            try
            {
                int userId = GetAuthenticatedUserId();
                List<BotSettings> settings;
                BotUserData userData;
                using (KiaGalleryContext db = new KiaGalleryContext())
                {

                    var item = db.BotOrder.First(x => x.Id == id);
                    item.Status = BotOrderStatus.PendingPrepayment;
                    item.FirstName = firstName;
                    item.LastName = lastName;
                    item.Description = description;
                    //item.AnswerDate = DateTime.Now;
                    BotOrderLog log = new BotOrderLog()
                    {
                        OrderId = item.Id,
                        Status = BotOrderStatus.PendingPrepayment,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.BotOrderLog.Add(log);
                    settings = db.BotSettings.ToList();
                    userData = db.BotUserData.First(x => x.ChatId == item.ChatId);

                    db.SaveChanges();
                }

                TelegramBotClient Bot = new TelegramBotClient(settings.First(x => x.Key == "BotApi").Value);
                if (userData.Language == 0)
                {
                    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "SaveMainInfoText").ValueFa, Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
                    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "AccountInfo").ValueFa, Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
                }
                else
                {
                    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "SaveMainInfoText").Value,Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
                    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "AccountInfo").Value, Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
                }

                response = new Response()
                {
                    status = 200
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
        public JsonResult SavePrePayment(int id, int deposit, string cardDetails)
        {
            Response response = null;
            try
            {
                int userId = GetAuthenticatedUserId();
                List<BotSettings> settings;
                BotUserData userData;
                BotOrder item;
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    item = db.BotOrder.Include(x=> x.BotOrderLogList).Include(x=>x.Product).First(x => x.Id == id);
                    item.Status = BotOrderStatus.UnderConstruction;
                    item.Deposit = deposit;
                    item.CardDetails = cardDetails;
                    // item.FirstPaymentDate = DateTime.Now;
                    BotOrderLog log = new BotOrderLog()
                    {
                        OrderId = item.Id,
                        Status = BotOrderStatus.UnderConstruction,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.BotOrderLog.Add(log);

                    settings = db.BotSettings.ToList();
                    userData = db.BotUserData.First(x => x.ChatId == item.ChatId);

                    db.SaveChanges();
                }

                TelegramBotClient Bot = new TelegramBotClient(settings.First(x => x.Key == "BotApi").Value);
                if (userData.Language == 0)
                {
                    string message = settings.First(x => x.Key == "SavePrePaymentText").ValueFa
                        .Replace("{Name}", item.FirstName + " " + item.LastName)
                        .Replace("{Date}", DateUtility.GetPersianDateTime(DateTime.Now))
                        .Replace("{OrderNo}", item.OrderSerial)
                        .Replace("{Code}", item.Product.Code)
                        .Replace("{Desc}", item.Description)
                        .Replace("{Price}", deposit.ToString());


                    if (string.IsNullOrEmpty(item.Product.ProductFileList.FirstOrDefault(x=> x.FileType == FileType.Bot).FileId))
                    {
                        string filePath = "/Upload/Product/" + item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileName;
                        Stream stream = System.IO.File.OpenRead(filePath);
                        FileToSend file = new FileToSend(System.IO.Path.GetFileName(filePath), stream);
                        Bot.SendPhotoAsync(userData.ChatId, file, message, false, 0);
                    }
                    else
                    {
                        FileToSend file = new FileToSend(item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileId);
                        Bot.SendPhotoAsync(userData.ChatId, file, message, false, 0);
                    }
                }
                else
                {
                    string message = settings.First(x => x.Key == "SavePrePaymentText").Value
                        .Replace("{Name}", item.FirstName + " " + item.LastName)
                        .Replace("{Date}", DateUtility.GetPersianDateTime(DateTime.Now))
                        .Replace("{OrderNo}", item.OrderSerial)
                        .Replace("{Code}", item.Product.Code)
                        .Replace("{Desc}", item.Description)
                        .Replace("{Price}", deposit.ToString());

                    if (string.IsNullOrEmpty(item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileId))
                    {
                        string filePath = "/Upload/Data/" + item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileName;
                        Stream stream = System.IO.File.OpenRead(filePath);
                        FileToSend file = new FileToSend(System.IO.Path.GetFileName(filePath), stream);
                        Bot.SendPhotoAsync(userData.ChatId, file, message, false, 0);
                    }
                    else
                    {
                        FileToSend file = new FileToSend(item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileId);
                        Bot.SendPhotoAsync(userData.ChatId, file, message, false, 0);
                    }
                }

                response = new Response()
                {
                    status = 200
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
        public JsonResult Built(int id)
        {
            Response response = null;
            try
            {
                int userId = GetAuthenticatedUserId();
                //List<BotSettings> settings;
                //UserData userData;
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var item = db.BotOrder.First(x => x.Id == id);
                    item.Status = BotOrderStatus.PendingPayment;
                    //item.BuildDate = DateTime.Now;
                    BotOrderLog log = new BotOrderLog()
                    {
                        OrderId = item.Id,
                        Status = BotOrderStatus.PendingPayment,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.BotOrderLog.Add(log);
                    //settings = db.BotSettings.ToList();
                    //userData = db.BotUserData.First(x => x.ChatId == item.ChatId);

                    db.SaveChanges();
                }

                //TelegramBotClient Bot = new TelegramBotClient(settings.First(x => x.Key == "BotApi").Value);
                //if (userData.Language == 0)
                //    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "BuiltComplete").ValueFa, false, false);
                //else
                //    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "BuiltComplete").Value, false, false);

                response = new Response()
                {
                    status = 200
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
        public JsonResult Send(int id, string address, int paymentType)
        {
            Response response = null;
            try
            {
                int userId = GetAuthenticatedUserId();
                List<BotSettings> settings;
                BotUserData userData;
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var item = db.BotOrder.First(x => x.Id == id);
                    item.Status = BotOrderStatus.Sent;
                    item.Address = address;
                    item.PaymentType = paymentType;
                    BotOrderLog log = new BotOrderLog()
                    {
                        OrderId = item.Id,
                        Status = BotOrderStatus.Sent,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.BotOrderLog.Add(log);
                    settings = db.BotSettings.ToList();
                    userData = db.BotUserData.First(x => x.ChatId == item.ChatId);

                    db.SaveChanges();
                }

                TelegramBotClient Bot = new TelegramBotClient(settings.First(x => x.Key == "BotApi").Value);
                if (userData.Language == 0)
                    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "SendText").ValueFa, Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
                else
                    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "SendText").Value, Telegram.Bot.Types.Enums.ParseMode.Html, false, false);

                response = new Response()
                {
                    status = 200
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
        public JsonResult Cancel(int id)
        {
            Response response = null;
            try
            {
                int userId = GetAuthenticatedUserId();
                List<BotSettings> settings;
                BotUserData userData;
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var item = db.BotOrder.First(x => x.Id == id);
                    item.Status = BotOrderStatus.Canceled;
                    BotOrderLog log = new BotOrderLog()
                    {
                        OrderId = item.Id,
                        Status = BotOrderStatus.Canceled,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.BotOrderLog.Add(log);
                    settings = db.BotSettings.ToList();
                    userData = db.BotUserData.First(x => x.ChatId == item.ChatId);

                    db.SaveChanges();
                }

                TelegramBotClient Bot = new TelegramBotClient(settings.First(x => x.Key == "BotApi").Value);
                if (userData.Language == 0)
                    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "CancelText").ValueFa, Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
                else
                    Bot.SendTextMessageAsync(userData.ChatId, settings.First(x => x.Key == "CancelText").Value, Telegram.Bot.Types.Enums.ParseMode.Html, false, false);

                response = new Response()
                {
                    status = 200
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
        public JsonResult SilentCancel(int id)
        {
            Response response = null;
            try
            {
                int userId = GetAuthenticatedUserId();
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var item = db.BotOrder.First(x => x.Id == id);
                    item.Status = BotOrderStatus.Canceled;
                    BotOrderLog log = new BotOrderLog()
                    {
                        OrderId = item.Id,
                        Status = BotOrderStatus.Canceled,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.BotOrderLog.Add(log);
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200
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
        public JsonResult PendingCustomer(int id)
        {
            Response response = null;
            try
            {
                int userId = GetAuthenticatedUserId();
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var item = db.BotOrder.First(x => x.Id == id);
                    item.Status = BotOrderStatus.PendingCustomer;
                    BotOrderLog log = new BotOrderLog()
                    {
                        OrderId = item.Id,
                        Status = BotOrderStatus.PendingCustomer,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.BotOrderLog.Add(log);
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200
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
        public JsonResult NoAnswer(int id)
        {
            Response response = null;
            try
            {
                int userId = GetAuthenticatedUserId();
                using (KiaGalleryContext db = new KiaGalleryContext())
                {
                    var item = db.BotOrder.First(x => x.Id == id);
                    item.Status = BotOrderStatus.RejectCall;
                    BotOrderLog log = new BotOrderLog()
                    {
                        OrderId = item.Id,
                        Status = BotOrderStatus.RejectCall,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.BotOrderLog.Add(log);
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200
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