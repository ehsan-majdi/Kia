using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// چاپخانه
    /// </summary>
    public class PrintingHouseViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ترتیب چاپخانه
        /// </summary>
        public string Rate { get; set; }
        /// <summary>
        /// کد چاپخانه
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// نام چاپخانه
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// مدیریت چاپخانه
        /// </summary>
        public string management { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// تلفن ثابت
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool active { get; set; }
    }

    public class GetFinalOrderPrintingHouseViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// موجودی محصولات مصرفی دفتر مرکزی
        /// </summary>
        public List<int> officeInventory { get; set; }
        /// <summary>
        /// باقی مانده
        /// </summary>
        public long? remain { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// تعداد سفارش ثبت شده و قابل مشاهده برای کاربران چاپخانه
        /// </summary>
        public int countPrintingHouse { get; set; }
        /// <summary>
        /// نام شعبه سفارش دهنده
        /// </summary>
        public string branch { get; set; }
        /// <summary>
        /// مخفف نام شعبه
        /// </summary>
        public string alias { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime createDate { get; set; }
        /// <summary>
        /// تاریخ شمسی
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// ردیف محصول مصرفی
        /// </summary>
        public int usableProductId { get; set; }
        /// <summary>
        /// ردیف سفارش محصول مصرفی
        /// </summary>
        public int orderUsableProductId { get; set; }
        /// <summary>
        /// لیست جزئیات محصول مصرفی
        /// </summary>
        public List<OrderUsableProductDetailList> orderDetailList  { get; set; }
        /// <summary>
        /// عنوان وضعیت سفارش محصول مصرفی
        /// </summary>
        public string orderUsableProductStatusTitle { get; set; }
        /// <summary>
        /// عنوان وضعیت کلی سفارش
        /// </summary>
        public string printingHouseOrderStatusTitle { get; set; }
        /// <summary>
        /// وضعیت سفارش محصولات مصرفی 
        /// </summary>
        public OrderUsableProductStatus orderUsableProductStatus { get; set; }
        /// <summary>
        /// وضعیت کلی سفارش محصولات مصرفی
        /// </summary>
        public PrintingHouseOrderStatus printingHouseOrderStatus { get; set; }
        public int? sumCount { get; set; }
        public int? registered { get; set; }
        public int? inPreparation { get; set; }
        public string bgColor { get; set; }
        public int? cancel { get; set; }
        public int? sent { get; set; }

    }
    public class PaginationFinalOrderViewModel
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
        /// برگرداندن مقدار جستجو شده از طریق موارد دریافتی از سوی کاربر
        /// </summary>
        public string term { get; set; }
    }

    public class OrderUsableProductDetailList
    {
        public int usableProductId { get; set; }
        public long remain { get; set; }
    }
}