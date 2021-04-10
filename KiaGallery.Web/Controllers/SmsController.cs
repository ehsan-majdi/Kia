using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر مدیریت اس ام اس
    /// </summary>
    public class SmsController : BaseController
    {
        /// <summary>
        /// صفحه اس ام اس و مدیریت
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch/*.Where(x => x.BranchType == BranchType.Branch)*/.Select(x => new BranchListViewModel
                {
                    name = x.Name,
                    id = x.Id
                }).ToList();

            }
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult CreateSms()
        {
            List<int> daysOfMonth = new List<int>();
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch/*.Where(x => x.BranchType == BranchType.Branch)*/.Select(x => new BranchListViewModel
                {
                    name = x.Name,
                    id = x.Id
                }).ToList();
                ViewBag.UserList = db.User.Where(x => x.Active == true).Select(x => new UserViewModel
                {
                    id = x.Id,
                    firstName = x.FirstName,
                    lastName = x.LastName
                }).ToList();

            }
            return View("Edit");
        }
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Id = id;
                ViewBag.BranchList = db.Branch/*.Where(x => x.BranchType == BranchType.Branch)*/.Select(x => new BranchListViewModel
                {
                    name = x.Name,
                    id = x.Id
                }).ToList();
                ViewBag.UserList = db.User.Where(x => x.Active == true).Select(x => new UserViewModel
                {
                    id = x.Id,
                    firstName = x.FirstName,
                    lastName = x.LastName
                }).ToList();
            }

            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult List()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch/*.Where(x => x.BranchType == BranchType.Branch)*/.Select(x => new BranchListViewModel
                {
                    name = x.Name,
                    id = x.Id
                }).ToList();
                ViewBag.UserList = db.User.Where(x => x.Active == true).Select(x => new UserViewModel
                {
                    id = x.Id,
                    firstName = x.FirstName,
                    lastName = x.LastName
                }).ToList();

            }
            return View();
        }
        [Authorize(Roles = "admin")]
        public JsonResult Send(SmsOption model)
        {
            Response response;
            List<string> personelNumber;
            try
            {
                var date = DateUtility.GetDateTime(model.persianDate);

                if (!string.IsNullOrEmpty(model.branchId))
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var branchIdList = model.branchId.Split();
                        List<int> myInts = branchIdList.Select(int.Parse).ToList();
                        personelNumber = db.User.Where(y => myInts.Contains(y.BranchId.Value) && y.PhoneNumber != null && y.PhoneNumber != "").Select(y => y.PhoneNumber).ToList();
                    }
                    Task.Factory.StartNew(() =>
                    {
                        NikSmsWebServiceClient.SendSmsNik(model.text, personelNumber);
                    });
                }
                if (model.phoneNumber != null && model.phoneNumber != "" && !model.phoneNumber.Contains('-'))
                {
                    Task.Factory.StartNew(() =>
                    {
                        NikSmsWebServiceClient.SendSmsNik(model.text, model.phoneNumber);
                    });
                }
                if (model.phoneNumber != null && model.phoneNumber != "" && model.phoneNumber.Contains('-'))
                {
                    var numberList = model.phoneNumber.Split('-');
                    var numbers = new List<string>();
                    numbers.AddRange(numberList);
                    Task.Factory.StartNew(() =>
                    {
                        NikSmsWebServiceClient.SendSmsNik(model.text, numbers);
                    });
                }

                response = new Response()
                {
                    status = 200,
                    message = "Done",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Sms.Where(x => x.Id == id).Select(x => new SearchSmsViewModel
                    {
                        id = x.Id,
                        text = x.Text,
                        phoneNumber = x.DestinationNumber,
                        time = x.Time,
                        sent = x.Sent,
                        userId = x.UserId,
                        branchId = x.BranchId,
                        date = x.SendingDate,
                        dayOfWeek = x.DayOfWeek,
                        dayOfMonth = x.DayOfMonth,
                        sendingTimeMethod = x.SendingTimeMethod
                    }).Single();
                    entity.persianDate = DateUtility.GetPersianDate(entity.date);
                    response = new Response()
                    {
                        status = 200,
                        data = entity
                    };
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin")]
        public JsonResult Save(SmsOption model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (model.id > 0 && model.id != null)
                    {
                        var entity = db.Sms.Single(x => x.Id == model.id);
                        entity.Text = model.text;
                        entity.DayOfMonth = model.dayOfMonth;
                        entity.DayOfWeek = model.dayOfWeek;
                        entity.SendingTimeMethod = model.sendingTimeMethod;
                        entity.Sent = false;
                        entity.DestinationNumber = model.phoneNumber;
                        entity.UserId = model.userId;
                        entity.BranchId = model.branchId;
                        entity.SendingDate = DateUtility.GetDateTime(model.persianDate).Value;
                        entity.TimeTotalMinutes = (int)TimeSpan.Parse(model.time).TotalMinutes;
                        entity.Time = model.time;
                        entity.CreateUserId = GetAuthenticatedUserId();
                        entity.ModifyUserId = GetAuthenticatedUserId();
                    }
                    else
                    {
                        var entity = new Sms()
                        {
                            Text = model.text,
                            DayOfMonth = model.dayOfMonth,
                            DayOfWeek = model.dayOfWeek,
                            SendingTimeMethod = model.sendingTimeMethod,
                            Sent = false,
                            DestinationNumber = model.phoneNumber,
                            UserId = model.userId,
                            BranchId = model.branchId,
                            SendingDate = DateUtility.GetDateTime(model.persianDate).Value,
                            TimeTotalMinutes = (int)TimeSpan.Parse(model.time).TotalMinutes,
                            Time = model.time,
                            CreateDate = DateTime.Now.Date,
                            ModifyDate = DateTime.Now.Date,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                        };
                        db.Sms.Add(entity);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "پیام با موفقیت ذخیره شد.",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Sms.Find(id);
                    db.Sms.Remove(entity);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin")]
        public JsonResult Search(SearchSmsViewModel model)
        {
            Response response;
            try
            {
                int dataCount = 0;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Sms.Select(x => x);
                    dataCount = query.Count();
                    var list = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count).Select(x => new SearchSmsViewModel
                    {
                        id = x.Id,
                        text = x.Text,
                        phoneNumber = x.DestinationNumber,
                        time = x.Time,
                        date = x.SendingDate,
                        type = x.SendingTimeMethod
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.date);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list,
                            pageCount = Math.Ceiling((double)dataCount / model.count),
                            count = dataCount,
                            page = model.page + 1,
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
    }
}
