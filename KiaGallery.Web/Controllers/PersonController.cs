using ClosedXML.Excel;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Salary;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class PersonController : BaseController
    {
        /// <summary>
        /// مدیریت پرسنل
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, person")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// پرسنل غیر فعال
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult DeactivePerson()
        {
            return View();
        }

        /// <summary>
        /// گزارش کلی پرسنل
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonReport()
        {
            using (var db = new KiaGalleryContext())
            {
                //ViewBag.BranchList = db.Branch.Where(x => x.Active == true).ToList();
                ViewBag.BranchList = db.Branch.Where(x => x.BranchType == BranchType.Branch).ToList();
            }
            return View();
        }

        /// <summary>
        /// صفحه فرم پرداخت سنوات
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>صفحه مورد نظر</returns>
        public ActionResult YearSum(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                var person = db.Person.Single(x => x.Id == id);
                ViewBag.Date = DateUtility.GetPersianDate(person.ContractEndDate);
                ViewBag.PersonNumber = person.PersonNumber;
                ViewBag.FullName = person.FirstName + " " + person.LastName;
                ViewBag.Branch = person.Branch.Name;
                ViewBag.StartDate = DateUtility.GetPersianDate(person.ContractStartDate);
                var startDate = person.ContractStartDate;
                var endDate = DateTime.Now;
                var startDay = DateUtility.GetPersianDay(startDate);
                var endDay = DateUtility.GetPersianDay(endDate);
                var date = DateTime.Now - person.ContractStartDate;
                var daysSum = date.Days;
                TimeSpan ts = endDate - startDate;
                int years = ts.Days / 365;
                int months = (ts.Days % 365) / 31;
                ViewBag.DaysSum = daysSum;
                ViewBag.Day = int.Parse(endDay) - int.Parse(startDay);
                ViewBag.Month = months;
                ViewBag.Year = years;
                ViewBag.EndDate = DateUtility.GetPersianDate(endDate);
                var _settings = db.Settings.ToList();
                var payment = _settings.SingleOrDefault(x => x.Key == Settings.KeyBaseSalary)?.Value;
                //var dailyPay = int.Parse(payment) / 365;
                //var yearpay = daysSum * dailyPay;
                ViewBag.ThisYear = _settings.Single(x => x.Key == Settings.KeyContractYear).Value; ;
                ViewBag.Payment = Core.ToSeparator(int.Parse(payment));
                //ViewBag.DailyPay = Core.ToSeparator(dailyPay);
                //ViewBag.YearPay = Core.ToSeparator(yearpay);
            }

            return View();
        }

        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="Id">ردیف پرسنل</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, person")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش پرسنل";
            return View();
        }
        /// <summary>
        /// پرسنل جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, person")]
        public ActionResult Add()
        {
            ViewBag.Title = "پرسنل جدید";
            return View("Edit");
        }
        /// <summary>
        /// مشاهده اطلاعات پرسنل
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, person")]
        public ActionResult View(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "مشاهده اطلاعات پرسنل";
            return View();
        }
        /// <summary>
        /// مشاهده اطلاعات پرسنل
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, person")]
        public JsonResult LoadView(int Id)
        {
            Response response;
            try
            {
                PersonView person;
                using (var db = new KiaGalleryContext())
                {
                    person = db.Person.Where(x => x.Id == Id).Select(item => new PersonView
                    {
                        id = item.Id,
                        firstName = item.FirstName,
                        lastName = item.LastName,
                        branch = item.Branch.Name,
                        personNumber = item.PersonNumber,
                        supervisor = item.Supervisor,
                        accountNumber = item.AccountNumber,
                        contractStart = item.ContractStartDate,
                        contractSubject = item.ContractSubject,
                        jobId = item.JobId,
                        gender = item.Gender,
                        export = item.ExportFrom,
                        married = item.Married,
                        children = item.Children,
                        insurance = item.Insurance,
                        insuranceExpireDateTime = item.InsuranceExpireDate,
                        nationalCode = item.NationalCode,
                        city = item.City,
                        birth = item.BirthDate,
                        phoneNumber = item.PhoneNumber,
                        address = item.Address,
                        fatherName = item.FatherName,
                        active = item.Active,
                        mobileNumber = item.MobileNumber,
                        birthCertificateNumber = item.BirthCertificateNumber,
                        insuranceNumber = item.InsuranceNumber,
                        education = item.Education,
                        major = item.Major,
                        majorCurrent = item.MajorCurrent,
                        educationalStatus = item.EducationalStatus,
                        personType = item.PersonType,
                        fileList = item.PersonFileList.Select(x => new PersonFileViewModel
                        {
                            id = x.Id,
                            personId = x.PersonId,
                            fileName = x.FileName,
                            title = x.Title,
                            category = x.Category,
                            fileType = x.FileType
                        }).ToList()
                    }).FirstOrDefault();
                    person.contractStartDate = DateUtility.GetPersianDate(person.contractStart);
                    person.birthDate = DateUtility.GetPersianDate(person.birth);
                    person.insuranceExpireDate = DateUtility.GetPersianDate(person.insuranceExpireDateTime);
                    person.fileList.ForEach(x =>
                    {
                        x.categoryTitle = Enums.GetTitle(x.category);
                        x.fileTypeTitle = Enums.GetTitle(x.fileType);
                    });
                }
                response = new Response()
                {
                    status = 200,
                    data = person
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// فعال کردن پرسنل
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin, person")]
        public JsonResult ActivePersonal(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var person = db.Person.Single(x => x.Id == id);
                    person.Active = true;
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "حساب کاربری " + person.FirstName + " " + person.LastName + " فعال شد."
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
        ///  جستجوی پرسنل غیر فعال
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست پرسنل پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, person")]
        public JsonResult GetDeactivePerson(PersonParamsViewModel model)
        {
            Response response;
            try
            {
                List<PersonSearchViewModel> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Person.Where(x => x.Active == false);
                    if (!string.IsNullOrEmpty(model.firstName))
                    {
                        query = query.Where(x => x.FirstName.Contains(model.firstName));
                    }
                    if (!string.IsNullOrEmpty(model.lastName))
                    {
                        query = query.Where(x => x.LastName.Contains(model.lastName));
                    }
                    if (model.personNumber != null)
                    {
                        query = query.Where(x => x.PersonNumber == model.personNumber);
                    }
                    if (model.branchId != null)
                    {
                        query = query.Where(x => x.BranchId == model.branchId);
                    }

                    if (model.notInsurance == true)
                    {
                        query = query.Where(x => x.Insurance == false);
                    }

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.PersonNumber).Skip(model.page * model.count).Take(model.count);
                    list = query.Select(item => new PersonSearchViewModel()
                    {
                        id = item.Id,
                        active = item.Active,
                        firstName = item.FirstName,
                        lastName = item.LastName,
                        branchName = item.Branch.Name,
                        personNumber = item.PersonNumber,
                        fileName = item.PersonFileList.FirstOrDefault(x => x.Category == PersonFileCategory.PersonalPhoto).FileName,
                    }).ToList();
                }

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

        /// <summary>
        /// غیرفعال کردن پرسنل
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin, person")]
        public JsonResult DeactivePersonal(DeactivePersonalViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var person = db.Person.Single(x => x.Id == model.id);
                    person.Active = false;
                    person.ContractEndDate = DateUtility.GetDateTime(model.date);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "حساب کاربری " + person.FirstName + " " + person.LastName + " غیر غعال شد."
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
        /// مشاهده فرم تسویه حساب
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,person")]
        public ActionResult Reckoning(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                var entity = db.Person.Single(x => x.Id == id);
                ViewBag.FullName = entity.FirstName + " " + entity.LastName;//نام و نام خانوادگی پرسنل
                ViewBag.FatherName = entity.FatherName;//نام پدر
                ViewBag.BirthDate = DateUtility.GetPersianDate(entity.BirthDate); //تاریخ تولد
                ViewBag.ExportFrom = entity.ExportFrom;//صادره از
                ViewBag.BirthCertificateNumber = entity.BirthCertificateNumber;//شماره شناسنامه
                ViewBag.NationalCode = entity.NationalCode;//کد ملی
                ViewBag.ContractStartDate = DateUtility.GetPersianDate(entity.ContractStartDate); //تاریخ شروع همکاری
                ViewBag.ContractEndDate = DateUtility.GetPersianDate(entity.ContractEndDate); //تاریخ پایان همکاری
                var gender = entity.Gender;
                if (gender == true)
                {
                    ViewBag.Gender = "آقای";
                }
                else
                {
                    ViewBag.Gender = "خانم";
                }

            }

            return View();
        }

        public ActionResult WorkContractForm(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                var entity = db.Person.Single(x => x.Id == id);
                var _settings = db.Settings.ToList();
                var year = _settings.SingleOrDefault(x => x.Key == Settings.KeyContractYear)?.Value;
                ViewBag.Year = year;
                if (entity.Gender)
                {
                    ViewBag.Sex = "آقای";
                }
                else
                {
                    ViewBag.Sex = "خانم";
                }

                ViewBag.WorkHoursValue = entity.Branch.WorkingHour;

                if (entity.BranchId == 1)
                {
                    ViewBag.Place = "دفتر مرکزی";
                }
                else
                {
                    ViewBag.Place = " شعبه " + entity.Branch.Name;
                }
                ViewBag.BranchAddress = entity.Branch.Address;
                if (entity.JobId != null)
                {
                    ViewBag.JobDescriptionTemplate = entity.Job.Description;
                    ViewBag.JobDescriptionTemplateTitle = entity.Job.Title;
                }
                else
                {
                    ViewBag.JobDescriptionTemplate = "";
                    ViewBag.JobDescriptionTemplateTitle = "";
                }
                ViewBag.FirstName = entity.FirstName + " " + entity.LastName;//نام و نام خانوادگی پرسنل
                ViewBag.FatherName = entity.FatherName;//نام پدر
                ViewBag.BirthCertificateNumber = entity.BirthCertificateNumber;//شماره شناسنامه
                ViewBag.NationalCode = entity.NationalCode;//کد ملی
                ViewBag.ContractSubject = entity.ContractSubject;//تاریخ آغاز همکاری
                ViewBag.BirthDate = DateUtility.GetPersianDate(entity.BirthDate); //تاریخ تولد
                ViewBag.Education = Enums.GetTitle(entity.Education);//مدرک تحصیلی
                ViewBag.Major = entity.Major;//رشته تحصیلی
                ViewBag.Married = entity.Married.ToString();//وضعیت تاهل
                ViewBag.post = "مدیریت سفارشات";
                ViewBag.Address = entity.Address;//آدرس پرسنل
                ViewBag.ExportFrom = entity.ExportFrom;//صادره از
                ViewBag.City = entity.City;//شهر
                ViewBag.NowTime = DateUtility.GetPersianDate(DateTime.Now);//تاریخ جاری
                DateTime contractDate = entity.ContractStartDate;//تاریخ شروع همکاری
                DateTime firstDayYear = DateUtility.GetDateTime(year + "/01/01").GetValueOrDefault();//تاریخ درخواست
                DateTime lastDayYear = DateUtility.GetDateTime(year + "/12/29").GetValueOrDefault();//تاریخ درخواست
                PersianCalendar Shamsi = new PersianCalendar();
                var shMonth = Shamsi.GetMonth(contractDate);
                PersianCalendar calendar = new PersianCalendar();
                var contractStartDate = entity.ContractStartDate;
                ContractSubject contractSubject = entity.ContractSubject;
                int shYear = calendar.GetYear(contractStartDate);
                var contractYearsDivision = (int.Parse(year)) - shYear;
                var contractMounthsDivision = contractYearsDivision * 12;
                var contractWageYearsSum = contractMounthsDivision * 83 * 10;
                string fromDate = "";
                string month = "";
                string toDate = "";
                if (contractDate > firstDayYear)
                {
                    fromDate = DateUtility.GetPersianDate(contractDate);
                    month = (12 - shMonth).ToString();
                }
                else
                {
                    fromDate = year + "/01/01";
                    month = "12";
                }
                toDate = year + "/12/29";
                var startMonth = Shamsi.GetMonth(DateUtility.GetDateTime(fromDate).Value);
                var endMonth = Shamsi.GetMonth(DateUtility.GetDateTime(toDate).Value);
                var contractTime = endMonth - startMonth;
                switch (contractTime)
                {
                    case 1:
                        ViewBag.contractTime = "یک ماه";
                        break;
                    case 2:
                        ViewBag.contractTime = "دو ماه";
                        break;
                    case 3:
                        ViewBag.contractTime = "سه ماه";
                        break;
                    case 4:
                        ViewBag.contractTime = "چهار ماه";
                        break;
                    case 5:
                        ViewBag.contractTime = "پنج ماه";
                        break;
                    case 6:
                        ViewBag.contractTime = "شش ماه";
                        break;
                    case 7:
                        ViewBag.contractTime = "هفت ماه";
                        break;
                    case 8:
                        ViewBag.contractTime = "هشت ماه";
                        break;
                    case 9:
                        ViewBag.contractTime = "نه ماه";
                        break;
                    case 10:
                        ViewBag.contractTime = "ده ماه";
                        break;
                    case 11:
                        ViewBag.contractTime = "یک سال";
                        break;
                    default:
                        ViewBag.contractTime = "";
                        break;

                }
                //ViewBag.contractTime = contractTime;
                ViewBag.fromDate = fromDate;//چاپ بخش از تاریخ
                ViewBag.toDate = toDate;//چاپ بخش لغایت تا تاریخ
                DateTime contractSartDate = entity.ContractStartDate.Date;
                ViewBag.ContractStartDate = DateUtility.GetPersianDate(contractSartDate);//تاریخ آغاز همکاری
                ViewBag.contractDate = contractDate;
                ViewBag.firstDayYear = firstDayYear;
                ViewBag.lastDayYear = toDate;
                ViewBag.branchOwnerName = entity.Branch.OwnerName;
                ViewBag.branchOwnerNationalCode = entity.Branch.OwnerNationalityCode;
                ViewBag.branchOwnerFatherName = entity.Branch.OwnerFatherName;
                ViewBag.branchOwnerNationalityNo = entity.Branch.OwnerNationalityNo;
                ViewBag.contractSubject = contractSubject;
                ViewBag.promissoryNoteNumber = entity.PromissoryNoteNumber;

            }

            return View();
        }

        /// <summary>
        /// فرم قرارداد
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Authorize(Roles = "admin,person")]
        //public ActionResult WorkContractForm(int id)
        //{
        //    using (var db = new KiaGalleryContext())
        //    {
        //        var entity = db.Person.Single(x => x.Id == id);
        //        var _settings = db.Settings.ToList();
        //        var year = _settings.SingleOrDefault(x => x.Key == Settings.KeyContractYear)?.Value;//سال
        //        var supervisor = _settings.SingleOrDefault(x => x.Key == Settings.KeySupervisor)?.Value;//سرپرست
        //        var branch = _settings.SingleOrDefault(x => x.Key == Settings.KeyBranch)?.Value;//شعبه
        //        var overtime = _settings.SingleOrDefault(x => x.Key == Settings.KeyOverTime)?.Value;//اضافه کاری
        //        var salaryBaseOffice = _settings.SingleOrDefault(x => x.Key == Settings.KeySalaryBaseOffice)?.Value;//پایه حقوق شعب
        //        var salaryBaseBranch = _settings.SingleOrDefault(x => x.Key == Settings.KeySalaryBaseBranch)?.Value;//پایه حقوق دفترمرکزی
        //        var yearAddedRate = _settings.SingleOrDefault(x => x.Key == Settings.KeyYearAddedRate)?.Value;//اضافه در سال
        //        ViewBag.FirstName = entity.FirstName + " " + entity.LastName;//نام و نام خانوادگی پرسنل
        //        ViewBag.FatherName = entity.FatherName;//نام پدر
        //        ViewBag.BirthCertificateNumber = entity.BirthCertificateNumber;//شماره شناسنامه
        //        ViewBag.NationalCode = entity.NationalCode;//کد ملی
        //        ViewBag.ContractSubject = entity.ContractSubject;//تاریخ آغاز همکاری
        //        ViewBag.BirthDate = DateUtility.GetPersianDate(entity.BirthDate); //تاریخ تولد
        //        ViewBag.Education = Enums.GetTitle(entity.Education);//مدرک تحصیلی
        //        ViewBag.Major = entity.Major;//رشته تحصیلی
        //        ViewBag.Married = entity.Married.ToString();//وضعیت تاهل
        //        ViewBag.Address = entity.Address;//آدرس پرسنل
        //        ViewBag.ExportFrom = entity.ExportFrom;//صادره از
        //        ViewBag.City = entity.City;//شهر
        //        ViewBag.NowTime = DateUtility.GetPersianDate(DateTime.Now);//تاریخ جاری
        //        if (entity.JobId != null)
        //        {
        //            var job = entity.JobId;
        //            ViewBag.Job = db.PersonJobDescriptionTemplate.Where(x => x.Id == job).Select(x => x.Description).SingleOrDefault();//وظیفه یا شغل پرسنل
        //        }
        //        else
        //        {
        //            ViewBag.Job = "";
        //        }
        //        //وضعیت تاهل
        //        var married = entity.Married;
        //        if (married == true)
        //        {
        //            ViewBag.Married = "متاهل";
        //        }
        //        else
        //        {
        //            ViewBag.Married = "مجرد";
        //        }
        //        bool contractSubject = entity.ContractSubject;
        //        string fromDate = "";
        //        string month = "";
        //        string toDate = "";
        //        DateTime contractDate = entity.ContractStartDate;//تاریخ شروع همکاری
        //        DateTime firstDayYear = DateUtility.GetDateTime(year + "/01/01").GetValueOrDefault();//تاریخ درخواست
        //        PersianCalendar Shamsi = new PersianCalendar();
        //        var shMonth = Shamsi.GetMonth(contractDate);
        //        ViewBag.contractSubject = contractSubject;//تعیین نوع قرار داد
        //        PersianCalendar calendar = new PersianCalendar();
        //        var contractStartDate = entity.ContractStartDate;
        //        int shYear = calendar.GetYear(contractStartDate);
        //        var contractYearsDivision = (int.Parse(year)) - shYear;
        //        var contractMounthsDivision = contractYearsDivision * 12;
        //        var contractWageYearsSum = contractMounthsDivision * 83 * 10;
        //        if (!contractSubject)
        //        {
        //            if (contractDate > firstDayYear)
        //            {
        //                fromDate = DateUtility.GetPersianDate(contractDate);
        //                month = (12 - shMonth).ToString();
        //            }
        //            else
        //            {
        //                fromDate = year + "/01/01";
        //                month = "12";
        //            }
        //            toDate = year + "/12/29";

        //        }
        //        else
        //        {
        //            fromDate = ".................";
        //            month = ".......";
        //            toDate = ".../.../...";
        //        }

        //        ViewBag.monthDate = month;//تعداد ماه باقی مانده از آغاز سال تا پایان قرارداد
        //        ViewBag.fromDate = fromDate;//چاپ بخش از تاریخ
        //        ViewBag.toDate = toDate;//چاپ بخش لغایت تا تاریخ
        //        long wageHourlyRate = 0;///دستمزد روزانه براساس نرخ ثابت ساعتی
        //        long overtimeRate = 0;///اضافه کاری براساس نرخ ثابت ساعتی
        //        string wageHourlyRateToSeperator = Core.ToSeparator(wageHourlyRate);///جداکننده ارقام
        //        string overTimeRateToSeperator = "";///جداکننده ارقام
        //        if (entity.PersonType == PersonType.Branch)//شرط برای تایید بودن اینکه پرسنل متعلق به شعبه است یا خیر...
        //        {
        //            wageHourlyRate = long.Parse(salaryBaseBranch);
        //            wageHourlyRateToSeperator = Core.ToSeparator(wageHourlyRate);
        //        }
        //        else if (entity.PersonType == PersonType.CentralOffice)//شرط برای تایید بودن اینکه پرسنل متعلق به دفترمرکزی است یا خیر...
        //        {
        //            wageHourlyRate = long.Parse(salaryBaseOffice);
        //            wageHourlyRateToSeperator = Core.ToSeparator(wageHourlyRate);
        //        }
        //        if (entity.Supervisor)
        //        {
        //            overtimeRate = contractWageYearsSum + long.Parse(overtime) + wageHourlyRate + 40000;
        //        }
        //        else
        //            overtimeRate = contractWageYearsSum + long.Parse(overtime) + wageHourlyRate + 10000;
        //        long overtimeRatemonthly = long.Parse(overtime);

        //        string overtimeRatemonthlystring = Core.ToSeparator(overtimeRatemonthly);
        //        overTimeRateToSeperator = Core.ToSeparator(overtimeRate);

        //        if (contractStartDate <= firstDayYear)//تاریخ شروع همکاری اگر بیشتر از یکسال باشد طبق عدد خواسته شده
        //        {
        //            //int diffYear = (int.Parse(year) - shYear) * int.Parse(yearAddedRate);
        //            //wageHourlyRate = diffYear + wageHourlyRate;
        //            wageHourlyRateToSeperator = Core.ToSeparator(wageHourlyRate);
        //            if (entity.Supervisor)//در صورتی که پرسنل وارد شده سرپرست بود مشخصات مربوط به سرپرست بودن چاپ می گردد
        //            {
        //                long wageHourlyRateSupervisor = long.Parse(salaryBaseBranch);
        //                wageHourlyRate = wageHourlyRateSupervisor + (int.Parse(supervisor)) + contractWageYearsSum;
        //                wageHourlyRateToSeperator = Core.ToSeparator(wageHourlyRate);
        //            }
        //            else
        //            {
        //                long wageHourlyRateSupervisor = long.Parse(salaryBaseBranch);
        //                wageHourlyRate = wageHourlyRateSupervisor + contractWageYearsSum;
        //                wageHourlyRateToSeperator = Core.ToSeparator(wageHourlyRate);
        //            };
        //        }
        //        else
        //        {
        //            wageHourlyRate = long.Parse(salaryBaseBranch);
        //            var BaseAndYearsWageSum = wageHourlyRate + contractWageYearsSum;
        //            wageHourlyRateToSeperator = Core.ToSeparator(BaseAndYearsWageSum);
        //            if (entity.Supervisor)
        //            {
        //                wageHourlyRate = wageHourlyRate + int.Parse(supervisor);
        //                wageHourlyRateToSeperator = Core.ToSeparator(wageHourlyRate);
        //            }
        //        }
        //        ViewBag.HourlyWageRate = year + " ، " + wageHourlyRateToSeperator;//پایه حقوق به صورت سه رقم،سه رقم قابل رویت است
        //        ViewBag.OvertimeRate = year + " ، " + overTimeRateToSeperator;//اضافه کاری به صورت سه رقم،سه رقم قابل رویت است            
        //        //حق سرپرستی براساس نرخ ثابت ساعتی
        //        if (entity.Supervisor == true)
        //        {
        //            ViewBag.SupervisorText = "حق سرپرستی (براساس نرخ ثابت ساعتی) در سال " + year + " ، " + Core.ToSeparator(long.Parse(supervisor)) + "  ریال ";
        //        }
        //        else
        //        {
        //            ViewBag.SupervisorText = "";
        //        }
        //        //تاریخ شروع همکاری
        //        DateTime contractSartDate = entity.ContractStartDate.Date;
        //        ViewBag.ContractStartDate = DateUtility.GetPersianDate(contractSartDate);//تاریخ آغاز همکاری
        //        ViewBag.OvertimeYearPriceRate = overtimeRatemonthlystring;//اضافه کاری بر حسب سال
        //        ViewBag.contractDate = contractDate;
        //        ViewBag.firstDayYear = firstDayYear;
        //        if (entity.PersonType == PersonType.Branch)
        //        {
        //            ViewBag.SalaryBase = year + " ، " + Core.ToSeparator(long.Parse(salaryBaseBranch));
        //        }
        //        else
        //            ViewBag.SalaryBase = year + " ، " + Core.ToSeparator(long.Parse(salaryBaseOffice));

        //        if (entity.PersonType == PersonType.Branch)
        //            ViewBag.OverTime = year + " ، " + Core.ToSeparator(long.Parse(salaryBaseBranch) + long.Parse(overtime));
        //        else
        //            ViewBag.OverTime = year + " ، " + Core.ToSeparator(long.Parse(salaryBaseOffice) + long.Parse(overtime));
        //        ViewBag.Gender = entity.Gender;
        //        if (entity.PersonType == PersonType.Branch)
        //        {
        //           var mission =  long.Parse(salaryBaseBranch) + long.Parse(salaryBaseBranch) * 0.4;
        //            ViewBag.Mission = year + " ، " + Core.ToSeparator(mission);
        //        }
        //        else
        //        {
        //            var mission = long.Parse(salaryBaseOffice) + long.Parse(salaryBaseOffice) * 0.4;
        //            ViewBag.Mission = year + " ، " + Core.ToSeparator(mission);
        //        }

        //        return View();
        //    }
        //}

        /// <summary>
        /// فایل پرسنل
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, person")]
        public ActionResult File(int Id)
        {
            ViewBag.PersonId = Id;
            ViewBag.Title = "فایل پرسنل";
            return View();
        }

        /// <summary>
        /// ذخیره پرسنل
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات پرسنل</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, person")]
        public JsonResult Save(PersonViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    DateTime contractStartDate = DateUtility.GetDateTime(model.contractStartDate).GetValueOrDefault();
                    int userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.Person.Single(x => x.Id == model.id);
                        entity.FirstName = model.firstName;
                        entity.LastName = model.lastName;
                        entity.ShortName = model.shortName;
                        entity.NikName = model.nikName;
                        entity.PersonNumber = model.personNumber;
                        entity.BranchId = model.branchId;
                        entity.Supervisor = model.supervisor;
                        entity.AccountNumber = model.accountNumber;
                        entity.ContractStartDate = contractStartDate;
                        entity.Insurance = model.insurance;
                        entity.InsuranceExpireDate = DateUtility.GetDateTime(model.insuranceExpireDate);
                        entity.InsuranceBeginDate = DateUtility.GetDateTime(model.insuranceBeginDate);
                        entity.NationalCode = model.nationalCode;
                        entity.BirthDate = DateUtility.GetDateTime(model.birthDate);
                        entity.PhoneNumber = model.phoneNumber;
                        entity.Address = model.address;
                        entity.FatherName = model.fatherName;
                        entity.Active = model.active;
                        entity.MobileNumber = model.mobileNumber;
                        entity.NecessaryNumber = model.necessaryNumber;
                        entity.Married = model.married;
                        entity.Children = model.children;
                        entity.JobId = model.jobId;
                        entity.Gender = model.gender;
                        entity.ContractSubject = model.contractSubject;
                        entity.PromissoryNoteNumber = model.promissoryNoteNumber;
                        entity.ExportFrom = model.exportFrom;
                        entity.City = model.city;
                        entity.ContractInMonth = model.contractInMonth;
                        entity.Contract = model.contract;
                        entity.BirthCertificateNumber = model.birthCertificateNumber;
                        entity.InsuranceNumber = model.insuranceNumber;
                        entity.Education = model.education;
                        entity.Major = model.major;
                        entity.MajorCurrent = model.majorCurrent;
                        entity.EducationalStatus = model.educationalStatus;
                        entity.Reward = model.reward;
                        entity.SupervisorSalary = model.supervisorSalary;
                        entity.ActivityAmount = model.activityAmount;
                        entity.PersonType = model.personType;
                        entity.ModifyUserId = userId;
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;
                        message = "پرسنل با موفقیت ویرایش شد.";
                    }
                    else
                    {
                        var entity = new Person()
                        {
                            FirstName = model.firstName,
                            LastName = model.lastName,
                            ShortName = model.shortName,
                            NikName = model.nikName,
                            PersonNumber = model.personNumber,
                            BranchId = model.branchId,
                            Supervisor = model.supervisor,
                            AccountNumber = model.accountNumber,
                            ContractStartDate = contractStartDate,
                            Insurance = model.insurance,
                            InsuranceExpireDate = DateUtility.GetDateTime(model.insuranceExpireDate),
                            InsuranceBeginDate = DateUtility.GetDateTime(model.insuranceBeginDate),
                            NationalCode = model.nationalCode,
                            BirthDate = DateUtility.GetDateTime(model.birthDate),
                            PhoneNumber = model.phoneNumber,
                            NecessaryNumber = model.necessaryNumber,
                            Address = model.address,
                            FatherName = model.fatherName,
                            Active = model.active,
                            MobileNumber = model.mobileNumber,
                            Married = model.married,
                            Children = model.children,
                            JobId = model.jobId,
                            Gender = model.gender,
                            ContractSubject = model.contractSubject,
                            PromissoryNoteNumber = model.promissoryNoteNumber,
                            ExportFrom = model.exportFrom,
                            City = model.city,
                            ContractInMonth = model.contractInMonth,
                            Contract = model.contract,
                            BirthCertificateNumber = model.birthCertificateNumber,
                            InsuranceNumber = model.insuranceNumber,
                            Education = model.education,
                            Major = model.major,
                            MajorCurrent = model.majorCurrent,
                            EducationalStatus = model.educationalStatus,
                            Reward = model.reward,
                            SupervisorSalary = model.supervisorSalary,
                            ActivityAmount = model.activityAmount,
                            PersonType = model.personType,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.Person.Add(entity);
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
        /// ذخیره پرسنل
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات پرسنل</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, person")]
        public JsonResult SaveSetting(PersonViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    DateTime contractStartDate = DateUtility.GetDateTime(model.contractStartDate).GetValueOrDefault();
                    int userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.Person.Single(x => x.Id == model.id);
                        entity.Reward = model.reward;
                        entity.SupervisorSalary = model.supervisorSalary;
                        entity.ActivityAmount = model.activityAmount;
                        message = "پرسنل با موفقیت ویرایش شد.";
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
        [Authorize(Roles = "admin, person")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Person item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Person.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new PersonViewModel
                        {
                            id = item.Id,
                            firstName = item.FirstName,
                            lastName = item.LastName,
                            shortName = item.ShortName,
                            nikName = item.NikName,
                            branchId = item.BranchId,
                            personNumber = item.PersonNumber,
                            supervisor = item.Supervisor,
                            accountNumber = item.AccountNumber,
                            contractStartDate = DateUtility.GetPersianDate(item.ContractStartDate),
                            insurance = item.Insurance,
                            nationalCode = item.NationalCode,
                            insuranceExpireDate = DateUtility.GetPersianDate(item.InsuranceExpireDate),
                            insuranceBeginDate = DateUtility.GetPersianDate(item.InsuranceBeginDate),
                            birthDate = DateUtility.GetPersianDate(item.BirthDate),
                            phoneNumber = item.PhoneNumber,
                            address = item.Address,
                            fatherName = item.FatherName,
                            active = item.Active,
                            mobileNumber = item.MobileNumber,
                            necessaryNumber = item.NecessaryNumber,
                            married = item.Married,
                            children = item.Children,
                            jobId = item.JobId,
                            gender = item.Gender,
                            contractSubject = item.ContractSubject,
                            promissoryNoteNumber = item.PromissoryNoteNumber,
                            exportFrom = item.ExportFrom,
                            city = item.City,
                            contractInMonth = item.ContractInMonth,
                            contract = item.Contract,
                            birthCertificateNumber = item.BirthCertificateNumber,
                            insuranceNumber = item.InsuranceNumber,
                            education = item.Education,
                            reward = item.Reward,
                            activityAmount = item.ActivityAmount,
                            supervisorSalary = item.SupervisorSalary,
                            major = item.Major,
                            majorCurrent = item.MajorCurrent,
                            educationalStatus = item.EducationalStatus,
                            personType = item.PersonType
                        }
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
        /// جستجوی پرسنل
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست پرسنل پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, person")]
        public JsonResult Search(PersonParamsViewModel model)
        {
            Response response;
            try
            {
                List<PersonSearchViewModel> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Person.Where(x => x.Active == true);
                    if (!string.IsNullOrEmpty(model.firstName))
                    {
                        query = query.Where(x => x.FirstName.Contains(model.firstName));
                    }
                    if (!string.IsNullOrEmpty(model.lastName))
                    {
                        query = query.Where(x => x.LastName.Contains(model.lastName));
                    }
                    if (model.personNumber != null)
                    {
                        query = query.Where(x => x.PersonNumber == model.personNumber);
                    }
                    if (model.branchId != null)
                    {
                        query = query.Where(x => x.BranchId == model.branchId);
                    }

                    if (model.notInsurance == true)
                    {
                        query = query.Where(x => x.Insurance == false);
                    }

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.PersonNumber).Skip(model.page * model.count).Take(model.count);
                    list = query.Select(item => new PersonSearchViewModel()
                    {
                        id = item.Id,
                        active = item.Active,
                        firstName = item.FirstName,
                        lastName = item.LastName,
                        branchName = item.Branch.Name,
                        personNumber = item.PersonNumber,
                        fileName = item.PersonFileList.FirstOrDefault(x => x.Category == PersonFileCategory.PersonalPhoto).FileName,
                    }).ToList();
                }

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


        /// <summary>
        /// گزفتن پرسنل مورد نظر
        /// </summary>
        /// <param name="id">ردیف شخص مورد نظر</param>
        /// <returns>شخص مورد نظر</returns>
        [HttpPost]
        [Authorize(Roles = "admin, person")]
        public JsonResult GetPerson(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Person.Where(x => x.Id == id).Select(x => new
                    {
                        supervisor = x.Supervisor,
                        reward = x.Reward,
                        activityAmount = x.ActivityAmount,
                        supervisorSalary = x.SupervisorSalary

                    }).Single();

                    response = new Response()
                    {
                        status = 200,
                        data = item
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
        /// حذف پرسنل
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, person")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Person.Find(Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "پرسنل با موفقیت حذف شد."
                    };
                    db.Person.Remove(item);
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
        /// جستجوی پرسنل
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست پرسنل پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, person")]
        public JsonResult SearchFile(int personId)
        {
            Response response;
            try
            {
                List<PersonFileViewModel> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.PersonFile.Where(x => x.PersonId == personId);
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id);
                    list = query.Select(item => new PersonFileViewModel()
                    {
                        id = item.Id,
                        title = item.Title,
                        fileName = item.FileName,
                        personId = item.PersonId,
                        fileType = item.FileType,
                        category = item.Category
                    }).ToList();

                    list.ForEach(x =>
                    {
                        x.type = Enums.GetTitle(x.fileType);
                        x.categoryTitle = Enums.GetTitle(x.category);
                    });
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
        /// ذخیره پرسنل
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات پرسنل</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, person")]
        public JsonResult SaveFile(PersonFileViewModel model)
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
                        var entity = db.PersonFile.Single(x => x.Id == model.id);
                        entity.Category = model.category;
                        entity.Title = model.title;
                        entity.FileName = model.fileName;
                        entity.FileType = model.fileType;
                        message = "فایل پرسنل با موفقیت ویرایش شد.";
                    }
                    else
                    {
                        var entity = new PersonFile()
                        {
                            Title = model.title,
                            FileName = model.fileName,
                            FileType = model.fileType,
                            PersonId = model.personId,
                            Category = model.category
                        };
                        db.PersonFile.Add(entity);
                        message = "فایل پرسنل با موفقیت ایجاد شد.";
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
        [Authorize(Roles = "admin, person")]
        public JsonResult LoadFile(int id)
        {
            Response response;
            try
            {
                PersonFile item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.PersonFile.FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new PersonFileViewModel
                        {
                            id = item.Id,
                            title = item.Title,
                            personId = item.PersonId,
                            fileName = item.FileName,
                            fileType = item.FileType
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "فایل پرسنل مورد نظر یافت نشد."
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
        /// حذف فایل پرسنل
        /// </summary>
        /// <param name="id">ردیف فایل پرسنل</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, person")]
        public JsonResult DeleteFile(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.PersonFile.Find(Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "فایل پرسنل با موفقیت حذف شد."
                    };
                    db.PersonFile.Remove(item);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Test()
        //{
        //    return View();
        //}

        //public ActionResult Import(HttpPostedFileBase file)
        //{
        //    List<ImportViewModel> data = new List<ImportViewModel>();
        //    string serverPath = Server.MapPath("~/Temp/");
        //    HttpPostedFileBase hpf = Request.Files[0];

        //    if (hpf.ContentLength == 0)
        //        throw new Exception("File length can't be equal to zero");

        //    string fileName = Path.GetFileName(hpf.FileName);
        //    string savedFileName = Path.Combine(serverPath, fileName);

        //    if (System.IO.File.Exists(savedFileName))
        //    {
        //        Random random = new Random();
        //        string prefix = random.Next(1000, 9999).ToString() + "-";
        //        fileName = prefix + fileName;
        //    }
        //    savedFileName = Path.Combine(serverPath, fileName);

        //    if (!Directory.Exists(serverPath))
        //    {
        //        Directory.CreateDirectory(serverPath);
        //    }

        //    hpf.SaveAs(savedFileName);
        //    using (var db = new KiaGalleryContext())
        //    {
        //        using (XLWorkbook wb = new XLWorkbook(savedFileName))
        //        {
        //            var ws = wb.Worksheets.First();
        //            var range = ws.RangeUsed();

        //            for (int i = 1; i < range.RowCount() + 1; i++)
        //            {

        //                var item = new ImportViewModel()
        //                {
        //                    PersonNumber = ws.Cell(i, 1).Value.ToString(),
        //                    ShiftActivity = ws.Cell(i, 3).Value.ToString(),

        //                };
        //                //db.Test.Add(data);
        //                data.Add(item);
        //            }
        //            data.RemoveAt(0);
        //            foreach (var y in data)
        //            {
        //                var num = int.Parse(y.PersonNumber);
        //                var person = db.Person.Where(x => x.PersonNumber == num).Single();
        //                var intShiftActivity = decimal.Parse(y.ShiftActivity);
        //                person.ShiftActivity = intShiftActivity;
        //            }
        //        }
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("Test");
        //}


    }
}
