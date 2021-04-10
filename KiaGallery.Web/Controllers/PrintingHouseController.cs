using DocumentFormat.OpenXml.Bibliography;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Telegram.Bot.Types;

namespace KiaGallery.Web.Controllers
{
    public class PrintingHouseController : BaseController
    {
        // GET: PrintingHouse
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// صفحه ویرایش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin ")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// افزودن چاپخانه
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(int? id)
        {
            ViewBag.Id = id;
            return View("Edit");
        }

        /// <summary>
        /// ذخیره چاپخانه
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin ")]
        public JsonResult Save(PrintingHouseViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {
                        var printingHouse = db.PrintingHouse.Where(x => x.Id == model.id).SingleOrDefault();
                        printingHouse.Name = model.name;
                        printingHouse.Code = model.code;
                        printingHouse.PhoneNumber = model.phoneNumber;
                        printingHouse.Phone = model.phone;
                        printingHouse.Management = model.management;
                        printingHouse.Address = model.address;
                        printingHouse.Active = model.active;
                        printingHouse.ModifyUserId = userId;
                        printingHouse.ModifyDate = DateTime.Now;
                        printingHouse.Ip = Request.UserHostAddress;
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "چاپخانه ویرایش شد",
                        };
                    }
                    else
                    {
                        var item = new PrintingHouse()
                        {
                            Name = model.name,
                            Code = model.code,
                            PhoneNumber = model.phoneNumber,
                            Phone = model.phone,
                            Management = model.management,
                            Address = model.address,
                            Active = model.active,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.PrintingHouse.Add(item);
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "چاپخانه جدید ثبت شد",
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
        /// خواندن اطلاعات چاپخانه
        /// </summary>
        /// <param name="id">ردیف چاپخانه</param>
        /// <returns>جیسون اطلاعات لود شده چاپخانه</returns>
        [HttpGet]
        [Authorize(Roles = "admin ")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                PrintingHouse item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.PrintingHouse.FirstOrDefault(x => x.Id == id);
                }
                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new PrintingHouseViewModel
                        {
                            id = item.Id,
                            name = item.Name,
                            code = item.Code,
                            phoneNumber = item.PhoneNumber,
                            phone = item.Phone,
                            management = item.Management,
                            address = item.Address,
                            active = item.Active,
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "چاپخانه مورد نظر یافت نشد."
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
        /// لیست چاپخانه
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin ")]
        public JsonResult Search(PrintingHouseViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    List<PrintingHouseViewModel> list;
                    var query = db.PrintingHouse.Select(x => x);
                    list = query.Select(item => new PrintingHouseViewModel()
                    {
                        id = item.Id,
                        name = item.Name,
                        phone = item.Phone,
                        phoneNumber = item.PhoneNumber,
                        management = item.Management,
                        active = item.Active,
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
        /// حذف 
        /// </summary>
        /// <param name="id">ردیف چاپخانه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.PrintingHouse.Find(Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "چاپخانه با موفقیت حذف شد."
                    };
                    db.PrintingHouse.Remove(item);
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
        /// سفارش های نهایی برای ارسال به چاپخانه مورد نظر
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, printingHouse")]
        public ActionResult FinalOrderPrintinHouse()
        {
            return View();
        }
        /// <summary>
        /// برگرداندن لیست سفارشات نهایی از دفتر مرکزی برای چاپخانه ها
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, printingHouse")]
        public JsonResult GetfinalOrder(PaginationFinalOrderViewModel model)
        {
            Response response;
            int dataCount;
            var user = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    List<GetFinalOrderPrintingHouseViewModel> list;
                    var query = db.OrderUsableProduct.Where(x => x.PrintingHouserOrder == true).Select(x => x);
                    dataCount = query.Count();
                    query = query.OrderBy(x => x.CreateDate).Skip(model.page * model.count).Take(model.count);
                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.Branch.Name.ToString().Contains(model.term.Trim()) || x.Id.ToString().Contains(model.term.Trim()));
                    }
                    if (!User.IsInRole("admin"))
                    {
                        query = query.Where(x => x.PrintingHouserOrder == true && x.OrderUsableProductDetailList.Select(y => y.UsableProduct.PrintingHouseId == user.PrintingHouseId).Count() > 0);
                    }

                    list = query.OrderByDescending(x => x.CreateDate).Select(item => new GetFinalOrderPrintingHouseViewModel()
                    {
                        id = item.Id,
                        //usableProductId = item.Id,
                        branch = item.Branch.Name,
                        alias = item.Branch.Alias,
                        createDate = item.CreateDate,
                        sumCount = item.OrderUsableProductDetailList.Sum(x => x.Count),
                        remain = item.OrderUsableProductDetailList.Where(y => y.UsableProduct.PrintingHouse.Id == user.PrintingHouseId).Count(),
                        registered = item.OrderUsableProductDetailList.Where(x => x.OrderUsableProduct.OrderUsableProductStatus == OrderUsableProductStatus.Registered).Sum(x => x.Count),
                        inPreparation = item.OrderUsableProductDetailList.Where(x => x.OrderUsableProduct.OrderUsableProductStatus == OrderUsableProductStatus.InPreparation).Sum(x => x.Count),
                        sent = item.OrderUsableProductDetailList.Where(x => x.OrderUsableProduct.OrderUsableProductStatus == OrderUsableProductStatus.Sent).Sum(x => x.Count),
                        //remain = item.OrderUsableProductDetailList.Count(),
                        orderUsableProductStatus = item.OrderUsableProductStatus,
                        printingHouseOrderStatus = item.PrintingHouseOrderStatus,

                    }).ToList();

                    list.ForEach(x => GetBackgroundColor(x));

                    var count = 0;
                    if (User.IsInRole("admin"))
                    {
                        count = query.Select(x => x.OrderUsableProductDetailList.Sum(y => y.Remain.Value)).Sum();
                    };
                    list.ForEach(x =>
                    {
                        x.orderUsableProductStatusTitle = Enums.GetTitle(x.orderUsableProductStatus);
                        x.printingHouseOrderStatusTitle = Enums.GetTitle(x.printingHouseOrderStatus);
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
        /// قرار دادن رنگ زمینه مناسب برای هر سفارش که بسته به وضعیت محصولات داخل سفارش تعیین میشود
        /// </summary>
        /// <param name="x">سفارش مورد نظر</param>
        private void GetBackgroundColor(GetFinalOrderPrintingHouseViewModel x)
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
        /// جزئیات سفارشات نهایی ارسال شده برای چاپخانه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, printingHouse")]
        public ActionResult FinalOrderDetail(int id)
        {
            ViewBag.OrderUsableProductId = id;
            return View();
        }

        /// <summary>
        /// برگرداندن لیست جزئیات سفارشات ارسال شده از سمت دفترمرکزی برای نمایش در کاربر چاپخانه
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetFinalOrderDetail(ShowOrderUsableProductViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var query = db.OrderUsableProductDetail.Where(x => x.OrderUsableProductId == model.orderUsableProductId && x.UsableProduct.PrintingHouseId == user.PrintingHouseId);
                    var list = query.Select(item => new ShowOrderUsableProductViewModel
                    {
                        id = item.Id,
                        name = item.UsableProduct.Name,
                        printingHouseId = item.UsableProduct.PrintingHouseId,
                        orderUsableProductId = item.OrderUsableProductId,
                        productCount = item.Count,
                        specification = item.Specification,
                        unit = item.UsableProduct.Unit,
                        code = item.UsableProduct.Code,
                        image = item.UsableProduct.UsableProductFileList.Select(y => y.FileName).FirstOrDefault(),
                        category = item.UsableProduct.CategoryUsableProduct.Title,
                        createUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        createBranch = item.OrderUsableProduct.Branch.Name,
                        createDate = item.CreateDate,
                        officeInventory = item.OfficeInventory.Value,
                        remain = item.Remain.Value,
                        delivered = item.Delivered,
                        confirmDelivered = item.ConfirmDelivered,
                        printingHouseInventory = item.PrintingHouseInventory,
                        remainFinal = item.RemainFinal,
                        readyForDelivery = item.ReadyForDelivery,
                        branchType = item.OrderUsableProduct.Branch.BranchType,

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
        /// چاپ کلی پرینت سفارش
        /// </summary>
        /// <param name="id">لیست ردیف های مورد نظر جهت چاپ</param>
        /// <returns>فایل پی دی اف</returns>
        [Authorize(Roles = "admin, printingHouse")]
        public ActionResult PrintOrder(int id)
        {
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                var listOrderItem = db.OrderUsableProductDetail.Where(x => x.OrderUsableProductId == id && x.UsableProduct.PrintingHouseId == user.PrintingHouseId).ToList();

                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Row");
                dataTable.Columns.Add("Name");
                dataTable.Columns.Add("Count");
                dataTable.Columns.Add("Unit");

                var rowNumber = 1;
                for (int j = 0; j < listOrderItem.Count; j++)
                {
                    DataRow row = dataTable.NewRow();
                    row["Row"] = rowNumber;
                    row["Name"] = listOrderItem[j].UsableProduct.Name;
                    row["Count"] = listOrderItem[j].Remain;
                    row["Unit"] = listOrderItem[j].UsableProduct.Unit;

                    rowNumber++;
                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);

                StiReport report = new StiReport();
                report.Load(Server.MapPath("~/Report/PrintingHouse/PrintOrder.mrt"));
                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.RegData("DataSource", dataset.Tables[0].DefaultView);
                var branchPhone = listOrderItem.Select(x => x.OrderUsableProduct.Branch.Phone).FirstOrDefault();
                var branchName = listOrderItem.Select(x => x.OrderUsableProduct.Branch.Name).FirstOrDefault();
                var alis = listOrderItem.Select(x => x.OrderUsableProduct.Branch.Alias).FirstOrDefault();
                var orderNumber = listOrderItem.Select(x => "SPLY-" + alis + "-" + x.OrderUsableProduct.Id).FirstOrDefault();
                report.Dictionary.Variables["Date"].Value = DateUtility.GetPersianDate(DateTime.Now);
                report.Dictionary.Variables["BranchPhone"].Value = branchPhone;
                report.Dictionary.Variables["BranchName"].Value = branchName;
                report.Dictionary.Variables["OrderNumber"].Value = orderNumber.ToPersianNumber();

                //report.Dictionary.Synchronize();
                report.Compile();
                report.Render(false);
                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();

                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(report, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"Report-" + DateUtility.GetPersianDate(DateTime.Now) + ".pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();

                return new FileStreamResult(stream, "application/pdf");
            }
        }
        /// <summary>
        /// تغییر وضعیت سفارش ارسال شده از دفتر مرکزی به چاپخانه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult ChangeStatusOrder(int id)
        {
            Response response;
            var user = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var listOrderItem = db.OrderUsableProductDetail.Where(x => x.OrderUsableProductId == id && x.UsableProduct.PrintingHouseId == user.PrintingHouseId).ToList();
                    var orderId = listOrderItem.Select(x => x.OrderUsableProductId).FirstOrDefault();
                    var entity = db.OrderUsableProduct.SingleOrDefault(x => x.Id == orderId);
                    entity.OrderUsableProductStatus = OrderUsableProductStatus.Sent;
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "وضعیت فاکتور مورد نظر به حالت ارسال شد تبدیل گردید."
                };
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
        public JsonResult SaveOrder(ConfirmOrderViewModel model)
        {
            Response response;
            try
            {
                var userId = GetAuthenticatedUserId();
                OrderUsableProduct order = new OrderUsableProduct();
                using (var db = new KiaGalleryContext())
                {
                    var orderUsableProductDetail = db.OrderUsableProductDetail.Where(x => model.idList.Contains(x.Id)).ToList();
                    for (var i = 0; i < orderUsableProductDetail.Count; i++)
                    {
                        //orderUsableProductDetail[i].Id = model.idList[i];
                        orderUsableProductDetail[i].PrintingHouseInventory = model.printingHouseInventory[i];
                        //orderUsableProductDetail[i].ReadyForDelivery = model.readyForDelivery[i];
                        //orderUsableProductDetail[i].Delivered = model.delivered[i];
                        //orderUsableProductDetail[i].ConfirmDelivered = model.readyForDelivery[i];
                        //orderUsableProductDetail[i].RemainFinal = model.remainFinal[i];
                        var query = orderUsableProductDetail.Where(x => x.OrderUsableProductId == model.orderId).ToList();

                        //if (orderUsableProductDetail[i].ConfirmDelivered != null)
                        //{
                        //    orderUsableProductDetail[i].ConfirmReadyForDelivery = model.readyForDelivery[i];
                        //}

                        //if (model.readyForDelivery[i] > model.printingHouseInventory[i])
                        //{
                        //    response = new Response()
                        //    {
                        //        status = 500,
                        //        message = "کاربر محترم؛تعداد آماده تحویل بیشتر از تعداد قابل ساخت در چاپخانه می باشد."
                        //    };
                        //    return Json(response, JsonRequestBehavior.AllowGet);
                        //}
                    }
                    var sum = orderUsableProductDetail.Sum(x => x.RemainFinal);
                    if (sum == 0)
                    {
                        foreach (var item in orderUsableProductDetail)
                        {
                            item.OrderUsableProduct.PrintingHouserOrder = true;
                            item.OrderUsableProduct.OrderUsableProductStatus = OrderUsableProductStatus.Sent;
                            item.OrderUsableProduct.PrintingHouseOrderStatus = PrintingHouseOrderStatus.Closed;
                            item.CreateUserId = userId;
                            item.ModifyUserId = userId;
                            item.CreateDate = DateTime.Now;
                            item.ModifyDate = DateTime.Now;
                            item.Ip = Request.UserHostAddress;
                        }
                    }
                    else
                    {
                        foreach (var item in orderUsableProductDetail)
                        {
                            item.OrderUsableProduct.PrintingHouserOrder = true;
                            item.OrderUsableProduct.OrderUsableProductStatus = OrderUsableProductStatus.InPreparation;
                            item.OrderUsableProduct.PrintingHouseOrderStatus = PrintingHouseOrderStatus.HalfOpen;
                            item.CreateUserId = userId;
                            item.ModifyUserId = userId;
                            item.CreateDate = DateTime.Now;
                            item.ModifyDate = DateTime.Now;
                            item.Ip = Request.UserHostAddress;
                        }
                    }
                    //var log = new OrderUsableProductLog()
                    //{
                    //    Id = order.Id,
                    //    OrderUsableProductId = model.orderUsableProductId,
                    //    CreateUserId = userId,
                    //    CreateDate = DateTime.Now,
                    //    Ip = Request.UserHostAddress
                    //};
                    //db.OrderUsableProductLog.Add(log);
                    var date = orderUsableProductDetail.Select(x => x.CreateDate).FirstOrDefault();
                    var orderBranchID = db.OrderUsableProduct.Where(x => x.Id == model.orderId).Select(x => x.BranchId).FirstOrDefault();
                    var persianDate = DateUtility.GetPersianDate(date);
                    var orderId = orderUsableProductDetail.Select(x => x.OrderUsableProductId).FirstOrDefault();
                    var mobileNumber = db.User.Where(x => x.BranchId == orderBranchID).FirstOrDefault().PhoneNumber;
                    var branchId = orderUsableProductDetail.Select(x => x.OrderUsableProduct.Branch).FirstOrDefault();
                    var branchMobile = orderUsableProductDetail.Select(x => x.OrderUsableProduct.Branch.Phone).First();
                    var mobileAdmin = db.User.Where(x => x.Id == 2).FirstOrDefault().PhoneNumber;
                    var alias = branchId.Alias;

                    //if (mobileNumber != null)
                    //{
                    //    if (!User.IsInRole("admin"))
                    //    {
                    //        Task.Factory.StartNew(() =>
                    //        {
                    //            NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + "سفارش شما به شماره سفارش" + "\n" + "SPLY-" + alias + "-" + orderId + "\n" + " درتاریخ " + persianDate + "تایید و آماده ارسال می باشد.", mobileNumber);
                    //        });
                    //    }
                    //    else
                    //    {
                    //        Task.Factory.StartNew(() =>
                    //        {
                    //            NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + "سفارش شما به شماره سفارش" + "\n" + "SPLY-" + alias + "-" + orderId + "\n" + " درتاریخ " + persianDate + "تایید و آماده ارسال می باشد.", mobileAdmin);
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

        public ActionResult ConfirmSending(int id)
        {
            ViewBag.OrderUsableProductId = id;
            return View();
        }

        /// <summary>
        /// ارسال سفارش محصول مصرفی به سمت چاپخانه
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ConfirmOrder(ConfirmOrderViewModel model)
        {
            Response response;
            try
            {
                var userId = GetAuthenticatedUserId();
                OrderUsableProduct order = new OrderUsableProduct();
                using (var db = new KiaGalleryContext())
                {
                    var orderUsableProductDetail = db.OrderUsableProductDetail.Where(x => model.idList.Contains(x.Id)).ToList();
                    for (var i = 0; i < orderUsableProductDetail.Count; i++)
                    {
                        orderUsableProductDetail[i].Id = model.idList[i];
                        orderUsableProductDetail[i].PrintingHouseInventory = model.printingHouseInventory[i];
                        orderUsableProductDetail[i].ReadyForDelivery = model.confirmDelivered[i];
                        orderUsableProductDetail[i].ConfirmDelivered = model.confirmDelivered[i];
                        orderUsableProductDetail[i].RemainFinal = model.remainFinal[i];
                        var query = orderUsableProductDetail.Where(x => x.OrderUsableProductId == model.orderId).ToList();
                        if (orderUsableProductDetail[i].ReadyForDelivery != null)
                        {
                            orderUsableProductDetail[i].ReadyForDelivery = model.confirmDelivered[i];
                        }

                        if (model.confirmDelivered[i] > model.readyForDelivery[i])
                        {
                            response = new Response()
                            {
                                status = 500,
                                message = "کاربر محترم؛تعداد آماده تحویل بیشتر از تعداد قابل ساخت در چاپخانه می باشد."
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                    }
                    var sum = orderUsableProductDetail.Sum(x => x.RemainFinal);
                    if (sum == 0)
                    {
                        foreach (var item in orderUsableProductDetail)
                        {
                            item.OrderUsableProduct.PrintingHouserOrder = true;
                            item.OrderUsableProduct.OrderUsableProductStatus = OrderUsableProductStatus.Sent;
                            item.OrderUsableProduct.PrintingHouseOrderStatus = PrintingHouseOrderStatus.Closed;
                            item.CreateUserId = userId;
                            item.ModifyUserId = userId;
                            item.CreateDate = DateTime.Now;
                            item.ModifyDate = DateTime.Now;
                            item.Ip = Request.UserHostAddress;
                        }
                    }
                    else
                    {
                        foreach (var item in orderUsableProductDetail)
                        {
                            item.OrderUsableProduct.PrintingHouserOrder = true;
                            item.OrderUsableProduct.OrderUsableProductStatus = OrderUsableProductStatus.InPreparation;
                            item.OrderUsableProduct.PrintingHouseOrderStatus = PrintingHouseOrderStatus.HalfOpen;
                            item.CreateUserId = userId;
                            item.ModifyUserId = userId;
                            item.CreateDate = DateTime.Now;
                            item.ModifyDate = DateTime.Now;
                            item.Ip = Request.UserHostAddress;
                        }
                    }
                    var date = orderUsableProductDetail.Select(x => x.CreateDate).FirstOrDefault();
                    var orderCreateUserId = orderUsableProductDetail.Select(x => x.CreateUserId).FirstOrDefault();
                    var persianDate = DateUtility.GetPersianDate(date);
                    var orderId = orderUsableProductDetail.Select(x => x.OrderUsableProductId).FirstOrDefault();
                    var mobileNumber = db.User.Where(x => x.Id == orderCreateUserId).FirstOrDefault().PhoneNumber;
                    var branchId = orderUsableProductDetail.Select(x => x.OrderUsableProduct.Branch).FirstOrDefault();
                    var alias = branchId.Alias;

                    //if (!User.IsInRole("admin"))
                    //{
                    //    Task.Factory.StartNew(() =>
                    //    {
                    //        NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + "سفارش شما به شماره سفارش" + "\n" + "SPLY-" + alias + "-" + orderId + "\n" + " درتاریخ " + persianDate + "تایید و در حال بررسی می باشد.", mobileNumber);
                    //    });
                    //}
                    //else
                    //{
                    //    Task.Factory.StartNew(() =>
                    //    {
                    //        NikSmsWebServiceClient.SendSmsNik("همکار گرامی؛" + "\n" + "سفارش شما به شماره سفارش" + "\n" + "SPLY-" + alias + "-" + orderId + "\n" + " درتاریخ " + persianDate + "تایید و در حال بررسی می باشد.", "09193121247");
                    //    });
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
        /// برگرداندن لیست جزئیات سفارشات ارسال شده از سمت دفترمرکزی برای نمایش در کاربر چاپخانه
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetListConfrimOrder(ShowOrderUsableProductViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    var query = db.OrderUsableProductDetail.Where(x => x.OrderUsableProductId == model.orderUsableProductId && x.UsableProduct.PrintingHouseId == user.PrintingHouseId);
                    var list = query.Select(item => new ShowOrderUsableProductViewModel
                    {
                        id = item.Id,
                        name = item.UsableProduct.Name,
                        orderUsableProductId = item.OrderUsableProductId,
                        productCount = item.Count,
                        description = item.Description,
                        unit = item.UsableProduct.Unit,
                        code = item.UsableProduct.Code,
                        image = item.UsableProduct.UsableProductFileList.Select(y => y.FileName).FirstOrDefault(),
                        category = item.UsableProduct.CategoryUsableProduct.Title,
                        createUser = item.CreateUser.FirstName + " " + item.CreateUser.LastName,
                        createBranch = item.OrderUsableProduct.Branch.Name,
                        createDate = item.CreateDate,
                        officeInventory = item.OfficeInventory.Value,
                        remain = item.Remain.Value,
                        confirmDelivered = item.ConfirmDelivered,
                        printingHouseInventory = item.PrintingHouseInventory,
                        remainFinal = item.RemainFinal,
                        readyForDelivery = item.ReadyForDelivery,
                        branchType = item.OrderUsableProduct.Branch.BranchType,

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
        /// چاپ برگه فاکتور سفارش به صورت html
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PrintOrderPage(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                var user = GetAuthenticatedUser();
                var entity = db.OrderUsableProductDetail.Where(x => x.OrderUsableProductId == id && x.UsableProduct.PrintingHouseId == user.PrintingHouseId);
                var list = entity.Select(item => new ShowOrderUsableProductViewModel
                {
                    name = item.UsableProduct.Name,
                    code = item.UsableProduct.Code,
                    unit = item.UsableProduct.Unit,
                    category = item.UsableProduct.CategoryUsableProduct.Title,
                    createDate = item.CreateDate,
                    productCount = item.Count,
                    officeInventory = item.OfficeInventory.Value,
                    remain = item.Remain.Value,
                    delivered = item.Delivered,
                    printingHouseInventory = item.PrintingHouseInventory,
                    remainFinal = item.RemainFinal,
                    readyForDelivery = item.ReadyForDelivery,
                    specification = item.Specification,

                }).ToList();
                list.ForEach(x =>
                {
                    x.orderUsableProductStatusTitle = Enums.GetTitle(x.orderUsableProductStatus);
                    x.persianDate = DateUtility.GetPersianDate(x.createDate);
                });
                ViewBag.OrderUsableProductDetailList = list;
                ViewBag.Branch = entity.Select(x => x.OrderUsableProduct.Branch.Name).FirstOrDefault();
                ViewBag.BranchPhone = entity.Select(x => x.OrderUsableProduct.Branch.Phone).FirstOrDefault();
                ViewBag.Order = entity.Select(x => x.OrderUsableProduct.Branch.Alias + "-" + x.OrderUsableProduct.Id).FirstOrDefault();
                ViewBag.Address = entity.Select(x => x.OrderUsableProduct.Branch.Address).FirstOrDefault();
                ViewBag.PrintingHouseOrderTitle = entity.Select(x => x.UsableProduct.PrintingHouse.Name).FirstOrDefault();
                var date = entity.Select(x => x.CreateDate).FirstOrDefault();
                var finaldate = DateUtility.GetPersianDate(date);
                ViewBag.Date = finaldate;
                ViewBag.NowTime = DateUtility.GetPersianDate(DateTime.Now);
            }
            return View();
        }
    }
}