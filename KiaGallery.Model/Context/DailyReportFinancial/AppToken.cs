using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.DailyReportFinancial
{
    /// <summary>
    /// توکن های صادر شده برای استفاده از سیستم
    /// </summary>
    [Table(name: "AppToken", Schema = "drf")]
    public class AppToken
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// کد شناسایی
        /// </summary>
        [MaxLength(100)]
        public string Code { get; set; }
        /// <summary>
        /// منقضی شده
        /// </summary>
        public bool Voided { get; set; }
        /// <summary>
        /// نوع کاربر توکن
        /// </summary>
        public TokenType TokenType { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ منقضی شدن
        /// </summary>
        public DateTime? VoidedDate { get; set; }

        /// <summary>
        /// کاربر
        /// </summary>
        public virtual User User { get; set; }
    }
}
