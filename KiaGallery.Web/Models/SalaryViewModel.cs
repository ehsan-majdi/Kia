using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class SalaryViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف بعد
        /// </summary>
        public int? nextId { get; set; }
        /// <summary>
        /// ردیف قبل
        /// </summary>
        public int? prevId { get; set; }
        /// <summary>
        /// شماره پرسنلی
        /// </summary>
        public int personNumber { get; set; }
        /// <summary>
        /// شماره پرسنلی
        /// </summary>
        public string branch { get; set; }
        /// <summary>
        /// نام، نام خانوادگی
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// ردیف پرسنلی
        /// </summary>
        public int personId { get; set; }
        /// <summary>
        /// ساعت کارکرد
        /// </summary>
        public string workHours { get; set; }
        /// <summary>
        /// نرخ دستمزد ساعتی
        /// </summary>
        public long? hourlyWageRate { get; set; }
        /// <summary>
        /// نرخ اضافه کاری
        /// </summary>
        public long? overtimeRate { get; set; }
        /// <summary>
        ///  ماموریت 
        /// </summary>
        public long? mission { get; set; }
        /// <summary>
        ///  عیدی 
        /// </summary>
        public long? reward { get; set; }
        /// <summary>
        ///  پاداش 
        /// </summary>
        public long? remuneration { get; set; }
        /// <summary>
        ///  سایر 
        /// </summary>
        public long? others { get; set; }

        /// <summary>
        ///  قسط وام 
        /// </summary>
        public long? loanInstallment { get; set; }
        /// <summary>
        ///  مساعده 
        /// </summary>
        public long? imprest { get; set; }
        /// <summary>
        ///  قسط کالا 
        /// </summary>
        public long? commodityItem { get; set; }
        /// <summary>
        /// بیمه
        /// </summary>
        public bool insurance { get; set; }

        /// <summary>
        ///  ماه مورد محاسبه 
        /// </summary>
        public DateTime monthCalculatedDate { get; set; }
        /// <summary>
        ///  ماه مورد محاسبه 
        /// </summary>
        public string monthCalculated { get; set; }
        /// <summary>
        ///  نوع پرسنلی 
        /// </summary>
        public PersonType personType { get; set; }
    }
    public class SalaryParamsViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
        public string date { get; set; }
        public int? branchId { get; set; }
    }
    public class SalarySearchViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// شماره پرسنلی
        /// </summary>
        public string personNumber { get; set; }
        /// <summary>
        /// نام، نام خانوادگی
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public string branch { get; set; }
        /// <summary>
        /// ساعت کارکرد
        /// </summary>
        public string workHours { get; set; }
        /// <summary>
        ///  ماه مورد محاسبه 
        /// </summary>
        public DateTime monthCalculatedDate { get; set; }
        /// <summary>
        ///  ماه مورد محاسبه 
        /// </summary>
        public string monthCalculated { get; set; }
    }

    public class SettingsViewModel
    {
        public string contractYear { get; set; }
        public string baseSalary { get; set; }
        public string yearAddedRate { get; set; }
        public string branchPersonelHourlyPayment { get; set; }
        public string branchPersonelOverTime { get; set; }
        public string branchPersonelMission { get; set; }
        public string branchPersonelMonthlyWorkingHour { get; set; }
        public string superVisorHourlyPayment { get; set; }
        public string superVisorOverTime { get; set; }
        public string superVisorMission { get; set; }
        public string superVisorMonthlyWorkingHour { get; set; }
        public string officeHourlyPayment { get; set; }
        public string officeOverTime { get; set; }
        public string officeMission { get; set; }
        public string officeMonthlyWorkingHour { get; set; }
    }
}