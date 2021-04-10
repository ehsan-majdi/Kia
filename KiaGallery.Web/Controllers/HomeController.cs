using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر داشبورد
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// داشبورد
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Index()
        {
            var date = DateUtility.GetPersianDate(DateTime.Now);
            ViewBag.Date = date;

            ViewBag.DashDate = date.Replace("/", "-");
            return View();
        }

        public ActionResult _Notification()
        {
            var currentUser = GetAuthenticatedUser();
            var notification = new List<Notification>();
            using (var db = new KiaGalleryContext())
            {
                notification = db.Notification.Where(x => x.UserId == currentUser.Id && x.Ticket.TicketStatus == TicketStatus.New).ToList();
            }
            return PartialView(notification);
        }

        /// <summary>
        /// دفترچه تلفن
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , phoneBook")]
        public ActionResult PhoneBook()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true && x.BranchType == BranchType.Branch).OrderBy(x => x.Order).Select(x => x).ToList();
            }
            return View();
        }

        /// <summary>
        /// گرفتن شماره تلفن شعبه
        /// </summary>
        /// <param name="id">ردیف شعبه</param>
        /// <returns>شماره تلفن شعبه</returns>
        [Authorize(Roles = "admin , phoneBook")]
        public JsonResult GetBranchTelephone(int? id)
        {
            Response response;
            try
            {
                Branch item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Branch.SingleOrDefault(x => x.Id == id);
                }
                response = new Response()
                {
                    status = 200,
                    data = new BranchViewModel
                    {
                        phone = item.Phone,
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
        /// جیسون حاوی اطلاعات دفترچه تلفن
        /// </summary>
        /// <param name="model"></param>
        /// <returns>تلفن شعب و پرسنل</returns>
        [Authorize(Roles = "admin , phoneBook")]
        public JsonResult PhoneBookJson(PhoneBookViewModel model)
        {
            List<PhoneBookViewModel> list;
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Person.Where(x => x.Active == true);

                    if (model.id != null)
                    {
                        query = query.Where(x => x.BranchId == model.id);
                    }

                    list = query.Select(x => new PhoneBookViewModel()
                    {
                        id = x.BranchId,
                        branchName = x.Branch.Name,
                        branchPhone = x.Branch.Phone,
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        personPhone = x.MobileNumber
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list
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
        /// صفحه گزارش محصولات
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin")]
        public ActionResult ProductChart()
        {
            using (var db = new KiaGalleryContext())
            {
                var weekTime = DateTime.Today.AddDays(-7);
                var mounthTime = DateTime.Today.AddDays(-30);

                ///گزارش روزانه
                var query = db.OrderDetail.Where(x => x.CreateDate >= DateTime.Today);
                var orderCount = query.Where(x => x.Order.OrderStatus == OrderStatus.Normal).Select(x => x.OrderId).Distinct().Count();
                var weight = query.Sum(x => x.Product.Weight * x.Count);
                var productCount = query.Count();
                var postquery = db.PostItem.Where(x => x.CreateDate >= DateTime.Today);
                var postCount = postquery.Count();
                var soldCheckQuery = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.SoldToTheCustomer && x.CreateDate >= DateTime.Today);
                var usedCheckQuery = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.Used && x.CreateDate >= DateTime.Today);
                var burnedCheckQuery = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.Burn && x.CreateDate >= DateTime.Today);

                var soldCheckCount = soldCheckQuery.Count();
                var usedCheckCount = usedCheckQuery.Count();
                var burnedCheckCount = burnedCheckQuery.Count();

                ViewBag.ProductCount = productCount;
                ViewBag.OrderCount = orderCount;
                ViewBag.OrderWeight = weight;
                ViewBag.PostCount = postCount;
                ViewBag.SoldCheckCount = soldCheckCount;
                ViewBag.UsedCheckCount = usedCheckCount;
                ViewBag.burnedCheckCount = burnedCheckCount;


                ///گزارش هفتگی
                var weeklyQuery = db.OrderDetail.Where(x => x.CreateDate >= weekTime);
                var weeklyOrderCount = weeklyQuery.Where(x => x.Order.OrderStatus == OrderStatus.Normal).Select(x => x.OrderId).Distinct().Count();
                var weeklyWeight = weeklyQuery.Sum(x => x.Product.Weight * x.Count);
                var weeklyOrderCountWeight = weeklyQuery.Sum(x => x.Product.Weight * x.Count);
                var weeklyOrderCountProductCount = weeklyQuery.Count();
                var weeklyPostquery = db.PostItem.Where(x => x.CreateDate >= weekTime);
                var weeklyPostCount = weeklyPostquery.Count();
                var weeklySoldCheckQuery = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.SoldToTheCustomer && x.CreateDate >= weekTime);
                var weeklyUsedCheckQuery = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.Used && x.CreateDate >= weekTime);
                var weeklyburnedCheckQuery = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.Burn && x.CreateDate >= weekTime);

                var weeklySoldCheckCount = weeklySoldCheckQuery.Count();
                var weeklyUsedCheckCount = weeklyUsedCheckQuery.Count();
                var weeklyProductCount = weeklyQuery.Count();
                var weeklyburnedCheckCount = burnedCheckQuery.Count();


                ViewBag.weeklyProductCount = weeklyProductCount;
                ViewBag.weeklyOrderWeight = weeklyWeight;
                ViewBag.weeklyPostCount = weeklyPostCount;
                ViewBag.weeklySoldCheckCount = weeklySoldCheckCount;
                ViewBag.weeklyUsedCheckCount = weeklyUsedCheckCount;
                ViewBag.weeklyOrderCount = weeklyOrderCount;
                ViewBag.weeklyburnedCheckCount = weeklyburnedCheckCount;


                ///گزارش ماهیانه
                var mounthlyQuery = db.OrderDetail.Where(x => x.CreateDate >= mounthTime);
                var mounthlyOrderCount = mounthlyQuery.Where(x => x.Order.OrderStatus == OrderStatus.Normal).Select(x => x.OrderId).Distinct().Count();
                var mounthlyWeight = mounthlyQuery.Sum(x => x.Product.Weight * x.Count);
                var mounthlyOrderCountWeight = mounthlyQuery.Sum(x => x.Product.Weight * x.Count);
                var mounthlyOrderCountProductCount = mounthlyQuery.Count();
                var mounthlyPostquery = db.PostItem.Where(x => x.CreateDate >= mounthTime);
                var mounthlyPostCount = mounthlyPostquery.Count();
                var mounthlySoldCheckQuery = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.SoldToTheCustomer && x.CreateDate >= mounthTime);
                var mounthlyUsedCheckQuery = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.Used && x.CreateDate >= mounthTime);
                var mounthlyburnedCheckQuery = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.Burn && x.CreateDate >= mounthTime);

                var mounthlySoldCheckCount = mounthlySoldCheckQuery.Count();
                var mounthlyUsedCheckCount = mounthlyUsedCheckQuery.Count();
                var mounthlyProductCount = mounthlyQuery.Count();
                var mounthlyburnedCheckCount = burnedCheckQuery.Count();


                ViewBag.mounthlyProductCount = mounthlyProductCount;
                ViewBag.mounthlyOrderWeight = mounthlyWeight;
                ViewBag.mounthlyPostCount = mounthlyPostCount;
                ViewBag.mounthlySoldCheckCount = mounthlySoldCheckCount;
                ViewBag.mounthlyUsedCheckCount = mounthlyUsedCheckCount;
                ViewBag.mounthlyOrderCount = mounthlyOrderCount;
                ViewBag.mounthlyburnedCheckCount = mounthlyburnedCheckCount;


            }
            return View();
        }

        /// <summary>
        /// گزارش روزانه
        /// </summary>
        /// <returns> جیسون حاوی اطلاعات گزارش هفتگی محصولات هر کارگاه </returns>
        [Authorize(Roles = "admin")]
        public JsonResult ProductChartJson()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= DateTime.Today);

                    var Weight = query.Sum(x => x.Product.Weight * x.Count);

                    var list = query.GroupBy(x => x.Product.Workshop).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = (int)(x.Sum(y => y.Product.Weight * y.Count)),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
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
        /// گزارش روزانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش روزانه محصولات شعبه یا نمایندگی بر اساس گرم  </returns>
        [Authorize(Roles = "admin")]
        public JsonResult BranchTypeProductChartJson()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= DateTime.Today);

                    var Weight = query.Sum(x => x.Product.Weight * x.Count);

                    var list = query.GroupBy(x => x.CreateUser.Branch.BranchType).Select(x => new ChartModel
                    {
                        name = ((int)x.Key).ToString(),
                        com = (int)(x.Sum(y => y.Product.Weight * y.Count)),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 10
                    }).ToList();

                    list.ForEach(x =>
                    {
                        BranchType branchType = (BranchType)int.Parse(x.name);
                        x.name = Enums.GetTitle(branchType);
                    });
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
        /// گزارش روزانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش روزانه محصولات هر شعبه بر اساس گرم </returns>
        [Authorize(Roles = "admin")]
        public JsonResult BranchProductChartJson()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= DateTime.Today);

                    var Weight = query.Sum(x => x.Product.Weight * x.Count);

                    var list = query.GroupBy(x => x.CreateUser.Branch).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = (int)(x.Sum(y => y.Product.Weight * y.Count)),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
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
        /// گزارش روزانه
        /// </summary>
        /// <returns>  
        /// جیسون حاوی اطلاعات گزارش روزانه محصولات شعبه ها و نمایندگی بر اساس تعداد کالا </returns>
        [Authorize(Roles = "admin")]
        public JsonResult BranchTypeOrderCountChartJson()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= DateTime.Today);

                    var Weight = query.Sum(x => x.Product.Weight * x.Count);

                    var list = query.GroupBy(x => x.CreateUser.Branch).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = x.Select(y => y.ProductId).Count(),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
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
        /// گزارش روزانه
        /// </summary>
        /// <returns> جیسون حاوی اطلاعات گزارش روزانه نوع محصولات سفارش داده شده  </returns>
        [Authorize(Roles = "admin")]
        public JsonResult ProductTypeChartJson()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= DateTime.Today);
                    var Weight = query.Sum(x => x.Product.Weight * x.Count);
                    var ProductCount = query.Select(x => x.ProductId).Count();

                    var list = query.GroupBy(x => x.Product.ProductType).Select(x => new ChartModel
                    {
                        name = ((int)x.Key).ToString(),
                        com = x.Count(),
                        y = (int)(((float)x.Count() / ProductCount) * 100),
                        z = 100
                    }).ToList();

                    list.ForEach(x =>
                    {
                        x.name = Enums.GetTitle((ProductType)(int.Parse(x.name)));
                    });
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
        /// گزارش روزانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش روزانه پست های ارسال شده به هر شهر </returns>
        [Authorize(Roles = "admin")]
        public JsonResult PostChartJson()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.PostItem.Where(x => x.CreateDate >= DateTime.Today);

                    var PostCount = query.Count();

                    var list = query.GroupBy(x => x.City).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = x.Count(),
                        y = (int)(((float)x.Count() / PostCount) * 100),
                        z = 5
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
        /// گزارش روزانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش روزانه بن خرید </returns>
        [Authorize(Roles = "admin")]
        public JsonResult GiftChartJson()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.Used && x.CreateDate >= DateTime.Today);

                    var count = query.Count();

                    var list = query.GroupBy(x => x.CreateUser.Branch).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = x.Count(),
                        y = (int)(((float)x.Count() / count) * 100),
                        z = 100
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
        /// گزارش هفتگی
        /// </summary>
        /// <returns> جیسون حاوی اطلاعات گزارش هفتگی محصولات هر کارگاه </returns>
        [Authorize(Roles = "admin")]
        public JsonResult WeekProductChartJson()
        {
            Response response;
            try
            {
                var WeekTime = DateTime.Today.AddDays(-7);

                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= WeekTime);

                    var Weight = query.Sum(x => x.Product.Weight * x.Count);

                    var list = query.GroupBy(x => x.Product.Workshop).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = (int)(x.Sum(y => y.Product.Weight * y.Count)),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
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
        /// گزارش هفتگی
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش هفتگی محصولات شعبه یا نمایندگی بر اساس گرم </returns>
        [Authorize(Roles = "admin")]
        public JsonResult WeekBranchTypeProductChartJson()
        {
            Response response;
            try
            {
                var WeekTime = DateTime.Today.AddDays(-7);

                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= WeekTime);

                    var Weight = query.Sum(x => x.Product.Weight * x.Count);

                    var list = query.GroupBy(x => x.CreateUser.Branch.BranchType).Select(x => new ChartModel
                    {
                        name = ((int)x.Key).ToString(),
                        com = (int)(x.Sum(y => y.Product.Weight * y.Count)),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
                    }).ToList();

                    list.ForEach(x =>
                    {
                        BranchType branchType = (BranchType)int.Parse(x.name);
                        x.name = Enums.GetTitle(branchType);
                    });
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
        /// گزارش هفتگی
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش هفتگی محصولات هر شعبه بر اساس گرم </returns>
        [Authorize(Roles = "admin")]
        public JsonResult WeekBranchProductChartJson()
        {
            Response response;
            try
            {
                var WeekTime = DateTime.Today.AddDays(-7);

                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= WeekTime);

                    var Weight = query.Sum(x => x.Product.Weight * x.Count);

                    var list = query.GroupBy(x => x.CreateUser.Branch).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = (int)(x.Sum(y => y.Product.Weight * y.Count)),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
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
        /// گزارش روزانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش هفتگی محصولات شعبه ها و نمایندگی بر اساس تعداد کالا </returns>
        [Authorize(Roles = "admin")]
        public JsonResult WeekBranchTypeOrderCountChartJson()
        {
            Response response;
            try
            {
                var WeekTime = DateTime.Today.AddDays(-7);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= WeekTime);

                    var Weight = query.Sum(x => x.Product.Weight * x.Count);

                    var list = query.GroupBy(x => x.CreateUser.Branch).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = x.Select(y => y.ProductId).Count(),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
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
        /// گزارش هفتگی
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش هفتگی نوع محصولات سفارش داده شده </returns>
        [Authorize(Roles = "admin")]
        public JsonResult WeekProductTypeChartJson()
        {
            Response response;
            try
            {
                var WeekTime = DateTime.Today.AddDays(-7);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= WeekTime);
                    var Weight = query.Sum(x => x.Product.Weight * x.Count);
                    var ProductCount = query.Count();
                    var list = query.GroupBy(x => x.Product.ProductType).Select(x => new ChartModel
                    {
                        name = ((int)x.Key).ToString(),
                        com = x.Count(),
                        y = (int)(((float)x.Count() / ProductCount) * 100),
                        z = 100
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.name = Enums.GetTitle((ProductType)(int.Parse(x.name)));
                    });
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
        /// گزارش هفتگی
        /// </summary>
        /// <returns> جیسون حاوی اطلاعات گزارش هفتگی پست های ارسال شده به هر شهر  </returns>
        [Authorize(Roles = "admin")]
        public JsonResult WeekPostChartJson()
        {
            Response response;
            try
            {
                var WeekTime = DateTime.Today.AddDays(-7);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.PostItem.Where(x => x.CreateDate >= WeekTime);

                    var PostCount = query.Select(x => x.Id).Count();

                    var list = query.GroupBy(x => x.City).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = x.Count(),
                        y = (int)(((float)x.Count() / PostCount) * 100),
                        z = 100
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
        /// گزارش هفتگی
        /// </summary>
        /// <returns> جیسون حاوی اطلاعات گزارش هفتگی بن خرید  </returns>
        [Authorize(Roles = "admin")]
        public JsonResult WeekGiftChartJson()
        {
            Response response;
            try
            {
                var WeekTime = DateTime.Today.AddDays(-7);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.Used && x.CreateDate >= WeekTime);
                    var count = query.Count();
                    var list = query.GroupBy(x => x.CreateUser.Branch).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = x.Count(),
                        y = (int)(((float)x.Count() / count) * 100),
                        z = 100
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
        /// گزارش ماهیانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش هفتگی محصولات هر کارگاه </returns>
        [Authorize(Roles = "admin")]
        public JsonResult MounthProductChartJson()
        {
            Response response;
            try
            {
                var mounthTime = DateTime.Today.AddDays(-30);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= mounthTime);
                    var Weight = query.Sum(x => x.Product.Weight * x.Count);
                    var list = query.GroupBy(x => x.Product.Workshop).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = (int)(x.Sum(y => y.Product.Weight * y.Count)),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
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
        /// گزارش ماهیانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش ماهیانه محصولات شعبه یا نمایندگی بر اساس گرم </returns>
        [Authorize(Roles = "admin")]
        public JsonResult MounthBranchTypeProductChartJson()
        {
            Response response;
            try
            {
                var mounthTime = DateTime.Today.AddDays(-30);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= mounthTime);
                    var Weight = query.Sum(x => x.Product.Weight * x.Count);
                    var list = query.GroupBy(x => x.CreateUser.Branch.BranchType).Select(x => new ChartModel
                    {
                        name = ((int)x.Key).ToString(),
                        com = (int)(x.Sum(y => y.Product.Weight * y.Count)),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
                    }).ToList();
                    list.ForEach(x =>
                    {
                        BranchType branchType = (BranchType)int.Parse(x.name);
                        x.name = Enums.GetTitle(branchType);
                    });
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
        /// گزارش ماهانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش ماهیانه محصولات </returns>
        [Authorize(Roles = "admin")]
        public JsonResult MounthBranchProductChartJson()
        {
            Response response;
            try
            {
                var mounthTime = DateTime.Today.AddDays(-30);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= mounthTime);
                    var Weight = query.Sum(x => x.Product.Weight * x.Count);
                    var list = query.GroupBy(x => x.CreateUser.Branch).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = (int)(x.Sum(y => y.Product.Weight * y.Count)),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
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
        /// گزارش ماهانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش ماهیانه محصولات شعبه یا نمایندگی بر اساس گرم </returns>
        [Authorize(Roles = "admin")]
        public JsonResult MounthBranchTypeOrderCountChartJson()
        {
            Response response;
            try
            {
                var mounthTime = DateTime.Today.AddDays(-30);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= mounthTime);
                    var Weight = query.Sum(x => x.Product.Weight * x.Count);
                    var list = query.GroupBy(x => x.CreateUser.Branch).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = x.Select(y => y.ProductId).Count(),
                        y = (int)(x.Sum(y => y.Product.Weight * y.Count).Value / Weight * 100),
                        z = 100
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
        /// گزارش ماهیانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش ماهیانه نوع محصولات سفارش داده شده </returns>
        [Authorize(Roles = "admin")]
        public JsonResult MounthProductTypeChartJson()
        {
            Response response;
            try
            {
                var mounthTime = DateTime.Today.AddDays(-30);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.OrderDetail.Where(x => x.CreateDate >= mounthTime);
                    var Weight = query.Sum(x => x.Product.Weight * x.Count);
                    var ProductCount = query.Select(x => x.ProductId).Count();
                    var list = query.GroupBy(x => x.Product.ProductType).Select(x => new ChartModel
                    {
                        name = ((int)x.Key).ToString(),
                        com = x.Count(),
                        y = (int)(((float)x.Count() / ProductCount) * 100),
                        z = 100
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.name = Enums.GetTitle((ProductType)(int.Parse(x.name)));
                    });
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
        /// گزارش ماهیانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش ماهیانه پست های ارسال شده به هر شهر </returns>
        [Authorize(Roles = "admin")]
        public JsonResult MounthPostChartJson()
        {
            Response response;
            try
            {
                var mounthTime = DateTime.Today.AddDays(-30);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.PostItem.Where(x => x.CreateDate >= mounthTime);
                    var PostCount = query.Select(x => x.Id).Count();
                    var list = query.GroupBy(x => x.City).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = x.Count(),
                        y = (int)(((float)x.Count() / PostCount) * 100),
                        z = 100
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
        /// گزارش ماهیانه
        /// </summary>
        /// <returns>جیسون حاوی اطلاعات گزارش ماهیانه بن خرید </returns>
        [Authorize(Roles = "admin")]
        public JsonResult MounthGiftChartJson()
        {
            Response response;
            try
            {
                var mounthTime = DateTime.Today.AddDays(-30);
                using (var db = new KiaGalleryContext())
                {
                    var query = db.GiftLog.Where(x => x.Gift.GiftType == GiftType.Check && x.GiftStatus == GiftStatus.Used && x.CreateDate >= mounthTime);
                    var count = query.Count();
                    var list = query.GroupBy(x => x.CreateUser.Branch).Select(x => new ChartModel
                    {
                        name = x.Key.Name,
                        com = x.Count(),
                        y = (int)(((float)x.Count() / count) * 100),
                        z = 100
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
    }
}