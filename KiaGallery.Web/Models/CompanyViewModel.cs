using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class CompanyViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// نام مستعار
        /// </summary>
        public string alias { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string englishName { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// آدرس انگلیسی
        /// </summary>
        public string englishAddress { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool active { get; set; }
    }
    public class CompanySearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
    }
}