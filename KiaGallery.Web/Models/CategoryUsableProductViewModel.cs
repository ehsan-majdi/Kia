using KiaGallery.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// جدول کمکی دسته بندی محصول سفارشی 
    /// </summary>
    public class CategoryUsableProductViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف والد
        /// </summary>
        public int? parentId { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
        public int? orderChild { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// لیست پرنت
        /// </summary>
        public virtual List<SearchCategoryUsableProductSearchViewModel> children { get; set; }
    }
    public class SearchCategoryUsableProductSearchViewModel
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
        /// ردیف
        /// </summary>
        public int id { get; set; }
        public int? order { get; set; }
        /// <summary>
        /// عنوان فرزند
        /// </summary>
        public string childTitle { get; set; }
        /// <summary>
        /// ردیف والد
        /// </summary>
        public int? parentId { get; set; }
    }
}