using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// جزئیات سفارش محصول مصرفی
    /// </summary>
    public class OrderUsableProductDetail
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int OrderUsableProductId { get; set; }
        /// <summary>
        /// ردیف محصول مصرفی
        /// </summary>
        public int UsableProductId { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// تعداد موجودی دفتر مرکزی
        /// </summary>
        public int? OfficeInventory { get; set; }
        /// <summary>
        /// تعداد باقی مانده
        /// </summary>
        public int? Remain { get; set; }
        /// <summary>
        /// موجودی چاپخانه
        /// </summary>
        public int? PrintingHouseInventory { get; set; }
        /// <summary>
        /// موجودی تحویل داده شد
        /// </summary>
        public int? Delivered { get; set; }
        /// <summary>
        /// آماده تحویل
        /// </summary>
        public int? ReadyForDelivery { get; set; }
        /// <summary>
        /// آماده تحویل نهایی
        /// </summary>
        public int? ConfirmReadyForDelivery { get; set; }
        /// <summary>
        /// تحویل داده شده نهایی
        /// </summary>
        public int? ConfirmDelivered { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int RemainFinal { get; set; }
        /// <summary>
        /// مشخصات
        /// </summary>
        public string Specification { get; set; }
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
        /// لیست محصول مصرفی
        /// </summary>
        public virtual UsableProduct UsableProduct { get; set; }
        /// <summary>
        /// سبد سفارش پک
        /// </summary>
        public virtual OrderUsableProduct OrderUsableProduct { get; set; }
    }
}
