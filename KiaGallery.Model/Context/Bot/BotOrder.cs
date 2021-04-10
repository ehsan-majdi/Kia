using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// سفارش
    /// </summary>
    [Table(name: "BotOrder", Schema = "Bot")]
    public class BotOrder
    {
        public BotOrder()
        {
            BotOrderLogList = new List<BotOrderLog>();
            BotOrderLeatherList = new List<BotOrderLeather>();
            BotOrderStoneList = new List<BotOrderStone>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف کاربری
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// ردیف چت
        /// </summary>
        public long ChatId { get; set; }
        /// <summary>
        /// سریال سفارش
        /// </summary>
        public string OrderSerial { get; set; }
        /// <summary>
        /// ردیف محصول
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// سایز
        /// </summary>
        [MaxLength(100)]
        public string Size { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(100)]
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [MaxLength(100)]
        public string LastName { get; set; }
        /// <summary>
        /// شماره تماس
        /// </summary>
        [MaxLength(16)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public BotOrderStatus Status { get; set; }
        /// <summary>
        /// بیعانه
        /// </summary>
        public int? Deposit { get; set; }
        /// <summary>
        /// جزئیات خرید
        /// </summary>
        public string CardDetails { get; set; }
        /// <summary>
        /// نحوی پرداخت
        /// </summary>
        public int? PaymentType { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        [MaxLength(1024)]
        public string Address { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// لیست سوابق وضعیت محصولات
        /// </summary>
        public virtual List<BotOrderLog> BotOrderLogList { get; set; }
        /// <summary>
        /// محصول
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// لیست سنگ های سفارش
        /// </summary>
        public virtual List<BotOrderStone> BotOrderStoneList { get; set; }
        /// <summary>
        /// لیست چرم های سفارش
        /// </summary>
        public virtual List<BotOrderLeather> BotOrderLeatherList { get; set; }
        /// <summary>
        /// کاربر
        /// </summary>
        public virtual BotUserData BotUserData { get; set; }
    }
}