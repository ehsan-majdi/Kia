using System;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context.StoneTable
{
    /// <summary>
    /// سنگ های به اتمام رسیده
    /// </summary>
    public class StoneOutOfStock
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف سنگ
        /// </summary>
        public int StoneId { get; set; }
        /// <summary>
        /// ردیف ابعاد شکل سنگ
        /// </summary>
        public int ShapeSizeId { get; set; }

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
        /// سنگ
        /// </summary>
        public virtual Stone Stone { get; set; }
        /// <summary>
        /// ابعاد شکل سنگ
        /// </summary>
        public virtual ShapeSize ShapeSize { get; set; }
    }
}
