using br.com.arcnet.barcodegenerator;
using ClosedXML.Excel;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Gift;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر گیفت
    /// </summary>
    public class GiftController : BaseController
    {
        /// <summary>
        ///  صفحه عملیات گیفت 
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult Operation()
        {
            return View();
        }

        /// <summary>
        /// مدیریت گیفت
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, gift")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                List<CompanyListViewModel> companyList = db.Company.Where(x => x.Active == true).Select(x => new CompanyListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                ViewBag.CompanyList = companyList;
                ViewBag.BranchList = branchList;
                ViewBag.Count = db.Gift.Where(x => x.GiftType == GiftType.UnregisterFivePercentCard && x.GiftStatus == GiftStatus.Used).Count();
                var sum = db.Gift.Where(x => x.GiftType == GiftType.UnregisterFivePercentCard && x.GiftStatus == GiftStatus.Used).Sum(x => x.FactorPrice);
                ViewBag.Sum = Core.ToSeparator(sum);
            }

            return View();
        }

        /// <summary>
        /// مدیریت گیفت
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, gift,giftRegistration")]
        public ActionResult Manage()
        {
            using (var db = new KiaGalleryContext())
            {
                List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();

                ViewBag.BranchList = branchList;
            }
            var user = GetAuthenticatedUser();
            ViewBag.BranchId = user.BranchId;
            return View();
        }

        ///// <summary>
        ///// جستجو بر اساس اطلاعات کاربر
        ///// </summary>
        ///// <returns>صفحه مورد نظر</returns>
        //[HttpGet]
        //[Authorize(Roles = "admin, gift, giftRegistration, solicitorshipGift")]
        //public ActionResult Burn()
        //{
        //    var user = GetAuthenticatedUser();
        //    ViewBag.BranchId = user.BranchId;
        //    return View();
        //}

        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="Id">ردیف گیفت</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, gift")]
        public ActionResult Edit(int Id)
        {
            using (var db = new KiaGalleryContext())
            {
                List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                List<CompanyListViewModel> companyList = db.Company.Where(x => x.Active == true).Select(x => new CompanyListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                ViewBag.CompanyList = companyList;
                ViewBag.BranchList = branchList;
            }
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش گیفت";
            return View();
        }
        /// <summary>
        /// گیفت جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, gift")]
        public ActionResult Add()
        {
            ViewBag.Title = "گیفت جدید";
            return View("Edit");
        }

        public ActionResult BranchGiftReport()
        {
            using (var db = new KiaGalleryContext())
            {
                List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                ViewBag.BranchList = branchList;
            }

            return View();

        }
        public JsonResult BranchGiftReportJson(GiftReportSearchViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                    {
                        id = x.Id,
                        name = x.Name
                    }).ToList();
                    var query = db.Gift.Select(x => x);

                    var data = query.GroupBy(x => x.BranchShopping).Select(x => new GiftReportViewModel
                    {
                        branchName = x.Key.Name,
                        data = x.ToList().GroupBy(y => y.GiftStatus).Select(y => new GiftReportDetailViewModel
                        {
                            status = y.Key,
                            count = y.Count()

                        }).ToList()

                    }).ToList();
                    data.ForEach(x =>
                    {
                        x.data.ForEach(y =>
                        {
                            y.statusTitle = Enums.GetTitle(y.status);
                        });

                    });


                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
                            //pageCount = Math.Ceiling((double)dataCount / model.count),
                            //count = dataCount,
                            //page = model.page + 1
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


        [Authorize(Roles = "admin ,gift")]
        public ActionResult SellToBranch()
        {
            using (var db = new KiaGalleryContext())
            {
                List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                List<CompanyListViewModel> companyList = db.Company.Where(x => x.Active == true).Select(x => new CompanyListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                ViewBag.CompanyList = companyList;
                ViewBag.BranchList = branchList;
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin ,gift")]
        public JsonResult SellToBranchJson(string term)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Gift.Where(x => x.Code == term && x.GiftStatus == GiftStatus.DeliveryFromPrintingHouse);

                    var data = query.Select(item => new
                    {
                        id = item.Id,
                        code = item.Code,
                        type = item.GiftType,
                        value = item.Value,
                        status = item.GiftStatus,

                    }).SingleOrDefault();
                    if (data != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new
                            {
                                id = data.id,
                                code = data.code,
                                type = data.type,
                                typeTitle = Enums.GetTitle(data.type),
                                value = data.value,
                                status = data.status,
                                statusTitle = Enums.GetTitle(data.status),

                            }
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "موردی یافت نشد"
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


        [Authorize(Roles = "admin ,gift")]
        public ActionResult AcceptFromPrintingHouse()
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
        /// <summary>
        /// گرفتن گیفت بر اساس کد گیفت برای صفحه عملیات
        /// </summary>
        /// <param name="term">کد گیفت</param>
        /// <returns>گیفت مورد نظز</returns>
        [HttpGet]
        [Authorize(Roles = "admin ,gift")]
        public JsonResult OperationJson(string term)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Gift.Where(x => x.Code == term);
                    var data = query.Select(item => new
                    {
                        id = item.Id,
                        code = item.Code,
                        type = item.GiftType,
                        value = item.Value,
                        status = item.GiftStatus,
                    }).SingleOrDefault();
                    if (data != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new
                            {
                                id = data.id,
                                code = data.code,
                                type = data.type,
                                typeTitle = Enums.GetTitle(data.type),
                                value = data.value,
                                status = data.status,
                                statusTitle = Enums.GetTitle(data.status),
                            }
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "موردی یافت نشد"
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
        /// ذخیره گیفت
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات گیفت</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, gift")]
        public JsonResult Save(GiftViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                int userid = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.Gift.Single(x => x.Id == model.id);
                        entity.Value = model.value;
                        entity.GiftType = model.giftType;
                        entity.GiftStatus = model.giftStatus;
                        entity.BuyerCustomerName = model.buyerCustomerName;
                        entity.BuyerCustomerPhoneNumber = model.buyerCustomerPhoneNumber;
                        entity.RevocationCustomerName = model.revocationCustomerName;
                        entity.RevocationCustomerPhoneNumber = model.revocationCustomerPhoneNumber;
                        entity.BranchIdShopping = model.branchIdShopping;
                        entity.CompanyIdShopping = model.companyIdShopping;
                        entity.ExpirationTime = model.expirationTime;
                        entity.ExpiryDateToSolar = DateUtility.GetDateTime(model.expiryDateToSolar);
                        entity.Description = model.description;
                        message = "گیفت با موفقیت ویرایش شد.";
                        var log = new GiftLog()
                        {
                            Gift = entity,
                            CreateUserId = userid,
                            GiftStatus = model.giftStatus,
                            CreateDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.GiftLog.Add(log);
                    }
                    else
                    {
                        List<string> codeList = new List<string>();
                        for (int i = 1; i <= model.count; i++)
                        {
                            string code;
                            while (true)
                            {
                                code = RandomString(12);
                                Gift gift = db.Gift.FirstOrDefault(x => x.Code == code);
                                if (gift == null && codeList.Count(x => x == code) == 0)
                                {
                                    break;
                                }
                            }
                            codeList.Add(code);
                            var entity = new Gift()
                            {
                                Code = code,
                                Value = model.value,
                                ExpirationTime = model.expirationTime,
                                ExpiryDateToSolar = DateUtility.GetDateTime(model.expiryDateToSolar),
                                GiftType = model.giftType,
                                GiftStatus = model.giftStatus,
                                BuyerCustomerName = model.buyerCustomerName,
                                BuyerCustomerPhoneNumber = model.buyerCustomerPhoneNumber,
                                RevocationCustomerName = model.revocationCustomerName,
                                RevocationCustomerPhoneNumber = model.revocationCustomerPhoneNumber,
                                Description = model.description
                            };
                            var log = new GiftLog()
                            {
                                Gift = entity,
                                GiftStatus = model.giftStatus,
                                CreateUserId = userid,
                                CreateDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };
                            db.GiftLog.Add(log);
                            db.Gift.Add(entity);
                            var barcode = new Barcode(code, TypeBarcode.Code128C);
                            var bar128 = barcode.Encode(TypeBarcode.Code128C, code, 886, 142);

                            string serverPath = Server.MapPath("~/Upload/Gift/");
                            if (!Directory.Exists(serverPath))
                            {
                                Directory.CreateDirectory(serverPath);
                            }
                            bar128.Save(serverPath + code + ".jpg");
                        }
                        message = "گیفت با موفقیت ایجاد شد.";
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
        /// خواندن اطلاعات گیفت
        /// </summary>
        /// <param name="id">ردیف گیفت</param>
        /// <returns>جیسون اطلاعات لود شده گیفت</returns>
        [HttpGet]
        [Authorize(Roles = "admin, gift")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Gift item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Gift.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new GiftViewModel()
                        {
                            id = item.Id,
                            code = item.Code,
                            count = 1,
                            giftStatus = item.GiftStatus,
                            giftType = item.GiftType,
                            value = item.Value,
                            expirationTime = item.ExpirationTime.Value,
                            expiryDateToSolar = DateUtility.GetPersianDate(item.ExpiryDateToSolar),
                            buyerCustomerName = item.BuyerCustomerName,
                            buyerCustomerPhoneNumber = item.BuyerCustomerPhoneNumber,
                            revocationCustomerName = item.RevocationCustomerName,
                            revocationCustomerPhoneNumber = item.RevocationCustomerPhoneNumber,
                            branchIdShopping = item.BranchIdShopping,
                            companyIdShopping = item.CompanyIdShopping,
                            description = item.Description
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "گیفت مورد نظر یافت نشد."
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
        /// جستجوی شعب
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست شعب پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, gift, giftRegistration,solicitorshipgift")]
        public JsonResult Search(GiftSearchViewModel model)
        {
            Response response;
            try
            {
                List<GiftTitleViewModel> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Gift.Select(x => x);
                    if (model.status != null)
                    {
                        query = query.Where(x => x.GiftStatus == model.status);
                    }
                    if (!string.IsNullOrEmpty(model.term))
                    {
                        query = query.Where(x => x.Code == model.term || x.BuyerCustomerName.Contains(model.term) || x.BuyerCustomerPhoneNumber.Contains(model.term));
                    }
                    if (!string.IsNullOrEmpty(model.buyerCustomerName))
                    {
                        query = query.Where(x => x.BuyerCustomerName.Contains(model.buyerCustomerName));
                    }
                    if (!string.IsNullOrEmpty(model.buyerCustomerPhoneNumber))
                    {
                        query = query.Where(x => x.BuyerCustomerPhoneNumber.Contains(model.buyerCustomerPhoneNumber));
                    }
                    if (model.filter != null)
                    {
                        query = query.Where(x => x.Value == model.filter);
                    }
                    if (model.giftType != null)
                    {
                        query = query.Where(x => x.GiftType == model.giftType);
                    }
                    if (model.branchIdShopping != null && model.branchIdShopping > 0)
                    {
                        query = query.Where(x => x.BranchIdShopping == model.branchIdShopping);
                    }
                    if (model.companyIdShopping != null && model.companyIdShopping > 0)
                    {
                        query = query.Where(x => x.CompanyIdShopping == model.companyIdShopping);
                    }
                    if (model.branchReceiverCustomer != null && model.branchReceiverCustomer > 0)
                    {
                        query = query.Where(x => x.GiftLog.Where(y => y.GiftStatus == GiftStatus.Used).Any(z => z.CreateUser.BranchId == model.branchReceiverCustomer));
                    }
                    dataCount = query.Count();

                    if (!string.IsNullOrEmpty(model.order))
                    {
                        switch (model.order)
                        {
                            case "price":
                                query = query.OrderByDescending(x => x.Value);
                                break;
                            default:
                                query = query.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.dateSort))
                    {
                        switch (model.dateSort)
                        {
                            case "OrderBy":
                                query = query.OrderBy(x => x.ExpirationTime != null && x.ExpirationTime > 0? DbFunctions.AddDays(x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheBranch).CreateDate, (int)x.ExpirationTime.Value):x.ExpiryDateToSolar).ThenBy(x => x.ExpirationTime != null && x.ExpirationTime > 0? x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheCustomer).CreateDate:x.ExpiryDateToSolar);
                                break;
                            case "OrderByDescending":
                                query = query.OrderByDescending(x => x.ExpirationTime != null && x.ExpirationTime > 0 ? DbFunctions.AddDays(x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheBranch).CreateDate, (int)x.ExpirationTime.Value) : x.ExpiryDateToSolar).ThenBy(x => x.ExpirationTime != null && x.ExpirationTime > 0 ? x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheCustomer).CreateDate : x.ExpiryDateToSolar);
                                break;
                        }
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.Id);
                    }
                    query = query.Skip(model.page * model.count).Take(model.count);
                    list = query.Select(item => new GiftTitleViewModel()
                    {
                        id = item.Id,
                        code = item.Code,
                        giftStatus = item.GiftStatus,
                        giftType = item.GiftType,
                        value = item.Value,
                        companyShopping = item.CompanyShopping.Name,
                        branchShopping = item.BranchShopping.Name,
                        branchReceiverCustomer = item.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateUser.Branch.Name,
                        buyerCustomerName = item.BuyerCustomerName,
                        buyerCustomerPhoneNumber = item.BuyerCustomerPhoneNumber,
                        revocationCustomerName = item.RevocationCustomerName,
                        revocationCustomerPhoneNumber = item.RevocationCustomerPhoneNumber,
                        dateTimeSoldToTheCustomer = item.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheCustomer).CreateDate,
                        dateTimeSoldToTheBranch = item.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheBranch).CreateDate,
                        factorNumber = item.FactorNumber,
                        factorPriceToSeparator = item.FactorPrice,
                        expireTime = item.ExpirationTime != null && item.ExpirationTime > 0 ? item.ExpirationTime : 0,
                        expireDate = item.ExpiryDateToSolar,
                        canceledUserName = item.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Cancel).CreateUser.FirstName + " " + item.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Cancel).CreateUser.LastName,
                    }).ToList();

                    list.ForEach(x =>
                    {
                        x.giftStatusTitle = Enums.GetTitle(x.giftStatus);
                        x.valueToSeparator = Core.ToSeparator(x.value);
                        x.giftTypeTitle = Enums.GetTitle(x.giftType);
                        x.dateSoldToTheCustomer = DateUtility.GetPersianDateTime(x.dateTimeSoldToTheCustomer);
                        x.factorPrice = Core.ToSeparator(x.factorPriceToSeparator);
                        x.persianExpireTime = x.giftStatus == GiftStatus.SoldToTheCustomer ? x.expireTime != null && x.expireTime > 0 ? DateUtility.GetPersianDate(x.dateTimeSoldToTheCustomer != null ? x.dateTimeSoldToTheCustomer.Value.AddDays(x.expireTime.Value) : x.expireDate.Value.AddDays(x.expireTime.Value)) : DateUtility.GetPersianDate(x.expireDate) : x.giftStatus == GiftStatus.SoldToTheBranch ? x.expireTime != null && x.expireTime > 0 ? DateUtility.GetPersianDate(x.dateTimeSoldToTheBranch != null ? x.dateTimeSoldToTheBranch.Value.AddDays(x.expireTime.Value) : x.expireDate.Value.AddDays(x.expireTime.Value)) : DateUtility.GetPersianDate(x.expireDate) : null;

                    });
                }
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
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// گرفتن گزارش گیفت
        /// </summary>
        /// <param name="model">مدل حاوی فیلتر های گیفت</param>
        /// <returns>گزارش مورد نظر</returns>
        [HttpGet]
        public JsonResult GetUsedGiftFactor(UsedGiftFactorSearchViewModel model)
        {
            Response response;
            var convertedFromDate = model.fromDateToAc = DateUtility.GetDateTime(model.fromDate).GetValueOrDefault();
            var convertedtoDate = model.toDateToAc = DateUtility.GetDateTime(model.toDate).GetValueOrDefault();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    //var query = db.Gift.Where(x => !string.IsNullOrEmpty(x.FactorNumber) || (x.FactorNumber == null));
                    var query = db.Gift.Select(x => x);
                    if (model.giftStatus != null)
                    {
                        query = query.Where(x => x.GiftStatus == model.giftStatus);
                    }
                    if (model.giftType != null)
                    {
                        query = query.Where(x => x.GiftType == model.giftType);
                    }
                    if (model.fromDate != null && model.toDate != null)
                    {
                        query = query.Where(x => x.GiftLog.Where(y => y.CreateDate >= convertedFromDate && y.CreateDate <= convertedtoDate).Count() > 0);
                    }
                    if (!string.IsNullOrEmpty(model.revocationCustomerName))
                    {
                        query = query.Where(x => x.RevocationCustomerName.Contains(model.revocationCustomerName));
                    }
                    if (!string.IsNullOrEmpty(model.revocationCustomerPhoneNumber))
                    {
                        query = query.Where(x => x.RevocationCustomerPhoneNumber.Contains(model.revocationCustomerPhoneNumber));
                    }
                    if (model.companyIdShopping != null && model.companyIdShopping > 0)
                    {
                        query = query.Where(x => x.CompanyIdShopping == model.companyIdShopping);
                    }
                    if (model.branchReceiverCustomer != null && model.branchReceiverCustomer > 0)
                    {
                        query = query.Where(x => x.GiftLog.Where(y => y.GiftStatus == GiftStatus.Used).Any(z => z.CreateUser.BranchId == model.branchReceiverCustomer));
                    }

                    //var group = query.GroupBy(x => new
                    //{
                    //    x.BranchShopping,
                    //    x.FactorNumber,
                    //    x.FactorPrice,
                    //    x.BuyerCustomerName,
                    //    x.BuyerCustomerPhoneNumber,
                    //    x.BranchReceiver,
                    //    x.RevocationCustomerName,
                    //    x.RevocationCustomerPhoneNumber,
                    //});
                    //var dataCount = group.Count();
                    //var list = group.OrderBy(x => x.Key.FactorNumber).Skip(model.page * model.count).Take(model.count).Select(x => new UsedGiftFactorViewModel
                    //{
                    //    branchShopping = x.Key.BranchShopping.Name,
                    //    giftCount = x.Count(),
                    //    factorNumber = x.Key.FactorNumber,
                    //    factorPrice = x.Key.FactorPrice,
                    //    buyerCustomerName = x.Key.BuyerCustomerName,
                    //    buyerCustomerPhoneNumber = x.Key.BuyerCustomerPhoneNumber,
                    //    branchReceiverName = x.FirstOrDefault().GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateUser.Branch.Name,
                    //    revocationCustomerName = x.Key.RevocationCustomerName,
                    //    revocationCustomerPhoneNumber = x.Key.RevocationCustomerPhoneNumber,
                    //    codeList = x.Select(z => z.Code).ToList(),
                    //    branchShoppingList = x.Select(z => z.BranchShopping.Name).ToList(),
                    //    createDate = x.FirstOrDefault().GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateDate,
                    //    buyerCustomerNameList = x.Select(z => z.BuyerCustomerName).ToList(),
                    //    buyerCustomerPhoneNumberList = x.Select(z => z.BuyerCustomerPhoneNumber).ToList(),

                    //}).ToList();
                    var dataCount = query.Count();
                    long factorSum = 0;
                    if (dataCount > 0)
                    {
                        factorSum = query.Sum(x => x.FactorPrice);
                    }
                    var list = query.OrderBy(x => x.Id).Skip(model.page * model.count).Take(model.count).Select(x => new UsedGiftFactorViewModel
                    {
                        branchShopping = x.BranchShopping.Name,
                        factorNumber = x.FactorNumber,
                        factorPrice = x.FactorPrice,
                        buyerCustomerName = x.BuyerCustomerName,
                        buyerCustomerPhoneNumber = x.BuyerCustomerPhoneNumber,
                        branchReceiverName = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateUser.Branch.Name,
                        revocationCustomerName = x.RevocationCustomerName,
                        revocationCustomerPhoneNumber = x.RevocationCustomerPhoneNumber,
                    }).ToList();

                    list.ForEach(x =>
                    {
                        x.factorPriceSeparator = Core.ToSeparator(x.factorPrice);
                        x.usedDate = DateUtility.GetPersianDateTime(x.createDate);
                    });

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            factorSum = Core.ToSeparator(factorSum),
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
        /// تغییر وضعیت گیفت
        /// </summary>
        /// <param name="model">مدلی شامل ردیف های گیفت های می باشد</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, gift, giftRegistration, solicitorshipGift")]
        public JsonResult ChangeStatus(GiftChangeStatusViewModel model)
        {
            Response response;
            try
            {
                int userid = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    var giftList = db.Gift.Where(x => model.id.Any(y => y == x.Id)).ToList();
                    foreach (Gift gift in giftList)
                    {
                        gift.GiftStatus = model.status;
                        if (model.status == GiftStatus.SoldToTheBranch)
                        {
                            if (model.branchIdShopping != null)
                                gift.BranchIdShopping = model.branchIdShopping;
                            else
                                gift.CompanyIdShopping = model.companyIdShopping;
                        }
                        else if (model.status == GiftStatus.SoldToTheCustomer)
                        {
                            gift.BuyerCustomerName = model.customerName;
                            gift.BuyerCustomerPhoneNumber = model.customerPhoneNumber;
                        }
                        else if (model.status == GiftStatus.Used)
                        {
                            gift.RevocationCustomerName = model.customerName;
                            gift.RevocationCustomerPhoneNumber = model.customerPhoneNumber;
                        }
                        var log = new GiftLog()
                        {
                            GiftId = gift.Id,
                            GiftStatus = model.status,
                            CreateUserId = userid,
                            CreateDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.GiftLog.Add(log);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = " وضعیت گیفت به " + Enums.GetTitle(model.status) + " تغییر یافت. "
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// تغییر زمان انقضای گیفت
        /// </summary>
        /// <param name="model">مدلی شامل ردیف های گیفت های می باشد</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, gift, giftRegistration, solicitorshipGift")]
        public JsonResult ChangeExpirationTime(ChangeExpirationTimeViewModel model)
        {
            Response response;
            try
            {
                int userid = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    var giftList = db.Gift.Where(x => model.id.Any(y => y == x.Id)).ToList();
                    foreach (Gift gift in giftList)
                    {
                        if (model.expirationTime != null && model.expirationTime > 0)
                        {
                            gift.ExpirationTime = model.expirationTime;
                        }
                        else
                        {
                            gift.ExpirationTime = 0;
                            var date = DateUtility.GetDateTime(model.expirationDate);
                            gift.ExpiryDateToSolar = date;
                        }

                        if (gift.GiftStatus == GiftStatus.SoldToTheBranch)
                            gift.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheBranch).CreateDate = DateTime.Now;

                        if (gift.GiftStatus == GiftStatus.SoldToTheCustomer)
                            gift.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheCustomer).CreateDate = DateTime.Now;

                        //var log = new GiftLog()
                        //{
                        //    GiftId = gift.Id,
                        //    GiftStatus = model.status,
                        //    CreateUserId = userid,
                        //    CreateDate = DateTime.Now,
                        //    Ip = Request.UserHostAddress
                        //};
                        //db.GiftLog.Add(log);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "زمان انقضای گیفت های انتخاب شده تغییر یافت."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// حذف گیفت
        /// </summary>
        /// <param name="id">ردیف گیفت</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, gift")]
        public JsonResult Delete(List<int> Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var giftList = db.Gift.Where(x => Id.Any(y => y == x.Id)).ToList();
                    foreach (var item in giftList)
                    {
                        if (item.GiftStatus == GiftStatus.Cancel)
                        {
                            var logList = db.GiftLog.Where(x => Id.Any(y => y == x.GiftId)).ToList();
                            db.GiftLog.RemoveRange(logList);
                            db.Gift.Remove(item);
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 200,
                                message = string.Format("شما نمیتوانید گیفت را در وضعیت {0} حذف کنید.", Enums.GetTitle(item.GiftStatus))
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }

                    }
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "گیفت با موفقیت حذف شد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        /// <summary>
        /// ثبت کارت هدیه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, giftRegistration")]
        public ActionResult Registration()
        {
            return View();
        }

        /// <summary>
        /// ثبت کارت هدیه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpPost]
        [Authorize(Roles = "admin, giftRegistration,solicitorshipGift")]
        public JsonResult SaveRegistration(GiftRegistrationViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var giftList = db.Gift.Where(x => model.codeList.Any(y => y == x.Code) && x.GiftStatus != GiftStatus.Used && x.GiftStatus != GiftStatus.Cancel).ToList();
                    if (giftList != null)
                    {
                        long value = 0;
                        giftList.ForEach(item =>
                        {
                            value += item.Value;
                            item.GiftStatus = model.status;
                            if (model.status == GiftStatus.Used)
                            {
                                item.RevocationCustomerName = model.customerName;
                                item.RevocationCustomerPhoneNumber = model.customerPhoneNumber;
                                item.FactorNumber = model.factorNumber;
                                item.FactorPrice = model.factorPrice;
                            }
                            else
                            {
                                item.BuyerCustomerName = model.customerName;
                                item.BuyerCustomerPhoneNumber = model.customerPhoneNumber;
                            }
                            var log = new GiftLog()
                            {
                                GiftId = item.Id,
                                GiftStatus = model.status,
                                CreateUserId = user.Id,
                                CreateDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };
                            db.GiftLog.Add(log);
                        });

                        db.SaveChanges();
                        var status = "";
                        switch (model.status)
                        {
                            case GiftStatus.Used:
                                status = "با موفقیت ابطال گردید.";
                                break;
                            case GiftStatus.SoldToTheCustomer:
                                status = "برای مشتری ثبت شد.";
                                break;
                            default:
                                status = "";
                                break;
                        };
                        var valueTitle = "";
                        switch (value)
                        {
                            case 5:
                                valueTitle = " 5% ";
                                break;
                            case 5000000:
                                valueTitle = " 5,000,000 ریال ";
                                break;
                            case 2000000:
                                valueTitle = " 2,000,000 ریال ";
                                break;
                            case 1000000:
                                valueTitle = " 1,000,000 ریال ";
                                break;
                            default:
                                valueTitle = "";
                                break;
                        }
                        if (model.giftType == GiftType.UnregisterFivePercentCard)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                //NikSmsWebServiceClient.SendSmsNik(" یک عدد کارت تخفیف 5% توسط  " + model.owner + " با مبلغ فاکتور " + Core.ToSeparator(model.factorPrice) + " ریال در شعبه " + user.UserPlace + " استفاده شد. ", "09354047788");
                                //NikSmsWebServiceClient.SendSmsNik(" یک عدد کارت تخفیف 5% توسط  " + model.owner + " با مبلغ فاکتور " + Core.ToSeparator(model.factorPrice) + " ریال در شعبه " + user.UserPlace + " استفاده شد. ", "09123430339");
                                //NikSmsWebServiceClient.SendSmsNik(" یک عدد کارت تخفیف 5% توسط  " + model.owner + " با مبلغ فاکتور " + Core.ToSeparator(model.factorPrice) + " ریال در شعبه " + user.UserPlace + " استفاده شد. ", "09124254257");
                                //NikSmsWebServiceClient.SendSmsNik(" یک عدد کارت تخفیف 5% توسط  " + model.owner + " با مبلغ فاکتور " + Core.ToSeparator(model.factorPrice) + " ریال در شعبه " + user.UserPlace + " استفاده شد. ", "09126304190");
                            });

                        }
                        response = new Response()
                        {
                            status = 200,
                            message = " هدیه به ارزش " + valueTitle + status
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "گیفت کارتی با این شماره وجود ندارد."
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
        /// چک کارت هدیه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, giftRegistration,solicitorshipGift")]
        public JsonResult Check(string code)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Gift.Where(x => x.Code == code && (x.GiftStatus == GiftStatus.SoldToTheBranch || x.GiftStatus == GiftStatus.SoldToTheCustomer || x.GiftStatus == GiftStatus.Used || x.GiftStatus == GiftStatus.Cancel));

                    var item = entity.Select(x => new GiftCheckViewModel()
                    {
                        id = x.Id,
                        code = x.Code,
                        giftStatus = x.GiftStatus,
                        giftType = x.GiftType,
                        branchShopping = x.BranchShopping.Name,
                        branchShoppingId = x.BranchIdShopping,
                        companyIdShopping = x.CompanyIdShopping,
                        branchBuyerCustomer = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheCustomer).CreateUser.Branch.Name,
                        dateSaleCustomer = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheCustomer).CreateDate,
                        dateSaleBranch = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheBranch).CreateDate,
                        branchReceiverCustomer = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateUser.Branch.Name,
                        buyerCustomerName = x.BuyerCustomerName,
                        buyerCustomerPhoneNumber = x.BuyerCustomerPhoneNumber,
                        revocationCustomerName = x.RevocationCustomerName,
                        value = x.Value,
                        expirationTime = x.ExpirationTime.Value,
                        expirationDate = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Registered).CreateDate,
                        expirationDateToSolar = x.ExpiryDateToSolar.Value,
                        owner = x.CompanyIdShopping != null ? x.CompanyShopping.Name : (x.BranchIdShopping != null ? x.BranchShopping.Name : null)
                    }).FirstOrDefault();
                    if (item.giftStatus == GiftStatus.Cancel)
                    {
                        var date = entity.Select(x => x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Cancel).CreateDate).First();
                        var userFirstName = entity.Select(x => x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Cancel).CreateUser.FirstName).First();
                        var userLastName = entity.Select(x => x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Cancel).CreateUser.LastName).First();
                        var persianDateTime = DateUtility.GetPersianDateTime(date);
                        response = new Response()
                        {
                            status = 500,
                            message = string.Format("کاربر گرامی،کد هدیه مورد نظر در تاریخ {0} توسط {1} لغو گردیده است لطفا جهت بررسی با واحد پشتیبانی تماس بگیرید.", persianDateTime, userFirstName + " " + userLastName)
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    if (item.giftStatus == GiftStatus.SoldToTheCustomer)
                    {
                        item.persianExpirationDate = item.expirationTime != null && item.expirationTime > 0 ? DateUtility.GetPersianDate(item.dateSaleCustomer != null ? item.dateSaleCustomer.Value.AddDays(item.expirationTime.Value) : item.expirationDate.Value.AddDays(item.expirationTime.Value)) : DateUtility.GetPersianDate(item.expirationDateToSolar);
                    }
                    if (item.giftStatus == GiftStatus.SoldToTheBranch)
                    {
                        item.persianExpirationDate = item.expirationTime != null && item.expirationTime > 0 ? DateUtility.GetPersianDate(item.dateSaleBranch != null ? item.dateSaleBranch.Value.AddDays(item.expirationTime.Value) : item.expirationDate.Value.AddDays(item.expirationTime.Value)) : DateUtility.GetPersianDate(item.expirationDateToSolar);
                    }

                    var description = db.Gift.Where(x => x.Code == code).Select(x => x.Description).Single();
                    var comment = "";
                    if (!string.IsNullOrEmpty(description))
                    {
                        comment = " (" + description + ")";
                    }
                    else
                    {
                        comment = "";
                    }
                    if (item != null && item.giftStatus != GiftStatus.Used)
                    {
                        if (item.giftType != GiftType.UnregisterFivePercentCard)
                        {
                            if (item.giftStatus == GiftStatus.SoldToTheBranch && item.companyIdShopping == null && item.branchShoppingId != user.BranchId && item.giftType != GiftType.CheckNotRegistered)
                            {
                                response = new Response()
                                {
                                    status = 500,
                                    message = " هدیه ای با این کد به شعبه " + item.branchShopping + " فروخته شده است." + comment
                                };
                            }
                            else
                            {
                                DateTime dateSaleCustomer = item.dateSaleCustomer.GetValueOrDefault(); ///تاریخ فروش گیفت به مشتری برای محاسبه انقضا از این تاریخ
                                DateTime dateSaleBranch = item.dateSaleBranch.GetValueOrDefault(); ///تاریخ فروش گیفت به شعبه برای محاسبه انقضا از این تاریخ
                                DateTime expirationDateToSolar = item.expirationDateToSolar.GetValueOrDefault();/// تاریخ مشخص شده از روی تقویم هنگام ایجاد گیفت 
                                DateTime date2 = DateTime.Today; /// تاریخ جاری سیستم 
                                int daysDiff = (expirationDateToSolar - date2).Days; /// تعداد روز های بین تاریخ انقضای انتخاب شده از روی تقویم و تاریخ جاری سیسنم 

                                if (item.expirationTime > 0 && item.giftStatus == GiftStatus.SoldToTheCustomer && item.giftType != GiftType.CheckNotRegistered && dateSaleCustomer.AddDays(item.expirationTime.Value) <= DateTime.Now)
                                {
                                    string dateTime = DateUtility.GetPersianDate(dateSaleCustomer.AddDays(item.expirationTime.Value));
                                    response = new Response()
                                    {
                                        status = 201,
                                        data = item,
                                        message = " تاریخ اعتبار این هدیه تا  " + dateTime + " بوده است اکنون منقضی شده است،آیا میخواهید ادامه دهید؟ " + comment
                                    };
                                }
                                else if ((item.expirationTime == 0 || item.expirationTime == null) && item.giftStatus == GiftStatus.SoldToTheCustomer && item.giftType != GiftType.CheckNotRegistered && expirationDateToSolar <= DateTime.Today)
                                {
                                    string dateTime = DateUtility.GetPersianDate(expirationDateToSolar);
                                    response = new Response()
                                    {
                                        status = 201,
                                        data = item,
                                        message = " تاریخ اعتبار این هدیه تا  " + dateTime + " بوده است اکنون منقضی شده است،آیا میخواهید ادامه دهید؟ " + comment
                                    };
                                }
                                else if (item.expirationTime > 0 && item.giftStatus == GiftStatus.SoldToTheBranch && (item.giftType == GiftType.CheckNotRegistered || item.giftType == GiftType.Cart) && dateSaleBranch.AddDays(item.expirationTime.Value) <= DateTime.Now)
                                {
                                    string dateTime = DateUtility.GetPersianDate(dateSaleBranch.AddDays(item.expirationTime.Value));
                                    response = new Response()
                                    {
                                        status = 201,
                                        data = item,
                                        message = " تاریخ اعتبار این هدیه تا  " + dateTime + " بوده است اکنون منقضی شده است،آیا میخواهید ادامه دهید؟ " + comment
                                    };
                                }
                                else if ((item.expirationTime == 0 || item.expirationTime == null) && item.giftStatus == GiftStatus.SoldToTheBranch && (item.giftType == GiftType.CheckNotRegistered || item.giftType == GiftType.Cart) && expirationDateToSolar <= DateTime.Today)
                                {
                                    string dateTime = DateUtility.GetPersianDate(expirationDateToSolar);
                                    response = new Response()
                                    {
                                        status = 500,
                                        message = " تاریخ اعتبار این هدیه تا  " + dateTime + " بوده است اکنون منقضی شده است. " + comment
                                    };
                                }
                                else
                                {
                                    item.valueToSeparator = Core.ToSeparator(item.value);
                                    item.giftStatusTitle = Enums.GetTitle(item.giftStatus);
                                    item.giftTypeTitle = Enums.GetTitle(item.giftType);
                                    response = new Response()
                                    {
                                        status = 200,
                                        data = item
                                    };
                                }
                            }
                        }
                        else
                        {
                            if (item.giftStatus == GiftStatus.SoldToTheBranch && item.companyIdShopping == null && item.branchShoppingId != user.BranchId && item.giftType == GiftType.FivePercentCard)
                            {
                                response = new Response()
                                {
                                    status = 500,
                                    message = " هدیه ای با این کد به شعبه " + item.branchShopping + " فروخته شده است." + comment
                                };
                            }
                            else
                            {


                                DateTime dateSaleCustomer = item.dateSaleCustomer.GetValueOrDefault();
                                DateTime dateSaleBranch = item.dateSaleBranch.GetValueOrDefault();
                                DateTime expirationDateToSolar = item.expirationDateToSolar.GetValueOrDefault();/// تاریخ مشخص شده از روی تقویم هنگام ایجاد گیفت 

                                if (item.expirationTime > 0 && item.giftStatus == GiftStatus.SoldToTheCustomer && item.giftType != GiftType.UnregisterFivePercentCard && dateSaleCustomer.AddDays(item.expirationTime.Value) <= DateTime.Now)
                                {
                                    string dateTime = DateUtility.GetPersianDate(dateSaleCustomer.AddDays(item.expirationTime.Value));
                                    item.valueToSeparator = Core.ToSeparator(item.value);
                                    item.giftStatusTitle = Enums.GetTitle(item.giftStatus);
                                    item.giftTypeTitle = Enums.GetTitle(item.giftType);
                                    response = new Response()
                                    {
                                        status = 201,
                                        data = item,
                                        message = " تاریخ اعتبار این هدیه تا  " + dateTime + " بوده است اکنون منقضی شده است،آیا میخواهید ادامه دهید؟ " + comment
                                    };
                                }
                                else if ((item.expirationTime == 0 || item.expirationTime == null) && item.giftStatus == GiftStatus.SoldToTheCustomer && item.giftType != GiftType.UnregisterFivePercentCard && expirationDateToSolar <= DateTime.Today)
                                {
                                    string dateTime = DateUtility.GetPersianDate(expirationDateToSolar);
                                    item.valueToSeparator = Core.ToSeparator(item.value);
                                    item.giftStatusTitle = Enums.GetTitle(item.giftStatus);
                                    item.giftTypeTitle = Enums.GetTitle(item.giftType);
                                    response = new Response()
                                    {
                                        status = 201,
                                        data = item,
                                        message = " تاریخ اعتبار این هدیه تا  " + dateTime + " بوده است اکنون منقضی شده است،آیا میخواهید ادامه دهید؟ " + comment
                                    };
                                }
                                else if (item.expirationTime > 0 && item.giftStatus == GiftStatus.SoldToTheBranch && item.giftType == GiftType.UnregisterFivePercentCard && dateSaleBranch.AddDays(item.expirationTime.Value) <= DateTime.Now)
                                {
                                    string dateTime = DateUtility.GetPersianDate(dateSaleBranch.AddDays(item.expirationTime.Value));
                                    item.valueToSeparator = Core.ToSeparator(item.value);
                                    item.giftStatusTitle = Enums.GetTitle(item.giftStatus);
                                    item.giftTypeTitle = Enums.GetTitle(item.giftType);
                                    response = new Response()
                                    {
                                        status = 201,
                                        data = item,
                                        message = " تاریخ اعتبار این هدیه تا  " + dateTime + " بوده است اکنون منقضی شده است،آیا میخواهید ادامه دهید؟ " + comment

                                    };
                                }
                                else if ((item.expirationTime == 0 || item.expirationTime == null) && item.giftStatus == GiftStatus.SoldToTheBranch && item.giftType == GiftType.UnregisterFivePercentCard && expirationDateToSolar <= DateTime.Today)
                                {
                                    string dateTime = DateUtility.GetPersianDate(expirationDateToSolar);
                                    item.valueToSeparator = Core.ToSeparator(item.value);
                                    item.giftStatusTitle = Enums.GetTitle(item.giftStatus);
                                    item.giftTypeTitle = Enums.GetTitle(item.giftType);
                                    response = new Response()
                                    {
                                        status = 201,
                                        data = item,
                                        message = " تاریخ اعتبار این هدیه تا  " + dateTime + " بوده است اکنون منقضی شده است،آیا میخواهید ادامه دهید؟ " + comment

                                    };
                                }
                                else
                                {
                                    item.valueToSeparator = Core.ToSeparator(item.value);
                                    item.giftStatusTitle = Enums.GetTitle(item.giftStatus);
                                    item.giftTypeTitle = Enums.GetTitle(item.giftType);
                                    response = new Response()
                                    {
                                        status = 200,
                                        data = item
                                    };
                                }
                            }
                        }
                    }
                    else
                    {
                        if (item == null)
                        {
                            response = new Response()
                            {
                                status = 500,
                                message = "هدیه ای با این کد وجود ندارد" + comment
                            };
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 500,
                                message = " هدیه ای با این کد در شعبه " + item.branchReceiverCustomer + " به نام مشتری " + item.revocationCustomerName + " استفاده شده است." + comment
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
        /// سوابق کارت هدیه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, gift, giftRegistration")]
        public JsonResult GetLog(int id)
        {
            Response response;
            try
            {
                int userid = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    List<GiftLogViewModel> logList = db.GiftLog.Where(x => x.GiftId == id).Select(x => new GiftLogViewModel()
                    {
                        id = x.Id,
                        createdDateTime = x.CreateDate,
                        createUser = x.CreateUser.FirstName + " " + x.CreateUser.LastName,
                        giftStatus = x.GiftStatus
                    }).ToList();
                    logList.ForEach(x =>
                    {
                        x.createdDate = DateUtility.GetPersianDateTime(x.createdDateTime);
                        x.status = Enums.GetTitle(x.giftStatus);
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
        /// سوابق کارت هدیه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, gift, giftRegistration,solicitorshipgift")]
        public JsonResult GetLogByCode(string code)
        {
            Response response;
            try
            {
                int userid = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    List<GiftLogViewModel> logList = db.GiftLog.Where(x => x.Gift.Code == code).Select(x => new GiftLogViewModel()
                    {
                        id = x.Id,
                        createdDateTime = x.CreateDate,
                        createUser = x.CreateUser.FirstName + " " + x.CreateUser.LastName,
                        giftStatus = x.GiftStatus,
                        giftType = x.Gift.GiftType,
                        branch = x.CreateUser.Branch.Name,
                        customerName = x.Gift.RevocationCustomerName,
                        customerPhoneNumber = x.Gift.RevocationCustomerPhoneNumber,
                        factorNumber = x.Gift.FactorNumber,
                        factorPrice = x.Gift.FactorPrice,
                    }).ToList();
                    logList.ForEach(x =>
                    {
                        x.giftTypeTitle = Enums.GetTitle(x.giftType);
                        x.createdDate = DateUtility.GetPersianDateTime(x.createdDateTime);
                        x.status = Enums.GetTitle(x.giftStatus);
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
        /// پرینت کارت هدیه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, gift, giftRegistration")]
        public ActionResult Print(string code)
        {
            List<string> codeList = code.Split(',').Select(x => x).ToList();
            List<Gift> giftList;
            using (var db = new KiaGalleryContext())
            {
                giftList = db.Gift.Where(x => codeList.Any(y => y == x.Code)).ToList();
            }
            long value = giftList.Sum(x => x.Value);

            StiReport report = new StiReport();
            DataSet dataset = new DataSet("DataSource");
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Row");
            dataTable.Columns.Add("Code");
            dataTable.Columns.Add("Type");
            dataTable.Columns.Add("Price");
            int counter = 0;
            foreach (var item in giftList)
            {
                DataRow row = dataTable.NewRow();
                row["Row"] = ++counter;
                row["Code"] = item.Code;
                row["Type"] = Enums.GetTitle(item.GiftType);
                row["Price"] = Core.ToSeparator(item.Value);
                dataTable.Rows.Add(row);
            }
            dataset.Tables.Add(dataTable);

            report.Load(Server.MapPath("~/Report/Gift/Gift2.mrt"));
            report.Dictionary.Databases.Clear();
            report.ScriptLanguage = StiReportLanguageType.CSharp;
            report.RegData("DataSource", dataset.Tables[0].DefaultView);
            report.Dictionary.Variables["CustomerName"].Value = giftList.FirstOrDefault().BuyerCustomerName;
            report.Dictionary.Variables["CustomerPhoneNumber"].Value = giftList.FirstOrDefault().BuyerCustomerPhoneNumber;
            report.Dictionary.Variables["Date"].Value = DateUtility.GetPersianDate(DateTime.Now);
            report.Dictionary.Variables["PriceTotal"].Value = Core.ToSeparator(value);
            report.Dictionary.Variables["Number"].Value = (1000 + giftList.FirstOrDefault().Id).ToString();
            report.Compile();
            report.Render(false);

            StiReport joinedReport = new StiReport();
            joinedReport.NeedsCompiling = false;
            joinedReport.IsRendered = true;
            joinedReport.RenderedPages.Clear();

            foreach (StiPage page in report.CompiledReport.RenderedPages)
            {
                page.Report = joinedReport;
                page.NewGuid();
                joinedReport.RenderedPages.Add(page);
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
            return new FileStreamResult(stream, "application/pdf");
        }

        /// <summary>
        /// پرینت کارت هدیه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, gift")]
        public ActionResult PrintingHouse(string id)
        {
            List<int> idList = id.Split(',').Select(x => int.Parse(x)).ToList();
            List<GiftPrintViewModel> giftList;
            using (var db = new KiaGalleryContext())
            {
                giftList = db.Gift.Where(x => idList.Any(y => y == x.Id)).OrderBy(x => x.Value).Select(x => new GiftPrintViewModel
                {
                    id = x.Id,
                    code = x.Code,
                    value = x.Value,
                    giftType = x.GiftType
                }).ToList();
            }
            var data = giftList.GroupBy(x => new { x.id, x.code, x.value, x.giftType }).Select(x => new { x.Key.id, x.Key.code, x.Key.value, x.Key.giftType });
            List<StiReport> reports = new List<StiReport>();
            foreach (var gifts in data.GroupBy(x => new { x.value, x.giftType }))
            {
                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Code");
                dataTable.Columns.Add("Image", typeof(byte[]));

                foreach (var item in gifts)
                {
                    DataRow row = dataTable.NewRow();
                    row["Image"] = GetProductFileByte("~/upload/gift/" + item.code + ".jpg");
                    row["Code"] = item.code;
                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);
                StiReport report = new StiReport();
                if (gifts.FirstOrDefault().giftType == GiftType.Cart)
                    report.Load(Server.MapPath("~/Report/gift/giftPrint.mrt"));
                else
                    report.Load(Server.MapPath("~/Report/Gift/CheckPrint.mrt"));
                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.RegData("DataSource", dataset.Tables[0].DefaultView);
                report.Dictionary.Variables["Date"].Value = DateUtility.GetPersianDateTime(DateTime.Now);
                report.Dictionary.Variables["Price"].Value = Core.ToSeparator(gifts.FirstOrDefault().value);
                report.Dictionary.Variables["GiftType"].Value = Enums.GetTitle(gifts.FirstOrDefault().giftType);
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
            return new FileStreamResult(stream, "application/pdf");
        }

        /// <summary>
        /// پرینت بن خرید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, gift")]
        public ActionResult PrintCheck(string id)
        {
            List<int> idList = id.Split(',').Select(x => int.Parse(x)).ToList();
            List<GiftPrintViewModel> giftList;
            using (var db = new KiaGalleryContext())
            {
                giftList = db.Gift.Where(x => idList.Any(y => y == x.Id)).OrderBy(x => x.Value).Select(x => new GiftPrintViewModel
                {
                    id = x.Id,
                    code = x.Code,
                    value = x.Value,
                    giftType = x.GiftType
                }).ToList();
            }
            var data = giftList.GroupBy(x => new { x.id, x.code, x.value, x.giftType }).Select(x => new { x.Key.id, x.Key.code, x.Key.value, x.Key.giftType });
            List<StiReport> reports = new List<StiReport>();
            foreach (var gifts in data.GroupBy(x => new { x.value, x.giftType }))
            {
                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Code");
                dataTable.Columns.Add("Image", typeof(byte[]));

                foreach (var item in gifts)
                {
                    DataRow row = dataTable.NewRow();
                    row["Image"] = GetProductFileByte("~/upload/gift/" + item.code + ".jpg");
                    row["Code"] = item.code;
                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);
                StiReport report = new StiReport();
                report.Load(Server.MapPath("~/Report/Gift/CheckPrint2.mrt"));
                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.RegData("DataSource", dataset.Tables[0].DefaultView);
                report.Dictionary.Variables["Price"].Value = Core.ToSeparator(gifts.FirstOrDefault().value);
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
            return new FileStreamResult(stream, "application/pdf");
        }

        /// <summary>
        /// دریافت آرایه بایت تصویر برای محصول
        /// </summary>
        /// <param name="fileName">نام فایل</param>
        /// <returns>آرایه بایت شده تصویر</returns>
        private byte[] GetProductFileByte(string filePath)
        {
            Image image = Image.FromFile(Server.MapPath(filePath));
            var resizedImage = BitmapUtility.FixedSize(image, 886, 142, true);
            return BitmapUtility.ImageToByteArray(resizedImage);
        }

        /// <summary>
        /// صفحه گزارش گیفت های استفاده شده
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult UsedGiftFactor()
        {
            using (var db = new KiaGalleryContext())
            {
                List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                List<GiftLogViewModel> countlog = db.Gift.Where(x => x.GiftStatus == GiftStatus.Used).Select(x => new GiftLogViewModel()
                {
                    id = x.Id,
                }).ToList();
                List<CompanyListViewModel> companyList = db.Company.Where(x => x.Active == true).Select(x => new CompanyListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                ViewBag.CompanyList = companyList;
                ViewBag.BranchList = branchList;
                ViewBag.dataCount = countlog.Count();
            }
            return View();
        }

        [Authorize(Roles = "admin, solicitorshipGift")]
        public ActionResult SolicitorshipGift()
        {
            return View();
        }

        public ActionResult Export(UsedGiftFactorSearchViewModel model)
        {
            model.fromDateToAc = DateUtility.GetDateTime(model.fromDate).GetValueOrDefault();
            model.toDateToAc = DateUtility.GetDateTime(model.toDate).GetValueOrDefault();
            try
            {
                List<UsedGiftFactorViewModel> list;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Gift.Where(x => x.GiftStatus == GiftStatus.Used);
                    query = query.OrderByDescending(x => x.Id);
                    query = query.Where(x => x.GiftLog.Where(y => y.GiftStatus == GiftStatus.Used && y.CreateDate >= model.fromDateToAc && y.CreateDate <= model.toDateToAc).Count() > 0);

                    list = query.Select(x => new UsedGiftFactorViewModel()
                    {
                        branchShopping = x.BranchShopping.Name,
                        buyerCustomerName = x.BuyerCustomerName,
                        branchReceiverName = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateUser.Branch.Name,
                        buyerCustomerPhoneNumber = x.BuyerCustomerPhoneNumber,
                        revocationCustomerName = x.RevocationCustomerName,
                        revocationCustomerPhoneNumber = x.RevocationCustomerPhoneNumber,
                        factorNumber = x.FactorNumber,
                        factorPrice = x.FactorPrice,
                        code = x.Code,
                        createDate = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateDate,
                    }).ToList();
                }
                list.ForEach(x =>
                {
                    x.factorPriceSeparator = Core.ToSeparator(x.factorPrice);
                    x.persianCreateDate = DateUtility.GetPersianDateTime(x.createDate);
                });
                DataTable tableData = new DataTable();
                tableData.Columns.Add("کد");
                tableData.Columns.Add("شعبه خریدار گیفت");
                tableData.Columns.Add("نام مشتری خریدکننده");
                tableData.Columns.Add("تلفن مشتری خریدکننده");
                tableData.Columns.Add("شعبه دریافت کننده گیفت از مشتری");
                tableData.Columns.Add("نام مشتری باطل کننده");
                tableData.Columns.Add("تلفن مشتری باطل کننده");
                tableData.Columns.Add("شماره فاکتور");
                tableData.Columns.Add("مبلغ فاکتور");
                tableData.Columns.Add("تاریخ ایجاد");
                list.ForEach(x =>
                {
                    DataRow row = tableData.NewRow();
                    row["کد"] = x.code;
                    row["شعبه خریدار گیفت"] = x.branchShopping;
                    row["نام مشتری خریدکننده"] = x.buyerCustomerName;
                    row["تلفن مشتری خریدکننده"] = x.buyerCustomerPhoneNumber;
                    row["شعبه دریافت کننده گیفت از مشتری"] = x.branchReceiverName;
                    row["نام مشتری باطل کننده"] = x.revocationCustomerName;
                    row["تلفن مشتری باطل کننده"] = x.revocationCustomerPhoneNumber;
                    row["شماره فاکتور"] = x.factorNumber;
                    row["مبلغ فاکتور"] = x.factorPriceSeparator;
                    row["تاریخ ایجاد"] = x.persianCreateDate;
                    tableData.Rows.Add(row);
                });
                XLWorkbook wb = new XLWorkbook() { RightToLeft = true };
                wb.Worksheets.Add(tableData, "UsedGiftFactors");
                wb.SaveAs(Server.MapPath("~/Temp/UsedGiftFactors.xlsx"));
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Temp/UsedGiftFactors.xlsx"));
                string fileName = "UsedGiftFactors.xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View();
            }
        }
        public ActionResult DetailedExport(/*UsedGiftFactorSearchViewModel model*/)
        {
            //var convertedFromDate = model.fromDateToAc = DateUtility.GetDateTime(model.fromDate).GetValueOrDefault();
            //var convertedtoDate = model.toDateToAc = DateUtility.GetDateTime(model.toDate).GetValueOrDefault();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Gift.Where(x => x.GiftStatus == GiftStatus.Used);
                    //query = query.Where(x => x.GiftLog.Where(y => y.GiftStatus == GiftStatus.Used && y.CreateDate >= convertedFromDate && y.CreateDate <= convertedtoDate).Count() > 0);

                    var group = query.GroupBy(x => new
                    {
                        x.BranchShopping,
                        x.FactorNumber,
                        x.FactorPrice,
                        x.BuyerCustomerName,
                        x.BuyerCustomerPhoneNumber,
                        x.BranchReceiver,
                        x.RevocationCustomerName,
                        x.RevocationCustomerPhoneNumber
                    });

                    var dataCount = group.Count();
                    var list = group.OrderBy(x => x.Key.FactorNumber).Select(x => new UsedGiftFactorViewModel
                    {
                        branchShopping = x.Key.BranchShopping.Name,
                        giftCount = x.Count(),
                        factorNumber = x.Key.FactorNumber,
                        factorPrice = x.Key.FactorPrice,
                        buyerCustomerName = x.Key.BuyerCustomerName,
                        buyerCustomerPhoneNumber = x.Key.BuyerCustomerPhoneNumber,
                        branchReceiverName = x.FirstOrDefault().GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateUser.Branch.Name,
                        revocationCustomerName = x.Key.RevocationCustomerName,
                        revocationCustomerPhoneNumber = x.Key.RevocationCustomerPhoneNumber,
                        createDate = x.FirstOrDefault().GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateDate,

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.factorPriceSeparator = Core.ToSeparator(x.factorPrice);
                        x.persianCreateDate = DateUtility.GetPersianDateTime(x.createDate);
                    });
                    DataTable tableData = new DataTable();

                    tableData.Columns.Add("تعداد بن ها");
                    tableData.Columns.Add("مبلغ فاکتور");
                    tableData.Columns.Add("شعبه دریافت کننده گیفت از مشتری");
                    tableData.Columns.Add("نام مشتری باطل کننده");
                    tableData.Columns.Add("تلفن مشتری باطل کننده");
                    tableData.Columns.Add("شماره فاکتور");
                    tableData.Columns.Add("تاریخ ابطال");

                    list.ForEach(x =>
                    {
                        DataRow row = tableData.NewRow();
                        row["تعداد بن ها"] = x.giftCount;
                        row["مبلغ فاکتور"] = x.factorPriceSeparator;
                        row["شعبه دریافت کننده گیفت از مشتری"] = x.branchReceiverName;
                        row["نام مشتری باطل کننده"] = x.revocationCustomerName;
                        row["تلفن مشتری باطل کننده"] = x.revocationCustomerPhoneNumber;
                        row["شماره فاکتور"] = x.factorNumber;
                        row["تاریخ ابطال"] = x.persianCreateDate;

                        tableData.Rows.Add(row);
                    });

                    XLWorkbook wb = new XLWorkbook() { RightToLeft = true };
                    wb.Worksheets.Add(tableData, "UsedGiftFactors");
                    wb.SaveAs(Server.MapPath("~/Temp/UsedGiftFactors.xlsx"));
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Temp/UsedGiftFactors.xlsx"));
                    string fileName = "UsedGiftFactors.xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View();
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult ExpiredGift()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true && x.BranchType == BranchType.Branch).OrderBy(x => x.Order).Select(x => x).ToList();
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public JsonResult GetExpiredGift(GiftSearchViewModel model)
        {
            var currentUser = GetAuthenticatedUser();
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var query = db.Gift.Where(x => x.GiftStatus == GiftStatus.Used && DbFunctions.AddDays(x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.SoldToTheCustomer).CreateDate, 180) <= x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateDate);
                    if (model.branchId != null)
                    {
                        query = query.Where(x => x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateUser.BranchId == model.branchId);
                    }
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new GiftViewModel
                    {
                        id = x.Id,
                        code = x.Code,
                        giftType = x.GiftType,
                        giftStatus = x.GiftStatus,
                        branchReceiver = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateUser.Branch.Name,
                        value = x.Value,
                        expirationTime = x.ExpirationTime.Value,
                        buyerCustomerName = x.BuyerCustomerName,
                        buyerCustomerPhoneNumber = x.BuyerCustomerPhoneNumber,
                        revocationCustomerName = x.RevocationCustomerName,
                        revocationCustomerPhoneNumber = x.RevocationCustomerPhoneNumber,
                        description = x.Description,
                        date = x.GiftLog.FirstOrDefault(y => y.GiftStatus == GiftStatus.Used).CreateDate,

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.date);
                        x.giftTypeTitle = Enums.GetTitle(x.giftType);

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
    }
}