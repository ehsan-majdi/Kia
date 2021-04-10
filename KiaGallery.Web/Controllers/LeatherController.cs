using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر چرم
    /// </summary>
    public class LeatherController : BaseController
    {
        /// <summary>
        /// مدیریت چرم
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, leather")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ویرایش چرم
        /// </summary>
        /// <param name="id">ردیف چرم</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, leather")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            ViewBag.Title = "ویرایش چرم";
            return View();
        }

        /// <summary>
        /// چرم جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, leather")]
        public ActionResult Add()
        {
            ViewBag.Title = "چرم جدید";
            return View("Edit");
        }

        /// <summary>
        /// کاتالوگ چرم
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        //[OutputCache(Duration = 3600)] // کش شدن اطلاعات به مدت 1 ساعت
        public ActionResult Catalog()
        {
            using (var db = new KiaGalleryContext())
            {
                var leathers = db.Leather.ToList();
                ViewBag.Piped = leathers.Where(x => x.LeatherType == LeatherType.Piped && x.Active == true).OrderByDescending(x => x.Order).ToList();
                ViewBag.Sewing = leathers.Where(x => x.LeatherType == LeatherType.Sewing && x.Active == true).OrderByDescending(x => x.Order).ToList();
                ViewBag.Texture = leathers.Where(x => x.LeatherType == LeatherType.Texture && x.Active == true).OrderByDescending(x => x.Order).ToList();
                ViewBag.Rail = leathers.Where(x => x.LeatherType == LeatherType.Rail && x.Active == true).OrderByDescending(x => x.Order).ToList();
                ViewBag.Masculine = leathers.Where(x => x.LeatherType == LeatherType.Masculine && x.Active == true).OrderByDescending(x => x.Order).ToList();
                ViewBag.Rope = leathers.Where(x => x.LeatherType == LeatherType.Rope && x.Active == true).OrderByDescending(x => x.Order).ToList();
            }
            return View();
        }

        /// <summary>
        /// ذخیره چرم
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات چرم</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, leather")]
        public JsonResult Save(LeatherViewModel model)
        {
            Response response;
            try
            {
                string oldFileName = "";

                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {

                    if (string.IsNullOrEmpty(model.name))
                    {
                        status = 500;
                        message = "وارد کردن نام چرم اجباری است.";
                    }
                    else
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.Leather.Single(x => x.Id == model.id);
                            entity.Name = model.name.Trim();
                            entity.LeatherType = model.leatherType;
                            entity.Order = model.order;
                            entity.FileName = model.fileName?.Trim();
                            entity.Active = model.active;
                            entity.ModifyUserId = GetAuthenticatedUserId();
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            if (!string.IsNullOrEmpty(entity.FileName) && entity.FileName != model.fileName)
                                oldFileName = entity.FileName;

                            message = "چرم با موفقیت ویرایش شد.";
                        }
                        else
                        {
                            var entity = new Leather()
                            {
                                Name = model.name.Trim(),
                                LeatherType = model.leatherType,
                                Order = model.order,
                                FileName = model.fileName?.Trim(),
                                Active = model.active,
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };

                            db.Leather.Add(entity);
                            message = "چرم با موفقیت ایجاد شد.";
                        }
                        db.SaveChanges();

                        if (!string.IsNullOrEmpty(oldFileName) && System.IO.File.Exists(Server.MapPath("~/Upload/Leather/" + oldFileName)))
                            System.IO.File.Delete(Server.MapPath("~/Upload/Leather/" + oldFileName));
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
        /// خواندن اطلاعات چرم
        /// </summary>
        /// <param name="id">ردیف جرم</param>
        /// <returns>جیسون اطلاعات لود شده چرم</returns>
        [HttpGet]
        [Authorize(Roles = "admin, leather")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Leather item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Leather.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new LeatherViewModel
                        {
                            id = item.Id,
                            name = item.Name,
                            leatherType = item.LeatherType,
                            order = item.Order,
                            fileName = item.FileName,
                            active = item.Active
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "چرم مورد نظر یافت نشد."
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
        /// جستجوی چرم
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست چرم های پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, leather")]
        public JsonResult Search(LeatherSearchViewModel model)
        {
            Response response;
            try
            {
                List<Leather> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Leather.Select(x => x);

                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.Name.Contains(model.term.Trim()) || x.Name.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")));
                    }
                    if (model.active != null)
                    {
                        query = query.Where(x => x.Active == model.active);
                    }
                    if(model.leatherType != null)
                    {
                        query = query.Where(x => x.LeatherType == model.leatherType);
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
                            leatherType = item.LeatherType,
                            leatherTypeTitle = Enums.GetTitle(item.LeatherType),
                            order = item.Order,
                            fileName = item.FileName,
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
        /// حذف چرم
        /// </summary>
        /// <param name="id">ردیف چرم</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, leather")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Leather.First(x => x.Id == id);

                    if (item.ProductLeatherList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این چرم به محصول متصل می باشد."
                        };
                    }
                    else if (item.CartProductLeatherList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این چرم در سبد خرید بک شعبه ثبت شده می باشد."
                        };
                    }
                    else if (item.OrderDetailLeatherList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این چرم در یک سفارش ثبت شده می باشد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "چرم با موفقیت حذف شد."
                        };
                        db.Leather.Remove(item);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// غیر فعال کردن چرم
        /// </summary>
        /// <param name="id">ردیف چرم</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, leather")]
        public JsonResult Inactive(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Leather.First(x => x.Id == id);
                    item.Active = false;
                    response = new Response()
                    {
                        status = 200,
                        message = "چرم با موفقیت غیرفعال شد."
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
        /// فعال کردن چرم
        /// </summary>
        /// <param name="id">ردیف چرم</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, leather")]
        public JsonResult Active(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Leather.First(x => x.Id == id);
                    item.Active = true;
                    response = new Response()
                    {
                        status = 200,
                        message = "چرم با موفقیت فعال شد."
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
        /// دریافت همه چرم های فعال
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام چرم ها</returns>
        [HttpGet]
        public JsonResult GetAll()
        {
            Response response;
            try
            {
                List<Leather> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Leather.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
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
                            leatherType = item.LeatherType,
                            order = item.Order,
                            fileName = item.FileName
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