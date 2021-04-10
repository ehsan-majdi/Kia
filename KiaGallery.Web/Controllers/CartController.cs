using KiaGallery.Common;
using KiaGallery.Model;
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
    /// کنترلر سبد سفارش
    /// </summary>
    public class CartController : BaseController
    {
        /// <summary>
        /// مدیریت سبد سفارش
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public ActionResult Index()
        {
            ViewBag.CartType = 0; // سفارشات برای ویترین
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.OrderBy(x => x.Order).Where(x => x.Active == true).ToList();
            }
            return View();
        }

        /// <summary>
        /// مدیریت سبد سفارش
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public ActionResult Customer()
        {
            ViewBag.CartType = 1; // سفارشات برای مشتری
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.OrderBy(x => x.Order).Where(x => x.Active == true).ToList();
            }
            return View("Index");
        }

        /// <summary>
        /// لیست محصولات درون سبد خرید
        /// </summary>
        /// <param name="model">لیست پارامترهای جستجو</param>
        /// <returns>محصولات درون سبد خرید</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult CartList(CartSearchViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Cart.Where(x => string.IsNullOrEmpty(x.Customer) && x.BranchId == user.BranchId);

                    if (model.workshopIdList != null && model.workshopIdList.Count() > 0)
                    {
                        query = query.Where(x => model.workshopIdList.Contains(x.Product.WorkshopId));
                    }
                    if (model.workshopId != null && model.workshopId > 0)
                    {
                        query = query.Where(x => x.Product.WorkshopId == model.workshopId);
                    }
                    if (model.productType != null)
                    {
                        query = query.Where(x => x.Product.ProductType == model.productType);
                    }

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.ProductId);


                    List<CartListViewModel> data = query.Select(item => new CartListViewModel
                    {
                        id = item.Id,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        orderType = item.OrderType,
                        //orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        size = item.Size,
                        goldType = item.GoldType,
                        //goldTypeTitle = Enums.GetTitle(item.GoldType),
                        setNumber = item.SetNumber,
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        branchLabel = item.BranchLabel,
                        description = item.Description,
                        title = item.Product.Title,
                        //outerWerkTypeTitle = Enums.GetTitle(item.OuterWerkType),
                        code = item.Product.Code,
                        bookCode = item.Product.BookCode,
                        weight = item.Product.Weight,
                        count = item.Count,
                        createdUser = item.CreateUser.FullName,
                        stoneList = item.CartProductStoneList.Select(x => new CartStoneListViewModel()
                        {
                            order = x.Order,
                            stoneName = x.Stone.Name
                        }).ToList(),
                        leatherList = item.CartProductLeatherList.Select(x => new CartLeatherListViewModel()
                        {
                            order = x.Order,
                            leatherName = x.Leather.Name
                        }).ToList()
                    }).ToList();

                    data.ForEach(x =>
                    {
                        x.goldTypeTitle = Enums.GetTitle(x.goldType);
                        x.orderTypeTitle = Enums.GetTitle(x.orderType);
                        x.outerWerkTypeTitle = Enums.GetTitle(x.outerWerkType);
                    });

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
                            count = dataCount,
                            weight = data.Sum(x => x.weight)
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
        /// لیست محصولات درون سبد خرید به صورت تجمیعی
        /// </summary>
        /// <param name="model">لیست پارامترهای جستجو</param>
        /// <returns>محصولات درون سبد خرید</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult CartListAggregation(CartSearchViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Cart.Where(x => x.BranchId == user.BranchId);

                    if (model.cartType != null && model.cartType > 0)
                    {
                        switch (model.cartType)
                        {
                            case 1:
                                query = query.Where(x => string.IsNullOrEmpty(x.Customer));
                                break;
                            case 2:
                                query = query.Where(x => !string.IsNullOrEmpty(x.Customer));
                                break;
                        }
                    }

                    if (model.workshopId != null && model.workshopId > 0)
                    {
                        query = query.Where(x => x.Product.WorkshopId == model.workshopId);
                    }
                    if (model.workshopIdList != null && model.workshopIdList.Count() > 0)
                    {
                        query = query.Where(x => model.workshopIdList.Contains(x.Product.WorkshopId));
                    }

                    if (model.productType != null)
                    {
                        query = query.Where(x => x.Product.ProductType == model.productType);
                    }

                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.ProductId)/*.ThenByDescending(x=> x.SetNumber)*/;

                    //list = query.ToList();

                    List<CartListViewModel> list = query.Select(item => new CartListViewModel()
                    {
                        id = item.Id,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        orderType = item.OrderType,
                        //orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        workshopName = item.Product.Workshop.Name,
                        workshopName2 = item.Product.Workshop2.Name,
                        productSizeId = item.Product.SizeId,
                        workshopId = item.Product.WorkshopId,
                        size = item.Size,
                        size2 = item.Size2,
                        setNumber = item.SetNumber,
                        goldType = item.GoldType,
                        outerWerkType = item.OuterWerkType,
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        branchLabel = item.BranchLabel,
                        description = item.Description,
                        title = item.Product.Title,
                        code = item.Product.Code,
                        bookCode = item.Product.BookCode,
                        weight = item.Product.Weight,
                        count = item.Count,
                        createdUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        createDate = item.CreateDate,
                        //joinedStoneList = string.Join("-", item.CartProductStoneList?.Select(x => x.Order + ":" + x.Stone?.Name)),
                        //joinedLeatherList = string.Join("-", item.CartProductLeatherList?.Select(x => x.Order + ":" + x.Leather?.Name)),
                        stoneList = item.CartProductStoneList.Select(x => new CartStoneListViewModel()
                        {
                            order = x.Order,
                            stoneName = x.Stone.Name
                        }).ToList(),
                        leatherList = item.CartProductLeatherList.Select(x => new CartLeatherListViewModel()
                        {
                            order = x.Order,
                            leatherName = x.Leather.Name
                        }).ToList(),
                        
                    }).ToList();

                    list.ForEach(x =>
                    {
                        x.orderTypeTitle = Enums.GetTitle(x.orderType);
                        x.joinedStoneList = string.Join("-", x.stoneList.Select(y => y.order + ":" + y.stoneName));
                        x.joinedLeatherList = string.Join("-", x.leatherList.Select(y => y.order + ":" + y.leatherName));
                        x.outerWerkTypeTitle = Enums.GetTitle(x.outerWerkType);
                    });

                    var data = list.GroupBy(x => new
                    {
                        x.fileName,
                        x.orderType,
                        x.productId,
                        x.workshopName,
                        x.workshopName2,
                        x.productSizeId,
                        x.title,
                        x.bookCode,
                        x.code,
                        x.weight,
                        x.size,
                        x.size2,
                        x.setNumber,
                        x.goldType,
                        x.outerWerkType,
                        x.leatherLoop,
                        x.customer,
                        x.phoneNumber,
                        x.forceOrder,
                        x.branchLabel,
                        x.description,
                        x.joinedStoneList,
                        x.joinedLeatherList
                    }).Select(x => new
                    {
                        id = string.Join("-", x.Select(y => y.id).ToList()),
                        fileName = x.Key.fileName,
                        orderType = x.Key.orderType,
                        orderTypeTitle = x.First().orderTypeTitle,
                        productId = x.Key.productId,
                        workshopName = x.Key.workshopName,
                        workshopName2 = x.Key.workshopName2,
                        productSizeId = x.Key.productSizeId,
                        title = x.Key.title,
                        code = x.Key.code,
                        bookCode = x.Key.bookCode,
                        weight = x.Key.weight,
                        size = x.Key.size,
                        size2 = x.Key.size2,
                        setNumber = x.Key.setNumber,
                        goldType = x.Key.goldType,
                        goldTypeTitle = Enums.GetTitle(x.Key.goldType),
                        outerWerkTypeTitle = Enums.GetTitle(x.Key.outerWerkType),
                        leatherLoop = x.Key.leatherLoop,
                        customer = x.Key.customer,
                        phoneNumber = x.Key.phoneNumber,
                        forceOrder = x.Key.forceOrder,
                        branchLabel = x.Key.branchLabel,
                        description = x.Key.description,
                        count = x.Sum(y => y.count),
                        createdUser = x.Count() > 1 ? "-" : x.First().createdUser,
                        createUserList = x.GroupBy(y => y.createdUser).Select(y => new { fullName = y.Key, count = y.Count() }).ToList(),
                        createDate = x.Count() > 1 ? "-" : DateUtility.GetPersianDate(x.First().createDate),
                        stoneList = x.First().stoneList,
                        leatherList = x.First().leatherList
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
                            count = dataCount,
                            weight = (decimal)list.Sum(x => x.weight),
                            workshopWeight = list.GroupBy(x => x.workshopId).Select(x => new { workshopId = x.Key, weight = (decimal)x.Sum(y => (decimal)y.weight * y.count) }).ToList()
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
        /// لیست محصولات سفارش داده شده برای مشتری درون سبد خرید
        /// </summary>
        /// <param name="model">لیست پارامترهای جستجو</param>
        /// <returns>محصولات مشتری درون سبد خرید</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult CartCustomerList(CartSearchViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Cart.Where(x => !string.IsNullOrEmpty(x.Customer) && x.BranchId == user.BranchId);

                    if (model.workshopId != null && model.workshopId > 0)
                    {
                        query = query.Where(x => x.Product.WorkshopId == model.workshopId);
                    }

                    if (model.productType != null)
                    {
                        query = query.Where(x => x.Product.ProductType == model.productType);
                    }

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.ProductId);


                    List<CartListViewModel> data = query.Select(item => new CartListViewModel()
                    {
                        id = item.Id,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        orderType = item.OrderType,
                        orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        size = item.Size,
                        setNumber = item.SetNumber,
                        goldType = item.GoldType,
                        goldTypeTitle = Enums.GetTitle(item.GoldType),
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        branchLabel = item.BranchLabel,
                        description = item.Description,
                        title = item.Product.Title,
                        code = item.Product.Code,
                        bookCode = item.Product.BookCode,
                        weight = item.Product.Weight,
                        count = item.Count,
                        createdUser = item.CreateUser.FullName,
                        stoneList = item.CartProductStoneList.Select(x => new CartStoneListViewModel()
                        {
                            order = x.Order,
                            stoneName = x.Stone.Name
                        }).ToList(),
                        leatherList = item.CartProductLeatherList.Select(x => new CartLeatherListViewModel()
                        {
                            order = x.Order,
                            leatherName = x.Leather.Name
                        }).ToList()
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
                            count = dataCount,
                            weight = data.Sum(x => x.weight)
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
        /// اضافه کردن به سبد خرید
        /// </summary>
        /// <param name="productId">ردیف محصول</param>
        /// <param name="count">تعداد</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult AddToCart(int productId, int count)
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

                        var item = new Cart()
                        {
                            BranchId = branchId.GetValueOrDefault(),
                            ProductColor = ProductColor.Gold,
                            ProductId = productId,
                            OrderType = OrderType.None,
                            Count = 1,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.Cart.Add(item);
                    }

                    //var entity = db.Cart.SingleOrDefault(x => x.BranchId == branchId && x.OrderType == OrderType.None && x.ProductId == productId);
                    //if (entity != null)
                    //{
                    //    entity.Count = entity.Count + count;
                    //    entity.ModifyUserId = userId;
                    //    entity.ModifyDate = DateTime.Now;
                    //    entity.Ip = Request.UserHostAddress;
                    //}
                    //else
                    //{
                    //    var item = new Cart()
                    //    {
                    //        BranchId = branchId.GetValueOrDefault(),
                    //        ProductId = productId,
                    //        OrderType = OrderType.None,
                    //        Count = count,
                    //        CreateUserId = userId,
                    //        ModifyUserId = userId,
                    //        CreateDate = DateTime.Now,
                    //        ModifyDate = DateTime.Now,
                    //        Ip = Request.UserHostAddress
                    //    };
                    //    db.Cart.Add(item);
                    //}

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
        [Authorize(Roles = "admin, order, orderOuterWerk")]
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
                        var entityList = db.Cart.Where(x => x.BranchId == branchId && idList.Any(y => y == x.Id)).ToList();
                        if (entityList != null && entityList.Count > 0)
                        {
                            entityList.ForEach(entity =>
                            {
                                db.CartProductStone.RemoveRange(entity.CartProductStoneList);
                                db.CartProductLeather.RemoveRange(entity.CartProductLeatherList);
                                db.Cart.Remove(entity);
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
        /// حذف کردن محصول به تعداد از سبد خرید
        /// </summary>
        /// <param name="productId">ردیف محصول</param>
        /// <param name="count">تعداد</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult RemoveCount(int productId, int count)
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

                    if (count > 0)
                    {
                        var cartList = db.Cart.Where(x => x.BranchId == branchId && x.ProductId == productId && x.OrderType == OrderType.None).OrderBy(x => x.Id).ToList();
                        if (cartList.Count >= count)
                        {
                            var removedItem = cartList.Take(count);

                            db.Cart.RemoveRange(removedItem);
                            db.SaveChanges();
                            message = "محصول با موفیقت از سبد خرید کسر شد.";
                        }
                        else
                        {
                            status = 500;
                            message = "در حال حاضر سفارش سریع در این محصول وجود ندارد.";
                        }
                    }

                    //var entity = db.Cart.SingleOrDefault(x => x.BranchId == branchId && x.OrderType == OrderType.None && x.ProductId == productId);
                    //if (entity != null)
                    //{
                    //    if (entity.Count - count > 0)
                    //    {
                    //        entity.Count = entity.Count - count;
                    //        entity.ModifyUserId = user.Id;
                    //        entity.ModifyDate = DateTime.Now;
                    //        entity.Ip = Request.UserHostAddress;
                    //    }
                    //    else
                    //    {
                    //        db.Cart.Remove(entity);
                    //    }

                    //    db.SaveChanges();
                    //    message = "محصول با موفیقت از سبد خرید کسر شد.";
                    //}
                    //else
                    //{
                    //    status = 500;
                    //    message = "محصول مورد نظر یافت نشد.";
                    //}
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
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult Increment(string id)
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

                    var entity = db.Cart.First(x => x.BranchId == branchId && idList.Any(y => y == x.Id));
                    if (entity.OrderType == OrderType.Personalization)
                    {
                        var item = new Cart()
                        {
                            BranchId = branchId,
                            OrderType = OrderType.Personalization,
                            ProductId = entity.ProductId,
                            Count = 1,
                            Size = entity.Size,
                            GoldType = entity.GoldType,
                            LeatherLoop = entity.LeatherLoop,
                            Customer = entity.Customer,
                            PhoneNumber = entity.PhoneNumber,
                            ForceOrder = entity.ForceOrder,
                            BranchLabel = entity.BranchLabel,
                            Description = entity.Description,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        item.CartProductStoneList = entity.CartProductStoneList?.Select(x => new CartProductStone()
                        {
                            Cart = item,
                            Order = x.Order,
                            StoneId = x.StoneId
                        }).ToList();

                        item.CartProductLeatherList = entity.CartProductLeatherList?.Select(x => new CartProductLeather()
                        {
                            Cart = item,
                            Order = x.Order,
                            LeatherId = x.LeatherId
                        }).ToList();

                        db.Cart.Add(item);
                    }
                    else
                    {
                        var item = new Cart()
                        {
                            BranchId = branchId,
                            OrderType = OrderType.None,
                            ProductId = entity.ProductId,
                            Count = 1,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        db.Cart.Add(item);
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
        /// کاهش تعداد بر اساس یک سفارش
        /// </summary>
        /// <param name="id">لیست ردیف محصول که می بایست یکی از موارد آن حذف شود</param>
        /// <returns>جیسون حاوی نتیجه کاهش تعداد</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
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

                    var entity = db.Cart.OrderByDescending(x => x.Id).First(x => x.BranchId == user.BranchId && idList.Any(y => y == x.Id));
                    if (entity.CartProductStoneList.Count > 0)
                        db.CartProductStone.RemoveRange(entity.CartProductStoneList);
                    if (entity.CartProductLeatherList.Count > 0)
                        db.CartProductLeather.RemoveRange(entity.CartProductLeatherList);
                    db.Cart.Remove(entity);

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
        /// اضافه کردن محصول شخصی سازی شده به سبد سفارش
        /// </summary>
        /// <param name="models">شی حاوی اطلاعات شخصی سازی شده</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult AddToCartPersonalization(List<CartViewModel> models)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                var user = GetAuthenticatedUser();
                var branchId = user.BranchId.GetValueOrDefault();
                using (var db = new KiaGalleryContext())
                {

                    foreach (var model in models)
                    {

                        if (model.id != null && model.id > 0)
                        {

                            Cart entity = db.Cart.Single(x => x.Id == model.id && x.BranchId == branchId);

                            entity.ProductId = model.productId;
                            entity.Count = 1;
                            entity.Size = model.size;
                            entity.Size2 = model.size2;
                            entity.GoldType = model.goldType >= 0 ? model.goldType : null;
                            entity.ProductColor = model.productColor >= 0 ? model.productColor : null;
                            entity.OuterWerkType = model.outerWerkType >= 0 ? model.outerWerkType : null;
                            entity.LeatherLoop = model.leatherLoop;
                            entity.Customer = model.customer;
                            entity.PhoneNumber = model.phoneNumber;
                            entity.ForceOrder = model.forceOrder;
                            entity.BranchLabel = model.branchLabel;
                            entity.Description = model.description;
                            entity.ModifyUserId = user.Id;
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            if (model.stoneList != null && model.stoneList.Count > 0)
                            {
                                model.stoneList.ForEach(item =>
                                {
                                    var stoneItem = entity.CartProductStoneList.FirstOrDefault(x => x.Order == item.order);
                                    if (stoneItem != null)
                                    {
                                        stoneItem.StoneId = item.stoneId;
                                    }
                                    else
                                    {
                                        entity.CartProductStoneList.Add(new CartProductStone()
                                        {
                                            Cart = entity,
                                            Order = item.order,
                                            StoneId = item.stoneId
                                        });
                                    }
                                });
                            }

                            if (model.leatherList != null && model.leatherList.Count > 0)
                            {
                                model.leatherList.ForEach(item =>
                                {
                                    var leatherItem = entity.CartProductLeatherList.FirstOrDefault(x => x.Order == item.order);
                                    if (leatherItem != null)
                                    {
                                        leatherItem.LeatherId = item.leatherId;
                                    }
                                    else
                                    {
                                        entity.CartProductLeatherList.Add(new CartProductLeather()
                                        {
                                            Cart = entity,
                                            Order = item.order,
                                            LeatherId = item.leatherId
                                        });
                                    }
                                });
                            }

                            if (model.count > 1) // اگر عدد تعداد تغییر کرده بود، به ازای هر کدام مجدد رکورد ثبت می شود.
                            {
                                for (int i = 0; i < model.count - 1; i++)
                                {
                                    var item = new Cart()
                                    {
                                        BranchId = user.BranchId.GetValueOrDefault(),
                                        OrderType = OrderType.Personalization,
                                        ProductId = model.productId,
                                        Count = 1,
                                        Size = model.size,
                                        Size2 = model.size2,
                                        GoldType = model.goldType >= 0 ? model.goldType : null,
                                        ProductColor = model.productColor >= 0 ? model.productColor : null,
                                        OuterWerkType = model.outerWerkType >= 0 ? model.outerWerkType : null,
                                        LeatherLoop = model.leatherLoop,
                                        Customer = model.customer,
                                        PhoneNumber = model.phoneNumber,
                                        ForceOrder = model.forceOrder,
                                        DeliverDateRequest = model.deliverDateRequest,
                                        BranchLabel = model.branchLabel,
                                        Description = model.description,
                                        CreateUserId = user.Id,
                                        ModifyUserId = user.Id,
                                        CreateDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        Ip = Request.UserHostAddress
                                    };

                                    item.CartProductStoneList = model.stoneList?.Select(x => new CartProductStone()
                                    {
                                        Cart = item,
                                        Order = x.order,
                                        StoneId = x.stoneId
                                    }).ToList();

                                    item.CartProductLeatherList = model.leatherList?.Select(x => new CartProductLeather()
                                    {
                                        Cart = item,
                                        Order = x.order,
                                        LeatherId = x.leatherId
                                    }).ToList();

                                    db.Cart.Add(item);
                                }
                            }
                        }
                        else
                        {
                            var lastCartSetNumber = db.Cart.OrderByDescending(x => x.Id).FirstOrDefault(x => x.SetNumber != null && x.BranchId == branchId);
                            int lastSetNumber = 0;
                            if (lastCartSetNumber != null)
                            {
                                lastSetNumber = lastCartSetNumber.SetNumber.GetValueOrDefault();
                            }
                            for (int i = 0; i < model.count; i++)
                            {
                                if (model.setNumber != null)
                                {
                                    model.setNumber = model.setNumber + lastSetNumber;
                                }
                                var item = new Cart()
                                {
                                    BranchId = user.BranchId.GetValueOrDefault(),
                                    OrderType = OrderType.Personalization,
                                    ProductId = model.productId,
                                    Count = 1,
                                    SetNumber = model.setNumber,
                                    Size = model.size,
                                    Size2 = model.size2,
                                    GoldType = model.goldType >= 0 ? model.goldType : null,
                                    ProductColor = model.productColor >= 0 ? model.productColor : null,
                                    OuterWerkType = model.outerWerkType >= 0 ? model.outerWerkType : null,
                                    LeatherLoop = model.leatherLoop,
                                    Customer = model.customer,
                                    PhoneNumber = model.phoneNumber,
                                    ForceOrder = model.forceOrder,
                                    DeliverDateRequest = model.deliverDateRequest,
                                    BranchLabel = model.branchLabel,
                                    Description = model.description,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };

                                item.CartProductStoneList = model.stoneList?.Select(x => new CartProductStone()
                                {
                                    Cart = item,
                                    Order = x.order,
                                    StoneId = x.stoneId
                                }).ToList();

                                item.CartProductLeatherList = model.leatherList?.Select(x => new CartProductLeather()
                                {
                                    Cart = item,
                                    Order = x.order,
                                    LeatherId = x.leatherId
                                }).ToList();

                                db.Cart.Add(item);
                            }
                        }
                    }

                    db.SaveChanges();
                    message = "محصول با موفقیت به سبد سفارش اضافه شد.";
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
        /// خلاصه ای از وضعیت سبد سفارش
        /// </summary>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult CartSummary()
        {
            Response response;
            try
            {
                List<Cart> list;
                var user = GetAuthenticatedUser();
                int dataCount;
                object branch;
                float? inPreprationDebt;
                Branch branchData;
                List<int> orderNumberList;
                using (var db = new KiaGalleryContext())
                {
                    branchData = db.Branch.Single(x => x.Id == user.BranchId);
                    double dateSetting = double.Parse(db.Settings.Where(x => x.Key == Settings.KeyInPreprationGoldDebtDateRange).SingleOrDefault()?.Value);
                    dateSetting = dateSetting * -1;
                    var dateRange = DateTime.Today.AddDays(dateSetting);
                    orderNumberList = db.OrderDetail
                        .Where(x => x.CreateDate >= DbFunctions.TruncateTime(dateRange) && x.Order.BranchId == user.BranchId && x.Order.Branch.BranchType == BranchType.Solicitorship && x.OrderDetailStatus != OrderDetailStatus.Cancel && x.OrderDetailStatus != OrderDetailStatus.Sent)
                        .GroupBy(x => x.Order).Select(x => x.Key.Id).ToList();
                    inPreprationDebt = db.OrderDetail
                        .Where(x => x.CreateDate >= DbFunctions.TruncateTime(dateRange) && x.Order.BranchId == user.BranchId && x.Order.Branch.BranchType == BranchType.Solicitorship && x.OrderDetailStatus != OrderDetailStatus.Cancel && x.OrderDetailStatus != OrderDetailStatus.Sent)
                        .Sum(x => x.Product.Weight * x.Count);


                    var query = db.Cart.Include(x => x.Product).Where(x => x.BranchId == user.BranchId);
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id);

                    list = query.ToList();
                    branch = db.Branch.Where(x => x.Id == user.BranchId).Select(x => new { x.GoldCredit, x.GoldDebt }).FirstOrDefault();
                }
                var debtSumWeight = inPreprationDebt + branchData.GoldDebt;
                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.GroupBy(x => x.ProductId).Select(item => new
                        {
                            productId = item.First().ProductId,
                            count = item.Sum(x => x.Count)
                        }),
                        count = dataCount,
                        weight = list.Sum(x => x.Product.Weight * x.Count),
                        branchStatus = branch,
                        inPreprationDebt = inPreprationDebt != null && inPreprationDebt != 0 ? Math.Round(double.Parse(inPreprationDebt.ToString()), 2) : 0,
                        debtSumWeight = debtSumWeight != null && debtSumWeight != 0 ? Math.Round(double.Parse(debtSumWeight.ToString()), 2) : 0,
                        orderNumberList = orderNumberList

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
        /// خواندن اطلاعات ثبت شده برای محصول شخصی شده در سبد سفارش کاربر
        /// </summary>
        /// <param name="id">ردیف محصول در سبد سفارش</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Cart.Include(x => x.CartProductStoneList).Include(x => x.CartProductLeatherList).Single(x => x.BranchId == user.BranchId && x.Id == id);
                    response = new Response()
                    {
                        status = 200,
                        data = new CartViewModel
                        {
                            id = entity.Id,
                            orderType = entity.OrderType,
                            productId = entity.ProductId,
                            count = entity.Count,
                            size = entity.Size,
                            size2 = entity.Size2,
                            goldType = entity.GoldType,
                            outerWerkType = entity.OuterWerkType,
                            leatherLoop = entity.LeatherLoop,
                            customer = entity.Customer,
                            phoneNumber = entity.PhoneNumber,
                            forceOrder = entity.ForceOrder,
                            branchLabel = entity.BranchLabel,
                            description = entity.Description,
                            stoneList = entity.CartProductStoneList.Select(x => new CartStoneViewModel()
                            {
                                order = x.Order,
                                stoneId = x.StoneId
                            }).ToList(),
                            leatherList = entity.CartProductLeatherList.Select(x => new CartLeatherViewModel()
                            {
                                order = x.Order,
                                leatherId = x.LeatherId
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
        /// خواندن اطلاعات تعدادی از محصولات سبد شده در سبد خرید
        /// </summary>
        /// <param name="id">رشته ساخته شده با - برای هر ردیف</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult LoadList(string id)
        {
            Response response;
            try
            {
                var idList = id.Split('-').Select(x => int.Parse(x));
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var list = db.Cart.Where(x => x.BranchId == user.BranchId && idList.Any(y => y == x.Id)).Select(entity => new CartViewModel
                    {
                        id = entity.Id,
                        orderType = entity.OrderType,
                        productId = entity.ProductId,
                        count = entity.Count,
                        size = entity.Size,
                        size2 = entity.Size2,
                        goldType = entity.GoldType,
                        outerWerkType = entity.OuterWerkType,
                        leatherLoop = entity.LeatherLoop,
                        customer = entity.Customer,
                        phoneNumber = entity.PhoneNumber,
                        forceOrder = entity.ForceOrder,
                        branchLabel = entity.BranchLabel,
                        description = entity.Description,
                        stoneList = entity.CartProductStoneList.Select(x => new CartStoneViewModel()
                        {
                            order = x.Order,
                            stoneId = x.StoneId
                        }).ToList(),
                        leatherList = entity.CartProductLeatherList.Select(x => new CartLeatherViewModel()
                        {
                            order = x.Order,
                            leatherId = x.LeatherId
                        }).ToList()
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = list
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
        /// صدا زدن صفحه سفارشی کردن محصول
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public ActionResult Customize(string id)
        {
            using (var db = new KiaGalleryContext())
            {
                var user = GetAuthenticatedUser();
                var idList = id.Split('-').Select(x => int.Parse(x)).ToList();
                var cartList = db.Cart.Include(x => x.Product).Include(x => x.Product.NormalSizeValue).Include(x => x.Product.ProductFileList).Include(x => x.Product.ProductStoneList).Include(x => x.Product.ProductLeatherList).Include(x => x.Product.Size).Include(x => x.Product.Size.SizeValueList).Where(x => x.BranchId == user.BranchId && idList.Any(y => y == x.Id)).ToList();

                ViewBag.Cart = cartList;
                ViewBag.StoneList = db.Stone.Include(x => x.StoneOutOfStockList).OrderBy(x => x.Order).ToList();
                ViewBag.LeatherList = db.Leather.OrderBy(x => x.Order).ToList();
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.Product = cartList[0].Product;

                var productId = cartList[0].Product.Id;
                ViewBag.SetProduct = db.SetProduct.Include(x => x.SetTo.ProductOuterWerkList).Include(x => x.SetTo.ProductFileList).Include(x => x.SetTo.ProductStoneList).Include(x => x.SetTo.ProductLeatherList).Include(x => x.SetTo.Size).Include(x => x.SetTo.Size.SizeValueList).Where(x => x.ProductId == productId).ToList();
                ViewBag.RelatedProduct = db.RelatedProduct.Include(x => x.RelatedTo.ProductFileList).Include(x => x.RelatedTo.ProductStoneList).Include(x => x.RelatedTo.ProductLeatherList).Include(x => x.RelatedTo.Size).Include(x => x.RelatedTo.Size.SizeValueList).Where(x => x.ProductId == productId).ToList();
            }

            return View("_Customize");
        }

        /// <summary>
        /// ساخت سفارش از سبد خرید
        /// </summary>
        /// <param name="model">لیست فیلتر های برای اعمال </param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, confirm-order")]
        public JsonResult MakeOrder(MakeOrderViewModel model)
        {
            Response response;
            try
            {

                List<Cart> result;
                using (var db = new KiaGalleryContext())
                {
                    double dateSetting = double.Parse(db.Settings.Where(x => x.Key == Settings.KeyInPreprationGoldDebtDateRange).SingleOrDefault()?.Value);
                    dateSetting = dateSetting * -1;
                    var user = GetAuthenticatedUser();
                    var dateRange = DateTime.Today.AddDays(dateSetting);

                    Branch branch = db.Branch.Single(x => x.Id == user.BranchId);
                    var inPrepationDebt = db.OrderDetail
                        .Where(x => x.CreateDate >= DbFunctions.TruncateTime(dateRange) && x.Order.BranchId == user.BranchId && x.Order.Branch.BranchType == BranchType.Solicitorship && x.OrderDetailStatus != OrderDetailStatus.Cancel && x.OrderDetailStatus != OrderDetailStatus.Sent)
                        .Sum(x => x.Product.Weight * x.Count);


                    if (branch.GoldCredit < branch.GoldDebt + inPrepationDebt)
                    {
                        response = new Response()
                        {
                            status = 501,
                            message = "همکار گرامی مانده بدهی شما بیش از سقف مجاز می باشد. در حال حاضر امکان ثبت سفارش شما مقدور نیست در صورت امکان با دفتر مرکزی تماس حاصل فرمائید."
                        };
                    }
                    else
                    {
                        var query = db.Cart.Where(x => x.BranchId == user.BranchId).Select(x => x);

                        if (model.cartIdList != null && !string.IsNullOrEmpty(model.cartIdList))
                        {
                            var idList = model.cartIdList.Split('-').Select(x => int.Parse(x)).ToList();
                            query = query.Where(x => x.BranchId == user.BranchId && idList.Any(y => y == x.Id));
                        }
                        else
                        {
                            if (model.cartType == 1)
                                query = query.Where(x => string.IsNullOrEmpty(x.Customer) && x.BranchId == user.BranchId);
                            else if (model.cartType == 2)
                                query = query.Where(x => !string.IsNullOrEmpty(x.Customer) && x.BranchId == user.BranchId);

                            if ((model.cartType == 0 || model.cartType == 1) && model.workshop != null && model.workshop.Count > 0)
                            {
                                query = query.Where(x => model.workshop.Any(y => y == x.Product.WorkshopId));
                            }
                        }

                        result = query.ToList();
                        if (result.Count > 0)
                        {
                            #region MakeOrder
                            var order = new Order()
                            {
                                BranchId = GetAuthenticatedUser().BranchId.GetValueOrDefault(),
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };
                            var lastSetnumberOrder = db.OrderDetail.OrderByDescending(x => x.Id).FirstOrDefault(x => x.SetNumber != null);
                            int lastSetNumber = 0;
                            if (lastSetnumberOrder != null)
                            {
                                lastSetNumber = lastSetnumberOrder.SetNumber.GetValueOrDefault();
                            }
                            order.OrderDetailList = result.Select(x => new OrderDetail()
                            {
                                Order = order,
                                ProductId = x.ProductId,
                                Product = x.Product,
                                OrderType = x.OrderType,
                                OrderDetailStatus = x.Product.Workshop.AutoConfirm ? OrderDetailStatus.InWorkshop : OrderDetailStatus.Registered,
                                Size = x.Size,
                                Size2 = x.Size2,
                                SetNumber = x.SetNumber != null ? lastSetNumber + x.SetNumber : null,
                                GoldType = x.GoldType,
                                ProductColor = x.ProductColor,
                                OuterWerkType = x.OuterWerkType,
                                LeatherLoop = x.LeatherLoop,
                                Customer = x.Customer,
                                PhoneNumber = x.PhoneNumber,
                                ForceOrder = x.ForceOrder,
                                BranchLabel = x.BranchLabel,
                                Description = x.Description,
                                Count = x.Count,
                                CreateUserId = user.Id,
                                ModifyUserId = user.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress,
                                OrderDetailStoneList = x.CartProductStoneList?.Select(y => new OrderDetailStone()
                                {
                                    Order = y.Order,
                                    StoneId = y.StoneId
                                }).ToList(),
                                OrderDetailLeatherList = x.CartProductLeatherList?.Select(y => new OrderDetailLeather()
                                {
                                    Order = y.Order,
                                    LeatherId = y.LeatherId
                                }).ToList()
                            }).ToList();

                            order.OrderLogList.Add(new OrderLog()
                            {
                                Order = order,
                                OrderStatus = OrderStatus.Normal,
                                CreateUserId = user.Id,
                                CreateDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            });

                            order.OrderDetailList.ForEach(x =>
                            {
                                var status = OrderDetailStatus.Registered;
                                if (x.Product.Workshop.AutoConfirm)
                                    status = OrderDetailStatus.InWorkshop;

                                x.OrderDetailLogList.Add(new OrderDetailLog()
                                {
                                    OrderDetail = x,
                                    OrderDetailStatus = OrderDetailStatus.Ordered,
                                    CreateUserId = x.CreateUserId,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                });

                                x.OrderDetailLogList.Add(new OrderDetailLog()
                                {
                                    OrderDetail = x,
                                    OrderDetailStatus = status,
                                    CreateUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                });
                            });

                            db.Order.Add(order);
                            db.SaveChanges();
                            order.OrderSerial = branch.Alias + "-" + order.Id;
                            order.OrderNumber = order.Id.ToString();
                            var autoConfirmList = order.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).ToList();
                            if (autoConfirmList.Count > 0)
                            {
                                WorkshopOrder wsOrder = new WorkshopOrder()
                                {
                                    Order = order,
                                    WorkshopOrderSerial = order.OrderSerial + "-WF1",
                                    WorkshopOrderNumber = order.OrderNumber + "-1",
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress,
                                };

                                db.WorkshopOrder.Add(wsOrder);
                                //db.SaveChanges();
                                //wsOrder.WorkshopOrderSerial = branch.Alias + order.Id +"-WF" + wsOrder.Id;
                                //wsOrder.WorkshopOrderNumber = order.Id + "-" + wsOrder.Id;
                                autoConfirmList.ForEach(x => x.WorkshopOrder = wsOrder);
                            }

                            result.ForEach(x =>
                            {
                                db.CartProductStone.RemoveRange(x.CartProductStoneList);
                                db.CartProductLeather.RemoveRange(x.CartProductLeatherList);
                            });
                            db.Cart.RemoveRange(result);

                            db.SaveChanges();
                            #endregion

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
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// دریافت شماره سفارش جدید برای شعبه
        /// </summary>
        /// <param name="db">شی اتصال به پایگاه داده</param>
        /// <param name="branch">شعبه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        private string GetNewOrderNo(KiaGalleryContext db, Branch branch)
        {
            var lastItem = db.Order.Where(x => x.BranchId == branch.Id).OrderByDescending(x => x.OrderSerial).FirstOrDefault();
            if (lastItem == null)
            {
                return branch.Alias.ToUpper() + 1.ToString();
            }
            else
            {
                var lastNo = lastItem.OrderSerial.Split(new string[] { branch.Alias.ToUpper() }, StringSplitOptions.None)[1];
                if (lastNo.IndexOf('-') >= 0)
                {
                    lastNo = lastNo.Split('-')[1];
                }
                var newNo = int.Parse(lastNo) + 1;
                return branch.Alias.ToUpper() + "-" + newNo.ToString();
            }
        }

        /// <summary>
        /// استعلام وزن وکدکارگاه
        /// </summary>
        /// <param name="model"> </param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>

        [Authorize(Roles = "admin, confirm-order")]
        public JsonResult ModalDetailBasket()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var query = db.Cart.Where(x => x.BranchId == user.BranchId).Select(x => x).ToList();

                    var list = query.GroupBy(x => x.Product.Workshop).Select(x => new
                    {
                        workshopName = x.Key.Name,
                        weight = x.Sum(y => Convert.ToDecimal(y.Product.Weight))
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
        /// حذف سفارش ها در مودال
        /// </summary>
        /// <param name="id">ردیف گیفت</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin,confirm-order ")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Cart.Where(x => x.Product.WorkshopId == id).ToList();
                    response = new Response()
                    {
                        status = 200,
                        message = "سفارش با موفقیت حذف شد."
                    };
                    db.Cart.RemoveRange(item);
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