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
    public class CategoryUsableProductController : BaseController
    {
        /// <summary>
        /// صفحه اصلی 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , createUsableProduct")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// ویرایش دسته بندی
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin , createUsableProduct")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            ViewBag.ParentId = id;
            return View();
        }
        /// <summary>
        /// افزودن دسته بندی
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(int? id)
        {
            ViewBag.Id = id;
            return View("Edit");
        }

        public JsonResult SaveChild(CategoryUsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {

                        var category = db.CategoryUsableProduct.Single(x => x.Id == model.id);
                        category.Title = model.title;
                        category.Order = model.order;
                        category.Active = model.active;
                        category.ModifyUserId = userId;
                        category.ModifyDate = DateTime.Now;
                        category.Ip = Request.UserHostAddress;

                    }
                    else
                    {
                        var item = new CategoryUsableProduct()
                        {
                            Title = model.title,
                            ParentId = model.parentId,
                            Order = model.order,
                            Active = model.active,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.CategoryUsableProduct.Add(item);
                    }
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200,
                    message = "محصول جدید ثبت شد",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ذخیره محصول
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin , createUsableProduct")]

        public JsonResult Save(CategoryUsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {
                        if (model.parentId == null)
                        {
                            var category = db.CategoryUsableProduct.Single(x => x.Id == model.id);
                            category.Title = model.title;
                            category.Order = model.order;
                            category.ParentId = model.parentId;
                            category.Active = model.active;
                            category.ModifyUserId = userId;
                            category.ModifyDate = DateTime.Now;
                            category.Ip = Request.UserHostAddress;
                        }
                        else
                        {
                            var item = new CategoryUsableProduct()
                            {
                                Title = model.title,
                                ParentId = model.parentId,
                                Order = model.order,
                                Active = model.active,
                                CreateUserId = userId,
                                ModifyUserId = userId,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress,
                            };
                            db.CategoryUsableProduct.Add(item);
                        }
                    }
                    else
                    {
                        var item = new CategoryUsableProduct()
                        {
                            Title = model.title,
                            ParentId = model.parentId,
                            Order = model.order,
                            Active = model.active,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.CategoryUsableProduct.Add(item);
                    }
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200,
                    message = "محصول جدید ثبت شد",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// خواندن اطلاعات محصول
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون اطلاعات لود شده پرسنل</returns>
        [HttpGet]
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                CategoryUsableProduct item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.CategoryUsableProduct.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new CategoryUsableProductViewModel
                        {
                            id = item.Id,
                            title = item.Title,
                            order = item.Order,
                            active = item.Active,
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "محصول مورد نظر یافت نشد."
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
        /// خواندن اطلاعات فرزند دسته بندی
        /// </summary>
        /// <param name="id">ردیف فرزند</param>
        /// <returns>جیسون اطلاعات لود شده پرسنل</returns>
        [HttpGet]
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult LoadChild(int id)
        {
            Response response;
            try
            {
                CategoryUsableProduct item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.CategoryUsableProduct.FirstOrDefault(x => x.Id == id && x.ParentId != null);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new CategoryUsableProductViewModel
                        {
                            id = item.Id,
                            title = item.Title,
                            order=item.Order,
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "محصول مورد نظر یافت نشد."
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
        /// لیست محصولات
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin , createUsableProduct,orderUsableProduct")]

        public JsonResult Search(CategoryUsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    List<CategoryUsableProductViewModel> list;
                    var query = db.CategoryUsableProduct.Where(x => x.ParentId == null).OrderBy(x=>x.Order);
                    list = query.Select(item => new CategoryUsableProductViewModel()
                    {
                        id = item.Id,
                        title = item.Title,
                        parentId = item.ParentId,
                        order = item.Order,
                        active = item.Active,
                        children = item.ChildList.OrderByDescending(x=>x.Order).Where(x=>x.ParentId != null).Select(x => new SearchCategoryUsableProductSearchViewModel()
                        {
                            id = x.Id,
                            order=x.Order,
                            parentId = x.ParentId,
                            childTitle = x.Title,
                        }).ToList(),
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list
                        },
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
        /// حذف محصول
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin , createUsableProduct")]

        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var item = db.CategoryUsableProduct.Find(Id);
                    if (item.ChildList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "برای حذف دسته بندی ابتدا، زیردسته بندی های آن را حذف کنید ."
                        };
                    }
                    else
                    {
                        db.CategoryUsableProduct.Remove(item);
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "دسته بندی با موفقیت حذف شد."
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
        /// حذف محصول
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin , createUsableProduct")]

        public JsonResult DeleteChild(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.CategoryUsableProduct.Find(Id);
                    if (item.UsableProductList.Count == 0)
                    {
                        db.CategoryUsableProduct.Remove(item);
                        db.SaveChanges();

                        response = new Response()
                        {
                            status = 200,
                            message = "زیردسته بندی با موفقیت حذف شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "زیردسته بندی با محصول ثبت شده در بخش چاپخانه مرتبط است و امکان حذف آن نمیباشد.."
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
    }
}
