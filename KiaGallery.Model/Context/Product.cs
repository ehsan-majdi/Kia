using KiaGallery.Model.Context.Bot;
using KiaGallery.Model.Context.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// محصول
    /// </summary>
    public class Product
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public Product()
        {
            ProductFileList = new List<ProductFile>();
            ProductStoneList = new List<ProductStone>();
            ProductLeatherList = new List<ProductLeather>();
            CartList = new List<Cart>();
            OrderDetailList = new List<OrderDetail>();
            FavouritesProductList = new List<FavouritesProduct>();
            ProductOuterWerkList = new List<ProductOuterWerk>();
            SourceRelatedProduct = new List<RelatedProduct>();
            RelatedProduct = new List<RelatedProduct>();
            SourceSetProduct = new List<SetProduct>();
            SetProduct = new List<SetProduct>();
            BotOrderList = new List<BotOrder>();
            MessageList = new List<Message>();
            InquiryProductList = new List<InquiryProduct>();
            InternalOrderDetailList = new List<InternalOrder.InternalOrderDetail>();
            PhotographyManageList = new List<PhotographyManage>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// کد سایت
        /// </summary>
        [MaxLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// کد کتاب کد
        /// </summary>
        [MaxLength(50)]
        [Index("BookCode", IsUnique = true)]
        public string BookCode { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        [MaxLength(150)]
        public string Title { get; set; }
        /// <summary>
        /// نوع محصول
        /// </summary>
        public ProductType ProductType { get; set; }
        /// <summary>
        /// نوع انگشتر دو سایزی یا یک سایزی
        /// </summary>
        public bool? RingSizeType { get; set; }
        /// <summary>
        /// نوع طلا
        /// </summary>
        public GoldType GoldType { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
        public Sex Sex { get; set; }
        /// <summary>
        /// ردیف کارگاه
        /// </summary>
        public int WorkshopId { get; set; }
        /// <summary>
        /// ردیف کارگاه
        /// </summary>
        public int? WorkshopId2 { get; set; }
        /// <summary>
        /// ردیف تگ های کارگاه
        /// </summary>
        public int? WorkshopTagId { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public float? Weight { get; set; }
        /// <summary>
        /// وزن نقره
        /// </summary>
        public float? SilverWeight { get; set; }
        /// <summary>
        /// تعداد سنگ
        /// </summary>
        public int StoneCount { get; set; }
        /// <summary>
        /// قیمت سنگ
        /// </summary>
        public int StonePrice { get; set; }
        /// <summary>
        /// تعداد چرم
        /// </summary>
        public int LeatherCount { get; set; }
        /// <summary>
        /// قیمت چرم
        /// </summary>
        public int LeatherPrice { get; set; }
        /// <summary>
        /// ردیف سایز
        /// </summary>
        public int? SizeId { get; set; }
        /// <summary>
        /// ردیف مقدار سایز نرمال
        /// </summary>
        public int? NormalSizeValueId { get; set; }
        /// <summary>
        /// اجرت
        /// </summary>
        public int Wage { get; set; }
        /// <summary>
        /// دارای تعداد دور چرم
        /// </summary>
        public bool? CanLoop { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// تعداد روز های جهت نمایش علامت جدید
        /// </summary>
        public int New { get; set; }
        /// <summary>
        /// جایگاه خرج کار
        /// </summary>
        [MaxLength(20)]
        public string OuterWerkPlacement { get; set; }
        /// <summary>
        /// دسته خرج کار
        /// </summary>
        public OuterWerkCategory? OuterWerkCategory { get; set; }
        /// <summary>
        /// نمایش محصول برای فقط شعب تهران یا فقط شهرستان یا هررو
        /// </summary>
        //public ShowProduct? ShowProduct { get; set; }
        public bool SpecialProduct { get; set; }
        /// <summary>
        /// ردیف کالکشن محصول
        /// </summary>
        public int? ProductCollectionId { get; set; }
        /// <summary>
        /// غیر قابل سفارش برای کارگاه
        /// </summary>
        public bool? NotOrderable { get; set; }
        /// <summary>
        /// مخصوص ساخت کارگاه
        /// </summary>
        public bool? OnlyForWorkshop { get; set; }
        /// <summary>
        /// رنگ گلد
        /// </summary>
        public bool? GoldColor { get; set; }
        /// <summary>
        /// رنگ رزگلد
        /// </summary>
        public bool? RosegoldColor { get; set; }
        /// <summary>
        /// رنگ سفید
        /// </summary>
        public bool? WhiteColor { get; set; }
        /// <summary>
        /// رنگ ترکیبی
        /// </summary>
        public bool? MixedColor { get; set; }
        /// <summary>
        /// پشت گوشواره
        /// </summary>
        public bool? EarringBack { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? OrderableBranchType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? OrderableCoWorkerType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? OrderableSolicitorshipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? OrderableOtherType { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        [MaxLength(45)]
        public string Ip { get; set; }
        /// <summary>
        /// کارگاه
        /// </summary>
        public virtual Workshop Workshop { get; set; }
        /// <summary>
        /// کارگاه
        /// </summary>
        public virtual Workshop Workshop2 { get; set; }
        /// <summary>
        /// تگ کارگاه
        /// </summary>
        public virtual WorkshopTag WorkshopTag { get; set; }
        /// <summary>
        /// سایز
        /// </summary>
        public virtual Size Size { get; set; }
        /// <summary>
        /// مقدار سایز نرمال
        /// </summary>
        public virtual SizeValue NormalSizeValue { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// کالکشن محصول
        /// </summary>
        public virtual ProductCollection ProductCollection { get; set; }

        /// <summary>
        /// لیست فایل های محصول
        /// </summary>
        public virtual List<ProductFile> ProductFileList { get; set; }
        /// <summary>
        /// لیست سنگ های محصول
        /// </summary>
        public virtual List<ProductStone> ProductStoneList { get; set; }
        /// <summary>
        /// لیست چرم های محصول
        /// </summary>
        public virtual List<ProductLeather> ProductLeatherList { get; set; }
        /// <summary>
        /// لیست سبد خرید های حاوی محصول
        /// </summary>
        public virtual List<Cart> CartList { get; set; }
        /// <summary>
        /// لیست کالاهای موجود در سفارشات
        /// </summary>
        public virtual List<OrderDetail> OrderDetailList { get; set; }
        /// <summary>
        /// لیست محصولات مورد علاقه شعبه
        /// </summary>
        public virtual List<FavouritesProduct> FavouritesProductList { get; set; }
        /// <summary>
        /// منبع محصولات مرتبط
        /// </summary>
        public virtual List<RelatedProduct> SourceRelatedProduct { get; set; }
        /// <summary>
        /// محصولات مرتبط
        /// </summary>
        public virtual List<RelatedProduct> RelatedProduct { get; set; }
        /// <summary>
        /// منبع محصولات سست
        /// </summary>
        public virtual List<SetProduct> SourceSetProduct { get; set; }
        /// <summary>
        /// محصولات ست
        /// </summary>
        public virtual List<SetProduct> SetProduct { get; set; }
        /// <summary>
        /// لیست خرج کار محصول
        /// </summary>
        public virtual List<ProductOuterWerk> ProductOuterWerkList { get; set; }
        /// <summary>
        /// لیست سفارشتات ربات
        /// </summary>
        public virtual List<BotOrder> BotOrderList { get; set; }
        /// <summary>
        /// لیست پیام های شعبه
        /// </summary>
        public virtual List<Message> MessageList { get; set; }
        /// <summary>
        /// لیست استعلام محصول
        /// </summary>
        public virtual List<InquiryProduct> InquiryProductList { get; set; }
        /// <summary>
        /// لیست سفارشات شعب
        /// </summary>
        public virtual List<InternalOrder.InternalOrderDetail> InternalOrderDetailList { get; set; }
        /// <summary>
        /// لیست مدیریت عکس ها
        /// </summary>
        public virtual List<PhotographyManage> PhotographyManageList { get; set; }

    }
}
