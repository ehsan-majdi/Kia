using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// سفارش محصولات مصرفی
    /// </summary>
    public class OrderUsableProduct
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public OrderUsableProduct()
        {
            OrderUsableProductDetailList = new List<OrderUsableProductDetail>();
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
        /// توضیحات
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// وضعیت رسیدن سفارش به چاپخانه 
        /// </summary>
        public bool PrintingHouserOrder { get; set; }
        /// <summary>
        /// وضعیت سفارش محصولات مصرفی
        /// </summary>
        public OrderUsableProductStatus OrderUsableProductStatus { get; set; }
        /// <summary>
        /// تاییدیه وضعیت سفارش محصولات مصرفی در چاپخانه
        /// </summary>
        public PrintingHouseOrderStatus PrintingHouseOrderStatus { get; set; }
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
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// لیست جزئیات سفارش محصول مصرفی
        /// </summary>
        public virtual List<OrderUsableProductDetail> OrderUsableProductDetailList { get; set; }
        /// <summary>
        /// سوابق سفارش محصول مصرفی
        /// </summary>
        public virtual List<OrderUsableProductLog> OrderUsableProductLog { get; set; }
    }
}
