using System;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context.InternalOrder
{
    /// <summary>
    /// سوابق سفارشات مربوط به هر مشتری
    /// </summary>
    public class InternalOrderLog
    {
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف سفارش داخلی
        /// </summary>
        public int InternalOrderId { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// متن
        /// </summary>
        [MaxLength(500)]
        public string Text { get; set; }
        /// <summary>
        ///تاریخ ایجاد سفارش
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// سفارش داخلی
        /// </summary>
        public InternalOrder InternalOrder { get; set; }
        /// <summary>
        /// کاربر
        /// </summary>
        public User User { get; set; }
    }
}
