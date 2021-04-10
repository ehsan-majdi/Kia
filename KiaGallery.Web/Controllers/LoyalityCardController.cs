using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using br.com.arcnet.barcodegenerator;
using System.IO;
using Stimulsoft.Report;
using System.Data;
using System.Drawing;
using System.Text;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Components;

namespace KiaGallery.Web.Controllers
{
    public class LoyalityCardController : BaseController
    {
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                ViewBag.BranchList = branchList;

                ViewBag.SilverCardValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeySilverCardValue)?.Value;
                ViewBag.GoldenCardValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldenCardValue)?.Value;
                ViewBag.PlatinumCardValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyPlatinumCardValue)?.Value;

                ViewBag.SilverCardLevel = db.Settings.SingleOrDefault(x => x.Key == Settings.KeySilverCardLevel)?.Value;
                ViewBag.GoldenCardLevel = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldenCardLevel)?.Value;
                ViewBag.PlatinumCardLevel = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyPlatinumCardLevel)?.Value;
            }
            return View();
        }
        /// <summary>
        /// ثبت کارت
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int Id)
        {
            using (var db = new KiaGalleryContext())
            {
                List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true && x.BranchType != BranchType.Solicitorship).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();

                ViewBag.BranchList = branchList;
            }
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش کارت";
            return View();
        }
        /// <summary>
        /// کارت جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Add()
        {
            ViewBag.Title = "کارت جدید";
            return View("Edit");
        }
        public JsonResult Save(LoyalityCardViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    if (model.id > 0)
                    {

                    }
                    else
                    {
                        List<string> codeList = new List<string>();
                        for (int i = 1; i <= model.count; i++)
                        {
                            string code;
                            while (true)
                            {
                                code = RandomString(8);
                                LoyalityCard loyalityCard = db.LoyalityCard.FirstOrDefault(x => x.Code == code);
                                if (loyalityCard == null && codeList.Count(x => x == code) == 0)
                                {
                                    break;
                                }
                            }
                            codeList.Add(code);
                            var entity = new LoyalityCard()
                            {
                                Code = code,
                                CardType = model.cardType,
                                CardStatus = LoyalityCardStatus.Register,
                                CreateUserId = currentUser.Id,
                                ModifyUserId = currentUser.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };
                            var log = new LoyalityCardLog()
                            {
                                LoyalityCard = entity,
                                CardStatus = LoyalityCardStatus.Register,
                                CreateUserId = currentUser.Id,
                                CreateDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };
                            db.LoyalityCardLog.Add(log);
                            db.LoyalityCard.Add(entity);
                            var barcode = new Barcode(code, TypeBarcode.Code128C);
                            var bar128 = barcode.Encode(TypeBarcode.Code128C, code, 886, 142);

                            string serverPath = Server.MapPath("~/Upload/LoyalityCard/");
                            if (!Directory.Exists(serverPath))
                            {
                                Directory.CreateDirectory(serverPath);
                            }
                            bar128.Save(serverPath + code + ".jpg");
                        }
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "کارت با موفقیت ایجاد شد."
                };

            }
            catch (Exception ex)
            {

                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSetting(SaveCardSettingViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var SilverCardValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeySilverCardValue);
                    var GoldenCardValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldenCardValue);
                    var PlatinumCardValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyPlatinumCardValue);
                    SilverCardValue.Value = model.silverCardValue;
                    GoldenCardValue.Value = model.goldenCardValue;
                    PlatinumCardValue.Value = model.platinumCardValue;
                    db.SaveChanges();
                }
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

        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.LoyalityCard.Where(x => x.Id == id).Select(x => new LoyalityCardViewModel
                    {
                        id = x.Id,
                        cardType = x.CardType,
                        cardStatus = x.CardStatus,
                        code = x.Code,

                    }).SingleOrDefault();
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

        public JsonResult ChangeStatus(LoyalityCardViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var cardList = db.LoyalityCard.Where(x => model.idList.Any(y => y == x.Id)).ToList();
                    foreach (var item in cardList)
                    {
                        if (model.cardStatus == LoyalityCardStatus.SendToBranch)
                        {
                            item.BranchId = model.branchId;
                        }
                        item.CardStatus = model.cardStatus;
                        item.ModifyUserId = currentUser.Id;
                        item.ModifyDate = DateTime.Now;
                        item.Ip = Request.UserHostAddress;

                        var log = new LoyalityCardLog()
                        {
                            LoyalityCardId = item.Id,
                            CardStatus = model.cardStatus,
                            CreateUserId = currentUser.Id,
                            CreateDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.LoyalityCardLog.Add(log);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = " وضعیت کارت به " + Enums.GetTitle(model.cardStatus) + " تغییر یافت. "
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ReserveCard(LoyalityCardReserveViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            var birthDate = DateUtility.GetDateTime(model.persianBirthDate);
            var mariageDate = DateUtility.GetDateTime(model.persianMariageDate);
            try
            {
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    var card = db.LoyalityCard.Where(x => x.Code == model.code).SingleOrDefault();
                    var customer = db.CustomerLoyality.Where(x => x.PhoneNumber == model.phoneNumber).SingleOrDefault();
                    if(customer == null)
                    {
                        var entity = new CustomerLoyality
                        {
                            LoyalityCardId = card.Id,
                            FirstName = model.firstName,
                            LastName = model.lastName,
                            PhoneNumber = model.phoneNumber,
                            BirthDate =DateUtility.GetDateTime(model.birthDate),
                            MariageDate = DateUtility.GetDateTime(model.mariageDate),
                            Date=DateTime.Now,
                            ModifyUserId = currentUser.Id,
                            CreateUserId = currentUser.Id,
                            ModifyDate = DateTime.Now,
                            CreateDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        card.CustomerList.Add(entity);
                    }
                    else
                    {
                        customer.LoyalityCardId = card.Id;
                        customer.MariageDate = DateUtility.GetDateTime(model.mariageDate);
                        customer.BirthDate = DateUtility.GetDateTime(model.birthDate);
                        customer.ModifyUserId = currentUser.Id;
                        customer.ModifyDate = DateTime.Now;
                        customer.Date = DateTime.Now;

                    }
                    card.CardStatus = LoyalityCardStatus.Reserved;
                    var log = new LoyalityCardLog()
                    {
                        LoyalityCardId = card.Id,
                        CardStatus = LoyalityCardStatus.Reserved,
                        CreateUserId = currentUser.Id,
                        CreateDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.LoyalityCardLog.Add(log);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "اطلاعات با موفقیت ثبت شد."
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
        /// چک کردن کارت مشتری برای شناسایی و موجود بودن بارکد کارت در سیستم 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ValidateCard(string code)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var card = db.LoyalityCard.Where(x => x.Code == code).SingleOrDefault();
                    if (card == null)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "کارتی با این کد وجود ندارد."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (card.CardStatus != LoyalityCardStatus.SendToBranch)
                        {
                            response = new Response()
                            {
                                status = 200,
                                message = "شما نمیتوانید از این کارت در این وضعیت استفاده کنید."
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 200,
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
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

        [AllowAnonymous]
        public JsonResult Search(LoyalityCardSearchViewModel model)
        {
            var currentUser = GetAuthenticatedUser();
            Response response;
            int dataCount;
            IQueryable<LoyalityCard> query = null;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    query = db.LoyalityCard.Select(x => x);
                    if (model.status != null && model.status >= 0)
                    {
                        query = query.Where(x => x.CardStatus == model.status);
                    }
                    if (model.type != null && model.type >= 0)
                    {
                        query = query.Where(x => x.CardType == model.type);
                    }
                     dataCount = query.Count();
                    query = query.OrderBy(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new LoyalityCardViewModel
                    {
                        id = x.Id,
                        code = x.Code,
                        cardStatus = x.CardStatus,
                        cardType = x.CardType,
                        branchId = x.BranchId,
                        branchName = x.Branch.Name,
                        customerName = string.IsNullOrEmpty(x.CustomerList.FirstOrDefault().FirstName) && string.IsNullOrEmpty(x.CustomerList.FirstOrDefault().LastName) ? null : x.CustomerList.FirstOrDefault().FirstName + " " + x.CustomerList.FirstOrDefault().LastName
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.cardStatusTitle = Enums.GetTitle(x.cardStatus);
                        x.cardTypeTitle = Enums.GetTitle(x.cardType);
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
                };

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchLog(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.LoyalityCardLog.Where(x => x.LoyalityCardId == id);

                    var list = query.Select(x => new LoyalityCardViewModel
                    {
                        id = x.Id,
                        createDate = x.CreateDate,
                        cardStatus = x.CardStatus,
                        createUser = x.CreateUser.FirstName + " " + x.CreateUser.LastName,
                        branchName = x.LoyalityCard.Branch.Name,

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.persianCreateDate = DateUtility.GetPersianDateTime(x.createDate);
                        x.cardStatusTitle = Enums.GetTitle(x.cardStatus);
                        x.cardTypeTitle = Enums.GetTitle(x.cardType);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list
                        }
                    };
                };

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var card = db.LoyalityCard.Find(id);
                    var logList = card.LogList.Select(x => x).ToList();
                    db.LoyalityCardLog.RemoveRange(logList);
                    db.LoyalityCard.Remove(card);
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200,
                    message = "Deleted"
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Print(string id)
        {
            List<int> idList = id.Split(',').Select(x => int.Parse(x)).ToList();
            List<LoyalityCardViewModel> cardList;
            using (var db = new KiaGalleryContext())
            {
                cardList = db.LoyalityCard.Where(x => idList.Any(y => y == x.Id)).Select(x => new LoyalityCardViewModel
                {
                    id = x.Id,
                    code = x.Code,
                    cardType = x.CardType
                }).ToList();
            }
            var data = cardList.GroupBy(x => new { x.id, x.code, x.cardType }).Select(x => new { x.Key.id, x.Key.code, x.Key.cardType });
            List<StiReport> reports = new List<StiReport>();
            foreach (var cards in data.GroupBy(x => new { x.cardType }))
            {
                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Code");
                dataTable.Columns.Add("Image", typeof(byte[]));

                foreach (var item in cards)
                {
                    DataRow row = dataTable.NewRow();
                    row["Image"] = GetProductFileByte("~/upload/loyalityCard/" + item.code + ".jpg");
                    row["Code"] = item.code;
                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);
                StiReport report = new StiReport();
                report.Load(Server.MapPath("~/Report/loyalityCard/loyalityCard.mrt"));
                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.RegData("DataSource", dataset.Tables[0].DefaultView);
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

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
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
    }
}