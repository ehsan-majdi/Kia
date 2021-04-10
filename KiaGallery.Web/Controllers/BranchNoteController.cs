using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Order;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر یاداشت شعبه
    /// </summary>
    public class BranchNoteController : BaseController
    {
        /// <summary>
        /// مدیریت یاداشت شعبه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            
            return View();
        }

        /// <summary>
        /// ویرایش یاداشت شعبه
        /// </summary>
        /// <param name="id">ردیف یاداشت شعبه</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            ViewBag.Title = "ویرایش یاداشت";
            return View();
        }

        /// <summary>
        /// یاداشت شعبه جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Add()
        {
            ViewBag.Title = "یاداشت جدید";
            return View("Edit");
        }

        /// <summary>
        /// جستجوی یاداشت شعبه
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست یاداشت شعبه پیدا شده</returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult Search(SearchBranchNoteViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                var user = GetAuthenticatedUser();
                List<BranchNoteViewModel> list = new List<BranchNoteViewModel>();
                int dataCount = 0;
                double pageCount = 0;
                if (user.BranchId != null)
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var query = db.BranchNote.Where(x=> x.BranchId == user.BranchId);
                        if (!string.IsNullOrEmpty(model.term))
                            query = query.Where(x=> x.CreateUser.FirstName.Contains(model.term) || x.CreateUser.FirstName.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.CreateUser.LastName.Contains(model.term) || x.CreateUser.LastName.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")));
                        dataCount = query.Count();
                        list = query.OrderByDescending(x=> x.Id).Skip(model.count * model.page).Take(model.count).Select(x=> new BranchNoteViewModel() {
                            id = x.Id,
                            text = x.Text,
                            createUserName = x.CreateUser.FirstName + " "+ x.CreateUser.LastName,
                            CreateDate = x.CreateDate
                        }).ToList();
                    }
                    list.ForEach(x => x.createDate = DateUtility.GetPersianDate(x.CreateDate));

                    if (dataCount > 0)
                        pageCount = Math.Ceiling((double)dataCount / model.count);
                }
                response = new Response()
                {
                    data = new
                    {
                        list = list,
                        pageCount = pageCount,
                        count = dataCount,
                        page = model.page + 1
                    },
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
        /// ذخیره یاداشت شعبه
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات یاداشت شعبه</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Save(BranchNoteSaveViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                var user = GetAuthenticatedUser();
                if(user.BranchId != null)
                {
                    using (var db = new KiaGalleryContext())
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.BranchNote.Single(x => x.Id == model.id);
                            entity.Text = model.text;
                            entity.ModifyUserId = user.Id;
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            message = "یاداشت شما ثبت شد.";
                        }
                        else
                        {
                            BranchNote entity = new BranchNote()
                            {
                                Text = model.text,
                                BranchId = user.BranchId.GetValueOrDefault(),
                                CreateUserId = user.Id,
                                ModifyUserId = user.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };

                            db.BranchNote.Add(entity);
                            message = "یاداشت شما ثبت شد.";
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    message = "شما مجاز به ثبت یاداشت نمی باشید.";
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
        /// <param name="id">ردیف یاداشت شعبه</param>
        /// <returns>جیسون اطلاعات لود شده یاداشت شعبه</returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult Load(int id)
        {
            Response response;
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                var entity = db.BranchNote.FirstOrDefault(x => x.BranchId == user.BranchId && x.Id == id);
                if (entity != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            id = entity.Id,
                            text = entity.Text
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 500,
                    };
                }

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// حذف یادداشت شعبه
        /// </summary>
        /// <param name="id">ردیف یادداشت شعبه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.BranchNote.Find(Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "یادداشت شعبه با موفقیت حذف شد."
                    };
                    db.BranchNote.Remove(item);
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