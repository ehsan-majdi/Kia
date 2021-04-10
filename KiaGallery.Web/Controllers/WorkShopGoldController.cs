using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class WorkShopGoldController : BaseController
    {
        // GET: WorkShopGold
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetWorkShop(string date)
        {
            var dateTime = DateUtility.GetDateTime(date);

            Response response;
            using (var db = new KiaGalleryContext())
            {
                var query = db.WorkShopGold.OrderByDescending(x => x.Id).Where(x => DbFunctions.TruncateTime(x.Date) == dateTime);
                var workShopList = query.GroupBy(x=>x.WorkshopId).Select(x=> new 
                {
                    id = x.Select(y=>y.WorkshopId).FirstOrDefault(),
                    name = x.FirstOrDefault().Workshop.Name,
                    sum = x.Sum(y=>y.Weight)
                }).ToList();

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = workShopList,
                        goldSum = workShopList.Sum(x => x.sum)
                    }
                };
            }
            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult Save(WorkShopGoldViewModel model)
        {
            var currentUser = GetAuthenticatedUser();
            var dateTime = DateUtility.GetDateTime(model.date);
            Response response;
            using (var db = new KiaGalleryContext())
            {
                var boughtGold = db.GoldBalance.Where(x => x.TradeType == TradeType.Buy && DbFunctions.TruncateTime(x.Date) == dateTime ).Select(x => x.Weight).ToList().Sum();
                var soldGold = db.GoldBalance.Where(x => x.TradeType == TradeType.Sell && DbFunctions.TruncateTime(x.Date) == dateTime).Select(x => x.Weight).ToList().Sum();
                var workshopGold = db.WorkShopGold.Where(x=> DbFunctions.TruncateTime(x.Date) == dateTime).Select(x => x.Weight).ToList().Sum();
                var sum = boughtGold - soldGold - workshopGold;
                if (model.weight <= sum)
                {
                    var entity = new WorkShopGold()
                    {
                        RemittanceType = model.remittanceType,
                        WorkshopId = model.workshopId,
                        BoughtGoldPrice = model.workShopBoughtGoldPrice,
                        GoldRate = model.workShopGoldRate,
                        Weight = model.weight,
                        CreateUserId = currentUser.Id,
                        ModifyUserId = currentUser.Id,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Date = dateTime

                    };
                    db.WorkShopGold.Add(entity);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "حواله طلا با موفقیت انجام شد."
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "مقدار طلای وارد شده بیشتر از موجودی طلا میباشد."
                    };
                }
               
            }
            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult Edit(WorkShopGoldEditViewModel model)
        {
            Response response;
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    var boughtGold = db.GoldBalance.Where(x => x.TradeType == TradeType.Buy).Select(x => x.Weight).ToList().Sum();
                    var soldGold = db.GoldBalance.Where(x => x.TradeType == TradeType.Sell).Select(x => x.Weight).ToList().Sum();
                    var workshopGold = db.WorkShopGold.Select(x => x.Weight).ToList().Sum();
                    var sum = boughtGold - soldGold - workshopGold;
                    var entity = db.WorkShopGold.OrderByDescending(x=>x.Id).Where(x => x.WorkshopId == model.workShopId).FirstOrDefault();
                    if (model.weight < sum)
                    {
                        entity.Weight = model.weight;
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "Job Done!"

                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "مقدار طلای وارد شده بیشتر از موجودی طلا میباشد."
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
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                  var item = db.WorkShopGold.OrderByDescending(x=>x.Id).FirstOrDefault(x => x.WorkshopId == id);
                    response = new Response()
                    {
                        status = 200,
                        data = new WorkShopGoldEditViewModel()
                        {
                           weight = item?.Weight,
                            
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
        public JsonResult GetDetail(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var detailList = db.WorkShopGold.Where(x => x.WorkshopId == id).Select(x => new WorkShopDetailViewModel {

                        weight = x.Weight,
                        date = x.Date,
                        name = x.Workshop.Name,
                        goldRate = x.GoldRate,
                        boughtGoldPrice = x.BoughtGoldPrice
                    }).ToList();
                    detailList.ForEach(x =>
                    {
                        x.stringDate = DateUtility.GetPersianDateTime(x.date);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list=detailList
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