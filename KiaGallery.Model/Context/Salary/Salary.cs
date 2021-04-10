using System;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context.Salary
{
    /// <summary>
    /// حقوق دستمزد
    /// </summary>
    public class Salary
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف پرسنلی
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// ساعت کارکرد
        /// </summary>
        [MaxLength(6)]
        public string WorkHours { get; set; }
        /// <summary>
        /// نرخ دستمزد ساعتی
        /// </summary>
        public long? HourlyWageRate { get; set; }
        /// <summary>
        /// نرخ اضافه کاری
        /// </summary>
        public long? OvertimeRate{ get; set; }
        /// <summary>
        ///  ماموریت 
        /// </summary>
        public long? Mission { get; set; }
        /// <summary>
        ///  عیدی 
        /// </summary>
        public long? Reward { get; set; }
        /// <summary>
        ///  پاداش 
        /// </summary>
        public long? Remuneration { get; set; }
        /// <summary>
        ///  سایر 
        /// </summary>
        public long? Others { get; set; }

        /// <summary>
        ///  قسط وام 
        /// </summary>
        public long? LoanInstallment { get; set; }
        /// <summary>
        ///  مساعده 
        /// </summary>
        public long? Imprest { get; set; }
        /// <summary>
        ///  قسط کالا 
        /// </summary>
        public long? CommodityItem { get; set; }

        /// <summary>
        ///  ماه مورد محاسبه 
        /// </summary>
        public DateTime MonthCalculated { get; set; }
        /// <summary>
        /// بیمه
        /// </summary>
        public bool Insurance { get; set; }
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
        /// <summary>
        /// پرسنل
        /// </summary>
        public virtual Person Person { get; set; }

    }
}
