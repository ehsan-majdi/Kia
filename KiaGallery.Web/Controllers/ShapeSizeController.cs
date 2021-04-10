using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر سایز اشکال
    /// </summary>
    public class ShapeSizeController : BaseController
    {
        /// <summary>
        /// مدیریت سایز اشکال
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, size")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// جزئیات یک شکل سنگ
        /// </summary>
        /// <param name="id">ردیف شکل شنگ</param>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, size")]
        public ActionResult Details(StoneShape id)
        {
            ViewBag.StoneShape = id;
            ViewBag.StoneShapeTitle = Enums.GetTitle(id);
            return View();
        }

        /// <summary>
        /// ذخیره سایز
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات سایز</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, size")]
        public JsonResult Save(ShapeSizeViewModel model)
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
                        var entity = db.ShapeSize.Single(x => x.Id == model.id);
                        entity.Order = model.order;
                        entity.StoneShape = model.stoneShape;
                        entity.SizeLength = model.sizeLength;
                        entity.SizeWidth = model.sizeWidth;
                        entity.Active = model.active;
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;

                        status = 200;
                        message = "سایز با موفقیت ویرایش شد.";

                    }
                    else
                    {
                        var entity = new ShapeSize()
                        {
                            Order = model.order,
                            StoneShape = model.stoneShape,
                            SizeLength = model.sizeLength,
                            SizeWidth = model.sizeWidth,
                            Active = model.active,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        db.ShapeSize.Add(entity);

                        status = 200;
                        message = "سایز با موفقیت ایجاد شد.";
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
        /// <param name="id">ردیف سایز</param>
        /// <returns>جیسون اطلاعات لود شده سایز</returns>
        [HttpGet]
        [Authorize(Roles = "admin, size")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                ShapeSize item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.ShapeSize.Find(id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new ShapeSizeViewModel()
                        {
                            id = item.Id,
                            order = item.Order,
                            stoneShape = item.StoneShape,
                            sizeLength = item.SizeLength,
                            sizeWidth = item.SizeWidth,
                            active = item.Active
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
        /// <returns>لیست سایز پیدا شده</returns>
        [Authorize(Roles = "admin, size")]
        public JsonResult Search(ShapeSizeSearchViewModel model)
        {
            Response response;
            try
            {
                List<ShapeSize> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.ShapeSize.Where(x => x.StoneShape == model.stoneShape);

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
                            stoneShape = item.StoneShape,
                            sizeLength = item.SizeLength,
                            sizeWidth = item.SizeWidth,
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
        /// غیر فعال کردن سایز
        /// </summary>
        /// <param name="id">ردیف سایز</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, size")]
        public JsonResult Inactive(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.ShapeSize.Find(Id);
                    item.Active = false;
                    response = new Response()
                    {
                        status = 200,
                        message = "سایز با موفقیت غیرفعال شد."
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
        /// فعال کردن سایز
        /// </summary>
        /// <param name="id">ردیف سایز</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, size")]
        public JsonResult Active(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.ShapeSize.Find(Id);
                    item.Active = true;
                    response = new Response()
                    {
                        status = 200,
                        message = "سایز با موفقیت فعال شد."
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
                    var item = db.ShapeSize.Find(Id);
                    db.ShapeSize.Remove(item);
                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "سایز با موفقیت حذف شد."
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
        /// دریافت سایز های یک شکل سایز
        /// </summary>
        /// <param name="shape">شکل مورد نظر</param>
        /// <returns>لیست سایزهای فعال یک شکل</returns>
        [HttpGet]
        public JsonResult GetAllSize(StoneShape shape, int? order, int? selected)
        {
            Response response;
            try
            {
                List<ShapeSize> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.ShapeSize.Where(x => x.StoneShape == shape && x.Active == true);

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
                            size = item.SizeLength + "x" + item.SizeWidth
                        }),
                        count = dataCount,
                        order = order,
                        selected = selected
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