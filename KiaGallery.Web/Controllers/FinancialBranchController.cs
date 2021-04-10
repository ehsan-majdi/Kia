using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Order;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class FinancialBranchController : BaseController
    {
        /// <summary>
        /// مدیریت مالی شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, FinancialBranch")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.OrderBy(x => x.Order).Where(x => x.Active == true).ToList();
            }
            return View();
        }

        /// <summary>
        /// مدیریت مالی شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, FinancialBranch,branchGoldManage")]
        public ActionResult Calculate()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.OrderBy(x => x.Order).Where(x => x.Active == true).ToList();
                ViewBag.DateSetting = db.Settings.Where(x => x.Key == Settings.KeyInPreprationGoldDebtDateRange).SingleOrDefault()?.Value;
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, branchGoldManage")]
        public JsonResult SaveSetting(string value)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var dateSetting = db.Settings.Where(x => x.Key == Settings.KeyInPreprationGoldDebtDateRange).SingleOrDefault();
                    dateSetting.Value = value;
                    db.SaveChanges();
                };
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
        /// جستجوی مالی شعب
        /// </summary>
        /// <returns>لیست شعب پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, FinancialBranch")]
        public JsonResult Search()
        {
            List<FinancialBranch> listFinancialBranch;
            var currentUser = GetAuthenticatedUser().Id;
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    IQueryable<Branch> query = null;
                    List<int> idList = new List<int> { };

                    switch (currentUser)
                    {
                        case 1092:
                            idList = new List<int> { 10, 34, 15 };
                            query = GetQuery(db, idList);
                            break;
                        case 1093:
                            idList = new List<int> { 9, 7, 12 };
                            query = GetQuery(db, idList);
                            break;
                        case 1091:
                            idList = new List<int> { 11, 5, 41 };
                            query = GetQuery(db, idList);
                            break;
                        case 1094:
                            idList = new List<int> { 8, 13 };
                            query = GetQuery(db, idList);
                            break;
                        case 1102:
                            idList = new List<int> { 35, 34, 20, 6 };
                            query = GetQuery(db, idList);
                            break;
                        default:
                            query = GetQuery(db, db.Branch.Where(x => x.BranchType == BranchType.Solicitorship || x.BranchType == BranchType.CoWorker).Select(x => x.Id).ToList());
                            break;
                    }

                    listFinancialBranch = query.Select(x => new FinancialBranch()
                    {
                        id = x.Id,
                        name = x.Name,
                        goldCredit = x.GoldCredit,
                        goldDebt = x.GoldDebt,
                        rialDebt = x.RialDebt,
                        goldWeightCart = x.CartList.Sum(y => y.Product.Weight * y.Count)
                    }).ToList();
                }

                listFinancialBranch.ForEach(x =>
                {
                    if (x.goldWeightCart != 0 && x.goldWeightCart != null)
                        x.goldWeightCart = Math.Round(double.Parse(x.goldWeightCart.ToString()), 2);
                });

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = listFinancialBranch
                    }
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public IQueryable<Branch> GetQuery(KiaGalleryContext db, List<int> idList)
        {
            var query = db.Branch.Where(x => x.Active == true && (x.BranchType == BranchType.Solicitorship || x.BranchType == BranchType.CoWorker) && idList.Any(y => y == x.Id)).OrderBy(x => x.Order);

            return query;
        }

        /// <summary>
        /// ذخیره مالی شعب
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات شعبه</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, FinancialBranch")]
        public JsonResult Save(List<FinancialBranch> listFinancialBranch)
        {
            Response response;
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    List<string> keyList = new List<string> { Settings.KeyLastFinancialUpdate };
                    List<Settings> settingList = db.Settings.Where(x => keyList.Any(y => y == x.Key)).ToList();
                    var branch = db.Branch.Where(x => x.Active == true && x.BranchType == BranchType.Solicitorship || x.BranchType == BranchType.CoWorker).ToList();
                    branch.ForEach(x =>
                    {
                        var item = listFinancialBranch.FirstOrDefault(y => y.id == x.Id);
                        if (item != null)
                        {
                            x.GoldCredit = item.goldCredit;
                            x.GoldDebt = item.goldDebt;
                            x.RialDebt = item.rialDebt;
                        }
                    });
                    var lastUpdate = "ehsan";
                    Settings lastUpdateKey = settingList.SingleOrDefault(x => x.Key == Settings.KeyLastFinancialUpdate);
                    if (lastUpdateKey != null)
                    {
                        settingList.SingleOrDefault(x => x.Key == Settings.KeyLastFinancialUpdate).Value = lastUpdate;
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "اطلاعات شعب با موفقیت ویراش شد."
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
        /// <param name="model">لیست فیلتر های برای اعمال </param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, FinancialBranch")]
        public JsonResult MakeOrder(int branchId, string cartIdList, int cartType, List<int> workshop)
        {
            Response response;
            try
            {
                List<Cart> result;
                List<OrderDetailLog> loglist = new List<OrderDetailLog>();
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    Branch branch = db.Branch.Single(x => x.Id == branchId);
                    var query = db.Cart.Where(x => x.BranchId == branchId);
                    if (cartIdList != null && !string.IsNullOrEmpty(cartIdList))
                    {
                        var idList = cartIdList.Split('-').Select(x => int.Parse(x)).ToList();
                        query = query.Where(x => idList.Any(y => y == x.Id));
                    }
                    else
                    {
                        if (cartType == 1)
                            query = query.Where(x => string.IsNullOrEmpty(x.Customer) && x.BranchId == user.BranchId);
                        else if (cartType == 2)
                            query = query.Where(x => !string.IsNullOrEmpty(x.Customer) && x.BranchId == user.BranchId);

                        if ((cartType == 0 || cartType == 1) && workshop != null && workshop.Count > 0)
                        {
                            query = query.Where(x => workshop.Any(y => y == x.Product.WorkshopId));
                        }
                    }

                    result = query.ToList();

                    if (result.Count > 0)
                    {

                        var orderSerial = GetNewOrderNo(db, branch);

                        #region MakeOrder
                        var order = new Order()
                        {
                            BranchId = branch.Id,
                            OrderSerial = orderSerial,
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

                        order.OrderDetailList = new List<OrderDetail>();
                        result.ForEach(x =>
                        {

                            var orderDetail = new OrderDetail()
                            {
                                Order = order,
                                ProductId = x.ProductId,
                                Product = x.Product,
                                OrderType = x.OrderType,
                                OrderDetailStatus = x.Product.Workshop.AutoConfirm ? OrderDetailStatus.InWorkshop : OrderDetailStatus.Registered,
                                Size = x.Size,
                                SetNumber = x.SetNumber != null ? lastSetNumber + x.SetNumber : null,
                                GoldType = x.GoldType,
                                ProductColor = x.ProductColor,
                                OuterWerkType = x.OuterWerkType,
                                LeatherLoop = x.LeatherLoop,
                                Customer = x.Customer,
                                PhoneNumber = x.PhoneNumber,
                                ForceOrder = x.ForceOrder,
                                DeliverDateRequest = x.DeliverDateRequest,
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
                            };
                            orderDetail.OrderDetailLogList.Add(new OrderDetailLog()
                            {
                                OrderDetail = orderDetail,
                                OrderDetailStatus = OrderDetailStatus.Ordered,
                                CreateUserId = x.CreateUserId,
                                CreateDate = DateTime.Now,
                                Ip = Request.UserHostAddress,
                            });

                            order.OrderDetailList.Add(orderDetail);
                        });

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
                            foreach (var item in loglist)
                            {
                                item.OrderDetail = x;
                                x.OrderDetailLogList.Add(item);
                            }

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
                                WorkshopOrderSerial = orderSerial + "-WS-1",
                                CreateUserId = user.Id,
                                ModifyUserId = user.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress,
                            };

                            db.WorkshopOrder.Add(wsOrder);
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
                            message = "سفارش ثبت شد."
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
        /// ساخت سفارش از سبد خرید
        /// </summary>
        /// <param name="model">لیست فیلتر های برای اعمال </param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, FinancialBranch,branchGoldManage")]
        public JsonResult MakeOrderAll(List<int> branchIdList, string cartIdList, int cartType, List<int> workshop)
        {
            Response response;
            try
            {
                List<Cart> result;
                List<OrderDetailLog> loglist = new List<OrderDetailLog>();
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var branch = db.Branch.Where(x => branchIdList.Contains(x.Id)).ToList();
                    var query = db.Cart.Where(x => branchIdList.Contains(x.BranchId));
                    if (cartIdList != null && !string.IsNullOrEmpty(cartIdList))
                    {
                        var idList = cartIdList.Split('-').Select(x => int.Parse(x)).ToList();
                        query = query.Where(x => idList.Any(y => y == x.Id));
                    }
                    else
                    {
                        if (cartType == 1)
                            query = query.Where(x => string.IsNullOrEmpty(x.Customer) && x.BranchId == user.BranchId);
                        else if (cartType == 2)
                            query = query.Where(x => !string.IsNullOrEmpty(x.Customer) && x.BranchId == user.BranchId);

                        if ((cartType == 0 || cartType == 1) && workshop != null && workshop.Count > 0)
                        {
                            query = query.Where(x => workshop.Any(y => y == x.Product.WorkshopId));
                        }
                    }

                    result = query.ToList();

                    if (result.Count > 0)
                    {

                        foreach (var entity in branch)
                        {
                            var orderSerial = GetNewOrderNo(db, entity);
                            #region MakeOrder
                            var order = new Order()
                            {
                                BranchId = entity.Id,
                                OrderSerial = orderSerial,
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

                            order.OrderDetailList = new List<OrderDetail>();
                            result.ForEach(x =>
                            {

                                var orderDetail = new OrderDetail()
                                {
                                    Order = order,
                                    ProductId = x.ProductId,
                                    Product = x.Product,
                                    OrderType = x.OrderType,
                                    OrderDetailStatus = x.Product.Workshop.AutoConfirm ? OrderDetailStatus.InWorkshop : OrderDetailStatus.Registered,
                                    Size = x.Size,
                                    SetNumber = x.SetNumber != null ? lastSetNumber + x.SetNumber : null,
                                    GoldType = x.GoldType,
                                    ProductColor = x.ProductColor,
                                    OuterWerkType = x.OuterWerkType,
                                    LeatherLoop = x.LeatherLoop,
                                    Customer = x.Customer,
                                    PhoneNumber = x.PhoneNumber,
                                    ForceOrder = x.ForceOrder,
                                    DeliverDateRequest = x.DeliverDateRequest,
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
                                };
                                orderDetail.OrderDetailLogList.Add(new OrderDetailLog()
                                {
                                    OrderDetail = orderDetail,
                                    OrderDetailStatus = OrderDetailStatus.Ordered,
                                    CreateUserId = x.CreateUserId,
                                    CreateDate = DateTime.Now,
                                    Ip = Request.UserHostAddress,
                                });

                                order.OrderDetailList.Add(orderDetail);
                            });

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
                                foreach (var item in loglist)
                                {
                                    item.OrderDetail = x;
                                    x.OrderDetailLogList.Add(item);
                                }

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

                            db.Order.Add(order);
                            db.SaveChanges();
                            order.OrderSerial = entity.Alias + "-" + order.Id;
                            order.OrderNumber = order.Id.ToString();
                            var autoConfirmList = order.OrderDetailList.Where(x => x.OrderDetailStatus == OrderDetailStatus.InWorkshop).ToList();
                            if (autoConfirmList.Count > 0)
                            {
                                WorkshopOrder wsOrder = new WorkshopOrder()
                                {
                                    Order = order,
                                    WorkshopOrderSerial = orderSerial + "-WS-1",
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress,
                                };

                                db.WorkshopOrder.Add(wsOrder);
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
                        }



                        response = new Response()
                        {
                            status = 200,
                            message = "سفارش ثبت شد."
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
        /// مدیریت سبد سفارش
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, FinancialBranch")]
        public ActionResult Cart(int id)
        {

            ViewBag.BranchId = id;
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchName = db.Branch.Single(x => x.Id == id).Name;
            }
            ViewBag.CartType = 0; // سفارشات برای ویترین
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.OrderBy(x => x.Order).Where(x => x.Active == true).ToList();
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin, FinancialBranch,branchGoldManage")]
        public ActionResult ManipulateAll()
        {

            ViewBag.CartType = 0; // سفارشات برای ویترین
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.OrderBy(x => x.Order).Where(x => x.Active == true).ToList();
                ViewBag.BranchList = db.Branch.OrderBy(x => x.Order).Where(x => x.BranchType == BranchType.Solicitorship && x.Active == true).Select(x=>x.Id).ToList();
            }
            return View();
        }

        //public JsonResult GetAllCart()
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {

        //            var list = db.
        //                 response = new Response()
        //                 {
        //                     status = status,
        //                     message = message
        //                 };
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// حذف کردن محصول از سبد خرید
        /// </summary>
        /// <param name="id">ردیف سبد خرید</param>
        /// <param name="productId">ردیف محصول</param>
        /// <returns>جیسون نتیجه عملیات حذف</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult RemoveCartItem(int branchId, string id)
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
        /// افزایش تعداد بر اساس یک سفارش
        /// </summary>
        /// <param name="id">لیست ردیف محصول که می بایست تکرار شود</param>
        /// <returns>جیسون حاوی نتیجه افزایش تعداد</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult Increment(int branchId, string id)
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
                            ProductColor = entity.ProductColor,
                            LeatherLoop = entity.LeatherLoop,
                            Customer = entity.Customer,
                            PhoneNumber = entity.PhoneNumber,
                            ForceOrder = entity.ForceOrder,
                            DeliverDateRequest = entity.DeliverDateRequest,
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
        public JsonResult Decrement(int branchId, string id)
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

                    var entity = db.Cart.OrderByDescending(x => x.Id).First(x => x.BranchId == branchId && idList.Any(y => y == x.Id));
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
        /// لیست محصولات درون سبد خرید به صورت تجمیعی
        /// </summary>
        /// <param name="model">لیست پارامترهای جستجو</param>
        /// <returns>محصولات درون سبد خرید</returns>
        [HttpPost]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public JsonResult CartListAggregation(FinancialBranchCartSearchViewModel model)
        {
            Response response;
            try
            {
                string meli = " sss ";
                string sara = meli.Trim();
                //List<Cart> list;
                var user = GetAuthenticatedUser();
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Cart.Where(x=>x.Branch.BranchType == BranchType.Solicitorship || x.Branch.BranchType == BranchType.CoWorker).Select(x => x);

                    if (model.branchId > 0)
                    {
                        query = query.Where(x => x.BranchId == model.branchId);
                    }

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

                    if (model.workshopIdList != null && model.workshopIdList.Count() > 0)
                    {
                        query = query.Where(x => model.workshopIdList.Contains(x.Product.WorkshopId));
                    }

                    if (model.productType != null)
                    {
                        query = query.Where(x => x.Product.ProductType == model.productType);
                    }

                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).ThenByDescending(x => x.SetNumber);

                    //list = query.ToList();

                    List<CartListViewModel> list = query.Select(item => new CartListViewModel()
                    {
                        id = item.Id,
                        fileName = item.Product.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        orderType = item.OrderType,
                        //orderTypeTitle = Enums.GetTitle(item.OrderType),
                        productId = item.ProductId,
                        branchId = item.BranchId,
                        workshopName = item.Product.Workshop.Name,
                        productSizeId = item.Product.SizeId,
                        workshopId = item.Product.WorkshopId,
                        size = item.Size,
                        setNumber = item.SetNumber,
                        goldType = item.GoldType,
                        outerWerkType = item.OuterWerkType,
                        leatherLoop = item.LeatherLoop,
                        customer = item.Customer,
                        phoneNumber = item.PhoneNumber,
                        forceOrder = item.ForceOrder,
                        deliverDateRequest = item.DeliverDateRequest,
                        productColor = item.ProductColor,
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
                        }).ToList()
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
                        x.branchId,
                        x.workshopName,
                        x.productSizeId,
                        x.title,
                        x.bookCode,
                        x.code,
                        x.weight,
                        x.size,
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
                        branchId = x.Key.branchId,
                        workshopName = x.Key.workshopName,
                        productSizeId = x.Key.productSizeId,
                        title = x.Key.title,
                        code = x.Key.code,
                        bookCode = x.Key.bookCode,
                        weight = x.Key.weight,
                        size = x.Key.size,
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
                        createDate = x.Count() > 1 ? DateUtility.GetPersianDate(x.OrderByDescending(y => y.createDate).First().createDate) : DateUtility.GetPersianDate(x.First().createDate),
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
                            weight = list.Sum(x => x.weight),
                            workshopWeight = list.GroupBy(x => x.workshopId).Select(x => new { workshopId = x.Key, weight = x.Sum(y => y.weight * y.count) }).ToList()
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
        /// دریافت شماره سفارش جدید برای شعبه
        /// </summary>
        /// <param name="db">شی اتصال به پایگاه داده</param>
        /// <param name="branch">شعبه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        private string GetNewOrderNo(KiaGalleryContext db, Branch branch)
        {
            //var lastItem = db.Order.Where(x => x.BranchId == branch.Id).OrderByDescending(x => x.OrderSerial).FirstOrDefault();
            //if (lastItem == null)
            //{
            //    return branch.Alias.ToUpper() + 1.ToString();
            //}
            //else
            //{
            //    var lastNo = lastItem.OrderSerial.Split(new string[] { branch.Alias.ToUpper() }, StringSplitOptions.None)[1];
            //    if (lastNo.IndexOf('-') >= 0)
            //    {
            //        lastNo = lastNo.Split('-')[1];
            //    }
            //    var newNo = int.Parse(lastNo) + 1;
            //    return branch.Alias.ToUpper() +"-"+ newNo.ToString();
            //}

            var lastItem = db.Order.OrderByDescending(x => x.Id).FirstOrDefault();
            var newNo = lastItem.Id + 1;
            return branch.Alias.ToUpper() + "-" + newNo.ToString();
        }

        [Authorize(Roles = "admin, branchGoldManage")]
        [Route("financialBranch/getData/{branchType}")]
        public JsonResult GetData(BranchType branchType)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {

                    double dateSetting = double.Parse(db.Settings.Where(x => x.Key == Settings.KeyInPreprationGoldDebtDateRange).SingleOrDefault()?.Value);
                    dateSetting = dateSetting * -1;
                    var dateRange = DateTime.Today.AddDays(dateSetting);
                    var query = db.Branch.Where(x => x.BranchType == branchType);

                    var list = query.Select(x => new GoldDebtViewModel
                    {
                        id = x.Id,
                        inPreprationDebt = x.OrderList.Sum(y => y.OrderDetailList.Where(z => z.CreateDate >= DbFunctions.TruncateTime(dateRange) && z.OrderDetailStatus != OrderDetailStatus.Cancel && z.OrderDetailStatus != OrderDetailStatus.Sent).Sum(z => z.Product.Weight * z.Count)),
                        name = x.Name,
                        //date = x.Select(y => y.CreateDate).ToList(),
                        debt = x.GoldDebt,
                        rialDebt = x.RialDebt,
                        sumDebt = x.GoldDebt + x.OrderList.Sum(y => y.OrderDetailList.Where(z => z.CreateDate >= DbFunctions.TruncateTime(dateRange) && z.OrderDetailStatus != OrderDetailStatus.Cancel && z.OrderDetailStatus != OrderDetailStatus.Sent).Sum(z => z.Product.Weight * z.Count)),
                        credit = x.GoldCredit,
                        goldWeightCart = x.CartList.Sum(y => y.Product.Weight * y.Count),
                        order = x.Order,
                        debtSumWeight = x.GoldDebt + x.OrderList.Sum(y => y.OrderDetailList.Where(z => z.CreateDate >= DbFunctions.TruncateTime(dateRange) && z.OrderDetailStatus != OrderDetailStatus.Cancel && z.OrderDetailStatus != OrderDetailStatus.Sent).Sum(z => z.Product.Weight * z.Count)),
                    }).OrderBy(x => x.order).ToList();

                    list.ForEach(x =>
                    {
                        if (x.goldWeightCart != 0 && x.goldWeightCart != null)
                            x.goldWeightCart = Math.Round(double.Parse(x.goldWeightCart.ToString()), 2);
                        if (x.inPreprationDebt != 0 && x.inPreprationDebt != null)
                            x.inPreprationDebtRound = Math.Round(double.Parse(x.inPreprationDebt.ToString()), 2);
                        if (x.debtSumWeight != 0 && x.inPreprationDebt != null)
                            x.debtSumWeightRound = Math.Round(double.Parse(x.debtSumWeight.ToString()), 2);
                    });

                    //var query2 = db.OrderDetail.Where(x => query.Any(y => y.id == x.Order.BranchId) && x.CreateDate < dateRange).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            sumDebt = list.Sum(x => x.debt),
                            sumInPreprationWeight = list.Sum(x => x.inPreprationDebtRound),
                            sumDebtWeight = list.Sum(x => x.debtSumWeightRound),
                            sumCartGoldWeight = list.Sum(x => x.goldWeightCart)
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

        public class GoldDebtViewModel
        {
            public string name { get; set; }
            public int id { get; set; }
            public float? inPreprationDebt { get; set; }
            public double inPreprationDebtRound { get; set; }
            public float? debtSumWeight { get; set; }
            public double debtSumWeightRound { get; set; }
            public List<DateTime> date { get; set; }
            public string persianDate { get; set; }
            public int count { get; set; }
            public int order { get; set; }
            public float debt { get; set; }
            public float rialDebt { get; set; }
            public float? sumDebt { get; set; }
            public float credit { get; set; }
            public double? goldWeightCart { get; set; }
        }
    }
}