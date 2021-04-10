using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class InstagramPostController : BaseController
    {
        /// <summary>
        /// صفحه اصلی
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            return View();
        }
        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// اضافه کردن
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View("Edit");
        }
        /// <summary>
        /// ذخیره کردن مقادیر
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult Save(InstagramPostViewModel model)
        {
            Response response;
            try
            {
                var date = DateUtility.GetDateTime(model.publishDate);
                DateTime dateTimeNow = DateTime.Today;
                if (date < dateTimeNow)
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "شما قادر به انتخاب روزهای گذشته نیستید.",
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    int userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {

                        var entity = db.InstagramPost.Single(x => x.Id == model.id);
                        entity.Title = model.title;
                        entity.Respite = model.respite;
                        entity.DayCounter = model.dayCounter;
                        entity.FileId = model.fileId;
                        entity.FileName = model.fileName;
                        entity.PublishDate = DateUtility.GetDateTime(model.publishDate);
                        entity.InstagramPostType = model.instagramPostType;
                        entity.ModifyUserId = userId;
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;
                        message = "عملیات با موفقیت ویرایش شد.";
                    }
                    else
                    {
                        var entity = new InstagramPost()
                        {
                            Title = model.title,
                            Respite = model.respite,
                            DayCounter = model.dayCounter,
                            FileId = model.fileId,
                            FileName = model.fileName,
                            PublishDate = DateUtility.GetDateTime(model.publishDate),
                            InstagramPostType = model.instagramPostType,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.InstagramPost.Add(entity);
                        message = "عملیت با موفقیت ایجاد شد.";
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
        /// خواندن مقادیر 
        /// </summary>
        /// <param name="id">ردیف </param>
        /// <returns>جیسون اطلاعات لود شده سوالات</returns>
        [HttpGet]
        [Authorize(Roles = "admin,")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.InstagramPost.Where(x => x.Id == id).SingleOrDefault();
                    var data = new InstagramPostViewModel()
                    {
                        id = item.Id,
                        title = item.Title,
                        instagramPostType = item.InstagramPostType,
                        dayCounter = item.DayCounter,
                        publishDate = DateUtility.GetPersianDate(item.PublishDate),
                        respite = item.Respite,
                        fileId = item.FileId,
                        fileName = item.FileName,
                    };
                    response = new Response()
                    {
                        status = 200,
                        data = data,
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
        /// برگرداندن لیست مقادیر ذخیره شده
        /// </summary>
        /// <param name = "model" > مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست محصولات و پست های اینستاگرام</returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public JsonResult Search(InstagramPostParamsViewModel model)
        {
            Response response;
            try
            {
                List<InstagramPostSearchViewModel> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.InstagramPost.Select(x => x);
                    if (model.instagramPostType != null)
                    {
                        query = query.Where(x => x.InstagramPostType == model.instagramPostType);
                    }

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    list = query.Select(item => new InstagramPostSearchViewModel()
                    {
                        id = item.Id,
                        title = item.Title,
                        dayCounter = item.DayCounter,
                        publishDate = item.PublishDate,
                        fileId = item.FileId,
                        fileName = item.FileName,
                        respite = item.Respite,
                        instagramPostType = item.InstagramPostType,
                    }).ToList();

                    list.ForEach(x =>
                    {
                        x.persianPublishDate = DateUtility.GetPersianDate(x.publishDate);
                        x.instagramPostTypeTitle = Enums.GetTitle(x.instagramPostType);
                        x.finalPersianPublishDate = DateUtility.GetPersianDate(x.publishDate.Value.AddDays(x.dayCounter));

                    });
                }

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
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// برگرداندن لیست مقادیر ذخیره شده
        /// </summary>
        /// <param name = "model" > مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست محصولات و پست های اینستاگرام</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult ReturnDate(InstagramPostDateViewModel model)
        {
            Response response;
            try
            {
                PersianCalendar Calendar = new PersianCalendar();
                //var date = DateUtility.GetDateTime(model.publishDate);
                var date = DateUtility.GetDateTime(model.publishDate);
                var finalPublishDate = date.Value.AddDays(model.dayCounter);
                var finalPersianPublishDate = DateUtility.GetPersianDate(finalPublishDate);
                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        finalPersianPublishDate,
                    },
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// حذف پرسنل
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.InstagramPost.Find(Id);
                    response = new Response()
                    {
                        status = 200,
                        message = "پست مورد نظر با موفقیت حذف شد."
                    };
                    db.InstagramPost.Remove(item);
                    db.SaveChanges();
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