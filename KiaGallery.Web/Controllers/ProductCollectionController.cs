using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class ProductCollectionController : BaseController
    {
        /// <summary>
        /// صفحه لیست کالکشن محصولات
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// صفحه اضافه کردن کالکشن جدید 
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Add()
        {
            return View("Edit");
        }

        /// <summary>
        /// صفحه ویرایش کردن کالکشن جدید 
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// ذخیره کالکشن در دیتابیس
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات کالکشن</param>
        /// <returns>نتیجه ذخیره</returns>
        public JsonResult Save(ProductCollectionViewModel model)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                string oldFileName = "";
                using (var db = new KiaGalleryContext())
                {
                    if (model.id > 0)
                    {

                        var entity = db.ProductCollection.SingleOrDefault(x => x.Id == model.id);
                        entity.Title = model.title;
                        entity.Active = model.active;
                        entity.FileName = model.fileName;
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                        if (!string.IsNullOrEmpty(entity.FileName) && entity.FileName != model.fileName)
                            oldFileName = entity.FileName;
                    }
                    else
                    {
                        var entity = new ProductCollection()
                        {
                            Title = model.title,
                            Active = model.active,
                            FileName = model.fileName,
                            CreateUserId = currentUser.Id,
                            ModifyUserId = currentUser.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        };
                        db.ProductCollection.Add(entity);
                    }
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(oldFileName) && System.IO.File.Exists(Server.MapPath("~/Upload/ProductCollections/" + oldFileName)))
                        System.IO.File.Delete(Server.MapPath("~/Upload/ProductCollections/" + oldFileName));
                }
                response = new Response()
                {
                    status = 200,
                    message = "Done"
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// خواندن اطلاعات کالکشن از دیتابیس
        /// </summary>
        /// <param name="id">ردیف کالکشن</param>
        /// <returns>اطلاعات کالکشن</returns>
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var data = db.ProductCollection.Where(x=>x.Id == id).Select(x => new ProductCollectionViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        active = x.Active,
                        fileName = x.FileName
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
        /// حذف  کالکشن از دیتابیس
        /// </summary>
        /// <param name="id">ردیف کالکشن</param>
        /// <returns>نتیجه حذف</returns>
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.ProductCollection.Find(id);
                    db.ProductCollection.Remove(entity);
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
        /// خواندن لیست کالکشن
        /// </summary>
        /// <returns>بیس کالکشن</returns>
        public JsonResult Search()
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var list = db.ProductCollection.Select(x => new ProductCollectionViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        fileName = x.FileName,
                        active = x.Active
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list
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