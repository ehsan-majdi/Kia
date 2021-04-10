using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class BranchFactorController : BaseController
    {
        // GET: BranchFactor
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
        /// <summary>
        /// ذخیره کردن مقادیر تعداد فاکتور هر شعب
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(BranchFactorViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                if (model.number == 0)
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "لطفا در بخش تعداد فاکتور عدد مورد نظر را وارد کنید."
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.BranchFactor.Where(x => x.Id == model.id).SingleOrDefault();

                        entity.BranchId = model.branchId;
                        entity.Number = model.number;
                        entity.Date = DateUtility.GetDateTime(model.date);
                        entity.ModifyUserId = currentUser.Id;
                        entity.CreateDate = DateTime.Now;
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new BranchFactor()
                        {
                            BranchId = model.branchId,
                            Number = model.number,
                            Date = DateUtility.GetDateTime(model.date),
                            CreateUserId = currentUser.Id,
                            ModifyUserId = currentUser.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.BranchFactor.Add(item);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "اطلاعات با موفقیت به روز رسانی شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// بارگذاری اطلاعات از قبیل :تعداد فاکتور صادر شده برای هر شعبه
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public JsonResult Search(string date, int? branchId)
        {
            var dateTime = DateUtility.GetDateTime(date);
            Response response;
            try
            {
                var currentuser = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    var query = db.BranchFactor.Select(x => x);

                    if (date != null)
                    {
                        query = query.Where(x => DbFunctions.TruncateTime(x.Date) == dateTime);

                    }
                    var list = query.GroupBy(x => x.Branch).Select(x => new BranchFactorSearchViewModel()
                    {
                        branchId = x.Key.Id,
                        branchName = x.Key.Name,
                        number = x.Sum(y => (y.Number)),
                        factorCount = db.CustomerFactor.Count(y => y.BranchId == x.Key.Id && y.Date == dateTime),
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

        public JsonResult Getdetail()
        {
            Response response;

            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CustomerFactor.Select(x => x);

                    var list = query.GroupBy(x => new { Date = DbFunctions.TruncateTime(x.Date) }).Select(x => new BranchGoldSearchViewModel()
                    {
                        date = x.Key.Date,
                        count = x.Count(),


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

        /// <summary>
        /// حذف فاکتور
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult Delete(int branchId, string date)
        {
            Response response;
            var gDate = DateUtility.GetDateTime(date);
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.BranchFactor.Where(x => x.BranchId == branchId && x.Date == gDate).ToList();
                    db.BranchFactor.RemoveRange(list);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "مورد انتخاب شده با موفقیت حذف شد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult getFactorCustomerLoyality(BranchFactorViewModel model)
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var query = db.CustomerFactor.Where(x => x.BranchId==model.branchId).Count();

        //            var list = query.Select(x => new BranchFactorViewModel()
        //            {


        //            }).ToList();
        //            response = new Response()
        //            {
        //                status = 200,

        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }
        //    return Json(response, JsonRequestBehavior.AllowGet);

        //}

    }
}