using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// گزارش موجودی روزانه
    /// </summary>
    public class InventoryReportMembersViewModel
    {
        /// <summary>
        /// ردیف 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// ردیف دسته بندی
        /// </summary>
        public int categoryReportMemberId { get; set; }
        /// <summary>
        /// ترتیب 
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// عنوان 
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// لیست جزئیات اعم از مقدار و وزن
        /// </summary>
        public List<DailyInventoryDetailViewModel> dailyInventoryDetailViewModelList { get; set; }

    }
    /// <summary>
    /// ثبت جزئیات موجودی اعم از وزن و مقدار
    /// </summary>
    public class DailyInventoryDetailViewModel
    {
        public int? id { get; set; }
        /// <summary>
        /// ردیف گزارش موجودی روزانه
        /// </summary>
        public int categoryInventoryReportMemberId { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public float weight { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// تاریخ شمسی
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime? date { get; set; }

    }
    public class InventoryReportMembersSearchViewModel
    {
        /// <summary>
        /// صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
    }
    public class InfoInventoryViewModel
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// لیست جزئیات اعم از مقدار و وزن
        /// </summary>
        public List<DailyInventoryDetailViewModel> dailyInventoryDetailViewModelList { get; set; }
    }
}
