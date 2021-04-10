using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// جزئیات سفارش مشتری 
    /// </summary>
    public class InternalDetailViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// نام مشتری
        /// </summary>
        public string cutomerName { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// شماره ثابت
        /// </summary>
        public string telephone { get; set; }
        /// <summary>
        /// تاریخ میلادی
        /// </summary>
        public DateTime? date { get; set; }
        /// <summary>
        /// تاریخ شمسی
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// بیعانه
        /// </summary>
        public int deposit { get; set; }
        /// <summary>
        /// بیعانه سه رقم شده
        /// </summary>
        public string depositSeparator { get; set; }
        /// <summary>
        /// نوع سفارش
        /// </summary>
        public OrderType orderType { get; set; }
        /// <summary>
        /// نوع سفارش
        /// </summary>
        public OrderTypeForm orderTypeForm { get; set; }
        /// <summary>
        /// نوع محصول
        /// </summary>
        public ProductType productType { get; set; }
        /// <summary>
        ///   جدید نوع محصول
        /// </summary>
        public ProductType? newProductType { get; set; }
        /// <summary>
        /// نوع طلا
        /// </summary>
        public GoldType goldType { get; set; }
        /// <summary>
        /// رنگ طلا
        /// </summary>
        public ProductColor productColor { get; set; }
        /// <summary>
        /// عنوان محصول
        /// </summary>
        public string prdocutTitle { get; set; }
        /// <summary>
        /// کدمحصول
        /// </summary>
        public string productCode { get; set; }
        /// <summary>
        /// کد سایت
        /// </summary>
        public string siteCode { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// کدپیگیری
        /// </summary>
        public string trackCode { get; set; }
        /// <summary>
        /// کدکارگاه
        /// </summary>
        public string bookCode { get; set; }
        /// <summary>
        /// تصویر بارکد
        /// </summary>
        public string barcodeImage { get; set; }
        /// <summary>
        /// سایز
        /// </summary>
        public string size { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// تعداد دور چرم
        /// </summary>
        public int? leatherLoop { get; set; }
        /// <summary>
        /// کاربر ثبت کننده
        /// </summary>
        public string person { get; set; }
        /// <summary>
        /// لیست سنگ ها
        /// </summary>
        public List<string> stoneList { get; set; }
        /// <summary>
        /// لیست چرم ها
        /// </summary>
        public List<string> leatherList { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int? count { get; set; }
        /// <summary>
        /// بارکد
        /// </summary>
        public string barcode { get; set; }
        /// <summary>
        /// عنوان فارسی دسته بندی محصول
        /// </summary>
        public string prdocutTypeTitle { get; set; }
        /// <summary>
        /// عنوان فارسی دسته بندی نوع سفارش
        /// </summary>
        public string orderTypeFormTitle { get; set; }
        /// <summary>
        /// عنوان فارسی رنگ طلای محصول
        /// </summary>
        public string prdocutColorTitle { get; set; }
        public string newStone { get; set; }
        public string newProductTitle { get; set; }
        public string newProductTypeTitle { get; set; }
        public bool? goldOwnership { get; set; }
        public string newLeather { get; set; }
        public string image { get; set; }
        public string newSize { get; set; }
        public string newLeatherLoop { get; set; }
        public ProductColor? newProductColor { get; set; }
        public GoldType? newGoldType { get; set; }
        public string newDescription { get; set; }
        public int? newCount { get; set; }
        public string branchColor { get; set; }

    }
}