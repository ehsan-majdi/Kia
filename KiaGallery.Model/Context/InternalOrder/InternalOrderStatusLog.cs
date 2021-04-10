using System;


namespace KiaGallery.Model.Context.InternalOrder
{
    /// <summary>
    /// لیست سوابق وضعیت سفارش داخلی
    /// </summary>
    public class InternalOrderStatusLog
    {
        /// <summary>
        /// ردیف 
        /// /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int InternalOrderId { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// وضعیت سفارش داخلی
        /// </summary>
        public InternalOrderStatus InternalOrderStatus { get; set; }
        //[MaxLength(500)]
        //public string Description { get; set; }
        /// <summary>
        /// تاریخ یادآوری سفارش
        /// </summary>
        public DateTime? RemindDate { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ آی پی کاربر
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        ///سفارش داخلی 
        /// </summary>
        public InternalOrder InternalOrder { get; set; }
        /// <summary>
        /// کاربر
        /// </summary>
        public User User { get; set; }
    }
}
