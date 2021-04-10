using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Order;
using KiaGallery.Web.Areas.Order.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Areas.Order.Controllers
{
    public class OrderController : Controller
    {
        /// <summary>
        /// تعداد ماه های لود کردن اطلاعات جزئیات سفارش
        /// </summary>
        private const int MonthToLoadDetail = -1;

        /// <summary>
        /// صفحه سفارش جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public JsonResult GetWorkshop(string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        List<Workshop> workshopList = db.Workshop.Where(x => x.Active == true && x.ProductList.Count(y => y.Active == true) > 0).OrderBy(x => x.Order).Select(x => x).ToList();
                        response = new Response
                        {
                            status = 200,
                            data = workshopList
                        };
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// جزئیات سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>اطلاعات مورد نظر</returns>
        public JsonResult Detail(int id, string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        Model.Context.Order.Order order = db.Order.Single(x => x.Id == id && x.BranchId == user.BranchId && x.Deleted == false);
                        List<Workshop> workshopList = db.Workshop.Where(x => x.Active == true).ToList();
                        response = new Response
                        {
                            status = 200,
                            data = new { order = order, workshopList = workshopList }
                        };
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// جزئیات همه ی سفارش
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public JsonResult DetailAll(string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        List<Workshop> workshopList = db.Workshop.Where(x => x.Active == true).ToList();
                        List<OrderDateViewModel> orderDateList = db.Order.Where(x => x.BranchId == user.BranchId && x.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail) && x.Deleted == false).GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).OrderByDescending(x => x.Key).Select(x => new { Date = x.Key, Count = x.Count() }).ToList().Select(x => new OrderDateViewModel() { Date = DateUtility.GetPersianDate(x.Date), Count = x.Count }).ToList();
                        response = new Response
                        {
                            status = 200,
                            data = new { workshopList= workshopList , OrderDateList = orderDateList }
                        };
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// لیست جزئیات سفارش برای یک سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>لیست کالاهای ثبت شده در سفارش</returns>
        public JsonResult DetailList(OrderDetailSearchViewModel model)
        {
            Response response;
            try
            {
                List<OrderDetailViewModel> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if(user != null)
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
                            returned = item.OrderDetailLogList.Count(x => x.Description == "مرجوعی")
                        })
                        .ToList();
                        int conter = 1;
                        list.ForEach(item => {
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// لیست سواابق سفارش ثبت شده توسط شعبه
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست سفارش های ثبت شده شعبه</returns>
        public JsonResult GetAllHistory(OrderSearchViewModel model)
        {
            Response response;
            try
            {
                List<Model.Context.Order.Order> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if(user != null)
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// اضافه کردن محصول به لیست علاقه مندی ها
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns>نتیجه اضافه شدن به لیست علاقه مندی ها</returns>
        public JsonResult AddToFavorite(int id,string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if(user != null)
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        public JsonResult RemoveFavorite(int id, string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if(user != null)
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        public JsonResult GetLogDetail(int id, string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
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
                        logList.ForEach(x => {
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// مدیریت سفارش و تغییر وضعیت سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Manipulate(int id)
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
        public JsonResult GetAll(OrderSearchViewModel model)
        {
            Response response;
            try
            {
                //List<Order> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        var query = db.Order/*.Include(x => x.OrderDetailList).Include(x => x.Branch).Include(x => x.CreateUser).Include(x => x.CreateUser).Include("OrderDetailList.Product")*/.Where(x => x.Deleted == false);

                        if (model.archive == null || model.archive == false)
                        {
                            var notCompleteStatus = new OrderDetailStatus[] { OrderDetailStatus.Registered, OrderDetailStatus.InWorkshop, OrderDetailStatus.UnderConstruction, OrderDetailStatus.OutOfConstruction, OrderDetailStatus.InPreparation, OrderDetailStatus.ReadyForDelivery, OrderDetailStatus.Shortage };
                            query = query.Where(x => x.OrderDetailList.Count(y => notCompleteStatus.Any(z => z == y.OrderDetailStatus)) > 0);
                        }

                        if (model.branchId != null && model.branchId.Count > 0)
                        {
                            query = query.Where(x => model.branchId.Any(y => y == x.BranchId));
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

                        dataCount = query.Count();
                        query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                        //list = query.ToList();

                        var data = query.Select(item => new OrderViewModelFloat()
                        {
                            id = item.Id,
                            orderSerial = "KIA-" + item.OrderSerial,
                            sumCount = item.OrderDetailList.Sum(x => x.Count) > 0 ? item.OrderDetailList.Sum(x => x.Count) : 0,
                            sumCountSet = item.OrderDetailList.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count() > 0 ? item.OrderDetailList.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count() : 0,
                            //sumWeight = Math.Round(double.Parse(item.OrderDetailList.Sum(x => x.Product.Weight * x.Count).ToString()), 2),
                            sumWeight = item.OrderDetailList.Sum(x => x.Product.Weight * x.Count) > 0 ? item.OrderDetailList.Sum(x => x.Product.Weight * x.Count) : 0,
                            registered = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count) : 0,
                            registeredWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count * x.Product.Weight) : 0,
                            inWorkshop = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count) : 0,
                            inWorkshopWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count * x.Product.Weight) : 0,
                            underConstruction = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count) : 0,
                            underConstructionWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count * x.Product.Weight) : 0,
                            outOfConstruction = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count) : 0,
                            outOfConstructionWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count * x.Product.Weight) : 0,
                            inPreparation = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count) : 0,
                            inPreparationWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count * x.Product.Weight) : 0,
                            readyForDelivery = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count) : 0,
                            readyForDeliveryWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count * x.Product.Weight) : 0,
                            sent = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count) : 0,
                            sentWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count * x.Product.Weight) : 0,
                            shortage = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count) : 0,
                            shortageWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count * x.Product.Weight) : 0,
                            shortageOrder = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count) : 0,
                            shortageOrderWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count * x.Product.Weight) : 0,
                            cancel = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count) : 0,
                            cancelWeight = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count * x.Product.Weight) > 0 ? item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count * x.Product.Weight) : 0,
                            createUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                            createBranch = item.Branch.Name,
                            createDateTime = item.CreateDate
                            //createDate = DateUtility.GetPersianDate(item.CreateDate)
                        }).ToList();

                        var list = data.Select(x => new OrderViewModel()
                        {
                            id = x.id,
                            orderSerial = x.orderSerial,
                            sumCount = x.sumCount,
                            sumCountSet = x.sumCountSet,
                            sumWeight = Math.Round(double.Parse(x.sumWeight.ToString()), 2),
                            registered = x.registered,
                            registeredWeight = Math.Round(double.Parse(x.registeredWeight.ToString()), 2),
                            inWorkshop = x.inWorkshop,
                            inWorkshopWeight = Math.Round(double.Parse(x.inWorkshop.ToString()), 2),
                            underConstruction = x.underConstruction,
                            underConstructionWeight = Math.Round(double.Parse(x.underConstructionWeight.ToString()), 2),
                            outOfConstruction = x.outOfConstruction,
                            outOfConstructionWeight = Math.Round(double.Parse(x.outOfConstructionWeight.ToString()), 2),
                            inPreparation = x.inPreparation,
                            inPreparationWeight = Math.Round(double.Parse(x.inPreparationWeight.ToString()), 2),
                            readyForDelivery = x.readyForDelivery,
                            readyForDeliveryWeight = Math.Round(double.Parse(x.readyForDeliveryWeight.ToString()), 2),
                            sent = x.sent,
                            sentWeight = Math.Round(double.Parse(x.sentWeight.ToString()), 2),
                            shortage = x.shortage,
                            shortageWeight = Math.Round(double.Parse(x.shortageWeight.ToString()), 2),
                            shortageOrder = x.shortageOrder,
                            shortageOrderWeight = Math.Round(double.Parse(x.shortageOrderWeight.ToString()), 2),
                            cancel = x.cancel,
                            cancelWeight = Math.Round(double.Parse(x.cancelWeight.ToString()), 2),
                            createUser = x.createUser,
                            createBranch = x.createBranch,
                            createDate = DateUtility.GetPersianDate(x.createDateTime)
                        }).ToList();

                        list.ForEach(x => {
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
                                registeredWeight = list.Sum(x => x.registeredWeight),
                                inWorkshopWeight = list.Sum(x => x.inWorkshopWeight),
                                underConstructionWeight = list.Sum(x => x.underConstructionWeight),
                                outOfConstructionWeight = list.Sum(x => x.outOfConstructionWeight),
                                inPreparationWeight = list.Sum(x => x.inPreparationWeight),
                                readyForDeliveryWeight = list.Sum(x => x.readyForDeliveryWeight),
                                sentWeight = list.Sum(x => x.sentWeight),
                                shortageWeight = list.Sum(x => x.shortageWeight),
                                shortageOrderWeight = list.Sum(x => x.shortageOrderWeight),
                                cancelWeight = list.Sum(x => x.cancelWeight)
                            }
                        };
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
                query = query.Where(x => x.OrderId == model.orderId && x.Order.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail));
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
            List<OrderDetailViewModel> list;
            int dataCount;
            double pageCount = 0;
            Response response;
            using (var db = new KiaGalleryContext())
            {
                var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                if(user != null)
                {
                    var query = db.OrderDetail.Include(x => x.Order).Include(x => x.Product).Include(x => x.Product.ProductFileList).Include(x => x.Product.Workshop).Include(x => x.OrderDetailStoneList).Include(x => x.OrderDetailLeatherList).Include(x => x.RelatedOrderDetail.Order).Include("OrderDetailStoneList.Stone").Include("OrderDetailLeatherList.Leather").Include(x => x.CreateUser).Where(x => x.Order.Deleted == false);

                    if (model.orderId != null && model.orderId > 0)
                    {
                        query = query.Where(x => x.OrderId == model.orderId && x.Order.CreateDate >= DbFunctions.AddMonths(DateTime.Now, MonthToLoadDetail));
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
                        pageCount = Math.Ceiling((double)dataCount / model.count);
                    }
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
                        size = item.Size,
                        setNumber = item.SetNumber,
                        goldType = item.GoldType,
                        //goldTypeTitle = Enums.GetTitle(item.GoldType),
                        outerWerkType = item.OuterWerkType,
                        //outerWerkTypeTitle = Enums.GetTitle(item.OuterWerkType),
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
                        //orderDetailStatusTitle = Enums.GetTitle(item.OrderDetailStatus),
                        createdUser = item.CreateUser.FirstName + item.CreateUser.LastName,
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
                        returned = item.OrderDetailLogList.Count(x => x.Description == "مرجوعی")
                    }).ToList();
                    list.ForEach(x => {
                        x.orderTypeTitle = Enums.GetTitle(x.orderType);
                        x.goldTypeTitle = Enums.GetTitle(x.goldType);
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
                            page = model.page + 1
                        },
                        status = 200,
                    };
                }
                else
                {
                    response = new Response
                    {
                        status = 403,
                        message = "توکن ارسال شده معتبر نمی باشد."
                    };
                }
            }
           
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
        public JsonResult ChangeStatus(OrderDetailChangeStatusViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        if (model.orderId != null && model.orderId > 0) // در صورتی که ردیف یک سفارش یکتا به سمت سرور پاس داده شود برای ساخت سفارش نیازی به لود کردن اطلاعات نیست
                        {
                            WorkshopOrder workshopOrder = null;
                            if (model.status == OrderDetailStatus.InWorkshop)
                            {
                                workshopOrder = new WorkshopOrder()
                                {
                                    WorkshopOrderSerial = GetNewWorkshopOrderNo(db, model.orderId.GetValueOrDefault()),
                                    OrderId = model.orderId.GetValueOrDefault(),
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };

                                db.WorkshopOrder.Add(workshopOrder);
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
                                if (model.status == OrderDetailStatus.InWorkshop)
                                {
                                    workshopOrder = new WorkshopOrder()
                                    {
                                        WorkshopOrderSerial = GetNewWorkshopOrderNo(db, orderItem.Key),
                                        OrderId = orderItem.Key,
                                        CreateUserId = user.Id,
                                        ModifyUserId = user.Id,
                                        CreateDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        Ip = Request.UserHostAddress
                                    };

                                    db.WorkshopOrder.Add(workshopOrder);
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
                        db.SaveChanges();
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
                        };
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

        public JsonResult MakeShortageOrder(int orderId,string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        var order = db.Order.Single(x => x.Id == orderId && x.Deleted == false);
                        if (order.OrderDetailList.Count(x => x.OrderDetailStatus == OrderDetailStatus.Shortage) > 0)
                        {
                            #region MakeOrder

                            var orderSerial = GetNewShortageOrderNo(db, order);
                            var shortageOrder = new Model.Context.Order.Order()
                            {
                                BranchId = order.BranchId,
                                OrderSerial = orderSerial,
                                CreateUserId = user.Id,
                                ModifyUserId = user.Id,
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
                                    }).ToList()
                                };

                                x.OrderDetailStatus = OrderDetailStatus.ShortageOrder;
                                x.OrderDetailLogList.Add(new OrderDetailLog()
                                {
                                    OrderDetail = x,
                                    OrderDetailStatus = OrderDetailStatus.ShortageOrder,
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

                            shortageOrder.OrderDetailList.ForEach(x =>
                            {
                                var status = OrderDetailStatus.Registered;
                                if (x.Product.Workshop.AutoConfirm)
                                    status = OrderDetailStatus.InWorkshop;

                                x.OrderDetailLogList.Add(new OrderDetailLog()
                                {
                                    OrderDetail = x,
                                    OrderDetailStatus = status,
                                    CreateUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                });
                            });

                            db.Order.Add(shortageOrder);

                            var autoConfirmList = shortageOrder.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).ToList();
                            if (autoConfirmList.Count > 0)
                            {
                                WorkshopOrder wsOrder = new WorkshopOrder()
                                {
                                    Order = shortageOrder,
                                    WorkshopOrderSerial = orderSerial + "-WS1",
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress,
                                };

                                db.WorkshopOrder.Add(wsOrder);
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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


        public JsonResult MakeShortageOrderDetail(List<int> orderDetailIdList, string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        var orderDetailList = db.OrderDetail.Include(x => x.Product).Include(x => x.Product.Workshop).Include(x => x.OrderDetailStoneList).Include(x => x.OrderDetailLeatherList).Where(x => x.RelatedOrderDetailId == null && orderDetailIdList.Any(y => y == x.Id)).ToList();
                        if (orderDetailList.Count(x => x.OrderDetailStatus == OrderDetailStatus.Shortage) > 0)
                        {
                            #region MakeOrder

                            var orderList = orderDetailList.GroupBy(x => x.OrderId);
                            foreach (var order in orderList)
                            {
                                var orderSerial = GetNewShortageOrderNo(db, order.First().Order);
                                var shortageOrder = new Model.Context.Order.Order()
                                {
                                    BranchId = order.First().Order.BranchId,
                                    OrderSerial = orderSerial,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
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
                                        }).ToList()
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

                                shortageOrder.OrderDetailList.ForEach(x =>
                                {
                                    var status = OrderDetailStatus.Registered;
                                    if (x.Product.Workshop.AutoConfirm)
                                        status = OrderDetailStatus.InWorkshop;

                                    x.OrderDetailLogList.Add(new OrderDetailLog()
                                    {
                                        OrderDetail = x,
                                        OrderDetailStatus = status,
                                        CreateUserId = user.Id,
                                        CreateDate = DateTime.Now,
                                        Ip = Request.UserHostAddress
                                    });
                                });

                                db.Order.Add(shortageOrder);

                                var autoConfirmList = shortageOrder.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).ToList();
                                if (autoConfirmList.Count > 0)
                                {
                                    WorkshopOrder wsOrder = new WorkshopOrder()
                                    {
                                        Order = shortageOrder,
                                        WorkshopOrderSerial = orderSerial + "-WS1",
                                        CreateUserId = user.Id,
                                        ModifyUserId = user.Id,
                                        CreateDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        Ip = Request.UserHostAddress,
                                    };

                                    db.WorkshopOrder.Add(wsOrder);
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        public JsonResult GetBranchRegisteredOrder(string dateTime, string token)
        {
            Response response;
            try
            {
                List<int> list;
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        var date = DateUtility.GetDateTime(dateTime);
                        list = db.Order.Where(x => x.Deleted == false && x.CreateDate >= date && x.CreateDate <= DbFunctions.AddDays(date, 1)).Select(x => x.Branch.Id).ToList();
                        response = new Response()
                        {
                            status = 200,
                            data = new
                            {
                                list = list
                            }
                        };
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// حذف محصولات و تمام سابقه ها و خود سفارش
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>نتیجه عملیات حذف یک سفارش</returns>
        public JsonResult Delete(int id,string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// ریست کردن تمام سفارش و عملیات های انجام شده
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>نتیجه عملیات حذف یک سفارش</returns>
        public JsonResult Reset(int id, string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        var item = db.Order.Find(id);
                        if (item.OrderDetailList.Count(x => x.RelatedOrderDetailId != null) == 0)
                        {
                            HashSet<int> workshopOrderIdList = new HashSet<int>();
                            item.OrderLogList.RemoveRange(1, item.OrderLogList.Count - 1);
                            item.OrderDetailList.ForEach(x =>
                            {
                                if (x.WorkshopOrderId != null)
                                    workshopOrderIdList.Add(x.WorkshopOrderId.GetValueOrDefault());

                                x.OrderDetailStatus = OrderDetailStatus.Registered;
                                x.WorkshopOrderId = null;
                                db.OrderDetailLog.RemoveRange(x.OrderDetailLogList.Where(y => y.OrderDetailStatus != OrderDetailStatus.Registered));
                            });

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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// لیست سفارشات با وضعیت کالای ارسال شده به کارگاه
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست سفارشات با وضعیت ارسال شده به کارگاه</returns>
        public JsonResult GetAllWorkshopOrder(OrderSearchViewModel model)
        {
            Response response;
            try
            {
                List<WorkshopOrder> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        var query = db.WorkshopOrder.Include(x => x.OrderDetailList).Where(x => x.Order.Deleted == false && x.OrderDetailList.Any(y => y.OrderDetailStatus == OrderDetailStatus.InWorkshop && y.Product.WorkshopId == user.WorkshopId));

                        dataCount = query.Count();
                        query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                        list = query.ToList();

                        var data = list.Select(item => new OrderViewModel()
                        {
                            id = item.Id,
                            orderSerial = "KIA-" + item.WorkshopOrderSerial,
                            sumCount = item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop && x.Product.WorkshopId == user.WorkshopId).Sum(x => x.Count),
                            sumCountSet = item.OrderDetailList.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count(),
                            sumWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop && x.Product.WorkshopId == user.WorkshopId).Sum(x => x.Product.Weight * x.Count).ToString()), 2),
                            createUser = item.CreateUser.FullName,
                            createDate = DateUtility.GetPersianDate(item.CreateDate),
                        }).ToList();

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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// لیست سفارشات کارگاه
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست سفارشات کارگاه</returns>
        public JsonResult GetAllWorkshopHistoryOrder(OrderSearchViewModel model)
        {
            Response response;
            try
            {
                List<WorkshopOrder> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        var query = db.WorkshopOrder.Include(x => x.OrderDetailList).Where(x => x.Order.Deleted == false && x.OrderDetailList.Any(y => y.OrderDetailStatus != OrderDetailStatus.InWorkshop && y.Product.WorkshopId == user.WorkshopId));
                        if (!string.IsNullOrEmpty(model.term))
                            query = query.Where(x => x.WorkshopOrderSerial.Contains(model.term.Trim()) || x.OrderDetailList.Any(y => y.Product.Title.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.Customer.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.PhoneNumber.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.BranchLabel.Contains(model.term.Trim())) || x.OrderDetailList.Any(y => y.Description.Contains(model.term.Trim())));
                        dataCount = query.Count();
                        query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                        list = query.ToList();

                        var data = list.Select(item => new OrderViewModel()
                        {
                            id = item.Id,
                            orderSerial = "KIA-" + item.WorkshopOrderSerial,
                            sumCount = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId).Sum(x => x.Count),
                            sumCountSet = item.OrderDetailList.Where(x => x.SetNumber != null).GroupBy(x => x.SetNumber).Count(),
                            sumWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId).Sum(x => x.Product.Weight * x.Count).ToString()), 2),
                            registered = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count),
                            registeredWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Registered).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            inWorkshop = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count),
                            inWorkshopWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InWorkshop).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            underConstruction = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count),
                            underConstructionWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.UnderConstruction).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            outOfConstruction = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count),
                            outOfConstructionWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.OutOfConstruction).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            inPreparation = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count),
                            inPreparationWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.InPreparation).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            readyForDelivery = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count),
                            readyForDeliveryWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ReadyForDelivery).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            sent = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count),
                            sentWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Sent).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            shortage = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count),
                            shortageWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Shortage).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            shortageOrder = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count),
                            shortageOrderWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.ShortageOrder).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            cancel = item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count),
                            cancelWeight = Math.Round(double.Parse(item.OrderDetailList.Where(x => x.Product.WorkshopId == user.WorkshopId && x.OrderDetailStatus == OrderDetailStatus.Cancel).Sum(x => x.Count * x.Product.Weight).ToString()), 2),
                            createUser = item.CreateUser?.FullName,
                            createDate = DateUtility.GetPersianDate(item.CreateDate),
                        }).ToList();

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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// خلاصه وضعیت تعداد سفارشات ارسال شده به کارگاه شامل تعداد
        /// </summary>
        /// <returns>تعداد سفارشات باز</returns>
        public JsonResult WorkshopSummary(string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        var query = db.WorkshopOrder.Include(x => x.OrderDetailList).Where(x => x.Order.Deleted == false && x.OrderDetailList.Any(y => y.OrderDetailStatus == OrderDetailStatus.InWorkshop && y.Product.WorkshopId == user.WorkshopId));
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// صفحه مشاهده جزئیات یک سفارش برای کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارش</param>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult WorkshopDetail(int id)
        {
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
        public JsonResult WorkshopDetailList(int id, OrderDetailSearchViewModel model)
        {
            Response response;
            try
            {
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        var query = db.OrderDetail.Where(x => x.WorkshopOrderId == id && x.Product.WorkshopId == user.WorkshopId);

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
                            fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                            orderType = item.OrderType,
                            //orderTypeTitle = Enums.GetTitle(item.OrderType),
                            productId = item.ProductId,
                            productSizeId = item.Product.SizeId,
                            workshopName = item.Product.Workshop.Name,
                            size = item.Size,
                            goldType = item.GoldType,
                            setNumber = item.SetNumber,
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
                            returned = item.OrderDetailLogList.Count(x => x.Description == "مرجوعی")
                        })
                        .ToList();

                        data.ForEach(x =>
                        {
                            x.orderTypeTitle = Enums.GetTitle(x.orderType);
                            x.goldTypeTitle = Enums.GetTitle(x.goldType);
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// چاپ گزارش سفارش ثبت شده با ارسال ردیف های سفارشات برای کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارشات</param>
        /// <returns></returns>
        public JsonResult changeStatusOrderDetail(string id,string token)
        {
            List<int> idList = id.Split('-').Select(x => int.Parse(x)).ToList();

            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        List<OrderDetail> result = db.OrderDetail.Where(x => x.Product.WorkshopId == user.WorkshopId && idList.Any(y => y == x.WorkshopOrderId)).OrderBy(x => x.Product.Id).ToList();

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
                        response = new Response()
                        {
                            status = 200,
                            message = "فاکتور های انتخاب شده به حالت در حال ساخت منتقل شدند."
                        };
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// مدیریت سفارشات کارگاه
        /// </summary>
        /// <param name="id">ردیف سفارش مورد نظر</param>
        /// <returns>صفحه مورد نظر</returns>
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
            return View();
        }

        /// <summary>
        /// لیست محصولات یک سفارش کارگاه جهت ویرایش و تغییر وضعیت
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست محصولات یک سفارش</returns>
        public JsonResult ManipulateWorkshopDetailList(OrderDetailSearchViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        model.workshopList = new List<int>() { user.WorkshopId.GetValueOrDefault() };
                        response = GetData(model);
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
                    {
                        model.workshopList = new List<int>() { user.WorkshopId.GetValueOrDefault() };
                        response = GetData(model);
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// تغییر وضعیت محصولات برای کارگاه
        /// </summary>
        /// <param name="model">مدلی شامل ردیف های محصولات یک سفارش و وثعیتی که می بایست به آن تغییر کنند</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        public JsonResult ChangeWorkshopStatus(OrderDetailChangeStatusViewModel model)
        {
            Response response;
            try
            {
                if (model.status == OrderDetailStatus.OutOfConstruction || model.status == OrderDetailStatus.UnderConstruction)
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                        if (user != null)
                        {
                            var orderDetailList = db.OrderDetail.Where(x => x.RelatedOrderDetailId == null && x.Product.WorkshopId == user.WorkshopId && model.id.Any(y => y == x.Id)).ToList();
                            foreach (var orderItem in orderDetailList)
                            {
                                if (orderItem.OrderDetailStatus == OrderDetailStatus.UnderConstruction || orderItem.OrderDetailStatus == OrderDetailStatus.OutOfConstruction)
                                {
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
                            response = new Response()
                            {
                                status = 200,
                                message = "تغییر وضعیت با موفقیت انجام شد."
                            };
                        }
                        else
                        {
                            response = new Response
                            {
                                status = 403,
                                message = "توکن ارسال شده معتبر نمی باشد."
                            };
                        }
                    }
                    
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
        public JsonResult GetAllReasonList(string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// ذخیره علت وضعیت محصول
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات علت وضعیت محصول</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        public JsonResult SaveReason(OrderDetailLogReasonViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if(user != null)
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.OrderDetailLogReason.Single(x => x.Id == model.id);
                            entity.OrderDetailStatus = model.orderDetailStatus;
                            entity.Text = model.text;
                            entity.Active = model.active;
                            entity.ModifyUserId = user.Id;
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
                                CreateUserId = user.Id,
                                ModifyUserId = user.Id,
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
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "علت وضعیت محصول مورد نظر یافت نشد."
                        };
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
        /// خواندن اطلاعات علت وضعیت محصول
        /// </summary>
        /// <param name="id">ردیف علت وضعیت محصول</param>
        /// <returns>جیسون اطلاعات لود شده علت محصول</returns>
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
        public JsonResult InactiveReason(int id, string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// فعال کردن علت وضعیت محصول
        /// </summary>
        /// <param name="id">ردیف علت وضعیت محصول</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        public JsonResult ActiveReason(int id,string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// حذف علت وضعیت محصول
        /// </summary>
        /// <param name="id">ردیف علت وضعیت محصول</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        public JsonResult DeleteReason(int id,string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن
                    if (user != null)
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
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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
        /// دریافت علت وضعیت محصول فعال
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام کارگاه ها</returns>
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
                result = result + "نام مشتری " + customer + "\n";
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                result = result + "تلفن مشتری " + phoneNumber + "\n";
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
        private string GetNewShortageOrderNo(KiaGalleryContext db, Model.Context.Order.Order order)
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
        private string GetNewWorkshopOrderNo(KiaGalleryContext db, int orderId)
        {
            var order = db.Order.Single(x => x.Id == orderId);
            if (order.WorkshopOrderList.Count == 0)
            {
                return order.OrderSerial + "-WS1";
            }
            else
            {
                var lastWorkshopOrderSerial = order.WorkshopOrderList.OrderBy(x => x.Id).Last().WorkshopOrderSerial;
                var orderNo = int.Parse(lastWorkshopOrderSerial.Substring(lastWorkshopOrderSerial.LastIndexOf("WS") + 2));
                return order.OrderSerial + "-WS" + (orderNo + 1);
            }
        }
    }
}