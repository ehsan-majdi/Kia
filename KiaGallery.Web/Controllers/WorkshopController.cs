using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر کارگاه
    /// </summary>
    public class WorkshopController : BaseController
    {
        /// <summary>
        /// مدیریت کارگاه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, workshop")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// مشاهده محصولات خود
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult Products()
        {

            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true && x.ProductList.Count(y => y.Active == true) > 0).OrderBy(x => x.Order).Select(x => x).ToList();
            }

            if (user.WorkshopId == null)
                return RedirectToAction("Login", "Account");
            else
            {
                ViewBag.WorkshopId = user.WorkshopId;
                return View();
            }
        }

        /// <summary>
        /// ویرایش کارگاه
        /// </summary>
        /// <param name="id">ردیف کارگاه</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, workshop")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش کارگاه";
            return View();
        }

        /// <summary>
        /// کارگاه جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, workshop")]
        public ActionResult Add()
        {
            ViewBag.Title = "کارگاه جدید";
            return View("Edit");
        }
        

        /// <summary>
        /// ذخیره کارگاه
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات کارگاه</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, workshop")]
        public JsonResult Save(WorkshopViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    if (string.IsNullOrEmpty(model.name))
                    {
                        status = 500;
                        message = "وارد کردن نام کارگاه اجباری است.";
                    }
                    else
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.Workshop.Single(x => x.Id == model.id);
                            entity.Order = model.order;
                            entity.Alias = model.alias.Trim();
                            entity.Name = model.name.Trim();
                            entity.Color = model.color.Trim();
                            entity.AutoConfirm = model.autoConfirm;
                            entity.Active = model.active;
                            entity.GoldTrade = model.goldTrade;
                            entity.ModifyUserId = GetAuthenticatedUserId();
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            message = "کارگاه با موفقیت ویرایش شد.";
                        }
                        else
                        {
                            var entity = new Workshop()
                            {
                                Order = model.order,
                                Alias = model.alias.Trim(),
                                Name = model.name.Trim(),
                                Color = model.color.Trim(),
                                AutoConfirm = model.autoConfirm,
                                Active = model.active,
                                GoldTrade = model.goldTrade,
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };

                            db.Workshop.Add(entity);
                            message = "کارگاه با موفقیت ایجاد شد.";
                        }
                        db.SaveChanges();
                    }
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
        /// خواندن اطلاعات کارگاه
        /// </summary>
        /// <param name="id">ردیف جرم</param>
        /// <returns>جیسون اطلاعات لود شده کارگاه</returns>
        [HttpGet]
        [Authorize(Roles = "admin, workshop")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Workshop item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Workshop.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new WorkshopViewModel
                        {
                            id = item.Id,
                            order = item.Order,
                            alias = item.Alias,
                            name = item.Name,
                            color = item.Color,
                            autoConfirm = item.AutoConfirm,
                            active = item.Active,
                            goldTrade = item.GoldTrade
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "کارگاه مورد نظر یافت نشد."
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
        /// جستجوی کارگاه
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست کارگاه های پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, workshop")]
        public JsonResult Search(WorkshopSearchViewModel model)
        {
            Response response;
            try
            {
                List<Workshop> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Workshop.Select(x => x);

                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

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
                            alias = item.Alias,
                            name = item.Name,
                            order = item.Order,
                            color = item.Color,
                            active = item.Active
                        }),
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

        /// <summary>
        /// حذف کارگاه
        /// </summary>
        /// <param name="id">ردیف کارگاه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, workshop")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Workshop.Find(Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "کارگاه با موفقیت حذف شد."
                    };
                    db.Workshop.Remove(item);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// غیر فعال کردن کارگاه
        /// </summary>
        /// <param name="id">ردیف کارگاه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, workshop")]
        public JsonResult Inactive(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Workshop.Find(Id);
                    item.Active = false;
                    response = new Response()
                    {
                        status = 200,
                        message = "کارگاه با موفقیت غیرفعال شد."
                    };
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// فعال کردن کارگاه
        /// </summary>
        /// <param name="id">ردیف کارگاه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, workshop")]
        public JsonResult Active(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Workshop.Find(Id);
                    item.Active = true;
                    response = new Response()
                    {
                        status = 200,
                        message = "کارگاه با موفقیت فعال شد."
                    };
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// دریافت همه کارگاه های فعال
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام کارگاه ها</returns>
        [HttpGet]
        public JsonResult GetAll()
        {
            Response response;
            try
            {
                List<Workshop> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Workshop.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            alias = item.Alias,
                            name = item.Name
                        })
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