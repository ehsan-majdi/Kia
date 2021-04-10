using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.DailyReportFinancial
{
    /// <summary>
    /// تقویم شعبه
    /// </summary>
    [Table(name: "BranchCalendar", Schema = "drf")]
    public class BranchCalendar
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public BranchCalendar()
        {
            DailyReportList = new List<DailyReport>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// تاریخ گزارش
        /// </summary>
        public DateTime ReportDate { get; set; }

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
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }

        /// <summary>
        /// لیست گزارش های شعبه
        /// </summary>
        public virtual List<DailyReport> DailyReportList { get; set; }
    }
}
