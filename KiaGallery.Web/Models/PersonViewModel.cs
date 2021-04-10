using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Models
{
    public class PersonViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// شماره پرسنلی
        /// </summary>
        public int personNumber { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? branchId { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// نام کوتاه
        /// </summary>
        public string shortName { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
        public bool gender { get; set; }
        /// <summary>
        /// سرپرست
        /// </summary>
        public bool supervisor { get; set; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public string accountNumber { get; set; }
        /// <summary>
        /// تاریخ شروع همکاری
        /// </summary>
        public string contractStartDate { get; set; }
        /// <summary>
        /// تاریخ پایان همکاری
        /// </summary>
        public string contractEndDate { get; set; }
        /// <summary>
        /// قرارداد
        /// </summary>
        public bool contract { get; set; }
        /// <summary>
        /// مدت قرارداد به ماه
        /// </summary>
        public int contractInMonth { get; set; }
        /// <summary>
        /// صادره از
        /// </summary>
        public string exportFrom { get; set; }
        /// <summary>
        /// محل تولد
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// موضوع قرارداد
        /// </summary>
        public ContractSubject contractSubject { get; set; }
        /// <summary>
        /// شماره سفته
        /// </summary>
        public string promissoryNoteNumber { get; set; }
        /// <summary>
        /// وضعیت تاهل
        /// </summary>
        public bool married { get; set; }
        /// <summary>
        /// تعداد فرزند
        /// </summary>
        public int? children { get; set; }
        /// <summary>
        /// بیمه
        /// </summary>
        public bool insurance { get; set; }
        /// <summary>
        /// تاریخ انقضا بیمه پرسنل
        /// </summary>
        public string insuranceExpireDate { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        public string nationalCode { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public string birthDate { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// نام پدر
        /// </summary>
        public string fatherName { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string birthCertificateNumber { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string mobileNumber { get; set; }
        /// <summary>
        /// شماره تلفن ضروری
        /// </summary>
        public string necessaryNumber { get; set; }
        /// <summary>
        /// شماره بیمه
        /// </summary>
        public string insuranceNumber { get; set; }
        /// <summary>
        /// مدرک تحصیلی
        /// </summary>
        public Education education { get; set; }
        /// <summary>
        /// رشته تحصیلی
        /// </summary>
        public string major { get; set; }
        /// <summary>
        /// رشته تحصیلی
        /// </summary>
        public string majorCurrent { get; set; }
        /// <summary>
        /// وضعیت تحصیلی
        /// </summary>
        public bool educationalStatus { get; set; }
        /// <summary>
        /// حرفه یا شغل
        /// </summary>
        public int? jobId { get; set; }
        /// <summary>
        /// نوع کاربری
        /// </summary>
        public PersonType personType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? personPostId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nikName { get; set; }
        public long? reward { get; set; }
        public long? activityAmount { get; set; }
        public long? supervisorSalary { get; set; }
        public string insuranceBeginDate { get; set; }
        //public DateTime? insuranceBeginDate { get; set; }

        public List<PersonFileViewModel> fileList { get; set; }
    }

    public class PersonView
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// شماره پرسنلی
        /// </summary>
        public int personNumber { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public string branch { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
        public bool gender { get; set; }
        /// <summary>
        /// سرپرست
        /// </summary>
        public bool supervisor { get; set; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public string accountNumber { get; set; }
        /// <summary>
        /// تاریخ شروع همکاری
        /// </summary>
        public string contractStartDate { get; set; }
        /// <summary>
        /// تاریخ شروع همکاری
        /// </summary>
        public DateTime contractStart { get; set; }
        /// <summary>
        /// بیمه
        /// </summary>
        public bool insurance { get; set; }
        /// <summary>
        /// تاریخ انقضا بیمه پرسنل
        /// </summary>
        public DateTime? insuranceExpireDateTime { get; set; }
        /// <summary>
        /// تاریخ انقضا بیمه پرسنل
        /// </summary>
        public string insuranceExpireDate { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        public string nationalCode { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public string birthDate { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime? birth { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// نام پدر
        /// </summary>
        public string fatherName { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string birthCertificateNumber { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string mobileNumber { get; set; }
        /// <summary>
        /// شماره بیمه
        /// </summary>
        public string insuranceNumber { get; set; }
        /// <summary>
        /// مدرک تحصیلی
        /// </summary>
        public Education education { get; set; }
        /// <summary>
        /// رشته تحصیلی
        /// </summary>
        public string major { get; set; }
        /// <summary>
        /// رشته تحصیلی
        /// </summary>
        public string majorCurrent { get; set; }
        /// <summary>
        /// وضعیت تحصیلی
        /// </summary>
        public bool educationalStatus { get; set; }
        /// <summary>
        /// نوع کاربری
        /// </summary>
        public PersonType personType { get; set; }
        /// <summary>
        /// صادره از
        /// </summary>
        public string export { get; set; }
        /// <summary>
        /// محل تولد
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// شغل
        /// </summary>
        public int? jobId { get; set; }
        /// <summary>
        /// موضوع قرارداد
        /// </summary>
        public ContractSubject contractSubject { get; set; }
        /// <summary>
        /// وضعیت تاهل
        /// </summary>
        public bool married { get; set; }
        /// <summary>
        /// تعداد فرزند
        /// </summary>
        public int? children { get; set; }
        /// <summary>
        /// لیست فایل
        /// </summary>
        public List<PersonFileViewModel> fileList { get; set; }
    }

    public class PersonSearchViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// شماره پرسنلی
        /// </summary>
        public int personNumber { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// پرسنل فاقد بیمه
        /// </summary>
        public bool notInsurance { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// عنوان فایل
        /// </summary>
        public string fileName { get; set; }
    }
    public class PersonParamsViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string firstName { get; set; }
        public bool notInsurance { get; set; }
        public string lastName { get; set; }
        public int? personNumber { get; set; }
        public int? branchId { get; set; }
    }

    public class PersonFileViewModel
    {
        public int? id { get; set; }
        /// <summary>
        /// ردیف
        /// </summary>
        public int personId { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// دسته بندی
        /// </summary>
        public PersonFileCategory category { get; set; }
        /// <summary>
        /// نوع فایل
        /// </summary>
        public PersonFileType fileType { get; set; }
        /// <summary>
        /// دسته بندی
        /// </summary>
        public string categoryTitle { get; set; }
        /// <summary>
        /// نوع فایل
        /// </summary>
        public string fileTypeTitle { get; set; }
        /// <summary>
        /// نوع
        /// </summary>
        public string type { get; set; }
    }

    public class PersonFileParamsViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
        public int personId { get; set; }
    }
    /// <summary>
    /// بخش ذخیره تنظیمات پرسنل جدید
    /// </summary>
    public class PersonItemSettings
    {
        /// <summary>
        /// سال کارمند
        /// </summary>
        public string contractYear { get; set; }

    }
    /// <summary>
    /// لیست افراد فاقد بیمه
    /// </summary>
    public class InsuranceListViewModel
    {
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string phoneNumber { get; set; }
    }

    /// <summary>
    /// جدول کمکی برای پرسنل غیر فعال
    /// </summary>
    public class DeactivePersonalViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// تاریخ 
        /// </summary>
        public string date { get; set; }
    }
}