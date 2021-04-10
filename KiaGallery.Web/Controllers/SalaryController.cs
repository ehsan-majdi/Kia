using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Salary;
using KiaGallery.Web.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر حقوق دستمزد
    /// </summary>
    public class SalaryController : BaseController
    {
        /// <summary>
        /// مدیریت حقوق دستمزد
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, salary")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// حقوق پرسنل جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, salary")]
        public ActionResult Add()
        {
            ViewBag.Title = "حقوق جدید";
            return View("Edit");
        }

        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="Id">ردیف حقوق پرسنل</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, salary")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش حقوق پرسنل";
            return View();
        }

        /// <summary>
        /// ذخیره حقوق پرسنل
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات حقوق پرسنل</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, salary")]
        public JsonResult Save(SalaryViewModel model)
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
                        var entity = db.Salary.Single(x => x.Id == model.id);
                        entity.WorkHours = model.workHours;
                        entity.Mission = model.mission;
                        entity.Reward = model.reward;
                        entity.Remuneration = model.remuneration;
                        entity.Others = model.others;
                        entity.LoanInstallment = model.loanInstallment;
                        entity.Imprest = model.imprest;
                        entity.CommodityItem = model.commodityItem;
                        entity.Imprest = model.imprest;
                        entity.Insurance = model.insurance;
                        entity.ModifyUserId = userId;
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;
                        message = "پرسنل با موفقیت ویرایش شد.";
                    }
                    else
                    {
                        var entity = new Salary()
                        {
                            WorkHours = model.workHours,
                            HourlyWageRate = model.hourlyWageRate,
                            OvertimeRate = model.overtimeRate,
                            Mission = model.mission,
                            Reward = model.reward,
                            Remuneration = model.remuneration,
                            Others = model.others,
                            LoanInstallment = model.loanInstallment,
                            Imprest = model.imprest,
                            CommodityItem = model.commodityItem,
                            Insurance = model.insurance,
                            MonthCalculated = model.monthCalculatedDate,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.Salary.Add(entity);
                        message = "پرسنل با موفقیت ایجاد شد.";
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
        /// خواندن اطلاعات پرسنل
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون اطلاعات لود شده پرسنل</returns>
        [HttpGet]
        [Authorize(Roles = "admin, salary")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                SalaryViewModel item;
                using (var db = new KiaGalleryContext())
                {
                    int? nextId = db.Salary.FirstOrDefault(x => x.Id == (id + 1))?.Id;
                    int? prevId = db.Salary.FirstOrDefault(x => x.Id == (id - 1))?.Id;
                    item = db.Salary.Where(x => x.Id == id).Select(x => new SalaryViewModel()
                    {
                        id = x.Id,
                        fullName = x.Person.FirstName + " " + x.Person.LastName,
                        personNumber = x.Person.PersonNumber,
                        branch = x.Person.Branch.Name,
                        workHours = x.WorkHours,
                        hourlyWageRate = x.HourlyWageRate,
                        overtimeRate = x.OvertimeRate,
                        mission = x.Mission,
                        reward = x.Reward,
                        remuneration = x.Remuneration,
                        others = x.Others,
                        loanInstallment = x.LoanInstallment,
                        imprest = x.Imprest,
                        commodityItem = x.CommodityItem,
                        monthCalculatedDate = x.MonthCalculated,
                        insurance = x.Insurance
                    }).FirstOrDefault();
                    item.monthCalculated = DateUtility.GetPersianDate(item.monthCalculatedDate);
                    item.nextId = nextId;
                    item.prevId = prevId;
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = item
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "پرسنل مورد نظر یافت نشد."
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
        /// جستجوی حقوق دستمزد
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست حقوق دستمزد پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, salary")]
        public JsonResult Search(SalaryParamsViewModel model)
        {
            Response response;
            try
            {
                List<SalarySearchViewModel> salaryList;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Salary.Select(x => x);
                    if (!string.IsNullOrEmpty(model.date))
                    {
                        DateTime dateTime = DateUtility.GetDateTime(model.date).GetValueOrDefault();
                        query = query.Where(x => x.MonthCalculated == dateTime);
                    }
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    salaryList = query.Select(x => new SalarySearchViewModel() {
                        id = x.Id,
                        fullName = x.Person.FirstName + " "+ x.Person.LastName,
                        workHours = x.WorkHours,
                        branch = x.Person.Branch.Name,
                        monthCalculatedDate = x.MonthCalculated
                    }).ToList();
                    salaryList.ForEach(x => x.monthCalculated = DateUtility.GetPersianDate(x.monthCalculatedDate));
                }
                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = salaryList,
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
        /// وارد کردن اکسل
        /// </summary>
        /// <param name="model">مدلی حاوی اطلاعات کارکرد پرسنل</param>
        //[Authorize(Roles = "admin, salary")]
        //public ActionResult Import(string date)
        //{
        //    Response response;
        //    try
        //    {
        //        List<string> keyList = new List<string> { Settings.KeySalaryBaseBranch, Settings.KeySalaryBaseOffice, Settings.KeyOverTime, Settings.KeySupervisor, Settings.KeyYearAddedRate };

        //        string serverPath = Server.MapPath("~/Temp/");
        //        HttpPostedFileBase hpf = Request.Files[0];

        //        if (hpf.ContentLength == 0)
        //            throw new Exception("File length can't be equal to zero");

        //        string fileName = Path.GetFileName(hpf.FileName);
        //        string savedFileName = Path.Combine(serverPath, fileName);

        //        if (System.IO.File.Exists(savedFileName))
        //        {
        //            Random random = new Random();
        //            string prefix = random.Next(1000, 9999).ToString() + "-";
        //            fileName = prefix + fileName;
        //        }
        //        savedFileName = Path.Combine(serverPath, fileName);

        //        if (!Directory.Exists(serverPath))
        //        {
        //            Directory.CreateDirectory(serverPath);
        //        }

        //        hpf.SaveAs(savedFileName);

        //        string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=yes'", savedFileName);
        //        DataTable dt = GetDataTable("SELECT * from [خلاصه وضعيت$]", connString);

        //        Dictionary<string, int> categoryCodeList = new Dictionary<string, int>();
        //        Dictionary<string, int> productionLineList = new Dictionary<string, int>();

        //        var user = GetAuthenticatedUser();
        //        DateTime monthCalculatedDate = DateUtility.GetDateTime(date).GetValueOrDefault();
        //        DateTime currentTime = DateTime.Now;

        //        using (var db = new KiaGalleryContext())
        //        {
        //            List<Person> personList = db.Person.ToList();
        //            List<Settings> settingsList = db.Settings.Where(x => keyList.Any(y => y == x.Key)).ToList();
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                var personnumber = dr[1]?.ToString();
        //                if(personnumber != null && personnumber != "")
        //                {
        //                    int personNumber = int.Parse(personnumber);
        //                    DateTime workHours = DateTime.Parse( dr[4]?.ToString());
        //                    DateTime dateTime = DateTime.Parse("12/30/1899 12:00:00 AM");
        //                    TimeSpan WorkHours = workHours.Subtract(dateTime);
        //                    int days = WorkHours.Days * 24;
        //                    int hours = WorkHours.Hours + days;
        //                    int minutes = WorkHours.Minutes;
        //                    var person = personList.FirstOrDefault(x => x.PersonNumber == personNumber && x.Active == true);
        //                    if (hours > 24)
        //                        hours -= 24;
        //                    if (person != null)
        //                    {
        //                        long supervisor;
        //                        long baseRate;
        //                        if (person.PersonType == PersonType.CentralOffice)
        //                            baseRate = Int32.Parse(settingsList.Single(x => x.Key == Settings.KeySalaryBaseOffice).Value);
        //                        else
        //                            baseRate = Int32.Parse(settingsList.Single(x => x.Key == Settings.KeySalaryBaseBranch).Value);
        //                        long supervisorBase = Int32.Parse(settingsList.Single(x => x.Key == Settings.KeySupervisor).Value);
        //                        long yearAddedBase = Int32.Parse(settingsList.Single(x => x.Key == Settings.KeyYearAddedRate).Value);
        //                        long overtimeBase = Int32.Parse(settingsList.Single(x => x.Key == Settings.KeyOverTime).Value);

        //                        supervisor = person.Supervisor == true ? supervisorBase : 0;
        //                        DateTime contractStartDate = person.ContractStartDate;
        //                        int numberMonths = (currentTime.Month + currentTime.Year * 12) - (contractStartDate.Month + contractStartDate.Year * 12);
        //                        long hourlyWageRate = baseRate + ((numberMonths / 12) * yearAddedBase) + supervisor;
        //                        long overtimeRate = hourlyWageRate + overtimeBase;
        //                        string hour = hours.ToString() + ":" + minutes.ToString();
        //                        int len = hour.Length;
        //                        Salary item = new Salary()
        //                        {
        //                            PersonId = person.Id,
        //                            WorkHours = hour,
        //                            HourlyWageRate = hourlyWageRate,
        //                            OvertimeRate = overtimeRate,
        //                            Mission = 0,
        //                            Reward = 0,
        //                            Remuneration = 0,
        //                            Others = 0,
        //                            LoanInstallment = 0,
        //                            Imprest = 0,
        //                            CommodityItem = 0,
        //                            MonthCalculated = monthCalculatedDate,
        //                            Insurance = person.Insurance,
        //                            CreateUserId = user.Id,
        //                            ModifyUserId = user.Id,
        //                            CreateDate = currentTime,
        //                            ModifyDate = currentTime,
        //                            Ip = Request.UserHostAddress
        //                        };
        //                        db.Salary.Add(item);
        //                    }
        //                }
        //            }
        //            db.SaveChanges();
        //        }

        //        response = new Response()
        //        {
        //            status = 200,
        //            data = new
        //            {
        //                name = fileName,
        //                length = hpf.ContentLength,
        //                type = hpf.ContentType
        //            },
        //            message = "بارگذاری اطلاعات با موفقیت انجام شد."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// چاپ حقوق دستمزد
        /// </summary>
        /// <param name="model">تاریخ</param>
        /// <returns>فایل حقوق دستمزد</returns>
        //[HttpGet]
        //[Authorize(Roles = "admin, salary")]
        //public ActionResult Print(string date,int? branchId,int? salaryId)
        //{
        //    List<SalaryViewModel> salaryList;
        //    float baseWorkingHour, workingHourBranch, workingHourOffice;
        //    using (var db = new KiaGalleryContext())
        //    {
        //        workingHourBranch = float.Parse( db.Settings.Single(x => x.Key == Settings.KeyWorkingHourBranch).Value);
        //        workingHourOffice = float.Parse(db.Settings.Single(x => x.Key == Settings.KeyWorkingHourOffice).Value);
        //        var query = db.Salary.Select(x => x);
        //        if (salaryId != null)
        //            query = query.Where(x => x.Id == salaryId);
        //        if (branchId != null)
        //            query = query.Where(x => x.Person.BranchId == branchId);
        //        DateTime dateTime = DateUtility.GetDateTime(date).GetValueOrDefault();
        //        query = query.Where(x => x.MonthCalculated == dateTime);
        //        salaryList = query.Select(x => new SalaryViewModel()
        //        {
        //            id = x.Id,
        //            fullName = x.Person.FirstName + " " + x.Person.LastName,
        //            personNumber = x.Person.PersonNumber,
        //            workHours = x.WorkHours,
        //            hourlyWageRate = x.HourlyWageRate,
        //            overtimeRate = x.OvertimeRate,
        //            mission = x.Mission,
        //            reward = x.Reward,
        //            remuneration = x.Remuneration,
        //            others = x.Others,
        //            loanInstallment = x.LoanInstallment,
        //            imprest = x.Imprest,
        //            commodityItem = x.CommodityItem,
        //            monthCalculatedDate = x.MonthCalculated,
        //            insurance = x.Insurance,
        //            personType = x.Person.PersonType
        //        }).ToList();
        //        salaryList.ForEach(x => x.monthCalculated = DateUtility.GetPersianDate(x.monthCalculatedDate));
        //    }
        //    List<StiReport> reports = new List<StiReport>();
        //    foreach (var item in salaryList)
        //    {
        //        DataSet dataset = new DataSet("DataSource");
        //        DataTable dataTable = new DataTable();
        //        StiReport report = new StiReport();
        //        report.Load(Server.MapPath("~/Report/Salary/Salary.mrt"));

        //        report.Dictionary.Databases.Clear();
        //        report.ScriptLanguage = StiReportLanguageType.CSharp;
        //        long salary, overtime, salaryTotal, deductionsTotal, insurance, netPaidamount;
        //        List<string> WorkHours = item.workHours.Split(':').Select(x => x).ToList();
        //        float workHours, overtimeFunction, monthlyPerformance;
        //        if (WorkHours.Count() > 1)
        //        {
        //            float hours = float.Parse(WorkHours[0]);
        //            float min = float.Parse(WorkHours[1]);
        //            float number = min / 60;
        //            workHours = hours + number;
        //        }
        //        else
        //        {
        //            workHours = float.Parse(WorkHours[0]);
        //        }
        //        if (item.personType == PersonType.CentralOffice)
        //            baseWorkingHour = workingHourOffice;
        //        else
        //            baseWorkingHour = workingHourBranch;
        //        overtimeFunction = workHours > baseWorkingHour ? workHours - baseWorkingHour : 0;// کارکرد اضافه کاری
        //        overtime = (long)Math.Round( overtimeFunction * item.overtimeRate.GetValueOrDefault());
        //        monthlyPerformance = workHours > baseWorkingHour ? baseWorkingHour : workHours;// کارکرد ماهانه
        //        salary = (long)Math.Round(monthlyPerformance * item.hourlyWageRate.GetValueOrDefault());
        //        insurance = item.insurance == true ? 2100000 : 0;// بیمه
        //        salaryTotal = salary + overtime + item.mission.GetValueOrDefault() + item.reward.GetValueOrDefault() + item.remuneration.GetValueOrDefault() + item.others.GetValueOrDefault()+ insurance;
        //        deductionsTotal = item.loanInstallment.GetValueOrDefault() + item.imprest.GetValueOrDefault() + item.commodityItem.GetValueOrDefault()+ insurance;
        //        netPaidamount = salaryTotal - deductionsTotal;
        //        report.Dictionary.Variables["OvertimeFunction"].Value = workHours > baseWorkingHour ? (workHours - baseWorkingHour).ToString() : "0"; ;// کارکرد اضافه کاری
        //        report.Dictionary.Variables["Overtime"].Value = Core.ToSeparator(overtime);// اضافه کاری
        //        report.Dictionary.Variables["MonthlyPerformance"].Value = workHours > baseWorkingHour ? baseWorkingHour.ToString() : item.workHours;//کارکرد ماهانه
        //        report.Dictionary.Variables["Salary"].Value = Core.ToSeparator(salary);
        //        report.Dictionary.Variables["FullName"].Value = item.fullName;
        //        report.Dictionary.Variables["PersonNumber"].Value = item.personNumber.ToString();
        //        report.Dictionary.Variables["HourlyWageRate"].Value = Core.ToSeparator(item.hourlyWageRate.GetValueOrDefault());
        //        report.Dictionary.Variables["OvertimeRate"].Value = Core.ToSeparator(item.overtimeRate.GetValueOrDefault());
        //        report.Dictionary.Variables["Mission"].Value = Core.ToSeparator(item.mission.GetValueOrDefault());
        //        report.Dictionary.Variables["Reward"].Value = Core.ToSeparator(item.reward.GetValueOrDefault());
        //        report.Dictionary.Variables["Remuneration"].Value = Core.ToSeparator(item.remuneration.GetValueOrDefault());
        //        report.Dictionary.Variables["Others"].Value = Core.ToSeparator(item.others.GetValueOrDefault());
        //        report.Dictionary.Variables["LoanInstallment"].Value = Core.ToSeparator(item.loanInstallment.GetValueOrDefault());
        //        report.Dictionary.Variables["Imprest"].Value = Core.ToSeparator(item.imprest.GetValueOrDefault());
        //        report.Dictionary.Variables["CommodityItem"].Value = Core.ToSeparator(item.commodityItem.GetValueOrDefault());
        //        report.Dictionary.Variables["MonthCalculatedDate"].Value = DateUtility.GetPersianDate(item.monthCalculatedDate);
        //        report.Dictionary.Variables["TotalSalary"].Value = Core.ToSeparator(salaryTotal);// جمع حقوق
        //        report.Dictionary.Variables["DeductionsTotal"].Value = Core.ToSeparator(deductionsTotal);// جمع کسورات
        //        report.Dictionary.Variables["NetPaidamount"].Value = Core.ToSeparator(netPaidamount);//مبلغ خالص پرداختی
        //        report.Dictionary.Variables["SalaryInsurance"].Value = Core.ToSeparator(insurance);
        //        report.Dictionary.Variables["DeductionsInsurance"].Value = Core.ToSeparator(insurance);
        //        report.Compile();
        //        report.Render(false);
        //        reports.Add(report);

        //    }
        //    StiReport joinedReport = new StiReport();
        //    joinedReport.NeedsCompiling = false;
        //    joinedReport.IsRendered = true;
        //    joinedReport.RenderedPages.Clear();

        //    foreach (var report in reports)
        //    {
        //        foreach (StiPage page in report.CompiledReport.RenderedPages)
        //        {
        //            page.Report = joinedReport;
        //            page.NewGuid();
        //            joinedReport.RenderedPages.Add(page);
        //        }
        //    }
        //    MemoryStream stream = new MemoryStream();
        //    StiPdfExportSettings settings = new StiPdfExportSettings();
        //    StiPdfExportService service = new StiPdfExportService();
        //    service.ExportPdf(joinedReport, stream, settings);
        //    this.Response.Buffer = true;
        //    this.Response.ClearContent();
        //    this.Response.ClearHeaders();
        //    this.Response.ContentType = "application/pdf";
        //    this.Response.AddHeader("Content-Disposition", "attachment; filename=\"KIA-Report.pdf\"");
        //    this.Response.ContentEncoding = Encoding.UTF8;
        //    this.Response.AddHeader("Content-Length", stream.Length.ToString());
        //    this.Response.BinaryWrite(stream.ToArray());
        //    //this.Response.End();
        //    return new FileStreamResult(stream, "application/pdf");
        //}
        private DataTable GetDataTable(string sql, string connectionString)
        {
            DataTable dt = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    using (OleDbDataReader rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                        return dt;
                    }
                }
            }
        }


    }
}