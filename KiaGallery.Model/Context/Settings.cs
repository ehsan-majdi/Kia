using System;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// تنظیمات برنامه
    /// </summary>
    public class Settings
    {
        public static string KeyPostSenderAddress = "PostSenderAddress";
        public static string KeyBranch = "branch";
        public static string KeyDailyWage = "dailyWage";
        public static string KeyOccupationBeginDate = "OccupationBeginDate";
        public static string KeyLastFinancialUpdate = "lastFinancialUpdate";
        public static string KeyContractYear = "contractYear";
        public static string KeyBaseSalary = "baseSalary";
        public static string KeyYearAddedRate = "yearAddedRate";

        public static string KeyBranchPersonelHourlyPayment = "branchPersonelHourlyPayment";
        public static string KeyBranchPersonelOverTime = "branchPersonelOverTime";
        public static string KeyBranchPersonelMission = "branchPersonelMission";
        public static string KeyBranchPersonelMonthlyWorkingHour = "branchPersonelMonthlyWorkingHour";

        public static string KeySuperVisorHourlyPayment = "superVisorHourlyPayment";
        public static string KeySuperVisorOverTime = "superVisorOverTime";
        public static string KeySuperVisorMission = "superVisorMission";
        public static string KeySuperVisorMonthlyWorkingHour = "superVisorMonthlyWorkingHour";

        public static string KeyOfficeHourlyPayment = "officeHourlyPayment";
        public static string KeyOfficeOverTime = "officeOverTime";
        public static string KeyOfficeMission = "officeMission";
        public static string KeyOfficeMonthlyWorkingHour = "officeMonthlyWorkingHour";

        public static string KeyGoldPrice = "goldPrice";
        public static string KeyEuroPrice = "euroPrice";

        public static string KeyInPreprationGoldDebtDateRange = "inPreprationGoldDebtDateRange";

        //  تایین کننده این که  بر اساس سطحی که دارد چند درصد از خرید مشتری تبدیل به اعتبار شود مثلا اگر نقره ای 1.5 باشد از هر خرید 1.5 درصد وارد حساب مشتری میشود 
        public static string KeySilverCardValue = "silverCardValue";
        public static string KeyGoldenCardValue = "goldenCardValue";
        public static string KeyPlatinumCardValue = "platinumCardValue";

        //سطح هر کارت مه مشخص میکند هر مشتری به چه امتیازی نیاز دارد تا وارد سطح جدید شود مثلا اگر امتیاز مشتری به عدد 200 رسید سطحش بشه نقره ای که این عدد 200 مقدار هر کارت در زیر میشه 
        public static string KeySilverCardLevel = "silverCardLevel";
        public static string KeyGoldenCardLevel = "goldenCardLevel";
        public static string KeyPlatinumCardLevel = "platinumCardLevel";

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// کلید
        /// </summary>
        [MaxLength(150)]
        public string Key { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public string Value { get; set; }
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
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
    }
}
