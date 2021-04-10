//using KiaGallery.Common;
//using KiaGallery.Model.Context;
//using KiaGallery.Web.Models;
//using System;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using KiaGallery.Model;
//using KiaGallery.Model.Context.InternalOrder;

//using KiaGallery.Web.Controllers;

//namespace KiaGallery.Tracking.Controllers
//{
//    public class TrackingController : BaseController
//    {
//        [HttpGet]
//        [AllowAnonymous]
//        public ActionResult TrackLogin()
//        {
//            return View();
//        }
//        [AllowAnonymous]
//        public ActionResult Track(TrackLoginViewModel trackData)
//        {
//            InternalOrder orderData = null;
//            using (var db = new KiaGalleryContext())
//            {
//                orderData = db.InternalOrder.SingleOrDefault(x => x.PhoneNumber == trackData.phoneNumber && x.TrackCode == trackData.trackCode);
//                var query = db.InternalOrder.Where(x => x.PhoneNumber == trackData.phoneNumber);
//                if (orderData != null)
//                {
//                    ViewBag.Name = orderData.Name;
//                    switch (orderData.InternalOrderStatus)
//                    {
//                        case InternalOrderStatus.Cancel:
//                            ViewBag.Status = "لغو شده";
//                            break;
//                        case InternalOrderStatus.Registered:
//                            ViewBag.Status = "ثبت شده";
//                            break;
//                        case InternalOrderStatus.ReadyForDeliver:
//                            ViewBag.Status = "آماده تحویل میباشد";
//                            break;
//                        case InternalOrderStatus.PendingCustomer:
//                            ViewBag.Status = "خبر میده";
//                            break;
//                        case InternalOrderStatus.NoAnswer:
//                            ViewBag.Status = "جواب نداد";
//                            break;
//                        case InternalOrderStatus.Delivered:
//                            switch (orderData.DeliveryType)
//                            {
//                                case DeliveryType.DeliveryMan:
//                                    ViewBag.Status = "تحویل پیک";
//                                    break;
//                                case DeliveryType.Branch:
//                                    ViewBag.Status = "تحویل شعبه";
//                                    break;
//                                case DeliveryType.KiaPersonnel:
//                                    ViewBag.Status = "تحویل پرسنل کیا";
//                                    break;
//                                case DeliveryType.Post:
//                                    ViewBag.Status = "تحویل پست";
//                                    break;
//                                default:
//                                    ViewBag.Status = "نا مشخص";
//                                    break;
//                            }
//                            break;
//                        default:
//                            ViewBag.Status = "نا مشخص";
//                            break;
//                    }
//                    ViewBag.TrackCode = orderData.TrackCode;
//                    ViewBag.PhoneNumber = orderData.PhoneNumber;
//                    ViewBag.ProductType = orderData.ProductType;
//                    if (orderData.LeatherId != null)
//                    {
//                        ViewBag.Leather = orderData.Leather.Name;
//                    }
//                    else
//                    {
//                        if (orderData.ProductId != null)
//                        {
//                            if (orderData.Product.LeatherCount > 1)
//                            {
//                                ViewBag.Leather = "در توضیحات";
//                            }
//                            else
//                            {
//                                ViewBag.Leather = "چرم ندارد";
//                            }
//                        }
//                        else
//                        {
//                            ViewBag.Leather = "چرم ندارد";
//                        }
//                    }



//                    if (orderData.StoneId != null)
//                    {
//                        ViewBag.Stone = orderData.Stone.Name;
//                    }
//                    else
//                    {
//                        if (orderData.ProductId != null)
//                        {
//                            if (orderData.Product.StoneCount > 1)
//                            {
//                                ViewBag.Stone = "در توضیحات";
//                            }
//                            else
//                            {
//                                ViewBag.Stone = "سنگ ندارد";
//                            }
//                        }
//                        else
//                        {
//                            ViewBag.Stone = "سنگ ندارد";
//                        }
//                    }
//                    ViewBag.LeathearLoop = orderData.LeatherLoop;
//                    ViewBag.Size = orderData.Size;
//                    ViewBag.ProductName = orderData.Title;
//                    ViewBag.Date = Common.DateUtility.GetPersianDate(orderData.Date);
//                    ViewBag.Description = orderData.Description;
//                    ViewBag.Deposit = orderData.Deposit;
//                    ViewBag.FileName = orderData.Product?.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName;




//                    if (orderData.GoldType == GoldType.Shiny)
//                    {
//                        ViewBag.ProductType = "براق";
//                    }
//                    if (orderData.GoldType == GoldType.Matte)
//                    {
//                        ViewBag.ProductType = "مات";
//                    }

//                    if (orderData.PonyUp == true)
//                    {
//                        ViewBag.PonyUp = "تسویه شده";
//                    }
//                    else
//                    {
//                        ViewBag.PonyUp = "تسویه نشده";
//                    }
//                }
//                else ViewBag.message = "موردی یافت نشد";


//            }
//            return View();

//        }
//        [HttpGet]
//        [AllowAnonymous]
//        public ActionResult OrderList(string phoneNumber)
//        {
//            ViewBag.PhoneNumber = phoneNumber;

//            return View();
//        }
//        [HttpGet]
//        [AllowAnonymous]
//        public JsonResult GetData(string phoneNumber)
//        {
//            Response response;

//            try
//            {
//                using (var db = new KiaGalleryContext())
//                {
//                    var query = db.InternalOrder.Where(x => x.PhoneNumber == phoneNumber);
//                    var list = query.Select(x => new TrackOrderListViewModel
//                    {
//                        name = x.Name,
//                        ponyUp = x.PonyUp,
//                        status = x.InternalOrderStatus,
//                        productCode = x.Title,
//                        trackCode = x.TrackCode,
//                        date = x.Date,
//                        description = x.Description,
//                        phoneNumber = x.PhoneNumber,
//                        deposit = x.Deposit,
//                        branchName = x.Branch.Name,
//                        deliverType = x.DeliveryType


//                    }).ToList();
//                    list.ForEach(x =>
//                    {
//                        x.depositToSeparator = Core.ToSeparator(x.deposit);
//                        x.persianDate = Common.DateUtility.GetPersianDate(x.date);
//                    });

//                    response = new Response()
//                    {
//                        status = 200,
//                        data = new
//                        {
//                            list = list,

//                        }
//                    };


//                };
//            }
//            catch (Exception ex)
//            {
//                response = Core.GetExceptionResponse(ex);

//            }

//            return Json(response, JsonRequestBehavior.AllowGet);

//        }

//    }
//}