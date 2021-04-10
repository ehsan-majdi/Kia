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
    /// کنترلر سنگ
    /// </summary>
    public class StoneController : BaseController
    {
        /// <summary>
        /// مدیریت سنگ
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, stone")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ویرایش سنگ
        /// </summary>
        /// <param name="id">ردیف سنگ</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, stone")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            ViewBag.Title = "ویرایش سنگ";
            return View();
        }

        /// <summary>
        /// سنگ جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, stone")]
        public ActionResult Add()
        {
            ViewBag.Title = "سنگ جدید";
            return View("Edit");
        }

        /// <summary>
        /// کاتالوگ سنگ
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        public ActionResult Catalog()
        {
            using (var db = new KiaGalleryContext())
            {
                var stoneList = db.Stone.Where(x=>x.Active == true).ToList();
                ViewBag.Transparent = stoneList.Where(x => x.StoneType == StoneType.Transparent).OrderBy(x => x.Order).ToList();
                ViewBag.Sedimentary = stoneList.Where(x => x.StoneType == StoneType.Sedimentary).OrderBy(x => x.Order).ToList();
                ViewBag.Pearl = stoneList.Where(x => x.StoneType == StoneType.Pearl).OrderBy(x => x.Order).ToList();
                //ViewBag.Atomic = stoneList.Where(x => x.StoneType == StoneType.Atomic).OrderBy(x => x.Order).ToList();
                ViewBag.LeatherBraceletStone = stoneList.Where(x => x.StoneType == StoneType.LeatherBraceletStone).OrderBy(x => x.Order).ToList();
                ViewBag.BraceletStone = stoneList.Where(x => x.StoneType == StoneType.BraceletStone).OrderBy(x => x.Order).ToList();
            }
            return View();
        }

        /// <summary>
        /// ذخیره سنگ
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات سنگ</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, stone")]
        public JsonResult Save(StoneViewModel model)
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
                        message = "وارد کردن نام سنگ اجباری است.";
                    }
                    else
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.Stone.Single(x => x.Id == model.id);
                            entity.Name = model.name.Trim();
                            entity.EnglishName = model.englishName.Trim();
                            entity.StoneType = model.stoneType;
                            entity.Order = model.order;
                            entity.FileName = model.fileName?.Trim();
                            entity.Active = model.active;
                            entity.ModifyUserId = GetAuthenticatedUserId();
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            if (!string.IsNullOrEmpty(entity.FileName) && entity.FileName != model.fileName)
                                oldFileName = entity.FileName;

                            message = "سنگ با موفقیت ویرایش شد.";
                        }
                        else
                        {
                            var entity = new Stone()
                            {
                                Name = model.name.Trim(),
                                EnglishName = model.englishName.Trim(),
                                StoneType = model.stoneType,
                                Order = model.order,
                                FileName = model.fileName?.Trim(),
                                Active = model.active,
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };

                            db.Stone.Add(entity);
                            message = "سنگ با موفقیت ایجاد شد.";
                        }
                        db.SaveChanges();

                        if (!string.IsNullOrEmpty(oldFileName) && System.IO.File.Exists(Server.MapPath("~/Upload/Stone/" + oldFileName)))
                            System.IO.File.Delete(Server.MapPath("~/Upload/Stone/" + oldFileName));
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
        /// خواندن اطلاعات سنگ
        /// </summary>
        /// <param name="id">ردیف جرم</param>
        /// <returns>جیسون اطلاعات لود شده سنگ</returns>
        [HttpGet]
        [Authorize(Roles = "admin, stone")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Stone item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Stone.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new StoneViewModel
                        {
                            id = item.Id,
                            name = item.Name,
                            englishName = item.EnglishName,
                            stoneType = item.StoneType,
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
                        message = "سنگ مورد نظر یافت نشد."
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
        /// جستجوی سنگ
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست سنگ های پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, stone")]
        public JsonResult Search(StoneSearchViewModel model)
        {
            Response response;
            try
            {
                List<Stone> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Stone.Select(x => x);

                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.Name.Contains(model.term.Trim()) || x.Name.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")));
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
                            stoneType = item.StoneType,
                            stoneTypeTitle = Enums.GetTitle(item.StoneType),
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
        /// حذف سنگ
        /// </summary>
        /// <param name="id">ردیف سنگ</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, stone")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Stone.Single(x => x.Id == id);

                    if (item.ProductStoneList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این سنگ به محصول متصل می باشد."
                        };
                    }
                    else if (item.CartProductStoneList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این سنگ در سبد خرید بک شعبه ثبت شده می باشد."
                        };
                    }
                    else if (item.OrderDetailStoneList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این سنگ در یک سفارش ثبت شده می باشد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "سنگ با موفقیت حذف شد."
                        };
                        db.Stone.Remove(item);
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
        /// غیر فعال کردن سنگ
        /// </summary>
        /// <param name="id">ردیف سنگ</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, stone")]
        public JsonResult Inactive(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Stone.First(x => x.Id == id);
                    item.Active = false;
                    response = new Response()
                    {
                        status = 200,
                        message = "سنگ با موفقیت غیرفعال شد."
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
        /// فعال کردن سنگ
        /// </summary>
        /// <param name="id">ردیف سنگ</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, stone")]
        public JsonResult Active(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Stone.First(x => x.Id == id);
                    item.Active = true;
                    response = new Response()
                    {
                        status = 200,
                        message = "سنگ با موفقیت فعال شد."
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
        /// دریافت همه سنگ های فعال
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام سنگ ها</returns>
        [HttpGet]
        public JsonResult GetAll()
        {
            Response response;
            try
            {
                List<Stone> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Stone.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
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
                            stoneType = item.StoneType,
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