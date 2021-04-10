using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// جدول کمکی فاکتور حقوقی یا شرکتی
    /// </summary>
    public class CompanyInvoiceViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public int? branchId { get; set; }
        /// <summary>
        /// تخفیفات و کسورات
        /// </summary>
        public long reduction { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string number { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// نام شخص حقیقی
        /// </summary>
        public string sellerName { get; set; }
        /// <summary>
        /// شماره اقتصادی
        /// </summary>
        public string sellerEconomicalNumber { get; set; }
        /// <summary>
        /// شناسه ملی
        /// </summary>
        public string sellerNationalId { get; set; }
        /// <summary>
        /// کد پستی
        /// </summary>
        public string sellerPostalCode { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string sellerAddress { get; set; }
        /// <summary>
        /// تلفن
        /// </summary>
        public string sellerPhone { get; set; }
        /// <summary>
        /// نام شخص حقیقی
        /// </summary>
        public string buyerName { get; set; }
        /// <summary>
        /// شماره اقتصادی
        /// </summary>
        public string buyerEconomicalNumber { get; set; }
        /// <summary>
        /// شناسه ملی
        /// </summary>
        public string buyerNationalId { get; set; }
        /// <summary>
        /// کد پستی
        /// </summary>
        public string buyerPostalCode { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string buyerAddress { get; set; }
        /// <summary>
        /// تلفن
        /// </summary>
        public string buyerPhone { get; set; }
        /// <summary>
        /// فایل اصلی
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// فایل اصلی
        /// </summary>
        public string attachmentFile { get; set; }
        /// <summary>
        /// ویو مدل جزئیات فاکتور حقوقی یا شرکتی
        /// </summary>
        public List<CompanyInvoiceDetailViewModel> companyInvoiceDetailViewModel { get; set; }
    }
    /// <summary>
    /// جزئیات فاکتور حقوقی یا شرکتی
    /// </summary>
    public class CompanyInvoiceDetailViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? detailId { get; set; }
        /// <summary>
        /// ردیف جدوف فاکتور حقوقی یا شرکتی
        /// </summary>
        public int companyInvoiceId { get; set; }
        /// <summary>
        /// کد شناسایی کالا 
        /// </summary>
        public string identificationCode { get; set; }
        /// <summary>
        /// شرح کالا
        /// </summary>
        public string descriptionProduct { get; set; }
        /// <summary>
        /// عیار
        /// </summary>
        public int carat { get; set; }
        /// <summary>
        /// سوت
        /// </summary>
        public int whistle { get; set; }
        /// <summary>
        /// گرم
        /// </summary>
        public int gram { get; set; }
        /// <summary>
        /// وزن سنگ
        /// </summary>
        public int stoneWeight { get; set; }
        /// <summary>
        /// قیمت سنگ
        /// </summary>
        public long stonePrice { get; set; }
        /// <summary>
        /// اجرت ساخت
        /// </summary>
        public long wage { get; set; }
        /// <summary>
        /// قیمت طلا
        /// </summary>
        public long goldPrice { get; set; }
    }
    public class CompanyInvoiceSearchViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// نام خریدار
        /// </summary>
        public string buyerName { get; set; }
        /// <summary>
        /// تاریخ شمسی
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// شرح کالا
        /// </summary>
        public string descriptionProduct { get; set; }
        /// <summary>
        /// کدشناسایی کالا
        /// </summary>
        public string identificationCode { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// شماره اقتصادی
        /// </summary>
        public string buyerEconomicalNumber { get; set; }
        /// <summary>
        /// اجرت
        /// </summary>
        public long wage { get; set; }
        /// <summary>
        /// صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// نمایش اجرت به صورت سه رقم سه رقم
        /// </summary>
        public string wageSeparator { get; set; }
        /// <summary>
        /// فایل اصلی
        /// </summary>
        public string attachmentFile { get; set; }

    }

}