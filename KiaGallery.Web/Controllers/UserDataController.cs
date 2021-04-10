using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.BranchesPayments;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر کاربران ربات پرداخت شعب
    /// </summary>
    public class UserDataController : BaseController
    {
        /// <summary>
        /// کاربران ربات پرداخت شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, userData")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="Id">ردیف کاربر</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, userData")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش کاربر ربات فاکتور پرداختی";
            return View();
        }

        /// <summary>
        /// ذخیره کاربر
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات کاربر</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, userData")]
        public JsonResult Save(UserDataViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                int userid = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.UserData.Single(x => x.Id == model.id);
                    entity.BranchId = model.branchId;
                    entity.ModifyUserId = userid;
                    message = "کاربر ربات فاکتور پرداختی با موفقیت ویرایش شد.";
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
        /// خواندن اطلاعات کاربر
        /// </summary>
        /// <param name="id">ردیف کاربر</param>
        /// <returns>جیسون اطلاعات لود شده کاربر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, userData")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                UserDataViewModel item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.UserData.Where(x => x.Id == id).Select(x => new UserDataViewModel()
                    {
                        id = x.Id,
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        stoped = x.Stoped,
                        branchId = x.BranchId
                    }).FirstOrDefault();
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = item

                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "کاربر مورد نظر یافت نشد."
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
        /// جستجوی کاربر
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست شعب پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, userData")]
        public JsonResult Search(BranchesPaymentsSearchViewModel model)
        {
            Response response;
            try
            {
                List<UserDataViewModel> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.UserData.Select(x => x);

                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    list = query.Select(x => new UserDataViewModel()
                    {
                        id =x.Id,
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        stoped = x.Stoped,
                        branchId = x.BranchId,
                        branchName = x.Branch.Name
                    }).ToList();
                }
                response = new Response()
                {
                    status = 200,

                    data = new
                    {
                        list = list,
                        pageCount = Math.Ceiling((double)dataCount / model.count),
                        count = dataCount,
                        page = model.page + 1
                    }
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