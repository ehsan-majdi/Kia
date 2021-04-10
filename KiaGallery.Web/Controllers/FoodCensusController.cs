using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using Newtonsoft.Json;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر سرشماری غذا
    /// </summary>
    public class FoodCensusController : BaseController
    {
        /// <summary>
        /// تقویم
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin,foodCensus")]
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
                ViewBag.foodCount = db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyFoodCount)?.Value;
                ViewBag.appetizerCount = db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyAppetizerCount)?.Value;
                ViewBag.description = db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyDescription)?.Value;
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult SettingSave(FoodSettingViewModel model)
        {
            Response response;
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    var foodCount = db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyFoodCount);
                    var appetizerCount = db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyAppetizerCount);
                    var description = db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyDescription);

                    foodCount.Value = model.foodCount;
                    appetizerCount.Value = model.appetizerCount;
                    description.Value = model.description;

                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "اطلاعات با موفقیت ثبت شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ذخیره کردن مقادیر ثبت غذا
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin,foodCensus")]
        public JsonResult Save(FoodCensusViewModel model)
        {
            Response response;
            try
            {
                if (model.holiday == null)
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "لطفا روز تعطیل را انتخاب کنید",
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var date = DateUtility.GetDateTime(model.date);
                    if (model.id > 0 && model.id != null)
                    {
                        var entity = db.FoodCensus.Where(x => x.Id == model.id).SingleOrDefault();
                        entity.Date = DateUtility.GetDateTime(model.date);
                        entity.FoodName = model.foodName;
                        entity.TypeFood = model.typeFood;
                        entity.Holiday = model.holiday.Value;
                        entity.ModifyUserId = user.Id;
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new FoodCensus()
                        {
                            Date = DateUtility.GetDateTime(model.date),
                            FoodName = model.foodName,
                            Holiday = model.holiday.Value,
                            TypeFood = model.typeFood,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.FoodCensus.Add(item);
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
        /// نمایش آیکون دانلود در تقویم مخصوص روزهایی که موجودی ثبت شده است.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,foodCensus,foodRegistration")]
        public JsonResult ReturnDataValue(FoodCensusValueViewModel model)
        {
            Response response;
            try
            {
                var fromDate = DateUtility.GetDateTime(model.persianFirstDayOfMonth);
                var toDate = DateUtility.GetDateTime(model.persianLastDayOfMonth);
                var nextTommorow = DateTime.Now.AddDays(2);
                var nextDay = DateTime.Now.AddDays(1);
                using (var db = new KiaGalleryContext())
                {
                    var userId = GetAuthenticatedUserId();
                    List<FoodCensusValueViewModel> item;

                    var entity = db.FoodCensus.Where(x => x.Date >= fromDate && x.Date <= toDate).Select(x => new FoodCensusViewModel
                    {
                        id = x.Id,
                        boolDate = x.Date >= nextTommorow ? true : false,
                        boolCheckingDate = x.Date >= nextDay ? true : false,
                        dateTime = x.Date,
                        holiday = x.Holiday,
                        typeFood = x.TypeFood,
                        foodName = !string.IsNullOrEmpty(x.FoodName) ? x.FoodName : string.Empty,
                        food = x.FoodRegistrationList.Where(y => y.UserId == userId).FirstOrDefault().Food,
                        appertizer = x.FoodRegistrationList.Where(y => y.UserId == userId).FirstOrDefault().Appertizer,
                        foodCount = x.FoodRegistrationList.Where(y => y.Food == true).Count(),
                        foodRegisterList = x.FoodRegistrationList.Where(y => y.UserId > 0).Count(),
                        foodRegistrationId = x.FoodRegistrationList.Where(y => y.UserId == userId).FirstOrDefault().Id > 0 ? x.FoodRegistrationList.Where(y => y.UserId == userId).FirstOrDefault().Id : 0,
                        foodWithoutRice = x.FoodRegistrationList.Where(y => y.UserId == userId).FirstOrDefault().FoodWithoutRice,
                    }).ToList();

                    entity.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.dateTime);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = entity,
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
        /// خواندن مقادیر 
        /// </summary>
        /// <param name="id">ردیف </param>
        /// <returns>جیسون اطلاعات لود شده سوالات</returns>
        [HttpGet]
        [Authorize(Roles = "admin,foodCensus")]
        public JsonResult Load(string date)
        {
            Response response;
            try
            {

                var dt = DateUtility.GetDateTime(date);
                using (var db = new KiaGalleryContext())
                {
                    var item = db.FoodCensus.Where(x => x.Date == dt).SingleOrDefault();
                    var data = new FoodCensusViewModel()
                    {
                        id = item.Id,
                        foodName = item.FoodName,
                        typeFood = item.TypeFood,
                        holiday = item.Holiday,
                        date = DateUtility.GetPersianDate(item.Date)
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
        /// چاپ برگه گزارش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Route("foodCensus/FoodCensusReport/{date?}")]
        [AllowAnonymous]
        public ActionResult PrintReport(string model)
        {
            string persianDayName = string.Empty;
            FoodCensusViewModel modelData = JsonConvert.DeserializeObject<FoodCensusViewModel>(model);
            var date = modelData.date;

            var foreignDate = DateUtility.GetDateTime(date);

            var foreignDayOfWeek = foreignDate.Value.DayOfWeek;

            switch (foreignDayOfWeek)
            {
                case DayOfWeek.Saturday:
                    persianDayName = "شنبه";
                    break;
                case DayOfWeek.Sunday:
                    persianDayName = "یکشنبه";
                    break;
                case DayOfWeek.Monday:
                    persianDayName = "دوشنبه";
                    break;
                case DayOfWeek.Tuesday:
                    persianDayName = "سه شنبه";
                    break;
                case DayOfWeek.Wednesday:
                    persianDayName = "چهارشنبه";
                    break;
                case DayOfWeek.Thursday:
                    persianDayName = "پنج شنبه";
                    break;
                default:
                    persianDayName = "چنین روزی موجود نیست";
                    break;

            }
            using (var db = new KiaGalleryContext())
            {
                var persianDate = DateUtility.GetDateTime(date);
                var userList = db.User.Where(x => x.RoleList.Any(y => y.Title == "foodRegistration") && x.Active == true).ToList();
                var foodCensus = db.FoodRegistration.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(persianDate)).Select(x => new
                {
                    userId = x.UserId,
                    x.Appertizer,
                    food = x.Food,
                    x.FoodWithoutRice,

                }).ToList();

                List<StiReport> reports = new List<StiReport>();
                List<FoodCensusViewModel> dataPrint = new List<FoodCensusViewModel>();
                foreach (var item in userList)
                {
                    var itemPrint = new FoodCensusViewModel()
                    {
                        userName = item.FullName,
                        appertizer = foodCensus.FirstOrDefault(x => x.userId == item.Id)?.Appertizer ?? false,
                        food = foodCensus.FirstOrDefault(x => x.userId == item.Id)?.food ?? false,
                        foodWithoutRice = foodCensus.FirstOrDefault(x => x.userId == item.Id)?.FoodWithoutRice ?? false,
                    };
                    dataPrint.Add(itemPrint);
                }
                DataSet dataset = new DataSet("DataSource");
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Row");
                dataTable.Columns.Add("User");
                dataTable.Columns.Add("Appetizer");
                dataTable.Columns.Add("FoodStatus");
                dataTable.Columns.Add("FoodWithoutRice");
                dataTable.Columns.Add("Row2");
                dataTable.Columns.Add("User2");
                dataTable.Columns.Add("Appetizer2");
                dataTable.Columns.Add("FoodStatus2");
                dataTable.Columns.Add("FoodWithoutRice2");

                var rowNumber = 1;
                for (int j = 0; j < dataPrint.Count; j = j + 2)
                {
                    DataRow row = dataTable.NewRow();
                    row["Row"] = j + 1;
                    row["" + "User"] = dataPrint[j].userName;
                    row["Appetizer"] = dataPrint[j].appertizer;
                    row["FoodStatus"] = dataPrint[j].food;
                    row["FoodWithoutRice"] = dataPrint[j].foodWithoutRice;
                    if (dataPrint.Count > j + 1)
                    {
                        row["Row2"] = j + 2;
                        row["User2"] = dataPrint[j + 1].userName;
                        row["Appetizer2"] = dataPrint[j + 1].appertizer;
                        row["FoodStatus2"] = dataPrint[j + 1].food;
                        row["FoodWithoutRice2"] = dataPrint[j + 1].foodWithoutRice;
                    }
                    rowNumber++;
                    dataTable.Rows.Add(row);
                }

                dataset.Tables.Add(dataTable);
                StiReport report = new StiReport();
                report.Load(Server.MapPath("~/Report/FoodCensusReport/FoodCensusReport.mrt"));
                var currentUser = GetAuthenticatedUser();
                var foodName = db.FoodCensus.Where(x => x.Date == persianDate).FirstOrDefault();
                var foodCount = db.FoodRegistration.Where(x => x.Date == persianDate && x.Food == true).Count();
                var appetizerCount = db.FoodRegistration.Where(x => x.Date == persianDate && x.Appertizer == true).Count();
                var foodWithoutRiceCount = db.FoodRegistration.Where(x => x.Date == persianDate && x.FoodWithoutRice == true).Count();
                report.Dictionary.Variables["Date"].Value = date.ToPersianNumber();
                report.Dictionary.Variables["FoodName"].Value = foodName.FoodName;
                report.Dictionary.Variables["FoodCount"].Value = foodCount.ToString();
                report.Dictionary.Variables["AppetizerCount"].Value = appetizerCount.ToString();
                report.Dictionary.Variables["FoodWithoutRiceCount"].Value = foodWithoutRiceCount.ToString();
                report.Dictionary.Variables["Day"].Value = persianDayName;
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
                this.Response.AddHeader("Content-Disposition", "attachment; filename=\"برگه گزارش ثبت غذا-" + DateUtility.GetPersianDate(DateTime.Now) + ".pdf\"");
                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.AddHeader("Content-Length", stream.Length.ToString());
                this.Response.BinaryWrite(stream.ToArray());
                this.Response.End();
                return new FileStreamResult(stream, "application/pdf");
            }
        }

        public JsonResult GetAllRegistration(string date)
        {
            Response response;
            try
            {
                var selectedDate = DateUtility.GetDateTime(date);
                using (var db = new KiaGalleryContext())
                {
                    var queryFood = db.FoodRegistration.Where(x => x.Date == selectedDate && x.User.Active == true).ToList();
                    var foodSettingCount = int.Parse(db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyFoodCount)?.Value);
                    var appertizerSettingCount = int.Parse(db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyAppetizerCount)?.Value);
                    var listRegistration = queryFood.Select(x => new 
                    {
                        userId = x.UserId,
                        foodRegistrationId = x.Id,
                        food = x.Food,
                        appertizer = x.Appertizer,
                        foodCensusId = x.FoodCensusId,

                    }).ToList();
                    var query = db.User.Where(x => x.RoleList.Count(z => z.Title == "foodRegistration") > 0 && x.Active == true).ToList();
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        fullName = x.FirstName + " " + x.LastName,
                    }).ToList();


                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            listRegistration = listRegistration,
                            foodCount = listRegistration.Count(x => x.food == true),
                            appertizerCount = listRegistration.Count(x => x.appertizer == true),
                            sumFoodCount= foodSettingCount + listRegistration.Count(x => x.food == true),
                            sumAppertizerCount= appertizerSettingCount + listRegistration.Count(x => x.appertizer == true),
                            foodCensusId = listRegistration.Select(x => x.foodCensusId).FirstOrDefault(),
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

        [HttpPost]
        public JsonResult SendingSms(FoodDateViewModel model)
        {
            Response response;
            try
            {
                var userId = GetAuthenticatedUserId();
                var date = DateUtility.GetDateTime(model.date);
                var nextToday = DateTime.Now.AddDays(3);
                var persianDate = DateUtility.GetPersianDate(date);

                using (var db = new KiaGalleryContext())
                {
                    var food = db.FoodRegistration.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date)).ToList();

                    var foodName = db.FoodCensus.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date)).Select(x => x.FoodName).FirstOrDefault();
                    var foodWithoutRice = db.FoodRegistration.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date) && x.FoodWithoutRice == true).Count();
                    var registerFoodCount = db.FoodRegistration.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date) && x.Food == true).Count();
                    var foodSettingCount = int.Parse(db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyFoodCount)?.Value);
                    var foodSum = registerFoodCount + foodSettingCount;
                    var registerAppetizerCount = db.FoodRegistration.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date) && x.Appertizer == true).Count();
                    var appetizerSettingCount = int.Parse(db.FoodSetting.SingleOrDefault(x => x.Key == FoodSetting.KeyAppetizerCount)?.Value);
                    var apertizerSum = registerAppetizerCount + appetizerSettingCount;
                    var messageFood = db.FoodCensus.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date)).Select(x => x.TypeFood).SingleOrDefault();


                    if (date > DateTime.Now && date <= nextToday)
                    {
                        var numbers = new List<string>() { "09193121247", "09120815107", "09124254257" };
                        if (messageFood == true)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                NikSmsWebServiceClient.SendSmsNik("اصلاحیه غذا؛ \n اطلاعات غذا به تاریخ " + persianDate + "به شرح ذیل تغییر یافت.\n نام غذا:" + foodName + "\n تعداد غذا بدون برنج برای هر نفر: " + foodWithoutRice + "\n تعداد غذا با برنج برای هر نفر: " + foodSum + "\n تعداد سالاد به تعداد نفرات: " + apertizerSum + " \n با تشکر", numbers);
                            });
                        }
                        else
                        {
                            Task.Factory.StartNew(() =>
                            {
                                NikSmsWebServiceClient.SendSmsNik("اصلاحیه غذا؛ \n اطلاعات غذا به تاریخ " + persianDate + "به شرح ذیل تغییر یافت.\n نام غذا:" + foodName + "\n تعداد غذا برای هر نفر: " + foodSum + "\n تعداد سالاد به تعداد نفرات: " + apertizerSum + " \n با تشکر", numbers);
                            });
                        }
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "عملیات با موفقیت  انجام شد و پیامک در روز موعد ارسال خواهد شد."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }

                }
                response = new Response()
                {
                    status = 200,
                    message = "عملیات با موفقیت انجام شد و پیام ارسال گردید."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetFood(FoodDateViewModel model)
        {
            Response response;
            try
            {
                var userId = GetAuthenticatedUserId();
                var selectedDate = DateUtility.GetDateTime(model.date);
                using (var db = new KiaGalleryContext())
                {
                    var food = db.FoodRegistration.Where(x => x.FoodCensus.Date == selectedDate && x.UserId == model.userId && x.FoodCensusId == model.id).SingleOrDefault();
                    if (food != null)
                    {
                        food.Food = model.food;
                        food.Date = selectedDate;
                        food.FoodStatus = 0;
                        food.ModifyUserId = userId;
                        food.ModifyDate = DateTime.Now;
                        food.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new FoodRegistration()
                        {
                            Food = model.food,
                            UserId = model.userId.Value,
                            FoodCensusId = model.id.Value,
                            Date = DateUtility.GetDateTime(model.date),
                            FoodStatus = 0,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.FoodRegistration.Add(item);
                    }
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

        [HttpPost]
        public JsonResult SetAppertizer(FoodDateViewModel model)
        {
            Response response;
            try
            {
                var userId = GetAuthenticatedUserId();
                var selectedDate = DateUtility.GetDateTime(model.date);
                using (var db = new KiaGalleryContext())
                {
                    var appertizer = db.FoodRegistration.Where(x => x.FoodCensus.Date == selectedDate && x.UserId == model.userId && x.FoodCensusId == model.id).SingleOrDefault();
                    if (appertizer != null)
                    {
                        appertizer.Appertizer = model.appertizer;
                        appertizer.Date = selectedDate;
                        appertizer.FoodStatus = 0;
                        appertizer.ModifyUserId = userId;
                        appertizer.ModifyDate = DateTime.Now;
                        appertizer.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new FoodRegistration()
                        {
                            UserId = model.userId.Value,
                            FoodCensusId = model.id.Value,
                            Appertizer = model.appertizer,
                            Date = DateUtility.GetDateTime(model.date),
                            FoodStatus = 0,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.FoodRegistration.Add(item);
                    }
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
    }
}