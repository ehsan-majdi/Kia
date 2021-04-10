using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// کلاس کمکی دسته بندی گزارش موجودی طلا مثل کارگاه که کارگاه شامل افرادی می باشند که طلا نزد آنها نیز موجود می باشد
    /// </summary>
    public class CategoryInventoryReportViewModel
    {
        /// <summary>
        /// ردیف دسته بندی
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// عنوان دسته بندی
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool active { get; set; }
    }
}