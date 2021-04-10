using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.DailyReportFinancial;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Newtonsoft.Json;
using Stimulsoft.Report;
using System.IO;
using Stimulsoft.Report.Export;
using System.Text;
using System.Data;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر گزارشات مالی روزانه
    /// </summary>
    public class DailyReportController : BaseController
    {
        /// <summary>
        /// صفحه اصلی
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public ActionResult Index()
        {
            PersianCalendar helper = new PersianCalendar();
            var year = helper.GetYear(DateTime.Now);
            List<int> yearList = new List<int>();
            for (int i = year - 5; i <= year + 5; i++)
            {
                yearList.Add(i);
            }
            ViewBag.Year = year;
            ViewBag.YearList = yearList;

            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
            }
            return View();
        }

        /// <summary>
        /// تقویم
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public ActionResult Calendar()
        {
            PersianCalendar helper = new PersianCalendar();
            var year = helper.GetYear(DateTime.Now);
            List<int> yearList = new List<int>();
            for (int i = year - 5; i <= year + 5; i++)
            {
                yearList.Add(i);
            }
            ViewBag.Year = year;
            ViewBag.YearList = yearList;

            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true && x.BranchType == BranchType.Branch).OrderBy(x => x.Order).ToList();
            }
            return View();
        }

        /// <summary>
        /// تنظیمات
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public ActionResult Settings()
        {
            return View();
        }

        /// <summary>
        /// یانک ها
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public ActionResult Bank()
        {
            return View();
        }

        /// <summary>
        /// ارزها
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public ActionResult Currency()
        {
            return View();
        }

        #region بانک
        /// <summary>
        /// ذخیره بانک
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات بانک</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult SaveBank(BankViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.Bank.Single(x => x.Id == model.id);
                        entity.Order = model.order;
                        entity.Name = model.name;
                        entity.Active = model.active;
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;

                        status = 200;
                        message = "بانک با موفقیت ویرایش شد.";

                    }
                    else
                    {
                        var entity = new Bank()
                        {
                            Order = model.order,
                            BranchId = model.branchId,
                            Name = model.name,
                            Active = model.active,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        db.Bank.Add(entity);

                        status = 200;
                        message = "بانک با موفقیت ایجاد شد.";
                    }
                    db.SaveChanges();

                }

                response = new Response()
                {
                    status = status,
                    message = message
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// خواندن اطلاعات 
        /// </summary>
        /// <param name="id">ردیف بانک</param>
        /// <returns>جیسون اطلاعات لود شده بانک</returns>
        [HttpGet]
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult LoadBank(int id)
        {
            Response response;
            try
            {
                Bank item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Bank.Find(id);

                    if (item != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new BankViewModel()
                            {
                                id = item.Id,
                                order = item.Order,
                                branchId = item.BranchId,
                                branchName = item.Branch.Name,
                                name = item.Name,
                                active = item.Active
                            }
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "بانک مورد نظر یافت نشد."
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// جستجوی بانک
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست بانک پیدا شده</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult SearchBank()
        {
            Response response;
            try
            {
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Bank.Select(x => x);

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.Order);

                    var list = query.Select(item => new
                    {
                        id = item.Id,
                        order = item.Order,
                        branchId = item.BranchId,
                        branchName = item.Branch.Name,
                        name = item.Name,
                        active = item.Active
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            count = dataCount,
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

        /// <summary>
        /// حذف بانک
        /// </summary>
        /// <param name="id">ردیف بانک</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult DeleteBank(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Bank.Find(Id);
                    if (item.DailyReportBankList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "بانک دارای گزارش ثبت شده می باشد."
                        };
                    }
                    else
                    {
                        db.Bank.Remove(item);
                        db.SaveChanges();

                        response = new Response()
                        {
                            status = 200,
                            message = "بانک با موفقیت حذف شد."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ارز
        /// <summary>
        /// ذخیره ارز
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات ارز</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult SaveCurrency(CurrencyViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.Currency.Single(x => x.Id == model.id);
                        entity.Order = model.order;
                        entity.Name = model.name;
                        entity.Active = model.active;
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;

                        status = 200;
                        message = "ارز با موفقیت ویرایش شد.";

                    }
                    else
                    {
                        var entity = new Currency()
                        {
                            Order = model.order,
                            Name = model.name,
                            Active = model.active,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        db.Currency.Add(entity);

                        status = 200;
                        message = "ارز با موفقیت ایجاد شد.";
                    }
                    db.SaveChanges();

                }

                response = new Response()
                {
                    status = status,
                    message = message
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// خواندن اطلاعات 
        /// </summary>
        /// <param name="id">ردیف ارز</param>
        /// <returns>جیسون اطلاعات لود شده ارز</returns>
        [HttpGet]
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult LoadCurrency(int id)
        {
            Response response;
            try
            {
                Currency item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Currency.Find(id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new CurrencyViewModel()
                        {
                            id = item.Id,
                            order = item.Order,
                            name = item.Name,
                            active = item.Active
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "ارز مورد نظر یافت نشد."
                    };
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// جستجوی ارز
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست ارز پیدا شده</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult SearchCurrency()
        {
            Response response;
            try
            {
                List<Currency> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Currency.Select(x => x);

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.Order);

                    list = query.ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            order = item.Order,
                            name = item.Name,
                            active = item.Active
                        }),
                        count = dataCount,
                    }
                };

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// حذف ارز
        /// </summary>
        /// <param name="id">ردیف ارز</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult DeleteCurrency(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Currency.Find(Id);
                    if (item.DailyReportCurrencyList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "ارز دارای گزارش ثبت شده می باشد."
                        };
                    }
                    else
                    {
                        db.Currency.Remove(item);
                        db.SaveChanges();

                        response = new Response()
                        {
                            status = 200,
                            message = "ارز با موفقیت حذف شد."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region تقویم
        /// <summary>
        /// دریافت خلاصه روز های ثبت شده 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult GetDailyBranch(int year, int month)
        {
            Response response;
            try
            {
                var persianDate = year + "/" + month + "/1";
                var date = DateUtility.GetDateTime(persianDate);
                var daysCount = DateUtility.GetMonthDayCount(year, month);

                using (var db = new KiaGalleryContext())
                {
                    var endDate = date.GetValueOrDefault().AddDays(daysCount + 1);
                    var calendar = db.BranchCalendar.Where(x => x.ReportDate >= date && x.ReportDate < endDate).GroupBy(x => x.ReportDate).Select(x => new
                    {
                        date = x.Key,
                        count = x.Count()
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = calendar.Select(x => new
                        {
                            date = DateUtility.GetPersianDate(x.date),
                            count = x.count
                        }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// دخیره اطلاعات تقویم برای یک روز خاص برای شعبه های آن روز
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات برای روز و شعبه های باز</param>
        /// <returns>مقدار بازگشتی نتیجه به روز کردن تقویم</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult SaveBranchDate(CalendarViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var date = DateUtility.GetDateTime(model.date);
                    var endDate = date.GetValueOrDefault().AddDays(1);
                    var branchList = db.BranchCalendar.Where(x => x.ReportDate >= date && x.ReportDate < endDate).ToList();

                    var excludeList = branchList.Where(x => !model.branchList.Any(y => y == x.BranchId)).ToList();
                    db.BranchCalendar.RemoveRange(excludeList);

                    var newList = model.branchList.Where(x => !branchList.Any(y => y.BranchId == x)).ToList();
                    newList.ForEach(x =>
                    {
                        db.BranchCalendar.Add(new BranchCalendar()
                        {
                            BranchId = x,
                            ReportDate = date.GetValueOrDefault(),

                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        });
                    });

                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "تقویم با موفقیت به روز شد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// خواندن مقدار شعبه های ثبت شده برای یک روز
        /// </summary>
        /// <param name="date">تاریخ درخواستی</param>
        /// <returns>لیست شعبه های ثبت شده</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult LoadBranchData(string date)
        {
            Response respone;
            try
            {
                var dt = DateUtility.GetDateTime(date);
                using (var db = new KiaGalleryContext())
                {
                    var tomorrow = dt.GetValueOrDefault().AddDays(1);
                    var data = db.BranchCalendar.Where(x => x.ReportDate >= dt && x.ReportDate < tomorrow).Select(x => x.BranchId).ToList();

                    respone = new Common.Response()
                    {
                        status = 200,
                        data = new
                        {
                            branchList = data
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                respone = Core.GetExceptionResponse(ex);
            }
            return Json(respone, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region آمار
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult LoadBranchesSummary(string date)
        {
            Response respone;
            try
            {
                var dt = DateUtility.GetDateTime(date);
                using (var db = new KiaGalleryContext())
                {
                    var tomorrow = dt.GetValueOrDefault().AddDays(1);
                    var data = db.BranchCalendar.Where(x => x.ReportDate >= dt && x.ReportDate < tomorrow)
                        .Select(x => new BranchesSummary()
                        {
                            saleExit = x.DailyReportList.FirstOrDefault().SaleExit,
                            returnedEntry = x.DailyReportList.FirstOrDefault().ReturnedEntry,
                            otherKiaGoldEntry = x.DailyReportList.FirstOrDefault().OtherKiaGoldEntry,
                            saleWeight = x.DailyReportList.FirstOrDefault().SaleWeight,
                            saleWeightPercent = x.DailyReportList.FirstOrDefault().SaleWeightPercent,
                            returnedWeight = x.DailyReportList.FirstOrDefault().ReturnedWeight,
                            returnedWeightPercent = x.DailyReportList.FirstOrDefault().ReturnWeightPercent,
                            otherKiaGoldWeight = x.DailyReportList.FirstOrDefault().OtherKiaGoldWeight,
                            branchId = x.BranchId,
                            branchName = x.Branch.Name,
                            status = x.DailyReportList.FirstOrDefault().Status
                        }).ToList();

                    data.ForEach(x =>
                    {
                        x.status = x.status ?? CalendarStatus.None;
                        x.statusTitle = Enums.GetTitle(x.status);
                        x.date = date.Replace("/", "-");
                        x.totalPrice = x.saleExit - x.returnedEntry - x.otherKiaGoldEntry;
                        x.totalWeight = (x.saleWeight + x.saleWeightPercent) - (x.returnedWeight + x.returnedWeightPercent + x.otherKiaGoldWeight);
                    });

                    respone = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            sale = data.Sum(x => x.saleExit) - data.Sum(x => x.returnedEntry) - data.Sum(x => x.otherKiaGoldEntry),
                            totalSaleExit = data.Sum(x => x.saleExit),
                            totalreturnedEntry = data.Sum(x => x.returnedEntry),
                            totalOtherKiaGoldEntry = data.Sum(x => x.otherKiaGoldEntry),

                            weight = (data.Sum(x => x.saleWeight) + data.Sum(x => x.saleWeightPercent)) - (data.Sum(x => x.returnedWeight) + data.Sum(x => x.otherKiaGoldWeight) + data.Sum(x => x.returnedWeightPercent)),
                            totalSaleWeight = data.Sum(x => x.saleWeight),
                            totalSaleWeightPercent = data.Sum(x => x.saleWeightPercent),
                            totalReturnedWeight = data.Sum(x => x.returnedWeight),
                            totalReturnedWeightPercent = data.Sum(x => x.returnedWeightPercent),
                            totalOtherKiaGoldWeight = data.Sum(x => x.otherKiaGoldWeight),

                            list = data
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                respone = Core.GetExceptionResponse(ex);
            }
            return Json(respone, JsonRequestBehavior.AllowGet);
        }

        [Route("dailyReport/branch/{branchId}/report/{date}")]
        [Authorize(Roles = "admin, dailyReportManage")]
        public ActionResult ReportBranch(int branchId, string date)
        {
            if (string.IsNullOrEmpty(date))
                date = DateUtility.GetPersianDate(DateTime.Now);


            date = date.Replace("-", "/");
            ViewBag.Date = date;

            using (var db = new KiaGalleryContext())
            {
                var dateTime = DateUtility.GetDateTime(date);
                var calendar = db.BranchCalendar.Where(x => x.BranchId == branchId && x.ReportDate == dateTime).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                if (calendar == null)
                {
                    return View("ReturnDateNotValid");
                }

                ViewBag.Branch = db.Branch.Single(x => x.Id == branchId);
                ViewBag.BankList = db.Bank.Where(x => x.BranchId == branchId && x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.CurrencyList = db.Currency.Where(x => x.Active == true).ToList();
            }
            return View();
        }

        [Route("dailyReport/branchAdmin/{branchId}/report/{date}")]
        [Authorize(Roles = "admin, dailyReportManage")]
        public ActionResult ReportBranchAdmin(int branchId, string date)
        {
            if (string.IsNullOrEmpty(date))
                date = DateUtility.GetPersianDate(DateTime.Now);


            date = date.Replace("-", "/");
            ViewBag.Date = date;

            using (var db = new KiaGalleryContext())
            {
                var dateTime = DateUtility.GetDateTime(date);
                var calendar = db.BranchCalendar.Where(x => x.BranchId == branchId && x.ReportDate == dateTime).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                if (calendar == null)
                {
                    return View("ReturnDateNotValid");
                }

                ViewBag.Branch = db.Branch.Single(x => x.Id == branchId);
                ViewBag.BankList = db.Bank.Where(x => x.BranchId == branchId && x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.CurrencyList = db.Currency.Where(x => x.Active == true).ToList();
            }
            return View();
        }

        /// <summary>
        /// خواندن اطلاعات یک تاریخ
        /// </summary>
        /// <param name="date">تاریخ</param>
        /// <returns>نتیجه خواندن اطلاعات تقویم</returns>
        [HttpGet]
        [Route("dailyReport/branch/{branchId}/loadAdmin/{date}")]
        [Authorize(Roles = "admin, dailyReportManage, dailyReportPersonel")]
        public JsonResult LoadAdmin(int branchId, string date)
        {
            Response response;
            var dateTime = DateUtility.GetDateTime(date);
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    var calendar = db.BranchCalendar.Where(x => x.BranchId == branchId && x.ReportDate == dateTime).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                    if (calendar != null)
                    {
                        var entity = db.DailyReport.Where(x => x.BranchId == branchId && x.BranchCalendarId == calendar.Id).SingleOrDefault();
                        if (entity != null)
                        {
                            DailyReportViewModel model = new DailyReportViewModel()
                            {
                                numberSaleFactor = entity.NumberSaleFactor,
                                saleWeight = entity.SaleWeight,
                                saleWeightPercent = entity.SaleWeightPercent,
                                saleEntry = entity.SaleEntry,
                                saleExit = entity.SaleExit,

                                numberReturnedFactor = entity.NumberReturnedFactor,
                                returnedWeight = entity.ReturnedWeight,
                                returnWeightPercent = entity.ReturnWeightPercent,
                                returnedEntry = entity.ReturnedEntry,
                                returnedExit = entity.ReturnedExit,

                                otherCash = entity.OtherCash,
                                cashEntry = entity.CashEntry,
                                cashExit = entity.CashExit,

                                otherCurrency = entity.OtherCurrency,
                                otherCurrencyValue = entity.OtherCurrencyValue,
                                otherCurrencyRialValue = entity.OtherCurrencyRialValue,
                                otherCurrencyRialEntry = entity.OtherCurrencyRialEntry,
                                otherCurrencyRialExit = entity.OtherCurrencyRialExit,

                                inventoryCash = entity.InventoryCash,

                                goldDeficitWeight = entity.GoldDeficitWeight,
                                goldDeficitEntry = entity.GoldDeficitEntry,
                                goldDeficitExit = entity.GoldDeficitExit,

                                giftNumberEntry = entity.GiftNumberEntry,
                                giftNumberExit = entity.GiftNumberExit,
                                giftEntry = entity.GiftEntry,
                                giftExit = entity.GiftExit,

                                checkNumber = entity.CheckNumber,
                                checkEntry = entity.CheckEntry,
                                checkExit = entity.CheckExit,

                                leatherStoneDescriptionEntry = entity.LeatherStoneDescriptionEntry,
                                leatherStoneDescriptionExit = entity.LeatherStoneDescriptionExit,
                                leatherStoneEntry = entity.LeatherStoneEntry,
                                leatherStoneExit = entity.LeatherStoneExit,

                                coinNumber = entity.CoinNumber,
                                coinDescription = entity.CoinDescription,
                                coinEntry = entity.CoinEntry,
                                coinExit = entity.CoinExit,

                                otherKiaGoldWeight = entity.OtherKiaGoldWeight,
                                otherKiaGoldEntry = entity.OtherKiaGoldEntry,
                                otherKiaGoldExit = entity.OtherKiaGoldExit,

                                otherGoldWeight = entity.OtherGoldWeight,
                                otherGoldEntry = entity.OtherGoldEntry,
                                otherGoldExit = entity.OtherGoldExit,

                                creditorCustomerEntry = entity.CreditorCustomerEntry,
                                creditorCustomerExit = entity.CreditorCustomerExit,

                                debtorCustomerEntry = entity.DebtorCustomerEntry,
                                debtorCustomerExit = entity.DebtorCustomerExit,

                                depositBeforeCount = entity.DepositBeforeCount,
                                depositBeforeEntry = entity.DepositBeforeEntry,
                                depositBeforeExit = entity.DepositBeforeExit,

                                depositNewCount = entity.DepositNewCount,
                                depositNewEntry = entity.DepositNewEntry,
                                depositNewExit = entity.DepositNewExit,

                                discountEntry = entity.DiscountEntry,
                                discountExit = entity.DiscountExit,

                                costCourierPostEntry = entity.CostCourierPostEntry,
                                costCourierPostExit = entity.CostCourierPostExit,

                                costEntry = entity.CostEntry,
                                costExit = entity.CostExit,

                                dailyReportBankList = entity.DailyReportBankList.Select(x => new DailyReportBankViewModel()
                                {
                                    bankId = x.BankId,
                                    entry = x.Entry,
                                    exit = x.Exit
                                }).ToList(),

                                dailyReportCurrencyList = entity.DailyReportCurrencyList.Select(x => new DailyReportCurrencyViewModel()
                                {
                                    currencyId = x.CurrencyId,
                                    value = x.Value,
                                    rialValue = x.RialValue,
                                    rialEntry = x.RialEntry,
                                    rialExit = x.RialExit
                                }).ToList(),

                                logListViewModel = entity.DailyReportLogList.Select(x => new DailyReportLogViewModel()
                                {
                                    id = x.Id,
                                    status = x.Status,
                                    userId = x.UserId,
                                    userFullName = x.User.FirstName + " " + x.User.LastName,
                                    date = x.Date
                                }).ToList()
                            };

                            model.logListViewModel.ForEach(x =>
                            {
                                x.statusTitle = Enums.GetTitle(x.status);
                                x.persianDate = DateUtility.GetPersianDateTime(x.date);
                            });

                            response = new Response
                            {
                                status = 200,
                                data = model
                            };
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 201,
                                message = "گزارش روزانه در این تاریخ ثبت نشده."
                            };
                        }
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "مجاز به ثبت گزارش روزانه در این تاریخ نمی باشید."
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// خواندن اطلاعات یک تاریخ
        /// </summary>
        /// <param name="date">تاریخ</param>
        /// <returns>نتیجه خواندن اطلاعات تقویم</returns>
        [HttpGet]
        [Route("dailyReport/branch/{branchId}/load/{date}")]
        [Authorize(Roles = "admin, dailyReportManage, dailyReportPersonel")]
        public JsonResult Load(int branchId, string date)
        {
            Response response;
            var dateTime = DateUtility.GetDateTime(date);
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var calendar = db.BranchCalendar.Where(x => x.BranchId == branchId && x.ReportDate == dateTime).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                    if (calendar != null)
                    {
                        var entity = db.DailyReport.Where(x => x.BranchId == branchId && x.BranchCalendarId == calendar.Id).SingleOrDefault();
                        if (entity != null)
                        {
                            DailyReportViewModel model = new DailyReportViewModel()
                            {
                                numberSaleFactor = entity.NumberSaleFactor,
                                saleWeight = entity.SaleWeight,
                                saleWeightPercent = entity.SaleWeightPercent,
                                saleEntry = entity.SaleEntry,
                                saleExit = entity.SaleExit,

                                numberReturnedFactor = entity.NumberReturnedFactor,
                                returnedWeight = entity.ReturnedWeight,
                                returnWeightPercent = entity.ReturnWeightPercent,
                                returnedEntry = entity.ReturnedEntry,
                                returnedExit = entity.ReturnedExit,

                                otherCash = entity.OtherCash,
                                cashEntry = entity.CashEntry,
                                cashExit = entity.CashExit,

                                otherCurrency = entity.OtherCurrency,
                                otherCurrencyValue = entity.OtherCurrencyValue,
                                otherCurrencyRialValue = entity.OtherCurrencyRialValue,
                                otherCurrencyRialEntry = entity.OtherCurrencyRialEntry,
                                otherCurrencyRialExit = entity.OtherCurrencyRialExit,

                                inventoryCash = entity.InventoryCash,

                                goldDeficitWeight = entity.GoldDeficitWeight,
                                goldDeficitEntry = entity.GoldDeficitEntry,
                                goldDeficitExit = entity.GoldDeficitExit,

                                giftNumberEntry = entity.GiftNumberEntry,
                                giftNumberExit = entity.GiftNumberExit,
                                giftEntry = entity.GiftEntry,
                                giftExit = entity.GiftExit,

                                checkNumber = entity.CheckNumber,
                                checkEntry = entity.CheckEntry,
                                checkExit = entity.CheckExit,

                                leatherStoneDescriptionEntry = entity.LeatherStoneDescriptionEntry,
                                leatherStoneDescriptionExit = entity.LeatherStoneDescriptionExit,
                                leatherStoneEntry = entity.LeatherStoneEntry,
                                leatherStoneExit = entity.LeatherStoneExit,

                                coinNumber = entity.CoinNumber,
                                coinDescription = entity.CoinDescription,
                                coinEntry = entity.CoinEntry,
                                coinExit = entity.CoinExit,

                                otherKiaGoldWeight = entity.OtherKiaGoldWeight,
                                otherKiaGoldEntry = entity.OtherKiaGoldEntry,
                                otherKiaGoldExit = entity.OtherKiaGoldExit,

                                otherGoldWeight = entity.OtherGoldWeight,
                                otherGoldEntry = entity.OtherGoldEntry,
                                otherGoldExit = entity.OtherGoldExit,

                                creditorCustomerEntry = entity.CreditorCustomerEntry,
                                creditorCustomerExit = entity.CreditorCustomerExit,

                                debtorCustomerEntry = entity.DebtorCustomerEntry,
                                debtorCustomerExit = entity.DebtorCustomerExit,

                                depositBeforeCount = entity.DepositBeforeCount,
                                depositBeforeEntry = entity.DepositBeforeEntry,
                                depositBeforeExit = entity.DepositBeforeExit,

                                depositNewCount = entity.DepositNewCount,
                                depositNewEntry = entity.DepositNewEntry,
                                depositNewExit = entity.DepositNewExit,

                                discountEntry = entity.DiscountEntry,
                                discountExit = entity.DiscountExit,

                                costCourierPostEntry = entity.CostCourierPostEntry,
                                costCourierPostExit = entity.CostCourierPostExit,

                                costEntry = entity.CostEntry,
                                costExit = entity.CostExit,

                                dailyReportBankList = entity.DailyReportBankList.Select(x => new DailyReportBankViewModel()
                                {
                                    bankId = x.BankId,
                                    entry = x.Entry,
                                    exit = x.Exit
                                }).ToList(),

                                dailyReportCurrencyList = entity.DailyReportCurrencyList.Select(x => new DailyReportCurrencyViewModel()
                                {
                                    currencyId = x.CurrencyId,
                                    value = x.Value,
                                    rialValue = x.RialValue,
                                    rialEntry = x.RialEntry,
                                    rialExit = x.RialExit
                                }).ToList(),

                                logListViewModel = entity.DailyReportLogList.Select(x => new DailyReportLogViewModel()
                                {
                                    id = x.Id,
                                    status = x.Status,
                                    userId = x.UserId,
                                    userFullName = x.User.FirstName + " " + x.User.LastName,
                                    date = x.Date
                                }).ToList()
                            };

                            model.logListViewModel.ForEach(x =>
                            {
                                x.statusTitle = Enums.GetTitle(x.status);
                                x.persianDate = DateUtility.GetPersianDateTime(x.date);
                            });

                            response = new Response
                            {
                                status = 200,
                                data = model
                            };
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 201,
                                message = "گزارش روزانه در این تاریخ ثبت نشده."
                            };
                        }
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "مجاز به ثبت گزارش روزانه در این تاریخ نمی باشید."
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// صفحه گزارش برای آمار کلی بر اساس تاریخ و شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult TotalReport()
        {
            return View();
        }

        /// <summary>
        /// تهیه گزارش کلی از اطلاعات شعب
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات جهت تهیه گزارش</param>
        /// <returns>گزارش ایجاد شده</returns>
        public JsonResult MakeReport(TotalReportViewModel model)
        {
            Response response;
            try
            {
                if (string.IsNullOrEmpty(model.fromDate) || string.IsNullOrEmpty(model.toDate))
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "بازه تاریخ گزارش می بایست مشخص شود."
                    };
                }
                else
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var fromDate = DateUtility.GetDateTime(model.fromDate);
                        var toDate = DateUtility.GetDateTime(model.toDate);
                        var query = db.DailyReport.Where(x => x.BranchCalendar.ReportDate >= fromDate && x.BranchCalendar.ReportDate <= toDate);

                        if (model.branchList != null && model.branchList.Count(x => x > 0) > 0)
                        {
                            query = query.Where(x => model.branchList.Any(y => y == x.BranchId));
                        }

                        var data = query.GroupBy(x => x.Branch).Select(x => new BranchesSummary()
                        {
                            branchId = x.Key.Id,
                            branchName = x.Key.Name,
                            date = x.Count().ToString(),

                            saleExit = x.Sum(y => y.SaleExit),
                            returnedEntry = x.Sum(y => y.ReturnedEntry),
                            otherKiaGoldEntry = x.Sum(y => y.OtherKiaGoldEntry),
                            otherGoldEntry = x.Sum(y => y.OtherGoldEntry),
                            totalPrice = x.Sum(y => y.SaleExit) - x.Sum(y => y.ReturnedEntry) - x.Sum(y => y.OtherKiaGoldEntry),

                            saleWeight = x.Sum(y => y.SaleWeight),
                            saleWeightPercent = x.Sum(y => y.SaleWeightPercent),
                            returnedWeight = x.Sum(y => y.ReturnedWeight),
                            returnedWeightPercent = x.Sum(y => y.ReturnWeightPercent),
                            otherKiaGoldWeight = x.Sum(y => y.OtherKiaGoldWeight),
                            otherGoldWeight = x.Sum(y => y.OtherGoldWeight),
                            totalWeight = (x.Sum(y => y.SaleWeight) + x.Sum(y => y.SaleWeightPercent)) - (x.Sum(y => y.ReturnedWeight) + x.Sum(y => y.ReturnWeightPercent) + x.Sum(y => y.OtherKiaGoldWeight))
                        }).ToList();

                        response = new Response()
                        {
                            status = 200,
                            data = new
                            {
                                sale = data.Sum(x => x.saleExit) - data.Sum(x => x.returnedEntry) - data.Sum(x => x.otherKiaGoldEntry) - data.Sum(x => x.otherGoldEntry),
                                totalSaleExit = data.Sum(x => x.saleExit),
                                totalreturnedEntry = data.Sum(x => x.returnedEntry),
                                totalOtherGoldEntry = data.Sum(x => x.otherGoldEntry),
                                totalOtherKiaGoldEntry = data.Sum(x => x.otherKiaGoldEntry),

                                weight = ((data.Sum(x => x.saleWeight) + data.Sum(x => x.saleWeightPercent)) - (data.Sum(x => x.returnedWeight) + data.Sum(x => x.returnedWeightPercent) + data.Sum(x => x.otherKiaGoldWeight))) - data.Sum(x => x.otherGoldWeight),
                                totalSaleWeight = data.Sum(x => x.saleWeight),
                                totalSaleWeightPercent = data.Sum(x => x.saleWeightPercent),
                                totalReturnedWeight = data.Sum(x => x.returnedWeight),
                                totalReturnedWeightPercent = data.Sum(x => x.returnedWeightPercent),
                                totalOtherGoldWeight = data.Sum(x => x.otherGoldWeight),
                                totalOtherKiaGoldWeight = data.Sum(x => x.otherKiaGoldWeight),

                                list = data
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ثبت گزارش برای شعبه توسط شعبه
        /// <summary>
        /// تقویم گزارش روزانه شعبه
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, dailyReport")]
        public ActionResult Archive()
        {
            PersianCalendar helper = new PersianCalendar();
            var year = helper.GetYear(DateTime.Now);
            List<int> yearList = new List<int>();
            for (int i = year - 5; i <= year + 5; i++)
            {
                yearList.Add(i);
            }
            ViewBag.Year = year;
            ViewBag.YearList = yearList;

            return View();
        }

        /// <summary>
        /// دریافت خلاصه آرشیو ثبت شده شعبه 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, dailyReport")]
        public JsonResult GetDailyArchive(int year, int month)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                var persianDate = year + "/" + month + "/1";
                var date = DateUtility.GetDateTime(persianDate);
                var daysCount = DateUtility.GetMonthDayCount(year, month);

                using (var db = new KiaGalleryContext())
                {
                    var endDate = date.GetValueOrDefault().AddDays(daysCount + 1);

                    var calendar = db.BranchCalendar.Where(x => x.BranchId == currentUser.BranchId && x.ReportDate >= date && x.ReportDate < endDate).ToList();

                    var dailyReport = db.DailyReport.Where(x => x.BranchId == currentUser.BranchId && x.BranchCalendar.ReportDate >= date && x.BranchCalendar.ReportDate < endDate).ToList();

                    var data = dailyReport.Select(x => new DailyReportSummaryViewModel
                    {
                        date = x.BranchCalendar.ReportDate,
                        persianDate = DateUtility.GetPersianDate(x.BranchCalendar.ReportDate),
                        status = x.Status,
                        statusTitle = Enums.GetTitle(x.Status),
                        saleExit = x.SaleExit - x.ReturnedEntry - x.OtherKiaGoldEntry,
                        saleWeight = x.SaleWeight - x.ReturnedWeight - x.OtherKiaGoldWeight
                    }).ToList();

                    data.AddRange(calendar.Where(x => !dailyReport.Any(y => y.BranchCalendar.ReportDate == x.ReportDate)).Select(x => new DailyReportSummaryViewModel
                    {
                        date = x.ReportDate,
                        persianDate = DateUtility.GetPersianDate(x.ReportDate),
                        status = CalendarStatus.None,
                        statusTitle = Enums.GetTitle(CalendarStatus.None),
                        saleWeight = null,
                        saleExit = null
                    }).ToList());

                    data = data.OrderBy(x => x.date).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = data
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// صفحه ثبت گزارش برای شعبه
        /// </summary>
        /// <param name="date">تاریخ مورد نظر</param>
        /// <returns>بازگشت دادن صفحه مورد نظر</returns>
        [Route("dailyReport/report/{date?}")]
        [Authorize(Roles = "admin, dailyReport, dailyReportPersonel")]
        public ActionResult Report(string date)
        {
            var currentUser = GetAuthenticatedUser();

            if (string.IsNullOrEmpty(date))
                date = DateUtility.GetPersianDate(DateTime.Now);
            date = date.Replace("-", "/");
            ViewBag.Date = date;

            using (var db = new KiaGalleryContext())
            {
                var dateTime = DateUtility.GetDateTime(date);
                var calendar = db.BranchCalendar.Where(x => x.BranchId == currentUser.BranchId && x.ReportDate == dateTime).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                if (calendar == null || calendar.ReportDate > DateTime.Now || calendar.ReportDate < DateTime.Now.AddDays(-30))
                {
                    return View("ReturnDateNotValid");
                }
                ViewBag.Branch = db.Branch.Single(x => x.Id == currentUser.BranchId);
                ViewBag.BankList = db.Bank.Where(x => x.BranchId == currentUser.BranchId && x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.CurrencyList = db.Currency.Where(x => x.Active == true).ToList();
            }
            return View();
        }

        /// <summary>
        /// صفحه ثبت گزارش برای شعبه
        /// </summary>
        /// <param name="date">تاریخ مورد نظر</param>
        /// <returns>بازگشت دادن صفحه مورد نظر</returns>
        [Route("dailyReport/pdf/report/{date?}")]
        [Authorize(Roles = "admin, dailyReport")]
        public ActionResult ReportPdf(string date)
        {
            var currentUser = GetAuthenticatedUser();

            if (string.IsNullOrEmpty(date))
                date = DateUtility.GetPersianDate(DateTime.Now);
            date = date.Replace("-", "/");
            ViewBag.Date = date;

            using (var db = new KiaGalleryContext())
            {
                var dateTime = DateUtility.GetDateTime(date);
                var calendar = db.BranchCalendar.Where(x => x.BranchId == currentUser.BranchId && x.ReportDate == dateTime).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                if (calendar != null)
                {
                    var entity = db.DailyReport.Where(x => x.BranchId == currentUser.BranchId && x.BranchCalendarId == calendar.Id).SingleOrDefault();
                    if (entity != null)
                    {
                        StiReport report = new StiReport();
                        report.Load(Server.MapPath("~/Report/DailyReport/BranchDailyReport.mrt"));
                        report.Dictionary.Databases.Clear();
                        report.ScriptLanguage = StiReportLanguageType.CSharp;
                        #region Variable
                        report.Dictionary.Variables["Date"].Value = date;
                        report.Dictionary.Variables["Branch"].Value = entity.Branch.Name;

                        var totalSale = entity.SaleExit - entity.SaleEntry;
                        report.Dictionary.Variables["NumberSaleFactor"].Value = entity.NumberSaleFactor.ToString().ToPersianNumber() + " عدد";
                        report.Dictionary.Variables["SaleWeight"].Value = entity.SaleWeight.ToString().ToPersianNumber() + " گرم";
                        report.Dictionary.Variables["SaleWeightPercent"].Value = entity.SaleWeightPercent.ToString().ToPersianNumber() + " گرم";
                        report.Dictionary.Variables["SaleExit"].Value = entity.SaleExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["SaleEntry"].Value = entity.SaleEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["SaleRemaining"].Value = totalSale.ToString("N0").ToPersianNumber();

                        var totalReturned = totalSale + (entity.ReturnedExit - entity.ReturnedEntry);
                        report.Dictionary.Variables["ReturnedWeight"].Value = entity.ReturnedWeight.ToString().ToPersianNumber() + " گرم";
                        report.Dictionary.Variables["ReturnWeightPercent"].Value = entity.ReturnWeightPercent.ToString().ToPersianNumber() + " گرم";
                        report.Dictionary.Variables["ReturnedExit"].Value = entity.ReturnedExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["ReturnedEntry"].Value = entity.ReturnedEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["ReturnedRemaining"].Value = totalReturned.ToString("N0").ToPersianNumber();

                        var totalBank = totalReturned + (entity.DailyReportBankList.Sum(x => x.Exit) - entity.DailyReportBankList.Sum(x => x.Entry));
                        //report.Dictionary.Variables["BankExit"].ValueObject = entity.DailyReportBankList.Select(x => x.Exit.ToString("N0")).ToList();
                        //report.Dictionary.Variables["BankEntry"].Value = entity.DailyReportBankList.Sum(x => x.Entry).ToString("N0");
                        //report.Dictionary.Variables["BankRemaining"].Value = totalBank.ToString("N0");

                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Name");
                        dataTable.Columns.Add("Exit");
                        dataTable.Columns.Add("Entry");
                        dataTable.Columns.Add("Remaining");
                        var bank = totalReturned;
                        for (int j = 0; j < entity.DailyReportBankList.Count; j++)
                        {
                            bank = bank + (entity.DailyReportBankList[j].Exit - entity.DailyReportBankList[j].Entry);
                            DataRow row = dataTable.NewRow();
                            row["Name"] = entity.DailyReportBankList[j].Bank.Name;
                            row["Exit"] = entity.DailyReportBankList[j].Exit.ToString("N0").ToPersianNumber();
                            row["Entry"] = entity.DailyReportBankList[j].Entry.ToString("N0").ToPersianNumber();
                            row["Remaining"] = bank.ToString("N0").ToPersianNumber();

                            dataTable.Rows.Add(row);
                        }

                        var totalCash = totalBank + (entity.CashExit - entity.CashEntry);
                        report.Dictionary.Variables["OtherCash"].Value = entity.OtherCash.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CashExit"].Value = entity.CashExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CashEntry"].Value = entity.CashEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CashRemaining"].Value = totalCash.ToString("N0").ToPersianNumber();

                        var totalCurrency = totalCash + (entity.DailyReportCurrencyList.Sum(x => x.RialExit) - entity.DailyReportCurrencyList.Sum(x => x.RialEntry));
                        //report.Dictionary.Variables["CurrencyExit"].Value = entity.CashExit.ToString("N0");
                        //report.Dictionary.Variables["CurrencyEntry"].Value = entity.CashEntry.ToString("N0");
                        //report.Dictionary.Variables["CurrencyRemaining"].Value = totalCurrency.ToString("N0");

                        DataTable currencyDataTable = new DataTable();
                        currencyDataTable.Columns.Add("Name");
                        currencyDataTable.Columns.Add("Exit");
                        currencyDataTable.Columns.Add("Entry");
                        currencyDataTable.Columns.Add("Remaining");
                        var currency = totalCurrency;
                        for (int j = 0; j < entity.DailyReportCurrencyList.Count; j++)
                        {
                            currency = currency + (entity.DailyReportCurrencyList[j].RialExit - entity.DailyReportCurrencyList[j].RialEntry);
                            DataRow row = currencyDataTable.NewRow();
                            row["Name"] = entity.DailyReportCurrencyList[j].Currency.Name;
                            row["Exit"] = entity.DailyReportCurrencyList[j].RialExit.ToString("N0").ToPersianNumber();
                            row["Entry"] = entity.DailyReportCurrencyList[j].RialEntry.ToString("N0").ToPersianNumber();
                            row["Remaining"] = currency.ToString("N0").ToPersianNumber();

                            currencyDataTable.Rows.Add(row);
                        }

                        var totalOtherCurrency = totalCurrency + (entity.OtherCurrencyRialExit - entity.OtherCurrencyRialEntry);
                        report.Dictionary.Variables["OtherCurrencyExit"].Value = entity.OtherCurrencyRialExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["OtherCurrencyEntry"].Value = entity.OtherCurrencyRialEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["OtherCurrencyRemaining"].Value = totalOtherCurrency.ToString("N0").ToPersianNumber();

                        report.Dictionary.Variables["InventoryCash"].Value = entity.InventoryCash.ToString("N0").ToPersianNumber();

                        var totalGoldDeficit = totalOtherCurrency + (entity.GoldDeficitExit - entity.GoldDeficitEntry);
                        report.Dictionary.Variables["GoldDeficitWeight"].Value = entity.GoldDeficitWeight.ToString().ToPersianNumber();
                        report.Dictionary.Variables["GoldDeficitExit"].Value = entity.GoldDeficitExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["GoldDeficitEntry"].Value = entity.GoldDeficitEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["GoldDeficitRemaining"].Value = totalGoldDeficit.ToString("N0").ToPersianNumber();

                        var totalGift = totalGoldDeficit + (entity.GiftExit - entity.GiftEntry);
                        report.Dictionary.Variables["GiftNumberEntry"].Value = entity.GiftNumberEntry.ToString().ToPersianNumber();
                        report.Dictionary.Variables["GiftNumberExit"].Value = entity.GiftNumberExit.ToString().ToPersianNumber();
                        report.Dictionary.Variables["GiftExit"].Value = entity.GiftExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["GiftEntry"].Value = entity.GiftEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["GiftRemaining"].Value = totalGift.ToString("N0").ToPersianNumber();



                        var totalCheck = totalGift + (entity.CheckExit - entity.CheckEntry);
                        report.Dictionary.Variables["CheckNumber"].Value = entity.CheckNumber.ToString().ToPersianNumber();
                        report.Dictionary.Variables["CheckExit"].Value = entity.CheckExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CheckEntry"].Value = entity.CheckEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CheckRemaining"].Value = totalCheck.ToString("N0").ToPersianNumber();

                        var totalLeatherStone = totalCheck + (entity.LeatherStoneExit - entity.LeatherStoneEntry);
                        report.Dictionary.Variables["LeatherStoneDescEntry"].Value = entity.LeatherStoneDescriptionEntry?.ToPersianNumber();
                        report.Dictionary.Variables["LeatherStoneDescExit"].Value = entity.LeatherStoneDescriptionExit?.ToPersianNumber();
                        report.Dictionary.Variables["LeatherStoneExit"].Value = entity.LeatherStoneExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["LeatherStoneEntry"].Value = entity.LeatherStoneEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["LeatherStoneRemaining"].Value = totalLeatherStone.ToString("N0").ToPersianNumber();

                        var totalCoin = totalLeatherStone + (entity.CoinExit - entity.CoinEntry);
                        report.Dictionary.Variables["CoinNumber"].Value = entity.CoinNumber.ToString().ToPersianNumber();
                        report.Dictionary.Variables["CoinExit"].Value = entity.CoinExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CoinEntry"].Value = entity.CoinEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CoinRemaining"].Value = totalCoin.ToString("N0").ToPersianNumber();

                        var totalOtherKiaGold = totalCoin + (entity.OtherKiaGoldExit - entity.OtherKiaGoldEntry);
                        report.Dictionary.Variables["OtherKiaGoldWeight"].Value = entity.OtherKiaGoldWeight.ToString().ToPersianNumber();
                        report.Dictionary.Variables["OtherKiaGoldExit"].Value = entity.OtherKiaGoldExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["OtherKiaGoldEntry"].Value = entity.OtherKiaGoldEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["OtherKiaGoldRemaining"].Value = totalOtherKiaGold.ToString("N0").ToPersianNumber();

                        var totalOtherGold = totalOtherKiaGold + (entity.OtherGoldExit - entity.OtherGoldEntry);
                        report.Dictionary.Variables["OtherGoldWeight"].Value = entity.OtherGoldWeight.ToString().ToPersianNumber();
                        report.Dictionary.Variables["OtherGoldExit"].Value = entity.OtherGoldExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["OtherGoldEntry"].Value = entity.OtherGoldEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["OtherGoldRemaining"].Value = totalOtherGold.ToString("N0").ToPersianNumber();

                        var totalCreditorCustomer = totalOtherGold + (entity.CreditorCustomerExit - entity.CreditorCustomerEntry);
                        report.Dictionary.Variables["CreditorCustomerExit"].Value = entity.CreditorCustomerExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CreditorCustomerEntry"].Value = entity.CreditorCustomerEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CreditorCustomerRemaining"].Value = totalCreditorCustomer.ToString("N0").ToPersianNumber();

                        var totalDebtorCustomer = totalCreditorCustomer + (entity.DebtorCustomerExit - entity.DebtorCustomerEntry);
                        report.Dictionary.Variables["DebtorCustomerExit"].Value = entity.DebtorCustomerExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["DebtorCustomerEntry"].Value = entity.DebtorCustomerEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["DebtorCustomerRemaining"].Value = totalDebtorCustomer.ToString("N0").ToPersianNumber();

                        var totalDepositBefore = totalDebtorCustomer + (entity.DepositBeforeExit - entity.DepositBeforeEntry);
                        report.Dictionary.Variables["DepositBeforeCount"].Value = entity.DepositBeforeCount.ToString().ToPersianNumber();
                        report.Dictionary.Variables["DepositBeforeExit"].Value = entity.DepositBeforeExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["DepositBeforeEntry"].Value = entity.DepositBeforeEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["DepositBeforeRemaining"].Value = totalDepositBefore.ToString("N0").ToPersianNumber();

                        var totalDepositNew = totalDepositBefore + (entity.DepositNewExit - entity.DepositNewEntry);
                        report.Dictionary.Variables["DepositNewCount"].Value = entity.DepositNewCount.ToString().ToPersianNumber();
                        report.Dictionary.Variables["DepositNewExit"].Value = entity.DepositNewExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["DepositNewEntry"].Value = entity.DepositNewEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["DepositNewRemaining"].Value = totalDepositNew.ToString("N0").ToPersianNumber();

                        var totalLoyality = totalDepositNew + (entity.LoyalityExit - entity.LoyalityEntry);
                        report.Dictionary.Variables["LoyalityExit"].Value = entity.LoyalityExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["LoyalityEntry"].Value = entity.LoyalityEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["LoyalityRemaining"].Value = totalLoyality.ToString("N0").ToPersianNumber();

                        var totalDiscount = totalLoyality + (entity.DiscountExit - entity.DiscountEntry);
                        report.Dictionary.Variables["DiscountExit"].Value = entity.DiscountExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["DiscountEntry"].Value = entity.DiscountEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["DiscountRemaining"].Value = totalDiscount.ToString("N0").ToPersianNumber();

                        var totalCost = totalDiscount + (entity.CostExit - entity.CostEntry);
                        report.Dictionary.Variables["CostCourierPostExit"].Value = entity.CostExit.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CostCourierPostEntry"].Value = entity.CostEntry.ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["CostCourierPostRemaining"].Value = totalCost.ToString("N0").ToPersianNumber();

                        report.Dictionary.Variables["TotalProfit"].Value = (entity.SaleExit * 0.59).ToString("N0").ToPersianNumber();
                        report.Dictionary.Variables["TotalValue"].Value = (entity.SaleExit * 0.818).ToString("N0").ToPersianNumber();
                        #endregion
                        report.Dictionary.Databases.Clear();
                        report.RegData("BankDataSource", dataTable);
                        report.RegData("CurrencyDataSource", currencyDataTable);
                        report.Compile();
                        report.Render(false);

                        MemoryStream stream = new MemoryStream();

                        StiPdfExportSettings settings = new StiPdfExportSettings();

                        StiPdfExportService service = new StiPdfExportService();
                        service.ExportPdf(report, stream, settings);

                        Response.Buffer = true;
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + entity.Branch.Alias + " " + date.Replace("/", "-") + ".pdf\"");
                        Response.ContentEncoding = Encoding.UTF8;
                        Response.AddHeader("Content-Length", stream.Length.ToString());
                        Response.BinaryWrite(stream.ToArray());
                        Response.End();
                        return new FileStreamResult(stream, "application/pdf");
                    }
                }

            }

            return HttpNotFound();
        }

        /// <summary>
        /// ذخیره گزارش شعبه
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin, dailyReport, dailyReportPersonel")]
        public JsonResult Save(SaveDailyReportViewModel model)
        {
            try
            {
                Response response;
                var currentUser = GetAuthenticatedUser();
                var dateTime = DateUtility.GetDateTime(model.date);
                if (User.IsInRole("dailyReportPersonel"))
                {
                    if (dateTime != DateTime.Today)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "شما مجاز به انجام این کار نیستید."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                using (var db = new KiaGalleryContext())
                {
                    var date = DateUtility.GetDateTime(model.date);
                    var calendar = db.BranchCalendar.Where(x => x.BranchId == currentUser.BranchId && x.ReportDate == date).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                    if (calendar != null)
                    {
                        DailyReport entity = null;
                        string json = JsonConvert.SerializeObject(model);
                        entity = db.DailyReport.Where(x => x.BranchId == currentUser.BranchId && x.BranchCalendarId == calendar.Id).SingleOrDefault();

                        if (entity == null)
                        {
                            entity = new DailyReport();
                        }

                        entity.BranchId = currentUser.BranchId.GetValueOrDefault();
                        entity.BranchCalendarId = calendar.Id;
                        entity.Status = CheckDailyReportStatus(model.report) ? Model.CalendarStatus.Submit : Model.CalendarStatus.Draft;

                        var report = model.report;

                        entity.NumberSaleFactor = report.numberSaleFactor;
                        entity.SaleWeight = report.saleWeight;
                        entity.SaleWeightPercent = report.saleWeightPercent;
                        entity.SaleEntry = report.saleEntry;
                        entity.SaleExit = report.saleExit;

                        entity.NumberReturnedFactor = report.numberReturnedFactor;
                        entity.ReturnedWeight = report.returnedWeight;
                        entity.ReturnWeightPercent = report.returnWeightPercent;
                        entity.ReturnedEntry = report.returnedEntry;
                        entity.ReturnedExit = report.returnedExit;

                        entity.OtherCash = report.otherCash;
                        entity.CashEntry = report.cashEntry;
                        entity.CashExit = report.cashExit;

                        entity.OtherCurrency = report.otherCurrency;
                        entity.OtherCurrencyValue = report.otherCurrencyValue;
                        entity.OtherCurrencyRialValue = report.otherCurrencyRialValue;
                        entity.OtherCurrencyRialEntry = report.otherCurrencyRialEntry;
                        entity.OtherCurrencyRialExit = report.otherCurrencyRialExit;

                        entity.InventoryCash = report.inventoryCash;

                        entity.GoldDeficitWeight = report.goldDeficitWeight;
                        entity.GoldDeficitEntry = report.goldDeficitEntry;
                        entity.GoldDeficitExit = report.goldDeficitExit;

                        entity.GiftNumberEntry = report.giftNumberEntry;
                        entity.GiftNumberExit = report.giftNumberExit;
                        entity.GiftEntry = report.giftEntry;
                        entity.GiftExit = report.giftExit;

                        entity.LoyalityEntry = report.loyalityEntry;
                        entity.LoyalityExit = report.loyalityExit;

                        entity.CheckNumber = report.checkNumber;
                        entity.CheckEntry = report.checkEntry;
                        entity.CheckExit = report.checkExit;

                        entity.LeatherStoneDescriptionEntry = report.leatherStoneDescriptionEntry;
                        entity.LeatherStoneDescriptionExit = report.leatherStoneDescriptionExit;
                        entity.LeatherStoneEntry = report.leatherStoneEntry;
                        entity.LeatherStoneExit = report.leatherStoneExit;

                        entity.CoinNumber = report.coinNumber;
                        entity.CoinDescription = report.coinDescription;
                        entity.CoinEntry = report.coinEntry;
                        entity.CoinExit = report.coinExit;

                        entity.OtherKiaGoldWeight = report.otherKiaGoldWeight;
                        entity.OtherKiaGoldEntry = report.otherKiaGoldEntry;
                        entity.OtherKiaGoldExit = report.otherKiaGoldExit;

                        entity.OtherGoldWeight = report.otherGoldWeight;
                        entity.OtherGoldEntry = report.otherGoldEntry;
                        entity.OtherGoldExit = report.otherGoldExit;

                        entity.CreditorCustomerEntry = report.creditorCustomerEntry;
                        entity.CreditorCustomerExit = report.creditorCustomerExit;

                        entity.DebtorCustomerEntry = report.debtorCustomerEntry;
                        entity.DebtorCustomerExit = report.debtorCustomerExit;

                        entity.DepositBeforeCount = report.depositBeforeCount;
                        entity.DepositBeforeEntry = report.depositBeforeEntry;
                        entity.DepositBeforeExit = report.depositBeforeExit;

                        entity.DepositNewCount = report.depositNewCount;
                        entity.DepositNewEntry = report.depositNewEntry;
                        entity.DepositNewExit = report.depositNewExit;

                        entity.DiscountEntry = report.discountEntry;
                        entity.DiscountExit = report.discountExit;

                        entity.CostCourierPostEntry = report.costCourierPostEntry;
                        entity.CostCourierPostExit = report.costCourierPostExit;

                        entity.CostEntry = report.costEntry;
                        entity.CostExit = report.costExit;

                        report.dailyReportBankList.ForEach(x =>
                        {
                            var bank = entity.DailyReportBankList.SingleOrDefault(y => y.BankId == x.bankId);
                            if (bank != null)
                            {
                                bank.Entry = x.entry;
                                bank.Exit = x.exit;
                            }
                            else
                            {
                                bank = new DailyReportBank
                                {
                                    BankId = x.bankId,
                                    DailyReport = entity,
                                    Entry = x.entry,
                                    Exit = x.exit
                                };
                                entity.DailyReportBankList.Add(bank);
                            }
                        });

                        report.dailyReportCurrencyList.ForEach(x =>
                        {
                            var currency = entity.DailyReportCurrencyList.SingleOrDefault(y => y.CurrencyId == x.currencyId);
                            if (currency != null)
                            {
                                currency.Value = x.value;
                                currency.RialValue = x.rialValue;
                                currency.RialEntry = x.rialEntry;
                                currency.RialExit = x.rialExit;
                            }
                            else
                            {
                                currency = new DailyReportCurrency
                                {
                                    CurrencyId = x.currencyId,
                                    DailyReport = entity,
                                    Value = x.value,
                                    RialValue = x.rialValue,
                                    RialEntry = x.rialEntry,
                                    RialExit = x.rialExit
                                };
                                entity.DailyReportCurrencyList.Add(currency);
                            }
                        });

                        if (entity.Id == 0)
                        {
                            db.DailyReport.Add(entity);
                        }

                        entity.Sent = false;

                        DailyReportLog log = new DailyReportLog
                        {
                            DailyReport = entity,
                            UserId = currentUser.Id,
                            Date = DateTime.Now,
                            Status = CheckDailyReportStatus(model.report) ? Model.CalendarStatus.Submit : Model.CalendarStatus.Draft,
                            PrevData = json,
                            Ip = Request.UserHostAddress
                        };
                        db.DailyReportLog.Add(log);

                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "اطلاعات با موفقیت ذخیره شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "شما مجاز به ثبت گزارش روزانه در این تاریخ نیستید."
                        };
                    }
                }

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Core.GetExceptionResponse(ex), JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// ذخیره گزارش شعبه
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin, dailyReport, dailyReportPersonel,dailyReportManage")]
        public JsonResult SaveReportAdmin(SaveDailyReportViewModel model)
        {
            try
            {
                Response response;
                var currentUser = GetAuthenticatedUser();
                var dateTime = DateUtility.GetDateTime(model.date);
                if (User.IsInRole("dailyReportPersonel"))
                {
                    if (dateTime != DateTime.Today)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "شما مجاز به انجام این کار نیستید."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                using (var db = new KiaGalleryContext())
                {
                    var date = DateUtility.GetDateTime(model.date);
                    var calendar = db.BranchCalendar.Where(x => x.BranchId == model.branchId && x.ReportDate == date).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                    if (calendar != null)
                    {
                        DailyReport entity = null;
                        string json = JsonConvert.SerializeObject(model);
                        entity = db.DailyReport.Where(x => x.BranchId == model.branchId && x.BranchCalendarId == calendar.Id).SingleOrDefault();

                        if (entity == null)
                        {
                            entity = new DailyReport();
                        }

                        entity.BranchId = model.branchId;
                        entity.BranchCalendarId = calendar.Id;
                        entity.Status = CheckDailyReportStatus(model.report) ? Model.CalendarStatus.Submit : Model.CalendarStatus.Draft;

                        var report = model.report;

                        entity.NumberSaleFactor = report.numberSaleFactor;
                        entity.SaleWeight = report.saleWeight;
                        entity.SaleEntry = report.saleEntry;
                        entity.SaleExit = report.saleExit;
                        entity.ReturnWeightPercent = report.returnWeightPercent;
                        entity.SaleWeightPercent = report.saleWeightPercent;
                        entity.NumberReturnedFactor = report.numberReturnedFactor;
                        entity.ReturnedWeight = report.returnedWeight;
                        entity.ReturnedEntry = report.returnedEntry;
                        entity.ReturnedExit = report.returnedExit;

                        entity.OtherCash = report.otherCash;
                        entity.CashEntry = report.cashEntry;
                        entity.CashExit = report.cashExit;

                        entity.OtherCurrency = report.otherCurrency;
                        entity.OtherCurrencyValue = report.otherCurrencyValue;
                        entity.OtherCurrencyRialValue = report.otherCurrencyRialValue;
                        entity.OtherCurrencyRialEntry = report.otherCurrencyRialEntry;
                        entity.OtherCurrencyRialExit = report.otherCurrencyRialExit;

                        entity.InventoryCash = report.inventoryCash;

                        entity.GoldDeficitWeight = report.goldDeficitWeight;
                        entity.GoldDeficitEntry = report.goldDeficitEntry;
                        entity.GoldDeficitExit = report.goldDeficitExit;

                        entity.GiftNumberEntry = report.giftNumberEntry;
                        entity.GiftNumberExit = report.giftNumberExit;
                        entity.GiftEntry = report.giftEntry;
                        entity.GiftExit = report.giftExit;

                        entity.LoyalityEntry = report.loyalityEntry;
                        entity.LoyalityExit = report.loyalityExit;

                        entity.CheckNumber = report.checkNumber;
                        entity.CheckEntry = report.checkEntry;
                        entity.CheckExit = report.checkExit;

                        entity.LeatherStoneDescriptionEntry = report.leatherStoneDescriptionEntry;
                        entity.LeatherStoneDescriptionExit = report.leatherStoneDescriptionExit;
                        entity.LeatherStoneEntry = report.leatherStoneEntry;
                        entity.LeatherStoneExit = report.leatherStoneExit;

                        entity.CoinNumber = report.coinNumber;
                        entity.CoinDescription = report.coinDescription;
                        entity.CoinEntry = report.coinEntry;
                        entity.CoinExit = report.coinExit;

                        entity.OtherKiaGoldWeight = report.otherKiaGoldWeight;
                        entity.OtherKiaGoldEntry = report.otherKiaGoldEntry;
                        entity.OtherKiaGoldExit = report.otherKiaGoldExit;

                        entity.OtherGoldWeight = report.otherGoldWeight;
                        entity.OtherGoldEntry = report.otherGoldEntry;
                        entity.OtherGoldExit = report.otherGoldExit;

                        entity.CreditorCustomerEntry = report.creditorCustomerEntry;
                        entity.CreditorCustomerExit = report.creditorCustomerExit;

                        entity.DebtorCustomerEntry = report.debtorCustomerEntry;
                        entity.DebtorCustomerExit = report.debtorCustomerExit;

                        entity.DepositBeforeCount = report.depositBeforeCount;
                        entity.DepositBeforeEntry = report.depositBeforeEntry;
                        entity.DepositBeforeExit = report.depositBeforeExit;

                        entity.DepositNewCount = report.depositNewCount;
                        entity.DepositNewEntry = report.depositNewEntry;
                        entity.DepositNewExit = report.depositNewExit;

                        entity.DiscountEntry = report.discountEntry;
                        entity.DiscountExit = report.discountExit;

                        entity.CostCourierPostEntry = report.costCourierPostEntry;
                        entity.CostCourierPostExit = report.costCourierPostExit;

                        entity.CostEntry = report.costEntry;
                        entity.CostExit = report.costExit;

                        report.dailyReportBankList.ForEach(x =>
                        {
                            var bank = entity.DailyReportBankList.SingleOrDefault(y => y.BankId == x.bankId);
                            if (bank != null)
                            {
                                bank.Entry = x.entry;
                                bank.Exit = x.exit;
                            }
                            else
                            {
                                bank = new DailyReportBank
                                {
                                    BankId = x.bankId,
                                    DailyReport = entity,
                                    Entry = x.entry,
                                    Exit = x.exit
                                };
                                entity.DailyReportBankList.Add(bank);
                            }
                        });

                        report.dailyReportCurrencyList.ForEach(x =>
                        {
                            var currency = entity.DailyReportCurrencyList.SingleOrDefault(y => y.CurrencyId == x.currencyId);
                            if (currency != null)
                            {
                                currency.Value = x.value;
                                currency.RialValue = x.rialValue;
                                currency.RialEntry = x.rialEntry;
                                currency.RialExit = x.rialExit;
                            }
                            else
                            {
                                currency = new DailyReportCurrency
                                {
                                    CurrencyId = x.currencyId,
                                    DailyReport = entity,
                                    Value = x.value,
                                    RialValue = x.rialValue,
                                    RialEntry = x.rialEntry,
                                    RialExit = x.rialExit
                                };
                                entity.DailyReportCurrencyList.Add(currency);
                            }
                        });

                        if (entity.Id == 0)
                        {
                            db.DailyReport.Add(entity);
                        }

                        entity.Sent = false;

                        DailyReportLog log = new DailyReportLog
                        {
                            DailyReport = entity,
                            UserId = currentUser.Id,
                            Date = DateTime.Now,
                            Status = CheckDailyReportStatus(model.report) ? Model.CalendarStatus.Submit : Model.CalendarStatus.Draft,
                            PrevData = json,
                            Ip = Request.UserHostAddress
                        };
                        db.DailyReportLog.Add(log);

                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "اطلاعات با موفقیت ذخیره شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "شما مجاز به ثبت گزارش روزانه در این تاریخ نیستید."
                        };
                    }
                }

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Core.GetExceptionResponse(ex), JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// خواندن اطلاعات یک تاریخ
        /// </summary>
        /// <param name="date">تاریخ</param>
        /// <returns>نتیجه خواندن اطلاعات تقویم</returns>
        [HttpGet]
        [Authorize(Roles = "admin, dailyReport, dailyReportPersonel")]
        public JsonResult Load(string date)
        {
            Response response;
            try
            {
                var dateTime = DateUtility.GetDateTime(date);
                var currentUser = GetAuthenticatedUser();
                if (User.IsInRole("dailyReportPersonel"))
                {
                    if (dateTime != DateTime.Today)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "شما مجاز به انجام این کار نیستید."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                using (var db = new KiaGalleryContext())
                {
                    var calendar = db.BranchCalendar.Where(x => x.BranchId == currentUser.BranchId && x.ReportDate == dateTime).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                    if (calendar != null)
                    {
                        var entity = db.DailyReport.Where(x => x.BranchId == currentUser.BranchId && x.BranchCalendarId == calendar.Id).SingleOrDefault();
                        if (entity != null)
                        {
                            DailyReportViewModel model = new DailyReportViewModel()
                            {
                                numberSaleFactor = entity.NumberSaleFactor,
                                saleWeight = entity.SaleWeight,
                                saleWeightPercent = entity.SaleWeightPercent,
                                saleEntry = entity.SaleEntry,
                                saleExit = entity.SaleExit,

                                numberReturnedFactor = entity.NumberReturnedFactor,
                                returnedWeight = entity.ReturnedWeight,
                                returnWeightPercent = entity.ReturnWeightPercent,
                                returnedEntry = entity.ReturnedEntry,
                                returnedExit = entity.ReturnedExit,


                                otherCash = entity.OtherCash,
                                cashEntry = entity.CashEntry,
                                cashExit = entity.CashExit,

                                otherCurrency = entity.OtherCurrency,
                                otherCurrencyValue = entity.OtherCurrencyValue,
                                otherCurrencyRialValue = entity.OtherCurrencyRialValue,
                                otherCurrencyRialEntry = entity.OtherCurrencyRialEntry,
                                otherCurrencyRialExit = entity.OtherCurrencyRialExit,

                                inventoryCash = entity.InventoryCash,

                                goldDeficitWeight = entity.GoldDeficitWeight,
                                goldDeficitEntry = entity.GoldDeficitEntry,
                                goldDeficitExit = entity.GoldDeficitExit,

                                giftNumberEntry = entity.GiftNumberEntry,
                                giftNumberExit = entity.GiftNumberExit,
                                giftEntry = entity.GiftEntry,
                                giftExit = entity.GiftExit,

                                loyalityEntry = entity.LoyalityEntry,
                                loyalityExit = entity.LoyalityExit,

                                checkNumber = entity.CheckNumber,
                                checkEntry = entity.CheckEntry,
                                checkExit = entity.CheckExit,

                                leatherStoneDescriptionEntry = entity.LeatherStoneDescriptionEntry,
                                leatherStoneDescriptionExit = entity.LeatherStoneDescriptionExit,
                                leatherStoneEntry = entity.LeatherStoneEntry,
                                leatherStoneExit = entity.LeatherStoneExit,

                                coinNumber = entity.CoinNumber,
                                coinDescription = entity.CoinDescription,
                                coinEntry = entity.CoinEntry,
                                coinExit = entity.CoinExit,

                                otherKiaGoldWeight = entity.OtherKiaGoldWeight,
                                otherKiaGoldEntry = entity.OtherKiaGoldEntry,
                                otherKiaGoldExit = entity.OtherKiaGoldExit,

                                otherGoldWeight = entity.OtherGoldWeight,
                                otherGoldEntry = entity.OtherGoldEntry,
                                otherGoldExit = entity.OtherGoldExit,

                                creditorCustomerEntry = entity.CreditorCustomerEntry,
                                creditorCustomerExit = entity.CreditorCustomerExit,

                                debtorCustomerEntry = entity.DebtorCustomerEntry,
                                debtorCustomerExit = entity.DebtorCustomerExit,

                                depositBeforeCount = entity.DepositBeforeCount,
                                depositBeforeEntry = entity.DepositBeforeEntry,
                                depositBeforeExit = entity.DepositBeforeExit,

                                depositNewCount = entity.DepositNewCount,
                                depositNewEntry = entity.DepositNewEntry,
                                depositNewExit = entity.DepositNewExit,

                                discountEntry = entity.DiscountEntry,
                                discountExit = entity.DiscountExit,

                                costCourierPostEntry = entity.CostCourierPostEntry,
                                costCourierPostExit = entity.CostCourierPostExit,

                                costEntry = entity.CostEntry,
                                costExit = entity.CostExit,

                                dailyReportBankList = entity.DailyReportBankList.Select(x => new DailyReportBankViewModel()
                                {
                                    bankId = x.BankId,
                                    entry = x.Entry,
                                    exit = x.Exit
                                }).ToList(),

                                dailyReportCurrencyList = entity.DailyReportCurrencyList.Select(x => new DailyReportCurrencyViewModel()
                                {
                                    currencyId = x.CurrencyId,
                                    value = x.Value,
                                    rialValue = x.RialValue,
                                    rialEntry = x.RialEntry,
                                    rialExit = x.RialExit
                                }).ToList()
                            };

                            response = new Response
                            {
                                status = 200,
                                data = model
                            };
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 201,
                                message = "گزارش روزانه در این تاریخ ثبت نشده."
                            };
                        }
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "مجاز به ثبت گزارش روزانه در این تاریخ نمی باشید."
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// بررسی تکمیل بودن گزارش روزانه
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CheckDailyReportStatus(DailyReportViewModel model)
        {
            var totalSale = model.saleExit - model.saleEntry;
            var totalReturned = totalSale + (model.returnedExit - model.returnedEntry);
            var totalBank = totalReturned + (model.dailyReportBankList.Sum(x => x.exit) - model.dailyReportBankList.Sum(x => x.entry));
            var totalCash = totalBank + (model.cashExit - model.cashEntry);
            var totalCurrency = totalCash + (model.dailyReportCurrencyList.Sum(x => x.rialExit) - model.dailyReportCurrencyList.Sum(x => x.rialEntry));
            var totalOtherCurrency = totalCurrency + (model.otherCurrencyRialExit - model.otherCurrencyRialEntry);
            var totalGoldDeficit = totalOtherCurrency + (model.goldDeficitExit - model.goldDeficitEntry);
            var totalGift = totalGoldDeficit + (model.giftExit - model.giftEntry);
            var totalCheck = totalGift + (model.checkExit - model.checkEntry);
            var totalLeatherStone = totalCheck + (model.leatherStoneExit - model.leatherStoneEntry);
            var totalCoin = totalLeatherStone + (model.coinExit - model.coinEntry);
            var totalOtherKiaGold = totalCoin + (model.otherKiaGoldExit - model.otherKiaGoldEntry);
            var totalOtherGold = totalOtherKiaGold + (model.otherGoldExit - model.otherGoldEntry);
            var totalCreditorCustomer = totalOtherGold + (model.creditorCustomerExit - model.creditorCustomerEntry);
            var totalDebtorCustomer = totalCreditorCustomer + (model.debtorCustomerExit - model.debtorCustomerEntry);
            var totalDepositBefore = totalDebtorCustomer + (model.depositBeforeExit - model.depositBeforeEntry);
            var totalDepositNew = totalDepositBefore + (model.depositNewExit - model.depositNewEntry);
            var totalLoyality = totalDepositNew + (model.loyalityExit - model.loyalityEntry);
            var totalDiscount = totalLoyality + (model.discountExit - model.discountEntry);
            var totalCost = totalDiscount + (model.costExit - model.costEntry);

            return totalCost == 0;
        }
        #endregion

        /// <summary>
        /// مشاهده جزئیات لاگ گزارش روزانه
        /// </summary>
        /// <param name="id">ردیف لاگ گزارش روزانه</param>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public ActionResult Log(int id)
        {
            ViewBag.Id = id;
            var currentUser = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                var log = db.DailyReportLog.Single(x => x.Id == id);
                ViewBag.Branch = db.Branch.Single(x => x.Id == log.DailyReport.BranchId);
                ViewBag.BankList = db.Bank.Where(x => x.BranchId == currentUser.BranchId && x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.CurrencyList = db.Currency.Where(x => x.Active == true).ToList();
                ViewBag.LogDate = DateUtility.GetPersianDateTime(log.Date);
                ViewBag.LogUser = log.User.FirstName + " " + log.User.LastName;
            }

            return View();
        }

        public JsonResult LoadLog(int id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var report = db.DailyReportLog.Single(x => x.Id == id);
                    return Json(new Response() { status = 200, data = report.PrevData }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StoneBraceletReport()
        {
            return View();
        }
        [AllowAnonymous]
        public JsonResult GetStoneBraceletReport()
        {
            Response response;
            try
            {
                var date1 = DateUtility.GetDateTime("1398/07/01");
                var date2 = DateUtility.GetDateTime("1398/07/17");

                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.Product.WorkshopId == 13 && x.CreateDate > date1 && x.CreateDate < date2);
                    var list = query.GroupBy(x => x.Order.Branch).Select(x => new StoneBraceletReportViewModel
                    {
                        branchName = x.Key.Name,
                        weight = x.Sum(z => z.Product.Weight),
                        count = x.Sum(z => z.Count),

                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list
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