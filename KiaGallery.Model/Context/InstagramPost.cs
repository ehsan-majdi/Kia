using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// مدل تصاویر محصولات که قرار است در اینستاگرام منتشر شود
    /// </summary>
    public class InstagramPost
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// ردیف فایل
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// روزشمار جهت ساخت محصول و استخراج محصول از کارگاه 
        /// </summary>
        public int DayCounter { get; set; }
        /// <summary>
        /// تاریخ انتشار برای تایپ محصول
        /// </summary>
        public DateTime? PublishDate{ get; set; }
        /// <summary>
        /// فرجه تعداد روز 
        /// </summary>
        public int Respite { get; set; }
        /// <summary>
        /// تعیین نوع پست جهت نشر آن در اینستاگرام
        /// </summary>
        public InstagramPostType InstagramPostType { get; set; }
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
