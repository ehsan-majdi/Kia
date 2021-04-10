using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Salary;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    ///  شرح وظایف پرسنل
    /// </summary>
    public class PersonJobDescriptionTemplateController : BaseController
    {
        /// <summary>
        /// صفحه لیست وظایف
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ویرایش شرح وظایف
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// ایجاد شرح وظایف
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Add()
        {
            return View("Edit");
        }

        /// <summary>
        /// ذخیره شرح وظایف 
        /// </summary>
        /// <param name="model"> مدل حاوی اطلاعات  شرح وظایف</param>
        /// <returns>نتیجه ذخیره</returns>
        [HttpPost]
        public JsonResult Save(PersonJobDescTmplViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.PersonJobDescriptionTemplate.Single(x => x.Id == model.id);

                        entity.Title = model.title;
                        entity.Status = model.status;
                        entity.Description = model.text;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "شرح وظایف با موفقیت ویرایش شد."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var entity = new PersonJobDescriptionTemplate()
                        {
                            Title = model.title,
                            Status = model.status,
                            Description = model.text,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId()
                        };
                        db.PersonJobDescriptionTemplate.Add(entity);
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "شرح وظایف با موفقیت ایجاد شد."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
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
        /// خواندن اطلاعات  شرح وظایف
        /// </summary>
        /// <param name="id">ردیف  شرح وظایف</param>
        /// <returns>اطلاعات  شرح وظایف</returns>
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.PersonJobDescriptionTemplate.Where(x => x.Id == id).Select(x => new PersonJobDescTmplViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        text = x.Description,
                        status = x.Status,
                    }).Single();
                    response = new Response()
                    {
                        status = 200,
                        data = entity
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
        /// حذف کردن  شرح وظایف
        /// </summary>
        /// <param name="id">ریف  شرح وظایف</param>
        /// <returns>نتیجه حذف</returns>
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.PersonJobDescriptionTemplate.Find(id);
                    db.PersonJobDescriptionTemplate.Remove(entity);
                    db.SaveChanges();
                };
                response = new Response()
                {
                    status = 200,
                    message = "شرح وظایف با موفقیت حذف شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// گرفتن لیست  شرح وظایف
        /// </summary>
        /// <returns>لیست  شرح وظایف</returns>
        public JsonResult Search()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.PersonJobDescriptionTemplate.Select(x => x);
                    var data = query.Select(x => new PersonJobDescTmplViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        status = x.Status

                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data
                        }
                    };
                };

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// گرفتن لیست شرح وظایف پرسنل
        /// </summary>
        /// <returns>لیست شرح وظایف پرسنل</returns>
        public JsonResult GetList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.PersonJobDescriptionTemplate.Select(x => x);
                    var data = entity.Select(x => new
                    {
                        id = x.Id,
                        title = x.Title,
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data
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