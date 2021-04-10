using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر دسته بندی اس ام اس ها 
    /// </summary>
    public class SmsCategoryController : BaseController
    {
        /// <summary>
        /// صفحه لیست دسته بندی ها
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,createSms")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ویرایش دسته بندی
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,createSms")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// دسته بندی جدید
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,createSms")]
        public ActionResult Add()
        {
            return View("Edit");
        }

        /// <summary>
        /// ذخیره دسته بندی
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,createSms")]
        public JsonResult Save(SmsCategoryViewModel model)
        {
            Response response;
            try
            {
                if (model.categoryType == SmsCategoryType.MultiChoice)
                {
                    if (model.createMessageList == null || model.createMessageList.Count == 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "ثبت یک پاسخ برای نوع چند انتخابی الزامی است."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                if (model.categoryType == SmsCategoryType.SingleChoice)
                {
                    if (model.createMessageList == null || model.createMessageList.Count == 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "ثبت یک پاسخ برای نوع تک انتخابی الزامی است."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                if (model.categoryType == SmsCategoryType.Descriptive)
                {
                    if (model.description == null)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "ثبت یک پاسخ برای نوع تشریحی الزامی است."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                if (model.categoryType == SmsCategoryType.FreeMessage)
                {
                    if (model.freeMessage == null)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "ثبت یک پاسخ برای نوع پیام آزاد الزامی است."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                var userId = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.SmsCategory.Single(x => x.Id == model.id);
                        entity.Title = model.title;
                        entity.FreeMessage = model.freeMessage;
                        entity.Active = model.active;
                        entity.Description = model.description;
                        entity.Color = model.color;
                        entity.CategoryType = model.categoryType;
                        entity.Order = model.order;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyUserId = userId;
                        entity.Ip = Request.UserHostAddress;

                        if (model.categoryType == SmsCategoryType.MultiChoice || model.categoryType == SmsCategoryType.SingleChoice)
                        {
                            var idList = model.createMessageList.Where(x => x.id != null).Select(x => x.id).ToList();
                            var deletedItems = entity.CreateMessageList.Where(x => !idList.Any(y => y == x.Id)).ToList();
                            if (deletedItems.Count > 0)
                                db.CreateMessage.RemoveRange(deletedItems);
                            model.createMessageList.ForEach(x =>
                            {
                                if (x.id != null && x.id > 0)
                                {
                                    var item = entity.CreateMessageList.Single(y => y.Id == x.id);
                                    item.Name = string.IsNullOrEmpty(x.name) ? null : Regex.Replace(x.name, " {2,}", " ").Trim();
                                    item.DetailName = Regex.Replace(x.detailName, " {2,}", " ").Trim();
                                    item.ModifyUserId = userId;
                                    item.ModifyDate = DateTime.Now;
                                    item.Ip = Request.UserHostAddress;
                                }
                                else
                                {
                                    db.CreateMessage.Add(new CreateMessage()
                                    {
                                        SmsCategory = entity,
                                        Name = string.IsNullOrEmpty(x.name) ? null : Regex.Replace(x.name, " {2,}", " ").Trim(),
                                        DetailName = Regex.Replace(x.detailName, " {2,}", " ").Trim(),
                                        CreateUserId = userId,
                                        ModifyUserId = userId,
                                        CreateDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        Ip = Request.UserHostAddress,
                                    });
                                }
                            });
                        }
                        else
                        {
                            db.CreateMessage.RemoveRange(entity.CreateMessageList);
                        }
                    }
                    else
                    {
                        var entity = new SmsCategory()
                        {
                            Title = model.title,
                            Active = model.active,
                            FreeMessage = model.freeMessage,
                            CategoryType = model.categoryType,
                            Description = model.description,
                            Color = model.color,
                            Order = model.order,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreateUserId = 1,
                            ModifyUserId = 1,
                            Ip = Request.UserHostAddress
                        };
                        if (model.createMessageList != null)
                        {
                            for (int i = 0; i < model.createMessageList.Count; i++)
                            {
                                entity.CreateMessageList.Add(new CreateMessage()
                                {
                                    SmsCategory = entity,
                                    Name = string.IsNullOrEmpty(model.createMessageList[i].name) ? null : Regex.Replace(model.createMessageList[i].name, " {2,}", " ").Trim(),
                                    DetailName = Regex.Replace(model.createMessageList[i].detailName, " {2,}", " ").Trim(),
                                    CreateUserId = GetAuthenticatedUserId(),
                                    ModifyUserId = GetAuthenticatedUserId(),
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress,
                                });
                            }
                        }
                        db.SmsCategory.Add(entity);
                    }
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "دسته بندی با موفقیت ایجاد شد."
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
        /// بارگزاری اطلاعات دسته بندی
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,createSms,sendSms")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.SmsCategory.Single(x => x.Id == id);
                    var data = new SmsCategoryViewModel()
                    {
                        id = entity.Id,
                        title = entity.Title,
                        freeMessage = entity.FreeMessage,
                        active = entity.Active,
                        order = entity.Order,
                        description = entity.Description,
                        color = entity.Color,
                        categoryType = entity.CategoryType,
                        createMessageList = entity.CreateMessageList.Select(x => new CreateMessageViewModel()
                        {
                            id = x.Id,
                            detailName = x.DetailName,
                            name = x.Name,
                        }).ToList(),
                    };
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
        /// حذف دسته بندی
        /// </summary>
        /// <param name="id"></param>
        /// <returns>حذف دسته بندی</returns>
        [Authorize(Roles = "admin,createSms")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.SmsCategory.Find(id);

                    if (entity.SmsTextList.Count() == 0)
                    {
                        db.SmsCategory.Remove(entity);
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "دسته بندی با موفقیت حذف شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "پیام هایی برای این دسته بندی ثبت شده است,شما نمیتوانیداین دسته بندی را حذف کنید."
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
        /// <summary>
        /// جستجوی محصولات
        /// </summary>
        /// <returns>لیست محصولات</returns>
        [Authorize(Roles = "admin,createSms")]
        public JsonResult Search()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.SmsCategory.Select(x => new SmsCategoryViewModel()
                    {
                        id = x.Id,
                        title = x.Title,
                        active = x.Active,
                        order = x.Order,
                    }).OrderBy(x => x.order).ToList();

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
        /// <summary>
        /// گرفتن لیست دسته بندی
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,sendSms")]
        public JsonResult CategoryList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.SmsCategory.Select(x => x).OrderBy(x => x.Order);
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        title = x.Title,
                        color = x.Color,
                    }).ToList();
                    if (list.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new
                            {
                                list = list.Select(x => new
                                {
                                    id = x.id,
                                    title = x.title,
                                    color = x.color,
                                }).ToList()
                            }
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "دسته بندی تعریف نشده است."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
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
        /// گرفتن لیست دسته بندی
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,sendSms")]
        public JsonResult CategoryListDetail(int? id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.SmsCategory.Where(x => x.Id == id);
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        description = x.Description,
                        freeMessage = x.FreeMessage,
                        color = x.Color,
                        detail = x.CreateMessageList.Select(y => new
                        {
                            name = y.Name,
                            detailName = y.DetailName,
                        }).ToList(),

                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list.Select(x => new
                            {
                                id = x.id,
                                description = x.description,
                                freeMessage=x.freeMessage,
                                color = x.color,
                                detail = x.detail.Select(y => new
                                {
                                    name = y.name,
                                    detailName = y.detailName,
                                }).ToList(),
                            }).ToList()
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

        /// <summary>
        /// ارسال پیام
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,sendSms")]
        public ActionResult Send()
        {
            return View();
        }

        /// <summary>
        /// جیسون مربوط به ارسال پیام به مشتری
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,sendSms")]
        public JsonResult SendMessage(SendMessageViewModel model)
        {
            Response response;
            try
            {
                Task.Factory.StartNew(() =>
                {
                    NikSmsWebServiceClient.SendSmsNik("کیاگالری؛" + "\n" + model.text, model.phoneNumber);
                });
                response = new Response
                {
                    status = 200,
                    message = "پیام با موفقیت ارسال شد",
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