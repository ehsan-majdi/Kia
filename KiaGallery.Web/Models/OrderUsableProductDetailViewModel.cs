using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class OrderUsableProductViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف محصول مصرفی
        /// </summary>
        public int usableProductId { get; set; }
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int orderUsableProductId { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int branchId { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// تصویر محصول
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// دسته بندی محصول
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// تعداد سفارش
        /// </summary>
        public int? productCount { get; set; }

    }
    /// <summary>
    /// جزئیات سفارش محصول مصرفی
    /// </summary>
    public class OrderUsableProductDetailViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int orderUsableProductId { get; set; }
        /// <summary>
        /// ردیف محصول مصرفی
        /// </summary>
        public int usableProductId { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
    }
    /// <summary>
    /// نمایش تعداد سبد سفارشات و نمایش سفارشات انتخابی برای ثبت سفارش
    /// </summary>
    public class ShowOrderUsableProductViewModel
    {
        public int? sumCount { get; set; }
        public int? registered { get; set; }
        public int? inPreparation { get; set; }
        public string bgColor { get; set; }
        public int? cancel { get; set; }
        public int? sent { get; set; }
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// ردیف محصول مصرفی
        /// </summary>
        public int usableProductId { get; set; }
        /// <summary>
        /// ردیف سفارش چاپخانه
        /// </summary>
        public int printingHouseOrderId { get; set; }
        public int printingHouseId { get; set; }
        /// <summary>
        /// ایا به چاپخانه ارسال شده است یا خیر؟
        /// </summary>
        public bool printingHouserOrder { get; set; }
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int orderUsableProductId { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int branchId { get; set; }
        /// <summary>
        /// نام مخفف شعبه
        /// </summary>
        public string alias { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// تصویر محصول
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// دسته بندی محصول
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// تعداد سفارش
        /// </summary>
        public int? productCount { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime createDate { get; set; }
        /// <summary>
        /// تاریخ شمسی
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public string createUser { get; set; }
        /// <summary>
        /// کاربر شعبه ایجاد کننده
        /// </summary>
        public string createBranch { get; set; }
        /// <summary>
        /// وضعیت سفارش محصولات مصرفی 
        /// </summary>
        public OrderUsableProductStatus orderUsableProductStatus { get; set; }
        /// <summary>
        /// عنوان وضعیت سفارش محصول مصرفی
        /// </summary>
        public string orderUsableProductStatusTitle { get; set; }
        public string fileList { get; set; }
        /// <summary>
        /// موجودی محصولات مصرفی دفتر مرکزی
        /// </summary>
        public int? officeInventory { get; set; }
        /// <summary>
        /// باقی مانده
        /// </summary>
        public int? remain { get; set; }
        /// <summary>
        /// کد محصول
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// واحد
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// ردیف دسته بندی فرزند
        /// </summary>
        public int? childId { get; set; }
        /// <summary>
        /// ردیف دسته بندی پدر
        /// </summary>
        public int? categoryId { get; set; }
        /// <summary>
        /// توضیحات اضافی
        /// </summary>
        public string specification { get; set; }
        /// <summary>
        /// موجودی چاپخانه
        /// </summary>
        public int? printingHouseInventory { get; set; }
        /// <summary>
        /// باقی مانده نهایی
        /// </summary>
        public int? remainFinal { get; set; }
        /// <summary>
        /// تعداد آماده تحویل
        /// </summary>
        public int? readyForDelivery { get; set; } 
        /// <summary>
        /// تعداد آماده تحویل نهایی
        /// </summary>
        public int? confirmReadyForDelivery { get; set; }
        /// <summary>
        /// نتایج
        /// </summary>
        public int? result { get; set; }
        /// <summary>
        /// تحویل داده شده
        /// </summary>
        public int? delivered { get; set; }
        /// <summary>
        /// تحویل داده شده نهایی
        /// </summary>
        public int? confirmDelivered { get; set; }
        /// <summary>
        /// نوع شعبه
        /// </summary>
        public BranchType branchType { get; set; }
        public int number { get; set; }



    }
    /// <summary>
    /// ساختن سفارش از سبد خرید 
    /// </summary>
    public class MakeOrderUsableProductViewModel
    {

        /// <summary>
        /// لیست id سبد خرید
        /// </summary>
        public List<int> id { get; set; }
        /// <summary>
        /// لیست توضیحات اضافه شده در سبد خرید
        /// </summary>
        public List<string> specification { get; set; }
    }

    /// <summary>
    /// وضعیت سفارشات محصول
    /// </summary>
    public class OrderUsableChangeStatusViewModel
    {
        /// <summary>
        /// لیست ردیف
        /// </summary>
        public List<int> id { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public OrderUsableProductStatus status { get; set; }
    }
    public class PrintingHouseOrderViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public List<int> idList { get; set; }
        /// <summary>
        /// موجودی محصولات مصرفی دفتر مرکزی
        /// </summary>
        public List<int> officeInventory { get; set; }
        /// <summary>
        /// باقی مانده
        /// </summary>
        public List<int> remain { get; set; }
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int orderId { get; set; }
        /// <summary>
        /// ردیف سفارش محصولی
        /// </summary>
        public int orderUsableProductId { get; set; }

    }

    /// <summary>
    /// جدول کمکی برای نمایش مقدار در modal صفحه
    /// </summary>
    public class UsableProductCartViewModel
    {
        public int id { get; set; }
        public int count { get; set; }
        public string description { get; set; }
        public int branchId { get; set; }
    }

    public class SpecificatioViewModel
    {
        /// <summary>
        /// ردیف محصول مصرفی
        /// </summary>
        public int usableProductId { get; set; }
        public int? id { get; set; }
        public string specification { get; set; }
    }

    public class ConfirmOrderViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public List<int> idList { get; set; }
        /// <summary>
        /// موجودی محصولات مصرفی دفتر مرکزی
        /// </summary>
        public List<int> printingHouseInventory { get; set; }
        /// <summary>
        /// باقی مانده
        /// </summary>
        public List<int> remainFinal { get; set; }
        /// <summary>
        /// آماده تحویل
        /// </summary>
        public List<int> readyForDelivery { get; set; }
        /// <summary>
        /// تحویل داده شده
        /// </summary>
        public List<int> delivered { get; set; } 
        /// <summary>
        /// تحویل داده شده نهایی
        /// </summary>
        public List<int> confirmDelivered { get; set; }
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int orderId { get; set; }
        /// <summary>
        /// ردیف سفارش محصولی
        /// </summary>
        public int orderUsableProductId { get; set; }
    }
}