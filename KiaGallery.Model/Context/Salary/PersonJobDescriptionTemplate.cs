using System;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context.Salary
{
    /// <summary>
    /// متن پیش فرض شرح وظایف پرسنل
    /// </summary>
    public class PersonJobDescriptionTemplate
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        [MaxLength(200)]
        public string Title { get; set; }
        /// <summary>
        /// شرح وظایف
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Status { get; set; }
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
    }
}
