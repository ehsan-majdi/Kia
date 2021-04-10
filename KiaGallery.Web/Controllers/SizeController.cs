using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class SizeController : BaseController
    {
        /// <summary>
        /// مدیریت سایز
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, size")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ویرایش سایز
        /// </summary>
        /// <param name="id">ردیف سایز</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, size")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش سایز";
            return View();
        }

        /// <summary>
        /// سایز جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, size")]
        public ActionResult Add()
        {
            ViewBag.Title = "سایز جدید";
            return View("Edit");
        }

        /// <summary>
        /// صفحه مقادیر سایز
        /// </summary>
        /// <param name="id">ردیف سایز</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, size")]
        public ActionResult Value(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Title = db.Size.Find(id).Title;
            }
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// ذخیره سایز
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات سایز</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت</returns>
        [HttpPost]
        [Authorize(Roles = "admin, size")]
        public JsonResult Save(SizeViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {

                    if (string.IsNullOrEmpty(model.title))
                    {
                        status = 500;
                        message = "وارد کردن نام سایز اجباری است.";
                    }
                    else
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.Size.Single(x => x.Id == model.id);
                            entity.Title = model.title.Trim();
                            entity.DefaultValue = model.defaultValue.Trim();
                            entity.ModifyUserId = GetAuthenticatedUserId();
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            message = "سایز با موفقیت ویرایش شد.";
                        }
                        else
                        {
                            var entity = new Size()
                            {
                                Title = model.title.Trim(),
                                DefaultValue = model.defaultValue.Trim(),
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };

                            db.Size.Add(entity);
                            message = "سایز با موفقیت ایجاد شد.";
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
        /// خواندن اطلاعات سایز
        /// </summary>
        /// <param name="id">ردیف سایز</param>
        /// <returns>جیسون اطلاعات لود شده سایز</returns>
        [HttpGet]
        [Authorize(Roles = "admin, size")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Size item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Size.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new SizeViewModel
                        {
                            id = item.Id,
                            title = item.Title,
                            defaultValue = item.DefaultValue
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "سایز مورد نظر یافت نشد."
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
        /// جستجوی سایز
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست سایز های پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, size")]
        public JsonResult Search(SizeSearchViewModel model)
        {
            Response response;
            try
            {
                List<Size> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Size.Select(x => x);

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
                            title = item.Title,
                            defaultValue = item.DefaultValue
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
        /// حذف سایز
        /// </summary>
        /// <param name="id">ردیف سایز</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, size")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Size.Find(Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "سایز با موفقیت حذف شد."
                    };
                    db.Size.Remove(item);
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
        /// دریافت لیست مقادیر ثبت شده برای یک سایز
        /// </summary>
        /// <param name="id">ردیف سایز</param>
        /// <returns>مقادیر سایز</returns>
        [HttpGet]
        [Authorize(Roles = "admin, size")]
        public JsonResult GetSizeValueList(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Size.Find(id);

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = item.SizeValueList.OrderBy(x => x.Order).Select(x => new
                            {
                                id = x.Id,
                                order = x.Order,
                                value = x.Value
                            }).ToList()
                        }
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
        /// ذخیره مقدار جدید برای سایز
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات سایز</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت</returns>
        [HttpPost]
        [Authorize(Roles = "admin, size")]
        public JsonResult SaveSizeValue(SizeValueViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.SizeValue.Find(model.id);
                        entity.SizeId = model.sizeId;
                        entity.Order = model.order;
                        entity.Value = model.value;
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new SizeValue()
                        {
                            SizeId = model.sizeId,
                            Order = model.order,
                            Value = model.value,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.SizeValue.Add(item);
                    }

                    response = new Response()
                    {
                        status = 200,
                        message = "اطلاعات با موفقیت ثبت شد."
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
        /// خواندن اطلاعات از یک مقدار خاص
        /// </summary>
        /// <param name="id">ردیف مقدار سایز</param>
        /// <returns>اطلاعات مقدار سایز</returns>
        [HttpGet]
        [Authorize(Roles = "admin, size")]
        public JsonResult GetSizeValue(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.SizeValue.Find(id);

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            id = item.Id,
                            order = item.Order,
                            value = item.Value
                        }
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
        /// حذف مقدار سایز
        /// </summary>
        /// <param name="id">ردیف مقدار سایز</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, size")]
        public JsonResult DeleteSizeValue(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.SizeValue.Find(id);
                    db.SizeValue.Remove(item);
                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "مقدار با موفقیت حذف شد."
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
        /// دریافت همه سایز های فعال
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام سایز ها</returns>
        [HttpGet]
        public JsonResult GetAll()
        {
            Response response;
            try
            {
                List<Size> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Size.ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            title = item.Title,
                            defaultValue = item.DefaultValue
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