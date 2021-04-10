using KiaGallery.Common;
using KiaGallery.Model;
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
    public class GoldBalanceController : BaseController
    {
        // GET: GoldBalance
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
        [Route("goldbalance/trade/{date}")]
        [Authorize(Roles = "admin, boughtGold")]
        public ActionResult Trade(string date)
        {

            var changedDate = date.Replace("-", "/");
            ViewBag.Date = changedDate;

            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Workshop.Where(x => x.GoldTrade == true).Select(x => new WorkShopGoldViewModel
                {
                    id = x.Id,
                    name = x.Name,
                }).ToList();
            }


            return View();
        }
        public JsonResult Save(GoldBalanceViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = new GoldBalance()
                    {
                        TradeTime = model.tradeTime,
                        TradeType = model.tradeType,
                        DealerName = model.dealerName,
                        Weight = model.weight,
                        Description = model.description,
                        Date = DateUtility.GetDateTime(model.date, model.hour, model.minute, model.second),
                        CreateUserId = currentUser.Id,
                        ModifyUserId = currentUser.Id,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    };

                    db.GoldBalance.Add(entity);
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
                    var query = db.GoldBalance.OrderByDescending(x => x.Id).Where(x => DbFunctions.TruncateTime(x.Date) == dateTime);
                    var workShopGold = db.WorkShopGold.Where(x=> DbFunctions.TruncateTime(x.Date) == dateTime).Select(x => x.Weight).ToList().Sum();

                    var list = query.Select(x => new GoldBalanceSearchViewModel()
                    {
                        id = x.Id,
                        date = x.Date,
                        weight = x.Weight,
                        description = x.Description,
                        dealerName = x.DealerName,
                        tradeType = x.TradeType,
                        tradeTime = x.TradeTime,

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.tradeTypeTitle = Enums.GetTitle(x.tradeType);
                        x.stringDate = DateUtility.GetPersianDateTime(x.date);
                    });

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            weightSum = list.Where(x => x.tradeType == TradeType.Buy).Sum(x => x.weight) - list.Where(x => x.tradeType == TradeType.Sell).Sum(x => x.weight)
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

        public JsonResult GetBranchGold(string date)
        {
            Response response;
            try
            {
                var dateTime = DateUtility.GetDateTime(date);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.BranchGold.OrderByDescending(x => x.Branch.Order).Where(x => DbFunctions.TruncateTime(x.Date) == dateTime);

                    var list = query.GroupBy(x => x.BranchId).Select(x => new GetBranchGoldViewModel()
                    {
                        branchName = x.FirstOrDefault().Branch.Name,
                        weight = x.Sum(y => y.Weight),
                        price = x.Sum(y => y.Price),
                        date = x.OrderByDescending(y => y.Date).FirstOrDefault().Date,
                        detail = x.Select(y => new DetailSearchViewModel
                        {
                            weight = y.Weight,
                            date = y.Date,
                        }).ToList(),

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.detail.ForEach(y =>
                        {
                            y.stringDate = DateUtility.GetPersianDateTime(y.date);
                        });
                        x.stringDate = DateUtility.GetPersianDateTime(x.date);
                    });

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            weightSum = list.Sum(x => x.weight),
                            priceSum = list.Sum(x => x.price)

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
                    var query = db.GoldBalance.Select(x => x);


                    var list = query.GroupBy(x => new { Date = DbFunctions.TruncateTime(x.Date) }).Select(x => new BranchGoldSearchViewModel()
                    {
                        date = x.Key.Date,
                        count = x.Count(),
                        lastWeight = x.OrderByDescending(y => y.Id).FirstOrDefault().Weight,

                    }).ToList();

                    list.ForEach(x =>
                    {
                        x.stringDate = DateUtility.GetPersianDate(x.date);
                        x.lastWeight = Math.Floor(x.lastWeight);
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