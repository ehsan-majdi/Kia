using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class BranchGoldController : BaseController
    {
        // GET: GoldTrade
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

            return View();
        }
       
        public JsonResult Save(BranchGoldViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = new BranchGold()
                    {
                        Weight = model.weight,
                        Price = model.price,
                        Date = DateUtility.GetDateTime(model.date, model.hour, model.minute, model.second),
                        BranchId = currentUser.BranchId,
                        CreateUserId = currentUser.Id,
                        ModifyUserId = currentUser.Id,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    };
                    db.BranchGold.Add(entity);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "اطلاعات با موفقیت به روز رسانی شد."

                    };
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Search(string date)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                var dateTime = DateUtility.GetDateTime(date);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.BranchGold.OrderByDescending(x => x.Id).Where(x => DbFunctions.TruncateTime(x.Date) == dateTime && x.BranchId == currentUser.BranchId);

                    var list = query.Select(x => new BranchGoldSearchViewModel()
                    {
                        id = x.Id,
                        date = x.Date,
                        weight = x.Weight,
                        price = x.Price

                    }).ToList();

                    list.ForEach(x =>
                    {
                        x.stringDate = DateUtility.GetPersianDateTime(x.date);
                        x.stringPrice = Core.ToSeparator(x.price);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            weightSum = list.Sum(x => x.weight),
                            priceSum  = list.Sum(x=> x.price)


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
        public JsonResult GetCount()
        {
            Response response;

            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.BranchGold.Select(x => x);
                    if (!User.IsInRole("admin") || User.IsInRole("soldGoldManage") || User.IsInRole("soldGold"))
                    {
                        query = query.Where(x => x.BranchId == currentUser.BranchId);
                    }

                    var list = query.GroupBy(x => new { Date = DbFunctions.TruncateTime(x.Date) }).Select(x => new BranchGoldSearchViewModel()
                    {
                        date = x.Key.Date,
                        count = x.Count(),
                        lastWeight = x.OrderByDescending(y => y.Id).FirstOrDefault().Weight,

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.stringDate = DateUtility.GetPersianDate(x.date);
                    });
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