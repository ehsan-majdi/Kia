using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// ارسال فکس سفارش محصولات مصرفی
    /// </summary>
    public class FaxOrderUsableProduct
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// مشخصات کسی که فکس را دریافت کرده است
        /// </summary>
        public string ReceiverName { get; set; }
        /// <summary>
        /// تاریخ ارسال فکس
        /// </summary>
        public DateTime DateFaxSend { get; set; }
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
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
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
        /// سفارش محصول مصرفی
        /// </summary>
        public virtual OrderUsableProduct OrderUsableProduct { get; set; }
    }
}
