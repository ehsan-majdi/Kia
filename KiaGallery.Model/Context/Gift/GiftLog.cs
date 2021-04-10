using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Gift
{
    /// <summary>
    /// سوابق گیفت
    /// </summary>
    [Table(name: "GiftLog", Schema = "gift")]
    public class GiftLog
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// وضعیت گیف
        /// </summary>
        public GiftStatus GiftStatus { get; set; }
        /// <summary>
        /// ردیف گیفت
        /// </summary>
        public int GiftId { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
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
        /// گیفت
        /// </summary>
        public virtual Gift Gift { get; set; }
    }
}
