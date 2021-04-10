using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using KiaGallery.Model;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر مدیریت اس ام اس
    /// </summary>
    public class SmsTextController : BaseController
    {
        /// <summary>
        /// صفحه اس ام اس و مدیریت
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, createSms")]
        public ActionResult Index()
        {

            return View();
        }
        [Authorize(Roles ="admin,createSms")]
        public ActionResult Concept()
        {
            return View();
        }
        [Authorize(Roles = "admin, createSms")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [Authorize(Roles = "admin, createSms")]
        public ActionResult Add()
        {
            return View("Edit");
        }
        [HttpPost]
        [Authorize(Roles = "admin, createSms")]
        public JsonResult Save(SmsTextViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (model.id > 0 && model.id != null)
                    {
                        var entity = db.SmsText.Single(x => x.Id == model.id);
                        entity.Title = model.title;
                        entity.Active = model.active;
                        entity.Text = model.text;
                        entity.UrlKey = model.urlKey;
                        entity.SmsCategoryId = model.smsCategoryId;
                        entity.Order = model.order;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyUserId = currentUser.Id;
                        entity.Ip = Request.UserHostAddress;
                        db.SaveChanges();

                        response = new Response()
                        {
                            status = 200,
                            message = "پیام با موفقیت ویرایش شد."
                        };
                    }
                    else
                    {
                        var entity = new SmsText()
                        {
                            Title = model.title,
                            Text = model.text,
                            UrlKey = model.urlKey,
                            SmsCategoryId = model.smsCategoryId,
                            Order = model.order,
                            Active = model.active,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreateUserId = currentUser.Id,
                            ModifyUserId = currentUser.Id,
                            Ip = Request.UserHostAddress
                        };
                        db.SmsText.Add(entity);

                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "پیام با موفقیت ایجاد شد."
                        };
                    }
                }

            }
            catch (Exception ex)
            {

                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        [Authorize(Roles = "admin, createSms")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.SmsText.Single(x => x.Id == id);
                    response = new Response()
                    {
                        status = 200,
                        data = new SmsTextViewModel()
                        {
                            id = entity.Id,
                            title = entity.Title,
                            active = entity.Active,
                            text = entity.Text,
                            urlKey = entity.UrlKey,
                            smsCategoryId = entity.SmsCategoryId,
                            order = entity.Order

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
        [AllowAnonymous]
        [Authorize(Roles = "admin, createSms")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.SmsText.Find(id);
                    db.SmsText.Remove(entity);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "پیام با موفقیت حذف شد."
                    };


                }
            }
            catch (Exception ex)
            {

                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        [Authorize(Roles = "admin, createSms")]
        public JsonResult Search()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.SmsText.Where(x => x.Active == false).OrderBy(x=>x.Order).Select(x => new SmsTextViewModel()
                    {
                        id = x.Id,
                        title = x.Title,
                        text = x.Text,
                        active=x.Active,
                        urlKey = x.UrlKey,
                        categoryTitle = x.SmsCategory.Title,
                        order = x.Order
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
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
        [AllowAnonymous]
        [Authorize(Roles = "admin,createSms")]
        public JsonResult LoadConcept()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.SmsText.Where(x => x.Active == true).Select(x => new SmsTextViewModel()
                    {
                        id = x.Id,
                        text = x.Text,
                        title = x.Title,
                        urlKey = x.UrlKey,
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new { list = list },
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
