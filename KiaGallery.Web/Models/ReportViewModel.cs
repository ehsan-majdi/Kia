using KiaGallery.Model;
using System.Collections.Generic;

namespace KiaGallery.Web.Models
{
    public class ProductReportPrintViewModel
    {
        public byte[] Image { get; set; }
        public string BookCode { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public float? Weight { get; set; }
        public GoldType GoldType { get; set; }
    }

    public class ProductReportViewModel
    {
        /// <summary>
        /// لیست ردیف محصولات
        /// </summary>
        public List<int> id { get; set; }
        public List<int> idList { get; set; }
        public List<int> listRow { get; set; }
        public int type { get; set; }
        public int workshop { get; set; }
        public ProductType productType { get; set; }
    }

   /// <summary>
   /// گزارش مشخصات پرسنل
   /// </summary>
    public class PersonReportViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int branchId { get; set; }
        /// <summary>
        /// بیمه
        /// </summary>
        public bool insurance { get; set; }
        /// <summary>
        /// لیست افراد فعال
        /// </summary>
        public bool active { get; set; }
    }

    /// <summary>
    /// چاپ گزارش مشخصات پرسنل
    /// </summary>
    public class PersonReportPrintViewModel
    {
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        public int personNumber { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string mobileNumber { get; set; }

    }

}