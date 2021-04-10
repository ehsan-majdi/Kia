using KiaGallery.Model.Context.InternalOrder;
using KiaGallery.Model.Context.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// چرم
    /// </summary>
    public class Leather
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public Leather()
        {
            ProductLeatherList = new List<ProductLeather>();
            CartProductLeatherList = new List<CartProductLeather>();
            OrderDetailLeatherList = new List<OrderDetailLeather>();
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
        /// نوع چرم
        /// </summary>
        public LeatherType LeatherType { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        [Column("OrderNo")]
        public int Order { get; set; }
        /// <summary>
        /// نام فایل تصویر
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
        /// لیست محصولات چرم
        /// </summary>
        public virtual List<ProductLeather> ProductLeatherList { get; set; }
        /// <summary>
        /// لیست محصولات موجود در سبد خرید با چرم
        /// </summary>
        public virtual List<CartProductLeather> CartProductLeatherList { get; set; }
        /// <summary>
        /// لیست محصولات موجود در سفارشات با چرم
        /// </summary>
        public virtual List<OrderDetailLeather> OrderDetailLeatherList { get; set; }
    }
}
