using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KiaGallery.Model.Context.Salary
{
    /// <summary>
    /// پرسنل
    /// </summary>
    public class Person
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public Person()
        {
            SalaryList = new List<Salary>();
            PersonFileList = new List<PersonFile>();
            InternalOrderList = new List<InternalOrder.InternalOrder>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// شماره پرسنلی
        /// </summary>
        public int PersonNumber { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(50)]
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [MaxLength(50)]
        public string LastName { get; set; }
        /// <summary>
        /// نام کوتاه
        /// </summary>
        [MaxLength(50)]
        public string ShortName { get; set; }
        /// <summary>
        /// نام مستعار
        /// </summary>
        [MaxLength(50)]
        public string NikName { get; set; }
        /// <summary>
        /// نام پدر
        /// </summary>
        [MaxLength(50)]
        public string FatherName { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
        public bool Gender { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        [MaxLength(10)]
        public string NationalCode { get; set; }
        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        [MaxLength(12)]
        public string BirthCertificateNumber { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        [MaxLength(14)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        [MaxLength(14)]
        public string MobileNumber { get; set; }
        /// <summary>
        /// شماره تلفن ضروری
        /// </summary>
        public string NecessaryNumber { get; set; }
        /// <summary>
        /// صادره از
        /// </summary>
        public string ExportFrom{get;set;}
        /// <summary>
        /// محل تولد
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        [MaxLength(255)]
        public string Address { get; set; }
        /// <summary>
        /// سرپرست
        /// </summary>
        public bool Supervisor { get; set; }
        /// <summary>
        /// موضوع قرارداد
        /// </summary>
        public ContractSubject ContractSubject { get; set; }
        /// <summary>
        /// شماره سفته
        /// </summary>
        public string PromissoryNoteNumber { get; set; }
        /// <summary>
        /// شغل یا حرفه
        /// </summary>
        public int? JobId { get; set; }
        /// <summary>
        /// وضعیت تاهل
        /// </summary>
        public bool Married { get; set; }
        /// <summary>
        /// تعداد فرزند
        /// </summary>
        public int? Children { get; set; }
        /// <summary>
        /// قراردادکار
        /// </summary>
        public bool Contract { get; set; }
        /// <summary>
        /// مدت قرارداد به ماه
        /// </summary>
        public int ContractInMonth { get; set; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        [MaxLength(25)]
        public string AccountNumber { get; set; }
        /// <summary>
        /// تاریخ شروع همکاری
        /// </summary>
        public DateTime ContractStartDate { get; set; }
        /// <summary>
        /// تاریخ پایان همکاری
        /// </summary>
        public DateTime? ContractEndDate { get; set; }
        /// <summary>
        /// بیمه
        /// </summary>
        public bool Insurance { get; set; }
        /// <summary>
        /// شماره بیمه
        /// </summary>
        [MaxLength(20)]
        public string InsuranceNumber { get; set; }
        /// <summary>
        /// تاریخ انقضا بیمه پرسنل
        /// </summary>
        public DateTime? InsuranceExpireDate { get; set; }
        /// <summary>
        /// تاریخ شروع بیمه
        /// </summary>
        public DateTime? InsuranceBeginDate { get; set; }
        /// <summary>
        /// مدرک تحصیلی
        /// </summary>
        public Education Education { get; set; }
        /// <summary>
        /// پاداش مدیریتی
        /// </summary>
        public long? Reward { get; set; }
        /// <summary>
        /// میران کارکرد
        /// </summary>
        public long? ActivityAmount { get; set; }
        /// <summary>
        /// پایه سرپرست
        /// </summary>
        public long? SupervisorSalary { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// رشته تحصیلی
        /// </summary>
        [MaxLength(100)]
        public string Major { get; set; }
        /// <summary>
        /// رشته تحصیلی
        /// </summary>
        [MaxLength(100)]
        public string MajorCurrent { get; set; }
        /// <summary>
        /// وضعیت تحصیلی
        /// </summary>
        public bool EducationalStatus { get; set; }
        /// <summary>
        /// نوع کاربری
        /// </summary>
        public PersonType PersonType { get; set; }
        /// <summary>
        /// کارکرد در شیفت
        /// </summary>
        public decimal? ShiftActivity { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        [MaxLength(45)]
        public string Ip { get; set; }
        /// <summary>
        /// شرح وظایف
        /// </summary>
        public virtual PersonJobDescriptionTemplate Job { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// لیست حقوق و دستمزد
        /// </summary>
        public virtual List<Salary> SalaryList { get; set; }
        /// <summary>
        /// لیست فایل پرسنل
        /// </summary>
        public virtual List<PersonFile> PersonFileList { get; set; }
        /// <summary>
        /// لیست سفارشات شعب
        /// </summary>
        public virtual List<InternalOrder.InternalOrder> InternalOrderList { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        
    }
}
