using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// جزئیات سفارش
    /// </summary>
    [Table(name: "OrderDetail", Schema = "order")]
    public class OrderDetail
    {
        /// <summary>
        /// سفارش
        /// </summary>
        public OrderDetail()
        {
            OrderDetailStoneList = new List<OrderDetailStone>();
            OrderDetailLeatherList = new List<OrderDetailLeather>();
            OrderDetailLogList = new List<OrderDetailLog>();
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
        /// ردیف سفارش کارگاه
        /// </summary>
        public int? WorkshopOrderId { get; set; }
        /// <summary>
        /// ردیف سفارش کارگاه
        /// </summary>
        public int? WorkshopOrderId2 { get; set; }
        /// <summary>
        /// ارسال شده به کارگاه دوم
        /// </summary>
        public bool? SendWorkshopOrder2 { get; set; }
        /// <summary>
        /// ردیف محصول
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// شماره ست
        /// </summary>
        public int? SetNumber { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// نوع سفارش
        /// </summary>
        public OrderType OrderType { get; set; }
        /// <summary>
        /// وضعیت محصول
        /// </summary>
        public OrderDetailStatus OrderDetailStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ProductColor? ProductColor { get; set; }
        /// <summary>
        /// سایز
        /// </summary>
        [MaxLength(100)]
        public string Size { get; set; }
        /// <summary>
        /// سایز2
        /// </summary>
        [MaxLength(100)]
        public string Size2 { get; set; }
        /// <summary>
        /// نوع طلا
        /// </summary>
        public GoldType? GoldType { get; set; }
        /// <summary>
        /// تعداد دور
        /// </summary>
        public byte? LeatherLoop { get; set; }
        /// <summary>
        /// مشتری
        /// </summary>
        [MaxLength(50)]
        public string Customer { get; set; }
        /// <summary>
        /// تلفن مشتری
        /// </summary>
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// سفارش عجله ای
        /// </summary>
        public bool? ForceOrder { get; set; }
        /// <summary>
        /// برچسب شعبه
        /// </summary>
        [MaxLength(255)]
        public string BranchLabel { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }
        /// <summary>
        ///  2 توضیحات
        /// </summary>
        [MaxLength(1000)]
        public string Description2 { get; set; }
        /// <summary>
        /// ردیف محصول مرتبط در سفارش ثبت شده دیگر
        /// </summary>
        public int? RelatedOrderDetailId { get; set; }
        /// <summary>
        /// نوع خرج کار
        /// </summary>
        public OuterWerkType? OuterWerkType { get; set; }
        /// <summary>
        /// تاریخ تحویل درخواستی
        /// </summary>
        public DateTime? DeliverDateRequest { get; set; }
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
        /// محصول
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// سفارش
        /// </summary>
        public virtual Order Order { get; set; }
        /// <summary>
        /// سفارش کارگاه
        /// </summary>
        public virtual WorkshopOrder WorkshopOrder { get; set; }
        /// <summary>
        /// سفارش کارگاه
        /// </summary>
        public virtual WorkshopOrder WorkshopOrder2 { get; set; }
        /// <summary>
        /// محصول مرتبط در سفارش ثبت شده دیگر
        /// </summary>
        public virtual OrderDetail RelatedOrderDetail { get; set; }

        /// <summary>
        /// لیست سنگ های سفارش
        /// </summary>
        public virtual List<OrderDetailStone> OrderDetailStoneList { get; set; }
        /// <summary>
        /// لیست چرم های سفارش
        /// </summary>
        public virtual List<OrderDetailLeather> OrderDetailLeatherList { get; set; }
        /// <summary>
        /// لیست سوابق وضعیت محصولات
        /// </summary>
        public virtual List<OrderDetailLog> OrderDetailLogList { get; set; }
        /// <summary>
        /// لیست سوابق محصولات دارای ارتباط
        /// </summary>
        public virtual List<OrderDetail> RelatedOrderDetailList { get; set; }
    }
}
