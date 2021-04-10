using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// ثبت غذا توسط کاربران لاگین شده
    /// </summary>
    public class FoodRegistrationController : BaseController
    {
        /// <summary>
        /// صفحه اصلی
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , foodRegistration")]
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
        /// خواندن مقادیر 
        /// </summary>
        /// <param name="id">ردیف </param>
        /// <returns>جیسون اطلاعات لود شده سوالات</returns>
        [HttpGet]
        [Authorize(Roles = "admin,foodRegistration")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.FoodRegistration.Where(x => x.Id == id).SingleOrDefault();
                    var data = new FoodCensusViewModel()
                    {
                        id = item.Id,
                        foodCensusId = item.FoodCensusId,
                        appertizer = item.Appertizer,
                        typeFood=item.FoodWithoutRice,
                        food = item.Food,
                        date = DateUtility.GetPersianDate(item.Date)
                    };
                    response = new Response()
                    {
                        status = 200,
                        data = data,
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
        /// انتخاب غذا و پیش غذا
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin , foodRegistration")]
        public JsonResult FoodSelect(FoodRegistrationViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (model.id > 0 && model.id != null)
                    {
                        var entity = db.FoodRegistration.Where(x => x.Id == model.id).SingleOrDefault();
                        entity.FoodCensusId = model.foodCensusId;
                        entity.FoodStatus = model.foodStatus;
                        entity.Appertizer = model.appertizer;
                        entity.Food = model.food;
                        entity.FoodWithoutRice = model.foodWithoutRice;
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new FoodRegistration()
                        {
                            FoodCensusId = model.foodCensusId,
                            UserId = GetAuthenticatedUserId(),
                            FoodStatus = model.foodStatus,
                            Appertizer = model.appertizer,
                            Food = model.food,
                            FoodWithoutRice=model.foodWithoutRice,
                            Date = DateUtility.GetDateTime(model.date),
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.FoodRegistration.Add(item);
                    }

                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "ثبت اطلاعات با موفقیت انجام شد.",
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
