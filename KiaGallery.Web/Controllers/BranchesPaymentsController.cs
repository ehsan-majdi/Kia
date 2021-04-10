using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.BranchesPayments;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Telegram.Bot;
using System.IO;
using Stimulsoft.Report.Export;
using System.Data;
using Stimulsoft.Report;
using System.Text;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر پرداخت شعب
    /// </summary>
    public class BranchesPaymentsController : BaseController
    {
        private TelegramBotClient Bot;
        /// <summary>
        /// پرداخت شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPayments")]
        public ActionResult Index()
        {
            List<BranchModel> branchList;
            using (var db = new KiaGalleryContext())
            {
                branchList = db.Branch.Where(x => x.City.Name != "شهر تهران").Select(x => new BranchModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
            }


            ViewBag.BranchList = branchList;
            return View();
        }

        /// <summary>
        /// پرداخت شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsReturned")]
        public ActionResult Returned()
        {
            List<BranchModel> branchList;
            using (var db = new KiaGalleryContext())
            {
                branchList = db.Branch.Where(x => x.City.Name != "شهر تهران").OrderBy(x => x.CityId).Select(x => new BranchModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
            }
            ViewBag.BranchList = branchList;
            return View();
        }
        /// <summary>
        /// پرداخت شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDeposits")]
        public ActionResult Deposits()
        {
            List<BranchModel> branchList;
            using (var db = new KiaGalleryContext())
            {
                branchList = db.Branch.Where(x => x.City.Name != "شهر تهران").OrderBy(x => x.CityId).Select(x => new BranchModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
            }
            ViewBag.BranchList = branchList;
            return View();
        }
        /// <summary>
        /// پرداخت شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsSale")]
        public ActionResult Sale()
        {
            List<BranchModel> branchList;
            using (var db = new KiaGalleryContext())
            {
                branchList = db.Branch.Where(x => x.City.Name != "شهر تهران").OrderBy(x => x.CityId).Select(x => new BranchModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
            }
            ViewBag.BranchList = branchList;
            return View();
        }
        /// <summary>
        /// پرداخت شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDifferentReturns")]
        public ActionResult DifferentReturns()
        {
            List<BranchModel> branchList;
            using (var db = new KiaGalleryContext())
            {
                branchList = db.Branch.Where(x => x.City.Name != "شهر تهران").OrderBy(x => x.CityId).Select(x => new BranchModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
            }
            ViewBag.BranchList = branchList;
            return View();
        }
        /// <summary>
        /// فروش متفرقه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDifferentSale")]
        public ActionResult DifferentSale()
        {
            List<BranchModel> branchList;
            using (var db = new KiaGalleryContext())
            {
                branchList = db.Branch.Where(x => x.City.Name != "شهر تهران").OrderBy(x => x.CityId).Select(x => new BranchModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
            }
            ViewBag.BranchList = branchList;
            return View();
        }
        /// <summary>
        /// مرجوعی طلا متفرقه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDifferentGoldReturns")]
        public ActionResult DifferentGoldReturns()
        {
            List<BranchModel> branchList;
            using (var db = new KiaGalleryContext())
            {
                branchList = db.Branch.Where(x => x.City.Name != "شهر تهران").OrderBy(x => x.CityId).Select(x => new BranchModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
            }
            ViewBag.BranchList = branchList;
            return View();
        }
        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="Id">ردیف شعبه</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPayments")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش فاکتور پرداختی";
            return View();
        }
        /// <summary>
        /// شعبه جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPayments")]
        public ActionResult Add()
        {
            ViewBag.Title = "فاکتور پرداختی جدید";
            return View("Edit");
        }
        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="Id">ردیف شعبه</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsReturned")]
        public ActionResult EditReturned(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش فاکتور پرداختی";
            return View();
        }
        /// <summary>
        /// شعبه جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsReturned")]
        public ActionResult AddReturned()
        {
            ViewBag.Title = "فاکتور پرداختی جدید";
            return View("EditReturned");
        }


        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="Id">ردیف شعبه</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDeposits")]
        public ActionResult EditDeposits(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش فاکتور واریزی";
            return View();
        }
        /// <summary>
        /// شعبه جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDeposits")]
        public ActionResult AddDeposits()
        {
            ViewBag.Title = "فاکتور واریزی جدید";
            return View("EditDeposits");
        }

        /// <summary>
        /// ویرایش فروش
        /// </summary>
        /// <param name="Id">ردیف فروش</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsSale")]
        public ActionResult EditSale(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش فاکتور فروش";
            return View();
        }
        /// <summary>
        /// فروش جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsSale")]
        public ActionResult AddSale()
        {
            ViewBag.Title = "فاکتور فروش جدید";
            return View("EditSale");
        }

        /// <summary>
        /// ویرایش مرجوعی متفرقه
        /// </summary>
        /// <param name="Id">ردیف مرجوعی متفرقه</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDifferentReturns")]
        public ActionResult EditDifferentReturns(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش فاکتور مرجوعی متفرقه";
            return View();
        }
        /// <summary>
        /// مرجوعی متفرقه جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDifferentReturns")]
        public ActionResult AddDifferentReturns()
        {
            ViewBag.Title = "فاکتور مرجوعی متفرقه جدید";
            return View("EditDifferentReturns");
        }

        /// <summary>
        /// ویرایش فروش متفرقه
        /// </summary>
        /// <param name="Id">ردیف فروش متفرقه</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDifferentSale")]
        public ActionResult EditDifferentSale(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش فاکتور فروش متفرقه";
            return View();
        }
        /// <summary>
        /// فروش متفرقه جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDifferentSale")]
        public ActionResult AddDifferentSale()
        {
            ViewBag.Title = "فاکتور فروش متفرقه جدید";
            return View("EditDifferentSale");
        }

        /// <summary>
        /// ویرایش مرجوعی طلا متفرقه
        /// </summary>
        /// <param name="Id">ردیف مرجوعی طلا متفرقه</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDifferentGoldReturns")]
        public ActionResult EditDifferentGoldReturns(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش فاکتور مرجوعی طلا متفرقه";
            return View();
        }
        /// <summary>
        /// مرجوعی طلا متفرقه جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPaymentsDifferentGoldReturns")]
        public ActionResult AddDifferentGoldReturns()
        {
            ViewBag.Title = "فاکتور مرجوعی طلا متفرقه جدید";
            return View("EditDifferentGoldReturns");
        }

        /// <summary>
        /// ذخیره شعبه
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات شعبه</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, branchesPayments, branchesPaymentsDeposits, branchesPaymentsReturned, branchesPaymentsSale, branchesPaymentsDifferentReturns, branchesPaymentsDifferentSale, branchesPaymentsDifferentGoldReturns")]
        public JsonResult Save(BranchesPaymentsViewModel model)
        {
            Response response;
            try
            {
                BranchesPayments item = new BranchesPayments();
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();
                    if (model.id > 0 && model.id != null)
                    {
                        var payments = db.BranchesPayments.Where(x => x.Id == model.id).SingleOrDefault();
                        payments.TypePayments = model.typePayments;
                        payments.BranchId = model.branchId;
                        payments.SelectedWeight1 = model.selectedWeight1;
                        payments.SelectedUnit1 = model.selectedUnit1;
                        payments.PriceSelected1 = model.priceSelected1;
                        payments.SelectedWeight2 = model.selectedWeight2;
                        payments.SelectedUnit2 = model.selectedUnit2;
                        payments.PriceSelected2 = model.priceSelected2;
                        payments.WeightDeducted = model.weightDeducted;
                        payments.DeductedUnit = model.deductedUnit;
                        payments.PriceDeducted = model.priceDeducted;
                        payments.StoneWeight = model.stoneWeight;
                        payments.StoneUnit = model.stoneUnit;
                        payments.PriceStone = model.priceStone;
                        payments.LeatherWeight = model.leatherWeight;
                        payments.LeatherUnit = model.leatherUnit;
                        payments.PriceLeather = model.priceLeather;
                        payments.OneRoundLeatherWeight = model.oneRoundLeatherWeight;
                        payments.OneRoundLeatherUnit = model.oneRoundLeatherUnit;
                        payments.PriceOneRoundLeather = model.priceOneRoundLeather;
                        payments.TwoRoundLeatherWeight = model.twoRoundLeatherWeight;
                        payments.TwoRoundLeatherUnit = model.twoRoundLeatherUnit;
                        payments.PriceTwoRoundLeather = model.priceTwoRoundLeather;
                        payments.SelectedWeight3 = model.selectedWeight3;
                        payments.SelectedUnit3 = model.selectedUnit3;
                        payments.PriceSelected3 = model.priceSelected3;
                        payments.SelectedWeight4 = model.selectedWeight4;
                        payments.SelectedUnit4 = model.selectedUnit4;
                        payments.PriceSelected4 = model.priceSelected4;
                        payments.SelectedWeight5 = model.selectedWeight5;
                        payments.SelectedUnit5 = model.selectedUnit5;
                        payments.PriceSelected5 = model.priceSelected5;
                        payments.SelectedWeight6 = model.selectedWeight6;
                        payments.SelectedUnit6 = model.selectedUnit6;
                        payments.PriceSelected6 = model.priceSelected6;
                        payments.SelectedWeight7 = model.selectedWeight7;
                        payments.SelectedUnit7 = model.selectedUnit7;
                        payments.PriceSelected7 = model.priceSelected7;
                        payments.SelectedWeight8 = model.selectedWeight8;
                        payments.SelectedUnit8 = model.selectedUnit8;
                        payments.PriceSelected8 = model.priceSelected8;
                        payments.SelectedWeight9 = model.selectedWeight9;
                        payments.SelectedUnit9 = model.selectedUnit9;
                        payments.PriceSelected9 = model.priceSelected9;
                        payments.SelectedWeight10 = model.selectedWeight10;
                        payments.SelectedUnit10 = model.selectedUnit10;
                        payments.PriceSelected10 = model.priceSelected10;
                        payments.SelectedWeight11 = model.selectedWeight11;
                        payments.SelectedUnit11 = model.selectedUnit11;
                        payments.PriceSelected11 = model.priceSelected11;
                        payments.SelectedWeight12 = model.selectedWeight12;
                        payments.SelectedUnit12 = model.selectedUnit12;
                        payments.PriceSelected12 = model.priceSelected12;
                        payments.LastPeriodAccount = model.lastPeriodAccount;
                        payments.LastPeriodAccountWeight = model.lastPeriodAccountWeight;
                        payments.GoldDebt = model.goldDebt;
                        payments.RialDebt = model.rialDebt;
                        payments.DealerSignature = model.dealerSignature;
                        payments.Description = model.description;
                        payments.FactorNumber = model.factorNumber;
                        payments.Wage = model.wage;
                        payments.GoldWage = model.goldWage;
                        payments.GoldWageUnit = model.goldWageUnit;
                        payments.PriceGoldWage = model.priceGoldWage;
                        payments.DescriptionSelected1 = model.descriptionSelected1;
                        payments.DescriptionSelected2 = model.descriptionSelected2;
                        payments.DescriptionSelected3 = model.descriptionSelected3;
                        payments.DescriptionSelected4 = model.descriptionSelected4;
                        payments.DescriptionSelected5 = model.descriptionSelected5;
                        payments.DescriptionSelected6 = model.descriptionSelected6;
                        payments.DescriptionSelected7 = model.descriptionSelected7;
                        payments.DescriptionSelected8 = model.descriptionSelected8;
                        payments.DescriptionSelected9 = model.descriptionSelected9;
                        payments.DescriptionSelected10 = model.descriptionSelected10;
                        payments.DescriptionSelected11 = model.descriptionSelected11;
                        payments.DescriptionSelected12 = model.descriptionSelected12;
                        payments.Date = DateUtility.GetDateTime(model.date).Value;
                        payments.ModifyUserId = user.Id;
                        payments.ModifyDate = DateTime.Now;
                        payments.Ip = Request.UserHostAddress;

                        var log = new BranchesPaymentsLog
                        {
                            BranchesPayments = payments,
                            UserId = user.Id,
                            Date = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.BranchesPaymentsLog.Add(log);

                    }
                    else
                    {
                        item = new BranchesPayments()
                        {
                            TypePayments = model.typePayments,
                            BranchId = model.branchId,
                            SelectedWeight1 = model.selectedWeight1,
                            SelectedUnit1 = model.selectedUnit1,
                            PriceSelected1 = model.priceSelected1,
                            SelectedWeight2 = model.selectedWeight2,
                            SelectedUnit2 = model.selectedUnit2,
                            PriceSelected2 = model.priceSelected2,
                            SelectedWeight8 = model.selectedWeight8,
                            SelectedUnit8 = model.selectedUnit8,
                            PriceSelected8 = model.priceSelected8,
                            SelectedWeight9 = model.selectedWeight9,
                            SelectedUnit9 = model.selectedUnit9,
                            PriceSelected9 = model.priceSelected9,
                            SelectedWeight10 = model.selectedWeight10,
                            SelectedUnit10 = model.selectedUnit10,
                            PriceSelected10 = model.priceSelected10,
                            SelectedWeight11 = model.selectedWeight11,
                            SelectedUnit11 = model.selectedUnit11,
                            PriceSelected11 = model.priceSelected11,
                            SelectedWeight12 = model.selectedWeight12,
                            SelectedUnit12 = model.selectedUnit12,
                            PriceSelected12 = model.priceSelected12,
                            WeightDeducted = model.weightDeducted,
                            DeductedUnit = model.deductedUnit,
                            PriceDeducted = model.priceDeducted,
                            StoneWeight = model.stoneWeight,
                            StoneUnit = model.stoneUnit,
                            PriceStone = model.priceStone,
                            LeatherWeight = model.leatherWeight,
                            LeatherUnit = model.leatherUnit,
                            PriceLeather = model.priceLeather,
                            OneRoundLeatherWeight = model.oneRoundLeatherWeight,
                            OneRoundLeatherUnit = model.oneRoundLeatherUnit,
                            PriceOneRoundLeather = model.priceOneRoundLeather,
                            TwoRoundLeatherWeight = model.twoRoundLeatherWeight,
                            TwoRoundLeatherUnit = model.twoRoundLeatherUnit,
                            PriceTwoRoundLeather = model.priceTwoRoundLeather,
                            SelectedWeight3 = model.selectedWeight3,
                            SelectedUnit3 = model.selectedUnit3,
                            PriceSelected3 = model.priceSelected3,
                            SelectedWeight4 = model.selectedWeight4,
                            SelectedUnit4 = model.selectedUnit4,
                            PriceSelected4 = model.priceSelected4,
                            SelectedWeight5 = model.selectedWeight5,
                            SelectedUnit5 = model.selectedUnit5,
                            PriceSelected5 = model.priceSelected5,
                            SelectedWeight6 = model.selectedWeight6,
                            SelectedUnit6 = model.selectedUnit6,
                            PriceSelected6 = model.priceSelected6,
                            SelectedWeight7 = model.selectedWeight7,
                            SelectedUnit7 = model.selectedUnit7,
                            PriceSelected7 = model.priceSelected7,
                            LastPeriodAccount = model.lastPeriodAccount,
                            LastPeriodAccountWeight = model.lastPeriodAccountWeight,
                            GoldDebt = model.goldDebt,
                            RialDebt = model.rialDebt,
                            DealerSignature = model.dealerSignature,
                            Description = model.description,
                            FactorNumber = model.factorNumber,
                            Wage = model.wage,
                            GoldWage = model.goldWage,
                            GoldWageUnit = model.goldWageUnit,
                            PriceGoldWage = model.priceGoldWage,
                            DescriptionSelected1 = model.descriptionSelected1,
                            DescriptionSelected2 = model.descriptionSelected2,
                            DescriptionSelected3 = model.descriptionSelected3,
                            DescriptionSelected4 = model.descriptionSelected4,
                            DescriptionSelected5 = model.descriptionSelected5,
                            DescriptionSelected6 = model.descriptionSelected6,
                            DescriptionSelected7 = model.descriptionSelected7,
                            DescriptionSelected8 = model.descriptionSelected8,
                            DescriptionSelected9 = model.descriptionSelected9,
                            DescriptionSelected10 = model.descriptionSelected10,
                            DescriptionSelected11 = model.descriptionSelected11,
                            DescriptionSelected12 = model.descriptionSelected12,
                            Date = DateUtility.GetDateTime(model.date).Value,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };

                        var log = new BranchesPaymentsLog
                        {
                            BranchesPayments = item,
                            UserId = user.Id,
                            Date = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        db.BranchesPaymentsLog.Add(log);

                        db.BranchesPayments.Add(item);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "عملیات با موفقیت ذخیره شد.",
                    data = item.Id,
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, dailyReport")]
        public ActionResult ReportPdf(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                var print = db.BranchesPayments.SingleOrDefault(x => x.Id == id);
                StiReport report = new StiReport();
                report.Load(Server.MapPath("~/Report/branchPayment/branchPayment.mrt"));
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.Dictionary.Variables["DescriptionSelected1"].Value = print.DescriptionSelected1;
                report.Dictionary.Variables["DescriptionSelected2"].Value = print.DescriptionSelected2;
                report.Dictionary.Variables["DescriptionSelected3"].Value = print.DescriptionSelected3;
                report.Dictionary.Variables["DescriptionSelected4"].Value = print.DescriptionSelected4;
                report.Dictionary.Variables["DescriptionSelected5"].Value = print.DescriptionSelected5;
                report.Dictionary.Variables["DescriptionSelected6"].Value = print.DescriptionSelected6;
                //report.Dictionary.Variables["DescriptionSelected7"].Value = print.DescriptionSelected7;
                report.Dictionary.Variables["DescriptionSelected8"].Value = print.DescriptionSelected8;
                report.Dictionary.Variables["DescriptionSelected9"].Value = print.DescriptionSelected9;
                report.Dictionary.Variables["DescriptionSelected10"].Value = print.DescriptionSelected10;
                report.Dictionary.Variables["DescriptionSelected11"].Value = print.DescriptionSelected11;
                report.Dictionary.Variables["DescriptionSelected12"].Value = print.DescriptionSelected12;
                report.Dictionary.Variables["Date"].Value = DateUtility.GetPersianDate(print.Date);
                if (print.SelectedWeight8 == null)
                {
                    print.SelectedWeight8 = 0;
                    report.Dictionary.Variables["SelectedWeight8"].Value = print.SelectedWeight8.ToString();
                }
                report.Dictionary.Variables["SelectedWeight8"].Value = print.SelectedWeight8.ToString();
                if (print.SelectedUnit8 == null)
                {
                    print.SelectedUnit8 = 0;
                    report.Dictionary.Variables["SelectedUnit8"].Value = print.SelectedUnit8.ToString();
                }
                report.Dictionary.Variables["SelectedUnit8"].Value = print.SelectedUnit8.ToString();
                if (print.PriceSelected8 == null)
                {
                    print.PriceSelected8 = 0;
                    report.Dictionary.Variables["PriceSelected8"].Value = print.PriceSelected8?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected8"].Value = print.PriceSelected8?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight9 == null)
                {
                    print.SelectedWeight9 = 0;
                    report.Dictionary.Variables["SelectedWeight9"].Value = print.SelectedWeight9.ToString();
                }
                report.Dictionary.Variables["SelectedWeight9"].Value = print.SelectedWeight9.ToString();
                if (print.SelectedUnit9 == null)
                {
                    print.SelectedUnit9 = 0;
                    report.Dictionary.Variables["SelectedUnit9"].Value = print.SelectedUnit9.ToString();
                }
                report.Dictionary.Variables["SelectedUnit9"].Value = print.SelectedUnit9.ToString();
                if (print.PriceSelected9 == null)
                {
                    print.PriceSelected9 = 0;
                    report.Dictionary.Variables["PriceSelected9"].Value = print.PriceSelected9?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected9"].Value = print.PriceSelected9?.ToString("N0").ToPersianNumber();
                //
                if (print.GoldWage == null)
                {
                    print.GoldWage = 0;
                    report.Dictionary.Variables["GoldWage"].Value = print.GoldWage.ToString();
                }
                report.Dictionary.Variables["GoldWage"].Value = print.GoldWage.ToString();
                if (print.GoldWageUnit == null)
                {
                    print.GoldWageUnit = 0;
                    report.Dictionary.Variables["GoldWageUnit"].Value = print.GoldWageUnit.ToString();
                }
                report.Dictionary.Variables["GoldWageUnit"].Value = print.GoldWageUnit.ToString();
                if (print.PriceGoldWage == null)
                {
                    print.PriceGoldWage = 0;
                    report.Dictionary.Variables["PriceGoldWage"].Value = print.PriceGoldWage?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceGoldWage"].Value = print.PriceGoldWage?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight10 == null)
                {
                    print.SelectedWeight10 = 0;
                    report.Dictionary.Variables["SelectedWeight10"].Value = print.SelectedWeight10.ToString();
                }
                report.Dictionary.Variables["SelectedWeight10"].Value = print.SelectedWeight10.ToString();
                if (print.SelectedUnit10 == null)
                {
                    print.SelectedUnit10 = 0;
                    report.Dictionary.Variables["SelectedUnit10"].Value = print.SelectedUnit10.ToString();
                }
                report.Dictionary.Variables["SelectedUnit10"].Value = print.SelectedUnit10.ToString();
                if (print.PriceSelected10 == null)
                {
                    print.PriceSelected10 = 0;
                    report.Dictionary.Variables["PriceSelected10"].Value = print.PriceSelected10?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected10"].Value = print.PriceSelected10?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight11 == null)
                {
                    print.SelectedWeight11 = 0;
                    report.Dictionary.Variables["SelectedWeight11"].Value = print.SelectedWeight11.ToString();
                }
                report.Dictionary.Variables["SelectedWeight11"].Value = print.SelectedWeight11.ToString();
                if (print.SelectedUnit11 == null)
                {
                    print.SelectedUnit11 = 0;
                    report.Dictionary.Variables["SelectedUnit11"].Value = print.SelectedUnit11.ToString();
                }
                report.Dictionary.Variables["SelectedUnit11"].Value = print.SelectedUnit11.ToString();
                if (print.PriceSelected11 == null)
                {
                    print.PriceSelected11 = 0;
                    report.Dictionary.Variables["PriceSelected11"].Value = print.PriceSelected11?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected11"].Value = print.PriceSelected11?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight12 == null)
                {
                    print.SelectedWeight12 = 0;
                    report.Dictionary.Variables["SelectedWeight12"].Value = print.SelectedWeight12.ToString();
                }
                report.Dictionary.Variables["SelectedWeight12"].Value = print.SelectedWeight12.ToString();
                if (print.SelectedUnit12 == null)
                {
                    print.SelectedUnit12 = 0;
                    report.Dictionary.Variables["SelectedUnit12"].Value = print.SelectedUnit12.ToString();
                }
                report.Dictionary.Variables["SelectedUnit12"].Value = print.SelectedUnit12.ToString();
                if (print.PriceSelected12 == null)
                {
                    print.PriceSelected12 = 0;
                    report.Dictionary.Variables["PriceSelected12"].Value = print.PriceSelected12?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected12"].Value = print.PriceSelected12?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight1 == null)
                {
                    print.SelectedWeight1 = 0;
                    report.Dictionary.Variables["SelectedWeight1"].Value = print.SelectedWeight1.ToString();
                }
                report.Dictionary.Variables["SelectedWeight1"].Value = print.SelectedWeight1.ToString();
                if (print.SelectedUnit1 == null)
                {
                    print.SelectedUnit1 = 0;
                    report.Dictionary.Variables["SelectedUnit1"].Value = print.SelectedUnit1.ToString();
                }
                report.Dictionary.Variables["SelectedUnit1"].Value = print.SelectedUnit1.ToString();
                if (print.PriceSelected1 == null)
                {
                    print.PriceSelected1 = 0;
                    report.Dictionary.Variables["PriceSelected1"].Value = print.PriceSelected1?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected1"].Value = print.PriceSelected1?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight2 == null)
                {
                    print.SelectedWeight2 = 0;
                    report.Dictionary.Variables["SelectedWeight2"].Value = print.SelectedWeight2.ToString();
                }
                report.Dictionary.Variables["SelectedWeight2"].Value = print.SelectedWeight2.ToString();
                if (print.SelectedUnit2 == null)
                {
                    print.SelectedUnit2 = 0;
                    report.Dictionary.Variables["SelectedUnit2"].Value = print.SelectedUnit2.ToString();
                }
                report.Dictionary.Variables["SelectedUnit2"].Value = print.SelectedUnit2.ToString();
                if (print.PriceSelected2 == null)
                {
                    print.PriceSelected2 = 0;
                    report.Dictionary.Variables["PriceSelected2"].Value = print.PriceSelected2?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected2"].Value = print.PriceSelected2?.ToString("N0").ToPersianNumber();
                //
                var sumFirstWeight = print.SelectedWeight8 + print.SelectedWeight9 + print.GoldWage + print.SelectedWeight10 + print.SelectedWeight11 + print.SelectedWeight12 + print.SelectedWeight1 + print.SelectedWeight2;
                //var sumFirstUnit = print.SelectedUnit8 + print.SelectedUnit9 + print.GoldWageUnit + print.SelectedUnit10 + print.SelectedUnit11 + print.SelectedUnit12 + print.SelectedUnit1 + print.SelectedUnit2;
                var sumFirstPrice = print.PriceSelected8 + print.PriceSelected9 + print.PriceGoldWage + print.PriceSelected10 + print.PriceSelected11 + print.PriceSelected12 + print.PriceSelected1 + print.PriceSelected2;
                report.Dictionary.Variables["SumFirstWeight"].Value = sumFirstWeight.ToString();
                //report.Dictionary.Variables["SumFirstUnit"].Value = sumFirstUnit.ToString();
                report.Dictionary.Variables["SumFirstPrice"].ValueObject = sumFirstPrice?.ToString("N0").ToPersianNumber();

                if (print.WeightDeducted == null)
                {
                    print.WeightDeducted = 0;
                    report.Dictionary.Variables["WeightDeducted"].Value = print.WeightDeducted?.ToString();
                }
                report.Dictionary.Variables["WeightDeducted"].Value = print.WeightDeducted.ToString();
                if (print.DeductedUnit == null)
                {
                    print.DeductedUnit = 0;
                    report.Dictionary.Variables["DeductedUnit"].Value = print.DeductedUnit.ToString();
                }
                report.Dictionary.Variables["DeductedUnit"].Value = print.DeductedUnit.ToString();
                if (print.PriceDeducted == null)
                {
                    print.PriceDeducted = 0;
                    report.Dictionary.Variables["PriceDeducted"].Value = print.PriceDeducted?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceDeducted"].Value = print.PriceDeducted?.ToString("N0").ToPersianNumber();
                //
                if (print.StoneWeight == null)
                {
                    print.StoneWeight = 0;
                    report.Dictionary.Variables["StoneWeight"].Value = print.StoneWeight.ToString();
                }
                report.Dictionary.Variables["StoneWeight"].Value = print.StoneWeight.ToString();
                if (print.StoneUnit == null)
                {
                    print.StoneUnit = 0;
                    report.Dictionary.Variables["StoneUnit"].Value = print.StoneUnit.ToString();
                }
                report.Dictionary.Variables["StoneUnit"].Value = print.StoneUnit.ToString();
                if (print.PriceStone == null)
                {
                    print.PriceStone = 0;
                    report.Dictionary.Variables["PriceStone"].Value = print.PriceStone?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceStone"].Value = print.PriceStone?.ToString("N0").ToPersianNumber();
                //
                if (print.LeatherWeight == null)
                {
                    print.LeatherWeight = 0;
                    report.Dictionary.Variables["LeatherWeight"].Value = print.LeatherWeight.ToString();
                }
                report.Dictionary.Variables["LeatherWeight"].Value = print.LeatherWeight.ToString();
                if (print.LeatherUnit == null)
                {
                    print.LeatherUnit = 0;
                    report.Dictionary.Variables["LeatherUnit"].Value = print.LeatherUnit.ToString();
                }
                report.Dictionary.Variables["LeatherUnit"].Value = print.LeatherUnit.ToString();
                if (print.PriceLeather == null)
                {
                    print.PriceLeather = 0;
                    report.Dictionary.Variables["PriceLeather"].Value = print.PriceLeather?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceLeather"].Value = print.PriceLeather?.ToString("N0").ToPersianNumber();
                //
                if (print.OneRoundLeatherWeight == null)
                {
                    print.OneRoundLeatherWeight = 0;
                    report.Dictionary.Variables["OneRoundLeatherWeight"].Value = print.LeatherUnit.ToString();
                }
                report.Dictionary.Variables["OneRoundLeatherWeight"].Value = print.OneRoundLeatherWeight.ToString();
                if (print.OneRoundLeatherUnit == null)
                {
                    print.OneRoundLeatherUnit = 0;
                    report.Dictionary.Variables["OneRoundLeatherUnit"].Value = print.OneRoundLeatherUnit.ToString();
                }
                report.Dictionary.Variables["OneRoundLeatherUnit"].Value = print.OneRoundLeatherUnit.ToString();
                if (print.PriceOneRoundLeather == null)
                {
                    print.PriceOneRoundLeather = 0;
                    report.Dictionary.Variables["PriceOneRoundLeather"].Value = print.PriceOneRoundLeather?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceOneRoundLeather"].Value = print.PriceOneRoundLeather?.ToString("N0").ToPersianNumber();
                //
                if (print.TwoRoundLeatherWeight == null)
                {
                    print.TwoRoundLeatherWeight = 0;
                    report.Dictionary.Variables["TwoRoundLeatherWeight"].Value = print.TwoRoundLeatherWeight.ToString();
                }
                report.Dictionary.Variables["TwoRoundLeatherWeight"].Value = print.TwoRoundLeatherWeight.ToString();
                if (print.TwoRoundLeatherUnit == null)
                {
                    print.TwoRoundLeatherUnit = 0;
                    report.Dictionary.Variables["TwoRoundLeatherUnit"].Value = print.TwoRoundLeatherUnit.ToString();
                }
                report.Dictionary.Variables["TwoRoundLeatherUnit"].Value = print.TwoRoundLeatherUnit.ToString();
                if (print.PriceTwoRoundLeather == null)
                {
                    print.PriceTwoRoundLeather = 0;
                    report.Dictionary.Variables["PriceTwoRoundLeather"].Value = print.PriceTwoRoundLeather?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceTwoRoundLeather"].Value = print.PriceTwoRoundLeather?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight3 == null)
                {
                    print.SelectedWeight3 = 0;
                    report.Dictionary.Variables["SelectedWeight3"].Value = print.SelectedWeight3.ToString();
                }
                report.Dictionary.Variables["SelectedWeight3"].Value = print.SelectedWeight3.ToString();
                if (print.SelectedUnit3 == null)
                {
                    print.SelectedUnit3 = 0;
                    report.Dictionary.Variables["SelectedUnit3"].Value = print.SelectedUnit3.ToString();
                }
                report.Dictionary.Variables["SelectedUnit3"].Value = print.SelectedUnit3.ToString();
                if (print.PriceSelected3 == null)
                {
                    print.PriceSelected3 = 0;
                    report.Dictionary.Variables["PriceSelected3"].Value = print.PriceSelected3?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected3"].Value = print.PriceSelected3?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight4 == null)
                {
                    print.SelectedWeight4 = 0;
                    report.Dictionary.Variables["SelectedWeight4"].Value = print.SelectedWeight4.ToString();
                }
                report.Dictionary.Variables["SelectedWeight4"].Value = print.SelectedWeight4.ToString();
                if (print.SelectedUnit4 == null)
                {
                    print.SelectedUnit4 = 0;
                    report.Dictionary.Variables["SelectedUnit4"].Value = print.SelectedUnit4.ToString();
                }
                report.Dictionary.Variables["SelectedUnit4"].Value = print.SelectedUnit4.ToString();
                if (print.PriceSelected4 == null)
                {
                    print.PriceSelected4 = 0;
                    report.Dictionary.Variables["PriceSelected4"].Value = print.PriceSelected4?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected4"].Value = print.PriceSelected4?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight5 == null)
                {
                    print.SelectedWeight5 = 0;
                    report.Dictionary.Variables["SelectedWeight5"].Value = print.SelectedWeight5.ToString();
                }
                report.Dictionary.Variables["SelectedWeight5"].Value = print.SelectedWeight5.ToString();
                if (print.SelectedUnit5 == null)
                {
                    print.SelectedUnit5 = 0;
                    report.Dictionary.Variables["SelectedUnit5"].Value = print.SelectedUnit5.ToString();
                }
                report.Dictionary.Variables["SelectedUnit5"].Value = print.SelectedUnit5.ToString();
                if (print.PriceSelected5 == null)
                {
                    print.PriceSelected5 = 0;
                    report.Dictionary.Variables["PriceSelected5"].Value = print.PriceSelected5?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected5"].Value = print.PriceSelected5?.ToString("N0").ToPersianNumber();
                //
                if (print.SelectedWeight6 == null)
                {
                    print.SelectedWeight6 = 0;
                    report.Dictionary.Variables["SelectedWeight6"].Value = print.SelectedWeight6.ToString();
                }
                report.Dictionary.Variables["SelectedWeight6"].Value = print.SelectedWeight6.ToString();
                if (print.SelectedUnit6 == null)
                {
                    print.SelectedUnit6 = 0;
                    report.Dictionary.Variables["SelectedUnit6"].Value = print.SelectedUnit6.ToString();
                }
                report.Dictionary.Variables["SelectedUnit6"].Value = print.SelectedUnit6.ToString();
                if (print.PriceSelected6 == null)
                {
                    print.PriceSelected6 = 0;
                    report.Dictionary.Variables["PriceSelected6"].Value = print.PriceSelected6?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["PriceSelected6"].Value = print.PriceSelected6?.ToString("N0").ToPersianNumber();
                //

                if (print.LastPeriodAccount == null)
                {
                    print.LastPeriodAccount = 0;
                    report.Dictionary.Variables["LastPeriodAccount"].Value = print.LastPeriodAccount?.ToString("N0").ToPersianNumber();
                }
                report.Dictionary.Variables["LastPeriodAccount"].Value = print.LastPeriodAccount?.ToString("N0").ToPersianNumber();
                if (print.LastPeriodAccountWeight == null)
                {
                    print.LastPeriodAccountWeight = 0;
                    report.Dictionary.Variables["LastPeriodAccountWeight"].Value = print.LastPeriodAccountWeight.ToString();
                }
                report.Dictionary.Variables["LastPeriodAccountWeight"].Value = print.LastPeriodAccountWeight.ToString();

                var sumSecondWeight = print.WeightDeducted + print.StoneWeight + print.LeatherWeight + print.OneRoundLeatherWeight + print.TwoRoundLeatherWeight + print.SelectedWeight3 + print.SelectedWeight4 + print.SelectedWeight5 + print.SelectedWeight6 ;
                //var sumSecondUnit = print.DeductedUnit + print.StoneUnit + print.LeatherUnit + print.OneRoundLeatherUnit + print.TwoRoundLeatherUnit + print.SelectedUnit3 + print.SelectedUnit4 + print.SelectedUnit5 + print.SelectedUnit6 + print.SelectedUnit7;
                var sumSecondPrice = print.PriceDeducted + print.PriceStone + print.PriceLeather + print.PriceOneRoundLeather + print.PriceTwoRoundLeather + print.PriceSelected3 + print.PriceSelected4 + print.PriceSelected5 + print.PriceSelected6;

                var sumLastPeriodAccount = print.LastPeriodAccount;
                var sumLastPeriodAccountWeight = print.LastPeriodAccountWeight;
                report.Dictionary.Variables["sumSecondWeight"].Value = sumSecondWeight?.ToString();
                report.Dictionary.Variables["SumSecondPrice"].Value = sumSecondPrice?.ToString("N0").ToPersianNumber();
                //
                var sumLastWeight = sumFirstWeight + sumSecondWeight + sumLastPeriodAccountWeight;
                //var sumLastUnit = sumFirstUnit + sumSecondUnit;
                var sumLastPrice = sumFirstPrice + sumSecondPrice + sumLastPeriodAccount;
                report.Dictionary.Variables["SumLastWeight"].Value = sumLastWeight.ToString();
                //report.Dictionary.Variables["SumLastUnit"].Value = sumLastUnit.ToString();
                report.Dictionary.Variables["SumLastPrice"].Value = sumLastPrice?.ToString("N0").ToPersianNumber();
                //
                report.Dictionary.Variables["Wage"].Value = print.Wage.ToString();
                report.Dictionary.Variables["Description"].ValueObject = print.Description;
                report.Dictionary.Variables["DealerSignature"].ValueObject = print.DealerSignature;
                report.Dictionary.Variables["FactorNumber"].Value = print.FactorNumber;
                report.Dictionary.Variables["Branch"].Value = print.Branch.Name;
                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.Dictionary.Synchronize();
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
        /// خواندن اطلاعات شعبه
        /// </summary>
        /// <param name="id">ردیف شعبه</param>
        /// <returns>جیسون اطلاعات لود شده شعبه</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPayments, branchesPaymentsReturned")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                BranchesPayments item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.BranchesPayments.FirstOrDefault(x => x.Id == id);

                    var data = new BranchesPaymentsViewModel
                    {
                        id = item.Id,
                        typePayments = item.TypePayments,
                        branchId = item.BranchId,
                        selectedWeight8 = item.SelectedWeight8,
                        selectedUnit8 = item.SelectedUnit8,
                        priceSelected8 = item.PriceSelected8,
                        selectedWeight9 = item.SelectedWeight9,
                        selectedUnit9 = item.SelectedUnit9,
                        priceSelected9 = item.PriceSelected9,
                        selectedWeight10 = item.SelectedWeight10,
                        selectedUnit10 = item.SelectedUnit10,
                        priceSelected10 = item.PriceSelected10,
                        selectedWeight11 = item.SelectedWeight11,
                        selectedUnit11 = item.SelectedUnit11,
                        priceSelected11 = item.PriceSelected11,
                        selectedWeight12 = item.SelectedWeight12,
                        selectedUnit12 = item.SelectedUnit12,
                        priceSelected12 = item.PriceSelected12,
                        goldWage = item.GoldWage,
                        goldWageUnit = item.GoldWageUnit,
                        priceGoldWage = item.PriceGoldWage,
                        selectedWeight1 = item.SelectedWeight1,
                        selectedUnit1 = item.SelectedUnit1,
                        priceSelected1 = item.PriceSelected1,
                        selectedWeight2 = item.SelectedWeight2,
                        selectedUnit2 = item.SelectedUnit2,
                        priceSelected2 = item.PriceSelected2,
                        weightDeducted = item.WeightDeducted,
                        deductedUnit = item.DeductedUnit,
                        priceDeducted = item.PriceDeducted,
                        stoneWeight = item.StoneWeight,
                        stoneUnit = item.StoneUnit,
                        priceStone = item.PriceStone,
                        leatherWeight = item.LeatherWeight,
                        leatherUnit = item.LeatherUnit,
                        priceLeather = item.PriceLeather,
                        oneRoundLeatherWeight = item.OneRoundLeatherWeight,
                        oneRoundLeatherUnit = item.OneRoundLeatherUnit,
                        priceOneRoundLeather = item.PriceOneRoundLeather,
                        twoRoundLeatherWeight = item.TwoRoundLeatherWeight,
                        twoRoundLeatherUnit = item.TwoRoundLeatherUnit,
                        priceTwoRoundLeather = item.PriceTwoRoundLeather,
                        selectedWeight3 = item.SelectedWeight3,
                        selectedUnit3 = item.SelectedUnit3,
                        priceSelected3 = item.PriceSelected3,
                        selectedWeight4 = item.SelectedWeight4,
                        selectedUnit4 = item.SelectedUnit4,
                        priceSelected4 = item.PriceSelected4,
                        description = item.Description,
                        goldDebt = item.GoldDebt,
                        rialDebt = item.RialDebt,
                        dealerSignature = item.DealerSignature,
                        lastPeriodAccount = item.LastPeriodAccount,
                        lastPeriodAccountWeight = item.LastPeriodAccountWeight,
                        date = DateUtility.GetPersianDate(item.Date),
                        factorNumber = item.FactorNumber,
                        wage = item.Wage,
                        descriptionSelected1 = item.DescriptionSelected1,
                        descriptionSelected2 = item.DescriptionSelected2,
                        descriptionSelected3 = item.DescriptionSelected3,
                        descriptionSelected4 = item.DescriptionSelected4,
                        descriptionSelected5 = item.DescriptionSelected5,
                        descriptionSelected6 = item.DescriptionSelected6,
                        descriptionSelected7 = item.DescriptionSelected7,
                        descriptionSelected8 = item.DescriptionSelected8,
                        descriptionSelected9 = item.DescriptionSelected9,
                        descriptionSelected10 = item.DescriptionSelected10,
                        descriptionSelected11 = item.DescriptionSelected11,
                        descriptionSelected12 = item.DescriptionSelected12,

                        branchesPaymentsLogList = item.BranchesPaymentsLogList.Select(x => new BranchesPaymentsLogViewModel()
                        {
                            id = x.Id,
                            status = x.Status,
                            userId = x.UserId,
                            userFullName = x.User.FirstName + " " + x.User.LastName,
                            date = x.Date
                        }).ToList()
                    };

                    data.branchesPaymentsLogList.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDateTime(x.date);
                    });

                    if (item != null)
                    {
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
                            status = 404,
                            message = "اطلاعات مورد نظر یافت نشد."
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
        /// دریافت همه شعبه های فعال
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام شعبه ها</returns>
        [HttpGet]
        public JsonResult GetAllBranch()
        {
            Response response;
            try
            {
                List<Branch> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Branch.Where(x => x.Active == true && x.City.Name != "شهر تهران").OrderBy(x => x.Order).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            name = item.Name
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
        /// جستجوی شعب
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست شعب پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branchesPayments, branchesPaymentsDeposits, branchesPaymentsReturned, branchesPaymentsSale, branchesPaymentsDifferentReturns, branchesPaymentsDifferentSale, branchesPaymentsDifferentGoldReturns, branchesPaymentsDifferentGoldSale")]
        public JsonResult Search(BranchesPaymentsSearchViewModel model)
        {
            Response response;
            try
            {
                List<BranchesPaymentsViewModel> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.BranchesPayments.Select(x => x);
                    if (model.typePayments != null)
                    {
                        query = query.Where(x => x.TypePayments == model.typePayments);
                    }
                    if (model.branch != null && model.branch != -1)
                    {
                        query = query.Where(x => x.BranchId == model.branch);
                    }
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    list = query.Select(x => new BranchesPaymentsViewModel
                    {
                        id = x.Id,
                        typePayments = x.TypePayments,
                        branchName = x.Branch.Name
                    }).ToList();
                }
                list.ForEach(x =>
                {
                    x.typePaymentsTitle = Enums.GetTitle(x.typePayments);
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
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}
