using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// سفارش
    /// </summary>
    [Table(name: "Order", Schema = "order")]
    public class Order
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public Order()
        {
            OrderDetailList = new List<OrderDetail>();
            WorkshopOrderList = new List<WorkshopOrder>();
            OrderLogList = new List<OrderLog>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// شماره سفارش
        /// </summary>
        [MaxLength(25)]
        public string OrderNumber { get; set; }
        /// <summary>
        /// سریال سفارش
        /// </summary>
        [MaxLength(25)]
        public string OrderSerial { get; set; }
        /// <summary>
        /// وضعیت سفارش
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }
        /// <summary>
        /// سفارشات حذف شده
        /// </summary>
        public bool Deleted { get; set; }

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
        /// لیست محصولات
        /// </summary>
        public virtual List<OrderDetail> OrderDetailList { get; set; }
        /// <summary>
        /// لیست سفارشات کارگاه
        /// </summary>
        public virtual List<WorkshopOrder> WorkshopOrderList { get; set; }
        /// <summary>
        /// لیست سوابق سفارش
        /// </summary>
        public virtual List<OrderLog> OrderLogList { get; set; }
    }

}
