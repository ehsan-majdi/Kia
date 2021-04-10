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
    public class CompanyController : BaseController
    {
        /// <summary>
        /// مدیریت شرکت
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, company")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="Id">ردیف شرکت</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, company")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش شرکت";
            return View();
        }
        /// <summary>
        /// شرکت جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, company")]
        public ActionResult Add()
        {
            ViewBag.Title = "شرکت جدید";
            return View("Edit");
        }

        /// <summary>
        /// ذخیره شرکت
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات شرکت</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, company")]
        public JsonResult Save(CompanyViewModel model)
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
                        message = "وارد کردن نام شرکت اجباری است.";
                    }
                    else
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.Company.Single(x => x.Id == model.id);
                            entity.Order = model.order;
                            entity.Alias = model.alias.Trim();
                            entity.Name = model.name.Trim();
                            entity.EnglishName = model.englishName?.Trim();
                            entity.Address = model.address?.Trim();
                            entity.EnglishAddress = model.englishAddress?.Trim();
                            entity.Active = model.active;
                            entity.ModifyUserId = GetAuthenticatedUserId();
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            message = "شرکت با موفقیت ویرایش شد.";
                        }
                        else
                        {
                            var entity = new Company()
                            {
                                Order = model.order,
                                Alias = model.alias.Trim(),
                                Name = model.name.Trim(),
                                EnglishName = model.englishName?.Trim(),
                                Address = model.address?.Trim(),
                                EnglishAddress = model.englishAddress?.Trim(),
                                Active = model.active,
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };

                            db.Company.Add(entity);
                            message = "شرکت با موفقیت ایجاد شد.";
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
        /// خواندن اطلاعات شرکت
        /// </summary>
        /// <param name="id">ردیف شرکت</param>
        /// <returns>جیسون اطلاعات لود شده شرکت</returns>
        [HttpGet]
        [Authorize(Roles = "admin, company")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Company item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Company.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new CompanyViewModel
                        {
                            id = item.Id,
                            order = item.Order,
                            alias = item.Alias,
                            name = item.Name,
                            englishName = item.EnglishName,
                            address = item.Address,
                            englishAddress = item.EnglishAddress,
                            active = item.Active
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "شرکت مورد نظر یافت نشد."
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
        /// جستجوی شرکت
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست شرکت پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, company")]
        public JsonResult Search(CompanySearchViewModel model)
        {
            Response response;
            try
            {
                List<Company> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Company.Select(x => x);

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
        /// حذف شرکت
        /// </summary>
        /// <param name="id">ردیف شرکت</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, company")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Company.Find(Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "شرکت با موفقیت حذف شد."
                    };
                    db.Company.Remove(item);
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
        /// فعال کردن شرکت
        /// </summary>
        /// <param name="id">ردیف شرکت</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, company")]
        public JsonResult Inactive(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Company.Find(Id);
                    item.Active = false;
                    response = new Response()
                    {
                        status = 200,
                        message = "شرکت با موفقیت غیرفعال شد."
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
        /// غیر فعال کردن شرکت
        /// </summary>
        /// <param name="id">ردیف شرکت</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, company")]
        public JsonResult Active(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Company.Find(Id);
                    item.Active = true;
                    response = new Response()
                    {
                        status = 200,
                        message = "شرکت با موفقیت فعال شد."
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
        /// دریافت همه شرکت های فعال
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام شرکت ها</returns>
        [HttpGet]
        public JsonResult GetAll()
        {
            Response response;
            try
            {
                List<Company> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Company.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
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