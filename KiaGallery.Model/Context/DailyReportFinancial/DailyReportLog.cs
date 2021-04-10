using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.DailyReportFinancial
{
    /// <summary>
    /// تاریخچه تغییر وضعیت سفارش ها
    /// </summary>
    [Table(name: "DailyReportLog", Schema = "drf")]
    public class DailyReportLog
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف گزارش روزانه
        /// </summary>
        public int DailyReportId { get; set; }
        /// <summary>
        /// ردیف کاربر ثبت کننده
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// تاریخ ثبت
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public CalendarStatus Status { get; set; }
        /// <summary>
        /// اطلاعات ثبت شده و تغییر داده شده
        /// </summary>
        public string PrevData { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        [MaxLength(48)]
        public string Ip { get; set; }

        /// <summary>
        /// گزارش روزانه
        /// </summary>
        public virtual DailyReport DailyReport { get; set; }
        /// <summary>
        /// کاربر ثبت گننده
        /// </summary>
        public virtual User User { get; set; }
    }
}
