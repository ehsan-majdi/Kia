using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KiaGallery.Web.SmsHandler;
using ClosedXML.Excel;

namespace KiaGallery.Web.Controllers
{
    public class FaxCartController : BaseController
    {
        /// <summary>
        /// صفحه اصلی جهت مشاهده تمام سفارشات محصول چاپخانه
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , orderUsableProduct")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.ParentCategory = db.CategoryUsableProduct.OrderBy(x => x.Order).Where(x => x.ParentId == null).ToList();
            }
            return View();
        }
        /// <summary>
        /// نمایش سفارشات به صورت لیستی و تایید آنها برای ارسال به سمت وضعیت سفارشات
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult GetOrderUsableProductCartList(ShowOrderUsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var query = db.UsableProductCart.Where(x => x.BranchId == user.BranchId).Select(x => x);
                    query = query.Where(x => x.Count > 0);

                    if (model.categoryId != null && model.categoryId > 0)
                    {
                        query = query.Where(x => x.UsableProduct.CategoryUsableProduct.Id == model.categoryId);
                    }
                    if (model.childId != null && model.childId > 0)
                    {
                        query = query.Where(x => x.UsableProduct.CategoryUsableProduct.Id == model.childId);
                    }

                    var list = query.Select(item => new ShowOrderUsableProductViewModel
                    {
                        id = item.Id,
                        name = item.UsableProduct.Name,
                        childId = item.UsableProduct.CategoryUsableProduct.ParentId,
                        productCount = item.Count,
                        usableProductId = item.UsableProductId,
                        description = item.Description,
                        specification = item.Specification,
                        createUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        image = item.UsableProduct.UsableProductFileList.Select(y => y.FileName).FirstOrDefault(),
                        category = item.UsableProduct.CategoryUsableProduct.Title,
                        unit = item.UsableProduct.Unit,
                        createDate = item.CreateDate
                    }).OrderBy(item => item.id).ToList();

                    list.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.createDate);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
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
        /// اضافه کردن به سبد خرید
        /// </summary>
        /// <param name="productId">ردیف محصول</param>
        /// <param name="count">تعداد</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult AddToCart(int usableProductId, int count)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    var userId = GetAuthenticatedUserId();

                    var branchId = db.User.Find(userId).BranchId;

                    for (int i = 0; i < count; i++)
                    {
                        var item = new OrderUsableProduct()
                        {
                            BranchId = branchId.GetValueOrDefault(),
                            Count = 1,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.OrderUsableProduct.Add(item);
                    }
                    db.SaveChanges();
                    message = "محصول با موفیقت به سبد خرید اضافه شد.";
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
        /// حذف کردن محصول از سبد خرید
        /// </summary>
        /// <param name="id">ردیف سبد خرید</param>
        /// <param name="productId">ردیف محصول</param>
        /// <returns>جیسون نتیجه عملیات حذف</returns>
        [HttpPost]
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult RemoveCartItem(string id)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var idList = id.Split('-').Select(x => int.Parse(x)).ToList();

                    var branchId = user.BranchId.GetValueOrDefault();
                    if (idList != null && idList.Count > 0)
                    {
                        var entityList = db.UsableProductCart.Where(x => x.BranchId == branchId && idList.Any(y => y == x.Id)).ToList();
                        if (entityList != null && entityList.Count > 0)
                        {
                            entityList.ForEach(entity =>
                            {
                                db.UsableProductCart.Remove(entity);
                            });
                            db.SaveChanges();
                            status = 200;
                            message = "محصول با موفیقت از سبد خرید حذف شد.";
                        }
                        else
                        {
                            status = 200;
                            message = "محصول مورد نظر یافت نشد.";
                        }
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
        /// کاهش تعداد بر اساس یک سفارش
        /// </summary>
        /// <param name="id">لیست ردیف محصول که می بایست یکی از موارد آن حذف شود</param>
        /// <returns>جیسون حاوی نتیجه کاهش تعداد</returns>
        [HttpPost]
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult Decrement(string id)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();

                    var idList = id.Split('-').Select(x => int.Parse(x));

                    var entity = db.UsableProductCart.OrderByDescending(x => x.Id).First(x => x.BranchId == user.BranchId && idList.Any(y => y == x.Id));
                    entity.Count = entity.Count - 1;
                    if (entity.Count == 0)
                    {
                        db.UsableProductCart.Remove(entity);
                    }
                    db.SaveChanges();
                    message = "محصول با موفیقت به سبد خرید اضافه شد.";
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
        /// افزایش تعداد بر اساس یک سفارش
        /// </summary>
        /// <param name="id">لیست ردیف محصول که می بایست تکرار شود</param>
        /// <returns>جیسون حاوی نتیجه افزایش تعداد</returns>
        [HttpPost]
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult Increment(ShowOrderUsableProductViewModel model, string id)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var branchId = user.BranchId.GetValueOrDefault();
                    var idList = id.Split('-').Select(x => int.Parse(x));

                    var entity = db.UsableProductCart.First(x => x.BranchId == branchId && idList.Any(y => y == x.Id));
                    if (entity != null)
                    {
                        entity.Count = entity.Count + 1;
                    }
                    db.SaveChanges();
                    message = "محصول با موفیقت به سبد خرید اضافه شد.";
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
        /// ساخت سفارش از سبد خرید
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin , orderUsableProduct")]
        [HttpPost]
        public JsonResult MakeOrder(MakeOrderUsableProductViewModel model)
        {
            Response response;
            try
            {
                List<UsableProductCart> result;
                List<string> specification = new List<string>();
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    Branch branch = db.Branch.Single(x => x.Id == user.BranchId);
                    var query = db.UsableProductCart.Select(x => x);
                    result = query.ToList();
                    if (result.Count > 0)
                    {
                        var order = new OrderUsableProduct()
                        {
                            BranchId = GetAuthenticatedUser().BranchId.GetValueOrDefault(),
                            PrintingHouserOrder = false,
                            OrderUsableProductStatus = OrderUsableProductStatus.Registered,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        if (user.BranchType == BranchType.Solicitorship)
                        {
                            order.PrintingHouserOrder = true;
                        }

                        order.OrderUsableProductDetailList = result.Select(x => new OrderUsableProductDetail()
                        {
                            UsableProductId = x.UsableProductId,
                            OrderUsableProduct = order,
                            Count = x.Count,
                            Description = x.Description,
                            Specification=x.Specification,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        }).ToList();
                        db.OrderUsableProduct.Add(order);
                        //for (var i = 0; i < model.id.Count(); i++)
                        //{
                        //    order.OrderUsableProductDetailList[i].Specification = model.specification[i];
                        //}
                        db.UsableProductCart.RemoveRange(result);
                        db.SaveChanges();

                        var persianDate = DateUtility.GetPersianDate(order.CreateDate);
                        var mobileNumber = order.Branch.PersonList.Where(x => x.Supervisor == true).Select(x => x.MobileNumber).FirstOrDefault();
                        var alias = order.Branch.Alias;
                        if (!User.IsInRole("admin"))
                        {
                            Task.Factory.StartNew(() =>
                            {
                                NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + " تعداد : " + order.OrderUsableProductDetailList.Sum(x => x.Count) + "عدد محصول به شماره سفارش" + "\n" + "SPLY-" + alias + "-" + order.Id + "\n" + " درتاریخ " + persianDate + "جهت تامین به دفتر مرکزی ارسال شد.", mobileNumber);
                                //NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + " تعداد :" + order.OrderUsableProductDetailList.Sum(x => x.Count) + "" + " به شماره سفارش SPLY- " + alias + "محصول به" + order.Id + " درتاریخ " + persianDate + "جهت تامین به دفتر مرکزی ارسال شد.", mobileNumber);
                            });
                        }
                        else
                        {
                            Task.Factory.StartNew(() =>
                            {
                                NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + " تعداد : " + order.OrderUsableProductDetailList.Sum(x => x.Count) + "عدد محصول به شماره سفارش" + "\n" + "SPLY-" + alias + "-" + order.Id + "\n" + " درتاریخ " + persianDate + "جهت تامین به دفتر مرکزی ارسال شد.", "09193121247");
                                //NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + "سفارش شما به شماره سفارش KIA-" + order.Id + " درتاریخ " + persianDate + " به تعداد :" + order.OrderUsableProductDetailList.Sum(x => x.Count) + " ثبت شد ", "09193121247");
                            });
                        }
                        response = new Response()
                        {
                            status = 200,
                            data = new
                            {
                                id = order.Id
                            }
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "محصولی جهت ساخت سفارش یافت نشد."
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
        /// ذخیره متن توضیحات برای هر سفارش
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SpecificationSave(SpecificatioViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var item = db.UsableProductCart.Where(x => x.UsableProductId == model.usableProductId).FirstOrDefault();
                    if (item.Count > 0)
                    {
                        if (model.specification == null)
                        {
                            response = new Response()
                            {
                                status = 500,
                                message = "همکار گرامی فیلد توضیحات خالی ثبت شد."
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                        item.Specification = model.specification;
                        item.ModifyUserId = GetAuthenticatedUserId();
                        item.ModifyDate = DateTime.Now;
                        item.Ip = Request.UserHostAddress;
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "توضیحات ثبت شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "توضیحاتی یافت نشد."
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
