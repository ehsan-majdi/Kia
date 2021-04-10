using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// ردیف
    /// </summary>
    [Table(name: "BotOrderLog", Schema = "Bot")]
    public class BotOrderLog
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف محصول سفارش داده شده
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// وضعیت محصول سفارش داده شده
        /// </summary>
        public BotOrderStatus Status { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
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
        /// محصول سفارش داده شده
        /// </summary>
        public virtual BotOrder BotOrder { get; set; }
        /// <summary>
        /// کاربر ایحاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
    }
}
