using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// مدل کمکی برای تعریف تصاویر محصول جهت انتشار محصولات
    /// </summary>
    public class InstagramPostViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// ردیف فایل
        /// </summary>
        public string fileId { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// روزشمار جهت ساخت محصول و استخراج محصول از کارگاه 
        /// </summary>
        public int dayCounter { get; set; }
        /// <summary>
        /// تاریخ انتشار برای تایپ محصول
        /// </summary>
        public string publishDate { get; set; }
        /// <summary>
        /// تاریخ انتشار برای تبدیل کردن
        /// </summary>
        public DateTime? date { get; set; }
        /// <summary>
        /// فرجه تعداد روز 
        /// </summary>
        public int respite { get; set; }
        /// <summary>
        /// تعیین نوع پست جهت نشر آن در اینستاگرام
        /// </summary>
        public InstagramPostType instagramPostType { get; set; }
    }
    /// <summary>
    /// برگرداندن لیست محصولات و پست های اینستاگرام که در دریتابیس ذخیره شده اند
    /// </summary>
    public class InstagramPostSearchViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// ردیف فایل
        /// </summary>
        public string fileId { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// روزشمار جهت ساخت محصول و استخراج محصول از کارگاه 
        /// </summary>
        public int dayCounter { get; set; }
        /// <summary>
        /// تاریخ انتشار برای تایپ محصول
        /// </summary>
        public DateTime? publishDate { get; set; }
        /// <summary>
        /// تاریخ انتشار شمسی
        /// </summary>
        public string persianPublishDate { get; set; }
        /// <summary>
        /// فرجه تعداد روز 
        /// </summary>
        public int respite { get; set; }
        /// <summary>
        /// تعیین نوع پست یا محصول در اینستاگرام
        /// </summary>
        public InstagramPostType instagramPostType { get; set; }
        /// <summary>
        /// عنوان نوع پست یا محصول
        /// </summary>
        public string instagramPostTypeTitle { get; set; }
        public string finalPersianPublishDate { get; set; }
        public DateTime? finalPublishDate { get; set; }
    }
    /// <summary>
    /// پارامتر های ارسالی مربوط به متد جستجو
    /// </summary>
    public class InstagramPostParamsViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public InstagramPostType? instagramPostType { get; set; }
    }
    public class InstagramPostDateViewModel
    {
        /// <summary>
        /// روزشمار جهت ساخت محصول و استخراج محصول از کارگاه 
        /// </summary>
        public int dayCounter { get; set; }
        /// <summary>
        /// تاریخ انتشار برای تایپ محصول
        /// </summary>
        public string publishDate { get; set; }
    }


}