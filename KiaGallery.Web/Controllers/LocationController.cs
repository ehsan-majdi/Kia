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
    /// کنترلر شهر
    /// </summary>
    public class LocationController : BaseController
    {
        /// <summary>
        /// مدیریت شهر
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ویرایش شهر
        /// </summary>
        /// <param name="id">ردیف کاربر</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش شهر";
            return View();
        }

        /// <summary>
        /// شهر جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public ActionResult Add()
        {
            ViewBag.Title = "شهر جدید";
            return View("Edit");
        }

        /// <summary>
        /// ذخیره شهر
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات شهر</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Save(LocationViewModel model)
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
                        message = "وارد کردن نام شهر اجباری است.";
                    }
                    else
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.Location.Single(x => x.Id == model.id);
                            entity.ParentId = model.parentId;
                            entity.LocationType = model.locationType;
                            entity.Name = model.name.Trim();
                            entity.EnglishName = model.englishName.Trim();
                            entity.Order = model.order;
                            entity.ModifyUserId = GetAuthenticatedUserId();
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            message = "شهر با موفقیت ویرایش شد.";
                        }
                        else
                        {
                            var entity = new Location()
                            {
                                ParentId = model.parentId,
                                LocationType = model.locationType,
                                Name = model.name.Trim(),
                                EnglishName = model.englishName.Trim(),
                                Order = model.order,
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };

                            db.Location.Add(entity);
                            message = "شهر با موفقیت ایجاد شد.";
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
        /// خواندن اطلاعات شهر
        /// </summary>
        /// <param name="id">ردیف شهر</param>
        /// <returns>جیسون اطلاعات لود شده شهر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Location item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Location.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new LocationViewModel
                        {
                            id = item.Id,
                            parentId = item.ParentId,
                            locationType = item.LocationType,
                            name = item.Name,
                            englishName = item.EnglishName,
                            order = item.Order
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "شهر مورد نظر یافت نشد."
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
        /// جستجوی شهر
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست شهر پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Search(LocationSearchViewModel model)
        {
            Response response;
            try
            {
                List<Location> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Location.Select(x => x);
                    if (!string.IsNullOrEmpty(model.term))
                    {
                        query = query.Where(x=> x.Name.Contains(model.term) || x.EnglishName.Contains(model.term) || x.Name.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.EnglishName.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")));
                    }
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
                            name = item.Name,
                            englishName = item.EnglishName,
                            order = item.Order
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
        /// حذف شهر
        /// </summary>
        /// <param name="id">ردیف شهر</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Location.First(x => x.Id == Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "شهر با موفقیت حذف شد."
                    };
                    db.Location.Remove(item);
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
        /// دریافت همه استان ها
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام استان ها</returns>
        [HttpGet]
        public JsonResult   GetAllProvince()
        {
            Response response;
            try
            {
                List<Location> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Location.Where(x => x.LocationType == Model.LocationType.Province).OrderBy(x => x.Order).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            name = item.Name,
                            englishName = item.EnglishName,
                            order = item.Order
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

        /// <summary>
        /// دریافت همه شهر ها
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام شهر ها</returns>
        [HttpGet]
        public JsonResult GetAllCity(int id)
        {
            Response response;
            try
            {
                List<Location> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Location.Where(x => x.LocationType == Model.LocationType.City && x.ParentId == id).OrderBy(x => x.Order).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            name = item.Name,
                            englishName = item.EnglishName,
                            order = item.Order
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