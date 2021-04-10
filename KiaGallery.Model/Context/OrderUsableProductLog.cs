using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// لاگ سفارشات محصول مصرفی
    /// </summary>
    public class OrderUsableProductLog
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// وضعیت سفارش محصول مصرفی
        /// </summary>
        public OrderUsableProductStatus OrderUsableProductStatus { get; set; }
        /// <summary>
        /// ردیف سفارش محصول مصرفی
        /// </summary>
        public int OrderUsableProductId { get; set; }
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
        /// سفارش محصولات مصرفی
        /// </summary>
        public virtual OrderUsableProduct OrderUsableProduct { get; set; }
    }
}
