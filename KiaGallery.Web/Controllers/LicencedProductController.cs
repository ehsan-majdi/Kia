using ClosedXML.Excel;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using Newtonsoft.Json;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class LicencedProductController : BaseController
    {
        // GET: LicencedProduct
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.GoldPrice = Core.ToSeparator(long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice)?.Value));
                ViewBag.EuroPrice = Core.ToSeparator(long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyEuroPrice)?.Value));
            }
            return View();
        }

        public ActionResult Add()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.GoldPrice = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice)?.Value;
                ViewBag.EuroPrice = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyEuroPrice)?.Value;
            }
            return View("Edit");
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            using (var db = new KiaGalleryContext())
            {
                ViewBag.GoldPrice = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice)?.Value;
                ViewBag.EuroPrice = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyEuroPrice)?.Value;
            }
            return View();
        }

        public JsonResult Save(LicencedProductViewModel model)
        {
            Response response;
            string oldFileName = "";
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (model.id > 0 && model.id != null)
                    {
                        var entity = db.LicencedProduct.Single(x => x.Id == model.id);
                        entity.ProductId = model.productId;
                        entity.Weight = model.weight;
                        entity.Color = model.color;
                        entity.Purity = "18K";
                        entity.Code = model.code;
                        entity.Barcode = model.barcode;
                        entity.FileName = model.fileName;
                        entity.Euro = model.euro;
                        entity.Wage = model.wage;
                        entity.LeatherStonePrice = model.leatherStonePrice;
                        if (!string.IsNullOrEmpty(entity.FileName) && entity.FileName != model.fileName)
                            oldFileName = entity.FileName;
                    }
                    else
                    {
                        var entity = new LicencedProduct()
                        {
                            ProductId = model.productId,
                            Weight = model.weight,
                            Color = model.color,
                            Purity = "18K",
                            Code = model.code,
                            Barcode = model.barcode,
                            FileName = model.fileName,
                            Euro = model.euro,
                            Wage = model.wage,
                            LeatherStonePrice = model.leatherStonePrice,
                        };
                        db.LicencedProduct.Add(entity);
                    }
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(oldFileName) && System.IO.File.Exists(Server.MapPath("~/Upload/LicencedProduct/" + oldFileName)))
                        System.IO.File.Delete(Server.MapPath("~/Upload/LicencedProduct/" + oldFileName));
                };
                response = new Response()
                {
                    status = 200,
                    message = "محصول با موفقیت اضافه شد."
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
                    var entity = db.LicencedProduct.Single(x => x.Id == id);

                    var data = new LicencedProductViewModel
                    {
                        id = entity.Id,
                        productId = entity.ProductId,
                        code = entity.Code,
                        wage = entity.Wage,
                        leatherStonePrice = entity.LeatherStonePrice,
                        barcode = entity.Barcode,
                        euro = entity.Euro,
                        purity = entity.Purity,
                        color = entity.Color,
                        fileName = entity.FileName,
                        weight = entity.Weight,
                        productName = entity.Product?.Title
                    };
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

        public JsonResult Search(LicencedProductSearchViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var goldPrice = long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice)?.Value);
                    var euroPrice = long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyEuroPrice)?.Value);

                    var query = db.LicencedProduct.Select(x => x);

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    var list = query.Select(x => new LicencedProductViewModel
                    {
                        id = x.Id,
                        code = x.Code,
                        barcode = x.Barcode,
                        euro = x.Euro,
                        weight = x.Weight,
                        fileName = x.Product.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName != null ? x.Product.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName : x.FileName,
                        color = x.Color,
                        purity = x.Purity,
                        wage = x.Wage,
                        leatherStonePrice = x.LeatherStonePrice,
                    }).ToList();
                    list.ForEach(x =>
                    {
                        if (x.color != "Silver" && x.color !="Silver&Beads" && x.color != "Silver&Leather")
                        {
                            x.stringPrice = Core.ToSeparator(Convert.ToInt32(((((goldPrice + x.wage) * x.weight) + ((goldPrice + x.wage) * x.weight) * 0.07) + x.leatherStonePrice.Value) + ((((goldPrice + x.wage) * x.weight) + ((goldPrice + x.wage) * x.weight) * 0.07) + x.leatherStonePrice.Value) * 0.09) - (Convert.ToInt32(((((goldPrice + x.wage) * x.weight) + ((goldPrice + x.wage) * x.weight) * 0.07) + x.leatherStonePrice.Value) + ((((goldPrice + x.wage) * x.weight) + ((goldPrice + x.wage) * x.weight) * 0.07) + x.leatherStonePrice.Value) * 0.09) % 1000));

                        }
                        else
                        {
                            x.stringPrice = Core.ToSeparator(x.leatherStonePrice.Value - x.leatherStonePrice.Value % 1000);
                        }
                        x.euroPrice = Convert.ToInt32(x.stringPrice.Replace(",", "")) / euroPrice + x.euro.Value;
                        x.stringRialPrice = Core.ToSeparator(x.rialPrice);
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

        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var entity = db.LicencedProduct.Find(id);
                    db.LicencedProduct.Remove(entity);
                    db.SaveChanges();

                }
                response = new Response()
                {
                    status = 200,
                    message = "محصول با موفقیت حذف شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSetting(SaveLicencedProductSettingViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    model.euroPrice = model.euroPrice.Replace(",", "");
                    model.goldPrice = model.goldPrice.Replace(",", "");

                    var goldPrice = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice);
                    var euroPrice = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyEuroPrice);

                    goldPrice.Value = model.goldPrice;
                    euroPrice.Value = model.euroPrice;
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200,
                    message = "Done!"
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        public ActionResult MakeProductLicenceReport(string model)
        {
            ProductReportViewModel modelData = JsonConvert.DeserializeObject<ProductReportViewModel>(model);
            var id = modelData.id;
            using (var db = new KiaGalleryContext())
            {
                var goldPrice = long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice)?.Value);
                var euroPrice = long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyEuroPrice)?.Value);
                var result = db.LicencedProduct.Where(x => id.Any(y => y == x.Id)).Select(x => new
                {
                    x.Id,
                    FileName = x.Product.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName != null ? x.Product.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName : x.FileName,
                    x.Code,
                    x.Weight,
                    x.Color,
                    x.Purity,
                    x.Wage,
                    x.LeatherStonePrice,
                    x.Euro
                }).ToList();

                #region Prints

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();

                List<LicencedProductViewModel> dataPrint = new List<LicencedProductViewModel>();
                foreach (var item in result)
                {
                    var itemPrint = new LicencedProductViewModel()
                    {
                        id = item.Id,
                        image = string.IsNullOrEmpty(item.FileName) ? defaultImage : GetProductFileByte(item.FileName),
                        code = item.Code,
                        weight = item.Weight,
                        color = item.Color,
                        purity = item.Purity,
                        wage = item.Wage,
                        leatherStonePrice = item.LeatherStonePrice,
                        euro = item.Euro,
                        stringPrice = Core.ToSeparator(Convert.ToInt32(((((goldPrice + item.Wage) * item.Weight) + ((goldPrice + item.Wage) * item.Weight) * 0.07) + item.LeatherStonePrice.Value) + ((((goldPrice + item.Wage) * item.Weight) + ((goldPrice + item.Wage) * item.Weight) * 0.07) + item.LeatherStonePrice.Value) * 0.09) - (Convert.ToInt32(((((goldPrice + item.Wage) * item.Weight) + ((goldPrice + item.Wage) * item.Weight) * 0.07) + item.LeatherStonePrice.Value) + ((((goldPrice + item.Wage) * item.Weight) + ((goldPrice + item.Wage) * item.Weight) * 0.07) + item.LeatherStonePrice.Value) * 0.09) % 1000)),

                    };

                    dataPrint.Add(itemPrint);
                }

                dataPrint = dataPrint.OrderBy(x => x.id).ToList();

                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Image", typeof(byte[]));
                dataTable.Columns.Add("Code");
                dataTable.Columns.Add("Weight");
                dataTable.Columns.Add("Purity");
                dataTable.Columns.Add("Colour");
                dataTable.Columns.Add("Price");
                dataTable.Columns.Add("Wage");
                dataTable.Columns.Add("LeatherStonePrice");
                dataTable.Columns.Add("Euro");
                dataTable.Columns.Add("StringPrice");
                dataTable.Columns.Add("EuroPrice");


                for (int j = 0; j < dataPrint.Count; j++)
                {
                    DataRow row = dataTable.NewRow();
                    row["Image"] = dataPrint[j].image;
                    row["Code"] = dataPrint[j].code;
                    row["Weight"] = dataPrint[j].weight;
                    row["Purity"] = dataPrint[j].purity;
                    row["Colour"] = dataPrint[j].color;
                    row["Wage"] = dataPrint[j].wage;
                    row["LeatherStonePrice"] = dataPrint[j].leatherStonePrice;
                    row["Euro"] = dataPrint[j].euro;
                    if (dataPrint[j].color != "Silver" && dataPrint[j].color != "Silver&Beads" && dataPrint[j].color != "Silver&Leather")
                    {
                        row["StringPrice"] = dataPrint[j].stringPrice;
                        row["EuroPrice"] = Convert.ToInt32(dataPrint[j].stringPrice.Replace(",", "")) / euroPrice + dataPrint[j].euro;
                    }
                    else
                    {
                        row["StringPrice"] = dataPrint[j].leatherStonePrice;
                        row["EuroPrice"] = Convert.ToInt32(dataPrint[j].leatherStonePrice.ToString().Replace(",", "")) / euroPrice + dataPrint[j].euro;
                    }
                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);

                StiReport singleReport = new StiReport();

                singleReport.Load(Server.MapPath("~/Report/Report/licence.mrt"));

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
                settings.ImageQuality = 1.0f;
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

        public ActionResult MakeProductLicenceReportMin(string model)
        {
            ProductReportViewModel modelData = JsonConvert.DeserializeObject<ProductReportViewModel>(model);
            var id = modelData.id;
            using (var db = new KiaGalleryContext())
            {
                var goldPrice = long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice)?.Value);
                var euroPrice = long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyEuroPrice)?.Value);
                var result = db.LicencedProduct.Where(x => id.Any(y => y == x.Id)).Select(x => new
                {
                    x.Id,
                    FileName = x.Product.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName != null ? x.Product.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName : x.FileName,
                    x.Code,
                    x.Weight,
                    x.Color,
                    x.Purity,
                    x.Wage,
                    x.LeatherStonePrice,
                    x.Euro,
                    x.Barcode,
                }).ToList();

                #region Prints

                byte[] defaultImage = System.IO.File.ReadAllBytes(Server.MapPath("~/content/image/kia-gallery-logo-square.png"));

                List<StiReport> reports = new List<StiReport>();

                List<LicencedProductViewModel> dataPrint = new List<LicencedProductViewModel>();
                foreach (var item in result)
                {
                    var itemPrint = new LicencedProductViewModel()
                    {
                        id = item.Id,
                        image = string.IsNullOrEmpty(item.FileName) ? defaultImage : GetProductFileByte(item.FileName),
                        code = item.Code,
                        weight = item.Weight,
                        color = item.Color,
                        purity = item.Purity,
                        wage = item.Wage,
                        leatherStonePrice = item.LeatherStonePrice,
                        euro = item.Euro,
                        barcode = item.Barcode,
                        stringPrice = Core.ToSeparator(Convert.ToInt32(((((goldPrice + item.Wage) * item.Weight) + ((goldPrice + item.Wage) * item.Weight) * 0.07) + item.LeatherStonePrice.Value) + ((((goldPrice + item.Wage) * item.Weight) + ((goldPrice + item.Wage) * item.Weight) * 0.07) + item.LeatherStonePrice.Value) * 0.09) - (Convert.ToInt32(((((goldPrice + item.Wage) * item.Weight) + ((goldPrice + item.Wage) * item.Weight) * 0.07) + item.LeatherStonePrice.Value) + ((((goldPrice + item.Wage) * item.Weight) + ((goldPrice + item.Wage) * item.Weight) * 0.07) + item.LeatherStonePrice.Value) * 0.09) % 1000)),

                    };
                    dataPrint.Add(itemPrint);
                }

                dataPrint = dataPrint.OrderBy(x => x.id).ToList();

                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Image", typeof(byte[]));
                dataTable.Columns.Add("Code");
                dataTable.Columns.Add("Weight");
                dataTable.Columns.Add("Purity");
                dataTable.Columns.Add("Colour");
                dataTable.Columns.Add("Price");
                dataTable.Columns.Add("Wage");
                dataTable.Columns.Add("LeatherStonePrice");
                dataTable.Columns.Add("Euro");
                dataTable.Columns.Add("StringPrice");
                dataTable.Columns.Add("EuroPrice");
                dataTable.Columns.Add("Index");
                dataTable.Columns.Add("Barcode");


                for (int j = 0; j < dataPrint.Count; j++)
                {
                    DataRow row = dataTable.NewRow();
                    row["Image"] = dataPrint[j].image;
                    row["Code"] = dataPrint[j].code;
                    row["Weight"] = dataPrint[j].weight;
                    row["Purity"] = dataPrint[j].purity;
                    row["Colour"] = dataPrint[j].color;
                    row["Wage"] = dataPrint[j].wage;
                    row["LeatherStonePrice"] = dataPrint[j].leatherStonePrice;
                    row["Euro"] = dataPrint[j].euro;
                    row["Barcode"] = dataPrint[j].barcode;
                    if (dataPrint[j].color != "Silver" && dataPrint[j].color != "Silver&Beads" && dataPrint[j].color != "Silver&Leather")
                    {
                        row["StringPrice"] = dataPrint[j].stringPrice;
                        row["EuroPrice"] = Convert.ToInt32(dataPrint[j].stringPrice.Replace(",", "")) / euroPrice + dataPrint[j].euro;
                    }
                    else
                    {
                        row["StringPrice"] = dataPrint[j].leatherStonePrice;
                        row["EuroPrice"] = Convert.ToInt32(dataPrint[j].leatherStonePrice.ToString().Replace(",", "")) / euroPrice + dataPrint[j].euro;
                    }
                    row["Index"] = modelData.listRow[j];
                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);

                StiReport singleReport = new StiReport();

                singleReport.Load(Server.MapPath("~/Report/Report/licenceMin.mrt"));

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
                settings.ImageQuality = 1.0f;
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
        [AllowAnonymous]
        public ActionResult Export()
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.LicencedProduct.Select(x => x);
                    query = query.OrderBy(x => x.Id);
                    var goldPrice = long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice)?.Value);
                    var euroPrice = long.Parse(db.Settings.SingleOrDefault(x => x.Key == Settings.KeyEuroPrice)?.Value);
                    List<LicencedProductExportModel> list = query.Select(x => new LicencedProductExportModel()
                    {
                        id = x.Id,
                        productName = x.Product.Title,
                        code = x.Code,
                        barcode = x.Barcode,
                        euro = x.Euro,
                        weight = x.Weight,
                        color = x.Color,
                        purity = x.Purity,
                        wage = x.Wage,
                        leatherStonePrice = x.LeatherStonePrice,

                    }).OrderBy(x=>x.id).ToList();
                    list.ForEach(x =>
                    {
                        if (x.color != "Silver" && x.color != "Silver&Beads" && x.color != "Silver&Leather")
                        {
                            x.stringPrice = Core.ToSeparator(Convert.ToInt32(((((goldPrice + x.wage) * x.weight) + ((goldPrice + x.wage) * x.weight) * 0.07) + x.leatherStonePrice.Value) + ((((goldPrice + x.wage) * x.weight) + ((goldPrice + x.wage) * x.weight) * 0.07) + x.leatherStonePrice.Value) * 0.09) - (Convert.ToInt32(((((goldPrice + x.wage) * x.weight) + ((goldPrice + x.wage) * x.weight) * 0.07) + x.leatherStonePrice.Value) + ((((goldPrice + x.wage) * x.weight) + ((goldPrice + x.wage) * x.weight) * 0.07) + x.leatherStonePrice.Value) * 0.09) % 1000));
                        }
                        else
                        {
                            x.stringPrice = Core.ToSeparator(x.leatherStonePrice.Value - x.leatherStonePrice.Value % 1000);
                        }
                        x.euroPrice = Convert.ToInt32(x.stringPrice.Replace(",", "")) / euroPrice + x.euro.Value;
                    });

                    DataTable tableData = new DataTable();
                    //tableData.Columns.Add("ردیف");
                    tableData.Columns.Add("کد محصول");
                    tableData.Columns.Add("نام محصول");
                    tableData.Columns.Add("قیمت محصول");
                    tableData.Columns.Add("یورو اضافی");
                    tableData.Columns.Add("قیمت یورو");
                    tableData.Columns.Add("بارکد");

                    list.ForEach(x =>
                    {
                        DataRow row = tableData.NewRow();
                        //row["ردیف"] = x.id;
                        row["کد محصول"] = x.code;
                        row["نام محصول"] = x.productName;
                        row["قیمت محصول"] = x.stringPrice;
                        row["یورو اضافی"] = x.euro;
                        row["قیمت یورو"] = x.euroPrice;
                        row["بارکد"] = x.barcode;

                        tableData.Rows.Add(row);
                    });

                    XLWorkbook wb = new XLWorkbook();
                    wb.Worksheets.Add(tableData, "ProductCertificate");
                    wb.SaveAs(Server.MapPath("~/Temp/ProductCertificate.xlsx"));

                    byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Temp/ProductCertificate.xlsx"));
                    string fileName = "ProductCertificate.xlsx";
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

        public class SaveLicencedProductSettingViewModel
        {
            public string goldPrice { get; set; }
            public string euroPrice { get; set; }

        }
        public class LicencedProductExportModel
        {
            public int id { get; set; }
            public string stringPrice { get; set; }
            public string productName { get; set; }
            public long euroPrice { get; set; }
            public string barcode { get; set; }
            public string code { get; set; }
            public long? euro { get; set; }
            public float weight { get; set; }
            public long wage { get; set; }
            public string color { get; set; }
            public string purity { get; set; }
            public long? leatherStonePrice { get; set; }

        }
    }
}