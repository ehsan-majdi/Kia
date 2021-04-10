using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using Newtonsoft.Json;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر گزارشات
    /// </summary>
    public class ReportController : BaseController
    {
        /// <summary>
        /// صفحه گزارشات محصولات
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, report")]
        public ActionResult Product()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true && x.ProductList.Count(y => y.Active == true) > 0).ToList();
            }
            return View();
        }

        /// <summary>
        /// گزارش محصول به صورت انتخابی
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, singleReport")]
        public ActionResult ProductSingle()
        {
            return View();
        }

        /// <summary>
        /// گرفتن محصول به صورت تکی
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, singleReport")]
        public JsonResult GetProduct(int id)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Product.Select(x => x);
                    if (currentUser.UserType == UserType.Workshop)
                    {
                        query = query.Where(x => x.WorkshopId == currentUser.WorkshopId);
                    }
                    var data = query.Where(x => x.Id == id).Select(item => new
                    {
                        id = item.Id,
                        siteCode = item.Code,
                        bookCode = item.BookCode,
                        title = item.Title,
                        type = item.ProductType,
                    }).SingleOrDefault();
                    if (data != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new
                            {
                                id = data.id,
                                siteCode = data.siteCode,
                                bookCode = data.bookCode,
                                title = data.title,
                                typeTitle = Enums.GetTitle(data.type)
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
        /// گرفتن لیست محصولات
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات محصول</param>
        /// <returns>لیست محصولات</returns>
        [Authorize(Roles = "admin, singleReport")]
        public JsonResult GetProductAutoComplete(EditViewModel model)
        {
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Product.Where(x => x.Active == true);
                    if (currentUser.UserType == UserType.Workshop)
                    {
                        query = query.Where(x => x.WorkshopId == currentUser.WorkshopId);
                    }
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

        /// <summary>
        /// ساختن گزارش محصول و چاپ
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات گزارش</param>
        /// <returns>پی دی اف گزارش</returns>
        [Authorize(Roles = "admin, report")]
        public ActionResult MakeProductReport(string model)
        {
            ProductReportViewModel modelData = JsonConvert.DeserializeObject<ProductReportViewModel>(model);
            using (var db = new KiaGalleryContext())
            {
                List<ProductType> listEnum = modelData.idList.Select(x => (ProductType)x).ToList();
                var result = db.Product.Where(x => x.WorkshopId == modelData.workshop && listEnum.Any(y => y == x.ProductType)).OrderByDescending(x => x.BookCode).Select(x => new
                {
                    FileName = x.ProductFileList.FirstOrDefault(y => y.FileType == Model.FileType.WhiteBack).FileName,
                    x.BookCode,
                    x.Title
                }).ToList();
                #region Print
                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));
                List<StiReport> reports = new List<StiReport>();
                List<ProductReportPrintViewModel> dataPrint = new List<ProductReportPrintViewModel>();
                foreach (var item in result)
                {
                    var itemPrint = new ProductReportPrintViewModel()
                    {
                        Image = string.IsNullOrEmpty(item.FileName) ? defaultImage : GetProductFileByte(item.FileName),
                        Title = item.Title,
                        BookCode = item.BookCode
                    };
                    dataPrint.Add(itemPrint);
                }
                dataPrint = dataPrint.OrderByDescending(x => x.BookCode).ToList();
                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Image", typeof(byte[]));
                dataTable.Columns.Add("Title");
                dataTable.Columns.Add("BookCode");
                for (int j = 0; j < dataPrint.Count; j++)
                {
                    DataRow row = dataTable.NewRow();
                    row["Image"] = dataPrint[j].Image;
                    row["Title"] = dataPrint[j].Title;
                    row["BookCode"] = dataPrint[j].BookCode;
                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);
                StiReport singleReport = new StiReport();
                if (modelData.type == 0)
                    singleReport.Load(Server.MapPath("~/Report/Report/ProductReportDetail.mrt"));
                else
                    singleReport.Load(Server.MapPath("~/Report/Report/ProductReport.mrt"));

                singleReport.Dictionary.Databases.Clear();
                singleReport.ScriptLanguage = StiReportLanguageType.CSharp;
                singleReport.RegData("DataSource", dataset.Tables[0].DefaultView);

                singleReport.Dictionary.Variables["ProductType"].Value = string.Join("، ", listEnum.Select(x => Enums.GetTitle(x)).ToArray());


                //report.Dictionary.Synchronize();
                singleReport.Compile();
                singleReport.Render(false);

                reports.Add(singleReport);

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();
                settings.ImageQuality = 1f;
                settings.ImageResolution = 300;
                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(singleReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        /// <summary>
        /// ساختن گزارش تکی محصول 
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات محصولات انتخاب شده</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, singleReport")]
        public ActionResult MakeSingleProductReport(string model)
        {
            ProductReportViewModel modelData = JsonConvert.DeserializeObject<ProductReportViewModel>(model);
            var id = modelData.id;
            using (var db = new KiaGalleryContext())
            {
                var result = db.Product.Where(x => id.Any(y => y == x.Id)).Select(x => new
                {
                    FileName = x.ProductFileList.FirstOrDefault(y => y.FileType == Model.FileType.WhiteBack).FileName,
                    x.BookCode,
                    x.Title
                }).ToList();

                #region Prints

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();

                List<ProductReportPrintViewModel> dataPrint = new List<ProductReportPrintViewModel>();
                foreach (var item in result)
                {
                    var itemPrint = new ProductReportPrintViewModel()
                    {
                        Image = string.IsNullOrEmpty(item.FileName) ? defaultImage : GetProductFileByte(item.FileName),
                        Title = item.Title,
                        BookCode = item.BookCode
                    };

                    dataPrint.Add(itemPrint);
                }

                dataPrint = dataPrint.OrderByDescending(x => x.BookCode).ToList();

                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Image", typeof(byte[]));
                dataTable.Columns.Add("Title");
                dataTable.Columns.Add("BookCode");

                for (int j = 0; j < dataPrint.Count; j++)
                {
                    DataRow row = dataTable.NewRow();
                    row["Image"] = dataPrint[j].Image;
                    row["Title"] = dataPrint[j].Title;
                    row["BookCode"] = dataPrint[j].BookCode;

                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);

                StiReport singleReport = new StiReport();
                if (modelData.type == 0)
                    singleReport.Load(Server.MapPath("~/Report/Report/ProductReportDetail.mrt"));
                else
                    singleReport.Load(Server.MapPath("~/Report/Report/ProductReport.mrt"));

                singleReport.Dictionary.Databases.Clear();
                singleReport.ScriptLanguage = StiReportLanguageType.CSharp;
                singleReport.RegData("DataSource", dataset.Tables[0].DefaultView);
                //singleReport.Dictionary.Variables["ProductType"].Value = Enums.GetTitle(modelData.productType);
                //report.Dictionary.Synchronize();
                singleReport.Compile();
                singleReport.Render(false);
                reports.Add(singleReport);
                MemoryStream stream = new MemoryStream();
                StiPdfExportSettings settings = new StiPdfExportSettings();
                settings.ImageQuality = 1f;
                settings.ImageResolution = 300;
                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(singleReport, stream, settings);
                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        /// <summary>
        /// چاپ گزارش مشخصات پرسنل
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viemodel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, report")]
        public ActionResult MakePersonReport(string model)
        {
            PersonReportViewModel modelData = JsonConvert.DeserializeObject<PersonReportViewModel>(model);
            using (var db = new KiaGalleryContext())
            {
                var query = db.Person.Select(x => x);

                var result = query.Where(x => x.BranchId == modelData.branchId).Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.PersonNumber,
                    x.MobileNumber,

                }).ToList();

                #region Print
                List<StiReport> reports = new List<StiReport>();

                List<PersonReportPrintViewModel> dataPrint = new List<PersonReportPrintViewModel>();
                foreach (var item in result)
                {
                    var itemPrint = new PersonReportPrintViewModel()
                    {

                        firstName = item.FirstName,
                        lastName = item.LastName,
                        personNumber = item.PersonNumber,
                        mobileNumber = item.MobileNumber
                    };

                    dataPrint.Add(itemPrint);
                }

                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("FirstName");
                dataTable.Columns.Add("LastName");
                dataTable.Columns.Add("PersonNumber");
                dataTable.Columns.Add("MobileNumber");


                for (int j = 0; j < dataPrint.Count; j++)
                {
                    DataRow row = dataTable.NewRow();
                    row["FirstName"] = dataPrint[j].firstName;
                    row["LastName"] = dataPrint[j].lastName;
                    row["PersonNumber"] = dataPrint[j].personNumber;
                    row["MobileNumber"] = dataPrint[j].mobileNumber;


                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);

                StiReport singleReport = new StiReport();
                singleReport.Load(Server.MapPath("~/Report/PersonReport/PersonReportDetail.mrt"));

                singleReport.Dictionary.Databases.Clear();
                singleReport.ScriptLanguage = StiReportLanguageType.CSharp;
                singleReport.Dictionary.Variables["ReportDate"].Value = DateUtility.GetPersianDate(DateTime.Now);
                

                singleReport.RegData("DataSource", dataset.Tables[0].DefaultView);
                //report.Dictionary.Synchronize();
                singleReport.Compile();
                singleReport.Render(false);

                reports.Add(singleReport);

                MemoryStream stream = new MemoryStream();

                StiPdfExportSettings settings = new StiPdfExportSettings();
                settings.ImageQuality = 1f;
                settings.ImageResolution = 300;
                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(singleReport, stream, settings);

                this.Response.Buffer = true;
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                #endregion
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        public ActionResult ProductLicence()
        {
            return View();
        }


   

        /// <summary>
        /// دریافت آرایه بایت تصویر برای محصول
        /// </summary>
        /// <param name="fileName">نام فایل</param>
        /// <returns>آرایه بایت شده تصویر</returns>
        private byte[] GetProductFileByte(string fileName)
        {
            var filePath = string.Format("~/upload/product/{0}", fileName);
            Image image = Image.FromFile(Server.MapPath(filePath));
            var resizedImage = BitmapUtility.FixedSize(image, 500, 500, true);
            return BitmapUtility.ImageToByteArray(resizedImage);
        }

    }
}