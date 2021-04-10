using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class FaxController : BaseController
    {
        // GET: Fax
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
        /// دریافت لیست محصولات مصرفی
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult GetUsableProductList(UsableProductViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var query = db.UsableProduct.Where(x => x.UsableProductStatus == UsableProductStatus.Active || x.UsableProductStatus == UsableProductStatus.DisabledVisible).Select(x => x);
                    query = query.Where(x => x.PrintingHouse.Active == true);
                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.Code.ToString().Contains(model.term.Trim()) || x.Name.Contains(model.term.Trim()));
                    }
                    if (model.categoryId != null && model.categoryId > 0)
                    {
                        query = query.Where(x => x.CategoryUsableProduct.ParentId == model.categoryId);
                    }
                    if (model.categoryChildId != null && model.categoryChildId.Count > 0)
                    {
                        query = query.Where(x => model.categoryChildId.Contains(x.CategoryUsableProductId));
                    }
                    if (user.PrintingHouseId > 0 && user.PrintingHouseId != null)
                    {
                        query = query.Where(x => x.PrintingHouseId == user.PrintingHouseId);
                    }
                    query = query.OrderBy(x => x.CategoryUsableProduct.Parent.Order).ThenBy(x => x.CategoryUsableProduct.Order).ThenByDescending(x => x.CreateDate);
                    dataCount = query.Count();
                    var list = query.Select(x => new UsableProductViewModel()
                    {
                        id = x.Id,
                        name = x.Name,
                        code = x.Code,
                        description = x.Description,
                        usableProductStatus = x.UsableProductStatus,
                        unit = x.Unit,
                        count = x.UsableProductCartList.Where(y => y.BranchId == user.BranchId).Select(y => y.Count).FirstOrDefault(),
                        orderId = x.UsableProductCartList.Where(y => y.BranchId == user.BranchId).Select(y => y.Id).FirstOrDefault(),
                        firstFileName = x.UsableProductFileList.OrderBy(y => y.Id).FirstOrDefault().FileName,
                        secondFileName = x.UsableProductFileList.OrderByDescending(y => y.Id).FirstOrDefault().FileName,
                        fileList = x.UsableProductFileList.Select(y => new UsableProductSearchViewModel()
                        {
                            id = y.Id,
                            fileId = y.FileId,
                            fileName = y.FileName,
                        }).ToList(),
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.usableProductStatusTitle = Enums.GetTitle(x.usableProductStatus);
                    });
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
        /// لیست محصولات
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin , createUsableProduct,orderUsableProduct")]
        public JsonResult SearchFilter(CategoryUsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    List<CategoryUsableProductViewModel> list;
                    var query = db.CategoryUsableProduct.Where(x => x.ParentId == null).OrderBy(x => x.Order);
                    list = query.Select(item => new CategoryUsableProductViewModel()
                    {
                        id = item.Id,
                        title = item.Title,
                        parentId = item.ParentId,
                        order = item.Order,
                        active = item.Active,
                        children = item.ChildList.OrderByDescending(x => x.Order).Select(x => new SearchCategoryUsableProductSearchViewModel()
                        {
                            id = x.Id,
                            parentId = x.ParentId,
                            childTitle = x.Title,
                        }).ToList(),
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
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
        /// لیست محصولات
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin , createUsableProduct,orderUsableProduct")]
        public JsonResult ChildFilter(int? id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CategoryUsableProduct.Select(x => x);
                    if (id != null)
                    {
                        query = query.Where(x => x.ParentId == id);
                    }
                    var listChild = query.OrderByDescending(x => x.Order).Select(x => new
                    {
                        id = x.Id,
                        parentId = x.ParentId,
                        childTitle = x.Title,
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            listChild = listChild,
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
        /// نمایش تصاویر محصول
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAllImage(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var currentUser = GetAuthenticatedUser();
                    var query = db.UsableProductFile.Where(x => x.UsableProductId == id).ToList();
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        fileId = x.FileId,
                        fileName = x.FileName,
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
        /// <summary>
        /// ذخیره جزئیات سفارش
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult Save(OrderUsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();

                    var product = db.UsableProductCart.Where(x => x.UsableProductId == model.usableProductId && x.BranchId == user.BranchId).SingleOrDefault();
                    if (product != null)
                    {
                        if (product.Count > 0)
                        {
                            product.Count = model.count;
                            product.BranchId = user.BranchId.Value;
                            product.Description = model.description;
                            product.ModifyUserId = user.Id;
                            product.ModifyDate = DateTime.Now;
                            product.Ip = Request.UserHostAddress;
                        }
                        else
                        {
                            product.Count = model.count;
                            product.BranchId = user.BranchId.Value;
                            product.Description = model.description;
                            product.ModifyUserId = user.Id;
                            product.ModifyDate = DateTime.Now;
                            product.Ip = Request.UserHostAddress;
                        }

                    }
                    else
                    {
                        var item = new UsableProductCart()
                        {
                            Description = model.description,
                            Count = model.count,
                            UsableProductId = model.usableProductId,
                            BranchId = user.BranchId.Value,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.UsableProductCart.Add(item);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "تعداد سفارش ثبت شد",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ذخیره جزئیات سفارش
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult AddCount(OrderUsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();

                    var product = db.UsableProductCart.Where(x => x.UsableProductId == model.usableProductId && x.BranchId == user.BranchId).SingleOrDefault();
                    if (product != null)
                    {
                        product.Count = product.Count + 1;
                        product.BranchId = user.BranchId.Value;
                        product.ModifyUserId = user.Id;
                        product.ModifyDate = DateTime.Now;
                        product.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new UsableProductCart()
                        {
                            UsableProductId = model.usableProductId,
                            BranchId = user.BranchId.Value,
                            Count = 1,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.UsableProductCart.Add(item);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ذخیره جزئیات سفارش
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult RemoveCount(OrderUsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var product = db.UsableProductCart.Where(x => x.UsableProductId == model.usableProductId && x.BranchId == user.BranchId).SingleOrDefault();
                    if (product != null)
                    {

                        if (product.Count > 0)
                        {
                            product.Count = product.Count - 1;
                            product.ModifyUserId = user.Id;
                            product.ModifyDate = DateTime.Now;
                            product.Ip = Request.UserHostAddress;
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 500,
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        };
                    }
                    else
                    {
                        var item = new UsableProductCart()
                        {
                            UsableProductId = model.usableProductId,
                            BranchId = user.BranchId.Value,
                            Count = 1,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.UsableProductCart.Add(item);
                    }
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
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
        /// برگرداندن مقدار تعداد در modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCountModal(int id)
        {
            var user = GetAuthenticatedUser();
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    UsableProductCart item;
                    item = db.UsableProductCart.FirstOrDefault(x => x.Id == id && x.BranchId == user.BranchId);
                    if (item != null)
                    {
                        var data = new UsableProductCartViewModel
                        {
                            id = item.Id,
                            branchId = item.BranchId,
                            count = item.Count,
                            description = item.Description,

                        };
                        response = new Response()
                        {
                            status = 200,
                            data = data,
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "تعداد سفارشی ثبت نشده است.",
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
        /// دریافت همه دسته بندی سوالات
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult GetAllCategory()
        {
            Response response;
            try
            {
                List<CategoryUsableProduct> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.CategoryUsableProduct.Where(x => x.ParentId == null).OrderBy(x => x.Order).ToList();
                }
                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            title = item.Title,
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
        /// <summary>
        /// مدیریت سفارشات
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , orderUsableProduct , orderUsableProductManager , orderUsableProductManagerBranch")]
        public ActionResult Manage()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.ToList();
            }
            return View();
        }
        /// <summary>
        /// دریافت لیست محصولات چاپخانه و نمایش همه محصولات  ثبت شده در سیستم جهت 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult GetOrderUsableProductList(ShowOrderUsableProductViewModel model)
        {
            Response response;
            int dataCount;
            var user = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderUsableProduct.Select(x => x);
                    if (!User.IsInRole("admin"))
                    {
                        query = query.Where(x => x.BranchId == user.BranchId);
                    }
                    dataCount = query.Count();
                    query = query.OrderBy(x => x.CreateDate).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(item => new ShowOrderUsableProductViewModel
                    {
                        id = item.Id,
                        productCount = item.OrderUsableProductDetailList.Count,
                        sumCount = item.OrderUsableProductDetailList.Sum(x => x.Count),
                        registered = item.OrderUsableProductDetailList.Where(x => x.OrderUsableProduct.OrderUsableProductStatus == OrderUsableProductStatus.Registered).Sum(x => x.Count),
                        inPreparation = item.OrderUsableProductDetailList.Where(x => x.OrderUsableProduct.OrderUsableProductStatus == OrderUsableProductStatus.InPreparation).Sum(x => x.Count),
                        sent = item.OrderUsableProductDetailList.Where(x => x.OrderUsableProduct.OrderUsableProductStatus == OrderUsableProductStatus.Sent).Sum(x => x.Count),
                        //usableProductId=item.UsableProductId,
                        createUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        createBranch = item.Branch.Name,
                        alias = item.Branch.Alias,
                        createDate = item.CreateDate,
                        orderUsableProductStatus = item.OrderUsableProductStatus,
                        branchType = item.Branch.BranchType,
                    }).ToList();

                    list.ForEach(x => GetBackgroundColor(x));

                    list.ForEach(x =>
                    {
                        x.orderUsableProductStatusTitle = Enums.GetTitle(x.orderUsableProductStatus);
                        x.persianDate = DateUtility.GetPersianDate(x.createDate);
                    });

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


        /// <summary>
        /// قرار دادن رنگ زمینه مناسب برای هر سفارش که بسته به وضعیت محصولات داخل سفارش تعیین میشود
        /// </summary>
        /// <param name="x">سفارش مورد نظر</param>
        private void GetBackgroundColor(ShowOrderUsableProductViewModel x)
        {
            if (x.registered == x.sumCount)
            {
                x.bgColor = "bg-new-order";
            }
            else if (x.sent == x.sumCount)
            {
                x.bgColor = "bg-done-order";
            }
            else
            {
                x.bgColor = "bg-open-order";
            }
        }

        /// <summary>
        /// مشاهده جزئیات سفارش های محصول چاپخانه
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , orderUsableProduct")]
        public ActionResult Manipulate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// نمایش جزئیات سفارش
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin , orderUsableProduct")]
        public JsonResult ManipulateAll(ShowOrderUsableProductViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var query = db.OrderUsableProductDetail.Where(x => x.OrderUsableProductId == model.orderUsableProductId);
                    var list = query.Select(item => new ShowOrderUsableProductViewModel
                    {
                        id = item.Id,
                        name = item.UsableProduct.Name,
                        orderUsableProductId = item.OrderUsableProductId,
                        printingHouseOrderId = item.UsableProduct.PrintingHouseId,
                        productCount = item.Count,
                        printingHouserOrder = item.OrderUsableProduct.PrintingHouserOrder,
                        orderUsableProductStatus = item.OrderUsableProduct.OrderUsableProductStatus,
                        image = item.UsableProduct.UsableProductFileList.Select(y => y.FileName).FirstOrDefault(),
                        category = item.UsableProduct.CategoryUsableProduct.Title,
                        description = item.Description,
                        specification = item.Specification,
                        createUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        createBranch = item.OrderUsableProduct.Branch.Name,
                        createDate = item.CreateDate
                    }).ToList();
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
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// نمایش جزئیات سفارش برای هرشعبه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult OrderDetail(int id)
        {
            ViewBag.OrderUsableProductId = id;
            return View();
        }

        /// <summary>
        /// جزئیات 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetOrderDetail(ShowOrderUsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var query = db.OrderUsableProductDetail.Where(x => x.OrderUsableProductId == model.orderUsableProductId);
                    var list = query.Select(item => new ShowOrderUsableProductViewModel
                    {
                        id = item.Id,
                        name = item.UsableProduct.Name,
                        orderUsableProductId = item.OrderUsableProductId,
                        productCount = item.Count,
                        unit = item.UsableProduct.Unit,
                        code = item.UsableProduct.Code,
                        image = item.UsableProduct.UsableProductFileList.Select(y => y.FileName).FirstOrDefault(),
                        category = item.UsableProduct.CategoryUsableProduct.Title,
                        createUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        createBranch = item.OrderUsableProduct.Branch.Name,
                        createDate = item.CreateDate,

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.orderUsableProductStatusTitle = Enums.GetTitle(x.orderUsableProductStatus);
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
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ارسال سفارش محصول مصرفی به سمت چاپخانه
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendToPrintingHouse(PrintingHouseOrderViewModel model)
        {
            Response response;
            try
            {
                var userId = GetAuthenticatedUserId();
                OrderUsableProduct order = new OrderUsableProduct();
                using (var db = new KiaGalleryContext())
                {
                    var orderUsableProductDetail = db.OrderUsableProductDetail.Where(x => model.idList.Contains(x.Id)).ToList();
                    if (orderUsableProductDetail.Select(x => x.OrderUsableProduct.PrintingHouserOrder).FirstOrDefault() == true)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "کاربر محترم؛این سفارش یک بار به چاپخانه مورد نظر ارسال شده است."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    for (var i = 0; i < orderUsableProductDetail.Count; i++)
                    {
                        orderUsableProductDetail[i].Id = model.idList[i];
                        orderUsableProductDetail[i].OfficeInventory = model.officeInventory[i];
                        orderUsableProductDetail[i].Remain = model.remain[i];
                        if (orderUsableProductDetail[i].OfficeInventory != null && orderUsableProductDetail[i].OfficeInventory >= 0)
                        {
                            if (orderUsableProductDetail[i].OfficeInventory > orderUsableProductDetail[i].Count)
                            {
                                response = new Response()
                                {
                                    status = 500,
                                    message = "کاربر محترم؛تعداد یکی از موجودی دفتر مرکزی بزرگتر از تعداد سفارش می باشد."
                                };
                                return Json(response, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                if (orderUsableProductDetail[i].Remain != 0)
                                {
                                    var query = orderUsableProductDetail.Where(x => x.OrderUsableProductId == model.orderId).ToList();
                                    if (query != null && query.Count > 0)
                                    {
                                        foreach (var item in orderUsableProductDetail)
                                        {
                                            item.OrderUsableProduct.PrintingHouserOrder = true;
                                            item.OrderUsableProduct.OrderUsableProductStatus = OrderUsableProductStatus.Registered;
                                            item.CreateUserId = userId;
                                            item.ModifyUserId = userId;
                                            item.CreateDate = DateTime.Now;
                                            item.ModifyDate = DateTime.Now;
                                            item.Ip = Request.UserHostAddress;
                                        }
                                    }
                                }
                                if (orderUsableProductDetail.Count(x => x.Remain > 0) == 0)
                                {
                                    var query = orderUsableProductDetail.Where(x => x.OrderUsableProductId == model.orderId).ToList();
                                    if (query != null && query.Count > 0)
                                    {
                                        foreach (var item in orderUsableProductDetail)
                                        {
                                            item.OrderUsableProduct.PrintingHouserOrder = false;
                                            item.CreateUserId = userId;
                                            item.ModifyUserId = userId;
                                            item.CreateDate = DateTime.Now;
                                            item.ModifyDate = DateTime.Now;
                                            item.Ip = Request.UserHostAddress;
                                        }
                                    }
                                }

                            }

                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 500,
                                message = "کاربر محترم؛تعداد یکی از موجودی دفتر مرکزی ثبت نشده است."
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                    }
                    var date = orderUsableProductDetail.Select(x => x.CreateDate).FirstOrDefault();
                    var orderCreateUserId = orderUsableProductDetail.Select(x => x.CreateUserId).FirstOrDefault();
                    var orderBranchID = db.OrderUsableProduct.Where(x => x.Id == model.orderId).Select(x => x.BranchId).FirstOrDefault();
                    var persianDate = DateUtility.GetPersianDate(date);
                    var orderId = orderUsableProductDetail.Select(x => x.OrderUsableProductId).FirstOrDefault();
                    //var mobileNumber = db.User.Where(x => x.Id == orderCreateUserId).FirstOrDefault().PhoneNumber;
                    var mobileNumber = db.User.Where(x => x.BranchId == orderBranchID).FirstOrDefault().PhoneNumber;
                    var branchId = orderUsableProductDetail.Select(x => x.OrderUsableProduct.Branch).FirstOrDefault();
                    var mobileAdmin = db.User.Where(x => x.Id == 2).FirstOrDefault().PhoneNumber;
                    var alias = branchId.Alias;

                    //if (mobileNumber != null)
                    //{
                    //    if (!User.IsInRole("admin"))
                    //    {
                    //        Task.Factory.StartNew(() =>
                    //        {
                    //            NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + "سفارش شما به شماره سفارش" + "\n" + "SPLY-" + alias + "-" + orderId + "\n" + " درتاریخ " + persianDate + "تایید و در حال بررسی می باشد.", mobileNumber);
                    //        });
                    //    }
                    //    else
                    //    {
                    //        Task.Factory.StartNew(() =>
                    //        {
                    //            NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + "سفارش شما به شماره سفارش" + "\n" + "SPLY-" + alias + "-" + orderId + "\n" + " درتاریخ " + persianDate + "تایید و در حال بررسی می باشد.", mobileAdmin);
                    //        });
                    //    }
                    //}
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "سفارش برای چاپخانه ارسال گردید."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// نمایش تعداد سفارش روی آیکون سبد خرید
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult GetCartCount()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var currentUser = GetAuthenticatedUser();
                    var query = db.UsableProductCart.Where(x => x.Branch.Id == currentUser.BranchId);
                    var count = query.Sum(x => x.Count);
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            count = count,
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
        /// تغییر وضعیت گیفت
        /// </summary>
        /// <param name="model">مدلی شامل ردیف های گیفت های می باشد</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, gift, giftRegistration, solicitorshipGift")]
        public JsonResult ChangeStatus(OrderUsableChangeStatusViewModel model)
        {
            Response response;
            try
            {
                int userid = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    var orderUsableProductList = db.OrderUsableProduct.Where(x => model.id.Any(y => y == x.Id)).ToList();
                    foreach (OrderUsableProduct order in orderUsableProductList)
                    {
                        order.OrderUsableProductStatus = model.status;

                        var log = new OrderUsableProductLog()
                        {
                            OrderUsableProductId = order.Id,
                            OrderUsableProductStatus = model.status,
                            CreateUserId = userid,
                            CreateDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.OrderUsableProductLog.Add(log);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = " وضعیت محصول سفارشی به " + Enums.GetTitle(model.status) + " تغییر یافت. "
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// حذف سفارش ثبت شده از سمت مرکزی
        /// </summary>
        /// <returns></returns>
        public JsonResult Delete(int Id)
        {
            {
                Response response;
                try
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var item = db.OrderUsableProduct.Find(Id);
                        if (item.PrintingHouserOrder == true)
                        {
                            response = new Response()
                            {
                                status = 200,
                                message = "این سفارش به چاپخانه ارسال شده است و قابل حذف نمی باشد."
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                        if (item.OrderUsableProductDetailList != null)
                        {
                            response = new Response()
                            {
                                status = 200,
                                message = "این سفارش حاوی اطلاعات سفارش می باشد و قابل حذف نمی باشد."
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                        response = new Response()
                        {
                            status = 200,
                            message = "سفارش با موفقیت حذف شد."
                        };
                        db.OrderUsableProduct.Remove(item);
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
}