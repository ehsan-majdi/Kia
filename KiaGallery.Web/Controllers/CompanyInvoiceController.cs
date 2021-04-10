using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر فاکتور حقوقی یا شرکتی
    /// </summary>
    public class CompanyInvoiceController : BaseController
    {
        /// <summary>
        /// صفحه اصلی
        /// </summary>
        /// <returns></returns>
        // GET: CompanyInvoice
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// ویرایش
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// افزودن
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View("Edit");
        }
        /// <summary>
        /// ذخیره
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Save(CompanyInvoiceViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                string oldFileName = "";
                using (var db = new KiaGalleryContext())
                {

                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.CompanyInvoices.Where(x => x.Id == model.id).SingleOrDefault();
                        entity.BranchId = user.BranchId;
                        entity.BuyerAddress = model.buyerAddress;
                        entity.Reduction = model.reduction;
                        entity.BuyerEconomicalNumber = model.buyerEconomicalNumber;
                        entity.BuyerName = model.buyerName;
                        entity.BuyerNationalId = model.buyerNationalId;
                        entity.BuyerPhone = model.buyerPhone;
                        entity.BuyerPostalCode = model.buyerPostalCode;
                        entity.Date = DateUtility.GetDateTime(model.date);
                        entity.ModifyUserId = user.Id;
                        entity.ModifyDate = DateTime.Now;


                        foreach (var item in model.companyInvoiceDetailViewModel)
                        {
                            if (item.detailId != null && item.detailId > 0)
                            {
                                var detail = entity.CompanyInvoiceDetailList.Where(y => y.Id == item.detailId).SingleOrDefault();
                                detail.Carat = item.carat;
                                detail.DescriptionProduct = item.descriptionProduct;
                                detail.Gram = item.gram;
                                detail.IdentificationCode = item.identificationCode;
                                detail.StonePrice = item.stonePrice;
                                detail.StoneWeight = item.stoneWeight;
                                detail.Wage = item.wage;
                                detail.Whistle = item.whistle;
                                detail.GoldPrice = item.goldPrice;
                            }
                        }
                    }
                    else
                    {
                        var item = new CompanyInvoice()
                        {
                            BranchId = user.BranchId,
                            BuyerAddress = model.buyerAddress,
                            Reduction = model.reduction,
                            BuyerEconomicalNumber = model.buyerEconomicalNumber,
                            BuyerName = model.buyerName,
                            BuyerNationalId = model.buyerNationalId,
                            BuyerPhone = model.buyerPhone,
                            BuyerPostalCode = model.buyerPostalCode,
                            Date = DateTime.Now,
                            //Number = model.number,
                            AttachmentFile = model.attachmentFile,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        };
                        db.CompanyInvoices.Add(item);
                        foreach (var value in model.companyInvoiceDetailViewModel)
                        {
                            var detail = new CompanyInvoiceDetail()
                            {
                                CompanyInvoice = item,
                                Carat = value.carat,
                                DescriptionProduct = value.descriptionProduct,
                                Gram = value.gram,
                                IdentificationCode = value.identificationCode,
                                StonePrice = value.stonePrice,
                                StoneWeight = value.stoneWeight,
                                Wage = value.wage,
                                Whistle = value.whistle,
                                GoldPrice = value.goldPrice,
                            };
                            db.CompanyInvoiceDetails.Add(detail);
                        }
                    }
                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "عملیات با موفقیت انجام شد."
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
        /// لیست فاکتور
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult Search(CompanyInvoiceSearchViewModel model)

        {
            var currentUser = GetAuthenticatedUser();
            Response response;
            int dataCount;
            try
            {
                var customerCount = 0;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CompanyInvoices.Select(x => x);
                    if (User.IsInRole("admin"))
                    {
                        query = query.Select(x => x);
                    }
                    else
                    {
                        query = query.Where(x => x.BranchId == currentUser.BranchId);
                    }
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.CreateDate).Skip(model.page * model.count).Take(model.count);

                    var list = query.Select(x => new CompanyInvoiceSearchViewModel
                    {
                        id = x.Id,
                        buyerName = x.BuyerName,
                        date = x.CreateDate,
                        descriptionProduct = x.CompanyInvoiceDetailList.Select(y => y.DescriptionProduct).FirstOrDefault(),
                        identificationCode = x.CompanyInvoiceDetailList.Select(y => y.IdentificationCode).FirstOrDefault(),
                        wage = x.CompanyInvoiceDetailList.Select(y => y.Wage).FirstOrDefault(),
                        buyerEconomicalNumber = x.BuyerEconomicalNumber,
                        attachmentFile = x.AttachmentFile,
                        branchName = x.Branch.Name,
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.date);
                        x.wageSeparator = Core.ToSeparator(x.wage);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            customerCount = Core.ToSeparator(customerCount),
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
        /// خواندن اطلاعات فاکتور
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون اطلاعات لود شده پرسنل</returns>
        [HttpGet]
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult Load(int id)
        {
            var user = GetAuthenticatedUser();
            Response response;
            try
            {
                CompanyInvoice item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.CompanyInvoices.FirstOrDefault(x => x.Id == id);
                    var data = new CompanyInvoiceViewModel
                    {
                        id = item.Id,
                        buyerAddress = item.BuyerAddress,
                        reduction = item.Reduction,
                        buyerEconomicalNumber = item.BuyerEconomicalNumber,
                        buyerName = item.BuyerName,
                        buyerNationalId = item.BuyerNationalId,
                        buyerPhone = item.BuyerPhone,
                        buyerPostalCode = item.BuyerPostalCode,
                        date = DateUtility.GetPersianDate(item.Date),
                        companyInvoiceDetailViewModel = item.CompanyInvoiceDetailList.Select(x => new CompanyInvoiceDetailViewModel()
                        {
                            detailId = x.Id,
                            carat = x.Carat,
                            descriptionProduct = x.DescriptionProduct,
                            gram = x.Gram,
                            identificationCode = x.IdentificationCode,
                            stonePrice = x.StonePrice,
                            stoneWeight = x.StoneWeight,
                            wage = x.Wage,
                            whistle = x.Whistle,
                            goldPrice = x.GoldPrice,
                        }).ToList(),
                    };
                    response = new Response
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
        /// حذف فاکتور
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
                    var item = db.CompanyInvoices.Find(Id);
                    db.CompanyInvoiceDetails.RemoveRange(item.CompanyInvoiceDetailList);
                    db.CompanyInvoices.Remove(item);

                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "فاکتور با موفقیت حذف شد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveFile(CompanyInvoiceViewModel model)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                string oldFileName = "";
                using (var db = new KiaGalleryContext())
                {
                    if (model.id > 0)
                    {
                        var entity = db.CompanyInvoices.SingleOrDefault(x => x.Id == model.id);
                        entity.AttachmentFile = model.attachmentFile;
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                        if (!string.IsNullOrEmpty(entity.AttachmentFile) && entity.AttachmentFile != model.attachmentFile)
                            oldFileName = entity.FileName;
                    }
                    else
                    {
                        var entity = new CompanyInvoice()
                        {
                            AttachmentFile = model.attachmentFile,
                            CreateUserId = currentUser.Id,
                            ModifyUserId = currentUser.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        };
                        db.CompanyInvoices.Add(entity);
                    }
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(oldFileName) && System.IO.File.Exists(Server.MapPath("~/Upload/companyInvoice/" + oldFileName)))
                        System.IO.File.Delete(Server.MapPath("~/Upload/companyInvoice/" + oldFileName));
                }
                response = new Response()
                {
                    status = 200,
                    message = "عملیات با موفقیت ثبت شد"
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// چاپ برگه گزارش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult PrintReport(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                var data = db.CompanyInvoiceDetails.Where(x => x.CompanyInvoiceId == id).ToList();

                int count = (int)(data.Count() / 4);

                if (data.Count() % 4 != 0) count++;
                List<StiReport> repo = new List<StiReport>();

                for (int i = 0; i < count; i++)
                {
                    var dataForPrint = data.Skip(i * 4).Take(4).ToList();

                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Carat");
                    dataTable.Columns.Add("Wage");
                    dataTable.Columns.Add("Gram");
                    dataTable.Columns.Add("DescriptionProduct");
                    dataTable.Columns.Add("IdentificationCode");
                    dataTable.Columns.Add("StonePrice");
                    dataTable.Columns.Add("StoneWeight");
                    dataTable.Columns.Add("Whistle");
                    dataTable.Columns.Add("GoldPrice");
                    dataTable.Columns.Add("Percent");
                    dataTable.Columns.Add("SumPrice");
                    
                    var rowNumber = 1;
                    double whistle = 0;
                    int gram = 0;
                    double Percent = 0.07;
                    int stonePrice = 0;
                    int goldPrice = 0;
                    int wage = 0;
                    List<string> sum = new List<string>();
                    for (int j = 0; j < dataForPrint.Count; j++)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = j + 1;
                        row["Carat"] = dataForPrint[j].Carat.ToString().ToPersianNumber();
                        row["Wage"] = dataForPrint[j].Wage.ToString("N0").ToPersianNumber();
                        row["Gram"] = dataForPrint[j].Gram.ToString().ToPersianNumber();
                        row["IdentificationCode"] = dataForPrint[j].IdentificationCode.ToPersianNumber();
                        row["StonePrice"] = dataForPrint[j].StonePrice.ToString("N0").ToPersianNumber(); ;
                        row["StoneWeight"] = dataForPrint[j].StoneWeight.ToString().ToPersianNumber();
                        row["Whistle"] = dataForPrint[j].Whistle.ToString().ToPersianNumber();
                        row["GoldPrice"] = dataForPrint[j].GoldPrice.ToString("N0").ToPersianNumber();
                        row["Percent"] = "7%";
                        whistle = data[j].Whistle;
                        whistle = whistle / 1000;

                        gram = dataForPrint[j].Gram;
                        stonePrice = Convert.ToInt32(dataForPrint[j].StonePrice);
                        goldPrice = Convert.ToInt32(dataForPrint[j].GoldPrice);
                        wage = Convert.ToInt32(dataForPrint[j].Wage);
                        var section1 = whistle + gram;
                        var section2 = goldPrice + wage;
                        var section3 = (section1 * section2) + stonePrice;
                        var section = Convert.ToInt32(section3 * Percent);
                        var phrase2 = section + section3;
                        row["SumPrice"] = phrase2.ToString("N0").ToPersianNumber();
                        sum.Add(phrase2.ToString());
                        rowNumber++;
                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);

                    StiReport report = new StiReport();
                    report.Load(Server.MapPath("~/Report/CompanyInvoice/Factor.mrt"));


                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.RegData("DataSource", dataset.Tables[0].DefaultView);
                    var branchPhone = data.Select(x => x.CompanyInvoice.Branch.Phone).FirstOrDefault();
                    var branchName = data.Select(x => x.CompanyInvoice.Branch.Name).FirstOrDefault();
                    var companyInvoice = data.Select(x => x.CompanyInvoice).FirstOrDefault();
                    var collect = sum.Sum(x => Convert.ToDecimal(x));
                    report.Dictionary.Variables["Date"].Value = DateUtility.GetPersianDate(DateTime.Now);
                    report.Dictionary.Variables["BuyerName"].Value = companyInvoice.BuyerName;
                    report.Dictionary.Variables["BuyerEconomicalNumber"].Value = companyInvoice.BuyerEconomicalNumber;
                    report.Dictionary.Variables["BuyerNationalId"].Value = companyInvoice.BuyerNationalId;
                    report.Dictionary.Variables["BuyerPhone"].Value = companyInvoice.BuyerPhone;
                    report.Dictionary.Variables["BuyerPostalCode"].Value = companyInvoice.BuyerPostalCode;
                    report.Dictionary.Variables["BuyerAddress"].Value = companyInvoice.BuyerAddress;
                    report.Dictionary.Variables["Collect"].Value = collect.ToString("N0").ToPersianNumber();
                    report.Dictionary.Variables["Reduction"].Value = companyInvoice.Reduction.ToString("N0").ToPersianNumber();
                    var DeductionDiscounts = collect + companyInvoice.Reduction;
                    report.Dictionary.Variables["DeductionDiscounts"].Value = DeductionDiscounts.ToString("N0").ToPersianNumber();
                    decimal value1 = 0.06M;
                    decimal value2 = 0.03M;
                    var Taxation = Math.Round((DeductionDiscounts * value1) + (DeductionDiscounts * value2));
                    report.Dictionary.Variables["Taxation"].Value = Taxation.ToString("N0").ToPersianNumber();
                    var Result = Math.Round(Taxation + DeductionDiscounts);
                    report.Dictionary.Variables["Result"].Value = Result.ToString("N0").ToPersianNumber();

                    report.Compile();
                    report.Render(false);

                    repo.Add(report);
                    sum = null;
                } 
                StiReport joinedReport = new StiReport();
                joinedReport.NeedsCompiling = false;
                joinedReport.IsRendered = true;
                joinedReport.RenderedPages.Clear();
                foreach (var report in repo)
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
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                return new FileStreamResult(stream, "application/pdf");
            }
        }
    }
}
