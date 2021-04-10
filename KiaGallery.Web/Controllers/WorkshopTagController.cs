using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class WorkshopTagController : BaseController
    {
        /// <summary>
        /// صفحه لیست برچسب کارگاه محصولات
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// صفحه اضافه کردن برچسب کارگاه جدید 
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Add()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.WorkshopList = db.Workshop.ToList();
            };
            return View("Edit");
        }

        /// <summary>
        /// صفحه ویرایش کردن برچسب کارگاه جدید 
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Edit(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.WorkshopList = db.Workshop.ToList();
            };
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// ذخیره برچسب کارگاه در دیتابیس
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات برچسب کارگاه</param>
        /// <returns>نتیجه ذخیره</returns>
        public JsonResult Save(WorkshopTagViewModel model)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    if (model.id > 0)
                    {
                        var entity = db.WorkshopTag.SingleOrDefault(x => x.Id == model.id);
                        entity.Title = model.title;
                        entity.WorkshopId = model.workshopId;
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                    }
                    else
                    {
                        var entity = new WorkshopTag()
                        {
                            Title = model.title,
                            WorkshopId = model.workshopId,
                            CreateUserId = currentUser.Id,
                            ModifyUserId = currentUser.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        };
                        db.WorkshopTag.Add(entity);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "برچسب با موفقیت ذخیره شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// خواندن اطلاعات برچسب کارگاه از دیتابیس
        /// </summary>
        /// <param name="id">ردیف برچسب کارگاه</param>
        /// <returns>اطلاعات برچسب کارگاه</returns>
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var data = db.WorkshopTag.Where(x => x.Id == id).Select(x => new WorkshopTagViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        workshopId = x.WorkshopId
                    }).SingleOrDefault();
                    response = new Response()
                    {
                        status = 200,
                        data = data
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
        /// حذف  برچسب کارگاه از دیتابیس
        /// </summary>
        /// <param name="id">ردیف برچسب کارگاه</param>
        /// <returns>نتیجه حذف</returns>
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.WorkshopTag.Find(id);
                    db.WorkshopTag.Remove(entity);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        data = "Done"
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
        /// خواندن لیست برچسب کارگاه
        /// </summary>
        /// <returns>بیس برچسب کارگاه</returns>
        public JsonResult Search(WorkshopTagViewModel model)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var query = db.WorkshopTag.Select(x => x);
                    var list = query.OrderBy(x => x.Id).Skip(model.page * model.count).Take(model.count).Select(x => new WorkshopTagViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        workshopId = x.WorkshopId,
                        workshop = x.Workshop.Name
                    }).ToList();
                    var dataCount = query.Count();
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
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}