using System;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// جدول تعداد فاکتور شعب
    /// </summary>
    public class BranchFactor
    {
        /// <summary>
        /// ردیف فاکتور شعب
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعب
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// تعدادفاکتور
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ویرایش
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
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

    }
}
