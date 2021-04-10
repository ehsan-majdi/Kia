using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// کارگاه
    /// </summary>
    public class Workshop
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public Workshop()
        {
            ProductList = new List<Product>();
            UserList = new List<User>();
            ProductList2 = new List<Product>();
            WorkShopGoldList = new List<WorkShopGold>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        [Column("OrderNo")]
        public int Order { get; set; }
        /// <summary>
        /// نام مستعار
        /// </summary>
        [MaxLength(5)]
        public string Alias { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// رنگ
        /// </summary>
        [MaxLength(7)]
        public string Color { get; set; }
        /// <summary>
        /// تایید خودکار بعد ثبت سفارش
        /// </summary>
        public bool AutoConfirm { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// دسترسی خرید و فروش طلا
        /// </summary>
        public bool? GoldTrade { get; set; }
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
        /// لیست محصولات
        /// </summary>
        public virtual List<Product> ProductList { get; set; }
        /// <summary>
        /// لیست محصولات
        /// </summary>
        public virtual List<Product> ProductList2 { get; set; }
        /// <summary>
        /// لیست کاربران
        /// </summary>
        public virtual List<User> UserList { get; set; }
        /// <summary>
        /// لیست طلای کارگاه ها
        /// </summary>
        public virtual List<WorkShopGold> WorkShopGoldList { get; set; }
    }
}
