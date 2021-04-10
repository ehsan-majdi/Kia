using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context.InternalOrder
{
    /// <summary>
    /// جزئیات سفارشات مشتری
    /// </summary>
    public class InternalOrderDetail
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public InternalOrderDetail()
        {
            InternalOrderDetailStonesList = new List<InternalOrderDetailStone>();
            InternalOrderDetailLeathersList = new List<InternalOrderDetailLeather>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف سفارش
        /// </summary>
        public int InternalOrderId { get; set; }
        /// <summary>
        /// مالکیت طلا(طلای ویترین یا طلای مشتری)
        /// </summary>
        public bool? GoldOwnership { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// کد کارگاه
        /// </summary>
        public string BookCode { get; set; }
        /// <summary>
        /// کد سایت
        /// </summary>
        public string SiteCode { get; set; }
        /// <summary>
        /// ردیف محصول
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// تعداد دور چرم
        /// </summary>
        public int? LeatherLoop { get; set; }
        /// <summary>
        /// کدپیگیری
        /// </summary>
        public string TrackCode { get; set; }
        /// <summary>
        /// تاریخ بارکد
        /// </summary>
        public DateTime? BarcodeDate { get; set; }
        /// <summary>
        /// بارکد
        /// </summary>
        [MaxLength(20)]
        public string Barcode { get; set; }
        /// <summary>
        /// سایز
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// نوع طلا
        /// </summary>
        public GoldType GoldType { get; set; }
        /// <summary>
        /// رنگ طلا
        /// </summary>
        public ProductColor ProductColor { get; set; }
        /// <summary>
        ///  نوع محصول
        /// </summary>
        public ProductType ProductType { get; set; }
        /// <summary>
        ///  نوع محصول
        /// </summary>
        public ProductType? NewProductType { get; set; }
        /// <summary>
        /// نوع سفارش
        /// </summary>
        public OrderType OrderType { get; set; }
        /// <summary>
        /// نام محصول
        /// </summary>
        [MaxLength(150)]
        public string Title { get; set; }
        /// <summary>
        /// شرح
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// عنوان سنگ
        /// </summary>
        public string Stone { get; set; }
        /// <summary>
        /// عنوان چرم
        /// </summary>
        public string Leather { get; set; }
        /// <summary>
        /// عنوان سنگ برای محصول سفارشی جدید
        /// </summary>
        public string NewStone { get; set; }
        /// <summary>
        /// عنوان چرم برای محصول سفارشی جدید
        /// </summary>
        public string NewLeather { get; set; }
        /// <summary>
        /// عنوان سایز برای محصول سفارشی جدید
        /// </summary>
        public string NewSize { get; set; }
        /// <summary>
        /// عنوان تعداد دور برای محصول سفارشی جدید
        /// </summary>
        public string NewLeatherLoop { get; set; }
        /// <summary>
        ///توضیحات برای محصول سفارشی جدید
        /// </summary>
        public string NewDescription { get; set; }
        /// <summary>
        /// عنوان محصول جدید
        /// </summary>
        public string NewProductTitle { get; set; }
        /// <summary>
        /// مقدار محصول جدید
        /// </summary>
        public int? NewCount { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// رنگ طلا
        /// </summary>
        public ProductColor? NewProductColor { get; set; }
        /// <summary>
        /// نوع طلا
        /// </summary>
        public GoldType? NewGoldType { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// سفارشات داخلی
        /// </summary>
        public virtual InternalOrder InternalOrder { get; set; }
        /// <summary>
        /// محصول
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// لیست سنگ ها در سفارشات
        /// </summary>
        public virtual List<InternalOrderDetailStone> InternalOrderDetailStonesList { get; set; }
        /// <summary>
        /// لیست چرم ها در سفارشات
        /// </summary>
        public virtual List<InternalOrderDetailLeather> InternalOrderDetailLeathersList { get; set; }
    }
}
