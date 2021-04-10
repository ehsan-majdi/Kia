using KiaGallery.Model.Context.Salary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.InternalOrder
{
    /// <summary>
    /// سیستم سفارش داخلی 
    /// </summary>
    [Table(name: "InternalOrder", Schema = "internalOrder")]
    public class InternalOrder
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public InternalOrder()
        {
            InternalOrderStatusLogList = new List<InternalOrderStatusLog>();
            InternalOrderDetailList = new List<InternalOrderDetail>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// تعداد پیش فاکتورها
        /// </summary>
        public int DetailCount { get; set; }
        /// <summary>
        /// بیعانه
        /// </summary>
        public int Deposit { get; set; }
        /// <summary>
        /// نام مشتری
        /// </summary>
        [MaxLength(150)]
        public string Name { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
        public Gender Gender { get; set; }
        /// <summary>
        /// شماره تماس مشتری
        /// </summary>
        [MaxLength(12)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// تلفن ثابت مشتری
        /// </summary>
        [MaxLength(12)]
        public string Telephone { get; set; }
        /// <summary>
        /// نوع محصول سفارشی
        /// </summary>
        public OrderTypeForm OrderTypeForm { get; set; }
        /// <summary>
        /// وضعیت سفارش داخلی
        /// </summary>
        public InternalOrderStatus InternalOrderStatus { get; set; }
        /// <summary>
        /// نوع ارسال
        /// </summary>
        public DeliveryType? DeliveryType { get; set; }
        /// <summary>
        /// نوع مشتری
        /// </summary>
        public UserType UserType { get; set; }
        /// <summary>
        /// شعبه تحویل گیرنده
        /// </summary>
        public int? DeliveredBranchId { get; set; }
        /// <summary>
        /// وضعیت مالی
        /// </summary>
        public bool? PonyUp { get; set; }
        /// <summary>
        /// ردیف پرسنل ایجاد کننده
        /// </summary>
        public int? CreatePersonId { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int? CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int? ModifyUserId { get; set; }
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
        public string Ip { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// ردیف شعبه تحویل گیرنده
        /// </summary>
        public virtual Branch DeliveredBranch { get; set; }
        /// <summary>
        /// پرسنل ثبت کننده
        /// </summary>
        public virtual Person CreatePerson { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// لیست سوابق وضعیت سفارش داخلی
        /// </summary>
        public virtual List<InternalOrderStatusLog> InternalOrderStatusLogList { get; set; }
        /// <summary>
        ///جزییات سفارشات داخلی 
        /// </summary>
        public virtual List<InternalOrderDetail> InternalOrderDetailList { get; set; }
    }

}


