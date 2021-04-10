using DocumentFormat.OpenXml.Bibliography;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Order;
using KiaGallery.Web.Models;
using KiaGallery.Web.Query;
using Newtonsoft.Json;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر سفارش
    /// </summary>
    public class OrderController : BaseController
    {
        /// <summary>
        /// تعداد ماه های لود کردن اطلاعات جزئیات سفارش
        /// </summary>
        private const int MonthToLoadDetail = -1;

        /// <summary>
        /// صفحه سفارش جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                var currentUserBranchType = GetAuthenticatedUser().BranchType;
                var query = db.Workshop.Where(x => x.Active == true && x.ProductList.Count(y => y.Active == true && y.ProductType != ProductType.OuterWerk) > 0);
                if (!User.IsInRole("admin"))
                {
                    switch (currentUserBranchType)
                    {
                        case BranchType.Branch:
                            query = query.Where(x => x.ProductList.Count(y => y.OrderableBranchType == true) > 0);
                            break;
                        case BranchType.Solicitorship:
                            query = query.Where(x => x.ProductList.Count(y => y.OrderableSolicitorshipType == true) > 0);
                            break;
                        case BranchType.CoWorker:
                            query = query.Where(x => x.ProductList.Count(y => y.OrderableCoWorkerType == true) > 0);
                            break;
                        case BranchType.Other:
                            query = query.Where(x => x.ProductList.Count(y => y.OrderableOtherType == true) > 0);
                            break;
                    }
                    ViewBag.Workshop = query.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    ViewBag.Workshop = query.OrderBy(x => x.Id).ToList();
                }

                ViewBag.CollectionList = db.ProductCollection.Select(x => new SelectOptionViewModel { id = x.Id, title = x.Title }).ToList();
            }
            return View();
        }
        /// <summary>
        /// صفحه سفارش جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, orderOuterWerk")]
        public ActionResult OuterWerk()
        {

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin, orderOuterWerk")]
        public ActionResult Operation()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.WorkshopList = db.Workshop.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
            }

            return View();
        }

        /// <summary>
        /// علت های وضعیت
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, reason")]
        public ActionResult OrderDetailReason()
        {
            return View();
        }

        /// <summary>
        /// صفحه سوابق سفارش
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public ActionResult History()
        {
            return View();
        }

        /// <summary>
        /// جزئیات سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public ActionResult Detail(int id)
        {
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Order = db.Order.Single(x => x.Id == id && x.BranchId == user.BranchId);
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true).ToList();
            }
            return View();
        }

        /// <summary>
        /// جزئیات همه ی سفارش
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public ActionResult DetailAll()
        {
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true).ToList();
                ViewBag.DateList = db.Order.Where(x => x.BranchId == user.BranchId && /*x.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail) &&*/ x.Deleted == false).GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).OrderByDescending(x => x.Key).Select(x => new { Date = x.Key, Count = x.Count() }).ToList().Select(x => new OrderDateViewModel() { Date = DateUtility.GetPersianDate(x.Date), Count = x.Count }).ToList();
            }
            return View();
        }

        /// <summary>
        /// لیست جزئیات سفارش برای یک سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>لیست کالاهای ثبت شده در سفارش</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult DetailList(OrderDetailSearchViewModel model)
        {
            Response response;
            try
            {
                List<OrderDetailViewModel> list;
                var user = GetAuthenticatedUser();
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.Order.BranchId == user.BranchId);
                    if (model.orderId != null)
                    {
                        query = query.Where(x => x.OrderId == model.orderId);
                    }
                    if (!string.IsNullOrEmpty(model.term))
                    {
                        query = query.Where(x => x.Order.OrderSerial.Contains(model.term.Trim()) || x.Product.Code.Contains(model.term.Trim()) || x.Product.BookCode.Contains(model.term.Trim()) || x.Product.Title.Contains(model.term.Trim()) || x.Product.Title.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.Order.Branch.Name.Contains(model.term.Trim()) || x.Order.Branch.Name.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.Customer.Contains(model.term.Trim()) || x.Customer.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.PhoneNumber.Contains(model.term.Trim()) || x.BranchLabel.Contains(model.term.Trim()) || x.Description.Contains(model.term.Trim()) || x.Description.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")));
                    }

                    if (model.typeList != null && model.typeList.Count > 0)
                    {
                        query = query.Where(x => model.typeList.Any(y => y == x.Product.ProductType));
                    }

                    if (model.workshopList != null && model.workshopList.Count(y => y > 0) > 0)
                    {
                        query = query.Where(x => model.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId));
                    }

                    if (model.status != null)
                    {
                        query = query.Where(x => x.OrderDetailStatus == model.status);
                    }

                    if (model.setNumber != null)
                    {
                        query = query.Where(x => x.SetNumber == model.setNumber);
                    }

                    if (!string.IsNullOrEmpty(model.date))
                    {
                        var date = DateUtility.GetDateTime(model.date);
                        query = query.Where(x => x.CreateDate >= date && x.CreateDate <= DbFunctions.AddDays(date, 1));
                    }
                    else if (!string.IsNullOrEmpty(model.fromDate) || !string.IsNullOrEmpty(model.toDate))
                    {
                        if (!string.IsNullOrEmpty(model.fromDate) && DateUtility.GetDateTime(model.fromDate) != null)
                        {
                            var fromDate = DateUtility.GetDateTime(model.fromDate).GetValueOrDefault();
                            query = query.Where(x => x.Order.CreateDate >= fromDate);
                        }

                        if (!string.IsNullOrEmpty(model.toDate) && DateUtility.GetDateTime(model.toDate) != null)
                        {
                            var toDate = DateUtility.GetDateTime(model.toDate).GetValueOrDefault();
                            query = query.Where(x => x.Order.CreateDate <= toDate);
                        }
                    }

                    switch (model.order)
                    {
                        case "productType":
                            query = query.OrderBy(x => x.Product.ProductType);
                            break;
                        case "Vitrin":
                            query = query.OrderByDescending(x => x.OrderType == OrderType.None).ThenByDescending(x => x.Customer == null && x.OrderType == OrderType.Personalization);
                            break;
                        case "OrderVitrin":
                            query = query.OrderByDescending(x => x.Customer == null && x.OrderType == OrderType.Personalization).ThenByDescending(x => x.OrderType == OrderType.None);
                            break;
                        case "Customer":
                            query = query.OrderByDescending(x => x.Customer).ThenByDescending(x => x.OrderType == OrderType.Personalization);
                            break;
                        case "ForceOrder":
                            query = query.OrderByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenBy(x => x.OrderType == OrderType.None);
                            break;
                        case "DateAscending":
                            query = query.OrderBy(x => x.Id);
                            break;
                        case "SetNumber":
                            query = query.OrderByDescending(x => x.SetNumber).ThenByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenByDescending(x => x.SetNumber).ThenBy(x => x.OrderType == OrderType.None);
                            break;
                        default:
                            query = query.OrderByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenByDescending(x => x.SetNumber).ThenBy(x => x.OrderType == OrderType.None);
                            break;
                    }

                    dataCount = query.Count();
                    if (model.count > 0)
                    {
                        query = query.Skip(model.page * model.count).Take(model.count);
                    }
                    list = query.Select(item => new OrderDetailViewModel()
                    {
                        id = item.Id,
                        orderSerial = "KIA-" + item.Order.OrderSerial,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        orderType = item.OrderType,
                        productSizeId = item.Product.SizeId,
                        //orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        workshopName = item.Product.Workshop.Name,
                        size = item.Size,
                        setNumber = item.SetNumber,
                        goldType = item.GoldType,
                        outerWerkType = item.OuterWerkType,
                        //goldTypeTitle = Enums.GetTitle(item.GoldType),
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        deliverDateRequest = item.DeliverDateRequest,
                        branchLabel = item.BranchLabel,
                        description = item.Description,
                        title = item.Product.Title,
                        code = item.Product.Code,
                        bookCode = item.Product.BookCode,
                        weight = item.Product.Weight,
                        count = item.Count,
                        orderDetailStatus = item.OrderDetailStatus,
                        //orderDetailStatusTitle = Enums.GetTitle(item.OrderDetailStatus),
                        createdUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        CreateDate = item.CreateDate,
                        //createDate = DateUtility.GetPersianDate(item.CreateDate),
                        relatedOrderDetailSerial = "KIA-" + item.RelatedOrderDetail.Order.OrderSerial,
                        stoneList = item.OrderDetailStoneList.Select(x => new OrderDetailStoneViewModel()
                        {
                            order = x.Order,
                            stoneName = x.Stone.Name
                        }).ToList(),
                        leatherList = item.OrderDetailLeatherList.Select(x => new OrderDetailLeatherViewModel()
                        {
                            order = x.Order,
                            leatherName = x.Leather.Name
                        }).ToList(),
                        returned = item.OrderDetailLogList.Count(x => x.Description == "مرجوعی" || x.Description == "مرجوعی به کارگاه اول" || x.Description == "مرجوعی به کارگاه دوم")
                    })
                    .ToList();
                    int conter = 1;
                    list.ForEach(item =>
                    {
                        item.orderTypeTitle = Enums.GetTitle(item.orderType);
                        item.goldTypeTitle = Enums.GetTitle(item.goldType);
                        item.orderDetailStatusTitle = Enums.GetTitle(item.orderDetailStatus);
                        item.outerWerkTypeTitle = Enums.GetTitle(item.outerWerkType);
                        item.createDate = DateUtility.GetPersianDate(item.CreateDate);
                        item.index = conter + (model.page * model.count);
                        conter++;
                    });

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            weight = list.Sum(x => x.weight),
                            pageCount = model.count > 0 ? Math.Ceiling((double)dataCount / model.count) : 1,
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
        /// لیست سواابق سفارش ثبت شده توسط شعبه
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست سفارش های ثبت شده شعبه</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult GetAllHistory(OrderSearchViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                List<Order> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Order.Include(x => x.OrderDetailList).Include(x => x.CreateUser).Include("OrderDetailList.Product").Where(x => x.BranchId == user.BranchId && x.Deleted == false);

                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.OrderSerial.Contains(model.term.Trim()) || x.OrderDetailList.Any(y => y.Product.Title.Contains(model.term.Trim())) || x.Branch.Name.Contains(model.term.Trim()) || x.OrderDetailList.Any(y => y.Customer.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.PhoneNumber.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.BranchLabel.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.Description.Contains(model.term.Trim())));
                    }

                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    list = query.ToList();

                    var data = list.Select(item => new OrderViewModel()
                    {
                        id = item.Id,
                        orderSerial = "KIA-" + item.OrderSerial,
                        orderNumber = "KIA-" + item.OrderNumber,
                        sumCount = item.OrderDetailList.Sum(x => x.Count),
                        sumCountSet = item.OrderDetailList.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count(),
                        sumWeight = Math.Round(double.Parse(item.OrderDetailList.Sum(x => x.Product.Weight * x.Count).ToString()), 2),
                        registered = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count),
                        registeredWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        inWorkshop = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count),
                        inWorkshopWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        underConstruction = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count),
                        underConstructionWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        outOfConstruction = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count),
                        outOfConstructionWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        inWorkshop2 = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop2).Sum(x => x.Count),
                        inWorkshopWeight2 = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop2).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        underConstruction2 = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction2).Sum(x => x.Count),
                        underConstructionWeight2 = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction2).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        outOfConstruction2 = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction2).Sum(x => x.Count),
                        outOfConstructionWeight2 = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction2).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        inPreparation = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count),
                        inPreparationWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        readyForDelivery = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count),
                        readyForDeliveryWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        sent = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count),
                        sentWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        shortage = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count),
                        shortageWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        shortageOrder = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count),
                        shortageOrderWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        cancel = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count),
                        cancelWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        createUser = item.CreateUser?.FullName,
                        createDate = DateUtility.GetPersianDate(item.CreateDate),
                    }).ToList();

                    data.ForEach(x => GetBackgroundColor(x));

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
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
        /// اضافه کردن محصول به لیست علاقه مندی ها
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns>نتیجه اضافه شدن به لیست علاقه مندی ها</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult AddToFavorite(int id)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    if (db.FavouritesProduct.Count(x => x.ProductId == id && x.BranchId == user.BranchId) > 0)
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "محصول در لیست علاقه مندی های شما موجود است."
                        };
                    }
                    else
                    {
                        var fav = new FavouritesProduct()
                        {
                            BranchId = user.BranchId.GetValueOrDefault(),
                            ProductId = id,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        db.FavouritesProduct.Add(fav);
                        db.SaveChanges();

                        response = new Response()
                        {
                            status = 200,
                            message = "محصول با موفقیت به لیست علاقه مندی ها اضافه شد."
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
        /// حذف کردن محصول از لیست علاقه مندی ها
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns>نتیجه حذف شدن به لیست علاقه مندی ها</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult RemoveFavorite(int id)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    if (db.FavouritesProduct.Count(x => x.ProductId == id && x.BranchId == user.BranchId) > 0)
                    {
                        var item = db.FavouritesProduct.Single(x => x.ProductId == id && x.BranchId == user.BranchId);
                        db.FavouritesProduct.Remove(item);
                        db.SaveChanges();

                        response = new Response()
                        {
                            status = 200,
                            message = "محصول با موفقیت از لیست علاقه مندی های شعبه حذف شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "محصول در لیست علاقه مندی های شما یافت نشد."
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
        /// دریافت لیست سوابق سفارش یک محصول در سفارش
        /// </summary>
        /// <param name="id">ردیف محصول در سفارش</param>
        /// <returns>لیست سوابق سفارش یک محصول</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk, admin-order, order-workshop")]
        public JsonResult GetLogDetail(int id)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    List<LogDetailViewModel> logList = db.OrderDetailLog.Where(x => x.OrderDetailId == id).Select(x => new LogDetailViewModel()
                    {
                        id = x.Id,
                        status = x.OrderDetailStatus,
                        reasonText = x.OrderDetailLogReason.Text,
                        description = x.Description,
                        createdDateTime = x.CreateDate,
                        createUser = x.CreateUser.FirstName + " " + x.CreateUser.LastName
                    }).ToList();
                    logList.ForEach(x =>
                    {
                        x.reasonText = x.reasonText != null ? "علت: " + x.reasonText : "";
                        x.createdDate = DateUtility.GetPersianDateTime(x.createdDateTime);
                        x.statusText = Enums.GetTitle(x.status);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = logList
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
        /// مدیریت سفارش
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-order")]
        public ActionResult Manage()
        {
            using (var db = new KiaGalleryContext())
            {

                ViewBag.BranchList = db.Branch.ToList();

                ViewBag.CoWorkerList = db.Branch.Where(item => item.BranchType == BranchType.CoWorker).Select(x => new BranchViewModel
                {
                    id = x.Id,
                    orderCount = x.OrderList.Count,
                    name = x.Name
                }).ToList();


                ViewBag.DateList = db.Order.Where(x => /*x.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail) &&*/ x.Deleted == false).GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).OrderByDescending(x => x.Key).Select(x => new { Date = x.Key, Count = x.Count() }).ToList().Select(x => new OrderDateViewModel() { Date = DateUtility.GetPersianDate(x.Date), Count = x.Count }).ToList();
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true).ToList();
            }
            return View();
        }

        public JsonResult GetManageDateList(OrderDateViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Order.Where(x => /*x.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail) &&*/ x.Deleted == false);
                    
                    if (model.branchType != null && model.branchType.Count > 0)
                    {
                        query = query.Where(x => model.branchType.Contains(x.Branch.BranchType));
                    }

                    if (model.coWorker > 0)
                    {
                        query = query.Where(x => x.BranchId == model.coWorker);
                    }

                    var list = query.GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).OrderByDescending(x => x.Key).Select(x => new { Date = x.Key, Count = x.Count() }).ToList().Select(x => new OrderDateViewModel() { Date = DateUtility.GetPersianDate(x.Date), CountString = DateUtility.GetPersianDate(x.Date) + " " + "("+ x.Count + ")" }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new { list }
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
        ///  مدیریت سفارش حذف شده
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-order")]
        public ActionResult Deleted()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.ToList();
                ViewBag.DateList = db.Order.Where(x => /*x.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail) &&*/ x.Deleted == false).GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).OrderByDescending(x => x.Key).Select(x => new { Date = x.Key, Count = x.Count() }).ToList().Select(x => new OrderDateViewModel() { Date = DateUtility.GetPersianDate(x.Date), Count = x.Count }).ToList();
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true).ToList();
            }
            return View();
        }

        /// <summary>
        ///  مدیریت سفارش حذف شده
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-order")]
        public ActionResult Archive()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.ToList();
                ViewBag.DateList = db.Order.Where(x => /*x.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail) &&*/ x.Deleted == false).GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).OrderByDescending(x => x.Key).Select(x => new { Date = x.Key, Count = x.Count() }).ToList().Select(x => new OrderDateViewModel() { Date = DateUtility.GetPersianDate(x.Date), Count = x.Count }).ToList();
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true).ToList();
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin, admin-order")]
        //[Route("order/ManipulateAll/{branch}")]
        public ActionResult ManipulateAll(int? id)
        {
            using (var db = new KiaGalleryContext())
            {
                var currentUser = GetAuthenticatedUser();
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.WorkshopList = db.Workshop.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.DateList = db.Order.Where(x => /*x.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail) &&*/ x.Deleted == false).GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).OrderByDescending(x => x.Key).Select(x => new { Date = x.Key, Count = x.Count() }).ToList().Select(x => new OrderDateViewModel() { Date = DateUtility.GetPersianDate(x.Date), Count = x.Count }).ToList();
                ViewBag.BranchType = id;
                var roleList = db.Role.Where(x => x.UserId == currentUser.Id && x.Title.StartsWith("order_")).Select(x => x.Title).ToList();
                roleList = roleList.Select(x => "\"" + x.Substring(6) + "\"").ToList();
                ViewBag.RoleList = "[" + string.Join(",", roleList) + "]";
                var query = db.WorkshopOrder.Where(x => x.Order.Deleted == false);
                ViewBag.WorkshopOrder = query.Select(x => new WorkshopDropDwonViewModel()
                {
                    id = x.Id,
                    val = x.WorkshopOrderSerial
                }).ToList();
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin, admin-order")]
        public ActionResult ManipulateCoWorker(int? id)
        {
            using (var db = new KiaGalleryContext())
            {
                var currentUser = GetAuthenticatedUser();
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.WorkshopList = db.Workshop.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.DateList = db.Order.Where(x => /*x.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail) &&*/ x.Deleted == false).GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).OrderByDescending(x => x.Key).Select(x => new { Date = x.Key, Count = x.Count() }).ToList().Select(x => new OrderDateViewModel() { Date = DateUtility.GetPersianDate(x.Date), Count = x.Count }).ToList();
                ViewBag.Branch = id;
                var roleList = db.Role.Where(x => x.UserId == currentUser.Id && x.Title.StartsWith("order_")).Select(x => x.Title).ToList();
                roleList = roleList.Select(x => "\"" + x.Substring(6) + "\"").ToList();
                ViewBag.RoleList = "[" + string.Join(",", roleList) + "]";
                var query = db.WorkshopOrder.Where(x => x.Order.Deleted == false);
                ViewBag.WorkshopOrder = query.Select(x => new WorkshopDropDwonViewModel()
                {
                    id = x.Id,
                    val = x.WorkshopOrderSerial
                }).ToList();
            }
            return View("ManipulateAll");
        }

        /// <summary>
        /// مدیریت سفارش و تغییر وضعیت سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-order")]
        public ActionResult Manipulate(int id)
        {
            var currentUser = GetAuthenticatedUser();

            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true).ToList();
                ViewBag.Order = db.Order.Single(x => x.Id == id && x.Deleted == false);

                var roleList = db.Role.Where(x => x.UserId == currentUser.Id && x.Title.StartsWith("order_")).Select(x => x.Title).ToList();
                roleList = roleList.Select(x => "\"" + x.Substring(6) + "\"").ToList();
                ViewBag.RoleList = "[" + string.Join(",", roleList) + "]";
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin, admin-order")]
        public ActionResult ViewAllOrderDetail()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.WorkshopList = db.Workshop.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.DateList = db.Order.Where(x => /*x.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail) &&*/ x.Deleted == false).GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).OrderByDescending(x => x.Key).Select(x => new { Date = x.Key, Count = x.Count() }).ToList().Select(x => new OrderDateViewModel() { Date = DateUtility.GetPersianDate(x.Date), Count = x.Count }).ToList();
            }
            return View();
        }

        /// <summary>
        /// مدیریت سفارش و تغییر وضعیت سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-order")]
        public ActionResult ViewOrderDetail(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true).ToList();
                ViewBag.Order = db.Order.Single(x => x.Id == id && x.Deleted == false);
            }
            return View();
        }

        /// <summary>
        /// لیست سفارشات جهت مدیریت
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست سفارشات</returns>
        [HttpPost]
        [Authorize(Roles = "admin, admin-order, allOrder")]
        public JsonResult GetAll(OrderSearchViewModel model)
        {
            var user = GetAuthenticatedUser();
            Response response;
            try
            {
                //QueryUserType type;
                //if (User.IsInRole("admin"))
                //{
                //    type = QueryUserType.Admin;
                //}
                //else
                //{
                //    if (User.IsInRole("leatherProductUser"))
                //    {
                //        type = QueryUserType.LeatherProduct;
                //    }
                //    else
                //    {
                //        type = QueryUserType.NotLeatherProduct;
                //    }
                //}

                //return Json(OrderQuery.OrderSearchQuery(type, model), JsonRequestBehavior.AllowGet);

                //List<Order> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Order.Select(x => x);
                    if (model.deleted == true)
                    {
                        query = query.Where(x => x.Deleted == true);
                    }
                    else
                    {
                        query = query.Where(x => x.Deleted == false);
                    }

                    if (model.archiveOnly == null && (model.archive == null || model.archive == false))
                    {
                        var notCompleteStatus = new OrderDetailStatus[] { OrderDetailStatus.Registered, OrderDetailStatus.InWorkshop, OrderDetailStatus.UnderConstruction, OrderDetailStatus.OutOfConstruction, OrderDetailStatus.InPreparation, OrderDetailStatus.ReadyForDelivery, OrderDetailStatus.Shortage };
                        query = query.Where(x => x.OrderDetailList.Count(y => notCompleteStatus.Any(z => z == y.OrderDetailStatus)) > 0);
                    }

                    if (model.archiveOnly == true)
                    {
                        var notCompleteStatus = new OrderDetailStatus[] { OrderDetailStatus.Registered, OrderDetailStatus.InWorkshop, OrderDetailStatus.UnderConstruction, OrderDetailStatus.OutOfConstruction, OrderDetailStatus.InPreparation, OrderDetailStatus.ReadyForDelivery, OrderDetailStatus.Shortage };
                        query = query.Where(x => x.OrderDetailList.Count(y => y.OrderDetailStatus == OrderDetailStatus.Sent) > 0);
                    }

                    if (model.branchId != null && model.branchId.Count > 0)
                    {
                        query = query.Where(x => model.branchId.Any(y => y == x.BranchId));
                    }
                    if (model.branchType != null)
                    {
                        query = query.Where(x => model.branchType.Any(y => y == x.Branch.BranchType));
                    }

                    if (model.coWorker != null)
                    {
                        query = query.Where(x => model.coWorker == x.BranchId);
                    }

                    if (!string.IsNullOrEmpty(model.date))
                    {
                        var date = DateUtility.GetDateTime(model.date);
                        query = query.Where(x => x.CreateDate >= date && x.CreateDate <= DbFunctions.AddDays(date, 1));
                    }

                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.OrderSerial.Contains(model.term.Trim()) || x.OrderDetailList.Any(y => y.Product.Title.Contains(model.term.Trim())) || x.Branch.Name.Contains(model.term.Trim()) || x.OrderDetailList.Any(y => y.Customer.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.PhoneNumber.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.BranchLabel.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.Description.Contains(model.term.Trim())));
                    }
                    if (User.IsInRole("leatherProductUser"))
                    {
                        query = query.Where(x => x.OrderDetailList.Count(z => z.Product.WorkshopId2 == 5 || z.Product.ProductType == ProductType.OuterWerk || z.Product.ProductType == ProductType.WatchPendent2 || z.Product.ProductType == ProductType.Plaque) > 0);
                    }
                    //if (User.IsInRole("outerwerkProductUser"))
                    //{
                    //    query = query.Where(x => x.OrderDetailList.Count(z=>z.Product.ProductType == ProductType.OuterWerk) > 0);
                    //}
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    //list = query.ToList();

                    var leatherProductRole = User.IsInRole("leatherProductUser");
                    var isAdmin = User.IsInRole("admin") || User.IsInRole("allOrder");
                    var data = query.Select(item => new OrderViewModelFloat()
                    {
                        id = item.Id,
                        orderSerial = item.Branch.BranchType != BranchType.CoWorker ? "KIA-" + item.OrderSerial : item.OrderSerial,
                        sumCount = item.OrderDetailList.Sum(x => x.Count) > 0 ?
                        (
                        isAdmin ?
                            (item.OrderDetailList.Sum(x => x.Count))
                        :
                            (!leatherProductRole ? item.OrderDetailList.Where(z => z.Product.WorkshopId2 != 5 && z.Product.ProductType != ProductType.OuterWerk && z.Product.ProductType != ProductType.WatchPendent2 && z.Product.ProductType != ProductType.Plaque).Sum(x => x.Count) : item.OrderDetailList.Where(z => z.Product.WorkshopId2 == 5 || z.Product.ProductType == ProductType.OuterWerk || z.Product.ProductType == ProductType.WatchPendent2 || z.Product.ProductType == ProductType.Plaque).Sum(x => x.Count))
                        )
                        : 0,///چک کردن دسترسی کاربر محصولات چرمی که اگر شرط برقرار بود فقط وزن محصولات چرمی را ببیند اگر نبود فقط محصولات غیر چرمی و اگر دسترسی ادمین بود همه را ببیند
                        sumCountSet = item.OrderDetailList.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count() > 0 ? item.OrderDetailList.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count() : 0,
                        //sumWeight = Math.Round(double.Parse(item.OrderDetailList.Sum(x => x.Product.Weight * x.Count).ToString()), 2),
                        sumWeight = item.OrderDetailList.Sum(x => x.Product.Weight * x.Count) > 0 ?
                        (
                        isAdmin ?
                            (item.OrderDetailList.Sum(x => x.Product.Weight * x.Count))
                        :
                            (!leatherProductRole ? item.OrderDetailList.Where(z => z.Product.WorkshopId2 != 5 && z.Product.ProductType != ProductType.OuterWerk && z.Product.ProductType != ProductType.WatchPendent && z.Product.ProductType != ProductType.Plaque).Sum(x => x.Product.Weight * x.Count) : item.OrderDetailList.Where(z => z.Product.WorkshopId2 == 5 || z.Product.ProductType == ProductType.OuterWerk || z.Product.ProductType == ProductType.WatchPendent || z.Product.ProductType == ProductType.Plaque).Sum(x => x.Product.Weight * x.Count))
                        )
                        : 0,///چک کردن دسترسی کاربر محصولات چرمی که اگر شرط برقرار بود فقط تعداد محصولات چرمی را ببیند اگر نبود فقط محصولات غیر چرمی و اگر دسترسی ادمین بود همه را ببیند
                        createUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        createBranch = item.Branch.Name,
                        createDateTime = item.CreateDate,
                        registered = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count) : 0,
                        sent = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count) : 0,
                        shortageOrder = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count) : 0,
                        cancel = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count) : 0,
                        shortage = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count) : 0,
                        branchType = item.Branch.BranchType
                        //createDate = DateUtility.GetPersianDate(item.CreateDate)
                    }).ToList();
                    var coWorkerCount = db.Order.Where(item => (item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count) : 0) == (item.OrderDetailList.Sum(x => x.Count) > 0 ?
                       (
                       isAdmin ?
                           (item.OrderDetailList.Sum(x => x.Count))
                       :
                           (!leatherProductRole ? item.OrderDetailList.Where(z => z.Product.WorkshopId2 != 5 && z.Product.ProductType != ProductType.OuterWerk && z.Product.ProductType != ProductType.WatchPendent2 && z.Product.ProductType != ProductType.Plaque).Sum(x => x.Count) : item.OrderDetailList.Where(z => z.Product.WorkshopId2 == 5 || z.Product.ProductType == ProductType.OuterWerk || z.Product.ProductType == ProductType.WatchPendent2 || z.Product.ProductType == ProductType.Plaque).Sum(x => x.Count))
                       )
                       : 0) && item.Branch.BranchType == BranchType.CoWorker).Count();
                    var branchesCount = query.Where(item => (item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count) : 0) == (item.OrderDetailList.Sum(x => x.Count) > 0 ?
                       (
                       isAdmin ?
                           (item.OrderDetailList.Sum(x => x.Count))
                       :
                           (!leatherProductRole ? item.OrderDetailList.Where(z => z.Product.WorkshopId2 != 5 && z.Product.ProductType != ProductType.OuterWerk && z.Product.ProductType != ProductType.WatchPendent2 && z.Product.ProductType != ProductType.Plaque).Sum(x => x.Count) : item.OrderDetailList.Where(z => z.Product.WorkshopId2 == 5 || z.Product.ProductType == ProductType.OuterWerk || z.Product.ProductType == ProductType.WatchPendent2 || z.Product.ProductType == ProductType.Plaque).Sum(x => x.Count))
                       )
                       : 0) && item.Branch.BranchType != BranchType.CoWorker).Count();
                    var list = data.Select(x => new OrderViewModel()
                    {
                        id = x.id,
                        orderSerial = x.orderSerial,
                        sumCount = x.sumCount,
                        sumCountSet = x.sumCountSet,
                        sumWeight = x.sumWeight > 0 ? Math.Round(double.Parse(x.sumWeight.ToString()), 2) : 0,
                        createUser = x.createUser,
                        createBranch = x.createBranch,
                        createDate = DateUtility.GetPersianDate(x.createDateTime),
                        sent = x.sent,
                        cancel = x.cancel,
                        registered = x.registered,
                        shortage = x.shortage,
                        shortageOrder = x.shortageOrder,
                        branchType = x.branchType

                    }).ToList();

                    list.ForEach(x =>
                    {
                        GetBackgroundColor(x);
                    });

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            pageCount = Math.Ceiling((double)dataCount / model.count),
                            count = dataCount,
                            page = model.page + 1,
                            coWorkerCount = coWorkerCount,
                            branchesCount
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                response = new Response()
                {
                    status = 500,
                    message = ex.InnerException.InnerException.ToString()
                };
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusDetail(int id)
        {
            Response response;
            try
            {
                var leatherProductRole = User.IsInRole("leatherProductUser");
                var isAdmin = User.IsInRole("admin");
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Order.Where(x => x.Id == id);

                    var data = query.Select(item => new
                    {
                        sumCount = item.OrderDetailList.Sum(x => x.Count) > 0 ?
                        (
                        isAdmin ?
                            (item.OrderDetailList.Sum(x => x.Count))
                        :
                            (!leatherProductRole ? item.OrderDetailList.Where(z => z.Product.WorkshopId2 != 5 && z.Product.ProductType != ProductType.OuterWerk && z.Product.ProductType != ProductType.WatchPendent && z.Product.ProductType != ProductType.Plaque).Sum(x => x.Count) : item.OrderDetailList.Where(z => z.Product.WorkshopId2 == 5 || z.Product.ProductType == ProductType.OuterWerk || z.Product.ProductType == ProductType.WatchPendent2 || z.Product.ProductType == ProductType.Plaque).Sum(x => x.Count))
                        )
                        : 0,
                        sumWeight = Math.Round((float?)item.OrderDetailList.Sum(x => x.Count * x.Product.Weight) ?? 0, 2) > 0 ?
                        (
                        isAdmin ?
                            (Math.Round((float?)item.OrderDetailList.Sum(x => x.Product.Weight * x.Count) ?? 0, 2))
                        :
                            (!leatherProductRole ? Math.Round((float?)item.OrderDetailList.Where(z => z.Product.WorkshopId2 != 5 && z.Product.ProductType != ProductType.OuterWerk && z.Product.ProductType != ProductType.WatchPendent && z.Product.ProductType != ProductType.Plaque).Sum(x => x.Product.Weight * x.Count) ?? 0, 2) : Math.Round((float?)item.OrderDetailList.Where(z => z.Product.WorkshopId2 == 5 || z.Product.ProductType == ProductType.OuterWerk || z.Product.ProductType == ProductType.WatchPendent2 || z.Product.ProductType == ProductType.Plaque).Sum(x => x.Product.Weight * x.Count) ?? 0, 2))
                        )
                        : 0,
                        registered = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count)) ?? 0,
                        registeredWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        inWorkshop = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count)) ?? 0,
                        inWorkshopWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        underConstruction = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count)) ?? 0,
                        underConstructionWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        outOfConstruction = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count)) ?? 0,
                        outOfConstructionWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        inWorkshop2 = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop2).Sum(x => x.Count)) ?? 0,
                        inWorkshopWeight2 = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop2).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        underConstruction2 = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction2).Sum(x => x.Count)) ?? 0,
                        underConstructionWeight2 = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction2).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        outOfConstruction2 = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction2).Sum(x => x.Count)) ?? 0,
                        outOfConstructionWeight2 = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction2).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        inPreparation = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count)) ?? 0,
                        inPreparationWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        readyForDelivery = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count)) ?? 0,
                        readyForDeliveryWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        sent = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count)) ?? 0,
                        sentWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        shortage = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count)) ?? 0,
                        shortageWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        shortageOrder = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count)) ?? 0,
                        shortageOrderWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        cancel = ((int?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count)) ?? 0,
                        cancelWeight = Math.Round((float?)item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count * x.Product.Weight) ?? 0, 2),
                        createUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        createBranch = item.Branch.Name,
                        createDateTime = item.CreateDate
                    }).SingleOrDefault();
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
        /// گرفتن کوئری اطلاعات یک یا کل سفارشات برای اجرا
        /// </summary>
        /// <param name="db">شی دیتابیس</param>
        /// <param name="model">مدلی که فیلتر های مورد نظر را در خود دارد</param>
        /// <returns>کوئری ساخته و آماده اجرا</returns>
        public IQueryable<OrderDetail> GetQuery(KiaGalleryContext db, OrderDetailSearchViewModel model)
        {
            var query = db.OrderDetail.Include(x => x.Order).Include(x => x.Product).Include(x => x.Product.ProductFileList).Include(x => x.Product.Workshop).Include(x => x.OrderDetailStoneList).Include(x => x.OrderDetailLeatherList).Include(x => x.RelatedOrderDetail.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.CreateUser).Where(x => x.Order.Deleted == false);
            if (model.orderId != null && model.orderId > 0)
            {
                query = query.Where(x => x.OrderId == model.orderId /*&& x.Order.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail)*/);
            }

            if (model.workshopOrderId != null && model.workshopOrderId > 0)
            {
                query = query.Where(x => x.WorkshopOrderId == model.workshopOrderId);
            }

            if (!string.IsNullOrEmpty(model.term?.Trim()))
            {
                query = query.Where(x => x.Order.OrderSerial.Contains(model.term.Trim()) || x.BranchLabel.Contains(model.term.Trim()) || x.Description.Contains(model.term.Trim()) || x.Customer.Contains(model.term.Trim()) || x.PhoneNumber.Contains(model.term.Trim()) || x.Product.Code.Contains(model.term.Trim()) || x.Product.BookCode.Contains(model.term.Trim()) || x.Product.Title.Contains(model.term.Trim()));
            }

            if (model.typeList != null && model.typeList.Count > 0)
            {
                query = query.Where(x => model.typeList.Any(y => y == x.Product.ProductType));
            }

            if (model.status != null)
            {
                query = query.Where(x => x.OrderDetailStatus == model.status);
            }

            if (model.setNumber != null)
            {
                query = query.Where(x => x.SetNumber == model.setNumber);
            }

            if (model.workshopList != null && model.workshopList.Count(y => y > 0) > 0)
            {
                query = query.Where(x => model.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId));
            }

            if (model.branchList != null && model.branchList.Count(y => y > 0) > 0)
            {
                query = query.Where(x => model.branchList.Where(y => y > 0).Any(y => y == x.Order.BranchId));
            }

            if (!string.IsNullOrEmpty(model.date))
            {
                var date = DateUtility.GetDateTime(model.date);
                query = query.Where(x => x.CreateDate >= date && x.CreateDate <= DbFunctions.AddDays(date, 1));
            }
            else if (!string.IsNullOrEmpty(model.fromDate) || !string.IsNullOrEmpty(model.toDate))
            {
                if (!string.IsNullOrEmpty(model.fromDate) && DateUtility.GetDateTime(model.fromDate) != null)
                {
                    var fromDate = DateUtility.GetDateTime(model.fromDate).GetValueOrDefault();
                    query = query.Where(x => x.Order.CreateDate >= fromDate);
                }

                if (!string.IsNullOrEmpty(model.toDate) && DateUtility.GetDateTime(model.toDate) != null)
                {
                    var toDate = DateUtility.GetDateTime(model.toDate).GetValueOrDefault();
                    query = query.Where(x => x.Order.CreateDate <= toDate);
                }
            }

            switch (model.order)
            {
                case "productType":
                    query = query.OrderBy(x => x.Product.ProductType);
                    break;
                case "Vitrin":
                    query = query.OrderByDescending(x => x.OrderType == OrderType.None).ThenByDescending(x => x.Customer == null && x.OrderType == OrderType.Personalization);
                    break;
                case "OrderVitrin":
                    query = query.OrderByDescending(x => x.Customer == null && x.OrderType == OrderType.Personalization).ThenByDescending(x => x.OrderType == OrderType.None);
                    break;
                case "Customer":
                    query = query.OrderByDescending(x => x.Customer).ThenByDescending(x => x.OrderType == OrderType.Personalization);
                    break;
                case "ForceOrder":
                    query = query.OrderByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenBy(x => x.OrderType == OrderType.None); ;
                    break;
                case "DateAscending":
                    query = query.OrderBy(x => x.Id);
                    break;
                case "SetNumber":
                    query = query.OrderByDescending(x => x.SetNumber).ThenByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenByDescending(x => x.SetNumber).ThenBy(x => x.OrderType == OrderType.None);
                    break;
                default:
                    query = query.OrderByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenByDescending(x => x.SetNumber).ThenBy(x => x.OrderType == OrderType.None);
                    break;
            }

            return query;
        }

        /// <summary>
        /// گرفتن اطلاعات یک یا کل سفارشات برای اجرا
        /// </summary>
        /// <param name="db">شی دیتابیس</param>
        /// <param name="model">مدلی که فیلتر های مورد نظر را در خود دارد</param>
        /// <returns>اطلاعات یک یا کل سفارشات برای اجرا</returns>
        public Response GetData(OrderDetailSearchViewModel model)
        {
            Response response;


            var Roles = new string[] { "registeredTab", "sentToWorkshopTab", "underConstructionTab", "outOfConstructionTab", "inPreprationTab", "readyForDeliverTab", "sentTab", "shortageTab", "", "canceledTab", "sentToWorkshopTwoTab", "inPreprationInWorkshopTwoTab", "outOfConstructionInWorkshopTwoTab" };
            if (!User.IsInRole("admin") && model.status != null && model.status >= 0 && !User.IsInRole(Roles[(int)model.status]))
            {
                response = new Response
                {
                    status = 404,
                    message = "شما مجاز به انجام این کار نیستید"
                };
                return response;
            }
            List<OrderDetailViewModel> list;
            var orderNumberList = new List<string>();
            var persianDateList = new List<string>();

            int dataCount;
            int allDataCount;
            double pageCount = 0;
            float? allWeight;
            using (var db = new KiaGalleryContext())
            {
                var query = db.OrderDetail.Include(x => x.Order).Include(x => x.Product).Include(x => x.Product.ProductFileList).Include(x => x.Product.Workshop).Include(x => x.OrderDetailStoneList).Include(x => x.OrderDetailLeatherList).Include(x => x.RelatedOrderDetail.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.CreateUser).Where(x => x.Order.Deleted == false);

                if (model.branch != null && model.branch > 0)
                {
                    query = query.Where(x => x.Order.BranchId == model.branch);
                }
                if (model.branchType != null && model.branchType >= 0)
                {
                    query = query.Where(x => x.Order.Branch.BranchType == model.branchType);
                }
                if (model.orderId != null && model.orderId > 0)
                {
                    query = query.Where(x => x.OrderId == model.orderId /*&& x.Order.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail)*/);
                }
                if (model.status != null && model.status >= 0)
                {
                    if (User.IsInRole("leatherProductUser"))
                    {
                        query = query.Where(x => x.Product.WorkshopId2 == 5 || x.Product.ProductType == ProductType.OuterWerk || x.Product.ProductType == ProductType.WatchPendent2 || x.Product.ProductType == ProductType.Plaque);
                    }
                    if (!User.IsInRole("leatherProductUser") && !User.IsInRole("admin") && !User.IsInRole("allOrder"))
                    {
                        query = query.Where(x => x.Product.WorkshopId2 != 5 && x.Product.ProductType != ProductType.OuterWerk && x.Product.ProductType != ProductType.WatchPendent2 && x.Product.ProductType != ProductType.Plaque);
                    }
                }

                if (model.workshopOrderId != null && model.workshopOrderId > 0)
                {
                    query = query.Where(x => x.WorkshopOrderId == model.workshopOrderId);
                }
                if (model.workshopOrderList != null && model.workshopOrderList.Count() > 0)
                {
                    query = query.Where(x => model.workshopOrderList.Any(y => (y == x.WorkshopOrderId && x.SendWorkshopOrder2 != true) || (y == x.WorkshopOrderId2 && x.SendWorkshopOrder2 == true)));
                }
                if (!string.IsNullOrEmpty(model.term?.Trim()))
                {
                    query = query.Where(x => x.BranchLabel.Contains(model.term.Trim()) || x.Description.Contains(model.term.Trim()) || x.Customer.Contains(model.term.Trim()) || x.PhoneNumber.Contains(model.term.Trim()) || x.Product.Code.Contains(model.term.Trim()) || x.Product.BookCode.Contains(model.term.Trim()) || x.Product.Title.Contains(model.term.Trim()));
                }

                if (!string.IsNullOrEmpty(model.orderNumberTerm?.Trim()))
                {
                    query = query.Where(x => x.Order.OrderSerial.Contains(model.orderNumberTerm.Trim()));
                }

                if (!string.IsNullOrEmpty(model.orderCode))
                {
                    var orderCode = model.orderCode.Split('-');
                    query = query.Where(x => orderCode.Any(z => z.Contains(x.Order.OrderNumber)));
                }

                if (model.fromCode != null && model.fromCode > 0 && model.toCode == null)
                {
                    query = query.Where(x => x.OrderId >= model.fromCode);
                }
                if (model.toCode != null && model.toCode > 0 && model.fromCode == null)
                {
                    query = query.Where(x => x.OrderId <= model.toCode);
                }

                if (model.fromCode != null && model.fromCode > 0 && model.toCode != null && model.toCode > 0)
                {
                    query = query.Where(x => x.OrderId >= model.fromCode && x.OrderId <= model.toCode);
                    orderNumberList = query.GroupBy(x => x.Order).Select(x => x.Key.OrderNumber).ToList();
                }

                if (model.exceptOrderIdList?.Count > 0)
                {
                    query = query.Where(x => !model.exceptOrderIdList.Any(y => y == x.OrderId));

                    orderNumberList = query.GroupBy(x => x.Order).Select(x => x.Key.OrderNumber).ToList();
                }
                if (model.typeList != null && model.typeList.Count > 0)
                {
                    query = query.Where(x => model.typeList.Any(y => y == x.Product.ProductType));
                }
                if (model.branchTypeList != null && model.branchTypeList.Count > 0)
                {
                    query = query.Where(x => model.branchTypeList.Any(y => y == x.Order.Branch.BranchType));
                }
                if (model.status != null && model.status >= 0)
                {
                    query = query.Where(x => x.OrderDetailStatus == model.status);
                }
                if (model.setNumber != null)
                {
                    query = query.Where(x => x.SetNumber == model.setNumber);
                }

                if (model.workshopList != null && model.workshopList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => (x.SendWorkshopOrder2 != true && model.workshopList.Where(y => y > 0).Contains(x.Product.WorkshopId)) || (x.SendWorkshopOrder2 == true && model.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId2 || y == x.Product.WorkshopId)));
                }

                if (model.branchList != null && model.branchList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => model.branchList.Where(y => y > 0).Any(y => y == x.Order.BranchId));
                }

                if (!string.IsNullOrEmpty(model.date))
                {
                    var date = DateUtility.GetDateTime(model.date);
                    query = query.Where(x => x.CreateDate >= date && x.CreateDate <= DbFunctions.AddDays(date, 1));
                }
                else if (!string.IsNullOrEmpty(model.fromDate) || !string.IsNullOrEmpty(model.toDate))
                {
                    if (!string.IsNullOrEmpty(model.fromDate) && DateUtility.GetDateTime(model.fromDate) != null)
                    {
                        var fromDate = DateUtility.GetDateTime(model.fromDate).GetValueOrDefault();
                        query = query.Where(x => x.Order.CreateDate >= fromDate);
                    }

                    if (!string.IsNullOrEmpty(model.toDate) && DateUtility.GetDateTime(model.toDate) != null)
                    {
                        var toDate = DateUtility.GetDateTime(model.toDate).GetValueOrDefault();
                        query = query.Where(x => x.Order.CreateDate <= toDate);

                    }
                    if (!string.IsNullOrEmpty(model.toDate) && DateUtility.GetDateTime(model.toDate) != null && !string.IsNullOrEmpty(model.fromDate) && DateUtility.GetDateTime(model.fromDate) != null)
                    {
                        var orderDateList = query.GroupBy(x => DbFunctions.TruncateTime(x.Order.CreateDate)).Select(x => x.Key).OrderByDescending(x => x).ToList();
                        foreach (var item in orderDateList)
                        {
                            persianDateList.Add(DateUtility.GetPersianDate(item));
                        }
                        if (model.exceptPersianDateList != null && model.exceptPersianDateList.Count > 0)
                        {

                            List<DateTime?> newDateList = new List<DateTime?>();
                            foreach (var item in model.exceptPersianDateList)
                            {
                                newDateList.Add(DateUtility.GetDateTime(item));
                            }
                            persianDateList.Clear();
                            query = query.Where(x => !newDateList.Any(y => y == DbFunctions.TruncateTime(x.Order.CreateDate)));
                            var newPersianlist = query.GroupBy(x => DbFunctions.TruncateTime(x.Order.CreateDate)).Select(x => x.Key).OrderByDescending(x => x).ToList();
                            foreach (var item in newPersianlist)
                            {
                                persianDateList.Add(DateUtility.GetPersianDate(item));
                            }
                        }
                    }


                }

                switch (model.order)
                {
                    case "productType":
                        query = query.OrderBy(x => x.Product.ProductType);
                        break;
                    case "Vitrin":
                        query = query.OrderByDescending(x => x.OrderType == OrderType.None).ThenByDescending(x => x.Customer == null && x.OrderType == OrderType.Personalization);
                        break;
                    case "OrderVitrin":
                        query = query.OrderByDescending(x => x.Customer == null && x.OrderType == OrderType.Personalization).ThenByDescending(x => x.OrderType == OrderType.None);
                        break;
                    case "Customer":
                        query = query.OrderByDescending(x => x.Customer).ThenByDescending(x => x.OrderType == OrderType.Personalization);
                        break;
                    case "ForceOrder":
                        query = query.OrderByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenBy(x => x.OrderType == OrderType.None);
                        break;
                    case "DateAscending":
                        query = query.OrderBy(x => x.Id);
                        break;
                    case "SetNumber":
                        query = query.OrderByDescending(x => x.SetNumber).ThenByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenByDescending(x => x.SetNumber).ThenBy(x => x.OrderType == OrderType.None);
                        break;
                    //default:
                    //    query = query.OrderByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenByDescending(x => x.SetNumber).ThenBy(x => x.OrderType == OrderType.None);
                    //    break;
                    default:
                        query = query.OrderByDescending(x => x.Product.BookCode);
                        break;
                }
                dataCount = query.Count();
                allDataCount = query.Count();
                allWeight = query.Sum(x => x.Product.Weight);
                if (model.count > 0)
                {
                    query = query.Skip(model.page * model.count).Take(model.count);
                    pageCount = Math.Ceiling((double)dataCount / model.count);
                }
                if (model.status == OrderDetailStatus.InPreparation || model.status == OrderDetailStatus.OutOfConstruction)
                {
                    list = query.Select(item => new OrderDetailViewModel()
                    {
                        id = item.Id,
                        orderSerial = "KIA-" + item.Order.OrderSerial,
                        orderNumber = item.Order.OrderNumber,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order).FileName,
                        orderType = item.OrderType,
                        //orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        productSizeId = item.Product.SizeId,
                        workshopName = item.Product.Workshop.Name,
                        workshopName2 = item.Product.Workshop2.Name,
                        size = item.Size,
                        size2 = item.Size2,
                        setNumber = item.SetNumber,
                        goldType = item.GoldType,
                        productColor = item.ProductColor,
                        //goldTypeTitle = Enums.GetTitle(item.GoldType),
                        outerWerkType = item.OuterWerkType,
                        //outerWerkTypeTitle = Enums.GetTitle(item.OuterWerkType),
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        branchLabel = item.BranchLabel,
                        branchName = item.Order.Branch.Name,
                        description = item.Description,
                        description2 = item.Description2,
                        title = item.Product.Title,
                        code = item.Product.Code,
                        bookCode = item.Product.BookCode,
                        weight = item.Product.Weight,
                        count = item.Count,
                        orderDetailStatus = item.OrderDetailStatus,
                        //orderDetailStatusTitle = Enums.GetTitle(item.OrderDetailStatus),
                        createdUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        //createDate = DateUtility.GetPersianDate(item.CreateDate),
                        CreateDate = item.CreateDate,
                        relatedOrderDetailSerial = "KIA-" + item.RelatedOrderDetail.Order.OrderSerial,
                        countUnderConstruction = item.OrderDetailLogList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Count(),
                        stoneList = item.OrderDetailStoneList.Select(x => new OrderDetailStoneViewModel()
                        {
                            order = x.Order,
                            stoneName = x.Stone.Name
                        }).ToList(),
                        leatherList = item.OrderDetailLeatherList.Select(x => new OrderDetailLeatherViewModel()
                        {
                            order = x.Order,
                            leatherName = x.Leather.Name
                        }).ToList(),
                        returned = item.OrderDetailLogList.Count(x => x.Description == "مرجوعی" || x.Description == "مرجوعی به کارگاه اول" || x.Description == "مرجوعی به کارگاه دوم")
                    }).ToList();
                }
                else
                {
                    list = query.Select(item => new OrderDetailViewModel()
                    {
                        id = item.Id,
                        orderSerial = "KIA-" + item.Order.OrderSerial,
                        orderNumber = item.Order.OrderNumber,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        orderType = item.OrderType,
                        //orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        productSizeId = item.Product.SizeId,
                        workshopName = item.Product.Workshop.Name,
                        workshopName2 = item.Product.Workshop2.Name,
                        size = item.Size,
                        size2 = item.Size2,
                        setNumber = item.SetNumber,
                        goldType = item.GoldType,
                        productColor = item.ProductColor,
                        //goldTypeTitle = Enums.GetTitle(item.GoldType),
                        outerWerkType = item.OuterWerkType,
                        //outerWerkTypeTitle = Enums.GetTitle(item.OuterWerkType),
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        branchLabel = item.BranchLabel,
                        branchName = item.Order.Branch.Name,
                        description = item.Description,
                        description2 = item.Description2,
                        title = item.Product.Title,
                        code = item.Product.Code,
                        bookCode = item.Product.BookCode,
                        weight = item.Product.Weight,
                        count = item.Count,
                        orderDetailStatus = item.OrderDetailStatus,
                        //orderDetailStatusTitle = Enums.GetTitle(item.OrderDetailStatus),
                        createdUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        //createDate = DateUtility.GetPersianDate(item.CreateDate),
                        CreateDate = item.CreateDate,
                        relatedOrderDetailSerial = "KIA-" + item.RelatedOrderDetail.Order.OrderSerial,
                        countUnderConstruction = item.OrderDetailLogList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Count(),
                        stoneList = item.OrderDetailStoneList.Select(x => new OrderDetailStoneViewModel()
                        {
                            order = x.Order,
                            stoneName = x.Stone.Name
                        }).ToList(),
                        leatherList = item.OrderDetailLeatherList.Select(x => new OrderDetailLeatherViewModel()
                        {
                            order = x.Order,
                            leatherName = x.Leather.Name
                        }).ToList(),
                        returned = item.OrderDetailLogList.Count(x => x.Description == "مرجوعی" || x.Description == "مرجوعی به کارگاه اول" || x.Description == "مرجوعی به کارگاه دوم")
                    }).ToList();
                }

            }
            list.ForEach(x =>
            {
                x.orderTypeTitle = Enums.GetTitle(x.orderType);
                x.goldTypeTitle = Enums.GetTitle(x.goldType);
                x.productColorTitle = x.productColor >= 0 ? Enums.GetTitle(x.productColor) : "";
                x.outerWerkTypeTitle = Enums.GetTitle(x.outerWerkType);
                x.orderDetailStatusTitle = Enums.GetTitle(x.orderDetailStatus);
                x.createDate = DateUtility.GetPersianDate(x.CreateDate);
            });
            response = new Response()
            {
                data = new
                {
                    list = list,
                    count = dataCount,
                    allCount = allDataCount,
                    weight = allWeight,
                    allWeight = allWeight,
                    pageCount = pageCount,
                    page = model.page + 1,
                    orderNumberList = orderNumberList,
                    persianDateList = persianDateList
                },
                status = 200,
            };
            return response;
        }

        /// <summary>
        ///  گرفتن اطلاعات یک یا کل سفارشات برای اجرا کارگاه
        /// </summary>
        /// <param name="db">شی دیتابیس</param>
        /// <param name="model">مدلی که فیلتر های مورد نظر را در خود دارد</param>
        /// <returns>اطلاعات یک یا کل سفارشات برای اجرا</returns>
        public Response GetDataWork(OrderDetailSearchViewModel model)
        {
            List<OrderDetailViewModel> list;
            var orderNumberList = new List<string>();
            var persianDateList = new List<string>();
            int dataCount;
            double pageCount = 0;
            Response response;
            var currentUser = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                var query = db.OrderDetail.Include(x => x.Order).Include(x => x.Product).Include(x => x.Product.ProductFileList).Include(x => x.Product.Workshop).Include(x => x.OrderDetailStoneList).Include(x => x.OrderDetailLeatherList).Include(x => x.RelatedOrderDetail.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.CreateUser).Where(x => x.Order.Deleted == false);

                if (model.workshopOrderList != null && model.workshopOrderList.Count() > 0)
                {
                    query = query.Where(x => model.workshopOrderList.Any(y => (y == x.WorkshopOrderId && x.SendWorkshopOrder2 != true) || (y == x.WorkshopOrderId2 && x.SendWorkshopOrder2 == true)));
                }
                if (model.branch != null && model.branch > 0)
                {
                    query = query.Where(x => x.Order.BranchId == model.branch);
                }
                if (model.branchType != null && model.branchType >= 0)
                {
                    query = query.Where(x => x.Order.Branch.BranchType == model.branchType);
                }
                if (model.orderId != null && model.orderId > 0)
                {
                    query = query.Where(x => x.OrderId == model.orderId/* && x.Order.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail)*/);
                }

                if (model.workshopOrderId != null && model.workshopOrderId > 0)
                {
                    query = query.Where(x => (x.WorkshopOrderId == model.workshopOrderId && x.SendWorkshopOrder2 != true) || (x.WorkshopOrderId2 == model.workshopOrderId && x.SendWorkshopOrder2 == true));
                }

                if (!string.IsNullOrEmpty(model.term?.Trim()))
                {
                    query = query.Where(x => x.Order.OrderSerial.Contains(model.term.Trim()) || x.BranchLabel.Contains(model.term.Trim()) || x.Description.Contains(model.term.Trim()) || x.Customer.Contains(model.term.Trim()) || x.PhoneNumber.Contains(model.term.Trim()) || x.Product.Code.Contains(model.term.Trim()) || x.Product.BookCode.Contains(model.term.Trim()) || x.Product.Title.Contains(model.term.Trim()));
                }

                if (!string.IsNullOrEmpty(model.orderNumberTerm?.Trim()))
                {
                    query = query.Where(x => x.Order.OrderSerial.Contains(model.orderNumberTerm.Trim()));
                }

                if (!string.IsNullOrEmpty(model.orderCode))
                {
                    var orderCode = model.orderCode.Split('-');
                    query = query.Where(x => orderCode.Any(z => z.Contains(x.Order.OrderNumber)));
                }

                if (model.fromCode != null && model.fromCode > 0 && model.toCode == null)
                {
                    query = query.Where(x => x.OrderId >= model.fromCode);
                }
                if (model.toCode != null && model.toCode > 0 && model.fromCode == null)
                {
                    query = query.Where(x => x.OrderId <= model.toCode);
                }

                if (model.fromCode != null && model.fromCode > 0 && model.toCode != null && model.toCode > 0)
                {
                    query = query.Where(x => x.OrderId >= model.fromCode && x.OrderId <= model.toCode);
                    orderNumberList = query.GroupBy(x => x.Order).Select(x => x.Key.OrderNumber).ToList();
                }

                if (model.exceptOrderIdList?.Count > 0)
                {
                    query = query.Where(x => !model.exceptOrderIdList.Any(y => y == x.OrderId));

                    orderNumberList = query.GroupBy(x => x.Order).Select(x => x.Key.OrderNumber).ToList();
                }
                if (!string.IsNullOrEmpty(model.date))
                {
                    var date = DateUtility.GetDateTime(model.date);
                    query = query.Where(x => x.CreateDate >= date && x.CreateDate <= DbFunctions.AddDays(date, 1));
                }
                else if (!string.IsNullOrEmpty(model.fromDate) || !string.IsNullOrEmpty(model.toDate))
                {
                    if (!string.IsNullOrEmpty(model.fromDate) && DateUtility.GetDateTime(model.fromDate) != null)
                    {
                        var fromDate = DateUtility.GetDateTime(model.fromDate).GetValueOrDefault();
                        query = query.Where(x => x.Order.CreateDate >= fromDate);
                    }

                    if (!string.IsNullOrEmpty(model.toDate) && DateUtility.GetDateTime(model.toDate) != null)
                    {
                        var toDate = DateUtility.GetDateTime(model.toDate).GetValueOrDefault();
                        query = query.Where(x => x.Order.CreateDate <= toDate);

                    }
                    if (!string.IsNullOrEmpty(model.toDate) && DateUtility.GetDateTime(model.toDate) != null && !string.IsNullOrEmpty(model.fromDate) && DateUtility.GetDateTime(model.fromDate) != null)
                    {
                        var orderDateList = query.GroupBy(x => DbFunctions.TruncateTime(x.Order.CreateDate)).Select(x => x.Key).OrderByDescending(x => x).ToList();
                        foreach (var item in orderDateList)
                        {
                            persianDateList.Add(DateUtility.GetPersianDate(item));
                        }
                        if (model.exceptPersianDateList != null && model.exceptPersianDateList.Count > 0)
                        {

                            List<DateTime?> newDateList = new List<DateTime?>();
                            foreach (var item in model.exceptPersianDateList)
                            {
                                newDateList.Add(DateUtility.GetDateTime(item));
                            }
                            persianDateList.Clear();
                            query = query.Where(x => !newDateList.Any(y => y == DbFunctions.TruncateTime(x.Order.CreateDate)));
                            var newPersianlist = query.GroupBy(x => DbFunctions.TruncateTime(x.Order.CreateDate)).Select(x => x.Key).OrderByDescending(x => x).ToList();
                            foreach (var item in newPersianlist)
                            {
                                persianDateList.Add(DateUtility.GetPersianDate(item));
                            }
                        }
                    }


                }
                if (model.typeList != null && model.typeList.Count > 0)
                {
                    query = query.Where(x => model.typeList.Any(y => y == x.Product.ProductType));
                }
                if (model.branchTypeList != null && model.branchTypeList.Count > 0)
                {
                    query = query.Where(x => model.branchTypeList.Any(y => y == x.Order.Branch.BranchType));
                }

                if (model.status != null)
                {
                    if (model.status == OrderDetailStatus.OutOfConstruction)
                    {
                        if (currentUser.WorkshopId == 5)
                        {
                            query = query.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction2);
                        }
                        else
                        {
                            query = query.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction);
                        }
                    }
                    else if (model.status == OrderDetailStatus.UnderConstruction)
                    {
                        if (currentUser.WorkshopId == 5)
                        {
                            query = query.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction2);
                        }
                        else
                        {
                            query = query.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction);
                        }
                    }
                    else
                    {
                        query = query.Where(x => x.OrderDetailStatus == model.status);
                    }
                }

                if (model.setNumber != null)
                {
                    query = query.Where(x => x.SetNumber == model.setNumber);
                }

                if (model.workshopList != null && model.workshopList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => (x.SendWorkshopOrder2 != true && model.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId)) || (x.SendWorkshopOrder2 == true && model.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId2)));
                }

                if (model.branchList != null && model.branchList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => model.branchList.Where(y => y > 0).Any(y => y == x.Order.BranchId));
                }

                if (!string.IsNullOrEmpty(model.date))
                {
                    var date = DateUtility.GetDateTime(model.date);
                    query = query.Where(x => x.CreateDate >= date && x.CreateDate <= DbFunctions.AddDays(date, 1));
                }
                else if (!string.IsNullOrEmpty(model.fromDate) || !string.IsNullOrEmpty(model.toDate))
                {
                    if (!string.IsNullOrEmpty(model.fromDate) && DateUtility.GetDateTime(model.fromDate) != null)
                    {
                        var fromDate = DateUtility.GetDateTime(model.fromDate).GetValueOrDefault();
                        query = query.Where(x => x.Order.CreateDate >= fromDate);
                    }

                    if (!string.IsNullOrEmpty(model.toDate) && DateUtility.GetDateTime(model.toDate) != null)
                    {
                        var toDate = DateUtility.GetDateTime(model.toDate).GetValueOrDefault();
                        query = query.Where(x => x.Order.CreateDate <= toDate);
                    }
                }

                switch (model.order)
                {
                    case "productType":
                        query = query.OrderBy(x => x.Product.ProductType);
                        break;
                    case "Vitrin":
                        query = query.OrderByDescending(x => x.OrderType == OrderType.None).ThenByDescending(x => x.Customer == null && x.OrderType == OrderType.Personalization);
                        break;
                    case "OrderVitrin":
                        query = query.OrderByDescending(x => x.Customer == null && x.OrderType == OrderType.Personalization).ThenByDescending(x => x.OrderType == OrderType.None);
                        break;
                    case "Customer":
                        query = query.OrderByDescending(x => x.Customer).ThenByDescending(x => x.OrderType == OrderType.Personalization);
                        break;
                    case "ForceOrder":
                        query = query.OrderByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenBy(x => x.OrderType == OrderType.None);
                        break;
                    case "DateAscending":
                        query = query.OrderBy(x => x.Id);
                        break;
                    case "SetNumber":
                        query = query.OrderByDescending(x => x.SetNumber).ThenByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenByDescending(x => x.SetNumber).ThenBy(x => x.OrderType == OrderType.None);
                        break;
                    default:
                        query = query.OrderByDescending(x => x.ForceOrder).ThenByDescending(x => x.Customer).ThenByDescending(x => x.SetNumber).ThenBy(x => x.OrderType == OrderType.None);
                        break;
                }
                if (User.IsInRole("nonLeatherOrder"))
                {
                    query = query.Where(x => x.Product.ProductType != ProductType.LeatherBracelet && x.Product.ProductType != ProductType.RailBracelet && x.Product.WorkshopId2 != 5);
                }
                if (User.IsInRole("leatherOrder"))
                {
                    query = query.Where(x => x.Product.ProductType == ProductType.LeatherBracelet && x.Product.ProductType == ProductType.RailBracelet);
                }
                dataCount = query.Count();
                if (model.count > 0)
                {
                    query = query.Skip(model.page * model.count).Take(model.count);
                    pageCount = Math.Ceiling((double)dataCount / model.count);
                }
                if (model.status == OrderDetailStatus.InPreparation || model.status == OrderDetailStatus.OutOfConstruction)
                {
                    list = query.Select(item => new OrderDetailViewModel()
                    {
                        id = item.Id,
                        orderSerial = "KIA-" + item.Order.OrderSerial,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order).FileName,
                        orderType = item.OrderType,
                        //orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        productSizeId = item.Product.SizeId,
                        workshopName = item.Product.Workshop.Name,
                        workshopName2 = item.Product.Workshop2.Name,
                        size = item.Size,
                        size2 = item.Size2,
                        setNumber = item.SetNumber,
                        goldType = item.GoldType,
                        productColor = item.ProductColor,
                        //goldTypeTitle = Enums.GetTitle(item.GoldType),
                        outerWerkType = item.OuterWerkType,
                        //outerWerkTypeTitle = Enums.GetTitle(item.OuterWerkType),
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        branchLabel = item.BranchLabel,
                        description = item.Description,
                        description2 = item.Description2,
                        title = item.Product.Title,
                        code = item.Product.Code,
                        bookCode = item.Product.BookCode,
                        weight = item.Product.Weight,
                        count = item.Count,
                        orderDetailStatus = item.OrderDetailStatus,
                        //orderDetailStatusTitle = Enums.GetTitle(item.OrderDetailStatus),
                        createdUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        //createDate = DateUtility.GetPersianDate(item.CreateDate),
                        CreateDate = item.CreateDate,
                        relatedOrderDetailSerial = "KIA-" + item.RelatedOrderDetail.Order.OrderSerial,
                        countUnderConstruction = item.OrderDetailLogList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Count(),
                        stoneList = item.OrderDetailStoneList.Select(x => new OrderDetailStoneViewModel()
                        {
                            order = x.Order,
                            stoneName = x.Stone.Name
                        }).ToList(),
                        leatherList = item.OrderDetailLeatherList.Select(x => new OrderDetailLeatherViewModel()
                        {
                            order = x.Order,
                            leatherName = x.Leather.Name
                        }).ToList(),
                        returned = item.OrderDetailLogList.Count(x => x.Description == "مرجوعی" || x.Description == "مرجوعی به کارگاه اول" || x.Description == "مرجوعی به کارگاه دوم")
                    }).ToList();
                }
                else
                {
                    list = query.Select(item => new OrderDetailViewModel()
                    {
                        id = item.Id,
                        orderSerial = "KIA-" + item.Order.OrderSerial,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        orderType = item.OrderType,
                        //orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        productSizeId = item.Product.SizeId,
                        workshopName = item.Product.Workshop.Name,
                        workshopName2 = item.Product.Workshop2.Name,
                        size = item.Size,
                        size2 = item.Size2,
                        setNumber = item.SetNumber,
                        goldType = item.GoldType,
                        productColor = item.ProductColor,
                        //goldTypeTitle = Enums.GetTitle(item.GoldType),
                        outerWerkType = item.OuterWerkType,
                        //outerWerkTypeTitle = Enums.GetTitle(item.OuterWerkType),
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        branchLabel = item.BranchLabel,
                        description = item.Description,
                        description2 = item.Description2,
                        title = item.Product.Title,
                        code = item.Product.Code,
                        bookCode = item.Product.BookCode,
                        weight = item.Product.Weight,
                        count = item.Count,
                        orderDetailStatus = item.OrderDetailStatus,
                        //orderDetailStatusTitle = Enums.GetTitle(item.OrderDetailStatus),
                        createdUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        //createDate = DateUtility.GetPersianDate(item.CreateDate),
                        CreateDate = item.CreateDate,
                        relatedOrderDetailSerial = "KIA-" + item.RelatedOrderDetail.Order.OrderSerial,
                        countUnderConstruction = item.OrderDetailLogList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Count(),
                        stoneList = item.OrderDetailStoneList.Select(x => new OrderDetailStoneViewModel()
                        {
                            order = x.Order,
                            stoneName = x.Stone.Name
                        }).ToList(),
                        leatherList = item.OrderDetailLeatherList.Select(x => new OrderDetailLeatherViewModel()
                        {
                            order = x.Order,
                            leatherName = x.Leather.Name
                        }).ToList(),
                        returned = item.OrderDetailLogList.Count(x => x.Description == "مرجوعی" || x.Description == "مرجوعی به کارگاه اول" || x.Description == "مرجوعی به کارگاه دوم")
                    }).ToList();
                }

            }
            list.ForEach(x =>
            {
                x.orderTypeTitle = Enums.GetTitle(x.orderType);
                x.goldTypeTitle = Enums.GetTitle(x.goldType);
                x.productColorTitle = x.productColor >= 0 ? Enums.GetTitle(x.productColor) : "";
                x.outerWerkTypeTitle = Enums.GetTitle(x.outerWerkType);
                x.orderDetailStatusTitle = Enums.GetTitle(x.orderDetailStatus);
                x.createDate = DateUtility.GetPersianDate(x.CreateDate);
            });
            response = new Response()
            {
                data = new
                {
                    list = list,
                    count = dataCount,
                    weight = list.Sum(x => x.weight),
                    pageCount = pageCount,
                    page = model.page + 1,
                    orderNumberList = orderNumberList,
                    persianDateList = persianDateList
                },
                status = 200,
            };
            return response;
        }


        /// <summary>
        /// لیست محصولات یک سفارش جهت ویرایش و تغییر وضعیت
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست محصولات یک سفارش</returns>
        [Authorize(Roles = "admin, admin-order")]
        public JsonResult ManipulateDetailList(OrderDetailSearchViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                response = GetData(model);
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// لیست محصولات یک سفارش جهت ویرایش و تغییر وضعیت به همراه صفحه بندی
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست محصولات یک سفارش</returns>
        [Authorize(Roles = "admin, admin-order")]
        public JsonResult ManipulateDetailListPaging(OrderDetailSearchViewModel model)
        {
            Response response;
            try
            {
                response = GetData(model);
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// گرفتن شی برای ارسال به سمت کاربر
        /// </summary>
        /// <param name="list">لیست سفارشات</param>
        /// <param name="dataCount">تعداد</param>
        /// <returns>شی آماده برای بازگرداندن به کاربر</returns>
        private static object GetResponse(List<OrderDetail> list, int dataCount)
        {
            var data = list.Select(item => new
            {
                id = item.Id,
                orderSerial = "KIA-" + item.Order.OrderSerial,
                fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack)?.FileName,
                orderType = item.OrderType,
                orderTypeTitle = Enums.GetTitle(item.OrderType),
                productId = item.ProductId,
                productSizeId = item.Product.SizeId,
                workshopName = item.Product.Workshop.Name,
                size = item.Size,
                setNumber = item.SetNumber,
                goldType = item.GoldType,
                goldTypeTitle = Enums.GetTitle(item.GoldType),
                outerWerkType = item.OuterWerkType,
                outerWerkTypeTitle = Enums.GetTitle(item.OuterWerkType),
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
                orderDetailStatus = item.OrderDetailStatus,
                orderDetailStatusTitle = Enums.GetTitle(item.OrderDetailStatus),
                createdUser = item.CreateUser.FullName,
                createDate = DateUtility.GetPersianDate(item.CreateDate),
                relatedOrderDetailSerial = "KIA-" + item.RelatedOrderDetail?.Order.OrderSerial,
                stoneList = item.OrderDetailStoneList?.Select(x => new
                {
                    order = x.Order,
                    stoneName = x.Stone?.Name
                }).ToList(),
                leatherList = item.OrderDetailLeatherList?.Select(x => new
                {
                    order = x.Order,
                    leatherName = x.Leather?.Name
                }).ToList()
            }).ToList();

            return data;
        }

        /// <summary>
        /// تغییر وضعیت محصولات
        /// </summary>
        /// <param name="model">مدلی شامل ردیف های محصولات یک سفارش و وثعیتی که می بایست به آن تغییر کنند</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, admin-order")]
        public JsonResult ChangeStatus(OrderDetailChangeStatusViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    if (model.orderId != null && model.orderId > 0) // در صورتی که ردیف یک سفارش یکتا به سمت سرور پاس داده شود برای ساخت سفارش نیازی به لود کردن اطلاعات نیست
                    {
                        int orderId = model.orderId.GetValueOrDefault();
                        Order order = db.Order.Single(x => x.Id == orderId);
                        WorkshopOrder workshopOrder = null;
                        if (model.status == OrderDetailStatus.InWorkshop)
                        {
                            WorkshopOrderNoModel workshopOrderNoModel = GetNewWorkshopOrderNo(db, orderId);
                            workshopOrder = new WorkshopOrder()
                            {
                                OrderId = model.orderId.GetValueOrDefault(),
                                WorkshopOrderSerial = workshopOrderNoModel.workshopOrderSerial,
                                WorkshopOrderNumber = workshopOrderNoModel.WorkshopOrderNumber,
                                CreateUserId = user.Id,
                                ModifyUserId = user.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };
                            db.WorkshopOrder.Add(workshopOrder);
                            //db.SaveChanges();
                            //workshopOrder.WorkshopOrderSerial = order.OrderSerial + "-WS" + workshopOrder.Id;
                            //workshopOrder.WorkshopOrderNumber = order.OrderNumber + "-" + workshopOrder.Id;
                        }

                        if (model.status == OrderDetailStatus.InWorkshop2)
                        {
                            var notWorkshopId2 = db.OrderDetail.Where(x => x.Product.WorkshopId2 == null && model.id.Any(y => y == x.Id)).ToList();
                            if (notWorkshopId2 != null && notWorkshopId2.Count() > 0)
                            {
                                response = new Response()
                                {
                                    status = 500,
                                    message = "شما وضعیت ارسال به کارگاه دوم انتخاب کرده اید که در لیست انتخابی بعضی از محصولات کارگاه دوم ندارد لطفا درست انتخاب کنید."
                                };
                                return Json(response, JsonRequestBehavior.AllowGet);
                            }
                            WorkshopOrderNoModel workshopOrderNoModel = GetNewWorkshopOrderNoSecond(db, orderId);
                            workshopOrder = new WorkshopOrder()
                            {
                                OrderId = model.orderId.GetValueOrDefault(),
                                WorkshopOrderSerial = workshopOrderNoModel.workshopOrderSerial,
                                WorkshopOrderNumber = workshopOrderNoModel.WorkshopOrderNumber,
                                CreateUserId = user.Id,
                                ModifyUserId = user.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };
                            db.WorkshopOrder.Add(workshopOrder);
                            //db.SaveChanges();
                            //workshopOrder.WorkshopOrderSerial = order.OrderSerial + "-WS" + workshopOrder.Id;
                            //workshopOrder.WorkshopOrderNumber = order.OrderNumber + "-" + workshopOrder.Id;
                        }

                        foreach (var item in model.id)
                        {
                            var orderDetail = db.OrderDetail.Single(x => x.Id == item);
                            if (orderDetail.RelatedOrderDetailId == null)
                            {
                                orderDetail.OrderDetailStatus = model.status;
                                orderDetail.ModifyUserId = user.Id;
                                orderDetail.ModifyDate = DateTime.Now;
                                orderDetail.Ip = Request.UserHostAddress;

                                if (model.status == OrderDetailStatus.InWorkshop)
                                    orderDetail.WorkshopOrder = workshopOrder;
                                //if (model.status == OrderDetailStatus.InWorkshop2)
                                //    orderDetail.WorkshopOrder2 = workshopOrder;
                                if (model.status == OrderDetailStatus.InWorkshop || model.status == OrderDetailStatus.UnderConstruction || model.status == OrderDetailStatus.OutOfConstruction || model.status == OrderDetailStatus.Registered)
                                {
                                    orderDetail.SendWorkshopOrder2 = false;
                                }
                                if (model.status == OrderDetailStatus.InWorkshop2 && orderDetail.Product.WorkshopId2 > 0)
                                {
                                    orderDetail.WorkshopOrder2 = workshopOrder;
                                    orderDetail.SendWorkshopOrder2 = true;
                                }
                                if (model.status == OrderDetailStatus.Registered)
                                {
                                    orderDetail.WorkshopOrderId = null;
                                    orderDetail.WorkshopOrderId2 = null;
                                    orderDetail.SendWorkshopOrder2 = false;
                                }
                                var detailLog = new OrderDetailLog()
                                {
                                    OrderDetailId = orderDetail.Id,
                                    OrderDetailStatus = model.status,
                                    OrderDetailLogReasonId = model.reasonId,
                                    Description = model.description,
                                    CreateUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };

                                db.OrderDetailLog.Add(detailLog);
                            }
                        }
                    }
                    else
                    {
                        var orderDetailList = db.OrderDetail.Where(x => x.RelatedOrderDetailId == null && model.id.Any(y => y == x.Id)).ToList();
                        var groupOrderDetailList = orderDetailList.GroupBy(x => x.OrderId);
                        foreach (var orderItem in groupOrderDetailList)
                        {
                            WorkshopOrder workshopOrder = null;
                            Order order = db.Order.Single(x => x.Id == orderItem.Key);
                            if (model.status == OrderDetailStatus.InWorkshop)
                            {
                                WorkshopOrderNoModel workshopOrderNoModel = GetNewWorkshopOrderNo(db, orderItem.Key);
                                workshopOrder = new WorkshopOrder()
                                {
                                    OrderId = orderItem.Key,
                                    WorkshopOrderSerial = workshopOrderNoModel.workshopOrderSerial,
                                    WorkshopOrderNumber = workshopOrderNoModel.WorkshopOrderNumber,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };
                                db.WorkshopOrder.Add(workshopOrder);
                                //db.SaveChanges();
                                //workshopOrder.WorkshopOrderSerial = order.OrderSerial + "-WS" + workshopOrder.Id;
                                //workshopOrder.WorkshopOrderNumber = order.OrderNumber + "-" + workshopOrder.Id;
                            }
                            if (model.status == OrderDetailStatus.InWorkshop2)
                            {
                                var notWorkshopId2 = db.OrderDetail.Where(x => x.Product.WorkshopId2 == null && model.id.Any(y => y == x.Id)).ToList();
                                if (notWorkshopId2 != null && notWorkshopId2.Count() > 0)
                                {
                                    response = new Response()
                                    {
                                        status = 500,
                                        message = "شما وضعیت ارسال به کارگاه دوم انتخاب کرده اید که در لیست انتخابی بعضی از محصولات کارگاه دوم ندارد لطفا درست انتخاب کنید."
                                    };
                                    return Json(response, JsonRequestBehavior.AllowGet);
                                }
                                WorkshopOrderNoModel workshopOrderNoModel = GetNewWorkshopOrderNoSecond(db, orderItem.Key);
                                workshopOrder = new WorkshopOrder()
                                {
                                    OrderId = orderItem.Key,
                                    WorkshopOrderSerial = workshopOrderNoModel.workshopOrderSerial,
                                    WorkshopOrderNumber = workshopOrderNoModel.WorkshopOrderNumber,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };
                                db.WorkshopOrder.Add(workshopOrder);
                                db.SaveChanges();
                                workshopOrder.WorkshopOrderSerial = order.OrderSerial + "-WS" + workshopOrder.Id;
                                workshopOrder.WorkshopOrderNumber = order.OrderNumber + "-" + workshopOrder.Id;
                            }

                            foreach (var detailItem in orderItem)
                            {
                                var orderDetail = db.OrderDetail.Single(x => x.Id == detailItem.Id);
                                orderDetail.OrderDetailStatus = model.status;
                                orderDetail.ModifyUserId = user.Id;
                                orderDetail.ModifyDate = DateTime.Now;
                                orderDetail.Ip = Request.UserHostAddress;

                                if (model.status == OrderDetailStatus.InWorkshop)
                                    orderDetail.WorkshopOrder = workshopOrder;
                                if (model.status == OrderDetailStatus.InWorkshop || model.status == OrderDetailStatus.UnderConstruction || model.status == OrderDetailStatus.OutOfConstruction)
                                {
                                    orderDetail.SendWorkshopOrder2 = false;
                                }
                                if (model.status == OrderDetailStatus.InWorkshop2 && orderDetail.Product.WorkshopId2 > 0)
                                {
                                    orderDetail.WorkshopOrder2 = workshopOrder;
                                    orderDetail.SendWorkshopOrder2 = true;
                                }
                                if (model.status == OrderDetailStatus.Registered)
                                {
                                    orderDetail.WorkshopOrderId = null;
                                    orderDetail.WorkshopOrderId2 = null;
                                    orderDetail.SendWorkshopOrder2 = false;
                                }

                                var detailLog = new OrderDetailLog()
                                {
                                    OrderDetailId = orderDetail.Id,
                                    OrderDetailStatus = model.status,
                                    OrderDetailLogReasonId = model.reasonId,
                                    Description = model.description,
                                    CreateUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };
                                db.OrderDetailLog.Add(detailLog);
                                if (user.BranchType == BranchType.Branch)
                                {
                                    if (model.status == OrderDetailStatus.ReadyForDelivery)
                                    {
                                        orderDetail.OrderDetailStatus = OrderDetailStatus.Sent;
                                        var detailLog1 = new OrderDetailLog()
                                        {
                                            OrderDetailId = orderDetail.Id,
                                            OrderDetailStatus = OrderDetailStatus.Sent,
                                            OrderDetailLogReasonId = model.reasonId,
                                            Description = model.description,
                                            CreateUserId = user.Id,
                                            CreateDate = DateTime.Now,
                                            Ip = Request.UserHostAddress
                                        };
                                        db.OrderDetailLog.Add(detailLog1);
                                    }
                                }

                            }
                        }
                    }
                    db.SaveChanges();
                    if (model.status == OrderDetailStatus.InWorkshop || model.status == OrderDetailStatus.OutOfConstruction || model.status == OrderDetailStatus.UnderConstruction || model.status == OrderDetailStatus.Registered)
                    {
                        var removeWorkshopOrderList = db.WorkshopOrder.Where(x => x.OrderDetailList.Count() == 0 && x.OrderDetailList2.Count() == 0).ToList();
                        if (removeWorkshopOrderList != null && removeWorkshopOrderList.Count > 0)
                        {
                            db.WorkshopOrder.RemoveRange(removeWorkshopOrderList);
                            db.SaveChanges();
                        }
                    }
                }

                response = new Response()
                {
                    status = 200,
                    message = "تغییر وضعیت با موفقیت انجام شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ساخت سفارش کسری برای یک سفارش
        /// </summary>
        /// <param name="orderId">ساخت سفارش کسری از محصولات کسری خورده</param>
        /// <returns>نتیجه ساخت سفارش کسری</returns>
        [HttpPost]
        [Authorize(Roles = "admin, admin-order")]
        public JsonResult MakeShortageOrder(int orderId)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var order = db.Order.Single(x => x.Id == orderId && x.Deleted == false);
                    if (order.OrderDetailList.Count(x => x.OrderDetailStatus == OrderDetailStatus.Shortage) > 0)
                    {
                        #region MakeOrder

                        //var orderSerial = GetNewShortageOrderNo(db, order);
                        var shortageOrder = new Order()
                        {
                            BranchId = order.BranchId,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        shortageOrder.OrderDetailList = new List<OrderDetail>();

                        order.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).ToList().ForEach(x =>
                        {
                            var orderDetailItem = new OrderDetail()
                            {
                                Order = shortageOrder,
                                ProductId = x.ProductId,
                                Product = x.Product,
                                OrderType = x.OrderType,
                                OrderDetailStatus = x.Product.Workshop.AutoConfirm ? OrderDetailStatus.InWorkshop : OrderDetailStatus.Registered,
                                Size = x.Size,
                                GoldType = x.GoldType,
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
                                OrderDetailStoneList = x.OrderDetailStoneList?.Select(y => new OrderDetailStone()
                                {
                                    Order = y.Order,
                                    StoneId = y.StoneId
                                }).ToList(),
                                OrderDetailLeatherList = x.OrderDetailLeatherList?.Select(y => new OrderDetailLeather()
                                {
                                    Order = y.Order,
                                    LeatherId = y.LeatherId
                                }).ToList(),
                                OrderDetailLogList = x.OrderDetailLogList.Where(y => y.OrderDetailStatus == OrderDetailStatus.Shortage)?.Select(z => new OrderDetailLog()
                                {
                                    OrderDetailStatus = x.Product.Workshop.AutoConfirm ? OrderDetailStatus.InWorkshop : OrderDetailStatus.Registered,
                                    OrderDetailLogReasonId = z.OrderDetailLogReasonId,
                                    Description = z.Description,
                                    CreateUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                }).ToList()
                            };

                            x.OrderDetailStatus = OrderDetailStatus.ShortageOrder;
                            x.OrderDetailLogList.Add(new OrderDetailLog()
                            {
                                OrderDetail = x,
                                OrderDetailStatus = OrderDetailStatus.ShortageOrder,
                                Description = x.OrderDetailLogList.Last(y => y.OrderDetailStatus == OrderDetailStatus.Shortage).Description,
                                CreateUserId = user.Id,
                                CreateDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            });

                            x.RelatedOrderDetail = orderDetailItem;

                            shortageOrder.OrderDetailList.Add(orderDetailItem);
                        });

                        shortageOrder.OrderLogList.Add(new OrderLog()
                        {
                            Order = shortageOrder,
                            OrderStatus = OrderStatus.Normal,
                            CreateUserId = user.Id,
                            CreateDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        });



                        db.Order.Add(shortageOrder);
                        db.SaveChanges();
                        shortageOrder.OrderSerial = order.OrderSerial + "-KS" + shortageOrder.Id;
                        shortageOrder.OrderNumber = shortageOrder.Id.ToString();
                        var autoConfirmList = shortageOrder.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).ToList();
                        if (autoConfirmList.Count > 0)
                        {
                            WorkshopOrder wsOrder = new WorkshopOrder()
                            {
                                Order = shortageOrder,
                                CreateUserId = user.Id,
                                ModifyUserId = user.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress,
                            };

                            db.WorkshopOrder.Add(wsOrder);
                            db.SaveChanges();
                            wsOrder.WorkshopOrderSerial = order.OrderSerial + "-KS" + shortageOrder.Id + "-WS" + wsOrder.Id;
                            wsOrder.WorkshopOrderNumber = shortageOrder.Id + "-WS" + wsOrder.Id;
                            autoConfirmList.ForEach(x => x.WorkshopOrder = wsOrder);
                        }

                        db.SaveChanges();
                        #endregion

                        response = new Response()
                        {
                            status = 200,
                            message = "سفارش کسری با موفقیت ایجاد شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "محصولی جهت ثبت سفارش کسری یافت نشد."
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

        [HttpPost]
        [Authorize(Roles = "admin, admin-order")]
        public JsonResult MakeShortageOrderDetail(List<int> orderDetailIdList)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var orderDetailList = db.OrderDetail.Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.OrderDetailStoneList).Include(x => x.OrderDetailLeatherList).Include(x => x.OrderDetailLogList).Where(x => x.RelatedOrderDetailId == null && orderDetailIdList.Any(y => y == x.Id)).ToList();
                    if (orderDetailList.Count(x => x.OrderDetailStatus == OrderDetailStatus.Shortage) > 0)
                    {
                        #region MakeOrder

                        var orderList = orderDetailList.GroupBy(x => x.OrderId);
                        foreach (var order in orderList)
                        {
                            //var orderSerial = GetNewShortageOrderNo(db, order.First().Order);
                            var shortageOrder = new Order()
                            {
                                BranchId = order.First().Order.BranchId,
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };

                            shortageOrder.OrderDetailList = new List<OrderDetail>();

                            foreach (var detailItem in order)
                            {
                                var orderDetailItem = new OrderDetail()
                                {
                                    Order = shortageOrder,
                                    ProductId = detailItem.ProductId,
                                    Product = detailItem.Product,
                                    OrderType = detailItem.OrderType,
                                    OrderDetailStatus = detailItem.Product.Workshop.AutoConfirm ? OrderDetailStatus.InWorkshop : OrderDetailStatus.Registered,
                                    Size = detailItem.Size,
                                    GoldType = detailItem.GoldType,
                                    LeatherLoop = detailItem.LeatherLoop,
                                    Customer = detailItem.Customer,
                                    PhoneNumber = detailItem.PhoneNumber,
                                    ForceOrder = detailItem.ForceOrder,
                                    BranchLabel = detailItem.BranchLabel,
                                    Description = detailItem.Description,
                                    Count = detailItem.Count,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress,
                                    OrderDetailStoneList = detailItem.OrderDetailStoneList?.Select(y => new OrderDetailStone()
                                    {
                                        Order = y.Order,
                                        StoneId = y.StoneId
                                    }).ToList(),
                                    OrderDetailLeatherList = detailItem.OrderDetailLeatherList?.Select(y => new OrderDetailLeather()
                                    {
                                        Order = y.Order,
                                        LeatherId = y.LeatherId
                                    }).ToList(),
                                    OrderDetailLogList = detailItem.OrderDetailLogList.Where(y => y.OrderDetailStatus == OrderDetailStatus.Shortage).Select(z => new OrderDetailLog()
                                    {
                                        OrderDetailStatus = detailItem.Product.Workshop.AutoConfirm ? OrderDetailStatus.InWorkshop : OrderDetailStatus.Registered,
                                        Description = z.Description,
                                        OrderDetailLogReasonId = z.OrderDetailLogReasonId,
                                        CreateUserId = user.Id,
                                        CreateDate = DateTime.Now,
                                        Ip = Request.UserHostAddress
                                    }).ToList(),
                                };

                                detailItem.OrderDetailStatus = OrderDetailStatus.ShortageOrder;
                                detailItem.OrderDetailLogList.Add(new OrderDetailLog()
                                {
                                    OrderDetail = detailItem,
                                    OrderDetailStatus = OrderDetailStatus.ShortageOrder,
                                    CreateUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                });
                                detailItem.RelatedOrderDetail = orderDetailItem;
                                shortageOrder.OrderDetailList.Add(orderDetailItem);
                            }

                            shortageOrder.OrderLogList.Add(new OrderLog()
                            {
                                Order = shortageOrder,
                                OrderStatus = OrderStatus.Normal,
                                CreateUserId = user.Id,
                                CreateDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            });



                            db.Order.Add(shortageOrder);
                            db.SaveChanges();
                            shortageOrder.OrderSerial = order.First().Order.OrderSerial + "-KS" + shortageOrder.Id;
                            shortageOrder.OrderNumber = shortageOrder.Id.ToString();
                            var autoConfirmList = shortageOrder.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).ToList();
                            if (autoConfirmList.Count > 0)
                            {
                                WorkshopOrder wsOrder = new WorkshopOrder()
                                {
                                    Order = shortageOrder,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress,
                                };

                                db.WorkshopOrder.Add(wsOrder);
                                db.SaveChanges();
                                wsOrder.WorkshopOrderSerial = order.First().Order.OrderSerial + "-KS" + shortageOrder.Id + "-WS" + wsOrder;
                                wsOrder.WorkshopOrderNumber = shortageOrder.Id + "-" + wsOrder;
                                autoConfirmList.ForEach(x => x.WorkshopOrder = wsOrder);
                            }
                        }

                        db.SaveChanges();
                        #endregion

                        response = new Response()
                        {
                            status = 200,
                            message = "سفارش کسری با موفقیت ایجاد شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "محصولی جهت ثبت سفارش کسری یافت نشد."
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
        /// لیست شعبه هایی که در یک تاریخ سفارش ثبت کرده اند
        /// </summary>
        /// <param name="dateTime">تاریخ</param>
        /// <returns>لیست شعبه هایی که اقدام به ثبت سفارش در تاریخ مشخص کرده اند</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-order")]
        public JsonResult GetBranchRegisteredOrder(string dateTime)
        {
            Response response;
            try
            {
                List<int> list;
                using (var db = new KiaGalleryContext())
                {
                    var date = DateUtility.GetDateTime(dateTime);
                    list = db.Order.Where(x => x.Deleted == false && x.CreateDate >= date && x.CreateDate <= DbFunctions.AddDays(date, 1)).Select(x => x.Branch.Id).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list
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
        /// حذف محصولات و تمام سابقه ها و خود سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>نتیجه عملیات حذف یک سفارش</returns>
        [Authorize(Roles = "admin, admin-order-delete")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Order.Find(id);
                    item.Deleted = true;
                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "سفارش با موفقیت حذف شد."
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
        /// فعال سازی محصولات و تمام سابقه ها و خود سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>نتیجه عملیات فعال سازی یک سفارش</returns>
        [Authorize(Roles = "admin, admin-order-delete")]
        public JsonResult Active(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Order.Find(id);
                    item.Deleted = false;
                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "سفارش با موفقیت فعال شد."
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
        /// ریست کردن تمام سفارش و عملیات های انجام شده
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>نتیجه عملیات حذف یک سفارش</returns>
        [Authorize(Roles = "admin, admin-order-delete")]
        public JsonResult Reset(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Order.Find(id);

                    if (item.OrderDetailList.Count(x => x.RelatedOrderDetailId != null) == 0)
                    {
                        HashSet<int> workshopOrderIdList = new HashSet<int>();
                        if (!User.IsInRole("leatherProductUser"))
                        {
                            var tt = item.OrderDetailList.Where(x => x.Product.WorkshopId2 != 5 && x.Product.ProductType != ProductType.OuterWerk && x.Product.ProductType != ProductType.WatchPendent && x.Product.ProductType != ProductType.Plaque).ToList();
                            tt.RemoveRange(1, item.OrderLogList.Count - 1);
                            tt.ForEach(x =>
                            {
                                if (x.WorkshopOrderId != null)
                                    workshopOrderIdList.Add(x.WorkshopOrderId.GetValueOrDefault());

                                x.OrderDetailStatus = OrderDetailStatus.Registered;
                                x.WorkshopOrderId = null;
                                db.OrderDetailLog.RemoveRange(x.OrderDetailLogList.Where(y => y.OrderDetailStatus != OrderDetailStatus.Registered));
                            });
                        }
                        else
                        {
                            item.OrderDetailList.RemoveRange(1, item.OrderLogList.Count - 1);
                            item.OrderDetailList.ForEach(x =>
                            {
                                if (x.WorkshopOrderId != null)
                                    workshopOrderIdList.Add(x.WorkshopOrderId.GetValueOrDefault());

                                x.OrderDetailStatus = OrderDetailStatus.Registered;
                                x.WorkshopOrderId = null;
                                db.OrderDetailLog.RemoveRange(x.OrderDetailLogList.Where(y => y.OrderDetailStatus != OrderDetailStatus.Registered));
                            });
                        }

                        var workshopOrderList = db.WorkshopOrder.Where(x => workshopOrderIdList.Any(y => y == x.Id));
                        db.WorkshopOrder.RemoveRange(workshopOrderList);

                        db.SaveChanges();

                        response = new Response()
                        {
                            status = 200,
                            message = "سفارش با موفقیت ریست شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "شما نمی توانید این سفارش را ریست کنید."
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
        /// صفحه سفارشات کارگاه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult Workshop()
        {
            using (var db = new KiaGalleryContext())
            {

                ViewBag.CoWorkerList = db.Branch.Where(x => x.BranchType == BranchType.CoWorker).Select(x => new BranchViewModel
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
            }
            return View();
        }

        /// <summary>
        /// سوابق صفارشات کارگاه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult WorkshopHistory()
        {
            using (var db = new KiaGalleryContext())
            {

                ViewBag.CoWorkerList = db.Branch.Where(x => x.BranchType == BranchType.CoWorker).Select(x => new BranchViewModel
                {
                    id = x.Id,
                    orderCount = x.OrderList.Where(y => y.OrderDetailList.Count(z => z.OrderDetailStatus == OrderDetailStatus.UnderConstruction) > 0).Count(),
                    name = x.Name
                }).ToList();
            }
            return View();
        }

        /// <summary>
        /// سوابق صفارشات کارگاه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult WorkshopManipulateCoWorker(int id)
        {
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                var query = db.WorkshopOrder.Where(x => x.Order.Deleted == false && (x.OrderDetailList.Any(y => y.SendWorkshopOrder2 != true && y.Product.WorkshopId == user.WorkshopId && y.OrderDetailStatus != OrderDetailStatus.InWorkshop) || x.OrderDetailList2.Any(z => z.SendWorkshopOrder2 == true && z.Product.WorkshopId2 == user.WorkshopId && z.OrderDetailStatus != OrderDetailStatus.InWorkshop2)));
                ViewBag.WorkshopOrder = query.Select(x => new WorkshopDropDwonViewModel()
                {
                    id = x.Id,
                    val = x.WorkshopOrderSerial
                }).ToList();
            }
            ViewBag.Branch = id;
            return View("WorkshopManipulateAll");
        }

        /// <summary>
        /// سوابق صفارشات کارگاه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult WorkshopArchive()
        {
            return View();
        }

        /// <summary>
        /// لیست سفارشات با وضعیت کالای ارسال شده به کارگاه
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست سفارشات با وضعیت ارسال شده به کارگاه</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order-workshop")]
        public JsonResult GetAllWorkshopOrder(OrderSearchViewModel model)
        {
            Response response;
            try
            {
                List<WorkshopOrder> list;
                var user = GetAuthenticatedUser();
                int dataCount;
                var nonLeatherUSer = User.IsInRole("nonLeatherOrder");
                using (var db = new KiaGalleryContext())
                {
                    var query = db.WorkshopOrder.Include(x => x.OrderDetailList).Include(x => x.OrderDetailList2)
                        .Where(x =>
                                    x.Order.Deleted == false &&
                                    (
                                        x.OrderDetailList.Any(y => y.SendWorkshopOrder2 != true && y.Product.WorkshopId == user.WorkshopId && y.OrderDetailStatus == OrderDetailStatus.InWorkshop) ||
                                        x.OrderDetailList2.Any(z => z.SendWorkshopOrder2 == true && z.Product.WorkshopId2 == user.WorkshopId && z.OrderDetailStatus == OrderDetailStatus.InWorkshop2)
                                    )
                                );
                    if (User.IsInRole("nonLeatherOrder"))
                    {
                        query = query.Where(x => x.OrderDetailList.Where(y => y.Product.ProductType != ProductType.LeatherBracelet && y.Product.ProductType != ProductType.RailBracelet).Count() > 0 || x.OrderDetailList2.Where(y => y.Product.ProductType != ProductType.LeatherBracelet && y.Product.ProductType != ProductType.RailBracelet).Count() > 0);
                    }
                    if (User.IsInRole("leatherOrder"))
                    {
                        query = query.Where(x => x.OrderDetailList.Where(y => y.Product.ProductType == ProductType.LeatherBracelet && y.Product.ProductType == ProductType.RailBracelet).Count() > 0 && x.OrderDetailList2.Where(y => y.Product.ProductType == ProductType.LeatherBracelet && y.Product.ProductType == ProductType.RailBracelet).Count() > 0);
                    }
                    if (model.branchType != null)
                    {
                        query = query.Where(x => model.branchType.Any(y => y == x.Order.Branch.BranchType));
                    }

                    var branchCount = query.Where(x => x.Order.Branch.BranchType != BranchType.CoWorker).Count();
                    var coWorkerCount = db.WorkshopOrder.Where(x => x.Order.Branch.BranchType == BranchType.CoWorker && x.OrderDetailList.Count(y => y.OrderDetailStatus == OrderDetailStatus.InWorkshop && y.Product.WorkshopId == user.WorkshopId) > 0).Count();
                    if (model.coWorker != null)
                    {
                        query = query.Where(x => model.coWorker == x.Order.BranchId);
                    }
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    list = query.ToList();
                    var data = list.Select(item => new OrderViewModel()
                    {
                        id = item.Id,
                        orderNumber = "KIA-" + item.Order.OrderNumber,
                        orderSerial = "KIA-" + item.WorkshopOrderSerial,
                        sumCount = !nonLeatherUSer ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop && x.Product.WorkshopId == user.WorkshopId).Sum(x => x.Count) + item.OrderDetailList2.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop2 && x.Product.WorkshopId2 == user.WorkshopId).Sum(x => x.Count) : item.OrderDetailList.Where(x => x.Product.ProductType != ProductType.LeatherBracelet && x.Product.ProductType != ProductType.RailBracelet && x.OrderDetailStatus == OrderDetailStatus.InWorkshop && x.Product.WorkshopId == user.WorkshopId).Sum(x => x.Count) + item.OrderDetailList2.Where(x => x.Product.ProductType != ProductType.LeatherBracelet && x.Product.ProductType != ProductType.RailBracelet && x.OrderDetailStatus == OrderDetailStatus.InWorkshop2 && x.Product.WorkshopId2 == user.WorkshopId).Sum(x => x.Count),
                        sumCountSet = item.OrderDetailList.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count() + item.OrderDetailList2.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count(),
                        sumWeight = !nonLeatherUSer ? Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop && x.Product.WorkshopId == user.WorkshopId).Sum(x => x.Product.Weight * x.Count).ToString()), 2) + Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop2 && x.Product.WorkshopId2 == user.WorkshopId).Sum(x => x.Product.Weight * x.Count).ToString()), 2) : Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop && x.Product.WorkshopId == user.WorkshopId && x.Product.ProductType != ProductType.LeatherBracelet && x.Product.ProductType != ProductType.RailBracelet).Sum(x => x.Product.Weight * x.Count).ToString()), 2) + Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop2 && x.Product.WorkshopId2 == user.WorkshopId && x.Product.ProductType != ProductType.LeatherBracelet && x.Product.ProductType != ProductType.RailBracelet).Sum(x => x.Product.Weight * x.Count).ToString()), 2),
                        createUser = item.CreateUser.FullName,
                        createDate = DateUtility.GetPersianDate(item.CreateDate),
                    }).OrderByDescending(x => x.orderNumber).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
                            pageCount = Math.Ceiling((double)dataCount / model.count),
                            count = dataCount,
                            page = model.page + 1,
                            branchCount,
                            coWorkerCount
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
        /// لیست سفارشات کارگاه
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست سفارشات کارگاه</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order-workshop")]
        public JsonResult GetAllWorkshopHistoryOrder(OrderSearchViewModel model)
        {
            Response response;
            try
            {
                List<WorkshopOrder> list;
                var user = GetAuthenticatedUser();
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.WorkshopOrder.Include(x => x.OrderDetailList).Include(x => x.OrderDetailList2).Where(x => x.Order.Deleted == false);
                    if (model.archiveOnly != true)
                    {
                        query = query.Where(x => (x.OrderDetailList.Any(y => y.SendWorkshopOrder2 != true && y.Product.WorkshopId == user.WorkshopId && y.OrderDetailStatus != OrderDetailStatus.InWorkshop) || x.OrderDetailList2.Any(z => z.SendWorkshopOrder2 == true && z.Product.WorkshopId2 == user.WorkshopId && z.OrderDetailStatus != OrderDetailStatus.InWorkshop2)));
                    }
                    else if (model.archiveOnly == true)
                    {
                        query = query.Where(x => (x.OrderDetailList.Any(y => y.Product.WorkshopId == user.WorkshopId && y.OrderDetailStatus != OrderDetailStatus.InWorkshop) || x.OrderDetailList2.Any(z => z.SendWorkshopOrder2 == true && z.Product.WorkshopId2 == user.WorkshopId && z.OrderDetailStatus != OrderDetailStatus.InWorkshop2)));
                        query = query.Where(x => x.OrderDetailList.Count(z => z.OrderDetailStatus == OrderDetailStatus.UnderConstruction) == 0 && x.OrderDetailList2.Count(z => z.Product.WorkshopId2 == user.WorkshopId && z.OrderDetailStatus == OrderDetailStatus.UnderConstruction2) == 0);
                    }
                    if (model.archive != true && model.archiveOnly != true)
                    {
                        query = query.Where(x => x.OrderDetailList.Any(y => y.WorkshopOrderId == x.Id && y.Product.WorkshopId == user.WorkshopId && y.OrderDetailStatus == OrderDetailStatus.UnderConstruction) || x.OrderDetailList2.Any(y => y.WorkshopOrderId2 == x.Id && y.Product.WorkshopId2 == user.WorkshopId && y.OrderDetailStatus == OrderDetailStatus.UnderConstruction2));
                    }
                    if (!string.IsNullOrEmpty(model.term))
                        query = query.Where(x => x.WorkshopOrderSerial.Contains(model.term.Trim()) || x.OrderDetailList.Any(y => y.Product.Title.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.Customer.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.PhoneNumber.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.BranchLabel.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.Description.Contains(model.term.Trim())));
                    //if (User.IsInRole("nonLeatherOrder"))
                    //{
                    //    query = query.Where(x => x.OrderDetailList.Where(y => y.Product.ProductType != ProductType.LeatherBracelet && y.Product.ProductType != ProductType.RailBracelet).Count() > 0 || x.OrderDetailList2.Where(y => y.Product.ProductType != ProductType.LeatherBracelet && y.Product.ProductType != ProductType.RailBracelet).Count() > 0);
                    //}
                    //if (User.IsInRole("leatherOrder"))
                    //{
                    //    query = query.Where(x => x.OrderDetailList.Where(y => y.Product.ProductType == ProductType.LeatherBracelet && y.Product.ProductType == ProductType.RailBracelet).Count() > 0 || x.OrderDetailList2.Where(y => y.Product.ProductType == ProductType.LeatherBracelet && y.Product.ProductType == ProductType.RailBracelet).Count() > 0);
                    //}
                    if (model.branchType != null)
                    {
                        query = query.Where(x => model.branchType.Any(y => y == x.Order.Branch.BranchType));
                    }

                    if (model.coWorker != null)
                    {
                        query = query.Where(x => model.coWorker == x.Order.BranchId);
                    }
                    var branchCount = query.Where(x => x.Order.Branch.BranchType != BranchType.CoWorker).Count();
                    var coWorkerCount = db.WorkshopOrder.Where(x => x.Order.Branch.BranchType == BranchType.CoWorker && x.OrderDetailList.Count(y => y.OrderDetailStatus == OrderDetailStatus.UnderConstruction && y.Product.WorkshopId == user.WorkshopId) > 0).Count();
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    list = query.ToList();

                    var data = list.Select(item => new OrderViewModel()
                    {
                        id = item.Id,
                        orderSerial = "KIA-" + item.WorkshopOrderSerial,
                        orderNumber = "KIA-" + item.Order.OrderNumber,
                        sumCount = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId).Sum(x => x.Count) + item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId).Sum(x => x.Count),
                        sumCountSet = item.OrderDetailList.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count() + item.OrderDetailList2.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count(),
                        sumWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId).Sum(x => x.Product.Weight * x.Count).ToString()), 2) + Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId).Sum(x => x.Product.Weight * x.Count).ToString()), 2),
                        registered = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count),
                        registeredWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        inWorkshop = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count),
                        inWorkshopWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        underConstruction = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count),
                        underConstructionWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        outOfConstruction = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count),
                        outOfConstructionWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        inWorkshop2 = item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InWorkshop2).Sum(x => x.Count),
                        inWorkshopWeight2 = Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InWorkshop2).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        underConstruction2 = item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.UnderConstruction2).Sum(x => x.Count),
                        underConstructionWeight2 = Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.UnderConstruction2).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        outOfConstruction2 = item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction2).Sum(x => x.Count),
                        outOfConstructionWeight2 = Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction2).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        inPreparation = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count) + item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count),
                        inPreparationWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count * x.Product.Weight).ToString()), 2) + Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        readyForDelivery = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count) + item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count),
                        readyForDeliveryWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count * x.Product.Weight).ToString()), 2) + Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        sent = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count) + item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count),
                        sentWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count * x.Product.Weight).ToString()), 2) + Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        shortage = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count) + item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count),
                        shortageWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count * x.Product.Weight).ToString()), 2) + Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        shortageOrder = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count) + item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count),
                        shortageOrderWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count * x.Product.Weight).ToString()), 2) + Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        cancel = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count) + item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count),
                        cancelWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count * x.Product.Weight).ToString()), 2) + Math.Round(double.Parse(item.OrderDetailList2.Where(x => x.Product.WorkshopId2 == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                        createUser = item.CreateUser?.FullName,
                        createDate = DateUtility.GetPersianDate(item.CreateDate),
                    }).ToList();
                    data.ForEach(x =>
                    {
                        if (x.sumCount == x.inWorkshop || x.sumCount == x.inWorkshop2)
                            x.bgColor = "";
                        else if (x.sumCount == x.underConstruction || x.sumCount == x.underConstruction2)
                            x.bgColor = "bg-new-order";
                        else if (x.sumCount == x.outOfConstruction || x.sumCount == x.outOfConstruction2)
                            x.bgColor = "bg-done-order";
                        else if (x.underConstruction > 0 || x.underConstruction2 > 0)
                            x.bgColor = "bg-open-order";
                        else
                            x.bgColor = "bg-done-order";
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
                            pageCount = Math.Ceiling((double)dataCount / model.count),
                            count = dataCount,
                            page = model.page + 1,
                            branchCount,
                            coWorkerCount

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
        /// خلاصه وضعیت تعداد سفارشات ارسال شده به کارگاه شامل تعداد
        /// </summary>
        /// <returns>تعداد سفارشات باز</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order-workshop")]
        public JsonResult WorkshopSummary()
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var query = db.WorkshopOrder.Where(x => x.Order.Deleted == false && (x.OrderDetailList.Any(y => y.SendWorkshopOrder2 != true && y.Product.WorkshopId == user.WorkshopId && y.OrderDetailStatus == OrderDetailStatus.InWorkshop) || x.OrderDetailList2.Any(z => z.SendWorkshopOrder2 == true && z.Product.WorkshopId2 == user.WorkshopId && z.OrderDetailStatus == OrderDetailStatus.InWorkshop2)));
                    var dataCount = query.Count();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            count = dataCount
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
        /// صفحه مشاهده جزئیات یک سفارش برای کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult WorkshopDetail(int id)
        {
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                ViewBag.WorkshopOrder = db.WorkshopOrder.Single(x => x.Id == id && x.Order.Deleted == false);
            }
            return View();
        }

        /// <summary>
        /// جزئیات محصولات داخل سفارش برای مشاهده کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>لیست محصولات سفارش مرتبط با کارگاه</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order-workshop")]
        public JsonResult WorkshopDetailList(int id, OrderDetailSearchViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => (x.WorkshopOrderId == id && x.Product.WorkshopId == user.WorkshopId) || (x.WorkshopOrderId2 == id && x.Product.WorkshopId2 == user.WorkshopId));

                    if (!string.IsNullOrEmpty(model.term))
                    {
                        query = query.Where(x => x.Order.OrderSerial.Contains(model.term.Trim()) || x.Product.Code.Contains(model.term.Trim()) || x.Product.BookCode.Contains(model.term.Trim()) || x.Product.Title.Contains(model.term.Trim()) || x.Product.Title.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.Order.Branch.Name.Contains(model.term.Trim()) || x.Order.Branch.Name.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.Customer.Contains(model.term.Trim()) || x.Customer.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.PhoneNumber.Contains(model.term.Trim()) || x.BranchLabel.Contains(model.term.Trim()) || x.Description.Contains(model.term.Trim()) || x.Description.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")));
                    }

                    if (model.typeList != null && model.typeList.Count > 0)
                    {
                        query = query.Where(x => model.typeList.Any(y => y == x.Product.ProductType));
                    }

                    switch (model.order)
                    {
                        case "productType":
                            query = query.OrderBy(x => x.Product.ProductType);
                            break;
                        default:
                            query = query.OrderBy(x => x.Id);
                            break;
                    }

                    dataCount = query.Count();
                    //list = query.ToList();

                    var data = query.Select(item => new OrderDetailViewModel()
                    {
                        id = item.Id,
                        orderSerial = "KIA-" + item.Order.OrderSerial,
                        orderNumber = "KIA-" + item.Order.OrderNumber,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        orderType = item.OrderType,
                        //orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        productSizeId = item.Product.SizeId,
                        workshopName = item.Product.Workshop.Name,
                        workshopName2 = item.Product.Workshop2.Name,
                        size = item.Size,
                        size2 = item.Size2,
                        goldType = item.GoldType,
                        productColor = item.ProductColor,
                        setNumber = item.SetNumber,
                        outerWerkType = item.OuterWerkType,
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        branchLabel = item.BranchLabel,
                        description = item.Description,
                        description2 = item.Description2,
                        title = item.Product.Title,
                        code = item.Product.Code,
                        bookCode = item.Product.BookCode,
                        weight = item.Product.Weight,
                        count = item.Count,
                        orderDetailStatus = item.OrderDetailStatus,
                        //orderDetailStatusTitle = Enums.GetTitle(item.OrderDetailStatus),
                        createdUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        //createDate = DateUtility.GetPersianDate(item.CreateDate),
                        relatedOrderDetailSerial = "KIA-" + item.RelatedOrderDetail.Order.OrderSerial,
                        stoneList = item.OrderDetailStoneList.Select(x => new OrderDetailStoneViewModel()
                        {
                            order = x.Order,
                            stoneName = x.Stone.Name
                        }).ToList(),
                        leatherList = item.OrderDetailLeatherList.Select(x => new OrderDetailLeatherViewModel()
                        {
                            order = x.Order,
                            leatherName = x.Leather.Name
                        }).ToList(),
                        returned = item.OrderDetailLogList.Count(x => x.Description == "مرجوعی" || x.Description == "مرجوعی به کارگاه اول" || x.Description == "مرجوعی به کارگاه دوم")
                    })
                    .ToList();

                    data.ForEach(x =>
                    {
                        x.orderTypeTitle = Enums.GetTitle(x.orderType);
                        x.goldTypeTitle = Enums.GetTitle(x.goldType);
                        x.productColorTitle = x.productColor >= 0 ? Enums.GetTitle(x.productColor) : "";
                        x.outerWerkTypeTitle = Enums.GetTitle(x.outerWerkType);
                        x.orderDetailStatusTitle = Enums.GetTitle(x.orderDetailStatus);
                        x.createDate = DateUtility.GetPersianDate(x.CreateDate);
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
                response = new Response()
                {
                    status = 500,
                    message = ex.Message + "e "
                };
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// چاپ گزارش سفارش ثبت شده با ارسال ردیف های سفارشات
        /// </summary>
        /// <param name="id">ردیف سفارشات</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, admin-order, order, allOrder")]
        public ActionResult Print(string id, string model)
        {
            OrderPrintViewModel modelData = null;
            if (!string.IsNullOrEmpty(model))
                modelData = JsonConvert.DeserializeObject<OrderPrintViewModel>(model);
            List<int> idList = id.Split(',').Select(x => int.Parse(x)).ToList();

            List<OrderDetail> result;
            using (var db = new KiaGalleryContext())
            {
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.ProductFileList).Where(x => idList.Any(y => y == x.OrderId));

                if (!User.IsInRole("admin") && !User.IsInRole("admin-order"))
                {
                    var user = GetAuthenticatedUser();
                    query = query.Where(x => x.Order.BranchId == user.BranchId);
                }
                if (User.IsInRole("leatherProductUser"))
                {
                    query = query.Where(x => x.Product.WorkshopId2 == 5 || x.Product.ProductType == ProductType.OuterWerk || x.Product.ProductType == ProductType.WatchPendent2 || x.Product.ProductType == ProductType.Plaque);
                }
                if (!User.IsInRole("leatherProductUser") && !User.IsInRole("admin") && !User.IsInRole("allOrder"))
                {
                    query = query.Where(x => x.Product.WorkshopId2 != 5 && x.Product.ProductType != ProductType.OuterWerk && x.Product.ProductType != ProductType.WatchPendent2 && x.Product.ProductType != ProductType.Plaque);
                }
                if (modelData != null && modelData.orderType != null && modelData.orderType > 0)
                {
                    switch (modelData.orderType)
                    {
                        case 1:
                            query = query.Where(x => string.IsNullOrEmpty(x.Customer));
                            break;
                        case 2:
                            query = query.Where(x => !string.IsNullOrEmpty(x.Customer));
                            break;
                    }
                }

                if (modelData != null && modelData.workshopList != null && modelData.workshopList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => modelData.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId));
                }

                if (modelData != null && modelData.statusList != null && modelData.statusList.Count() > 0)
                {
                    query = query.Where(x => modelData.statusList.Any(y => y == x.OrderDetailStatus));
                }

                result = query.OrderBy(x => x.Product.Id).ToList();

                var serialList = result.Select(x => "KIA-" + x.Order.OrderSerial).Distinct().ToList();
                var orderNumberList = result.Select(x => "KIA-" + x.Order.OrderNumber).Distinct().ToList();
                var dateList = result.Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var branchNameList = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    Image = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order)?.FileName,
                    Title = item.Product.Title,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = "KIA-" + item.Order.OrderNumber,
                    Code = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    WorkshopId2 = item.Product.WorkshopId2,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => "سنگ " + x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => "چرم " + x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    Description = item.Description,
                    Description2 = item.Description2,
                    SetNumber = item.SetNumber,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""
                })
                .ToList()
                .GroupBy(x => new
                {
                    x.Image,
                    x.Title,
                    x.Status,
                    x.OrderNumber,
                    x.Code,
                    x.Type,
                    x.WorkshopId,
                    x.Workshop,
                    x.WorkshopId2,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.ProductColor
                })
                .Select(x => new
                {
                    x.Key.Image,
                    x.Key.Title,
                    x.Key.Status,
                    x.Key.OrderNumber,
                    x.Key.Code,
                    x.Key.Type,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.WorkshopId2,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    x.Key.ProductColor,
                    Count = x.Count()
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));
                List<OrderPrintDataViewModel> setDataPrint = new List<OrderPrintDataViewModel>();
                List<StiReport> reports = new List<StiReport>();
                foreach (var itemWorkshop in data.GroupBy(x => x.WorkshopId))
                {
                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();
                    foreach (var item in itemWorkshop)
                    {
                        if (item.SetNumber == null)
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = item.Weight + " گرم",
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                ProductColor = item.ProductColor
                            };

                            dataPrint.Add(itemPrint);
                        }
                        else
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = item.Weight + " گرم",
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                ProductColor = item.ProductColor
                            };

                            setDataPrint.Add(itemPrint);

                        }


                    }
                    if (dataPrint.Count > 0)
                    {
                        dataPrint = dataPrint.OrderBy(x => x.OrderNumber).ToList();

                        DataSet dataset = new DataSet("DataSource");
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Row");
                        dataTable.Columns.Add("Image", typeof(byte[]));
                        dataTable.Columns.Add("Title");
                        dataTable.Columns.Add("Status");
                        dataTable.Columns.Add("OrderNumber");
                        dataTable.Columns.Add("Code");
                        dataTable.Columns.Add("Type");
                        dataTable.Columns.Add("Workshop");
                        dataTable.Columns.Add("Workshop2");
                        dataTable.Columns.Add("Weight");
                        dataTable.Columns.Add("GoldType");
                        dataTable.Columns.Add("OuterWerkType");
                        dataTable.Columns.Add("Count");
                        dataTable.Columns.Add("Size");
                        dataTable.Columns.Add("Stone");
                        dataTable.Columns.Add("Leather");
                        dataTable.Columns.Add("Customer");
                        dataTable.Columns.Add("Description");
                        dataTable.Columns.Add("Description2");
                        dataTable.Columns.Add("ProductColor");

                        for (int j = 0; j < dataPrint.Count; j++)
                        {
                            DataRow row = dataTable.NewRow();
                            row["Row"] = j + 1;
                            row["Image"] = dataPrint[j].Image;
                            row["Title"] = dataPrint[j].Title;
                            row["Status"] = dataPrint[j].Status;
                            row["OrderNumber"] = dataPrint[j].OrderNumber;
                            row["Code"] = dataPrint[j].Code;
                            row["Type"] = dataPrint[j].Type;
                            row["Workshop"] = dataPrint[j].Workshop;
                            row["Workshop2"] = dataPrint[j].Workshop2;
                            row["Weight"] = dataPrint[j].Weight;
                            row["GoldType"] = dataPrint[j].GoldType;
                            row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                            row["Count"] = dataPrint[j].Count;
                            row["Size"] = dataPrint[j].Size;
                            row["Stone"] = dataPrint[j].Stone;
                            row["Leather"] = dataPrint[j].Leather;
                            row["Customer"] = dataPrint[j].Customer;
                            row["Description"] = dataPrint[j].Description;
                            row["Description2"] = dataPrint[j].Description2;
                            row["ProductColor"] = dataPrint[j].ProductColor;

                            dataTable.Rows.Add(row);
                        }
                        dataset.Tables.Add(dataTable);

                        StiReport report = new StiReport();
                        if (modelData.branchType == BranchType.CoWorker)
                        {
                            report.Load(Server.MapPath("~/Report/Order/OrderReportCoWorker.mrt"));
                        }
                        else
                        {
                            report.Load(Server.MapPath("~/Report/Order/OrderReport.mrt"));
                        }
                        report.Dictionary.Databases.Clear();
                        report.ScriptLanguage = StiReportLanguageType.CSharp;
                        report.RegData("DataSource", dataset.Tables[0].DefaultView);
                        report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                        report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                        report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                        report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                        report.Dictionary.Variables["SetNumber"].Value = "";
                        report.Dictionary.Variables["SetNumberTitle"].Value = "";
                        //report.Dictionary.Synchronize();
                        report.Compile();
                        report.Render(false);

                        reports.Add(report);
                    }

                }

                foreach (var itemSet in setDataPrint.GroupBy(x => x.SetNumber))
                {
                    var setList = itemSet.ToList().OrderBy(x => x.Code).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Image", typeof(byte[]));
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("OrderNumber");
                    dataTable.Columns.Add("Code");
                    dataTable.Columns.Add("Type");
                    dataTable.Columns.Add("Workshop");
                    dataTable.Columns.Add("Workshop2");
                    dataTable.Columns.Add("Weight");
                    dataTable.Columns.Add("GoldType");
                    dataTable.Columns.Add("OuterWerkType");
                    dataTable.Columns.Add("Count");
                    dataTable.Columns.Add("Size");
                    dataTable.Columns.Add("Stone");
                    dataTable.Columns.Add("Leather");
                    dataTable.Columns.Add("Customer");
                    dataTable.Columns.Add("Description");
                    dataTable.Columns.Add("Description2");
                    dataTable.Columns.Add("ProductColor");

                    for (int j = 0; j < setList.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Image"] = setList[j].Image;
                        row["Title"] = setList[j].Title;
                        row["Status"] = setList[j].Status;
                        row["OrderNumber"] = setList[j].OrderNumber;
                        row["Code"] = setList[j].Code;
                        row["Type"] = setList[j].Type;
                        row["Workshop"] = setList[j].Workshop;
                        row["Workshop2"] = setList[j].Workshop2;
                        row["Weight"] = setList[j].Weight;
                        row["GoldType"] = setList[j].GoldType;
                        row["OuterWerkType"] = setList[j].OuterWerkType;
                        row["Count"] = setList[j].Count;
                        row["Size"] = setList[j].Size;
                        row["Stone"] = setList[j].Stone;
                        row["Leather"] = setList[j].Leather;
                        row["Customer"] = setList[j].Customer;
                        row["Description"] = setList[j].Description;
                        row["Description2"] = setList[j].Description2;
                        row["ProductColor"] = setList[j].ProductColor;

                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();

                    if (modelData.branchType == BranchType.CoWorker)
                    {
                        report.Load(Server.MapPath("~/Report/Order/OrderReportCoWorker.mrt"));
                    }
                    else
                    {
                        report.Load(Server.MapPath("~/Report/Order/OrderReport.mrt"));
                    }
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                    report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                    report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                    report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                    report.Dictionary.Variables["SetNumber"].Value = setList.FirstOrDefault().SetNumber.ToString();
                    report.Dictionary.Variables["SetNumberTitle"].Value = setList.FirstOrDefault().SetNumber != null ? "سریال نیم ست" : "";
                    //report.Dictionary.Synchronize();
                    report.Compile();
                    report.Render(false);

                    reports.Add(report);

                }
                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        /// <summary>
        /// چاپ گزارش سفارش ثبت شده با ارسال ردیف های سفارشات
        /// </summary>
        /// <param name="id">ردیف سفارشات</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, admin-order, order, allOrder")]
        public ActionResult PrintMin(string id, string model)
        {
            OrderPrintViewModel modelData = null;
            if (!string.IsNullOrEmpty(model))
                modelData = JsonConvert.DeserializeObject<OrderPrintViewModel>(model);
            List<int> idList = id.Split(',').Select(x => int.Parse(x)).ToList();

            List<OrderDetail> result;
            using (var db = new KiaGalleryContext())
            {
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.ProductFileList).Where(x => idList.Any(y => y == x.OrderId));

                if (!User.IsInRole("admin") && !User.IsInRole("admin-order"))
                {
                    var user = GetAuthenticatedUser();
                    query = query.Where(x => x.Order.BranchId == user.BranchId);
                }
                if (User.IsInRole("leatherProductUser"))
                {
                    query = query.Where(x => x.Product.WorkshopId2 == 5 || x.Product.ProductType == ProductType.OuterWerk || x.Product.ProductType == ProductType.WatchPendent2 || x.Product.ProductType == ProductType.Plaque);
                }
                if (!User.IsInRole("leatherProductUser") && !User.IsInRole("admin") && !User.IsInRole("allOrder"))
                {
                    query = query.Where(x => x.Product.WorkshopId2 != 5 && x.Product.ProductType != ProductType.OuterWerk && x.Product.ProductType != ProductType.WatchPendent2 && x.Product.ProductType != ProductType.Plaque);
                }
                if (modelData != null && modelData.orderType != null && modelData.orderType > 0)
                {
                    switch (modelData.orderType)
                    {
                        case 1:
                            query = query.Where(x => string.IsNullOrEmpty(x.Customer));
                            break;
                        case 2:
                            query = query.Where(x => !string.IsNullOrEmpty(x.Customer));
                            break;
                    }
                }

                if (modelData != null && modelData.workshopList != null && modelData.workshopList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => modelData.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId));
                }

                if (modelData != null && modelData.statusList != null && modelData.statusList.Count() > 0)
                {
                    query = query.Where(x => modelData.statusList.Any(y => y == x.OrderDetailStatus));
                }

                result = query.OrderBy(x => x.Product.Id).ToList();

                var serialList = result.Select(x => x.Order.OrderSerial).Distinct().ToList();
                var orderNumberList = result.Select(x => x.Order.OrderNumber).Distinct().ToList();
                var dateList = result.Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var branchNameList = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    Title = item.Product.Title,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = item.Order.OrderNumber,
                    Code = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    WorkshopId2 = item.Product.WorkshopId2,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    Size2 = item.Size2,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    Description = item.Description,
                    Description2 = item.Description2,
                    SetNumber = item.SetNumber,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""
                })
                .ToList()
                .GroupBy(x => new
                {
                    x.Title,
                    x.Status,
                    x.OrderNumber,
                    x.Code,
                    x.Type,
                    x.WorkshopId,
                    x.Workshop,
                    x.WorkshopId2,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.Size2,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.ProductColor
                })
                .Select(x => new
                {
                    x.Key.Title,
                    x.Key.Status,
                    x.Key.OrderNumber,
                    x.Key.Code,
                    x.Key.Type,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.WorkshopId2,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.Size2,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    Count = x.Count(),
                    x.Key.ProductColor
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));
                List<OrderPrintDataViewModel> setDataPrint = new List<OrderPrintDataViewModel>();
                List<StiReport> reports = new List<StiReport>();
                foreach (var itemWorkshop in data.GroupBy(x => x.WorkshopId))
                {
                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();
                    foreach (var item in itemWorkshop)
                    {
                        if (item.SetNumber == null)
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = item.Weight + " گرم",
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Size2 = string.IsNullOrEmpty(item.Size2) ? "" : item.Size2,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                ProductColor = item.ProductColor
                            };

                            dataPrint.Add(itemPrint);
                        }
                        else
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = item.Weight + " گرم",
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Size2 = string.IsNullOrEmpty(item.Size) ? "" : item.Size2,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                ProductColor = item.ProductColor
                            };

                            setDataPrint.Add(itemPrint);

                        }


                    }
                    if (dataPrint.Count > 0)
                    {
                        dataPrint = dataPrint.OrderBy(x => x.OrderNumber).ToList();

                        DataSet dataset = new DataSet("DataSource");
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Row");
                        dataTable.Columns.Add("Title");
                        dataTable.Columns.Add("Status");
                        dataTable.Columns.Add("OrderNumber");
                        dataTable.Columns.Add("Code");
                        dataTable.Columns.Add("Type");
                        dataTable.Columns.Add("Workshop");
                        dataTable.Columns.Add("Workshop2");
                        dataTable.Columns.Add("Weight");
                        dataTable.Columns.Add("GoldType");
                        dataTable.Columns.Add("OuterWerkType");
                        dataTable.Columns.Add("Count");
                        dataTable.Columns.Add("Size");
                        dataTable.Columns.Add("Size2");
                        dataTable.Columns.Add("Stone");
                        dataTable.Columns.Add("Leather");
                        dataTable.Columns.Add("Customer");
                        dataTable.Columns.Add("Description");
                        dataTable.Columns.Add("Description2");
                        dataTable.Columns.Add("ProductColor");

                        for (int j = 0; j < dataPrint.Count; j++)
                        {
                            DataRow row = dataTable.NewRow();
                            row["Row"] = j + 1;
                            row["Title"] = dataPrint[j].Title;
                            row["Status"] = dataPrint[j].Status;
                            row["OrderNumber"] = dataPrint[j].OrderNumber;
                            row["Code"] = dataPrint[j].Code;
                            row["Type"] = dataPrint[j].Type;
                            row["Workshop"] = dataPrint[j].Workshop;
                            row["Workshop2"] = dataPrint[j].Workshop2;
                            row["Weight"] = dataPrint[j].Weight;
                            row["GoldType"] = dataPrint[j].GoldType;
                            row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                            row["Count"] = dataPrint[j].Count;
                            row["Size"] = dataPrint[j].Size;
                            row["Size2"] = dataPrint[j].Size2;
                            row["Stone"] = dataPrint[j].Stone;
                            row["Leather"] = dataPrint[j].Leather;
                            row["Customer"] = dataPrint[j].Customer;
                            row["Description"] = dataPrint[j].Description;
                            row["Description2"] = dataPrint[j].Description2;
                            row["ProductColor"] = dataPrint[j].ProductColor;

                            dataTable.Rows.Add(row);
                        }
                        dataset.Tables.Add(dataTable);

                        StiReport report = new StiReport();
                        if (modelData.branchType == BranchType.CoWorker)
                        {
                            report.Load(Server.MapPath("~/Report/Order/OrderReportMinCoWorker.mrt"));

                        }
                        else
                        {
                            report.Load(Server.MapPath("~/Report/Order/OrderReportMin.mrt"));
                        }
                        report.Dictionary.Databases.Clear();
                        report.ScriptLanguage = StiReportLanguageType.CSharp;
                        report.RegData("DataSource", dataset.Tables[0].DefaultView);
                        report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                        report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                        report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                        report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                        report.Dictionary.Variables["SetNumber"].Value = "";
                        report.Dictionary.Variables["SetNumberTitle"].Value = "";
                        //report.Dictionary.Synchronize();
                        report.Compile();
                        report.Render(false);

                        reports.Add(report);
                    }

                }

                foreach (var itemSet in setDataPrint.GroupBy(x => x.SetNumber))
                {
                    var setList = itemSet.ToList().OrderBy(x => x.Code).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("OrderNumber");
                    dataTable.Columns.Add("Code");
                    dataTable.Columns.Add("Type");
                    dataTable.Columns.Add("Workshop");
                    dataTable.Columns.Add("Workshop2");
                    dataTable.Columns.Add("Weight");
                    dataTable.Columns.Add("GoldType");
                    dataTable.Columns.Add("OuterWerkType");
                    dataTable.Columns.Add("Count");
                    dataTable.Columns.Add("Size");
                    dataTable.Columns.Add("Size2");
                    dataTable.Columns.Add("Stone");
                    dataTable.Columns.Add("Leather");
                    dataTable.Columns.Add("Customer");
                    dataTable.Columns.Add("Description");
                    dataTable.Columns.Add("Description2");
                    dataTable.Columns.Add("ProductColor");

                    for (int j = 0; j < setList.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Title"] = setList[j].Title;
                        row["Status"] = setList[j].Status;
                        row["OrderNumber"] = setList[j].OrderNumber;
                        row["Code"] = setList[j].Code;
                        row["Type"] = setList[j].Type;
                        row["Workshop"] = setList[j].Workshop;
                        row["Workshop2"] = setList[j].Workshop2;
                        row["Weight"] = setList[j].Weight;
                        row["GoldType"] = setList[j].GoldType;
                        row["OuterWerkType"] = setList[j].OuterWerkType;
                        row["Count"] = setList[j].Count;
                        row["Size"] = setList[j].Size;
                        row["Size2"] = setList[j].Size2;
                        row["Stone"] = setList[j].Stone;
                        row["Leather"] = setList[j].Leather;
                        row["Customer"] = setList[j].Customer;
                        row["Description"] = setList[j].Description;
                        row["Description2"] = setList[j].Description2;
                        row["ProductColor"] = setList[j].ProductColor;

                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();
                    if (modelData.branchType == BranchType.CoWorker)
                    {
                        report.Load(Server.MapPath("~/Report/Order/OrderReportMinCoWorker.mrt"));
                    }
                    else
                    {
                        report.Load(Server.MapPath("~/Report/Order/OrderReportMin.mrt"));
                    }
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                    report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                    report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                    report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                    report.Dictionary.Variables["SetNumber"].Value = setList.FirstOrDefault().SetNumber.ToString();
                    report.Dictionary.Variables["SetNumberTitle"].Value = setList.FirstOrDefault().SetNumber != null ? "سریال نیم ست" : "";
                    //report.Dictionary.Synchronize();
                    report.Compile();
                    report.Render(false);

                    reports.Add(report);

                }
                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        /// <summary>
        /// چاپ گزارش سفارش ثبت شده با ارسال ردیف های محصولات
        /// </summary>
        /// <param name="id">ردیف محصولات</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, admin-order, order, allOrder")]
        public ActionResult PrintProduct(string id)
        {
            List<int> idList = id.Split(',').Select(x => int.Parse(x)).ToList();
            OrderPrintViewModel modelData = null;
            List<OrderDetail> result;
            using (var db = new KiaGalleryContext())
            {
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.ProductFileList).Where(x => idList.Any(y => y == x.Id));

                if (!User.IsInRole("admin") && !User.IsInRole("admin-order"))
                {
                    var user = GetAuthenticatedUser();
                    query = query.Where(x => x.Order.BranchId == user.BranchId);
                }
                if (User.IsInRole("leatherProductUser"))
                {
                    query = query.Where(x => x.Product.WorkshopId2 == 5 || x.Product.ProductType == ProductType.OuterWerk || x.Product.ProductType == ProductType.WatchPendent2 || x.Product.ProductType == ProductType.Plaque);
                }
                if (!User.IsInRole("leatherProductUser") && !User.IsInRole("admin") && !User.IsInRole("allOrder"))
                {
                    query = query.Where(x => x.Product.WorkshopId2 != 5 && x.Product.ProductType != ProductType.OuterWerk && x.Product.ProductType != ProductType.WatchPendent2 && x.Product.ProductType != ProductType.Plaque);
                }
                if (modelData != null && modelData.orderType != null && modelData.orderType > 0)
                {
                    switch (modelData.orderType)
                    {
                        case 1:
                            query = query.Where(x => string.IsNullOrEmpty(x.Customer));
                            break;
                        case 2:
                            query = query.Where(x => !string.IsNullOrEmpty(x.Customer));
                            break;
                    }
                }

                if (modelData != null && modelData.workshopList != null && modelData.workshopList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => modelData.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId));
                }

                if (modelData != null && modelData.statusList != null && modelData.statusList.Count() > 0)
                {
                    query = query.Where(x => modelData.statusList.Any(y => y == x.OrderDetailStatus));
                }

                result = query.OrderBy(x => x.Product.Id).ToList();



                var serialList = result.OrderBy(x => x.Order.OrderNumber).Select(x => "KIA-" + x.Order.OrderSerial).Distinct().ToList();
                var orderNumberList = result.OrderBy(x => x.Order.OrderNumber).Select(x => "KIA-" + x.Order.OrderNumber).Distinct().ToList();
                var dateList = result.OrderBy(x => x.Order.CreateDate).Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var branchNameList = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    Image = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order)?.FileName,
                    Title = item.Product.Title,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = "KIA-" + item.Order.OrderNumber,
                    Code = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    WorkshopId2 = item.Product.WorkshopId2,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => "سنگ " + x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => "چرم " + x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    Description = item.Description,
                    Description2 = item.Description2,
                    SetNumber = item.SetNumber,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""
                })
                .ToList()
                .GroupBy(x => new
                {
                    x.Image,
                    x.Title,
                    x.Status,
                    x.OrderNumber,
                    x.Code,
                    x.Type,
                    x.WorkshopId,
                    x.Workshop,
                    x.WorkshopId2,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.ProductColor
                })
                .Select(x => new
                {
                    x.Key.Image,
                    x.Key.Title,
                    x.Key.Status,
                    x.Key.OrderNumber,
                    x.Key.Code,
                    x.Key.Type,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.WorkshopId2,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    Count = x.Count(),
                    x.Key.ProductColor
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();
                List<OrderPrintDataViewModel> setDataPrint = new List<OrderPrintDataViewModel>();
                foreach (var itemWorkshop in data.GroupBy(x => x.WorkshopId))
                {

                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();

                    foreach (var item in itemWorkshop)
                    {
                        if (item.SetNumber == null)
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = " گرم " + item.Weight,
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                ProductColor = item.ProductColor
                            };

                            dataPrint.Add(itemPrint);
                        }
                        else
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = item.Weight + " گرم",
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                ProductColor = item.ProductColor
                            };

                            setDataPrint.Add(itemPrint);
                        }

                    }
                    if (dataPrint.Count > 0)
                    {
                        dataPrint = dataPrint.OrderBy(x => x.OrderNumber).ToList();

                        DataSet dataset = new DataSet("DataSource");
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Row");
                        dataTable.Columns.Add("Image", typeof(byte[]));
                        dataTable.Columns.Add("Title");
                        dataTable.Columns.Add("Status");
                        dataTable.Columns.Add("OrderNumber");
                        dataTable.Columns.Add("Code");
                        dataTable.Columns.Add("Type");
                        dataTable.Columns.Add("Workshop");
                        dataTable.Columns.Add("Workshop2");
                        dataTable.Columns.Add("Weight");
                        dataTable.Columns.Add("GoldType");
                        dataTable.Columns.Add("OuterWerkType");
                        dataTable.Columns.Add("Count");
                        dataTable.Columns.Add("Size");
                        dataTable.Columns.Add("Stone");
                        dataTable.Columns.Add("Leather");
                        dataTable.Columns.Add("Customer");
                        dataTable.Columns.Add("Description");
                        dataTable.Columns.Add("Description2");
                        dataTable.Columns.Add("ProductColor");

                        for (int j = 0; j < dataPrint.Count; j++)
                        {
                            DataRow row = dataTable.NewRow();
                            row["Row"] = j + 1;
                            row["Image"] = dataPrint[j].Image;
                            row["Title"] = dataPrint[j].Title;
                            row["Status"] = dataPrint[j].Status;
                            row["OrderNumber"] = dataPrint[j].OrderNumber;
                            row["Code"] = dataPrint[j].Code;
                            row["Type"] = dataPrint[j].Type;
                            row["Workshop"] = dataPrint[j].Workshop;
                            row["Workshop2"] = dataPrint[j].Workshop2;
                            row["Weight"] = dataPrint[j].Weight;
                            row["GoldType"] = dataPrint[j].GoldType;
                            row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                            row["Count"] = dataPrint[j].Count;
                            row["Size"] = dataPrint[j].Size;
                            row["Stone"] = dataPrint[j].Stone;
                            row["Leather"] = dataPrint[j].Leather;
                            row["Customer"] = dataPrint[j].Customer;
                            row["Description"] = dataPrint[j].Description;
                            row["Description2"] = dataPrint[j].Description2;
                            row["ProductColor"] = dataPrint[j].ProductColor;

                            dataTable.Rows.Add(row);
                        }
                        dataset.Tables.Add(dataTable);

                        StiReport report = new StiReport();
                        report.Load(Server.MapPath("~/Report/Order/OrderReportAdmin.mrt"));
                        report.Dictionary.Databases.Clear();
                        report.ScriptLanguage = StiReportLanguageType.CSharp;
                        report.RegData("DataSource", dataset.Tables[0].DefaultView);
                        report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                        report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                        report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                        report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                        report.Dictionary.Variables["SetNumber"].Value = "";
                        report.Dictionary.Variables["SetNumberTitle"].Value = "";
                        //report.Dictionary.Synchronize();
                        report.Compile();
                        report.Render(false);

                        reports.Add(report);
                    }


                }
                foreach (var itemSet in setDataPrint.GroupBy(x => x.SetNumber))
                {
                    var setList = setDataPrint.Where(x => x.SetNumber == itemSet.Key).ToList().OrderBy(x => x.Code).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Image", typeof(byte[]));
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("OrderNumber");
                    dataTable.Columns.Add("Code");
                    dataTable.Columns.Add("Type");
                    dataTable.Columns.Add("Workshop");
                    dataTable.Columns.Add("Workshop2");
                    dataTable.Columns.Add("Weight");
                    dataTable.Columns.Add("GoldType");
                    dataTable.Columns.Add("OuterWerkType");
                    dataTable.Columns.Add("Count");
                    dataTable.Columns.Add("Size");
                    dataTable.Columns.Add("Stone");
                    dataTable.Columns.Add("Leather");
                    dataTable.Columns.Add("Customer");
                    dataTable.Columns.Add("Description");
                    dataTable.Columns.Add("Description2");
                    dataTable.Columns.Add("ProductColor");

                    for (int j = 0; j < setList.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Image"] = setList[j].Image;
                        row["Title"] = setList[j].Title;
                        row["Status"] = setList[j].Status;
                        row["OrderNumber"] = setList[j].OrderNumber;
                        row["Code"] = setList[j].Code;
                        row["Type"] = setList[j].Type;
                        row["Workshop"] = setList[j].Workshop;
                        row["Workshop2"] = setList[j].Workshop2;
                        row["Weight"] = setList[j].Weight;
                        row["GoldType"] = setList[j].GoldType;
                        row["OuterWerkType"] = setList[j].OuterWerkType;
                        row["Count"] = setList[j].Count;
                        row["Size"] = setList[j].Size;
                        row["Stone"] = setList[j].Stone;
                        row["Leather"] = setList[j].Leather;
                        row["Customer"] = setList[j].Customer;
                        row["Description"] = setList[j].Description;
                        row["Description2"] = setList[j].Description2;
                        row["ProductColor"] = setList[j].ProductColor;

                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();
                    report.Load(Server.MapPath("~/Report/Order/OrderReport.mrt"));
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                    report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                    report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                    report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                    report.Dictionary.Variables["SetNumber"].Value = setList.FirstOrDefault().SetNumber.ToString();
                    report.Dictionary.Variables["SetNumberTitle"].Value = setList.FirstOrDefault().SetNumber != null ? "سریال نیم ست" : "";
                    //report.Dictionary.Synchronize();
                    report.Compile();
                    report.Render(false);

                    reports.Add(report);

                }

                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        /// <summary>
        /// چاپ گزارش سفارش ثبت شده با ارسال ردیف های محصولات
        /// </summary>
        /// <param name="id">ردیف محصولات</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, admin-order, order, allOrder,workshop")]
        public ActionResult PrintProductMin(string id, int type)
        {
            List<int> idList = id.Split(',').Select(x => int.Parse(x)).ToList();
            OrderPrintViewModel modelData = null;
            List<OrderDetail> result;
            using (var db = new KiaGalleryContext())
            {
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.ProductFileList).Where(x => idList.Any(y => y == x.Id));

                if (!User.IsInRole("admin") && !User.IsInRole("admin-order"))
                {
                    var user = GetAuthenticatedUser();
                    query = query.Where(x => x.Order.BranchId == user.BranchId);
                }
                if (User.IsInRole("leatherProductUser"))
                {
                    query = query.Where(x => x.Product.WorkshopId2 == 5 || x.Product.ProductType == ProductType.OuterWerk || x.Product.ProductType == ProductType.WatchPendent2 || x.Product.ProductType == ProductType.Plaque);
                }
                if (!User.IsInRole("leatherProductUser") && !User.IsInRole("admin") && !User.IsInRole("allOrder"))
                {
                    query = query.Where(x => x.Product.WorkshopId2 != 5 && x.Product.ProductType != ProductType.OuterWerk && x.Product.ProductType != ProductType.WatchPendent2 && x.Product.ProductType != ProductType.Plaque);
                }
                if (modelData != null && modelData.orderType != null && modelData.orderType > 0)
                {
                    switch (modelData.orderType)
                    {
                        case 1:
                            query = query.Where(x => string.IsNullOrEmpty(x.Customer));
                            break;
                        case 2:
                            query = query.Where(x => !string.IsNullOrEmpty(x.Customer));
                            break;
                    }
                }

                if (modelData != null && modelData.workshopList != null && modelData.workshopList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => modelData.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId));
                }

                if (modelData != null && modelData.statusList != null && modelData.statusList.Count() > 0)
                {
                    query = query.Where(x => modelData.statusList.Any(y => y == x.OrderDetailStatus));
                }

                result = query.OrderBy(x => x.Product.Id).ToList();

                var serialList = result.OrderBy(x => x.Order.OrderNumber).Select(x => x.Order.OrderSerial).Distinct().ToList();
                var orderNumberList = result.Select(x => x.Order.OrderNumber).Distinct().ToList();
                var dateList = result.OrderBy(x => x.Order.CreateDate).Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var branchNameList = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    Image = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order)?.FileName,
                    Title = item.Product.Title,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = item.Order.OrderNumber,
                    Code = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    WorkshopId2 = item.Product.WorkshopId2,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    Size2 = item.Size2,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => "س " + x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => "چ " + x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    Description = item.Description,
                    Description2 = item.Description2,
                    SetNumber = item.SetNumber,
                    BranchName = item.Order.Branch.Name,
                    Date = item.Order.CreateDate,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""
                })
                .ToList()
                .GroupBy(x => new
                {
                    x.Image,
                    x.Title,
                    x.Status,
                    x.OrderNumber,
                    x.Code,
                    x.Type,
                    x.WorkshopId,
                    x.Workshop,
                    x.WorkshopId2,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.Size2,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.BranchName,
                    x.Date,
                    x.ProductColor
                })
                .Select(x => new
                {
                    x.Key.Image,
                    x.Key.Title,
                    x.Key.Status,
                    x.Key.OrderNumber,
                    x.Key.Code,
                    x.Key.Type,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.WorkshopId2,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.Size2,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    x.Key.BranchName,
                    x.Key.Date,
                    Count = x.Count(),
                    x.Key.ProductColor
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();
                List<OrderPrintDataViewModel> setDataPrint = new List<OrderPrintDataViewModel>();
                foreach (var itemWorkshop in data.GroupBy(x => x.WorkshopId))
                {

                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();

                    foreach (var item in itemWorkshop)
                    {
                        if (item.SetNumber == null)
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = " گرم " + item.Weight,
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Size2 = string.IsNullOrEmpty(item.Size2) ? " " : item.Size2,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                BranchName = item.BranchName,
                                Date = item.Date,
                                ProductColor = item.ProductColor
                            };

                            dataPrint.Add(itemPrint);
                        }
                        else
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = item.Weight + " گرم",
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Size2 = string.IsNullOrEmpty(item.Size) ? "" : item.Size2,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                BranchName = item.BranchName,
                                Date = item.Date,
                                ProductColor = item.ProductColor
                            };

                            setDataPrint.Add(itemPrint);
                        }

                    }
                    if (dataPrint.Count > 0)
                    {
                        dataPrint = dataPrint.OrderBy(x => x.OrderNumber).ToList();

                        DataSet dataset = new DataSet("DataSource");
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Row");
                        dataTable.Columns.Add("Image", typeof(byte[]));
                        dataTable.Columns.Add("Title");
                        dataTable.Columns.Add("Status");
                        dataTable.Columns.Add("OrderNumber");
                        dataTable.Columns.Add("Code");
                        dataTable.Columns.Add("Type");
                        dataTable.Columns.Add("Workshop");
                        dataTable.Columns.Add("Workshop2");
                        dataTable.Columns.Add("Weight");
                        dataTable.Columns.Add("GoldType");
                        dataTable.Columns.Add("OuterWerkType");
                        dataTable.Columns.Add("Count");
                        dataTable.Columns.Add("Size");
                        dataTable.Columns.Add("Size2");
                        dataTable.Columns.Add("Stone");
                        dataTable.Columns.Add("Leather");
                        dataTable.Columns.Add("Customer");
                        dataTable.Columns.Add("Description");
                        dataTable.Columns.Add("Description2");
                        dataTable.Columns.Add("BranchName");
                        dataTable.Columns.Add("Date");
                        dataTable.Columns.Add("ProductColor");


                        for (int j = 0; j < dataPrint.Count; j++)
                        {
                            DataRow row = dataTable.NewRow();
                            row["Row"] = j + 1;
                            row["Image"] = dataPrint[j].Image;
                            row["Title"] = dataPrint[j].Title;
                            row["Status"] = dataPrint[j].Status;
                            row["OrderNumber"] = dataPrint[j].OrderNumber;
                            row["Code"] = dataPrint[j].Code;
                            row["Type"] = dataPrint[j].Type;
                            row["Workshop"] = dataPrint[j].Workshop;
                            row["Workshop2"] = dataPrint[j].Workshop2;
                            row["Weight"] = dataPrint[j].Weight;
                            row["GoldType"] = dataPrint[j].GoldType;
                            row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                            row["Count"] = dataPrint[j].Count;
                            row["Size"] = dataPrint[j].Size;
                            row["Size2"] = dataPrint[j].Size2;
                            row["Stone"] = dataPrint[j].Stone;
                            row["Leather"] = dataPrint[j].Leather;
                            row["Customer"] = dataPrint[j].Customer;
                            row["Description"] = dataPrint[j].Description;
                            row["Description2"] = dataPrint[j].Description2;
                            row["BranchName"] = dataPrint[j].BranchName;
                            row["Date"] = DateUtility.GetPersianDate(dataPrint[j].Date);
                            row["ProductColor"] = dataPrint[j].ProductColor;

                            dataTable.Rows.Add(row);
                        }
                        dataset.Tables.Add(dataTable);

                        StiReport report = new StiReport();
                        if (type == 1)
                        {
                            report.Load(Server.MapPath("~/Report/Order/OrderReportAdmin.mrt"));

                        }
                        else
                        {
                            report.Load(Server.MapPath("~/Report/Order/OrderReportMin.mrt"));

                        }

                        report.Dictionary.Databases.Clear();
                        report.ScriptLanguage = StiReportLanguageType.CSharp;
                        report.RegData("DataSource", dataset.Tables[0].DefaultView);
                        report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                        report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                        report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                        report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                        report.Dictionary.Variables["SetNumber"].Value = "";
                        report.Dictionary.Variables["SetNumberTitle"].Value = "";
                        //report.Dictionary.Synchronize();
                        report.Compile();
                        report.Render(false);

                        reports.Add(report);
                    }


                }
                foreach (var itemSet in setDataPrint.GroupBy(x => x.SetNumber))
                {
                    var setList = setDataPrint.Where(x => x.SetNumber == itemSet.Key).ToList().OrderBy(x => x.Code).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Image", typeof(byte[]));
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("OrderNumber");
                    dataTable.Columns.Add("Code");
                    dataTable.Columns.Add("Type");
                    dataTable.Columns.Add("Workshop");
                    dataTable.Columns.Add("Workshop2");
                    dataTable.Columns.Add("Weight");
                    dataTable.Columns.Add("GoldType");
                    dataTable.Columns.Add("OuterWerkType");
                    dataTable.Columns.Add("Count");
                    dataTable.Columns.Add("Size");
                    dataTable.Columns.Add("Stone");
                    dataTable.Columns.Add("Leather");
                    dataTable.Columns.Add("Customer");
                    dataTable.Columns.Add("Description");
                    dataTable.Columns.Add("Description2");
                    dataTable.Columns.Add("BranchName");
                    dataTable.Columns.Add("Date");
                    dataTable.Columns.Add("ProductColor");


                    for (int j = 0; j < setList.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Image"] = setList[j].Image;
                        row["Title"] = setList[j].Title;
                        row["Status"] = setList[j].Status;
                        row["OrderNumber"] = setList[j].OrderNumber;
                        row["Code"] = setList[j].Code;
                        row["Type"] = setList[j].Type;
                        row["Workshop"] = setList[j].Workshop;
                        row["Workshop2"] = setList[j].Workshop2;
                        row["Weight"] = setList[j].Weight;
                        row["GoldType"] = setList[j].GoldType;
                        row["OuterWerkType"] = setList[j].OuterWerkType;
                        row["Count"] = setList[j].Count;
                        row["Size"] = setList[j].Size;
                        row["Stone"] = setList[j].Stone;
                        row["Leather"] = setList[j].Leather;
                        row["Customer"] = setList[j].Customer;
                        row["Description"] = setList[j].Description;
                        row["Description2"] = setList[j].Description2;
                        row["BranchName"] = setList[j].BranchName;
                        row["Date"] = DateUtility.GetPersianDate(setList[j].Date);
                        row["ProductColor"] = setList[j].ProductColor;


                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();
                    if (User.IsInRole("admin") || User.IsInRole("allOrder"))
                    {
                        report.Load(Server.MapPath("~/Report/Order/OrderReportAdmin.mrt"));

                    }
                    else
                    {
                        report.Load(Server.MapPath("~/Report/Order/OrderReportMin.mrt"));

                    }
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                    report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                    report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                    report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                    report.Dictionary.Variables["SetNumber"].Value = setList.FirstOrDefault().SetNumber.ToString();
                    report.Dictionary.Variables["SetNumberTitle"].Value = setList.FirstOrDefault().SetNumber != null ? "سریال نیم ست" : "";
                    //report.Dictionary.Synchronize();
                    report.Compile();
                    report.Render(false);

                    reports.Add(report);

                }

                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                //this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        [AllowAnonymous]
        public ActionResult PrintWorkshopProductMin(string id, int type)
        {
            List<int> idList = id.Split(',').Select(x => int.Parse(x)).ToList();
            OrderPrintViewModel modelData = null;
            List<OrderDetail> result;
            using (var db = new KiaGalleryContext())
            {
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.ProductFileList).Where(x => idList.Any(y => y == x.Id));

                if (modelData != null && modelData.orderType != null && modelData.orderType > 0)
                {
                    switch (modelData.orderType)
                    {
                        case 1:
                            query = query.Where(x => string.IsNullOrEmpty(x.Customer));
                            break;
                        case 2:
                            query = query.Where(x => !string.IsNullOrEmpty(x.Customer));
                            break;
                    }
                }

                if (modelData != null && modelData.workshopList != null && modelData.workshopList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => modelData.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId));
                }

                if (modelData != null && modelData.statusList != null && modelData.statusList.Count() > 0)
                {
                    query = query.Where(x => modelData.statusList.Any(y => y == x.OrderDetailStatus));
                }

                result = query.OrderBy(x => x.Product.Id).ToList();

                var serialList = result.OrderBy(x => x.Order.OrderNumber).Select(x => x.Order.OrderSerial).Distinct().ToList();
                var orderNumberList = result.Select(x => x.Order.OrderNumber).Distinct().ToList();
                var dateList = result.OrderBy(x => x.Order.CreateDate).Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var branchNameList = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    Image = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order)?.FileName,
                    Title = item.Product.Title,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = item.Order.OrderNumber,
                    Code = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    WorkshopId2 = item.Product.WorkshopId2,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    Size2 = item.Size2,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => "س " + x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => "چ " + x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    Description = item.Description,
                    Description2 = item.Description2,
                    SetNumber = item.SetNumber,
                    BranchName = item.Order.Branch.Name,
                    Date = item.Order.CreateDate,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""
                })
                .ToList()
                .GroupBy(x => new
                {
                    x.Image,
                    x.Title,
                    x.Status,
                    x.OrderNumber,
                    x.Code,
                    x.Type,
                    x.WorkshopId,
                    x.Workshop,
                    x.WorkshopId2,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.Size2,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.BranchName,
                    x.Date,
                    x.ProductColor
                })
                .Select(x => new
                {
                    x.Key.Image,
                    x.Key.Title,
                    x.Key.Status,
                    x.Key.OrderNumber,
                    x.Key.Code,
                    x.Key.Type,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.WorkshopId2,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.Size2,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    x.Key.BranchName,
                    x.Key.Date,
                    Count = x.Count(),
                    x.Key.ProductColor
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();
                List<OrderPrintDataViewModel> setDataPrint = new List<OrderPrintDataViewModel>();
                foreach (var itemWorkshop in data.GroupBy(x => x.WorkshopId))
                {

                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();

                    foreach (var item in itemWorkshop)
                    {
                        if (item.SetNumber == null)
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = " گرم " + item.Weight,
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Size2 = string.IsNullOrEmpty(item.Size2) ? " " : item.Size2,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                BranchName = item.BranchName,
                                Date = item.Date,
                                ProductColor = item.ProductColor
                            };

                            dataPrint.Add(itemPrint);
                        }
                        else
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = item.Weight + " گرم",
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Size2 = string.IsNullOrEmpty(item.Size) ? "" : item.Size2,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                BranchName = item.BranchName,
                                Date = item.Date,
                                ProductColor = item.ProductColor
                            };

                            setDataPrint.Add(itemPrint);
                        }

                    }
                    if (dataPrint.Count > 0)
                    {
                        dataPrint = dataPrint.OrderBy(x => x.OrderNumber).ToList();

                        DataSet dataset = new DataSet("DataSource");
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Row");
                        dataTable.Columns.Add("Image", typeof(byte[]));
                        dataTable.Columns.Add("Title");
                        dataTable.Columns.Add("Status");
                        dataTable.Columns.Add("OrderNumber");
                        dataTable.Columns.Add("Code");
                        dataTable.Columns.Add("Type");
                        dataTable.Columns.Add("Workshop");
                        dataTable.Columns.Add("Workshop2");
                        dataTable.Columns.Add("Weight");
                        dataTable.Columns.Add("GoldType");
                        dataTable.Columns.Add("OuterWerkType");
                        dataTable.Columns.Add("Count");
                        dataTable.Columns.Add("Size");
                        dataTable.Columns.Add("Size2");
                        dataTable.Columns.Add("Stone");
                        dataTable.Columns.Add("Leather");
                        dataTable.Columns.Add("Customer");
                        dataTable.Columns.Add("Description");
                        dataTable.Columns.Add("Description2");
                        dataTable.Columns.Add("BranchName");
                        dataTable.Columns.Add("Date");
                        dataTable.Columns.Add("ProductColor");


                        for (int j = 0; j < dataPrint.Count; j++)
                        {
                            DataRow row = dataTable.NewRow();
                            row["Row"] = j + 1;
                            row["Image"] = dataPrint[j].Image;
                            row["Title"] = dataPrint[j].Title;
                            row["Status"] = dataPrint[j].Status;
                            row["OrderNumber"] = dataPrint[j].OrderNumber;
                            row["Code"] = dataPrint[j].Code;
                            row["Type"] = dataPrint[j].Type;
                            row["Workshop"] = dataPrint[j].Workshop;
                            row["Workshop2"] = dataPrint[j].Workshop2;
                            row["Weight"] = dataPrint[j].Weight;
                            row["GoldType"] = dataPrint[j].GoldType;
                            row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                            row["Count"] = dataPrint[j].Count;
                            row["Size"] = dataPrint[j].Size;
                            row["Size2"] = dataPrint[j].Size2;
                            row["Stone"] = dataPrint[j].Stone;
                            row["Leather"] = dataPrint[j].Leather;
                            row["Customer"] = dataPrint[j].Customer;
                            row["Description"] = dataPrint[j].Description;
                            row["Description2"] = dataPrint[j].Description2;
                            row["BranchName"] = dataPrint[j].BranchName;
                            row["Date"] = DateUtility.GetPersianDate(dataPrint[j].Date);
                            row["ProductColor"] = dataPrint[j].ProductColor;

                            dataTable.Rows.Add(row);
                        }
                        dataset.Tables.Add(dataTable);

                        StiReport report = new StiReport();
                        if (type == 1)
                        {
                            report.Load(Server.MapPath("~/Report/Order/OrderReportAdmin.mrt"));

                        }
                        else
                        {
                            report.Load(Server.MapPath("~/Report/Order/OrderReportMin.mrt"));

                        }

                        report.Dictionary.Databases.Clear();
                        report.ScriptLanguage = StiReportLanguageType.CSharp;
                        report.RegData("DataSource", dataset.Tables[0].DefaultView);
                        report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                        report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                        report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                        report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                        report.Dictionary.Variables["SetNumber"].Value = "";
                        report.Dictionary.Variables["SetNumberTitle"].Value = "";
                        //report.Dictionary.Synchronize();
                        report.Compile();
                        report.Render(false);

                        reports.Add(report);
                    }


                }
                foreach (var itemSet in setDataPrint.GroupBy(x => x.SetNumber))
                {
                    var setList = setDataPrint.Where(x => x.SetNumber == itemSet.Key).ToList().OrderBy(x => x.Code).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Image", typeof(byte[]));
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("OrderNumber");
                    dataTable.Columns.Add("Code");
                    dataTable.Columns.Add("Type");
                    dataTable.Columns.Add("Workshop");
                    dataTable.Columns.Add("Workshop2");
                    dataTable.Columns.Add("Weight");
                    dataTable.Columns.Add("GoldType");
                    dataTable.Columns.Add("OuterWerkType");
                    dataTable.Columns.Add("Count");
                    dataTable.Columns.Add("Size");
                    dataTable.Columns.Add("Stone");
                    dataTable.Columns.Add("Leather");
                    dataTable.Columns.Add("Customer");
                    dataTable.Columns.Add("Description");
                    dataTable.Columns.Add("Description2");
                    dataTable.Columns.Add("BranchName");
                    dataTable.Columns.Add("Date");
                    dataTable.Columns.Add("ProductColor");


                    for (int j = 0; j < setList.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Image"] = setList[j].Image;
                        row["Title"] = setList[j].Title;
                        row["Status"] = setList[j].Status;
                        row["OrderNumber"] = setList[j].OrderNumber;
                        row["Code"] = setList[j].Code;
                        row["Type"] = setList[j].Type;
                        row["Workshop"] = setList[j].Workshop;
                        row["Workshop2"] = setList[j].Workshop2;
                        row["Weight"] = setList[j].Weight;
                        row["GoldType"] = setList[j].GoldType;
                        row["OuterWerkType"] = setList[j].OuterWerkType;
                        row["Count"] = setList[j].Count;
                        row["Size"] = setList[j].Size;
                        row["Stone"] = setList[j].Stone;
                        row["Leather"] = setList[j].Leather;
                        row["Customer"] = setList[j].Customer;
                        row["Description"] = setList[j].Description;
                        row["Description2"] = setList[j].Description2;
                        row["BranchName"] = setList[j].BranchName;
                        row["Date"] = DateUtility.GetPersianDate(setList[j].Date);
                        row["ProductColor"] = setList[j].ProductColor;


                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();
                    if (User.IsInRole("admin") || User.IsInRole("allOrder"))
                    {
                        report.Load(Server.MapPath("~/Report/Order/OrderReportAdmin.mrt"));

                    }
                    else
                    {
                        report.Load(Server.MapPath("~/Report/Order/OrderReportMin.mrt"));

                    }
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                    report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                    report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                    report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                    report.Dictionary.Variables["SetNumber"].Value = setList.FirstOrDefault().SetNumber.ToString();
                    report.Dictionary.Variables["SetNumberTitle"].Value = setList.FirstOrDefault().SetNumber != null ? "سریال نیم ست" : "";
                    //report.Dictionary.Synchronize();
                    report.Compile();
                    report.Render(false);

                    reports.Add(report);

                }

                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                //this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        /// <summary>
        /// چاپ گزارش سفارش ثبت شده با ارسال ردیف های سفارشات برای کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارشات</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, order-workshop")]
        public JsonResult changeStatusOrderDetail(string id)
        {
            List<int> idList = id.Split('-').Select(x => int.Parse(x)).ToList();

            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    List<OrderDetail> result = db.OrderDetail.Where(x => (x.OrderDetailStatus == OrderDetailStatus.InWorkshop || x.OrderDetailStatus == OrderDetailStatus.InWorkshop2) && (x.SendWorkshopOrder2 != true && x.Product.WorkshopId == user.WorkshopId && idList.Any(y => y == x.WorkshopOrderId)) || (x.SendWorkshopOrder2 == true && x.Product.WorkshopId2 == user.WorkshopId && idList.Any(y => y == x.WorkshopOrderId2))).OrderBy(x => x.Product.Id).ToList();

                    result.Where(y => y.OrderDetailStatus == OrderDetailStatus.InWorkshop).ToList().ForEach(x =>
                     {
                         x.OrderDetailStatus = OrderDetailStatus.UnderConstruction;
                         var detailLog = new OrderDetailLog()
                         {
                             OrderDetailId = x.Id,
                             OrderDetailStatus = OrderDetailStatus.UnderConstruction,
                             OrderDetailLogReasonId = null,
                             Description = null,
                             CreateUserId = user.Id,
                             CreateDate = DateTime.Now,
                             Ip = Request.UserHostAddress
                         };
                         db.OrderDetailLog.Add(detailLog);
                     });
                    result.Where(y => y.OrderDetailStatus == OrderDetailStatus.InWorkshop2).ToList().ForEach(x =>
                    {
                        x.OrderDetailStatus = OrderDetailStatus.UnderConstruction2;
                        var detailLog = new OrderDetailLog()
                        {
                            OrderDetailId = x.Id,
                            OrderDetailStatus = OrderDetailStatus.UnderConstruction2,
                            OrderDetailLogReasonId = null,
                            Description = null,
                            CreateUserId = user.Id,
                            CreateDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.OrderDetailLog.Add(detailLog);
                    });
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "فاکتور های انتخاب شده به حالت در حال ساخت منتقل شدند."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// چاپ گزارش سفارش ثبت شده با ارسال ردیف های سفارشات برای کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارشات</param>
        /// <returns></returns>
        //[Authorize(Roles = "admin, order-workshop")]
        public ActionResult PrintWorkshop(string id)
        {
            List<int> idList = id.Split('-').Select(x => int.Parse(x)).ToList();

            List<OrderDetail> result;
            using (var db = new KiaGalleryContext())
            {
                var user = GetAuthenticatedUser();
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.Workshop2).Include(x => x.Product.ProductFileList).Where(x => (x.SendWorkshopOrder2 != true && x.Product.WorkshopId == user.WorkshopId && idList.Any(y => y == x.WorkshopOrderId)) || (x.SendWorkshopOrder2 == true && x.Product.WorkshopId2 == user.WorkshopId && idList.Any(y => y == x.WorkshopOrderId2)));

                result = query.OrderBy(x => x.Product.Id).ToList();

                result.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).OrderBy(x => x.Product.Id).ToList().ForEach(x =>
                {
                    x.OrderDetailStatus = OrderDetailStatus.UnderConstruction;
                    var detailLog = new OrderDetailLog()
                    {
                        OrderDetailId = x.Id,
                        OrderDetailStatus = OrderDetailStatus.UnderConstruction,
                        OrderDetailLogReasonId = null,
                        Description = null,
                        CreateUserId = user.Id,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.OrderDetailLog.Add(detailLog);
                });
                result.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop2).OrderBy(x => x.Product.Id).ToList().ForEach(x =>
                {
                    x.OrderDetailStatus = OrderDetailStatus.UnderConstruction2;
                    var detailLog = new OrderDetailLog()
                    {
                        OrderDetailId = x.Id,
                        OrderDetailStatus = OrderDetailStatus.UnderConstruction2,
                        OrderDetailLogReasonId = null,
                        Description = null,
                        CreateUserId = user.Id,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.OrderDetailLog.Add(detailLog);
                });
                db.SaveChanges();

                var serialList = result.Where(x => x.SendWorkshopOrder2 != true).OrderBy(x => x.Order.OrderNumber).Select(x => x.WorkshopOrder.WorkshopOrderSerial).Distinct().ToList();
                serialList.AddRange(result.Where(x => x.SendWorkshopOrder2 == true).Select(x => x.WorkshopOrder2.WorkshopOrderSerial).Distinct().ToList());
                var orderNumberList = result.OrderBy(x => x.Order.OrderNumber).Select(x => x.Order.OrderNumber).Distinct().ToList();
                //var workshopNumberList = result.Where(x => x.SendWorkshopOrder2 != true).Select(x => "KIA-" + x.WorkshopOrder.WorkshopOrderNumber).Distinct().ToList();
                //workshopNumberList.AddRange(result.Where(x => x.SendWorkshopOrder2 == true).Select(x => "KIA-" + x.WorkshopOrder2.WorkshopOrderNumber).Distinct().ToList());
                var dateList = result.OrderBy(x => x.Order.CreateDate).Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var branchNameList = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    Image = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order)?.FileName,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = "KIA-" + item.Order.OrderNumber,
                    Title = item.Product.Title,
                    ProductSize = item.Product.SizeId,
                    SetNumber = item.SetNumber,
                    Code = item.Product.BookCode,
                    BookCode = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    ProductType = item.Product.ProductType,
                    TypeNumber = item.Product.ProductType,
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    Size2 = item.Size2,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => "سنگ " + x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => "چرم " + x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    Description = item.Description,
                    Description2 = item.Description2,
                    BranchName = item.Order.Branch.Name,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""
                })
                .ToList()
                .GroupBy(x => new
                {
                    x.Image,
                    x.Status,
                    x.OrderNumber,
                    x.Title,
                    x.Code,
                    x.BookCode,
                    x.Type,
                    x.ProductType,
                    x.TypeNumber,
                    x.WorkshopId,
                    x.Workshop,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.Size2,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.BranchName,
                    x.ProductSize,
                    x.ProductColor
                })
                .Select(x => new
                {
                    x.Key.Image,
                    x.Key.Status,
                    x.Key.OrderNumber,
                    x.Key.Title,
                    x.Key.Code,
                    x.Key.BookCode,
                    x.Key.Type,
                    x.Key.ProductType,
                    x.Key.TypeNumber,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.Size2,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    x.Key.BranchName,
                    x.Key.ProductSize,
                    Count = x.Count(),
                    x.Key.ProductColor
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();
                foreach (var itemWorkshop in data.GroupBy(x => new { x.WorkshopId, x.SetNumber }))
                {
                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();
                    foreach (var item in itemWorkshop)
                    {
                        OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                        {
                            Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                            Status = item.Status,
                            BranchName = item.BranchName,
                            OrderNumber = item.OrderNumber,
                            Title = item.Title,
                            Code = item.Code,
                            BookCode = item.BookCode,
                            Type = item.Type,
                            ProductType = item.ProductType,
                            TypeNumber = item.TypeNumber,
                            Workshop = item.Workshop,
                            Workshop2 = item.Workshop2,
                            Weight = item.Weight + " گرم",
                            GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                            OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                            Count = item.Count.ToString(),
                            Size = string.IsNullOrEmpty(item.Size) ? item.ProductSize != null ? "مشخص نشده" : "-" : item.Size,
                            Size2 = string.IsNullOrEmpty(item.Size2) ? item.ProductSize != null ? " " : "-" : item.Size2,
                            Stone = GetStoneText(item.StoneList),
                            Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                            Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                            Description = GetDescriptionText(item.BranchLabel, item.Description),
                            Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                            ProductColor = item.ProductColor
                        };

                        dataPrint.Add(itemPrint);
                    }

                    dataPrint = dataPrint.OrderBy(x => x.Code.StartsWith("E") ? 0 : 1)
                                          .ThenBy(x => x.Code.StartsWith("N") ? 0 : 1).ThenBy(x => x.Code)
                                          .ThenBy(x => x.Code.StartsWith("R") ? 0 : 1).ThenBy(x => x.Code)
                                          .ThenBy(x => x.Code.StartsWith("B") ? 0 : 1).ThenBy(x => x.Code)
                                          .ThenBy(x => x.Code.StartsWith("L") ? 0 : 1).ThenBy(x => x.Code)
                                          .ThenBy(x => x.Code.StartsWith("W") ? 0 : 1).ThenBy(x => x.Code)
                                          .ThenBy(x => x.Code.StartsWith("A") ? 0 : 1).ThenBy(x => x.Code)
                                         .ThenBy(x => x.Code.StartsWith("KH") ? 0 : 1).ThenBy(x => x.Code)
                                          .ThenBy(x => x.Code.StartsWith("S") ? 0 : 1).ThenBy(x => x.Code).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Image", typeof(byte[]));
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("OrderNumber");
                    dataTable.Columns.Add("Code");
                    dataTable.Columns.Add("Type");
                    dataTable.Columns.Add("Workshop");
                    dataTable.Columns.Add("Workshop2");
                    dataTable.Columns.Add("Weight");
                    dataTable.Columns.Add("GoldType");
                    dataTable.Columns.Add("OuterWerkType");
                    dataTable.Columns.Add("Count");
                    dataTable.Columns.Add("Size");
                    dataTable.Columns.Add("Size2");
                    dataTable.Columns.Add("Stone");
                    dataTable.Columns.Add("Leather");
                    dataTable.Columns.Add("Customer");
                    dataTable.Columns.Add("Description");
                    dataTable.Columns.Add("Description2");
                    dataTable.Columns.Add("BranchName");
                    dataTable.Columns.Add("ProductColor");

                    for (int j = 0; j < dataPrint.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Image"] = dataPrint[j].Image;
                        row["Title"] = dataPrint[j].Title;
                        row["Status"] = dataPrint[j].Status;
                        row["OrderNumber"] = dataPrint[j].OrderNumber;
                        row["Code"] = dataPrint[j].Code;
                        row["Type"] = dataPrint[j].Type;
                        row["Workshop"] = dataPrint[j].Workshop;
                        row["Workshop2"] = dataPrint[j].Workshop2;
                        row["Weight"] = dataPrint[j].Weight;
                        row["GoldType"] = dataPrint[j].GoldType;
                        row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                        row["Count"] = dataPrint[j].Count;
                        row["Size"] = dataPrint[j].Size;
                        row["Size2"] = dataPrint[j].Size2;
                        row["Stone"] = dataPrint[j].Stone;
                        row["Leather"] = dataPrint[j].Leather;
                        row["Customer"] = dataPrint[j].Customer;
                        row["Description"] = dataPrint[j].Description;
                        row["Description2"] = dataPrint[j].Description2;
                        row["BranchName"] = dataPrint[j].BranchName;
                        row["ProductColor"] = dataPrint[j].ProductColor;

                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();
                    report.Load(Server.MapPath("~/Report/Order/OrderReportMin.mrt"));
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                    report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                    report.Dictionary.Variables["OrderSerial"].Value = string.Join(" , ", serialList);
                    report.Dictionary.Variables["OrderNumber"].Value = string.Join(" , ", orderNumberList);
                    report.Dictionary.Variables["SetNumber"].Value = itemWorkshop.FirstOrDefault().SetNumber.ToString();
                    report.Dictionary.Variables["SetNumberTitle"].Value = itemWorkshop.FirstOrDefault().SetNumber != null ? "سریال نیم ست" : "";
                    //report.Dictionary.Synchronize();
                    report.Compile();
                    report.Render(false);

                    reports.Add(report);
                }

                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        public ActionResult PrintWorkshopProduct(string id)
        {
            List<int> idList = id.Split('-').Select(x => int.Parse(x)).ToList();
            OrderPrintViewModel modelData = null;
            List<OrderDetail> result;
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.ProductFileList).Where(x => idList.Any(y => y == x.Id) && x.Product.WorkshopId == user.WorkshopId && x.Product.WorkshopId2 == user.WorkshopId);

                if (modelData != null && modelData.orderType != null && modelData.orderType > 0)
                {
                    switch (modelData.orderType)
                    {
                        case 1:
                            query = query.Where(x => string.IsNullOrEmpty(x.Customer));
                            break;
                        case 2:
                            query = query.Where(x => !string.IsNullOrEmpty(x.Customer));
                            break;
                    }
                }

                if (modelData != null && modelData.workshopList != null && modelData.workshopList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => modelData.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId));
                }

                if (modelData != null && modelData.statusList != null && modelData.statusList.Count() > 0)
                {
                    query = query.Where(x => modelData.statusList.Any(y => y == x.OrderDetailStatus));
                }

                result = query.OrderBy(x => x.Product.Id).ToList();

                var serialList = result.Select(x => "KIA-" + x.Order.OrderSerial).Distinct().ToList();
                var orderNumberList = result.Select(x => "KIA-" + x.Order.OrderNumber).Distinct().ToList();
                var dateList = result.Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var branchNameList = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    Image = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order)?.FileName,
                    Title = item.Product.Title,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = "KIA-" + item.Order.OrderNumber,
                    Code = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    WorkshopId2 = item.Product.WorkshopId2,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => "سنگ " + x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => "چرم " + x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    Description = item.Description,
                    Description2 = item.Description2,
                    SetNumber = item.SetNumber,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""
                })
                .ToList()
                .GroupBy(x => new
                {
                    x.Image,
                    x.Title,
                    x.Status,
                    x.OrderNumber,
                    x.Code,
                    x.Type,
                    x.WorkshopId,
                    x.Workshop,
                    x.WorkshopId2,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.ProductColor
                })
                .Select(x => new
                {
                    x.Key.Image,
                    x.Key.Title,
                    x.Key.Status,
                    x.Key.OrderNumber,
                    x.Key.Code,
                    x.Key.Type,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.WorkshopId2,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    Count = x.Count(),
                    x.Key.ProductColor
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();
                List<OrderPrintDataViewModel> setDataPrint = new List<OrderPrintDataViewModel>();
                foreach (var itemWorkshop in data.GroupBy(x => x.WorkshopId))
                {

                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();

                    foreach (var item in itemWorkshop)
                    {
                        if (item.SetNumber == null)
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = " گرم " + item.Weight,
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                ProductColor = item.ProductColor
                            };

                            dataPrint.Add(itemPrint);
                        }
                        else
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = item.Weight + " گرم",
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                SetNumber = item.SetNumber.ToString(),
                                ProductColor = item.ProductColor
                            };

                            setDataPrint.Add(itemPrint);
                        }

                    }
                    if (dataPrint.Count > 0)
                    {
                        dataPrint = dataPrint.OrderBy(x => x.Code).ToList();

                        DataSet dataset = new DataSet("DataSource");
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Row");
                        dataTable.Columns.Add("Image", typeof(byte[]));
                        dataTable.Columns.Add("Title");
                        dataTable.Columns.Add("Status");
                        dataTable.Columns.Add("OrderNumber");
                        dataTable.Columns.Add("Code");
                        dataTable.Columns.Add("Type");
                        dataTable.Columns.Add("Workshop");
                        dataTable.Columns.Add("Workshop2");
                        dataTable.Columns.Add("Weight");
                        dataTable.Columns.Add("GoldType");
                        dataTable.Columns.Add("OuterWerkType");
                        dataTable.Columns.Add("Count");
                        dataTable.Columns.Add("Size");
                        dataTable.Columns.Add("Stone");
                        dataTable.Columns.Add("Leather");
                        dataTable.Columns.Add("Customer");
                        dataTable.Columns.Add("Description");
                        dataTable.Columns.Add("Description2");
                        dataTable.Columns.Add("ProductColor");

                        for (int j = 0; j < dataPrint.Count; j++)
                        {
                            DataRow row = dataTable.NewRow();
                            row["Row"] = j + 1;
                            row["Image"] = dataPrint[j].Image;
                            row["Title"] = dataPrint[j].Title;
                            row["Status"] = dataPrint[j].Status;
                            row["OrderNumber"] = dataPrint[j].OrderNumber;
                            row["Code"] = dataPrint[j].Code;
                            row["Type"] = dataPrint[j].Type;
                            row["Workshop"] = dataPrint[j].Workshop;
                            row["Workshop2"] = dataPrint[j].Workshop2;
                            row["Weight"] = dataPrint[j].Weight;
                            row["GoldType"] = dataPrint[j].GoldType;
                            row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                            row["Count"] = dataPrint[j].Count;
                            row["Size"] = dataPrint[j].Size;
                            row["Stone"] = dataPrint[j].Stone;
                            row["Leather"] = dataPrint[j].Leather;
                            row["Customer"] = dataPrint[j].Customer;
                            row["Description"] = dataPrint[j].Description;
                            row["Description2"] = dataPrint[j].Description2;
                            row["ProductColor"] = dataPrint[j].ProductColor;

                            dataTable.Rows.Add(row);
                        }
                        dataset.Tables.Add(dataTable);

                        StiReport report = new StiReport();
                        report.Load(Server.MapPath("~/Report/Order/OrderReport.mrt"));
                        report.Dictionary.Databases.Clear();
                        report.ScriptLanguage = StiReportLanguageType.CSharp;
                        report.RegData("DataSource", dataset.Tables[0].DefaultView);
                        report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                        report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                        report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                        report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                        report.Dictionary.Variables["SetNumber"].Value = "";
                        report.Dictionary.Variables["SetNumberTitle"].Value = "";
                        //report.Dictionary.Synchronize();
                        report.Compile();
                        report.Render(false);

                        reports.Add(report);
                    }


                }
                foreach (var itemSet in setDataPrint.GroupBy(x => x.SetNumber))
                {
                    var setList = setDataPrint.Where(x => x.SetNumber == itemSet.Key).ToList().OrderBy(x => x.Code).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Image", typeof(byte[]));
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("OrderNumber");
                    dataTable.Columns.Add("Code");
                    dataTable.Columns.Add("Type");
                    dataTable.Columns.Add("Workshop");
                    dataTable.Columns.Add("Workshop2");
                    dataTable.Columns.Add("Weight");
                    dataTable.Columns.Add("GoldType");
                    dataTable.Columns.Add("OuterWerkType");
                    dataTable.Columns.Add("Count");
                    dataTable.Columns.Add("Size");
                    dataTable.Columns.Add("Stone");
                    dataTable.Columns.Add("Leather");
                    dataTable.Columns.Add("Customer");
                    dataTable.Columns.Add("Description");
                    dataTable.Columns.Add("Description2");
                    dataTable.Columns.Add("ProductColor");

                    for (int j = 0; j < setList.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Image"] = setList[j].Image;
                        row["Title"] = setList[j].Title;
                        row["Status"] = setList[j].Status;
                        row["OrderNumber"] = setList[j].OrderNumber;
                        row["Code"] = setList[j].Code;
                        row["Type"] = setList[j].Type;
                        row["Workshop"] = setList[j].Workshop;
                        row["Workshop2"] = setList[j].Workshop2;
                        row["Weight"] = setList[j].Weight;
                        row["GoldType"] = setList[j].GoldType;
                        row["OuterWerkType"] = setList[j].OuterWerkType;
                        row["Count"] = setList[j].Count;
                        row["Size"] = setList[j].Size;
                        row["Stone"] = setList[j].Stone;
                        row["Leather"] = setList[j].Leather;
                        row["Customer"] = setList[j].Customer;
                        row["Description"] = setList[j].Description;
                        row["Description2"] = setList[j].Description2;
                        row["ProductColor"] = setList[j].ProductColor;

                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();
                    report.Load(Server.MapPath("~/Report/Order/OrderReport.mrt"));
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                    report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                    report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                    report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                    report.Dictionary.Variables["SetNumber"].Value = setList.FirstOrDefault().SetNumber.ToString();
                    report.Dictionary.Variables["SetNumberTitle"].Value = setList.FirstOrDefault().SetNumber != null ? "سریال نیم ست" : "";
                    //report.Dictionary.Synchronize();
                    report.Compile();
                    report.Render(false);

                    reports.Add(report);

                }

                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        public ActionResult PrintWorkshopStone(string id)
        {
            List<int> idList = id.Split('-').Select(x => int.Parse(x)).ToList();
            OrderPrintViewModel modelData = null;
            List<OrderDetail> result;
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.ProductFileList).Where(x => idList.Any(y => y == x.Id) && x.Product.WorkshopId == user.WorkshopId);

                if (modelData != null && modelData.orderType != null && modelData.orderType > 0)
                {
                    switch (modelData.orderType)
                    {
                        case 1:
                            query = query.Where(x => string.IsNullOrEmpty(x.Customer));
                            break;
                        case 2:
                            query = query.Where(x => !string.IsNullOrEmpty(x.Customer));
                            break;
                    }
                }

                if (modelData != null && modelData.workshopList != null && modelData.workshopList.Count(y => y > 0) > 0)
                {
                    query = query.Where(x => modelData.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId));
                }

                if (modelData != null && modelData.statusList != null && modelData.statusList.Count() > 0)
                {
                    query = query.Where(x => modelData.statusList.Any(y => y == x.OrderDetailStatus));
                }

                result = query.OrderBy(x => x.Product.Id).ToList();

                var serialList = result.Select(x => x.Order.OrderSerial).Distinct().ToList();
                var orderNumberList = result.Select(x => x.Order.OrderNumber).Distinct().ToList();
                var dateList = result.Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var branchNameList = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    //Image = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order)?.FileName,
                    Title = item.Product.Title,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = item.Order.OrderNumber,
                    Code = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    WorkshopId2 = item.Product.WorkshopId2,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => "سنگ " + x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => "چرم " + x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    Description = item.Description,
                    Description2 = item.Description2,
                    SetNumber = item.SetNumber,
                    BranchName = item.Order.Branch.Name,
                    Date = item.Order.CreateDate,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""
                })
                .ToList()
                .GroupBy(x => new
                {
                    //x.Image,
                    x.Title,
                    x.Status,
                    x.OrderNumber,
                    x.Code,
                    x.Type,
                    x.WorkshopId,
                    x.Workshop,
                    x.WorkshopId2,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.BranchName,
                    x.Date,
                    x.ProductColor
                })
                .Select(x => new
                {
                    //x.Key.Image,
                    x.Key.Title,
                    x.Key.Status,
                    x.Key.OrderNumber,
                    x.Key.Code,
                    x.Key.Type,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.WorkshopId2,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    x.Key.BranchName,
                    x.Key.Date,
                    Count = x.Count(),
                    x.Key.ProductColor
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();
                List<OrderPrintDataViewModel> setDataPrint = new List<OrderPrintDataViewModel>();
                foreach (var itemWorkshop in data.GroupBy(x => x.WorkshopId))
                {

                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();

                    foreach (var item in itemWorkshop)
                    {
                        if (item.SetNumber == null)
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                //Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = " گرم " + item.Weight,
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                BranchName = item.BranchName,
                                SetNumber = item.SetNumber.ToString(),
                                Date = item.Date,
                                ProductColor = item.ProductColor
                            };

                            dataPrint.Add(itemPrint);
                        }
                        else
                        {
                            OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                            {
                                //Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                                Title = item.Title,
                                Status = item.Status,
                                OrderNumber = item.OrderNumber,
                                Code = item.Code,
                                Type = item.Type,
                                Workshop = item.Workshop,
                                Workshop2 = item.Workshop2,
                                Weight = item.Weight + " گرم",
                                GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                                OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                                Count = item.Count.ToString(),
                                Size = string.IsNullOrEmpty(item.Size) ? "مشخص نشده" : item.Size,
                                Stone = GetStoneText(item.StoneList),
                                Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                                Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                                Description = GetDescriptionText(item.BranchLabel, item.Description),
                                Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                                BranchName = item.BranchName,
                                Date = item.Date,
                                SetNumber = item.SetNumber.ToString(),
                                ProductColor = item.ProductColor
                            };

                            setDataPrint.Add(itemPrint);
                        }

                    }
                    if (dataPrint.Count > 0)
                    {
                        dataPrint = dataPrint.OrderBy(x => x.Code).ToList();

                        DataSet dataset = new DataSet("DataSource");
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Row");
                        //dataTable.Columns.Add("Image", typeof(byte[]));
                        dataTable.Columns.Add("Title");
                        dataTable.Columns.Add("Status");
                        dataTable.Columns.Add("OrderNumber");
                        dataTable.Columns.Add("Code");
                        dataTable.Columns.Add("Type");
                        dataTable.Columns.Add("Workshop");
                        dataTable.Columns.Add("Workshop2");
                        dataTable.Columns.Add("Weight");
                        dataTable.Columns.Add("GoldType");
                        dataTable.Columns.Add("OuterWerkType");
                        dataTable.Columns.Add("Count");
                        dataTable.Columns.Add("Size");
                        dataTable.Columns.Add("Stone");
                        dataTable.Columns.Add("Leather");
                        dataTable.Columns.Add("Customer");
                        dataTable.Columns.Add("Description");
                        dataTable.Columns.Add("Description2");
                        dataTable.Columns.Add("BranchName");
                        dataTable.Columns.Add("Date");
                        dataTable.Columns.Add("ProductColor");

                        for (int j = 0; j < dataPrint.Count; j++)
                        {
                            DataRow row = dataTable.NewRow();
                            row["Row"] = j + 1;
                            //row["Image"] = dataPrint[j].Image;
                            row["Title"] = dataPrint[j].Title;
                            row["Status"] = dataPrint[j].Status;
                            row["OrderNumber"] = dataPrint[j].OrderNumber;
                            row["Code"] = dataPrint[j].Code;
                            row["Type"] = dataPrint[j].Type;
                            row["Workshop"] = dataPrint[j].Workshop;
                            row["Workshop2"] = dataPrint[j].Workshop2;
                            row["Weight"] = dataPrint[j].Weight;
                            row["GoldType"] = dataPrint[j].GoldType;
                            row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                            row["Count"] = dataPrint[j].Count;
                            row["Size"] = dataPrint[j].Size;
                            row["Stone"] = dataPrint[j].Stone;
                            row["Leather"] = dataPrint[j].Leather;
                            row["Customer"] = dataPrint[j].Customer;
                            row["Description"] = dataPrint[j].Description;
                            row["Description2"] = dataPrint[j].Description2;
                            row["BranchName"] = dataPrint[j].BranchName;
                            row["Date"] = DateUtility.GetPersianDate(dataPrint[j].Date);
                            row["ProductColor"] = dataPrint[j].ProductColor;

                            dataTable.Rows.Add(row);
                        }
                        dataset.Tables.Add(dataTable);

                        StiReport report = new StiReport();
                        report.Load(Server.MapPath("~/Report/Order/OrderReportMin.mrt"));
                        report.Dictionary.Databases.Clear();
                        report.ScriptLanguage = StiReportLanguageType.CSharp;
                        report.RegData("DataSource", dataset.Tables[0].DefaultView);
                        report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", branchNameList);
                        report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                        report.Dictionary.Variables["OrderSerial"].Value = string.Join(" - ", serialList);
                        report.Dictionary.Variables["OrderNumber"].Value = string.Join(" - ", orderNumberList);
                        report.Dictionary.Variables["SetNumber"].Value = "";
                        report.Dictionary.Variables["SetNumberTitle"].Value = "";
                        //report.Dictionary.Synchronize();
                        report.Compile();
                        report.Render(false);

                        reports.Add(report);
                    }


                }


                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        /// <summary>
        /// چاپ گزارش سفارش ثبت شده با ارسال ردیف های سفارشات برای کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارشات</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult PrintWorkshopReplica(string id)
        {
            List<int> idList = id.Split('-').Select(x => int.Parse(x)).ToList();

            List<OrderDetail> result;
            using (var db = new KiaGalleryContext())
            {
                var user = GetAuthenticatedUser();
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.ProductFileList).Where(x => (x.SendWorkshopOrder2 != true && x.Product.WorkshopId == user.WorkshopId && idList.Any(y => y == x.WorkshopOrderId)) || (x.SendWorkshopOrder2 == true && x.Product.WorkshopId2 == user.WorkshopId && idList.Any(y => y == x.WorkshopOrderId2)));

                result = query.OrderBy(x => x.Product.Id).ToList();

                result.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).ToList().ForEach(x =>
                {
                    x.OrderDetailStatus = OrderDetailStatus.UnderConstruction;
                    var detailLog = new OrderDetailLog()
                    {
                        OrderDetailId = x.Id,
                        OrderDetailStatus = OrderDetailStatus.UnderConstruction,
                        OrderDetailLogReasonId = null,
                        Description = null,
                        CreateUserId = user.Id,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };

                    db.OrderDetailLog.Add(detailLog);
                });
                db.SaveChanges();

                var serialList = result.OrderBy(x => x.Order.OrderSerial).Where(x => x.SendWorkshopOrder2 != true).Select(x => x.WorkshopOrder.WorkshopOrderSerial).Distinct().ToList();
                serialList.AddRange(result.Where(x => x.SendWorkshopOrder2 == true).Select(x => x.WorkshopOrder2.WorkshopOrderSerial).Distinct().ToList());
                var orderNumberList = result.OrderBy(x => x.Order.OrderNumber).Select(x => x.Order.OrderNumber).Distinct().ToList();
                //var workshopOrderNumberList = result.Where(x=> x.SendWorkshopOrder2 != true).Select(x => "KIA-" + x.WorkshopOrder.WorkshopOrderNumber).Distinct().ToList();
                //workshopOrderNumberList.AddRange(result.Where(x => x.SendWorkshopOrder2 == true).Select(x => "KIA-" + x.WorkshopOrder.WorkshopOrderNumber).Distinct().ToList());
                var dateList = result.OrderBy(x => x.Order.CreateDate).Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var orderBranchName = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    Image = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order)?.FileName,
                    Title = item.Product.Title,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = item.Order.OrderNumber,
                    ProductSize = item.Product.SizeId,
                    SetNumber = item.SetNumber,
                    Code = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    TypeNumber = item.Product.ProductType,
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    WorkshopId2 = item.Product.WorkshopId2,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    Size2 = item.Size2,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => "سنگ " + x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => "چرم " + x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    BranchName = item.Order.Branch.Name,
                    Description = item.Description,
                    Description2 = item.Description2,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""
                })
                .ToList()
                .GroupBy(x => new
                {
                    x.Image,
                    x.Title,
                    x.OrderNumber,
                    x.Status,
                    x.Code,
                    x.Type,
                    x.TypeNumber,
                    x.WorkshopId,
                    x.Workshop,
                    x.WorkshopId2,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.Size2,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.BranchName,
                    x.ProductSize,
                    x.ProductColor
                })
                .Select(x => new
                {
                    x.Key.Image,
                    x.Key.Title,
                    x.Key.OrderNumber,
                    x.Key.Status,
                    x.Key.Code,
                    x.Key.Type,
                    x.Key.TypeNumber,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.WorkshopId2,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.Size2,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    x.Key.BranchName,
                    x.Key.ProductSize,
                    Count = x.Count(),
                    x.Key.ProductColor
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();
                foreach (var itemWorkshop in data.GroupBy(x => new { x.WorkshopId, x.SetNumber }))
                {
                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();
                    foreach (var item in itemWorkshop)
                    {
                        OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                        {
                            Image = string.IsNullOrEmpty(item.Image) ? defaultImage : GetProductFileByte(item.Image),
                            Title = item.Title,
                            BranchName = item.BranchName,
                            Status = item.Status,
                            OrderNumber = item.OrderNumber,
                            Code = item.Code,
                            Type = item.Type,
                            TypeNumber = item.TypeNumber,
                            Workshop = item.Workshop,
                            Workshop2 = item.Workshop2,
                            Weight = item.Weight + " گرم",
                            GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                            OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                            Count = item.Count.ToString(),
                            Size = string.IsNullOrEmpty(item.Size) ? item.ProductSize != null ? "مشخص نشده" : "-" : item.Size,
                            Size2 = string.IsNullOrEmpty(item.Size2) ? item.ProductSize != null ? "" : "-" : item.Size2,
                            Stone = GetStoneText(item.StoneList),
                            Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                            Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                            Description = GetDescriptionText(item.BranchLabel, item.Description),
                            Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                            ProductColor = item.ProductColor
                        };

                        dataPrint.Add(itemPrint);
                    }

                    dataPrint = dataPrint.OrderBy(x => x.Code.StartsWith("E") ? 0 : 1)
                      .ThenBy(x => x.Code.StartsWith("N") ? 0 : 1).ThenBy(x => x.Code)
                      .ThenBy(x => x.Code.StartsWith("R") ? 0 : 1).ThenBy(x => x.Code)
                      .ThenBy(x => x.Code.StartsWith("B") ? 0 : 1).ThenBy(x => x.Code)
                      .ThenBy(x => x.Code.StartsWith("L") ? 0 : 1).ThenBy(x => x.Code)
                      .ThenBy(x => x.Code.StartsWith("W") ? 0 : 1).ThenBy(x => x.Code)
                      .ThenBy(x => x.Code.StartsWith("A") ? 0 : 1).ThenBy(x => x.Code)
                     .ThenBy(x => x.Code.StartsWith("KH") ? 0 : 1).ThenBy(x => x.Code)
                      .ThenBy(x => x.Code.StartsWith("S") ? 0 : 1).ThenBy(x => x.Code).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Image", typeof(byte[]));
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("OrderNumber");
                    dataTable.Columns.Add("Code");
                    dataTable.Columns.Add("Type");
                    dataTable.Columns.Add("Workshop");
                    dataTable.Columns.Add("Workshop2");
                    dataTable.Columns.Add("Weight");
                    dataTable.Columns.Add("GoldType");
                    dataTable.Columns.Add("OuterWerkType");
                    dataTable.Columns.Add("Count");
                    dataTable.Columns.Add("Size");
                    dataTable.Columns.Add("Size2");
                    dataTable.Columns.Add("Stone");
                    dataTable.Columns.Add("Leather");
                    dataTable.Columns.Add("Customer");
                    dataTable.Columns.Add("Description");
                    dataTable.Columns.Add("Description2");
                    dataTable.Columns.Add("BranchName");
                    dataTable.Columns.Add("ProductColor");

                    for (int j = 0; j < dataPrint.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Image"] = dataPrint[j].Image;
                        row["Title"] = dataPrint[j].Title;
                        row["Status"] = dataPrint[j].Status;
                        row["OrderNumber"] = dataPrint[j].OrderNumber;
                        row["Code"] = dataPrint[j].Code;
                        row["Type"] = dataPrint[j].Type;
                        row["Workshop"] = dataPrint[j].Workshop;
                        row["Workshop2"] = dataPrint[j].Workshop2;
                        row["Weight"] = dataPrint[j].Weight;
                        row["GoldType"] = dataPrint[j].GoldType;
                        row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                        row["Count"] = dataPrint[j].Count;
                        row["Size"] = dataPrint[j].Size;
                        row["Size2"] = dataPrint[j].Size2;
                        row["Stone"] = dataPrint[j].Stone;
                        row["Leather"] = dataPrint[j].Leather;
                        row["Customer"] = dataPrint[j].Customer;
                        row["Description"] = dataPrint[j].Description;
                        row["Description2"] = dataPrint[j].Description2;
                        row["BranchName"] = dataPrint[j].BranchName;
                        row["ProductColor"] = dataPrint[j].ProductColor;

                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();
                    report.Load(Server.MapPath("~/Report/Order/OrderReportReplica.mrt"));
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                    report.Dictionary.Variables["OrderSerial"].Value = string.Join(" , ", serialList);
                    report.Dictionary.Variables["OrderNumber"].Value = string.Join(" , ", orderNumberList);
                    report.Dictionary.Variables["SetNumber"].Value = itemWorkshop.FirstOrDefault().SetNumber.ToString();
                    report.Dictionary.Variables["SetNumberTitle"].Value = itemWorkshop.FirstOrDefault().SetNumber != null ? "سریال نیم ست" : "";
                    report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", orderBranchName);
                    //report.Dictionary.Synchronize();
                    report.Compile();
                    report.Render(false);

                    reports.Add(report);
                }

                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        /// <summary>
        /// چاپ گزارش سفارش ثبت شده با ارسال ردیف های سفارشات برای کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارشات</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult PrintWorkshopReplicaMin(string id)
        {
            List<int> idList = id.Split('-').Select(x => int.Parse(x)).ToList();

            List<OrderDetail> result;
            using (var db = new KiaGalleryContext())
            {
                var user = GetAuthenticatedUser();
                var query = db.OrderDetail.Include(x => x.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.Order.Branch).Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.Product.ProductFileList).Where(x => (x.SendWorkshopOrder2 != true && x.Product.WorkshopId == user.WorkshopId && idList.Any(y => y == x.WorkshopOrderId)) || (x.SendWorkshopOrder2 == true && x.Product.WorkshopId2 == user.WorkshopId && idList.Any(y => y == x.WorkshopOrderId2)));

                result = query.OrderBy(x => x.Product.Id).ToList();

                result.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).ToList().ForEach(x =>
                {
                    x.OrderDetailStatus = OrderDetailStatus.UnderConstruction;
                    var detailLog = new OrderDetailLog()
                    {
                        OrderDetailId = x.Id,
                        OrderDetailStatus = OrderDetailStatus.UnderConstruction,
                        OrderDetailLogReasonId = null,
                        Description = null,
                        CreateUserId = user.Id,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.OrderDetailLog.Add(detailLog);
                });
                db.SaveChanges();

                var serialList = result.Where(x => x.SendWorkshopOrder2 != true).Select(x => "KIA-" + x.WorkshopOrder.WorkshopOrderSerial).Distinct().ToList();
                serialList.AddRange(result.Where(x => x.SendWorkshopOrder2 == true).Select(x => "KIA-" + x.WorkshopOrder2.WorkshopOrderSerial).Distinct().ToList());
                var orderNumberList = result.Select(x => "KIA-" + x.Order.OrderNumber).Distinct().ToList();
                //var workshopOrderNumberList = result.Where(x=> x.SendWorkshopOrder2 != true).Select(x => "KIA-" + x.WorkshopOrder.WorkshopOrderNumber).Distinct().ToList();
                //workshopOrderNumberList.AddRange(result.Where(x => x.SendWorkshopOrder2 == true).Select(x => "KIA-" + x.WorkshopOrder.WorkshopOrderNumber).Distinct().ToList());
                var dateList = result.Select(x => DateUtility.GetPersianDate(x.Order.CreateDate)).Distinct().ToList();
                var orderBranchName = result.Select(x => x.Order.Branch.Name).Distinct().ToList();
                var data = result.Select(item => new
                {
                    Title = item.Product.Title,
                    Status = Enums.GetTitle(item.OrderDetailStatus),
                    OrderNumber = "KIA-" + item.Order.OrderNumber,
                    ProductSize = item.Product.SizeId,
                    SetNumber = item.SetNumber,
                    Code = item.Product.BookCode,
                    Type = Enums.GetTitle(item.Product.ProductType),
                    WorkshopId = item.Product.WorkshopId,
                    Workshop = item.Product.Workshop.Name,
                    WorkshopId2 = item.Product.WorkshopId2,
                    Workshop2 = item.Product.Workshop2?.Name,
                    Weight = item.Product.Weight?.ToString(),
                    GoldType = Enums.GetTitle(item.GoldType),
                    OuterWerkType = Enums.GetTitle(item.OuterWerkType),
                    Count = 1,
                    Size = item.Size,
                    Size2 = item.Size2,
                    LeatherLoop = item.LeatherLoop,
                    ForceOrder = item.ForceOrder,
                    Customer = item.Customer,
                    PhoneNumber = item.PhoneNumber,
                    StoneList = string.Join("\n", item.OrderDetailStoneList.Select(x => "سنگ " + x.Order + ": " + (x.Stone != null ? x.Stone.Name : "سلیقه ای")).ToList()),
                    LeatherList = string.Join("\n", item.OrderDetailLeatherList.Select(x => "چرم " + x.Order + ": " + (x.Leather != null ? x.Leather.Name : "سلیقه ای")).ToList()),
                    BranchLabel = item.BranchLabel,
                    Description = item.Description,
                    Description2 = item.Description2,
                    ProductColor = item.ProductColor >= 0 && item.ProductColor != null ? Enums.GetTitle(item.ProductColor) : ""

                })
                .ToList()
                .GroupBy(x => new
                {
                    x.Title,
                    x.OrderNumber,
                    x.Status,
                    x.Code,
                    x.Type,
                    x.WorkshopId,
                    x.Workshop,
                    x.WorkshopId2,
                    x.Workshop2,
                    x.Weight,
                    x.GoldType,
                    x.OuterWerkType,
                    x.Size,
                    x.Size2,
                    x.LeatherLoop,
                    x.ForceOrder,
                    x.Customer,
                    x.PhoneNumber,
                    x.StoneList,
                    x.LeatherList,
                    x.BranchLabel,
                    x.Description,
                    x.Description2,
                    x.SetNumber,
                    x.ProductSize,
                    x.ProductColor
                })
                .Select(x => new
                {
                    x.Key.Title,
                    x.Key.OrderNumber,
                    x.Key.Status,
                    x.Key.Code,
                    x.Key.Type,
                    x.Key.WorkshopId,
                    x.Key.Workshop,
                    x.Key.WorkshopId2,
                    x.Key.Workshop2,
                    x.Key.Weight,
                    x.Key.GoldType,
                    x.Key.OuterWerkType,
                    x.Key.Size,
                    x.Key.Size2,
                    x.Key.LeatherLoop,
                    x.Key.ForceOrder,
                    x.Key.Customer,
                    x.Key.PhoneNumber,
                    x.Key.StoneList,
                    x.Key.LeatherList,
                    x.Key.BranchLabel,
                    x.Key.Description,
                    x.Key.Description2,
                    x.Key.SetNumber,
                    x.Key.ProductSize,
                    Count = x.Count(),
                    x.Key.ProductColor
                });

                #region Print

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();
                foreach (var itemWorkshop in data.GroupBy(x => new { x.WorkshopId, x.SetNumber }))
                {
                    List<OrderPrintDataViewModel> dataPrint = new List<OrderPrintDataViewModel>();
                    foreach (var item in itemWorkshop)
                    {
                        OrderPrintDataViewModel itemPrint = new OrderPrintDataViewModel()
                        {
                            Title = item.Title,
                            Status = item.Status,
                            OrderNumber = item.OrderNumber,
                            Code = item.Code,
                            Type = item.Type,
                            Workshop = item.Workshop,
                            Workshop2 = item.Workshop2,
                            Weight = item.Weight + " گرم",
                            GoldType = string.IsNullOrEmpty(item.GoldType) ? "سلیقه ای" : item.GoldType,
                            OuterWerkType = string.IsNullOrEmpty(item.OuterWerkType) ? "" : "نوع خرج کار " + item.OuterWerkType,
                            Count = item.Count.ToString(),
                            Size = string.IsNullOrEmpty(item.Size) ? item.ProductSize != null ? "مشخص نشده" : "-" : item.Size,
                            Size2 = string.IsNullOrEmpty(item.Size2) ? item.ProductSize != null ? "" : "-" : item.Size2,
                            Stone = GetStoneText(item.StoneList),
                            Leather = GetLeatherText(item.LeatherList, item.LeatherLoop),
                            Customer = GetCustomerText(item.Customer, item.PhoneNumber, item.ForceOrder),
                            Description = GetDescriptionText(item.BranchLabel, item.Description),
                            Description2 = GetDescriptionText(item.BranchLabel, item.Description2),
                            ProductColor = item.ProductColor,

                        };

                        dataPrint.Add(itemPrint);
                    }

                    dataPrint = dataPrint.OrderBy(x => x.Code).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("OrderNumber");
                    dataTable.Columns.Add("Code");
                    dataTable.Columns.Add("Type");
                    dataTable.Columns.Add("Workshop");
                    dataTable.Columns.Add("Workshop2");
                    dataTable.Columns.Add("Weight");
                    dataTable.Columns.Add("GoldType");
                    dataTable.Columns.Add("OuterWerkType");
                    dataTable.Columns.Add("Count");
                    dataTable.Columns.Add("Size");
                    dataTable.Columns.Add("Size2");
                    dataTable.Columns.Add("Stone");
                    dataTable.Columns.Add("Leather");
                    dataTable.Columns.Add("Customer");
                    dataTable.Columns.Add("Description");
                    dataTable.Columns.Add("Description2");
                    dataTable.Columns.Add("ProductColor");

                    for (int j = 0; j < dataPrint.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Title"] = dataPrint[j].Title;
                        row["Status"] = dataPrint[j].Status;
                        row["OrderNumber"] = dataPrint[j].OrderNumber;
                        row["Code"] = dataPrint[j].Code;
                        row["Type"] = dataPrint[j].Type;
                        row["Workshop"] = dataPrint[j].Workshop;
                        row["Workshop2"] = dataPrint[j].Workshop2;
                        row["Weight"] = dataPrint[j].Weight;
                        row["GoldType"] = dataPrint[j].GoldType;
                        row["OuterWerkType"] = dataPrint[j].OuterWerkType;
                        row["Count"] = dataPrint[j].Count;
                        row["Size"] = dataPrint[j].Size;
                        row["Size2"] = dataPrint[j].Size2;
                        row["Stone"] = dataPrint[j].Stone;
                        row["Leather"] = dataPrint[j].Leather;
                        row["Customer"] = dataPrint[j].Customer;
                        row["Description"] = dataPrint[j].Description;
                        row["Description2"] = dataPrint[j].Description2;
                        row["ProductColor"] = dataPrint[j].ProductColor;

                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();
                    report.Load(Server.MapPath("~/Report/Order/OrderReportReplicaMin.mrt"));
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report.Dictionary.Variables["OrderDate"].Value = string.Join(" - ", dateList);
                    report.Dictionary.Variables["OrderSerial"].Value = string.Join(" , ", serialList);
                    report.Dictionary.Variables["OrderNumber"].Value = string.Join(" , ", orderNumberList);
                    report.Dictionary.Variables["SetNumber"].Value = itemWorkshop.FirstOrDefault().SetNumber.ToString();
                    report.Dictionary.Variables["SetNumberTitle"].Value = itemWorkshop.FirstOrDefault().SetNumber != null ? "سریال نیم ست" : "";
                    report.Dictionary.Variables["OrderBranchName"].Value = string.Join(" - ", orderBranchName);
                    //report.Dictionary.Synchronize();
                    report.Compile();
                    report.Render(false);

                    reports.Add(report);
                }

                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();

                foreach (var report in reports)
                {
                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                }

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(joinedReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        /// <summary>
        /// مدیریت سفارشات کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارش مورد نظر</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult WorkshopManipulate(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.WorkshopOrder = db.WorkshopOrder.Single(x => x.Id == id && x.Order.Deleted == false);
            }
            return View();
        }
        /// <summary>
        /// مدیریت سفارشات کارگاه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order-workshop")]
        public ActionResult WorkshopManipulateAll()
        {
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                var query = db.WorkshopOrder.Where(x => x.Order.Deleted == false && (x.OrderDetailList.Any(y => y.SendWorkshopOrder2 != true && y.Product.WorkshopId == user.WorkshopId && y.OrderDetailStatus != OrderDetailStatus.InWorkshop) || x.OrderDetailList2.Any(z => z.SendWorkshopOrder2 == true && z.Product.WorkshopId2 == user.WorkshopId && z.OrderDetailStatus != OrderDetailStatus.InWorkshop2)));
                ViewBag.WorkshopOrder = query.Select(x => new WorkshopDropDwonViewModel()
                {
                    id = x.Id,
                    val = x.WorkshopOrderSerial
                }).ToList();
            }
            return View();
        }

        /// <summary>
        /// لیست محصولات یک سفارش کارگاه جهت ویرایش و تغییر وضعیت
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست محصولات یک سفارش</returns>
        [Authorize(Roles = "admin, order-workshop")]
        public JsonResult ManipulateWorkshopDetailList(OrderDetailSearchViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                model.workshopList = new List<int>() { user.WorkshopId.GetValueOrDefault() };
                response = GetDataWork(model);
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// لیست محصولات کل سفارش کارگاه جهت ویرایش و تغییر وضعیت
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست محصولات یک سفارش</returns>
        [Authorize(Roles = "admin, order-workshop")]
        public JsonResult ManipulateWorkshopDetailListPaging(OrderDetailSearchViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                model.workshopList = new List<int>() { user.WorkshopId.GetValueOrDefault() };
                response = GetDataWork(model);
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// تغییر وضعیت محصولات برای کارگاه
        /// </summary>
        /// <param name="model">مدلی شامل ردیف های محصولات یک سفارش و وثعیتی که می بایست به آن تغییر کنند</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order-workshop")]
        public JsonResult ChangeWorkshopStatus(OrderDetailChangeStatusViewModel model)
        {
            Response response;
            try
            {
                if (model.status == OrderDetailStatus.OutOfConstruction || model.status == OrderDetailStatus.UnderConstruction)
                {
                    var user = GetAuthenticatedUser();
                    using (var db = new KiaGalleryContext())
                    {
                        var orderDetailList = db.OrderDetail.Where(x => x.RelatedOrderDetailId == null && (x.Product.WorkshopId == user.WorkshopId || x.Product.WorkshopId2 == user.WorkshopId) && model.id.Any(y => y == x.Id)).ToList();
                        foreach (var orderItem in orderDetailList)
                        {
                            if (orderItem.OrderDetailStatus == OrderDetailStatus.UnderConstruction || orderItem.OrderDetailStatus == OrderDetailStatus.OutOfConstruction || orderItem.OrderDetailStatus == OrderDetailStatus.UnderConstruction2 || orderItem.OrderDetailStatus == OrderDetailStatus.OutOfConstruction2)
                            {
                                if (orderItem.SendWorkshopOrder2 == true)
                                {
                                    if (model.status == OrderDetailStatus.UnderConstruction)
                                        model.status = OrderDetailStatus.UnderConstruction2;
                                    if (model.status == OrderDetailStatus.OutOfConstruction)
                                        model.status = OrderDetailStatus.OutOfConstruction2;
                                }
                                orderItem.OrderDetailStatus = model.status;
                                orderItem.ModifyUserId = user.Id;
                                orderItem.ModifyDate = DateTime.Now;
                                orderItem.Ip = Request.UserHostAddress;

                                var detailLog = new OrderDetailLog()
                                {
                                    OrderDetailId = orderItem.Id,
                                    OrderDetailStatus = model.status,
                                    OrderDetailLogReasonId = model.reasonId,
                                    Description = model.description,
                                    CreateUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };

                                db.OrderDetailLog.Add(detailLog);
                            }
                        }

                        db.SaveChanges();
                    }

                    response = new Response()
                    {
                        status = 200,
                        message = "تغییر وضعیت با موفقیت انجام شد."
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 403,
                        message = "شما مجاز به تغییر این نوع وضعیت نیستید."
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
        /// تغییر وضعیت محصولات برای کارگاه
        /// </summary>
        /// <param name="model">مدلی شامل ردیف های محصولات یک سفارش و وثعیتی که می بایست به آن تغییر کنند</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order-workshop")]
        public JsonResult ChangeWorkshopStatusToInWorkshopTow(OrderDetailChangeStatusViewModel model)
        {
            Response response;
            try
            {
                if (model.status == OrderDetailStatus.InWorkshop2)
                {
                    var user = GetAuthenticatedUser();
                    using (var db = new KiaGalleryContext())
                    {
                        var orderDetailList = db.OrderDetail.Where(x => x.Product.Workshop2 != null && x.SendWorkshopOrder2 != true && (x.OrderDetailStatus == OrderDetailStatus.UnderConstruction || x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction) && x.RelatedOrderDetailId == null && (x.Product.WorkshopId == user.WorkshopId) && model.id.Any(y => y == x.Id)).ToList();
                        var groupOrderDetailList = orderDetailList.GroupBy(x => x.OrderId);
                        foreach (var orderItem in groupOrderDetailList)
                        {
                            Order order = db.Order.Single(x => x.Id == orderItem.Key);
                            WorkshopOrderNoModel workshopOrderNoModel = GetNewWorkshopOrderNoSecond(db, orderItem.Key);
                            WorkshopOrder workshopOrder = new WorkshopOrder()
                            {
                                OrderId = orderItem.Key,
                                WorkshopOrderSerial = workshopOrderNoModel.workshopOrderSerial,
                                WorkshopOrderNumber = workshopOrderNoModel.WorkshopOrderNumber,
                                CreateUserId = user.Id,
                                ModifyUserId = user.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };
                            db.WorkshopOrder.Add(workshopOrder);
                            //db.SaveChanges();
                            //workshopOrder.WorkshopOrderSerial = order.OrderSerial + "-WS" + workshopOrder.Id;
                            //workshopOrder.WorkshopOrderNumber = order.OrderNumber + "-" + workshopOrder.Id;
                            foreach (var detailItem in orderItem)
                            {
                                detailItem.WorkshopOrder2 = workshopOrder;
                                detailItem.OrderDetailStatus = model.status;
                                detailItem.SendWorkshopOrder2 = true;
                                detailItem.ModifyUserId = user.Id;
                                detailItem.ModifyDate = DateTime.Now;
                                detailItem.Ip = Request.UserHostAddress;

                                var detailLog = new OrderDetailLog()
                                {
                                    OrderDetailId = detailItem.Id,
                                    OrderDetailStatus = model.status,
                                    OrderDetailLogReasonId = model.reasonId,
                                    Description = model.description,
                                    CreateUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };
                                db.OrderDetailLog.Add(detailLog);
                            }
                        }
                        db.SaveChanges();
                    }
                    response = new Response()
                    {
                        status = 200,
                        message = "تغییر وضعیت با موفقیت انجام شد."
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 403,
                        message = "شما مجاز به تغییر این نوع وضعیت نیستید."
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
        /// لیست همه علت های محصول ثبت شده در سیستم
        /// </summary>
        /// <returns>جیسون حاوی لیست علت های ثبت شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, reason")]
        public JsonResult GetAllReasonList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var data = db.OrderDetailLogReason.Select(x => new
                    {
                        id = x.Id,
                        status = x.OrderDetailStatus,
                        text = x.Text,
                        active = x.Active
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data.Select(x => new
                            {
                                x.id,
                                x.status,
                                statusText = Enums.GetTitle(x.status),
                                x.text,
                                x.active
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
        /// ذخیره علت وضعیت محصول
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات علت وضعیت محصول</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, size")]
        public JsonResult SaveReason(OrderDetailLogReasonViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.OrderDetailLogReason.Single(x => x.Id == model.id);
                        entity.OrderDetailStatus = model.orderDetailStatus;
                        entity.Text = model.text;
                        entity.Active = model.active;
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;

                        status = 200;
                        message = "علت وضعیت محصول با موفقیت ویرایش شد.";
                    }
                    else
                    {
                        var entity = new OrderDetailLogReason()
                        {
                            OrderDetailStatus = model.orderDetailStatus,
                            Text = model.text,
                            Active = model.active,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        db.OrderDetailLogReason.Add(entity);

                        status = 200;
                        message = "علت وضعیت محصول با موفقیت ایجاد شد.";
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
        /// خواندن اطلاعات علت وضعیت محصول
        /// </summary>
        /// <param name="id">ردیف علت وضعیت محصول</param>
        /// <returns>جیسون اطلاعات لود شده علت محصول</returns>
        [HttpGet]
        [Authorize(Roles = "admin, reason")]
        public JsonResult LoadReason(int id)
        {
            Response response;
            try
            {
                OrderDetailLogReason item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.OrderDetailLogReason.Find(id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new OrderDetailLogReasonViewModel()
                        {
                            id = item.Id,
                            orderDetailStatus = item.OrderDetailStatus,
                            text = item.Text,
                            active = item.Active
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "علت وضعیت محصول مورد نظر یافت نشد."
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
        /// غیر فعال کردن علت وضعیت محصول
        /// </summary>
        /// <param name="id">ردیف علت وضعیت محصول</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, reason")]
        public JsonResult InactiveReason(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.OrderDetailLogReason.Find(id);
                    item.Active = false;
                    response = new Response()
                    {
                        status = 200,
                        message = "علت وضعیت محصول با موفقیت غیرفعال شد."
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
        /// فعال کردن علت وضعیت محصول
        /// </summary>
        /// <param name="id">ردیف علت وضعیت محصول</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, reason")]
        public JsonResult ActiveReason(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.OrderDetailLogReason.Find(id);
                    item.Active = true;
                    response = new Response()
                    {
                        status = 200,
                        message = "علت وضعیت محصول با موفقیت فعال شد."
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
        /// حذف علت وضعیت محصول
        /// </summary>
        /// <param name="id">ردیف علت وضعیت محصول</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, reason")]
        public JsonResult DeleteReason(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.OrderDetailLogReason.Find(id);
                    db.OrderDetailLogReason.Remove(item);
                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "علت وضعیت محصول با موفقیت حذف شد."
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
        /// دریافت علت وضعیت محصول فعال
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام کارگاه ها</returns>
        [HttpGet]
        public JsonResult GetReason(OrderDetailStatus id)
        {
            Response response;
            try
            {
                List<OrderDetailLogReason> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.OrderDetailLogReason.Where(x => x.Active == true && x.OrderDetailStatus == id).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            text = item.Text
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
        /// تغییر توضیحات محصول
        /// </summary>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        public JsonResult ChangeDescription(int id, string Description)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    OrderDetail item = db.OrderDetail.SingleOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        item.Description = Description;
                        item.ModifyDate = DateTime.Now;
                        item.ModifyUserId = GetAuthenticatedUserId();
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "توضیحات ویراش شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "توضیحات ویرایش نشد."
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

        [HttpPost]
        public JsonResult ChangeDescription2(int id, string Description)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    OrderDetail item = db.OrderDetail.SingleOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        item.Description2 = Description;
                        item.ModifyDate = DateTime.Now;
                        item.ModifyUserId = GetAuthenticatedUserId();
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "توضیحات ویراش شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "توضیحات ویرایش نشد."
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

        [HttpPost]
        public JsonResult SearchOperation(SearchOperationViewModel model)
        {
            Response response;

            var allCount = 0;
            float? allWeight = 0;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.Order.Deleted == false);
                    allCount = query.Count();
                    allWeight = query.Select(x => x.Product.Weight).Sum();
                    if (model.status >= 0 && model.status != null)
                    {
                        query = query.Where(x => x.OrderDetailStatus == model.status);
                    }
                    if (!string.IsNullOrEmpty(model.fromDate) || !string.IsNullOrEmpty(model.toDate))
                    {
                        if (!string.IsNullOrEmpty(model.fromDate) && DateUtility.GetDateTime(model.fromDate) != null)
                        {
                            var fromDate = DateUtility.GetDateTime(model.fromDate).GetValueOrDefault();
                            query = query.Where(x => x.Order.CreateDate >= fromDate);
                        }

                        if (!string.IsNullOrEmpty(model.toDate) && DateUtility.GetDateTime(model.toDate) != null)
                        {
                            var toDate = DateUtility.GetDateTime(model.toDate).GetValueOrDefault();
                            query = query.Where(x => x.Order.CreateDate <= toDate);
                        }
                    }

                    if (model.workshopList != null && model.workshopList.Count(y => y > 0) > 0)
                    {
                        query = query.Where(x => (x.SendWorkshopOrder2 != true && model.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId)) || (x.SendWorkshopOrder2 == true && model.workshopList.Where(y => y > 0).Any(y => y == x.Product.WorkshopId2)));
                    }
                    if (model.productTypeList != null && model.productTypeList.Count(y => y > 0) > 0)
                    {
                        query = query.Where(x => model.productTypeList.Contains(x.Product.ProductType));
                    }
                    if (model.orderType != null && model.orderType > 0)
                    {
                        switch (model.orderType)
                        {
                            case 1:
                                query = query.Where(x => string.IsNullOrEmpty(x.Customer));
                                break;
                            case 2:
                                query = query.Where(x => !string.IsNullOrEmpty(x.Customer));
                                break;
                        }
                    }
                    if (model.branchType != null && model.branchType >= 0)
                    {
                        switch (model.branchType)
                        {
                            case BranchType.Branch:
                                query = query.Where(x => x.CreateUser.Branch.BranchType == BranchType.Branch);
                                break;
                            case BranchType.Solicitorship:
                                query = query.Where(x => x.CreateUser.Branch.BranchType == BranchType.Solicitorship);
                                break;
                        }
                    }
                    if (model.branchList != null && model.branchList.Count(y => y > 0) > 0)
                    {
                        query = query.Where(x => model.branchList.Contains(x.Order.BranchId));
                    }
                    var weight = query.Select(x => x.Product.Weight).Sum();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            allCount = Core.ToSeparator(allCount),
                            allWeight = allWeight != null && allWeight >= 0 ? Math.Round(double.Parse(allWeight.ToString()), 2) : 0,
                            count = Core.ToSeparator(query.Count()),
                            weight = weight != null && weight >= 0 ? Math.Round(double.Parse(weight.ToString()), 2) : 0,
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

        private string ChangeOperationOrderDetailStatus(KiaGalleryContext db, OrderDetail query)
        {
            string result = "";

            query.OrderDetailStatus = OrderDetailStatus.Cancel;
            db.SaveChanges();
            if (!string.IsNullOrEmpty(result.Trim()))
                return result.Trim();
            else
                return "-";
        }

        /// <summary>
        /// متن برای چاپ توضیحات و نام مشتری
        /// </summary>
        /// <param name="description">توضیحات</param>
        /// <returns>رشته آماده چاپ</returns>
        private string GetDescriptionText(string branchLabel, string description)
        {
            string result = "";

            if (!string.IsNullOrEmpty(branchLabel))
            {
                result = result + branchLabel + "\n";
            }

            if (!string.IsNullOrEmpty(description))
            {
                result = result + description + "\n";
            }

            return result;
        }

        /// <summary>
        /// متن برای چاپ مشتری
        /// </summary>
        /// <param name="customer">نام مشتری</param>
        /// <param name="phoneNumber">تلفن</param>
        /// <param name="forceOrder">سفارش عجله ای</param>
        /// <returns>متن آماده چاپ</returns>
        private string GetCustomerText(string customer, string phoneNumber, bool? forceOrder)
        {
            string result = "";

            if (!string.IsNullOrEmpty(customer))
            {
                result = result + customer + "\n";
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                result = result + phoneNumber + "\n";
            }

            if (forceOrder == true)
            {
                result = result + "سفارش عجله ای " + "\n";
            }

            return result;
        }

        /// <summary>
        /// متن جهت چاپ برای سنگ
        /// </summary>
        /// <param name="stoneList">لیست سنگ ها</param>
        /// <returns>رشته آماده چاپ</returns>
        private string GetStoneText(string stoneList)
        {
            string result = "";

            if (!string.IsNullOrEmpty(stoneList))
            {
                result = result + stoneList + "\n";
            }

            if (!string.IsNullOrEmpty(result.Trim()))
                return result.Trim();
            else
                return "-";
        }

        /// <summary>
        /// متن جهت چاپ برای چرم
        /// </summary>
        /// <param name="leatherList">لیست چرم ها</param>
        /// <param name="leatherLoop">تعداد دور چرم</param>
        /// <returns>رشته آماده چاپ</returns>
        private string GetLeatherText(string leatherList, byte? leatherLoop)
        {
            string result = "";

            if (!string.IsNullOrEmpty(leatherList))
            {
                result = result + leatherList + "\n";
            }

            if (leatherLoop != null && leatherLoop > 0)
            {
                result = result + leatherLoop + " دور چرم\n";
            }

            if (!string.IsNullOrEmpty(result.Trim()))
                return result.Trim();
            else
                return "-";
        }

        /// <summary>
        /// دریافت آرایه بایت تصویر برای محصول
        /// </summary>
        /// <param name="fileName">نام فایل</param>
        /// <returns>آرایه بایت شده تصویر</returns>
        private byte[] GetProductFileByte(string fileName)
        {
            var filePath = string.Format("~/upload/product/{0}", fileName);
            Image image = Image.FromFile(Server.MapPath(filePath));
            var resizedImage = BitmapUtility.FixedSize(image, 200, 200, true);
            return BitmapUtility.ImageToByteArray(resizedImage);
        }

        /// <summary>
        /// قرار دادن رنگ زمینه مناسب برای هر سفارش که بسته به وضعیت محصولات داخل سفارش تعیین میشود
        /// </summary>
        /// <param name="x">سفارش مورد نظر</param>
        private void GetBackgroundColor(OrderViewModel x)
        {
            if (x.registered == x.sumCount)
            {
                x.bgColor = "bg-new-order";
            }
            else if (x.cancel + x.shortageOrder + x.sent == x.sumCount)
            {
                x.bgColor = "bg-done-order";
            }
            else if (x.shortage + x.cancel + x.shortageOrder + x.sent == x.sumCount)
            {
                x.bgColor = "bg-open-shortage-order";
            }
            else
            {
                x.bgColor = "bg-open-order";
            }
        }

        /// <summary>
        /// دریافت شماره سفارش کسری برای یه سفارش کسری جدید
        /// </summary>
        /// <param name="db">شی پایگاه داده</param>
        /// <param name="order">سفارش</param>
        /// <returns>شماره سفارش کسری</returns>
        private string GetNewShortageOrderNo(KiaGalleryContext db, Order order)
        {
            if (order.OrderDetailList.Count(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder) == 0)
            {
                return order.OrderSerial + "-KS1";
            }
            else
            {
                var lastOrderSerial = db.Order.OrderByDescending(x => x.Id).First(x => x.OrderSerial.StartsWith(order.OrderSerial + "-KS")).OrderSerial;
                var orderNo = int.Parse(lastOrderSerial.Substring(lastOrderSerial.LastIndexOf("KS") + 2));
                return order.OrderSerial + "-KS" + (orderNo + 1);
            }
        }

        /// <summary>
        /// دریافت شماره سفارش کارگاه برای ساخت یک سفارش ارسال شده به کارگاه
        /// </summary>
        /// <param name="db">شی پایگاه داده</param>
        /// <param name="orderId">ردیف سفارش</param>
        /// <returns>شماره سفارش کارگاه</returns>
        private WorkshopOrderNoModel GetNewWorkshopOrderNo(KiaGalleryContext db, int orderId)
        {
            var order = db.Order.Single(x => x.Id == orderId);
            int countWorkshopOrder = order.WorkshopOrderList.Where(x => x.WorkshopOrderSerial.Contains("WF") && x.OrderDetailList.Count() > 0).Count();
            if (countWorkshopOrder == 0)
            {
                WorkshopOrderNoModel item = new WorkshopOrderNoModel()
                {
                    workshopOrderSerial = order.OrderSerial + "-WF1",
                    WorkshopOrderNumber = order.OrderNumber + "-1"
                };
                return item;
            }
            else
            {
                var lastWorkshopOrderSerial = order.WorkshopOrderList.Where(x => x.OrderDetailList.Count() > 0).OrderBy(x => x.Id).Last().WorkshopOrderSerial;
                var orderNo = int.Parse(lastWorkshopOrderSerial.Substring(lastWorkshopOrderSerial.LastIndexOf("WF") + 2));
                WorkshopOrderNoModel item = new WorkshopOrderNoModel()
                {
                    workshopOrderSerial = order.OrderSerial + "-WF" + (orderNo + 1),
                    WorkshopOrderNumber = order.OrderNumber + "-" + (orderNo + 1)
                };
                return item;
            }
        }

        /// <summary>
        /// دریافت شماره سفارش کارگاه دوم برای ساخت یک سفارش ارسال شده به کارگاه
        /// </summary>
        /// <param name="db">شی پایگاه داده</param>
        /// <param name="orderId">ردیف سفارش</param>
        /// <returns>شماره سفارش کارگاه</returns>
        private WorkshopOrderNoModel GetNewWorkshopOrderNoSecond(KiaGalleryContext db, int orderId)
        {
            var order = db.Order.Single(x => x.Id == orderId);
            int countWorkshopOrder = order.WorkshopOrderList.Where(x => x.WorkshopOrderSerial.Contains("WS") && x.OrderDetailList2.Count() > 0).Count();
            if (countWorkshopOrder == 0)
            {
                WorkshopOrderNoModel item = new WorkshopOrderNoModel()
                {
                    workshopOrderSerial = order.OrderSerial + "-WS1",
                    WorkshopOrderNumber = order.OrderNumber + "-1"
                };
                return item;
            }
            else
            {
                var lastWorkshopOrderSerial = order.WorkshopOrderList.Where(x => x.OrderDetailList2.Count() > 0).OrderBy(x => x.Id).Last().WorkshopOrderSerial;
                var orderNo = int.Parse(lastWorkshopOrderSerial.Substring(lastWorkshopOrderSerial.LastIndexOf("WS") + 2));
                WorkshopOrderNoModel item = new WorkshopOrderNoModel()
                {
                    workshopOrderSerial = order.OrderSerial + "-WS" + (orderNo + 1),
                    WorkshopOrderNumber = order.OrderNumber + "-" + (orderNo + 1)
                };
                return item;
            }
        }

        public class OrderNumberListViewModel
        {
            public string orderNumber { get; set; }
        }
    }
}