using KiaGallery.Model.Context.InternalOrder;
using KiaGallery.Model.Context.Order;
using KiaGallery.Model.Context.StoneTable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// سنگ
    /// </summary>
    public class Stone
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public Stone()
        {
            DefaultStoneList = new List<ProductStone>();
            ProductStoneList = new List<ProductStone>();
            CartProductStoneList = new List<CartProductStone>();
            OrderDetailStoneList = new List<OrderDetailStone>();
            StoneOutOfStockList = new List<StoneOutOfStock>();
            //InternalOrderDetailStonesList = new List<InternalOrderDetailStone>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(50)]
        public string EnglishName { get; set; }
        /// <summary>
        /// نوع سنگ
        /// </summary>
        public StoneType StoneType { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        [Column("OrderNo")]
        public int Order { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        [MaxLength(255)]
        public string FileName { get; set; }
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
        /// لیست سنگ پیش فرض
        /// </summary>
        public virtual List<ProductStone> DefaultStoneList { get; set; }
        /// <summary>
        /// لیست محصولات سنگ
        /// </summary>
        public virtual List<ProductStone> ProductStoneList { get; set; }
        /// <summary>
        /// لیست محصولات موجود در سبد خرید با سنگ
        /// </summary>
        public virtual List<CartProductStone> CartProductStoneList { get; set; }
        /// <summary>
        /// لیست محصولات موجود در سفارشات با سنگ
        /// </summary>
        public virtual List<OrderDetailStone> OrderDetailStoneList { get; set; }
        /// <summary>
        /// سنگ های به اتمام رسیده
        /// </summary>
        public virtual List<StoneOutOfStock> StoneOutOfStockList { get; set; }
        ///// <summary>
        ///// سنگ هایی که هر مشتری برای سفارش خود انتخاب کرده است
        ///// </summary>
        //public virtual List<InternalOrderDetailStone> InternalOrderDetailStonesList { get; set; }
    }
}
