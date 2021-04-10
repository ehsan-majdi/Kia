using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class SiteApiViewModel
    {

    }

    /// <summary>
    /// مدل همگام سازی سایز
    /// </summary>
    public class ApiSizeViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// عنوان سایز
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// مقدار پیشفرضی که برای مقدار این سایز در نظر گرفته شده است
        /// </summary>
        public string defaultValue { get; set; }
    }
    /// <summary>
    /// مدل همگام سازی مقدار های ذخیره شده برای هر سایز
    /// </summary>
    public class ApiSizeValueViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public int sizeId { get; set; }
        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public string value { get; set; }

    }

    /// <summary>
    /// مدل همگام سازی کارگاه با پورتال
    /// </summary>
    public class ApiWorkshopViewModel
    {
        /// <summary>
        /// ردیف پورتال
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// شناسه مستعار کارگاه
        /// </summary>
        public string alias { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// رنگ
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// قابلیت تایید خودکار سفارشات برای کارگاه
        /// </summary>
        public bool autoConfirm { get; set; }
           /// <summary>
        /// نمایش این کارگاه در سیستم اعلان خرید و فروش طلا
        /// </summary>
        public bool? goldTrade { get; set; }
        /// <summary>
        /// ردیف وضعیت کارگاه
        /// </summary>
        public int statusId { get; set; }
    }
    
    /// <summary>
    /// مدل محصولاتی که از سمت پرتال به سایت هدایت می شوند.
    /// </summary>
    public class ApiProductViewModel
    {
        /// <summary>
        /// ردیف محصول
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// کارگاه محصول
        /// </summary>
        public int workshopId { get; set; }
        /// <summary>
        /// کارگاه دوم محصول
        /// </summary>
        public int? workshopSecondaryId { get; set; }
        /// <summary>
        /// کد محصول
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// کد کتاب کد
        /// </summary>
        public string bookCode { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// نوع محصول
        /// </summary>
        public int productTypeId { get; set; }
        /// <summary>
        /// انگشتر با دو حلقه انگشتی است یا خیر
        /// </summary>
        public bool? ringSizeType { get; set; }
        /// <summary>
        /// نوع طلا
        /// </summary>
        public int goldTypeId { get; set; }
        /// <summary>
        /// جنسیت محصول
        /// </summary>
        public int productSexId { get; set; }
        /// <summary>
        /// وزن حدودی
        /// </summary>
        public decimal weight { get; set; }
        /// <summary>
        /// وزن نقره محصول
        /// </summary>
        public decimal? silverWeight { get; set; }
        /// <summary>
        /// وزن حدودی
        /// </summary>
        public float? weightFloat { get; set; }
        /// <summary>
        /// وزن نقره محصول
        /// </summary>
        public float? silverWeightFloat { get; set; }
        /// <summary>
        /// قیمت سنگ
        /// </summary>
        public decimal stonePrice { get; set; }
        /// <summary>
        /// قیمت چرم
        /// </summary>
        public decimal leatherPrice { get; set; }
        /// <summary>
        /// ردیف سایز محصول
        /// </summary>
        public int? sizeId { get; set; }
        /// <summary>
        /// سایز نرمال محصول
        /// </summary>
        public int? normalSizeValueId { get; set; }
        /// <summary>
        /// اجرت ساخت
        /// </summary>
        public decimal wage { get; set; }
        /// <summary>
        /// امکان دور زدن محصول به دور دست (مربوط به دستبند)
        /// </summary>
        public bool? canLoop { get; set; }
        /// <summary>
        /// تعداد روزهای نمایش به عنوان محصول جدید
        /// </summary>
        public int newDays { get; set; }
        /// <summary>
        /// نوع خرج کار
        /// </summary>
        public int? outerWerkTypeId { get; set; }
        /// <summary>
        /// جایگاه خرج کار
        /// </summary>
        public string outerWerkPlacement { get; set; }
        /// <summary>
        /// محصول خاص می باشد؟
        /// </summary>
        public bool specialProduct { get; set; }
        /// <summary>
        /// سفارش پذیر بودن محصول
        /// </summary>
        public bool? orderable { get; set; }
        /// <summary>
        /// پشت گوشواره
        /// </summary>
        public bool? earringBack { get; set; }
        /// <summary>
        /// سفارش پذیر بودن محصول برای شعبه
        /// </summary>
        public bool? orderableForBranch { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public int statusId { get; set; }
        /// <summary>
        /// سنگ های محصول
        /// </summary>
        public List<ApiProductStoneViewModel> stones { get; set; }
        /// <summary>
        /// چرم های محصول
        /// </summary>
        public List<ApiProductLeatherViewModel> leathers { get; set; }
        /// <summary>
        /// تصاویر محصول
        /// </summary>
        public List<ApiProductImageViewModel> images { get; set; }
    }

    /// <summary>
    /// مدل همگام سازی سنگ های متصل به محصول
    /// </summary>
    public class ApiProductStoneViewModel
    {
        /// <summary>
        /// ردیف سنگ
        /// </summary>
        public int stoneId { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
    }

    /// <summary>
    /// مدل همگام سازی چرم های متصل به محصول
    /// </summary>
    public class ApiProductLeatherViewModel
    {
        /// <summary>
        /// ردیف چرم
        /// </summary>
        public int leatherId { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
    }

    /// <summary>
    /// مدل همگام سازی تصاویر محصول
    /// </summary>
    public class ApiProductImageViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// نوع تصویر
        /// </summary>
        public int productImageTypeId { get; set; }
        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// سایز عکس
        /// </summary>
        public int paddingImg { get; set; }
        /// <summary>
        /// آدرس فایل تصویر
        /// </summary>
        public string url { get; set; }
    
    }
}