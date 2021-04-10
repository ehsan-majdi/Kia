using KiaGallery.Model;
using KiaGallery.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// محصول مصرفی
    /// </summary>
    public class UsableProductViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف دسته بندی
        /// </summary>
        public int categoryUsableProductId { get; set; }
        /// <summary>
        /// ردیف زیردسته بندی
        /// </summary>
        public int categoryChild { get; set; }
        /// <summary>
        ///ردیف زیر دسته بندی 
        /// </summary>
        public List<int> categoryChildId { get; set; }
        /// <summary>
        /// ردیف چاپخانه
        /// </summary>
        public int printingHouseId { get; set; }
        /// <summary>
        /// آرایه لیست idList
        /// </summary>
        //public List<int> categoryId { get; set; }   
        public int? categoryId { get; set; }
        public string title { get; set; }
        public int count { get; set; }
        public int orderId { get; set; }
        public string firstFileName { get; set; }
        public string secondFileName { get; set; }
        public string parentTitle { get; set; }
        public string childTitle { get; set; }
        /// <summary>
        /// نام محصول
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// واحد
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// موجودی
        /// </summary>
        public bool available { get; set; }
        /// <summary>
        /// کد محصول
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// تصویر
        /// </summary>
        public string image { get; set; }
        public string category { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public UsableProductStatus usableProductStatus { get; set; }
        public string usableProductStatusTitle { get; set; }
        /// <summary>
        /// برگرداندن مقدار جستجو شده از طریق موارد دریافتی از سوی کاربر
        /// </summary>
        public string term { get; set; }
        /// <summary>
        ///  سفارش محصول مصرفی
        /// </summary>
        public virtual List<OrderUsableProduct> orderUsableProductList { get; set; }
        /// <summary>
        /// جزئیات سفارش محصول مصرفی
        /// </summary>
        public virtual List<OrderUsableProductDetail> orderUsableProductDetailList { get; set; }
        /// <summary>
        /// فایلهای محصولات چاپخانه
        /// </summary>
        public virtual List<UsableProductSearchViewModel> fileList { get; set; }
    }
    /// <summary>
    /// جدول کمکی فایل های محصولات چاپخانه
    /// </summary>
    public class UsableProductFileViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// فایل
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// فاصله
        /// </summary>
        public int paddingImg { get; set; }
        /// <summary>
        /// ردیف محصول چاپخانه
        /// </summary>
        public int usableProductId { get; set; }
    }
    /// <summary>
    /// لیست محصولات مصرفی
    /// </summary>
    public class UsableProductSearchViewModel
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
        /// ردیف فایل
        /// </summary>
        public string fileId { get; set; }
        /// <summary>
        /// نام فایل محصول مصرفی
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// برگرداندن مقدار جستجو شده از طریق موارد دریافتی از سوی کاربر
        /// </summary>
        public string term { get; set; }
        public bool printigHouseStatus { get; set; }



    }
    
}