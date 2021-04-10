using KiaGallery.Model.Context.StoneTable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// ابعاد اشکال سنگ
    /// </summary>
    public class ShapeSize
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public ShapeSize()
        {
            ProductStoneList = new List<ProductStone>();
            StoneOutOfStockList = new List<StoneOutOfStock>();

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
        /// شکل سنگ
        /// </summary>
        public StoneShape StoneShape { get; set; }
        /// <summary>
        /// اندازه طول سنگ
        /// </summary>
        public float SizeLength { get; set; }
        /// <summary>
        /// اندازه عرض سنگ
        /// </summary>
        public float SizeWidth { get; set; }
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
        /// لیست سنگ های دارای این سایز
        /// </summary>
        public virtual List<ProductStone> ProductStoneList { get; set; }
        /// <summary>
        /// سنگ های به اتمام رسیده
        /// </summary>
        public virtual List<StoneOutOfStock> StoneOutOfStockList { get; set; }
    }
}
