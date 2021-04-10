using System;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// مدل فایل های مارکیز
    /// </summary>
    public class MarquisFileViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int branchId { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// تعداد فایل
        /// </summary>
        public int fileCount { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// تاریخ ایجاد شمسی
        /// </summary>
        public string persianDate { get; set; }
    }

    /// <summary>
    /// مدل جزئیات فایل های مارکیز
    /// </summary>
    public class MarquisFileDetailViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// ردیف فایل مارکیز
        /// </summary>
        public int marquisFileId { get; set; }
        /// <summary>
        /// ردیف فایل
        /// </summary>
        public string fileId { get; set; }
        /// <summary>
        /// عنوان فایل
        /// </summary>
        public string fileName { get; set; }
    }

    /// <summary>
    /// مدل پارامترهای جستجو فایل های مارکیز
    /// </summary>
    public class MarquisFileSearchViewModel
    {
        /// <summary>
        /// صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? branchId { get; set; }
    }
}