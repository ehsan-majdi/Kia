using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// ثبت موجودی طلا در دفترمرکزی
    /// </summary>
    public class InventoryReportMemberController : BaseController
    {
        /// GET: InventortyReportMember
        /// <summary>
        /// صفحه اصلی
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, inventoryReport")]
        public ActionResult Index()
        {
            PersianCalendar helper = new PersianCalendar();
            var year = helper.GetYear(DateTime.Now);
            List<int> yearList = new List<int>();
            for (int i = year - 5; i <= year + 5; i++)
            {
                yearList.Add(i);
            }
            ViewBag.Year = year;
            ViewBag.YearList = yearList;

            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
            }
            return View();
        }
        /// <summary>
        /// لیست عناوین موجودی
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, inventoryReport")]
        public ActionResult ListDailyInventory()
        {
            return View();
        }
        /// <summary>
        /// ویرایش عناوین موجودی
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, inventortyReport")]
        /// 
        public ActionResult EditDailyInventory(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// افزودن عنوان
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, inventoryReport")]
        public ActionResult AddDailyInventory()
        {
            return View("EditDailyInventory");
        }
        /// <summary>
        /// ذخیره مقادیر جزئیات موجودی طلا اعم از عنوان،وزن،تعداد،ترتیب برای هرروز تعیین شده در تقویم
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin, inventoryReport")]
        public JsonResult SaveDailyInventory(DailyInventoryDetailViewModel model)
        {
            Response response;
            try
            {
                int id = 0;
                using (var db = new KiaGalleryContext())
                {
                    string message = string.Empty;
                    var userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {
                        id = model.id.Value;
                        var inventory = db.InventoryDetail.Single(x => x.Id == model.id);
                        inventory.Id = model.id.Value;
                        inventory.CategoryInventoryReportMemberId = model.categoryInventoryReportMemberId;
                        inventory.Order = model.order;
                        inventory.Title = model.title;
                        inventory.Weight = model.weight;
                        inventory.Count = model.count;
                        inventory.Date = DateUtility.GetDateTime(model.persianDate);
                        inventory.ModifyUserId = userId;
                        inventory.ModifyDate = DateTime.Now;
                        inventory.Ip = Request.UserHostAddress;

                        message = "دسته بندی با موفقیت ویرایش شد.";

                        db.SaveChanges();
                    }
                    else
                    {
                        var entity = new InventoryDetail()
                        {
                            CategoryInventoryReportMemberId = model.categoryInventoryReportMemberId,
                            Order = model.order,
                            Title = model.title,
                            Weight = model.weight,
                            Count = model.count,
                            Date = DateUtility.GetDateTime(model.persianDate),
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        };
                        db.InventoryDetail.Add(entity);
                        message = "دسته بندی با موفقیت ایجاد شد.";
                        db.SaveChanges();
                        id = entity.Id;
                    }

                }
                response = new Response()
                {
                    data = new InventoryDetail
                    {
                        Id = id
                    },
                    status = 200,
                    message = "عنوان جدید ثبت شد",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ذخیره عنوان
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin, inventoryReport")]
        public JsonResult SaveDetail(InfoInventoryViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var date = DateUtility.GetDateTime(model.date);
                    var userId = GetAuthenticatedUserId();
                    model.dailyInventoryDetailViewModelList.ForEach(x =>
                    {
                        if (db.InventoryDetail.Where(y => y.CategoryInventoryReportMemberId == x.categoryInventoryReportMemberId && y.Date == date).Count() > 0)
                        {
                            var entity = db.InventoryDetail.Where(y => y.CategoryInventoryReportMemberId == x.categoryInventoryReportMemberId && y.Date == date).Single();
                            entity.Count = x.count;
                            entity.Weight = x.weight;
                            entity.Title = x.title;
                            entity.ModifyUserId = userId;
                            entity.ModifyDate = DateTime.Now;
                        }
                        else
                        {
                            var item = new InventoryDetail()
                            {
                                Count = x.count,
                                CategoryInventoryReportMemberId = x.categoryInventoryReportMemberId,
                                Weight = x.weight,
                                Title = x.title,
                                Date = date,
                                CreateUserId = userId,
                                ModifyUserId = userId,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress,
                            };
                            db.InventoryDetail.Add(item);
                        }

                    });
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "عنوان جدید ثبت شد",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// خواندن اطلاعات دسته بندی موجودی انبار
        /// </summary>
        /// <param name="id">ردیف </param>
        /// <returns>جیسون اطلاعات لود شده </returns>
        [HttpGet]
        [Authorize(Roles = "admin, inventoryReport")]
        public JsonResult LoadDetailCategory(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.CategoryInventoryReportMember.Single(x => x.Id == id);
                    var data = new CategoryInventoryReportViewModel()
                    {
                        id = entity.Id,
                        title = entity.Title,
                        order = entity.Order,
                        active = entity.Active,
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
        /// خواندن اطلاعات 
        /// </summary>
        /// <param name="id">ردیف </param>
        /// <returns>جیسون اطلاعات لود شده </returns>
        [HttpGet]
        [Authorize(Roles = "admin, inventoryReport")]
        public JsonResult LoadDetail(string date)
        {
            Response response;
            try
            {
                var dateTime = DateUtility.GetDateTime(date);
                List<InventoryDetail> item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.InventoryDetail.Where(x => DbFunctions.TruncateTime(x.Date) == dateTime).ToList();
                }
                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new InventoryReportMembersViewModel
                        {
                            dailyInventoryDetailViewModelList = item.Select(x => new DailyInventoryDetailViewModel()
                            {
                                count = x.Count,
                                categoryInventoryReportMemberId = x.CategoryInventoryReportMemberId,
                                weight = x.Weight,

                            }).ToList(),
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "گزارش مورد نظر یافت نشد."
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
        /// لیست عناوین
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, inventoryReport")]
        public JsonResult SearchDailyInventory(InventoryReportMembersSearchViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CategoryInventoryReportMember.Select(x => x);
                    dataCount = query.Count();
                    query = query.OrderBy(x => x.Order).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        title = x.Title,
                        order = x.Order,
                        active=x.Active,
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list.Select(x => new
                            {
                                id = x.id,
                                title = x.title,
                                order = x.order,
                                active=x.active,

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

        /// <summary>
        /// حذف جزئیات موجودی اعم از ترتیب،عنوان،وزن،تعداد موجودی طلا
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, inventoryReport")]
        public JsonResult DeleteDailyInventory(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.InventoryDetail.Find(Id);
                    response = new Response()
                    {
                        status = 200,
                        message = "عنوان با موفقیت حذف شد."
                    };
                    db.InventoryDetail.Remove(item);
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
        /// حذف دسته بندی
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, inventoryReport")]
        public JsonResult DeleteCategory(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.CategoryInventoryReportMember.Find(Id);
                    response = new Response()
                    {
                        status = 200,
                        message = "عنوان با موفقیت حذف شد."
                    };
                    db.CategoryInventoryReportMember.Remove(item);
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
        /// خواندن عناوین
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون اطلاعات لود شده سوالات</returns>
        [HttpGet]
        [Authorize(Roles = "admin, inventoryReport")]
        public JsonResult LoadDailyInventory(int id)
        {
            Response response;
            try
            {
                CategoryInventoryReportMember item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.CategoryInventoryReportMember.FirstOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new CategoryInventoryReportViewModel
                            {
                                id = item.Id,
                                order = item.Order,
                                title = item.Title,
                                active = item.Active,
                            }
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "عنوان مورد نظر یافت نشد."
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
        /// تقویم
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, inventoryReport")]
        public ActionResult Calendar()
        {
            PersianCalendar helper = new PersianCalendar();
            var year = helper.GetYear(DateTime.Now);
            List<int> yearList = new List<int>();
            for (int i = year - 5; i <= year + 5; i++)
            {
                yearList.Add(i);
            }
            ViewBag.Year = year;
            ViewBag.YearList = yearList;

            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true && x.BranchType == BranchType.Branch).OrderBy(x => x.Order).ToList();
            }
            return View();
        }

        /// <summary>
        /// تقویم گزارش روزانه 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, inventoryReport")]
        public ActionResult Archive()
        {
            PersianCalendar helper = new PersianCalendar();
            var year = helper.GetYear(DateTime.Now);
            List<int> yearList = new List<int>();
            for (int i = year - 5; i <= year + 5; i++)
            {
                yearList.Add(i);
            }
            ViewBag.Year = year;
            ViewBag.YearList = yearList;

            return View();
        }
        /// <summary>
        /// مشاهده تقویم موجودی گزارش طلا در دفترمرکزی
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [Route("inventoryReportMember/reportDaily/{date}")]
        public ActionResult ReportDaily(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                date = date.Replace("-", "/");
            }

            DateTime? dateTime = DateUtility.GetDateTime(date);
            ViewBag.Date = date;
            using (var db = new KiaGalleryContext())
            {
                ViewBag.InventoryReportMembers = db.CategoryInventoryReportMember.Where(x => x.CreateDate == dateTime).Select(x => x).OrderBy(x => x.Order).SingleOrDefault();
                ViewBag.message = "اطلاعاتی ثبت نگردید است";
            }
            return View();
        }
        /// <summary>
        /// نمایش آیکون دانلود در تقویم مخصوص روزهایی که موجودی ثبت شده است.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, inventortyReport")]
        public JsonResult ReturnDataValue(InfoInventoryViewModel model)
        {
            Response response;
            try
            {
                var fromDate = DateUtility.GetDateTime(model.date);
                var firstDayOfMonth = new DateTime(fromDate.Value.Year, fromDate.Value.Month, 1);
                fromDate = firstDayOfMonth;
                string[] datePart = model.date.Split('/');
                string Month = datePart[1];
                int numberMonth = Convert.ToInt32(Month);
                var toDate = fromDate.Value.AddDays(30);
                if (numberMonth <= 6)
                {
                    toDate = fromDate.Value.AddDays(31);
                }

                using (var db = new KiaGalleryContext())
                {
                    List<DailyInventoryDetailViewModel> item;
                    item = db.InventoryDetail.Where(x => x.CreateDate >= fromDate && x.CreateDate <= toDate).Select(x => new DailyInventoryDetailViewModel
                    {
                        date = x.Date,
                    }).ToList();
                    item.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.date);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = item,
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
        /// ذخیره دسته بندی
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات دسته بندی</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin , inventoryReport")]
        public JsonResult SaveCategory(CategoryInventoryReportViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    int userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.CategoryInventoryReportMember.Single(x => x.Id == model.id);
                        entity.Order = model.order;
                        entity.Title = model.title;
                        entity.Active = model.active;
                        entity.ModifyUserId = userId;
                        entity.ModifyDate = DateTime.Now;

                        message = "دسته بندی با موفقیت ویرایش شد.";
                    }
                    else
                    {
                        var entity = new CategoryInventoryReportMember()
                        {
                            Title = model.title,
                            Order = model.order,
                            Active = model.active,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        };
                        db.CategoryInventoryReportMember.Add(entity);
                        message = "دسته بندی با موفقیت ایجاد شد.";
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
        /// برگرداندن لیست دسته بندی در صفحه
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , inventoryReport")]
        public JsonResult GetListCategory()
        {
            Response response;
            try
            {
                List<CategoryInventoryReportViewModel> list;
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.CategoryInventoryReportMember.Where(x => x.Active == true).Select(x => x).OrderBy(x => x.Order);
                    list = entity.Select(x => new CategoryInventoryReportViewModel()
                    {
                        id = x.Id,
                        title = x.Title,
                        order = x.Order,
                        active = x.Active,

                    }).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list
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
        /// برگرداندن لیست جزئیات عنوان ، وزن ، تعداد در صفحه
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, inventoryReport")]
        public JsonResult GetListInventoryDetail(int id)
        {
            Response response;
            try
            {
                List<DailyInventoryDetailViewModel> list;
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.InventoryDetail.Where(x => x.CategoryInventoryReportMemberId == id).Select(x => x).OrderBy(x => x.Order);
                    list = entity.Select(x => new DailyInventoryDetailViewModel()
                    {
                        id = x.Id,
                        categoryInventoryReportMemberId = x.CategoryInventoryReportMemberId,
                        title = x.Title,
                        order = x.Order,
                        count = x.Count,
                        weight = x.Weight,

                    }).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list
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
        /// چاپ برگه گزارش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("inventoryReportMember/PrintReport/{date?}")]
        public ActionResult PrintReport(string date)
        {
            using (var db = new KiaGalleryContext())
            {
                if (string.IsNullOrEmpty(date))
                    date = DateUtility.GetPersianDate(DateTime.Now);
                date = date.Replace("-", "/");

                var persianDate = DateUtility.GetDateTime(date);
                var Inventory = db.InventoryDetail.Where(x => DbFunctions.TruncateTime(x.Date) == persianDate && x.CategoryInventoryReportMember.Active == true).ToList();
                db.SaveChanges();
                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Row");
                dataTable.Columns.Add("Name");
                dataTable.Columns.Add("Category");
                dataTable.Columns.Add("Weight");
                dataTable.Columns.Add("Count");
                var rowNumber = 1;
                for (int j = 0; j < Inventory.Count; j++)
                {
                    DataRow row = dataTable.NewRow();
                    row["Row"] = rowNumber;
                    row["Name"] = Inventory[j].Title;
                    row["Category"] = Inventory[j].CategoryInventoryReportMember.Title;
                    row["Weight"] = Inventory[j].Weight.ToString("N0").ToPersianNumber();
                    row["Count"] = Inventory[j].Count.ToString("N0").ToPersianNumber();
                    rowNumber++;
                    dataTable.Rows.Add(row);
                }
                dataset.Tables.Add(dataTable);
                StiReport report = new StiReport();
                report.Load(Server.MapPath("~/Report/PrintReportInventory/PrintReport.mrt"));
                var currentUser = GetAuthenticatedUser();
                report.Dictionary.Variables["Date"].Value = DateUtility.GetPersianDate(persianDate).ToPersianNumber();
                var sumCount = Inventory.Sum(x => x.Count);
                var sumWeight = Inventory.Sum(x => x.Weight);
                var category = db.CategoryInventoryReportMember.Where(x => x.Active == true).Select(x => x.Title);
                report.Dictionary.Variables["SumCount"].Value = sumCount.ToString("N0").ToPersianNumber();
                report.Dictionary.Variables["SumWeight"].Value = sumWeight.ToString("#,#,#,#,#.00#").ToPersianNumber();
                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.RegData("DataSource", dataset.Tables[0].DefaultView);
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
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"برگه موجودی طلا-" + DateUtility.GetPersianDate(DateTime.Now) + ".pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                return new FileStreamResult(stream, "application/pdf");
            }
        }
    }
}
