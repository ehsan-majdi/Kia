using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using KiaGallery.Web.SmsService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// سیستم ثبت مشتریان وفادار
    /// </summary>
    public class CustomerLoyalityController : BaseController
    {

        /// <summary>
        /// صفحه اصلی و مشاهده لیست مشتریان
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,loyalCustomer")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// صفحه مشاهده مشخصات مشتریان  به صورت جزیی که قابل مشاهده برای شعب می باشد
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,loyalCustomer,loyalityFactor")]
        public ActionResult Factor(int? id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// گزارش فاکتور مشتریان وفادار
        /// </summary>
        /// <returns></returns>
        public ActionResult BranchFactorReport()
        {

            return View();
        }
        /// <summary>
        /// گرفتن لیست گزارش فاکتور های مشتریان وفادار
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBranchFactorReport(CustomerFactorViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var query = db.CustomerFactor.Select(x => x);

                    var list = query.GroupBy(x => x.Branch).Select(x => new CustomerFactorViewModel
                    {
                        branchName = x.Key.Name,
                        count = x.Count(),
                        returnCount = x.Where(y => y.PurchaseType == PurchaseType.Return).Count(),
                        branchType = x.Key.BranchType,

                    }).OrderBy(x => x.branchType).ToList();
                    var yesterdayList = query.Where(x => x.Date < DateTime.Today && x.Date >= DbFunctions.AddDays(DateTime.Today, -1)).GroupBy(x => x.Branch).Select(x => new CustomerFactorViewModel
                    {
                        branchName = x.Key.Name,
                        count = x.Count(),
                        returnCount = x.Where(y => y.PurchaseType == PurchaseType.Return).Count(),
                        branchType = x.Key.BranchType,

                    }).OrderBy(x => x.branchType).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            yesterdayList = yesterdayList
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
        /// صفحه ویرایش مشتری
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,loyalCustomer,loyalityFactor")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [Authorize(Roles = "admin,loyalCustomer,loyalityFactor")]
        public ActionResult Add()
        {
            return View("Edit");
        }
        public JsonResult EditCustomer(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var data = db.CustomerLoyality.Where(x => x.Id == id).Select(x => new
                    {
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        phoneNumber = x.PhoneNumber,
                        customerId = x.Id

                    }).Single();
                    response = new Response()
                    {
                        status = 200,
                        data = data
                    };
                }

            }
            catch (Exception ex)
            {

                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin,loyalCustomer,loyalityFactor")]
        public JsonResult Save(CustomerLoyalityViewModel model)
        {

            Response response;
            var currentUser = GetAuthenticatedUser();
            var date = DateUtility.GetDateTime(model.persianDate);
            var returnDate = DateUtility.GetDateTime(model.persianReturnDate);
            if (date > DateTime.Now)
            {
                response = new Response()
                {
                    status = 500,
                    message = "تاریخ انتخاب شده بزرگتر از تاریخ امروز می باشد.",
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var personel = db.Person.Where(x => x.MobileNumber == model.phoneNumber).Count();
                    if (personel > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این شماره متعلق به پرسنل کیا گالری میباشد که شامل مشتریان وفادار نمیشوند.",
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                if (!ValidateMobileNumber(model.phoneNumber))
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "تلفن همراه وارد شده صحیح نیست. لطفا تلفن همراه را در قالب 09123456789 وارد نمایید. \n مثال:0912123456789",
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var SilverCardValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeySilverCardValue)?.Value;
                        var GoldenCardValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldenCardValue)?.Value;
                        var PlatinumCardValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyPlatinumCardValue)?.Value;

                        var SilverCardLevel = db.Settings.SingleOrDefault(x => x.Key == Settings.KeySilverCardLevel)?.Value;
                        var GoldenCardLevel = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldenCardLevel)?.Value;
                        var PlatinumCardLevel = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyPlatinumCardLevel)?.Value;
                        if (model.customerId > 0)
                        {
                            var entity = db.CustomerLoyality.Single(x => x.Id == model.customerId);
                            entity.FirstName = model.firstName;
                            entity.LastName = model.lastName;
                            entity.PhoneNumber = model.phoneNumber;
                            db.SaveChanges();
                            response = new Response()
                            {
                                status = 200,
                                message = "ویرایش اطلاعات با موفقیت انجام شد."
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (model.id > 0)
                            {
                                if (model.purchaseType == PurchaseType.Return)
                                {
                                    var entity = db.CustomerFactor.Single(x => x.Id == model.id);
                                    entity.ReturnDate = returnDate;
                                    entity.ProductCode = model.productCode;
                                    entity.FactorNumber = model.factorNumber;
                                    entity.FactorPrice = model.factorPrice;
                                    entity.PurchaseType = model.purchaseType;
                                    entity.Ip = Request.UserHostAddress;
                                    entity.ModifyUserId = currentUser.Id;
                                    entity.ModifyDate = DateTime.Now;
                                    entity.CustomerLoyality.FirstName = model.firstName;
                                    entity.CustomerLoyality.LastName = model.lastName;
                                    entity.CustomerLoyality.PhoneNumber = model.phoneNumber;
                                    db.SaveChanges();
                                    response = new Response()
                                    {
                                        status = 200,
                                        message = "ویرایش اطلاعات با موفقیت انجام شد."
                                    };
                                }
                                else
                                {
                                    var entity = db.CustomerFactor.Single(x => x.Id == model.id);
                                    entity.Date = date.Value;
                                    entity.ProductCode = model.productCode;
                                    entity.FactorNumber = model.factorNumber;
                                    entity.FactorPrice = model.factorPrice;
                                    entity.FactorWeight = model.factorWeight;
                                    entity.PurchaseType = model.purchaseType;
                                    entity.Ip = Request.UserHostAddress;
                                    entity.ModifyUserId = currentUser.Id;
                                    entity.ModifyDate = DateTime.Now;
                                    entity.CustomerLoyality.FirstName = model.firstName;
                                    entity.CustomerLoyality.LastName = model.lastName;
                                    entity.CustomerLoyality.PhoneNumber = model.phoneNumber;
                                    db.SaveChanges();
                                    response = new Response()
                                    {
                                        status = 200,
                                        message = "ویرایش اطلاعات با موفقیت انجام شد."
                                    };
                                }
                            }
                            else
                            {
                                var entity = db.CustomerLoyality.SingleOrDefault(x => x.PhoneNumber == model.phoneNumber);
                                //double setting = 1;
                                if (entity != null)
                                {
                                    if (model.purchaseType == PurchaseType.Return)
                                    {
                                        var factorInfo = db.CustomerFactor.Single(x => x.Id == model.hiddenId && x.CustomerLoyality.PhoneNumber == model.phoneNumber);
                                        var productCode = db.CustomerFactor.Single(x => x.Id == model.hiddenId).ProductCode.Split('-');
                                        var codes = model.productCode.Split('-');
                                        foreach (var code in codes)
                                        {
                                            if (productCode.Count(v => v == code) > 0)
                                            {
                                                productCode = productCode.Where(x => x != code).ToArray();
                                            }
                                            else
                                            {
                                                response = new Response()
                                                {
                                                    status = 404,
                                                    message = "کد کالا یافت نشد یامحصولی با این کد قبلا مرجوع شده."
                                                };
                                                return Json(response, JsonRequestBehavior.AllowGet);
                                            }
                                            if (productCode.Count(v => v == code) == 0)
                                            {
                                                productCode = productCode.Where(x => x != code).ToArray();
                                            }
                                            else
                                            {
                                                response = new Response()
                                                {
                                                    status = 404,
                                                    message = "کدکالا وارد نشده است."
                                                };
                                                return Json(response, JsonRequestBehavior.AllowGet);
                                            }
                                        }
                                        string dash = "-";
                                        string stringProductCode;
                                        stringProductCode = string.Join(dash, productCode);
                                        var returnFactor = new CustomerFactor()
                                        {
                                            Date = factorInfo.Date,
                                            ReturnDate = returnDate,
                                            ProductCode = model.productCode,
                                            BranchId = currentUser.BranchId.Value,
                                            CustomerLoyalityId = entity.Id,
                                            FactorNumber = model.factorNumber,
                                            FactorPrice = model.factorPrice,
                                            PurchaseType = model.purchaseType,
                                            CreateUserId = currentUser.Id,
                                            ModifyUserId = currentUser.Id,
                                            CreateDate = DateTime.Now,
                                            ModifyDate = DateTime.Now,
                                            Ip = Request.UserHostAddress
                                        };
                                        db.CustomerFactor.Add(returnFactor);
                                        factorInfo.CustomerLoyality.FirstName = model.firstName;
                                        factorInfo.CustomerLoyality.LastName = model.lastName;
                                        factorInfo.ProductCode = stringProductCode;
                                        factorInfo.FactorNumber = model.factorNumber;
                                        factorInfo.FactorPrice = factorInfo.FactorPrice;
                                        factorInfo.PurchaseType = PurchaseType.Buy;
                                        factorInfo.Ip = Request.UserHostAddress;
                                        factorInfo.ModifyUserId = currentUser.Id;
                                        factorInfo.ModifyDate = DateTime.Now;
                                        factorInfo.CustomerLoyality.PhoneNumber = model.phoneNumber;
                                        response = new Response()
                                        {
                                            status = 200,
                                            message = "ثبت اطلاعات با موفقیت انجام شد."
                                        };
                                        db.SaveChanges();
                                    }
                                    else
                                    {

                                        var factor = new CustomerFactor()
                                        {
                                            Date = date.Value,
                                            ProductCode = model.productCode,
                                            BranchId = currentUser.BranchId.Value,
                                            CustomerLoyalityId = entity.Id,
                                            FactorNumber = model.factorNumber,
                                            FactorPrice = model.factorPrice,
                                            FactorWeight = model.factorWeight,
                                            PurchaseType = model.purchaseType,
                                            CreateUserId = currentUser.Id,
                                            ModifyUserId = currentUser.Id,
                                            CreateDate = DateTime.Now,
                                            ModifyDate = DateTime.Now,
                                            Ip = Request.UserHostAddress,
                                        };
                                        //entity.Credit = long.Parse(Math.Round((model.factorPrice * setting / 100) / 1000).ToString());
                                        Random random = new Random(Guid.NewGuid().GetHashCode());
                                        int number = random.Next(10000, 99999);
                                        var survey = new CustomerSurvey()
                                        {
                                            Code = number.ToString(),
                                            CustomerFactor = factor,
                                            CreateDate = DateTime.Now,
                                            CreateUserId = currentUser.Id,
                                            Ip = Request.UserHostAddress
                                        };
                                        //Task.Factory.StartNew(() =>
                                        //{
                                        //NikSmsWebServiceClient.SendSmsNik("srvey.kia-gallery.com/" + number.ToString(), "09354047788");
                                        //    NikSmsWebServiceClient.SendSmsNik("survey.kia-gallery.com/"+ number.ToString(), "09193121247");
                                        //    NikSmsWebServiceClient.SendSmsNik("www.kia-gallery.com/" + number.ToString(), model.phoneNumber);
                                        //});
                                        db.CustomerSurvey.Add(survey);
                                        response = new Response()
                                        {
                                            status = 200,
                                            message = "ثبت اطلاعات با موفقیت انجام شد."
                                        };
                                        db.CustomerFactor.Add(factor);
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    //if (entity.CustomerCardLevel == CustomerCardLevel.Silver)
                                    //{
                                    //    setting = double.Parse(SilverCardValue);
                                    //}
                                    //if (entity.CustomerCardLevel == CustomerCardLevel.Gold)
                                    //{
                                    //    setting = double.Parse(GoldenCardValue);
                                    //}
                                    //if (entity.CustomerCardLevel == CustomerCardLevel.Platinum)
                                    //{
                                    //    setting = double.Parse(PlatinumCardValue);
                                    //}

                                    var loyality = new CustomerLoyality()
                                    {
                                        FirstName = model.firstName,
                                        LastName = model.lastName,
                                        //Credit = long.Parse(Math.Round((model.factorPrice * setting / 100) / 1000).ToString()),
                                        PhoneNumber = model.phoneNumber,
                                        Date = DateTime.Now,
                                        CreateUserId = currentUser.Id,
                                        ModifyUserId = currentUser.Id,
                                        CreateDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        Ip = Request.UserHostAddress,
                                    };
                                    var factor = new CustomerFactor()
                                    {
                                        Date = date.Value,
                                        ProductCode = model.productCode,
                                        ReturnDate = returnDate,
                                        BranchId = currentUser.BranchId.Value,
                                        CustomerLoyality = loyality,
                                        FactorNumber = model.factorNumber,
                                        FactorPrice = model.factorPrice,
                                        FactorWeight = model.factorWeight,
                                        PurchaseType = model.purchaseType,
                                        CreateUserId = currentUser.Id,
                                        ModifyUserId = currentUser.Id,
                                        CreateDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        Ip = Request.UserHostAddress
                                    };

                                    Random random = new Random(Guid.NewGuid().GetHashCode());
                                    int number = random.Next(10000, 99999);
                                    var survey = new CustomerSurvey()
                                    {
                                        Code = number.ToString(),
                                        CustomerFactor = factor,
                                        CreateDate = DateTime.Now,
                                        CreateUserId = currentUser.Id,
                                        Ip = Request.UserHostAddress

                                    };
                                    //Task.Factory.StartNew(() =>
                                    //{
                                    //    NikSmsWebServiceClient.SendSmsNik("srvey.kia-gallery.com/" + number.ToString(), "09354047788");
                                    //    NikSmsWebServiceClient.SendSmsNik("survey.kia-gallery.com/" + number.ToString(), "09193121247");
                                    //});
                                    db.CustomerSurvey.Add(survey);
                                    response = new Response()
                                    {
                                        status = 200,
                                        message = "ثبت اطلاعات با موفقیت انجام شد."
                                    };
                                    db.CustomerLoyality.Add(loyality);

                                    db.CustomerFactor.Add(factor);
                                    db.SaveChanges();

                                    //if (loyality.Credit >= long.Parse(SilverCardLevel))
                                    //{
                                    //    var customerCardlevelLog = new CustomerCreditLog()
                                    //    {
                                    //        CustomerId = loyality.Id,
                                    //        Credit = long.Parse(Math.Round((model.factorPrice * setting / 100) / 1000).ToString()),
                                    //        CustomerCardLevel = CustomerCardLevel.Silver
                                    //    };
                                    //}
                                    //else if (loyality.Credit >= long.Parse(GoldenCardLevel))
                                    //{
                                    //    var customerCardlevelLog = new CustomerCreditLog()
                                    //    {
                                    //        CustomerId = loyality.Id,
                                    //        Credit = long.Parse(Math.Round((model.factorPrice * setting / 100) / 1000).ToString()),
                                    //        CustomerCardLevel = CustomerCardLevel.Gold
                                    //    };
                                    //}
                                    //else if (loyality.Credit >= long.Parse(PlatinumCardLevel))
                                    //{
                                    //    var customerCardlevelLog = new CustomerCreditLog()
                                    //    {
                                    //        CustomerId = loyality.Id,
                                    //        Credit = long.Parse(Math.Round((model.factorPrice * setting / 100) / 1000).ToString()),
                                    //        CustomerCardLevel = CustomerCardLevel.Platinum
                                    //    };
                                    //}
                                    //else
                                    //{
                                    //    var customerCardlevelLog = new CustomerCreditLog()
                                    //    {
                                    //        CustomerId = loyality.Id,
                                    //        Credit = long.Parse(Math.Round((model.factorPrice * setting / 100) / 1000).ToString()),
                                    //        CustomerCardLevel = CustomerCardLevel.None
                                    //    };
                                    //}
                                }
                            }
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
        [Authorize(Roles = "admin,loyalCustomer,loyalityFactor")]
        public JsonResult Load(int id)
        {
            var currentUser = GetAuthenticatedUser();
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.CustomerFactor.FirstOrDefault(x => x.Id == id);
                    if (!User.IsInRole("admin") && !User.IsInRole("loyalCustomer"))
                    {
                        entity = db.CustomerFactor.FirstOrDefault(x => x.BranchId == currentUser.BranchId && x.Id == id);
                    }
                    if (entity != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new CustomerLoyalityViewModel()
                            {
                                id = entity.Id,
                                productCode = entity.ProductCode,
                                persianDate = DateUtility.GetPersianDate(entity.Date),
                                persianReturnDate = DateUtility.GetPersianDate(entity.ReturnDate),
                                firstName = entity.CustomerLoyality.FirstName,
                                lastName = entity.CustomerLoyality.LastName,
                                phoneNumber = entity.CustomerLoyality.PhoneNumber,
                                branchId = entity.BranchId,
                                factorPrice = entity.FactorPrice,
                                factorWeight = entity.FactorWeight,
                                factorNumber = entity.FactorNumber,
                                purchaseType = entity.PurchaseType,
                            }
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "موردی یافت نشد."
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
        [Authorize(Roles = "admin,loyalCustomer,loyalityFactor")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var item = db.CustomerFactor.Find(id);
                    var surveyList = item.CustomerSurveyList.ToList();

                    foreach (var entity in surveyList)
                    {
                        entity.CustomerFactorId = null;
                    }

                    db.CustomerFactor.Remove(item);
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "فاکتور با موفقیت حذف شد."
                };
            }
            catch (Exception ex)
            {

                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin,loyalCustomer")]
        public JsonResult Search(CustomerLoyalitySearchViewModel model)
        {
            var currentUser = GetAuthenticatedUser();
            Response response;
            int dataCount;
            try
            {
                var customerCount = 0;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CustomerLoyality.Select(x => x);
                    if (User.IsInRole("admin"))
                    {
                        customerCount = query.Count();

                    }
                    if (model.goal)
                    {
                        query = query.Where(x => x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Buy).Sum(y => y.FactorPrice) - ((long?)x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Return).Sum(y => y.FactorPrice) ?? 0) >= model.priceGoal || x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Buy).Count() >= model.countGoal);
                    }
                    if (currentUser.BranchType == BranchType.Solicitorship)
                    {
                        query = query.Where(x => x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Buy).Any(y => y.BranchId == currentUser.BranchId));
                    }
                    if (model.factorPrice != null && model.factorPrice > 0)
                    {
                        query = query.Where(x => x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Buy).Sum(y => y.FactorPrice) - ((long?)x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Return).Sum(y => y.FactorPrice) ?? 0) >= model.factorPrice);
                    }
                    if (model.factorCount != null && model.factorCount > 0)
                    {
                        query = query.Where(x => x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Buy).Count() >= model.factorCount);
                    };


                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Buy).Sum(y => y.FactorPrice) - ((long?)x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Return).Sum(y => y.FactorPrice) ?? 0)).Skip(model.page * model.count).Take(model.count);

                    var list = query.Select(x => new CustomerLoyalitySearchViewModel
                    {
                        id = x.Id,
                        fullName = x.FirstName + " " + x.LastName,
                        phoneNumber = x.PhoneNumber,
                        returnFactorPrice = x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Return).Sum(y => y.FactorPrice),
                        factorPrice = x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Buy).Sum(y => y.FactorPrice),
                        factorCount = x.CustomerFactorList.Where(y => y.PurchaseType == PurchaseType.Buy).Count(),
                        branchName = x.CustomerFactorList.Select(y => y.Branch.Name).FirstOrDefault(),
                    }).ToList();
                    //list.ForEach(x =>
                    //{
                    //    x.separateFactorPrice = Core.ToSeparator(x.factorPrice);
                    //});

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            customerCount = Core.ToSeparator(customerCount),
                            //factorPrice = list.Sum(x => x.factorPrice),
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
        ///برگرداندن لیست مشخصات مشتریان وفادار،مشاهده مشتریان هر شعب توسط خود آن شعب 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,loyalCustomer,loyalityFactor")]
        public JsonResult SearchDetail(CustomerFactorViewModel model)
        {
            Response response;
            int dataCount;

            var currentUser = GetAuthenticatedUser();
            try
            {
                long priceSum = 0;
                var countSum = 0;
                decimal? branchWeightSum = 0;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CustomerFactor.Select(x => x);
                    if (User.IsInRole("admin"))
                    {
                        priceSum = query.Where(x => x.PurchaseType == PurchaseType.Buy).Sum(x => x.FactorPrice) - ((long?)query.Where(y => y.PurchaseType == PurchaseType.Return).Sum(y => y.FactorPrice) ?? 0);
                        countSum = query.Where(x => x.PurchaseType == PurchaseType.Buy).Count() - query.Where(y => y.PurchaseType == PurchaseType.Return).Count();
                        branchWeightSum = query.Sum(x => x.FactorWeight);
                    }
                    else
                    {
                        branchWeightSum = query.Where(x => DbFunctions.TruncateTime(x.Date) == DateTime.Today && x.BranchId == currentUser.BranchId).Sum(x => x.FactorWeight) ?? 0;
                    }
                    if (!User.IsInRole("admin") && !User.IsInRole("loyalCustomer") && currentUser.BranchType == BranchType.Branch)
                    {
                        query = query.Where(x => x.BranchId == currentUser.BranchId);
                    }
                    if (!User.IsInRole("admin") && currentUser.BranchType == BranchType.Solicitorship)
                    {
                        query = query.Where(x => x.BranchId == currentUser.BranchId);
                    }
                    if (model.id != null && model.id > 0)
                    {
                        query = query.Where(x => x.CustomerLoyality.Id == model.id);
                    }
                    if (!string.IsNullOrEmpty(model.term))
                    {
                        query = query.Where(x => x.CustomerLoyality.PhoneNumber.Contains(model.term.Trim()) || x.CustomerLoyality.FirstName.Contains(model.term.Trim()) || x.CustomerLoyality.LastName.Contains(model.term.Trim()) || x.FactorNumber.Contains(model.term.Trim()));
                    }
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Date).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new CustomerLoyalityViewModel
                    {
                        id = x.Id,
                        fullName = x.CustomerLoyality.FirstName + " " + x.CustomerLoyality.LastName,
                        phoneNumber = x.CustomerLoyality.PhoneNumber,
                        branchId = x.BranchId,
                        branchName = x.Branch.Name,
                        date = x.Date,
                        factorPrice = x.FactorPrice,
                        factorNumber = x.FactorNumber,
                        purchaseType = x.PurchaseType,
                        returnDate = x.ReturnDate,
                        factorWeight = x.FactorWeight
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.date);
                        x.persianReturnDate = DateUtility.GetPersianDate(x.returnDate);
                        x.separateFactorPrice = Core.ToSeparator(x.factorPrice);
                        x.purchaseTypeTitle = Enums.GetTitle(x.purchaseType);
                    });

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            priceSum = Core.ToSeparator(priceSum),
                            countSum = Core.ToSeparator(countSum),
                            branchWeightSum = branchWeightSum,
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
        [Authorize(Roles = "admin,loyalCustomer,loyalityFactor")]
        public JsonResult GetCustomerInfo(string phoneNumber)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var information = db.CustomerLoyality.Where(x => x.PhoneNumber == phoneNumber).Select(x => new
                    {
                        firstName = x.FirstName,
                        lastName = x.LastName,

                    }).SingleOrDefault();
                    if (information != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = information
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "مشتری یافت نشد لطفا نام و نام خانوادگی مشتری جدید را وارد کنید."
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
        [Authorize(Roles = "admin,loyalCustomer,loyalityFactor")]
        public JsonResult CustomerInfoAutoCompelete(string term)
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var list = db.CustomerLoyality.Where(x => x.PhoneNumber.Contains(term) || x.FirstName.Contains(term) || x.LastName.Contains(term)).Select(x => new SearchAutoCompeleteViewModel
                    {
                        phoneNumber = x.PhoneNumber,
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        birthDate = x.BirthDate != null ? x.BirthDate : DateTime.Now,
                        mariageDate = x.MariageDate != null ? x.MariageDate : DateTime.Now,

                    }).OrderBy(x => x.phoneNumber).Take(3).ToList();
                    list.ForEach(x =>
                    {
                        x.persianBirthDate = DateUtility.GetPersianDate(x.birthDate);
                        x.persianMarriageDate = DateUtility.GetPersianDate(x.mariageDate);
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
        public JsonResult SearchAutoCompelete(string term)
        {
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CustomerFactor.Where(x => x.PurchaseType == PurchaseType.Buy);
                    if (currentUser.BranchType == BranchType.Solicitorship)
                    {
                        query = query.Where(x => x.BranchId == currentUser.BranchId);
                    }
                    var date = DateTime.Today.AddMonths(-1);
                    var list = query.Where(x => /*DbFunctions.TruncateTime(x.Date) >= date &&*/ (x.FactorNumber.Contains(term) || x.CustomerLoyality.FirstName.Contains(term) || x.CustomerLoyality.LastName.Contains(term) || x.CustomerLoyality.PhoneNumber.Contains(term) || x.ProductCode.Contains(term))).Select(x => new SearchAutoCompeleteViewModel()
                    {
                        id = x.Id,
                        phoneNumber = x.CustomerLoyality.PhoneNumber,
                        firstName = x.CustomerLoyality.FirstName,
                        lastName = x.CustomerLoyality.LastName,
                        productCode = x.ProductCode,
                        factorNumber = x.FactorNumber,
                        branchName = x.Branch.Name,
                        date = x.Date,
                        expired = DbFunctions.AddDays(x.Date, 30) > DateTime.Today
                    }).Take(10).ToList();
                    list.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.date);
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
        public JsonResult SearchLocationAutoCompelete(string term)
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var list = db.CustomerLocation.Where(x => x.Address.Contains(term)).Select(x => new SearchAutoCompeleteViewModel
                    {
                        address = x.Address,
                        
                    }).OrderBy(x => x.address).Take(3).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return Json("[]", JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult ExpiredCustomer()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true && x.BranchType == BranchType.Branch).OrderBy(x => x.Order).Select(x => x).ToList();
            }
            return View();
        }

        [Authorize(Roles = "admin,loyalCustomer")]
        public JsonResult GetExpiredCustomer(CustomerLoyalitySearchViewModel model)
        {
            var currentUser = GetAuthenticatedUser();
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CustomerFactor.Where(x => x.PurchaseType == PurchaseType.Return && DbFunctions.AddDays(x.Date, 30) < x.ReturnDate);
                    if (model.branchId != null)
                    {
                        query = query.Where(x => x.BranchId == model.branchId);
                    }
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new CustomerLoyalityViewModel
                    {
                        id = x.Id,
                        fullName = x.CustomerLoyality.FirstName + " " + x.CustomerLoyality.LastName,
                        phoneNumber = x.CustomerLoyality.PhoneNumber,
                        branchId = x.BranchId,
                        branchName = x.Branch.Name,
                        date = x.Date,
                        returnDate = x.ReturnDate,
                        factorPrice = x.FactorPrice,
                        factorNumber = x.FactorNumber,
                        purchaseType = x.PurchaseType
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.date);
                        x.persianReturnDate = DateUtility.GetPersianDate(x.returnDate);
                        x.separateFactorPrice = Core.ToSeparator(x.factorPrice);
                        x.purchaseTypeTitle = Enums.GetTitle(x.purchaseType);
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
        /// بررسی شماره تلفن همراه
        /// </summary>
        /// <param name="mobileNumber">شماره تلفن همراه</param>
        /// <returns>نتیجه بررسی تلفن وارد شده</returns>
        public static bool ValidateMobileNumber(string mobileNumber)
        {
            if (string.IsNullOrEmpty(mobileNumber)) return false;

            if (mobileNumber.Length != 11) return false;

            if (!mobileNumber.StartsWith("09")) return false;

            return true;
        }

    }
}