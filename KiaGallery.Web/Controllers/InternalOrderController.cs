using br.com.arcnet.barcodegenerator;
using DocumentFormat.OpenXml.Bibliography;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.InternalOrder;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using RestSharp;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KiaGallery.Web.Controllers
{
    public class InternalOrderController : BaseController
    {
        /// <summary>
        /// ارسال پیامک
        /// </summary>
        /// <param name="text"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private string Sms(string text, string number)
        {
            string data = "username=" + "kiagallery" + "&password=" + "880866252" + "&to=" + number + "&from=" + "50004000040005" + "&text=" + text + "&isflash=false";

            var client = new RestClient("http://rest.payamak-panel.com/api/SendSMS/SendSMS");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/x-www-form-urlencoded", data, ParameterType.RequestBody);

            IRestResponse responsed = client.Execute(request);
            return null;
        }

        /// <summary>
        /// آپلود عکس با وب کم
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="base64"></param>
        /// <returns></returns>
        public JsonResult UploadCameraImage(string path, string fileName, string base64)
        {
            Response response;
            try
            {
                string serverPath = Server.MapPath("~/Upload/internalOrderDetail" + path);

                fileName = Imagefile(base64, fileName);
                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        name = fileName,
                        length = fileName.Length,
                    },
                    message = "بارگذاری فایل با موفقیت انجام شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        private string Imagefile(string base64String, string fileName)
        {
            if (base64String != null)
            {
                base64String = base64String.Replace("data:image/png;base64,", "");

            }
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            string serverPath = Server.MapPath("~/Upload/internalOrderDetail");

            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Upload/internalOrderDetail/"));
            }
            string savedFileName = Path.Combine(serverPath, fileName);
            if (System.IO.File.Exists(savedFileName))
            {
                Random random = new Random();
                string prefix = random.Next(1000, 9999).ToString() + "-";
                fileName = prefix + fileName;
                savedFileName = Path.Combine(serverPath, fileName);
            }
            image.Save(savedFileName);
            return fileName;
        }

        /// <summary>
        /// آپلود کردن تصویر
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadFile(int id)
        {
            Response response;
            try
            {
                string fileName = string.Empty;
                string serverPath = Server.MapPath("~/Upload/internalOrderDetail");
                HttpPostedFileBase hpf = Request.Files[0];
                string originalFileName = Path.GetFileName(hpf.FileName);

                string fileExtension = Path.GetExtension(originalFileName);
                using (var db = new KiaGalleryContext())
                {
                    var internalOrderDetail = db.InternalOrderDetail.Single(x => x.Id == id);
                    fileName = /*internalOrderDetail. + "_" + product.BookCode + "_" + type +*/ Path.GetExtension(originalFileName);
                }

                if (hpf.ContentLength == 0)
                    throw new Exception("File length can't be equal to zero");


                string savedFileName = Path.Combine(serverPath, fileName);

                if (System.IO.File.Exists(savedFileName))
                {
                    Random random = new Random();
                    string prefix = random.Next(1000, 9999).ToString();
                    fileName = "(" + prefix + ")" + fileName;
                }
                savedFileName = Path.Combine(serverPath, fileName);

                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }

                hpf.SaveAs(savedFileName);

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        name = fileName,
                        length = hpf.ContentLength,
                        type = hpf.ContentType
                    },
                    message = "بارگذاری فایل با موفقیت انجام شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        // GET: InternalOrder
        //[Authorize(Roles = "admin ,internalOrderBranch ,internalOrderOffice")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name,
                }).ToList();
            }
            ViewBag.branchId = GetAuthenticatedUser().BranchId;
            return View();
        }
        [Authorize(Roles = "admin ,internalOrderBranch ,internalOrderOffice")]
        public ActionResult Process()
        {
            return View();
        }

        //[Authorize(Roles = "admin ,internalOrderBranch ,internalOrderOffice")]
        //public JsonResult ProcessingJson()
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var query = db.InternalOrder.Where(x => x.InternalOrderStatus == InternalOrderStatus.AcceptFromBranch || x.InternalOrderStatus == InternalOrderStatus.SendToOffice || x.InternalOrderStatus == InternalOrderStatus.SendToBranch);
        //            var list = query.Select(item => new
        //            {
        //                id = item.Id,
        //                date = item.Date,
        //                barcodeDate = item.BarcodeDate,
        //                name = item.Name,
        //                phoneNumber = item.PhoneNumber,
        //                deposit = item.Deposit,
        //                barcode = item.Barcode,
        //                status = item.InternalOrderStatus,
        //                noAnswerCount = item.InternalOrderStatusLogList.Count(x => x.InternalOrderStatus == InternalOrderStatus.NoAnswer),
        //                deliverType = item.DeliveryType,
        //                ponyUp = item.PonyUp,
        //                user = item.CreatePerson.ShortName,
        //                userColor = item.CreateUser.Color,
        //                multiOrder = item.MultiOrder,
        //                branchName = item.DeliveredBranch.Name,
        //                orderedBranchName = item.Branch.Name,
        //                trackCode = item.TrackCode,
        //                //fileName= item.FileName,
        //                //log = item.InternalOrderLogList.Select(x => new
        //                //{
        //                //    user = x.User.FirstName,
        //                //    text = x.Text,
        //                //    createdDate = x.CreatedDate
        //                //}).ToList()

        //            }).ToList();
        //            if (list != null)
        //            {
        //                response = new Response()
        //                {
        //                    status = 200,
        //                    data = new
        //                    {
        //                        list = list.Select(item => new
        //                        {
        //                            id = item.id,
        //                            date = DateUtility.GetPersianDate(item.date),
        //                            barcodeDate = item.barcodeDate,
        //                            name = item.name,
        //                            phoneNumber = item.phoneNumber,
        //                            deposit = item.deposit,
        //                            barcode = item.barcode,
        //                            status = Enums.GetTitle(item.status),
        //                            noAnswerCount = item.noAnswerCount,
        //                            deliverType = item.deliverType,
        //                            ponyUp = item.ponyUp,
        //                            user = item.user,
        //                            //userColor = item.createUser.Color,
        //                            multiOrder = item.multiOrder,
        //                            branchName = item.branchName,
        //                            orderedBranchName = item.orderedBranchName,
        //                            trackCode = item.trackCode,


        //                        }).ToList()
        //                    }


        //                };
        //            }
        //            else
        //            {
        //                response = new Response()
        //                {
        //                    status = 500,
        //                    message = "موردی یافت نشد"
        //                };
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        [Authorize(Roles = "admin ,internalOrderBranch")]
        public ActionResult SendToOffice()
        {

            return View();
        }
        [Authorize(Roles = "admin ,internalOrderOffice")]
        public ActionResult AcceptFromBranch()
        {


            return View();
        }
        [Authorize(Roles = "admin ,internalOrderOfficeBarcodeRoom")]
        public ActionResult SendToBranch()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
            }

            return View();
        }
        [Authorize(Roles = "admin ,internalOrderBranch")]
        public ActionResult AcceptFromOffice()
        {


            return View();
        }


        //[HttpGet]
        //[Authorize(Roles = "admin ,internalOrderBranch")]
        //public JsonResult SendToOfficeJson(SendToOfficeViewModel model)
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var branchId = GetAuthenticatedUser().BranchId;
        //            var query = db.InternalOrder.Where(x => x.InternalOrderStatus == InternalOrderStatus.Registered && x.BranchId == branchId);


        //            var list = query.Select(item => new
        //            {

        //                id = item.Id,
        //                date = item.Date,
        //                name = item.Name,
        //                phoneNumber = item.PhoneNumber,
        //                deposit = item.Deposit,
        //                barcode = item.Barcode,
        //                status = item.InternalOrderStatus,
        //                noAnswerCount = item.InternalOrderStatusLogList.Count(x => x.InternalOrderStatus == InternalOrderStatus.NoAnswer),
        //                deliverType = item.DeliveryType,
        //                ponyUp = item.PonyUp,
        //                user = item.CreateUser.FirstName,
        //                userColor = item.CreateUser.Color,
        //                multiOrder = item.MultiOrder,
        //                branchName = item.DeliveredBranch.Name,
        //                orderedBranchName = item.Branch.Name,
        //                trackCode = item.TrackCode,
        //                detail = item.InternalOrderDetailList.Select(y => new
        //                {
        //                    title = y.Title,
        //                    fileName = y.Product.ProductFileList.FirstOrDefault(z => z.FileType == FileType.Order).FileName,

        //                }).ToList()

        //            }).ToList();

        //            if (list != null)
        //            {

        //                response = new Response()
        //                {
        //                    status = 200,
        //                    data = new
        //                    {
        //                        list = list.Select(item => new
        //                        {
        //                            id = item.id,
        //                            date = DateUtility.GetPersianDate(item.date),
        //                            barcodeDate = item.barcodeDate,
        //                            name = item.name,
        //                            phoneNumber = item.phoneNumber,
        //                            deposit = item.deposit,
        //                            barcode = item.barcode,
        //                            status = Enums.GetTitle(item.status),
        //                            noAnswerCount = item.noAnswerCount,
        //                            deliverType = item.deliverType,
        //                            ponyUp = item.ponyUp,
        //                            user = item.user,
        //                            //userColor = item.createUser.Color,
        //                            multiOrder = item.multiOrder,
        //                            branchName = item.branchName,
        //                            orderedBranchName = item.orderedBranchName,
        //                            trackCode = item.trackCode,
        //                            detail = item.detail.Select(y => new
        //                            {
        //                                title = y.title,

        //                            }).ToList()
        //                        }).ToList()
        //                    }
        //                };
        //            }
        //            else
        //            {
        //                response = new Response()
        //                {
        //                    status = 500,
        //                    message = "موردی یافت نشد"
        //                };
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //[Authorize(Roles = "admin ,internalOrderOffice")]
        //public JsonResult AcceptFromBranchJson(AcceptFromBranchViewModel model)
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {


        //            var query = db.InternalOrder.Where(x => x.InternalOrderStatus == InternalOrderStatus.SendToOffice);
        //            if (model.branchId != null && model.branchId > 0)
        //            {
        //                query = query.Where(x => x.BranchId == model.branchId);
        //            }

        //            var list = query.Select(item => new
        //            {

        //                id = item.Id,
        //                date = item.Date,
        //                barcodeDate = item.BarcodeDate,
        //                name = item.Name,
        //                phoneNumber = item.PhoneNumber,
        //                deposit = item.Deposit,
        //                barcode = item.Barcode,
        //                status = item.InternalOrderStatus,
        //                noAnswerCount = item.InternalOrderStatusLogList.Count(x => x.InternalOrderStatus == InternalOrderStatus.NoAnswer),
        //                deliverType = item.DeliveryType,
        //                ponyUp = item.PonyUp,
        //                user = item.CreateUser.FirstName,
        //                userColor = item.CreateUser.Color,
        //                multiOrder = item.MultiOrder,
        //                orderedBranchName = item.Branch.Name,
        //                trackCode = item.TrackCode,
        //                detail = item.InternalOrderDetailList.Select(y => new
        //                {
        //                    title = y.Title,
        //                    fileName = y.Product.ProductFileList.FirstOrDefault(z => z.FileType == FileType.Order).FileName,

        //                }).ToList()

        //            }).ToList();

        //            if (list != null)
        //            {

        //                response = new Response()
        //                {
        //                    status = 200,
        //                    data = new
        //                    {
        //                        list = list.Select(item => new
        //                        {
        //                            id = item.id,
        //                            date = DateUtility.GetPersianDate(item.date),
        //                            barcodeDate = item.barcodeDate,
        //                            name = item.name,
        //                            phoneNumber = item.phoneNumber,
        //                            deposit = item.deposit,
        //                            barcode = item.barcode,
        //                            status = Enums.GetTitle(item.status),
        //                            noAnswerCount = item.noAnswerCount,
        //                            deliverType = item.deliverType,
        //                            ponyUp = item.ponyUp,
        //                            user = item.user,
        //                            //userColor = item.createUser.Color,
        //                            multiOrder = item.multiOrder,
        //                            orderedBranchName = item.orderedBranchName,
        //                            trackCode = item.trackCode,
        //                            detail = item.detail.Select(y => new
        //                            {
        //                                title = y.title,

        //                            }).ToList()

        //                        }).ToList()
        //                    }
        //                };
        //            }
        //            else
        //            {
        //                response = new Response()
        //                {
        //                    status = 500,
        //                    message = "موردی یافت نشد"
        //                };
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //[Authorize(Roles = "admin ,internalOrderOfficeBarcodeRoom")]
        //public JsonResult SendToBranchJson(SendToBranchViewModel model)
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var query = db.InternalOrder.Where(x => x.InternalOrderStatus == InternalOrderStatus.AcceptFromBranch);


        //            var list = query.Select(item => new
        //            {

        //                id = item.Id,
        //                date = item.Date,
        //                barcodeDate = item.BarcodeDate,
        //                name = item.Name,
        //                phoneNumber = item.PhoneNumber,
        //                deposit = item.Deposit,
        //                barcode = item.Barcode,
        //                status = item.InternalOrderStatus,
        //                noAnswerCount = item.InternalOrderStatusLogList.Count(x => x.InternalOrderStatus == InternalOrderStatus.NoAnswer),
        //                deliverType = item.DeliveryType,
        //                ponyUp = item.PonyUp,
        //                user = item.CreateUser.FirstName,
        //                userColor = item.CreateUser.Color,
        //                multiOrder = item.MultiOrder,
        //                branchName = item.DeliveredBranch.Name,
        //                orderedBranchName = item.Branch.Name,
        //                trackCode = item.TrackCode,
        //                detail = item.InternalOrderDetailList.Select(y => new
        //                {
        //                    title = y.Title,
        //                    fileName = y.Product.ProductFileList.FirstOrDefault(z => z.FileType == FileType.Order).FileName,

        //                }).ToList()

        //            }).ToList();

        //            if (list != null)
        //            {

        //                response = new Response()
        //                {
        //                    status = 200,
        //                    data = new
        //                    {
        //                        list = list.Select(item => new
        //                        {
        //                            id = item.id,
        //                            date = DateUtility.GetPersianDate(item.date),
        //                            barcodeDate = item.barcodeDate,
        //                            name = item.name,
        //                            phoneNumber = item.phoneNumber,
        //                            deposit = item.deposit,
        //                            barcode = item.barcode,
        //                            status = Enums.GetTitle(item.status),
        //                            noAnswerCount = item.noAnswerCount,
        //                            deliverType = item.deliverType,
        //                            ponyUp = item.ponyUp,
        //                            user = item.user,
        //                            //userColor = item.createUser.Color,
        //                            multiOrder = item.multiOrder,
        //                            branchName = item.branchName,
        //                            orderedBranchName = item.orderedBranchName,
        //                            trackCode = item.trackCode,
        //                            detail = item.detail.Select(y => new
        //                            {
        //                                title = y.title,

        //                            }).ToList()


        //                        }).ToList()
        //                    }
        //                };
        //            }
        //            else
        //            {
        //                response = new Response()
        //                {
        //                    status = 500,
        //                    message = "موردی یافت نشد"
        //                };
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}


        //[HttpGet]
        //[Authorize(Roles = "admin ,internalOrderBranch")]
        //public JsonResult AcceptFromOfficeJson(string term)
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var branchId = GetAuthenticatedUser().BranchId;
        //            var query = db.InternalOrder.Where(x => x.TrackCode == term && x.InternalOrderStatus == InternalOrderStatus.SendToBranch && x.DeliveredBranchId == branchId);

        //            var data = query.Select(item => new
        //            {
        //                branchName = item.Branch.Name,
        //                name = item.Name,
        //                trackCode = item.TrackCode,
        //                id = item.Id,
        //                phoneNumber = item.PhoneNumber,
        //                detail = item.InternalOrderDetailList.Select(y => new
        //                {
        //                    title = y.Title,
        //                    fileName = y.Product.ProductFileList.FirstOrDefault(z => z.FileType == FileType.Order).FileName,

        //                }).ToList()

        //            }).SingleOrDefault();

        //            if (data != null)
        //            {

        //                response = new Response()
        //                {
        //                    status = 200,
        //                    data = data
        //                };
        //            }
        //            else
        //            {
        //                response = new Response()
        //                {
        //                    status = 500,
        //                    message = "موردی یافت نشد یا سفارش متعلق به شعبه دیگری می باشد."
        //                };
        //            }


        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);

        //    }
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetLogHistory(string term)
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {

        //            var query = db.InternalOrder.Select(x => x);
        //            if (!string.IsNullOrEmpty(term?.Trim()))
        //            {
        //                query = query.Where(x => x.PhoneNumber.Contains(term.Trim()) || x.TrackCode.Contains(term.Trim()));
        //            }
        //            var data = query.Select(item => new
        //            {
        //                branchName = item.Branch.Name,
        //                deliveredBranchName = item.DeliveredBranch.Name,
        //                customerName = item.Name,
        //                phoneNumber = item.PhoneNumber,
        //                status = item.InternalOrderStatus,
        //                trackCode = item.TrackCode,
        //                date = item.Date

        //            }).ToList();
        //            response = new Response()
        //            {
        //                status = 200,
        //                data = new
        //                {
        //                    list = data.Select(item => new
        //                    {
        //                        branchName = item.branchName,
        //                        deliveredBranchName = item.deliveredBranchName,
        //                        customerName = item.customerName,
        //                        phoneNumber = item.phoneNumber,
        //                        status = Enums.GetTitle(item.status),
        //                        trackCode = item.trackCode,
        //                        date = DateUtility.GetPersianDate(item.date)

        //                    }).ToList()

        //                }
        //            };
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);

        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);

        //}

        /// <summary>
        /// پرینت برگه سفارشات
        /// </summary>
        /// <param name="id">ردیف گزینه مورد نظر</param>
        /// <returns>فایل پی دی اف</returns>
        [Authorize(Roles = "admin ,internalOrderBranch")]
        public ActionResult OrderSheet(int id)
        {
            List<InternalDetailViewModel> list;
            using (var db = new KiaGalleryContext())
            {
                list = db.InternalOrderDetail.Where(x => x.InternalOrderId == id).Select(x => new InternalDetailViewModel()
                {
                    id = x.InternalOrder.Id,
                    cutomerName = x.InternalOrder.Name,
                    phoneNumber = x.InternalOrder.PhoneNumber,
                    telephone = x.InternalOrder.Telephone,
                    count = x.Count,
                    date = x.InternalOrder.Date,
                    deposit = x.InternalOrder.Deposit,
                    orderType = x.OrderType,
                    productType = x.ProductType,
                    newProductType = x.NewProductType,
                    goldType = x.GoldType,
                    prdocutTitle = x.Product.Title,
                    newProductTitle = x.NewProductTitle,
                    productCode = x.Product.Code,
                    productColor = x.ProductColor,
                    person = x.InternalOrder.CreatePerson.FirstName + " " + x.InternalOrder.CreatePerson.LastName,
                    siteCode = x.SiteCode,
                    description = x.Description,
                    trackCode = x.TrackCode,
                    bookCode = x.Product.BookCode,
                    barcodeImage = "/upload/orderSheet/" + x.TrackCode + ".jpg",
                    barcode = x.Barcode,
                    size = x.Size,
                    orderTypeForm = x.InternalOrder.OrderTypeForm,
                    newStone = x.NewStone,
                    newLeather = x.NewLeather,
                    newLeatherLoop = x.NewLeatherLoop,
                    newSize = x.NewSize,
                    newProductColor = x.NewProductColor,
                    newGoldType = x.NewGoldType,
                    newDescription = x.NewDescription,
                    newCount = x.NewCount,
                    goldOwnership = x.GoldOwnership,
                    image = x.FileName,
                    //fileName = x.Product.ProductFileList.Where(y => y.FileType == FileType.Order ? y.FileType == FileType.Order : y.FileType == FileType.WhiteBack).FirstOrDefault().FileName,
                    fileName = x.Product.ProductFileList.FirstOrDefault(y => y.FileType == FileType.Order).FileName,
                    leatherLoop = x.LeatherLoop,
                    stoneList = x.InternalOrderDetailStonesList.Select(c => c.Stone.Name).ToList(),
                    leatherList = x.InternalOrderDetailLeathersList.Select(d => d.Leather.Name).ToList(),
                }).ToList();

                list.ForEach(x =>
                {
                    x.persianDate = DateUtility.GetPersianDate(x.date);
                    x.depositSeparator = Core.ToSeparator(x.deposit);
                    x.prdocutTypeTitle = Enums.GetTitle(x.productType);
                    x.newProductTypeTitle = Enums.GetTitle(x.newProductType.GetValueOrDefault());
                    x.orderTypeFormTitle = Enums.GetTitle(x.orderTypeForm);
                    x.prdocutColorTitle = Enums.GetTitle(x.productColor);
                });
            }
            return View(list);
        }

        /// <summary>
        /// پرینت برگه سفارشات
        /// </summary>
        /// <param name="id">ردیف گزینه مورد نظر</param>
        /// <returns>فایل پی دی اف</returns>
        [Authorize(Roles = "admin ,internalOrderBranch")]
        public ActionResult View(int id)
        {
            List<InternalDetailViewModel> list;
            using (var db = new KiaGalleryContext())
            {
                list = db.InternalOrderDetail.Where(x => x.InternalOrderId == id).Select(x => new InternalDetailViewModel()
                {
                    id = x.InternalOrder.Id,
                    cutomerName = x.InternalOrder.Name,
                    goldOwnership = x.GoldOwnership,
                    image = x.FileName,
                    count = x.Count,
                    phoneNumber = x.InternalOrder.PhoneNumber,
                    telephone = x.InternalOrder.Telephone,
                    date = x.InternalOrder.Date,
                    deposit = x.InternalOrder.Deposit,
                    orderType = x.OrderType,
                    productType = x.ProductType,
                    newProductType = x.NewProductType,
                    goldType = x.GoldType,
                    prdocutTitle = x.Product.Title,
                    newProductTitle = x.NewProductTitle,
                    productCode = x.Product.Code,
                    productColor = x.ProductColor,
                    person = x.InternalOrder.CreatePerson.FirstName + " " + x.InternalOrder.CreatePerson.LastName,
                    siteCode = x.SiteCode,
                    description = x.Description,
                    trackCode = x.TrackCode,
                    bookCode = x.Product.BookCode,
                    barcodeImage = "/upload/orderSheet/" + x.Barcode + ".jpg",
                    barcode = x.Barcode,
                    size = x.Size,
                    orderTypeForm = x.InternalOrder.OrderTypeForm,
                    newStone = x.NewStone,
                    newLeather = x.NewLeather,
                    newLeatherLoop = x.NewLeatherLoop,
                    newSize = x.NewSize,
                    newProductColor = x.NewProductColor,
                    newGoldType = x.NewGoldType,
                    newDescription = x.NewDescription,
                    newCount = x.NewCount,
                    //fileName = x.Product.ProductFileList.Where(y => y.FileType == FileType.Order ? y.FileType == FileType.Order : y.FileType == FileType.WhiteBack).FirstOrDefault().FileName,
                    fileName = x.Product.ProductFileList.FirstOrDefault(y => y.FileType == FileType.Order).FileName,
                    leatherLoop = x.LeatherLoop,
                    stoneList = x.InternalOrderDetailStonesList.Select(c => c.Stone.Name).ToList(),
                    leatherList = x.InternalOrderDetailLeathersList.Select(d => d.Leather.Name).ToList(),
                    branchColor = x.InternalOrder.Branch.Color,
                }).ToList();

                list.ForEach(x =>
                {
                    x.persianDate = DateUtility.GetPersianDate(x.date);
                    x.depositSeparator = Core.ToSeparator(x.deposit);
                    x.prdocutTypeTitle = Enums.GetTitle(x.productType);
                    x.newProductTypeTitle = Enums.GetTitle(x.newProductType.GetValueOrDefault());
                    x.orderTypeFormTitle = Enums.GetTitle(x.orderTypeForm);
                    x.prdocutColorTitle = Enums.GetTitle(x.productColor);
                });
            }
            return View(list);
        }

        /// <summary>
        /// افزودن سفارش
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Add(GetDataRecordViewModel model)
        {
            var user = GetAuthenticatedUser();
            //var branchId = GetAuthenticatedUser().BranchId;
            using (var db = new KiaGalleryContext())
            {
                //ViewBag.UserList = db.Person.Where(x => x.Active == true && x.BranchId == branchId).ToList();
                ViewBag.SizeValueList = db.SizeValue.Select(x => x).ToList();
                ViewBag.LeatherList = db.Leather.Select(x => x).ToList();
                ViewBag.StoneList = db.Stone.Select(x => x).ToList();
                ViewBag.Data = model;
                ViewBag.createPersonId = db.Person.Where(x => x.Active == true && x.BranchId == user.BranchId).Select(x => new PersonView
                {
                    id = x.Id,
                    firstName = x.FirstName,
                    lastName = x.LastName,
                }).ToList();
            }
            return View("Edit");
        }

        /// <summary>
        /// ویرایش سفارش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Edit(int id)
        {
            var user = GetAuthenticatedUser();
            //var branchId = GetAuthenticatedUser().BranchId;
            using (var db = new KiaGalleryContext())
            {
                //ViewBag.UserList = db.Person.Where(x => x.Active == true && x.BranchId == branchId).ToList();
                ViewBag.SizeValueList = db.SizeValue.Select(x => x).ToList();
                ViewBag.LeatherList = db.Leather.Select(x => x).ToList();
                ViewBag.StoneList = db.Stone.Select(x => x).ToList();
                ViewBag.createPersonId = db.Person.Where(x => x.Active == true && x.BranchId == user.BranchId).Select(x => new PersonView
                {
                    id = x.Id,
                    firstName = x.FirstName,
                    lastName = x.LastName,
                }).ToList();

                ViewBag.Data = new GetDataRecordViewModel
                {
                };
            }

            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// عملیات ذخیره کردن سفارش مشتری
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult Save(InternalOrderViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    if (model.addProductViewModelList == null || model.addProductViewModelList.Count() == 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "کاربر محترم؛وجود یک محصول جهت ثبت سفارش الزامی است."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.InternalOrder.Where(x => x.Id == model.id).SingleOrDefault();
                        entity.Name = model.name;
                        entity.Count = model.count;
                        entity.BranchId = user.BranchId;
                        entity.PhoneNumber = model.phoneNumber;
                        entity.Telephone = model.telephone;
                        entity.OrderTypeForm = model.orderTypeForm;
                        entity.Date = DateUtility.GetDateTime(model.date);
                        entity.Deposit = model.deposit;
                        entity.DetailCount = model.detailCount;
                        entity.UserType = model.userType;
                        entity.Gender = model.gender;
                        entity.CreatePersonId = model.createPersonId;
                        entity.ModifyUserId = user.Id;
                        entity.ModifyDate = DateTime.Now;

                        foreach (var item in model.addProductViewModelList)
                        {
                            if (item.detailId != null && item.detailId > 0)
                            {
                                var detail = entity.InternalOrderDetailList.Where(y => y.Id == item.detailId).SingleOrDefault();
                                detail.BarcodeDate = DateUtility.GetDateTime(item.barcodeDate);
                                //detail.TrackCode = item.trackCode;
                                //detail.Barcode = detail.TrackCode;
                                detail.ProductId = item.productId;
                                detail.GoldOwnership = item.goldOwnership;
                                detail.SiteCode = item.siteCode;
                                detail.BookCode = item.bookCode;
                                detail.LeatherLoop = item.leatherLoop;
                                detail.GoldType = item.goldType;
                                detail.ProductType = item.productType;
                                detail.NewProductType = item.newProductType;
                                //detail.OrderType = item.orderType;
                                detail.Size = item.size;
                                detail.Count = item.count;
                                detail.Title = item.productTitle;
                                detail.NewProductTitle = item.title;
                                detail.FileName = item.fileName;
                                detail.Description = item.description;
                                detail.NewStone = item.newStone;
                                detail.NewLeather = item.newLeather;
                                detail.NewLeatherLoop = item.newLeatherLoop;
                                detail.NewSize = item.newSize;
                                detail.NewProductColor = item.newProductColor;
                                detail.NewGoldType = item.newGoldType;
                                detail.NewDescription = item.newDescription;
                                detail.NewCount = item.newCount;
                                if (item.internalOrderDetailStoneViewModelList != null)
                                {
                                    item.internalOrderDetailStoneViewModelList.ForEach(x =>
                                    {
                                        var editDetail = detail.InternalOrderDetailStonesList.Where(y => y.Order == x.order).SingleOrDefault();
                                        editDetail.StoneId = x.stoneId;
                                    });
                                }
                                if (item.internalOrderDetailLeatherViewModelList != null)
                                {
                                    item.internalOrderDetailLeatherViewModelList.ForEach(x =>
                                    {
                                        var editDetail = detail.InternalOrderDetailLeathersList.Where(y => y.Order == x.order).SingleOrDefault();
                                        editDetail.LeatherId = x.leatherId;
                                    });
                                }
                            }
                            else
                            {
                                if (entity.OrderTypeForm == 0)
                                {
                                    var newDetail = new InternalOrderDetail()
                                    {
                                        InternalOrder = entity,
                                        BarcodeDate = DateUtility.GetDateTime(item.barcodeDate),
                                        Barcode = item.barcode,
                                        TrackCode = item.trackCode,
                                        ProductId = item.productId,
                                        GoldOwnership = item.goldOwnership,
                                        SiteCode = item.siteCode,
                                        BookCode = item.bookCode,
                                        LeatherLoop = item.leatherLoop,
                                        GoldType = item.goldType,
                                        ProductColor = item.productColor,
                                        ProductType = item.productType,
                                        FileName = item.fileName,
                                        OrderType = item.orderType,
                                        Size = item.size,
                                        NewProductTitle = item.productTitle,
                                        Title = item.productTitle,
                                        Count = item.count,
                                        Description = item.description,
                                        NewStone = item.newStone,
                                        NewLeather = item.newLeather,
                                        NewLeatherLoop = item.newLeatherLoop,
                                        NewSize = item.newSize,
                                        NewProductColor = item.newProductColor,
                                        NewGoldType = item.newGoldType,
                                        NewDescription = item.newDescription,
                                        NewCount = item.newCount,
                                    };
                                    newDetail.InternalOrderDetailStonesList = item.internalOrderDetailStoneViewModelList.Select(y => new InternalOrderDetailStone
                                    {
                                        InternalOrderDetail = newDetail,
                                        Order = y.order,
                                        StoneId = y.stoneId

                                    }).ToList();
                                    newDetail.InternalOrderDetailLeathersList = item.internalOrderDetailLeatherViewModelList.Select(y => new InternalOrderDetailLeather
                                    {
                                        InternalOrderDetail = newDetail,
                                        Order = y.order,
                                        LeatherId = y.leatherId
                                    }).ToList();
                                    db.InternalOrderDetail.Add(newDetail);
                                }
                                else
                                {
                                    var newDetail = new InternalOrderDetail()
                                    {
                                        InternalOrder = entity,
                                        BarcodeDate = DateUtility.GetDateTime(item.barcodeDate),
                                        Barcode = item.barcode,
                                        TrackCode = item.trackCode,
                                        ProductId = item.productId,
                                        GoldOwnership = item.goldOwnership,
                                        SiteCode = item.siteCode,
                                        BookCode = item.bookCode,
                                        LeatherLoop = item.leatherLoop,
                                        GoldType = item.goldType,
                                        ProductColor = item.productColor,
                                        NewProductType = item.newProductType,
                                        Count = item.count,
                                        FileName = item.fileName,
                                        OrderType = item.orderType,
                                        Size = item.size,
                                        NewProductTitle = item.productTitle,
                                        Title = item.productTitle,
                                        Description = item.description,
                                        NewStone = item.newStone,
                                        NewLeather = item.newLeather,
                                        NewLeatherLoop = item.newLeatherLoop,
                                        NewSize = item.newSize,
                                        NewProductColor = item.newProductColor,
                                        NewGoldType = item.newGoldType,
                                        NewDescription = item.newDescription,
                                        NewCount = item.newCount,
                                    };
                                    newDetail.InternalOrderDetailStonesList = item.internalOrderDetailStoneViewModelList.Select(y => new InternalOrderDetailStone
                                    {
                                        InternalOrderDetail = newDetail,
                                        Order = y.order,
                                        StoneId = y.stoneId

                                    }).ToList();
                                    newDetail.InternalOrderDetailLeathersList = item.internalOrderDetailLeatherViewModelList.Select(y => new InternalOrderDetailLeather
                                    {
                                        InternalOrderDetail = newDetail,
                                        Order = y.order,
                                        LeatherId = y.leatherId
                                    }).ToList();
                                    db.InternalOrderDetail.Add(newDetail);
                                }

                            }
                        }
                        db.SaveChanges();

                        //List<InternalOrderDetailStone> internalOrderDetailStones = new List<InternalOrderDetailStone>();
                        //foreach (var d in entity.InternalOrderDetailList)
                        //{
                        //    var stone = d.InternalOrderDetailStonesList.ToList();
                        //    foreach (var s in stone)
                        //    {

                        //    }
                        //}
                        //db.InternalOrderDetailStone.RemoveRange(internalOrderDetailStones);

                        //List<InternalOrderDetailLeather> internalOrderDetailLeathers = new List<InternalOrderDetailLeather>();
                        //foreach (var i in entity.InternalOrderDetailList)
                        //{
                        //    var leather = i.InternalOrderDetailLeathersList.SingleOrDefault();
                        //    if (leather != null)
                        //    {
                        //        if (internalOrderDetailLeathers.Count(x => x.Id == i.Id) == 0)
                        //        {
                        //            internalOrderDetailLeathers.Add(leather);
                        //        }
                        //    }
                        //}
                        //db.InternalOrderDetailLeather.RemoveRange(internalOrderDetailLeathers);

                        var Log = new InternalOrderStatusLog()
                        {
                            InternalOrder = entity,
                            InternalOrderId = entity.Id,
                            InternalOrderStatus = InternalOrderStatus.Registered,
                            CreateDate = DateTime.Now,
                            RemindDate = DateTime.Now,
                            UserId = user.Id,
                            Ip = Request.UserHostAddress
                        };
                        db.InternalOrderStatusLog.Add(Log);
                        db.SaveChanges();
                    }
                    else
                    {
                        var item = new InternalOrder
                        {
                            Name = model.name,
                            Count = model.count,
                            BranchId = user.BranchId,
                            PhoneNumber = model.phoneNumber,
                            Telephone = model.telephone,
                            Date = DateTime.Now,
                            Deposit = model.deposit,
                            DetailCount = model.detailCount,
                            UserType = model.userType,
                            OrderTypeForm = model.orderTypeForm,
                            Gender = model.gender,
                            CreatePersonId = model.createPersonId,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.InternalOrder.Add(item);

                        item.InternalOrderDetailList = new List<InternalOrderDetail>();
                        foreach (var value in model.addProductViewModelList)
                        {
                            Random randomCode = new Random();
                            var trackCode = randomCode.Next(1000, 9999).ToString();
                            string phone = item.PhoneNumber.Substring(4, 2);
                            trackCode = phone + trackCode;
                            var detailItem = new InternalOrderDetail
                            {
                                InternalOrder = item,
                                BarcodeDate = DateUtility.GetDateTime(value.barcodeDate),
                                Barcode = trackCode,
                                TrackCode = trackCode,
                                ProductId = value.productId,
                                GoldOwnership = value.goldOwnership,
                                SiteCode = value.siteCode,
                                BookCode = value.bookCode,
                                LeatherLoop = value.leatherLoop,
                                GoldType = value.goldType,
                                ProductColor = value.productColor,
                                ProductType = value.productType,
                                NewProductType = value.newProductType,
                                FileName = value.fileName,
                                OrderType = value.orderType,
                                Size = value.size,
                                Count = value.count,
                                NewProductTitle = value.title,
                                Title = value.productTitle,
                                Description = value.description,
                                NewStone = value.newStone,
                                NewLeather = value.newLeather,
                                NewLeatherLoop = value.newLeatherLoop,
                                NewSize = value.newSize,
                                NewProductColor = value.newProductColor,
                                NewGoldType = value.newGoldType,
                                NewDescription = value.newDescription,
                                NewCount = value.newCount,
                            };
                            if (value.internalOrderDetailStoneViewModelList != null && value.internalOrderDetailStoneViewModelList.Count > 0)
                            {
                                detailItem.InternalOrderDetailStonesList = value.internalOrderDetailStoneViewModelList.Where(x => x.stoneId > 0).Select(y => new InternalOrderDetailStone
                                {
                                    InternalOrderDetail = detailItem,
                                    Order = y.order,
                                    StoneId = y.stoneId,
                                    Stone = detailItem.InternalOrderDetailStonesList.Select(x => x.Stone).FirstOrDefault(),
                                }).ToList();
                            }
                            if (value.internalOrderDetailLeatherViewModelList != null && value.internalOrderDetailLeatherViewModelList.Count > 0)
                            {
                                detailItem.InternalOrderDetailLeathersList = value.internalOrderDetailLeatherViewModelList.Where(x => x.leatherId > 0).Select(y => new InternalOrderDetailLeather
                                {
                                    InternalOrderDetail = detailItem,
                                    Order = y.order,
                                    LeatherId = y.leatherId,

                                }).ToList();
                            }
                            //if (detailItem.InternalOrderDetailLeathersList.Count > 0 && detailItem.InternalOrderDetailLeathersList != null)
                            //{
                            //    detailItem.InternalOrderDetailLeathersList = value.internalOrderDetailLeatherViewModelList.Select(y => new InternalOrderDetailLeather
                            //    {
                            //        InternalOrderDetail = detailItem,
                            //        Order = y.order,
                            //        LeatherId = y.leatherId,
                            //    }).ToList();
                            //}
                            db.InternalOrderDetail.Add(detailItem);
                            var barcode = new Barcode(trackCode, TypeBarcode.Code128C);
                            var bar128 = barcode.Encode(TypeBarcode.Code128C, trackCode, 886, 142);
                            string serverPath = Server.MapPath("~/Upload/OrderSheet/");
                            if (!Directory.Exists(serverPath))
                            {
                                Directory.CreateDirectory(serverPath);
                            }
                            bar128.Save(serverPath + trackCode + ".jpg");
                            var date = DateTime.Now;
                            var persianDate = DateUtility.GetPersianDate(date);
                            var mobileNumber = item.PhoneNumber;
                            var name = item.Name;
                            var branch = db.Branch.Where(x => x.Id == item.BranchId).Select(x => x.Name).FirstOrDefault();
                            var branchPhone = db.Branch.Where(x => x.Id == item.BranchId).Select(x => x.Phone).FirstOrDefault();
                            //List<int> stoneIds = detailItem.InternalOrderDetailStonesList.Select(x => x.StoneId).ToList();
                            //List<int> leatherIds = detailItem.InternalOrderDetailLeathersList.Select(x => x.LeatherId).ToList();
                            //List<string> stoneName = db.Stone.Where(x => stoneIds.Contains(x.Id)).Select(x => x.Name.ToString()).ToList();
                            //List<string> leatherName = db.Stone.Where(x => leatherIds.Contains(x.Id)).Select(x => x.Name.ToString()).ToList();
                            //string stoneNames = string.Join(",", stoneName);
                            //string leatherNames = string.Join(",", leatherName);
                            //string newStone = detailItem.NewStone;
                            //string newLeather = detailItem.NewLeather;
                            var productId = detailItem.ProductId;
                            var title = db.Product.Where(x => x.Id == productId).Select(x => x.Title).FirstOrDefault();
                            if (item.Gender == Gender.Male)
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    NikSmsWebServiceClient.SendSmsNik("کیاگالری" + "\n" + " جناب آقای " + name + "\n" + " سفارش شما به شماره پیگیری " + trackCode + "\n" + " درتاریخ " + persianDate + " ثبت گردید." + "\n" + " شعبه " + branch + "\n" + " تلفن: " + branchPhone, mobileNumber);
                                });
                            }
                            else
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    NikSmsWebServiceClient.SendSmsNik("کیاگالری" + "\n" + " جناب خانم " + name + "\n" + " سفارش شما به شماره پیگیری " + trackCode + "\n" + " درتاریخ " + persianDate + " ثبت گردید." + "\n" + " شعبه " + branch + "\n" + " تلفن: " + branchPhone, mobileNumber);
                                });
                            }
                            db.SaveChanges();
                        }
                        var Log = new InternalOrderStatusLog()
                        {
                            InternalOrder = item,
                            InternalOrderStatus = InternalOrderStatus.Registered,
                            CreateDate = DateTime.Now,
                            RemindDate = DateTime.Now,
                            UserId = user.Id,
                            Ip = Request.UserHostAddress
                        };
                        db.InternalOrderStatusLog.Add(Log);
                        db.SaveChanges();
                    }

                    response = new Response()
                    {
                        status = 200,
                        //data = entity.Id,
                        message = "ثبت اطلاعات با موفقیت انجام شد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        //[Authorize(Roles = "admin, add ,internalOrderBranch")]
        [AllowAnonymous]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.InternalOrder.Single(x => x.Id == id);
                    var data = new InternalOrderViewModel
                    {
                        id = entity.Id,
                        date = DateUtility.GetPersianDate(entity.Date),
                        name = entity.Name,
                        count = entity.Count,
                        phoneNumber = entity.PhoneNumber,
                        detailCount = entity.DetailCount,
                        deposit = entity.Deposit,
                        userType = entity.UserType,
                        orderTypeForm = entity.OrderTypeForm,
                        gender = entity.Gender,
                        roleBackStatus = entity.InternalOrderStatus,
                        telephone = entity.Telephone,
                        createPersonId = entity.CreatePersonId,
                        addProductViewModelList = entity.InternalOrderDetailList.Select(x => new AddProductViewModel()
                        {
                            detailId = x.Id,
                            barcodeDate = DateUtility.GetPersianDate(x.BarcodeDate),
                            //barcode = x.Barcode,
                            trackCode = x.TrackCode,
                            description = x.Description,
                            count = x.Count,
                            //orderType = x.OrderType,
                            title = x.Title,
                            newProductTitle = x.NewProductTitle,
                            productTitle = x.Product?.Title,
                            bookCode = x.BookCode,
                            image = x.FileName,
                            siteCode = x.SiteCode,
                            size = x.Size,
                            productType = x.ProductType,
                            newProductType = x.NewProductType,
                            leatherLoop = x.LeatherLoop,
                            goldType = x.GoldType,
                            productColor = x.ProductColor,
                            productId = x.ProductId,
                            goldOwnership = x.GoldOwnership,
                            leatherCount = x.Product?.LeatherCount,
                            stoneCount = x.Product?.StoneCount,
                            fileName = x.Product?.ProductFileList.FirstOrDefault(y => y.FileType == FileType.Order)?.FileName,
                            internalOrderDetailStoneViewModelList = x.InternalOrderDetailStonesList.Where(y => y.Id > 0).Select(z => new InternalOrderStoneDetailViewModel()
                            {
                                internalOrderDetailId = z.InternalOrderDetailId,
                                order = z.Order,
                                stoneId = z.StoneId,
                            }).ToList(),
                            internalOrderDetailLeatherViewModelList = x.InternalOrderDetailLeathersList.Where(y => y.Id > 0).Select(z => new InternalOrderLeatherDetailViewModel()
                            {
                                internalOrderDetailId = z.InternalOrderDetailId,
                                order = z.Order,
                                leatherId = z.LeatherId,
                            }).ToList(),
                            newStone = x.NewStone,
                            newLeather = x.NewLeather,
                            newLeatherLoop = x.NewLeatherLoop,
                            newSize = x.NewSize,
                            newProductColor = x.NewProductColor,
                            newGoldType = x.NewGoldType,
                            newDescription = x.NewDescription,
                            newCount = x.NewCount,
                        }).ToList(),
                    };
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
        /// گرفتن اطلاعات مشتری با استفاده از شماره همراه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("internalOrder/GetCustomerInfo/{phoneNumber}")]
        public JsonResult GetCustomerInfo(string phoneNumber)
        {
            Response response;
            try
            {
                if (phoneNumber != null)
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var entity = db.InternalOrder.Where(x => x.PhoneNumber == phoneNumber).FirstOrDefault();
                        if (entity != null)
                        {
                            var data = new InternalOrderViewModel
                            {
                                name = entity.Name,
                                phoneNumber = entity.PhoneNumber,
                                gender = entity.Gender,
                                telephone = entity.Telephone,
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
                                status = 500,
                                message = "اطلاعات مشتری یافت نشد.",
                            };
                        }
                    }
                }
                else
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "اطلاعات مشتری یافت نشد.",
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [AllowAnonymous]
        public JsonResult GetAllUserList()
        {
            var branchId = GetAuthenticatedUser().BranchId;
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.Person.Where(x => x.Active == true && x.BranchId == branchId).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = list.Select(x => new
                        {
                            id = x.Id,
                            name = x.ShortName,

                        })
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        //[Authorize]
        [AllowAnonymous]
        public JsonResult GetProductType(int productId)
        {

            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var productTypeList = db.Product.Where(x => x.Id == productId).Select(x => new
                    {
                        productType = x.ProductType

                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = productTypeList

                    };

                }


            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //[Authorize]
        [AllowAnonymous]
        public JsonResult GetProductDetail(int id)
        {

            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Product.Where(x => x.Id == id).Select(x => new
                    {
                        id = x.Id,
                        productType = x.ProductType,
                        fileName = x.ProductFileList.FirstOrDefault(y => y.FileType == FileType.Order).FileName,
                        stoneList = x.ProductStoneList.OrderBy(y => y.Order).Select(y => new
                        {
                            id = y.Stone.Id,
                            name = y.Stone.Name,
                            order = y.Stone.Order,
                        }).Distinct().ToList(),
                        stoneCount = x.StoneCount,

                        leatherList = x.ProductLeatherList.Select(y => new
                        {
                            id = y.Leather.Id,
                            name = y.Leather.Name
                        }).Distinct().ToList(),
                        leatherCount = x.LeatherCount,

                        size = x.Size.SizeValueList.Select(y => new
                        {
                            id = y.Id,
                            value = y.Value,


                        }).ToList()
                    }).Single();
                    response = new Response()
                    {
                        status = 200,
                        data = entity

                    };

                }


            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public JsonResult ReadyForDeliver(int orderId)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.InternalOrder.Single(x => x.Id == orderId && x.BranchId == user.BranchId || x.DeliveredBranchId == user.BranchId);

                    entity.InternalOrderStatusLogList.ForEach(x => x.RemindDate = null);

                    var statusLog = new InternalOrderStatusLog()
                    {
                        InternalOrderId = entity.Id,
                        InternalOrderStatus = InternalOrderStatus.ReadyForDeliver,
                        CreateDate = DateTime.Now,
                        UserId = GetAuthenticatedUserId(),
                        Ip = Request.UserHostAddress
                    };
                    db.InternalOrderStatusLog.Add(statusLog);
                    entity.InternalOrderStatus = InternalOrderStatus.ReadyForDeliver;
                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "ثبت اطلاعات با موفقیت انجام شد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        ///// <summary>
        ///// تغییر وضعیت سفارشات
        ///// </summary>
        ///// <param name="model">مدلی شامل ردیف های گیفت های می باشد</param>
        ///// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        //[HttpPost]
        //[Authorize(Roles = "admin, InternalOrderOffice, InternalOrderBranch ,internalOrderOfficeBarcodeRoom")]
        //public JsonResult ChangeStatus(InternalOrderChangeStatusViewModel model)
        //{
        //    Response response;
        //    try
        //    {
        //        int userid = GetAuthenticatedUserId();
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var internalOrderList = db.InternalOrder.Where(x => model.id.Any(y => y == x.Id)).ToList();
        //            foreach (InternalOrder internalOrder in internalOrderList)
        //            {
        //                internalOrder.InternalOrderStatus = model.status;
        //                if (model.status == InternalOrderStatus.SendToBranch)
        //                {
        //                    internalOrder.DeliveredBranchId = model.deliveredBranchId;
        //                }
        //                if (model.status == InternalOrderStatus.ReadyForDeliver)
        //                {
        //                    if (model.barcode != null)
        //                    {

        //                        internalOrder.Barcode = model.barcode;
        //                        Sms("کیا گالری\n" + "\n" + "مشتری گرامی " + " " + internalOrder.Name + " " + "سفارش شماآماده تحویل می باشد،جهت هماهنگی و تایید ارسال سفارش با شما تماس گرفته خواهد شد.", internalOrder.PhoneNumber);

        //                    }
        //                }
        //                if (model.status == InternalOrderStatus.Delivered && model.deliveredBranchId != null)
        //                {
        //                    //internalOrder.DeliveredBranchId = model.deliveredBranchId;
        //                    internalOrder.InternalOrderStatus = model.status;
        //                    var branchName = db.Branch.Where(x => x.Id == model.deliveredBranchId).Select(x => x.Name).SingleOrDefault();
        //                    Sms("کیا گالری\n" + "\n" + "مشتری گرامی" + " " + internalOrder.Name + " " + "سفارش شما به کدپیگیری " + " " + internalOrder.TrackCode + "جهت تحویل حضوری به شعبه" + " " + branchName + " " + "ارسال گردید.", internalOrder.PhoneNumber);
        //                }

        //                var log = new InternalOrderStatusLog()
        //                {
        //                    InternalOrderId = internalOrder.Id,
        //                    InternalOrderStatus = model.status,
        //                    CreateDate = DateTime.Now,
        //                    Ip = Request.UserHostAddress,
        //                    UserId = userid
        //                };
        //                db.InternalOrderStatusLog.Add(log);


        //            }
        //            db.SaveChanges();
        //        }
        //        response = new Response()
        //        {
        //            status = 200,
        //            message = " وضعیت سفارش به " + Enums.GetTitle(model.status) + " تغییر یافت. "
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        [Authorize]
        public JsonResult NoAnswer(int orderId, string date)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.InternalOrder.Single(x => x.Id == orderId && x.BranchId == user.BranchId || x.DeliveredBranchId == user.BranchId);
                    if (entity.DeliveryType == DeliveryType.DeliveryMan || entity.DeliveryType == DeliveryType.Branch || entity.DeliveryType == DeliveryType.KiaPersonnel || entity.DeliveryType == DeliveryType.Post)
                    {
                        entity.DeliveryType = null;
                    }
                    entity.InternalOrderStatusLogList.ForEach(x => x.RemindDate = null);


                    var statusLog = new InternalOrderStatusLog()
                    {
                        InternalOrderId = entity.Id,
                        InternalOrderStatus = InternalOrderStatus.NoAnswer,
                        CreateDate = DateTime.Now,
                        RemindDate = DateUtility.GetDateTime(date),
                        UserId = GetAuthenticatedUserId(),
                        Ip = Request.UserHostAddress
                    };
                    db.InternalOrderStatusLog.Add(statusLog);

                    entity.InternalOrderStatus = InternalOrderStatus.NoAnswer;

                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "ثبت اطلاعات با موفقیت انجام شد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //[Authorize]
        //public JsonResult PendingCustomer(int orderId, string date)
        //{
        //    Response response;
        //    try
        //    {
        //        var user = GetAuthenticatedUser();
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var entity = db.InternalOrder.Single(x => x.Id == orderId && x.BranchId == user.BranchId || x.DeliveredBranchId == user.BranchId);
        //            if (entity.DeliveryType == DeliveryType.DeliveryMan || entity.DeliveryType == DeliveryType.Branch || entity.DeliveryType == DeliveryType.KiaPersonnel || entity.DeliveryType == DeliveryType.Post)
        //            {
        //                entity.DeliveryType = null;
        //            }
        //            entity.InternalOrderStatusLogList.ForEach(x => x.RemindDate = null);
        //            var statusLog = new InternalOrderStatusLog()
        //            {
        //                InternalOrderId = entity.Id,
        //                InternalOrderStatus = InternalOrderStatus.PendingCustomer,
        //                CreateDate = DateTime.Now,
        //                RemindDate = DateUtility.GetDateTime(date),
        //                UserId = GetAuthenticatedUserId(),
        //                Ip = Request.UserHostAddress
        //            };
        //            db.InternalOrderStatusLog.Add(statusLog);
        //            entity.InternalOrderStatus = InternalOrderStatus.PendingCustomer;
        //            var orderInfo = db.InternalOrder.Where(x => x.Id == orderId).Single();
        //            Sms("کیا گالری\n" + "\n" + "مشتری گرامی" + " " + orderInfo.Name + " " + "پیرو تماس تلفنی منتظر اعلام نظر شما در خصوص سفارش به کدپیگیری" + orderInfo.TrackCode + "در چند روز آینده می باشیم.", orderInfo.PhoneNumber);
        //            db.SaveChanges();
        //            response = new Response()
        //            {
        //                status = 200,
        //                message = "ثبت اطلاعات با موفقیت انجام شد."
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //public JsonResult Delivered(SentViewModel model)
        //{
        //    Response response;
        //    try
        //    {
        //        var user = GetAuthenticatedUser();
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var entity = db.InternalOrder.Single(x => x.Id == model.orderId && x.BranchId == user.BranchId || x.DeliveredBranchId == user.BranchId);
        //            var statusLog = new InternalOrderStatusLog()
        //            {
        //                InternalOrderId = model.orderId,
        //                InternalOrderStatus = InternalOrderStatus.Delivered,
        //                CreateDate = DateTime.Now,
        //                UserId = GetAuthenticatedUserId(),
        //                Ip = Request.UserHostAddress
        //            };
        //            db.InternalOrderStatusLog.Add(statusLog);

        //            entity.InternalOrderStatusLogList.ForEach(x => x.RemindDate = null);

        //            entity.InternalOrderStatus = InternalOrderStatus.Delivered;
        //            entity.PonyUp = model.ponyUp;
        //            entity.DeliveryType = model.deliveryType;
        //            //entity.DeliveredBranchId = model.deliveredBranchId;

        //            var orderInfo = db.InternalOrder.Where(x => x.Id == model.orderId).Single();
        //            if (orderInfo.DeliveryType == DeliveryType.DeliveryMan)
        //            {
        //                Sms("کیا گالری\n" + "\n" + "مشتری گرامی" + " " + orderInfo.Name + " " + "سفارش شما به کدپیگیری " + " " + orderInfo.TrackCode + "به صورت پیک ارسال خواهد شد.", orderInfo.PhoneNumber);

        //            }
        //            if (orderInfo.DeliveryType == DeliveryType.Post)
        //            {
        //                Sms("کیا گالری\n" + "\n" + "مشتری گرامی" + " " + orderInfo.Name + " " + "سفارش شما به کدپیگیری " + " " + orderInfo.TrackCode + "به صورت پست ارسال خواهد شد.", orderInfo.PhoneNumber);
        //            }

        //            if (orderInfo.DeliveryType == DeliveryType.KiaPersonnel)
        //            {
        //                Sms("کیا گالری\n" + "\n" + "مشتری گرامی" + " " + orderInfo.Name + " " + "ضمن تشکر از خرید شما،سفارش شما تحویل پرسنل کیا گردید.", orderInfo.PhoneNumber);

        //            }


        //            db.SaveChanges();

        //            response = new Response()
        //            {
        //                status = 200,
        //                message = "ثبت اطلاعات با موفقیت انجام شد."
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //public JsonResult Cancel(int id)
        //{
        //    Response response;
        //    try
        //    {
        //        var user = GetAuthenticatedUser();
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var entity = db.InternalOrder.Single(x => x.Id == id);
        //            var statusLog = new InternalOrderStatusLog()
        //            {
        //                InternalOrderId = entity.Id,
        //                InternalOrderStatus = InternalOrderStatus.Cancel,
        //                CreateDate = DateTime.Now,
        //                UserId = GetAuthenticatedUserId(),

        //                Ip = Request.UserHostAddress
        //            };
        //            db.InternalOrderStatusLog.Add(statusLog);

        //            entity.InternalOrderStatusLogList.ForEach(x => x.RemindDate = null);

        //            entity.InternalOrderStatus = InternalOrderStatus.Cancel;
        //            var orderInfo = db.InternalOrder.Where(x => x.Id == id).Single();
        //            Sms("مشتری گرامی" + " " + orderInfo.Name + " " + "سفارش شما با کد پیگیری: " + orderInfo.TrackCode + "لغو شد.", orderInfo.PhoneNumber);

        //            db.SaveChanges();

        //            response = new Response()
        //            {
        //                status = 200,
        //                message = "سفارش با موفقیت لغو شد."
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //public JsonResult Delete(int id)
        //{
        //    Response response;
        //    try
        //    {
        //        var user = GetAuthenticatedUser();
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var entity = db.InternalOrder.Include(x => x.InternalOrderLogList).Include(x => x.InternalOrderStatusLogList).Single(x => x.Id == id);
        //            var statusLog = new InternalOrderStatusLog()
        //            {
        //                InternalOrderId = entity.Id,
        //                InternalOrderStatus = InternalOrderStatus.Deleted,
        //                CreateDate = DateTime.Now,
        //                UserId = GetAuthenticatedUserId(),
        //                Ip = Request.UserHostAddress
        //            };
        //            db.InternalOrderStatusLog.Add(statusLog);
        //            entity.InternalOrderStatus = InternalOrderStatus.Deleted;
        //            db.SaveChanges();

        //            response = new Response()
        //            {
        //                status = 200,
        //                message = "سفارش مورد نظر حذف شد."
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //public JsonResult AddComment(int id, string comment)
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var entity = new InternalOrderLog()
        //            {
        //                InternalOrderId = id,
        //                Text = comment,
        //                UserId = GetAuthenticatedUserId(),
        //                CreatedDate = DateTime.Now
        //            };
        //            db.InternalOrderLog.Add(entity);
        //            db.SaveChanges();

        //            response = new Response()
        //            {
        //                status = 200,
        //                message = "ثبت اطلاعات با موفقیت انجام شد."
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //[HttpPost]
        //public JsonResult ReminderNoAnswer(int orderId, string date)
        //{
        //    Response response;

        //    using (var db = new KiaGalleryContext())
        //    {
        //        var entity = db.InternalOrder.Single(x => x.Id == orderId);
        //        entity.InternalOrderStatus = InternalOrderStatus.NoAnswer;
        //        var entityLog = new InternalOrderLog()
        //        {
        //            InternalOrderId = orderId,
        //            Text = "جواب نداد" + date,
        //            UserId = GetAuthenticatedUserId(),
        //            CreatedDate = DateTime.Now
        //        };
        //        db.InternalOrderLog.Add(entityLog);
        //        if (entity.InternalOrderStatus == InternalOrderStatus.NoAnswer)
        //        {
        //            var orderInfo = db.InternalOrder.Where(x => x.Id == orderId).Single();
        //            Sms("کیا گالری\n" + "\n" + "مشتری گرامی" + " " + orderInfo.Name + " " + " جهت هماهنگی سفارش با کدپیگیری" + " " + orderInfo.TrackCode + " " + "در تاریخ" + Common.DateUtility.GetPersianDate(orderInfo.Date) + "با شما تماس گرفته شد و پاسخگو نبودید لطفا در صورت امکان با شماره " + " " + orderInfo.DeliveredBranch.Phone + " " + " تماس حاصل نمایید.", orderInfo.PhoneNumber);
        //        }
        //        db.SaveChanges();
        //    }

        //    response = new Response()
        //    {
        //        status = 200,
        //        message = " ثبت اطلاعات با موفقیت انجام شد."
        //    };

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //[HttpPost]
        //public JsonResult ReminderAnswered(int orderId, string date)
        //{
        //    Response response;

        //    using (var db = new KiaGalleryContext())
        //    {
        //        var entity = db.InternalOrder.Single(x => x.Id == orderId);
        //        entity.InternalOrderStatus = InternalOrderStatus.PendingCustomer;
        //        var entityLog = new InternalOrderLog()
        //        {
        //            InternalOrderId = orderId,
        //            Text = "جواب داد" + date,
        //            UserId = GetAuthenticatedUserId(),
        //            CreatedDate = DateTime.Now
        //        };
        //        db.InternalOrderLog.Add(entityLog);

        //        db.SaveChanges();
        //    }

        //    response = new Response()
        //    {
        //        status = 200,
        //        message = " ثبت اطلاعات با موفقیت انجام شد."
        //    };

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        [Authorize]
        public JsonResult GetStatusLogHistory(int id)
        {
            var user = GetAuthenticatedUser();
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var log = db.InternalOrderStatusLog.Where(x => x.InternalOrderId == id).Select(x => new InternalOrderChangeStatusViewModel
                    {
                        orderId = x.InternalOrderId,
                        createDate = x.CreateDate,
                        customerName = x.InternalOrder.Name,
                        branchName = x.InternalOrder.Branch.Name,
                        userName = x.User.FirstName + " " + x.User.LastName,

                    }).ToList();

                    log.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDateTime(x.createDate);
                    });
                    //var statusList = db.InternalOrderStatusLog.Include(x => x.User).Where(x => x.InternalOrderId == id).ToList().Select(x => new
                    //{
                    //    orderId = x.InternalOrderId,
                    //    status = x.InternalOrderStatus,
                    //    statusTitle = Enums.GetTitle(x.InternalOrderStatus),
                    //    userSubmitName = x.User.FirstName,
                    //    createDate = DateUtility.GetPersianDateTime(x.CreateDate)
                    //}).ToList();

                    response = new Common.Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = log
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

        [Authorize]
        public JsonResult GetAllBranch()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var listData = db.Branch.Select(x => new
                    {
                        id = x.Id,
                        name = x.Name
                    }).ToList();

                    response = new Common.Response()
                    {
                        status = 200,
                        data = listData
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetProductAutoComplete(EditViewModel model)
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Product.Where(x => x.Active == true);
                    if (!string.IsNullOrEmpty(model.term))
                    {
                        query = query.Where(x => x.Title.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.Title.Contains(model.term.Trim()) || x.Code.Contains(model.term) || x.BookCode.Contains(model.term));
                    };
                    if (model.productType != null && model.productType > 0)
                    {
                        query = query.Where(x => x.ProductType == model.productType);
                    }
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        code = x.Code,
                        bookCode = x.BookCode,
                        title = x.Title,
                        fileName = x.ProductFileList.FirstOrDefault(y => y.FileType == FileType.Order).FileName
                    }).OrderBy(x => x.title).Take(6).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);


                }

            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return Json("[]", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Search(InternalOrderSearchViewModel model)
        {
            var user = GetAuthenticatedUser();
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.InternalOrder.Select(x => x);
                    if (User.IsInRole("admin"))
                    {
                        query = query.Select(x => x);
                    }
                    else
                    {
                        query = query.Where(x => x.BranchId == user.BranchId);
                    }
                    if (model.notCustomer == true)
                    {
                        query = query.Where(x => x.UserType == UserType.KiaPersonnel);
                    }
                    //if (model.notCustomer == false)
                    //{
                    //    query = query.Where(x => x.UserType == UserType.Customer);
                    //}
                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.PhoneNumber.Contains(model.term.Trim()) || x.Name.Contains(model.term.Trim()) || x.Branch.Name.Contains(model.term.Trim()) || x.OrderTypeForm.ToString().Contains(model.term.Trim()) || x.CreatePerson.ShortName.Contains(model.term.Trim()) || x.Deposit.ToString().Contains(model.term.Trim()) || x.InternalOrderDetailList.Select(y => y.SiteCode).Contains(model.term.Trim()) || x.InternalOrderDetailList.Select(y => y.Product.Title).Contains(model.term.Trim()) || x.InternalOrderDetailList.Select(y => y.TrackCode).Contains(model.term.Trim()));
                    }
                    if (model.remind != true)
                    {
                        //if (!string.IsNullOrEmpty(model.text))
                        //{
                        //    query = query.Include(x => x.CreateUser).Include(x => x.InternalOrderLogList).Include("InternalOrdeLogList.CreateUser").Where(x => x.Barcode.Contains(model.text) || x.Name.Contains(model.text) || x.PhoneNumber.Contains(model.text));
                        //}

                        if (model.id != null)
                        {
                            query = query.Where(x => x.Id == model.id);
                        }

                        if (!string.IsNullOrEmpty(model.name))
                        {
                            query = query.Where(x => x.Name.Contains(model.name));
                        }

                        if (!string.IsNullOrEmpty(model.phoneNumber))
                        {
                            query = query.Where(x => x.PhoneNumber.Contains(model.phoneNumber));
                        }

                        if (!string.IsNullOrEmpty(model.title))
                        {
                            query = query.Where(x => x.InternalOrderDetailList.Select(z => z.Title).Contains(model.title));
                        }

                        //if (model.deposit != null)
                        //{
                        //    query = query.Where(x => x.InternalOrderDetailList.Where(x=>x.Deposit==model.deposit).ToList());
                        //}
                        //if (!string.IsNullOrEmpty(model.barcode))
                        //{
                        //    query = query.Where(x => x.Barcode.Contains(model.barcode));
                        //}

                        if (model.userId != null)
                        {
                            query = query.Where(x => x.CreateUserId == model.userId);
                        }

                        //if (model.status != null && model.status.Where(x => x != null).Count() > 0)
                        //{
                        //    query = query.Where(x => model.status.Any(y => y == x.InternalOrderStatus));
                        //    //query = query.Where(x => x.UserType == UserType.Customer);
                        //}

                        //if (model.status != null && model.status.Where(x => x != null).Count() > 0 && model.status.Any(y => y == InternalOrderStatus.Deleted))
                        //{

                        //    query = query.Where(x => x.InternalOrderStatus == InternalOrderStatus.Deleted);
                        //}

                        if (model.userType != null)
                        {
                            query = query.Where(x => x.UserType == UserType.KiaPersonnel);
                        }
                    }
                    else
                    {
                        var date = DateTime.Today.AddDays(1);
                        query = query.Where(x => x.InternalOrderStatusLogList.Any(y => y.RemindDate != null && y.RemindDate <= date && (y.InternalOrderStatus == InternalOrderStatus.PendingCustomer || y.InternalOrderStatus == InternalOrderStatus.NoAnswer)));
                    }

                    dataCount = query.Count();
                    var list = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    var data = list.Select(item => new
                    {
                        id = item.Id,
                        date = item.Date,
                        prodcutId = item.InternalOrderDetailList.Where(x => x.ProductId > 0).Select(x => x.ProductId).FirstOrDefault(),
                        name = item.Name,
                        phoneNumber = item.PhoneNumber,
                        deposit = item.Deposit,
                        status = item.InternalOrderStatus,
                        noAnswerCount = item.InternalOrderStatusLogList.Count(x => x.InternalOrderStatus == InternalOrderStatus.NoAnswer),
                        deliverType = item.DeliveryType,
                        siteBook = item.InternalOrderDetailList.Select(x => x.SiteCode).FirstOrDefault(),
                        bookCode = item.InternalOrderDetailList.Select(x => x.BookCode).FirstOrDefault(),
                        ponyUp = item.PonyUp,
                        user = item.CreatePerson.ShortName,
                        userColor = item.CreateUser.Color,
                        userType = item.UserType,
                        detailCount = item.DetailCount,
                        branchName = item.Branch.Name,
                        title = item.InternalOrderDetailList.Select(x => x.Title).FirstOrDefault(),
                        productTitle = item.InternalOrderDetailList.Where(x => x.ProductId > 0).Select(x => x.Product.Title).FirstOrDefault(),
                        orderTypeForm = item.OrderTypeForm,
                        //orderedBranchName = item.Branch.Name,
                        trackCode = item.InternalOrderDetailList.Select(x => x.TrackCode).FirstOrDefault(),
                        //fileName = item.Product.ProductFileList.FirstOrDefault(y => y.FileType == FileType.Order).FileName,
                        //log = item.InternalOrderLogList.Select(x => new
                        //{
                        //    user = x.User.FirstName,
                        //    text = x.Text,
                        //    createdDate = x.CreatedDate
                        //}).ToList(),
                        detail = item.InternalOrderDetailList.Select(y => new
                        {
                            image = item.InternalOrderDetailList.Select(z => z.FileName).FirstOrDefault(),
                            fileName = y.Product.ProductFileList.FirstOrDefault(z => z.FileType == FileType.Order).FileName,

                        }).ToList()

                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data.Select(item => new
                            {
                                id = item.id,
                                date = DateUtility.GetPersianDate(item.date),
                                name = item.name,
                                phoneNumber = item.phoneNumber,
                                productId = item.prodcutId,
                                deposit = item.deposit,
                                //barcode = item.barcode,
                                statusCode = item.status,
                                deliverType = item.deliverType,
                                deliverTypeTitle = Enums.GetTitle(item.deliverType),
                                //status = Enums.GetTitle(item.status),
                                noAnswerCount = item.noAnswerCount,
                                ponyUp = item.ponyUp,
                                user = item.user,
                                userColor = item.userColor,
                                siteCode = item.siteBook,
                                bookCode = item.bookCode,
                                //userType = Enums.GetTitle(item.userType),
                                detailCount = item.detailCount,
                                branchName = item.branchName,
                                trackCode = item.trackCode,
                                title = item.title,
                                productTitle = item.productTitle,
                                orderTypeForm = item.orderTypeForm,
                                orderTypeFromTitle = Enums.GetTitle(item.orderTypeForm),
                                //orderedBranchName = item.orderedBranchName,
                                //fileName = item.fileName,
                                //log = item.log.Select(x => new
                                //{
                                //    x.user,
                                //    x.text,
                                //    createdDate = DateUtility.GetPersianDateTime(x.createdDate)
                                //}).ToList(),
                                detail = item.detail.Select(y => new
                                {
                                    image = y.image,
                                    fileName = y.fileName,
                                }).ToList()

                            }).ToList(),
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

        [AllowAnonymous]
        public JsonResult GetDataRecord(string phoneNumber)
        {
            List<string> ss = new List<string>();
            using (var db = new KiaGalleryContext())
            {
                var data = db.InternalOrderDetail.Where(x => x.InternalOrder.PhoneNumber == phoneNumber && x.InternalOrder.InternalOrderStatus != InternalOrderStatus.Deleted).Select(x => new GetDataRecordViewModel
                {
                    productTitle = x.Product.Title,
                    productType = x.ProductType,
                    bookCode = x.Product.BookCode,
                    siteCode = x.SiteCode,
                    //stoneId = x.StoneId,
                    //leatherId = x.LeatherId,
                    size = x.Size,
                    goldType = x.GoldType,
                    image = "/upload/product/" + x.Product.ProductFileList.Where(y => y.FileType == FileType.WhiteBack).Select(y => y.FileName).FirstOrDefault()
                }).ToList();

                List<int> stoneIdList = new List<int>();
                List<int> leatherIdList = new List<int>();
                foreach (var item in data)
                {
                    if (!string.IsNullOrEmpty(item.stoneId))
                        stoneIdList.AddRange(item.stoneId.Split('-').Select(x => int.Parse(x)).ToList());

                    if (!string.IsNullOrEmpty(item.leatherId))
                        leatherIdList.AddRange(item.leatherId.Split('-').Select(x => int.Parse(x)).ToList());


                }
                stoneIdList.Distinct();
                leatherIdList.Distinct();
                var stoneList = db.Stone.Where(x => x.Active == true && stoneIdList.Contains(x.Id)).Select(x => new
                {
                    x.Id,
                    x.Name
                }).ToArray();
                var leatherList = db.Leather.Where(x => x.Active == true && leatherIdList.Contains(x.Id)).Select(x => new
                {
                    x.Id,
                    x.Name
                }).ToArray();

                data.ForEach(item =>
                {
                    item.productTypeTitle = Enums.GetTitle(item.productType);
                    if (!string.IsNullOrEmpty(item.stoneId))
                    {
                        List<int> productStoneIdList = item.stoneId.Split('-').Select(x => int.Parse(x)).ToList();
                        item.description = "سنگ: " + string.Join(" / ", stoneList.Where(x => productStoneIdList.Contains(x.Id)).Select(x => x.Name).ToArray());
                    }
                    if (!string.IsNullOrEmpty(item.leatherId))
                    {
                        List<int> productLeatherIdList = item.leatherId.Split('-').Select(x => int.Parse(x)).ToList();
                        item.description += " , " + string.Join(" / ", leatherList.Where(x => productLeatherIdList.Contains(x.Id)).Select(x => x.Name).ToArray()) + ":چرم";
                    }

                    item.goldTypeTitle = Enums.GetTitle(item.goldType);

                });

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult ProductInquiry(EditViewModel model)
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Product.Where(x => x.Active == true);
                    if (!string.IsNullOrEmpty(model.term))
                    {
                        query = query.Where(x => x.Title.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.Title.Contains(model.term.Trim()) || x.Code.Contains(model.term) || x.BookCode.Contains(model.term));
                    };
                    if (model.productType != null && model.productType > 0)
                    {
                        query = query.Where(x => x.ProductType == model.productType);
                    }
                    var list = query.Select(x => new ProductInquiryViewModel
                    {
                        productTitle = x.Title,
                        productType = x.ProductType,
                        bookCode = x.BookCode,
                        siteCode = x.Code,
                        goldType = x.GoldType,
                        image = "/upload/product/" + x.ProductFileList.Where(y => y.FileType == FileType.WhiteBack).Select(y => y.FileName).FirstOrDefault()

                    }).ToList();

                    list.ForEach(item =>
                    {
                        item.productTypeTitle = Enums.GetTitle(item.productType);
                        item.goldTypeTitle = Enums.GetTitle(item.goldType);
                    });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return Json("[]", JsonRequestBehavior.AllowGet);
            }
        }

        public static void RestClient(object body)
        {
            string URI = "https://api.cloudware.ir/apigateway/crm/kiagallery/externalorder";
            var client = new RestClient(URI);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("x-clientId", "a8b1d2c7-3a73-474e-b53b-105fc930a9d0");
            request.AddJsonBody(body);
            var res = client.Execute(request);
            var resString = res.Content;
        }
        /// <summary>
        /// حذف سفارش
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.InternalOrder.Find(Id);
                    foreach (var i in item.InternalOrderDetailList)
                    {
                        db.InternalOrderDetailStone.RemoveRange(i.InternalOrderDetailStonesList);
                        db.InternalOrderDetailLeather.RemoveRange(i.InternalOrderDetailLeathersList);
                        if (item.InternalOrderStatusLogList != null)
                        {
                            db.InternalOrderStatusLog.RemoveRange(i.InternalOrder.InternalOrderStatusLogList);
                        }
                    }
                    db.InternalOrderDetail.RemoveRange(item.InternalOrderDetailList);
                    db.InternalOrder.Remove(item);

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
    }
}

