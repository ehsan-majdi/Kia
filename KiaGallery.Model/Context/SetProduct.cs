using System;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// محصولات ست
    /// </summary>
    public class SetProduct
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف محصول
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// ردیف محصول ست
        /// </summary>
        public int SetToId { get; set; }

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
        /// محصول
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// محصول ُت
        /// </summary>
        public virtual Product SetTo { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
    }
}
