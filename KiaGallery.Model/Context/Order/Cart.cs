using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Order
{
    /// <summary>
    /// سبد خرید
    /// </summary>
    [Table(name: "Cart", Schema = "order")]
    public class Cart
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public Cart()
        {
            CartProductStoneList = new List<CartProductStone>();
            CartProductLeatherList = new List<CartProductLeather>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int BranchId { get; set; }
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
        /// نوع طلا
        /// </summary>
        public GoldType? GoldType { get; set; }
        /// <summary>
        /// نوع خرج کار
        /// </summary>
        public OuterWerkType? OuterWerkType { get; set; }
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
        /// 
        /// </summary>
        [MaxLength(100)]
        public string Size2 { get; set; }
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
        /// عجله ای
        /// </summary>
        public bool? ForceOrder { get; set; }
        /// <summary>
        /// تاریخ تحویل درخواستی
        /// </summary>
        public DateTime? DeliverDateRequest { get; set; }
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
        /// محصول
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// لیست سنگ ها
        /// </summary>
        public virtual List<CartProductStone> CartProductStoneList { get; set; }
        /// <summary>
        /// لیست چرم ها
        /// </summary>
        public virtual List<CartProductLeather> CartProductLeatherList { get; set; }
    }
}
