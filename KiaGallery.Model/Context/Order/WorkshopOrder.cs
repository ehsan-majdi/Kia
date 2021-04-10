using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// سفارشات کارگاه
    /// </summary>
    [Table(name: "WorkshopOrder", Schema = "order")]
    public class WorkshopOrder
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public WorkshopOrder()
        {
            OrderDetailList = new List<OrderDetail>();
            OrderDetailList2 = new List<OrderDetail>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// سریال سفارش کارگاه
        /// </summary>
        [MaxLength(25)]
        public string WorkshopOrderSerial { get; set; }
        /// <summary>
        /// شماره سفارش کارگاه
        /// </summary>
        [MaxLength(25)]
        public string WorkshopOrderNumber { get; set; }

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
        /// سفارش
        /// </summary>
        public virtual Order Order { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// لیست جزئیات سفارش
        /// </summary>
        public virtual List<OrderDetail> OrderDetailList { get; set; }
        /// <summary>
        /// لیست جزئیات سفارش
        /// </summary>
        public virtual List<OrderDetail> OrderDetailList2 { get; set; }
    }
}
