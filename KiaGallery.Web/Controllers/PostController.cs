using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Post;
using KiaGallery.Web.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر پست
    /// </summary>
    public class PostController : BaseController
    {
        /// <summary>
        /// مدیریت پست های ارسال شده
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin, post")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// مدیریت تنظیمات پست
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin, post")]
        public ActionResult Settings()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.SenderAddress = db.Settings.SingleOrDefault(x => x.Key == Model.Context.Settings.KeyPostSenderAddress)?.Value;
            }
            return View();
        }

        /// <summary>
        /// ویرایش اطلاعات پست
        /// </summary>
        /// <param name="id">ردیف اطلاعات پست</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, post-edit")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش اطلاعات پست";
            return View();
        }

        /// <summary>
        /// اطلاعات پست جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, post")]
        public ActionResult Add()
        {
            ViewBag.Title = "اطلاعات پست جدید";
            return View("Edit");
        }

        /// <summary>
        /// ذخیره اطلاعات پست
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات اطلاعات پست</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, post, post-edit")]
        public JsonResult Save(PostViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;

                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.PostItem.Single(x => x.Id == model.id);
                        entity.CityId = model.cityId;
                        entity.InvoiceNo = model.invoiceNo.Trim();
                        entity.Count = model.count;
                        entity.Weight = model.weight;
                        entity.SubmitUser = model.submitUser;
                        entity.Price = model.price;
                        entity.Customer = model.customer.Trim();
                        entity.Sex = model.sex;
                        entity.SubmitDate = DateUtility.GetDateTime(model.submitDate);
                        entity.PostDate = DateUtility.GetDateTime(model.postDate);
                        entity.PhoneNumber = model.phoneNumber.Trim();
                        entity.MobileNumber = model.mobileNumber.Trim();
                        entity.Address = model.address.Replace("_", ",").Trim();
                        entity.PostalCode = model.postalCode.Trim();
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;

                        message = "اطلاعات پست با موفقیت ویرایش شد.";
                    }
                    else
                    {
                        var entity = new PostItem()
                        {
                            CityId = model.cityId,
                            InvoiceNo = model.invoiceNo,
                            Count = model.count,
                            Weight = model.weight,
                            SubmitUser = model.submitUser,
                            Price = model.price,
                            Customer = model.customer,
                            Sex = model.sex,
                            SubmitDate = DateUtility.GetDateTime(model.submitDate),
                            PostDate = DateUtility.GetDateTime(model.postDate),
                            PhoneNumber = model.phoneNumber,
                            MobileNumber = model.mobileNumber,
                            Address = model.address.Replace("_", ",").Trim(),
                            PostalCode = model.postalCode,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        db.PostItem.Add(entity);
                        message = "اطلاعات پست با موفقیت ایجاد شد.";
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

        public JsonResult SaveRefId(int id, string trackingCode)
        {
            Response response;
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    var entity = db.PostItem.Find(id);
                    entity.TrackingCode = trackingCode;
                    Task.Factory.StartNew(() =>
                    {
                        SmsHandler.NikSmsWebServiceClient.SendSmsNik(string.Format("کیا گالری؛ \n {0} {1} کد  رهگیری مرسوله پستی شما به شرح زیر می باشد: \n {2} \n لطفا جهت رهگیری به سایت زیر مراجعه نمایید: \n {3}", entity.Sex.Value == true ? "آقای" : "خانم", entity.Customer, trackingCode, "Tracking.post.ir"), entity.MobileNumber);
                    });
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "کد پیگری با موفقیت ثبت شد",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadRefId(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.PostItem.SingleOrDefault(x => x.Id == id);
                    response = new Response()
                    {
                        status = 200,
                        data = entity.TrackingCode
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
        /// ذخیره تنظیمات پست
        /// </summary>
        /// <param name="model">مدلی حاوی تنظیمات پست</param>
        /// <returns>جیسون نتیجه ذخیره عملیات</returns>
        [HttpPost]
        [Authorize(Roles = "admin, post")]
        public JsonResult SaveSettings(PostItemSettings model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = "اطلاعات با موفقیت ثبت شد.";
                using (var db = new KiaGalleryContext())
                {
                    #region Key Post Sender Address
                    var senderAddress = db.Settings.SingleOrDefault(x => x.Key == Model.Context.Settings.KeyPostSenderAddress);
                    if (senderAddress != null)
                    {
                        senderAddress.Value = model.senderAddress;
                        senderAddress.ModifyUserId = GetAuthenticatedUserId();
                        senderAddress.ModifyDate = DateTime.Now;
                        senderAddress.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        db.Settings.Add(new Model.Context.Settings()
                        {
                            Key = Model.Context.Settings.KeyPostSenderAddress,
                            Value = model.senderAddress,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        });
                    }
                    #endregion

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
        /// خواندن اطلاعات اطلاعات پست
        /// </summary>
        /// <param name="id">ردیف پست</param>
        /// <returns>جیسون اطلاعات لود شده اطلاعات پست</returns>
        [HttpGet]
        [Authorize(Roles = "admin, post, post-edit")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                PostItem item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.PostItem.Find(id);

                    if (item != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new PostViewModel
                            {
                                id = item.Id,
                                provinceId = item.City.ParentId,
                                submitDate = DateUtility.GetPersianDate(item.SubmitDate),
                                postDate = DateUtility.GetPersianDate(item.PostDate),
                                cityId = item.CityId,
                                invoiceNo = item.InvoiceNo,
                                count = item.Count,
                                weight = item.Weight,
                                submitUser = item.SubmitUser,
                                price = item.Price,
                                customer = item.Customer,
                                phoneNumber = item.PhoneNumber,
                                mobileNumber = item.MobileNumber,
                                address = item.Address,
                                postalCode = item.PostalCode,
                                sex = item.Sex
                            }
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "اطلاعات پست مورد نظر یافت نشد."
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
        /// جستجوی اطلاعات پست
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست اطلاعات پست های پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, post")]
        public JsonResult Search(PostSearchViewModel model)
        {
            Response response;
            try
            {
                List<PostItem> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.PostItem.Include(x => x.City).Select(x => x);

                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.TrackingCode.Contains(model.term.Trim()) || x.InvoiceNo.Contains(model.term.Trim()) || x.Customer.Contains(model.term.Trim()) || x.Customer.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.PhoneNumber.Contains(model.term.Trim()) || x.MobileNumber.Contains(model.term.Trim()) || x.Address.Contains(model.term.Trim()) || x.Address.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.PostalCode.Contains(model.term.Trim()));
                    }

                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    list = query.ToList();
                }

                response = new Response()
                {
                    status = 200,

                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            cityName = item.City.Name,
                            invoiceNo = item.InvoiceNo,
                            count = item.Count,
                            weight = item.Weight,
                            customer = item.Customer,
                            submitUser = item.SubmitUser,
                            submitDate = DateUtility.GetPersianDate(item.SubmitDate),
                            postDate = DateUtility.GetPersianDate(item.PostDate),
                            trackingCode = item.TrackingCode
                        }),
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
        /// حذف اطلاعات پست
        /// </summary>
        /// <param name="id">ردیف اطلاعات پست</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, post-edit")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.PostItem.First(x => x.Id == Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "اطلاعات پست با موفقیت حذف شد."
                    };
                    db.PostItem.Remove(item);
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
        /// پرینت برگه جهت چسباندن به بسته پستی
        /// </summary>
        /// <param name="id">ردیف گزینه مورد نظر</param>
        /// <returns>فایل پی دی اف</returns>
        [Authorize(Roles = "admin, post")]
        public ActionResult Print(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                var entity = db.PostItem.Single(x => x.Id == id);

                var sender = db.Settings.SingleOrDefault(x => x.Key == Model.Context.Settings.KeyPostSenderAddress)?.Value;

                StiReport report = new StiReport();
                report.Load(Server.MapPath("~/Report/Post/Report.mrt"));
                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.Dictionary.Variables["Count"].Value = entity.Count.ToString();
                report.Dictionary.Variables["Weight"].Value = entity.Weight.ToString();
                report.Dictionary.Variables["Price"].Value = Core.ToSeparator(entity.Price);
                report.Dictionary.Variables["Customer"].Value = entity.Customer;
                report.Dictionary.Variables["Address"].Value = entity.City.Name + " - " + entity.Address;
                report.Dictionary.Variables["PhoneNumber"].Value = entity.PhoneNumber;
                report.Dictionary.Variables["MobileNumber"].Value = entity.MobileNumber;
                report.Dictionary.Variables["PostalCode"].Value = entity.PostalCode;
                report.Dictionary.Variables["Sender"].Value = sender;

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
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"Kia-Post-" + entity.Id + ".pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();

                return new FileStreamResult(stream, "application/pdf");
            }

        }

        /// <summary>
        /// چاپ کلی پرینت برای مامور پست
        /// </summary>
        /// <param name="id">لیست ردیف های مورد نظر جهت چاپ</param>
        /// <returns>فایل پی دی اف</returns>
        [Authorize(Roles = "admin, post")]
        public ActionResult PrintAll(string id)
        {
            var ids = id.Split(',').Select(x => int.Parse(x)).ToArray();

            using (var db = new KiaGalleryContext())
            {
                var listOrderItem = db.PostItem.Where(x => ids.Any(y => y == x.Id)).ToList();

                foreach (var item in listOrderItem)
                {
                    item.PostDate = DateTime.Now;
                }
                db.SaveChanges();
                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Row");
                dataTable.Columns.Add("Customer");
                dataTable.Columns.Add("Weight");
                dataTable.Columns.Add("Address");
                dataTable.Columns.Add("PostalCode");

                var rowNumber = 1;
                for (int j = 0; j < listOrderItem.Count; j++)
                {
                    DataRow row = dataTable.NewRow();
                    row["Row"] = rowNumber;
                    row["Customer"] = listOrderItem[j].Customer;
                    row["Weight"] = listOrderItem[j].Weight;
                    row["Address"] = listOrderItem[j].City.Name + "-" + listOrderItem[j].Address + "\nتلفن: " + listOrderItem[j].PhoneNumber + " - " + listOrderItem[j].MobileNumber;
                    row["PostalCode"] = listOrderItem[j].PostalCode;

                    rowNumber++;
                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);

                StiReport report = new StiReport();
                report.Load(Server.MapPath("~/Report/Post/Details.mrt"));
                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.RegData("DataSource", dataset.Tables[0].DefaultView);

                report.Dictionary.Variables["ReportDate"].Value = DateUtility.GetPersianDate(DateTime.Now);

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
        /// گرفتن لیست پرسنل شعبه آنلاین برای انتخاب کاربر ثبت کننده پست
        /// </summary>
        /// <returns>لیست پرسنل</returns>
        public JsonResult GetPostUserList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Person.Where(x => x.BranchId == 16).Select(x => new
                    {
                        id = x.Id,
                        name = x.FirstName,
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = user
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