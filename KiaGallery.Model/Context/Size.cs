using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// سایز
    /// </summary>
    public class Size
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        [MaxLength(100)]
        public string Title { get; set; }
        /// <summary>
        /// مقدار پیشفرض
        /// </summary>
        [MaxLength(100)]
        public string DefaultValue { get; set; }

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
        /// لیست محصولات دارای سایز
        /// </summary>
        public virtual List<Product> ProductList { get; set; }
        /// <summary>
        /// لیست مقادیر سایز
        /// </summary>
        public virtual List<SizeValue> SizeValueList { get; set; }
    }
}
