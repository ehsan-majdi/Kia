using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// تنظیمات تخفیفات فاکتور مشتری
    /// </summary>
    [Table("DiscountSetting", Schema = "crm")]
    public class CrmDiscountSetting
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// از قیمت
        /// </summary>
        public int FromPrice { get; set; }
        /// <summary>
        /// تا قیمت
        /// </summary>
        public int ToPrice { get; set; }
        /// <summary>
        /// میزان تخفیف
        /// </summary>
        public float Discount { get; set; }

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
        /// آی پی کاربر ایجاد کننده
        /// </summary>
        [MaxLength(45)]
        public string CreateIp { get; set; }
        /// <summary>
        /// آی پی کاربر ویرایش کننده
        /// </summary>
        [MaxLength(45)]
        public string ModifyIp { get; set; }

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
