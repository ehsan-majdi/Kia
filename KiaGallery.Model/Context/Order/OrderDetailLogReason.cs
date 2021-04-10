using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// علت ثبت شده برای وضعیت محصول سفارش داده شده
    /// </summary>
    [Table(name: "OrderDetailLogReason", Schema = "order")]
    public class OrderDetailLogReason
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public OrderDetailLogReason()
        {
            OrderDetailLogList = new List<OrderDetailLog>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// وضعیت محصول سفارش داده شده
        /// </summary>
        public OrderDetailStatus OrderDetailStatus { get; set; }
        /// <summary>
        /// متن
        /// </summary>
        [MaxLength(255)]
        public string Text { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }

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
        /// لیست محصول های سفارشات
        /// </summary>
        public virtual List<OrderDetailLog> OrderDetailLogList { get; set; }
    }
}
