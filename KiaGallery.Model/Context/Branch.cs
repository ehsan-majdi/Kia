using KiaGallery.Model.Context.DailyReportFinancial;
using KiaGallery.Model.Context.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// شعبه
    /// </summary>
    public class Branch
    {
        /// <summary>
        /// سارنده
        /// </summary>
        public Branch()
        {
            CustomerFactorList = new List<CustomerFactor>();

            UserList = new List<User>();
            DailyReportList = new List<DailyReport>();
            CartList = new List<Cart>();
            UsableProductCartList = new List<UsableProductCart>();
            OrderList = new List<Order.Order>();
            OrderUsableProductList = new List<OrderUsableProduct>();
            MessageList = new List<Message>();
            FavouritesProductList = new List<FavouritesProduct>();
            BankList = new List<Bank>();
            BranchCalendarList = new List<BranchCalendar>();
            BranchNoteList = new List<BranchNote>();
            GiftShoppingList = new List<Gift.Gift>();
            GiftReceiverList = new List<Gift.Gift>();
            BranchesPaymentsList = new List<BranchesPayments.BranchesPayments>();
            UserDataList = new List<BranchesPayments.UserData>();
            PersonList = new List<Salary.Person>();
            InquiryProductList = new List<InquiryProduct>();
            InvoiceList = new List<CrmInvoice>();
            InternalOrderList = new List<InternalOrder.InternalOrder>();
            BranchGoldList = new List<BranchGold>();
            CompanyInvoiceList = new List<CompanyInvoice>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شهر
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// نوع شعب
        /// </summary>
        public BranchType BranchType { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        [Column("OrderNo")]
        public int Order { get; set; }
        /// <summary>
        /// ساعت کاری
        /// </summary>
        public int WorkingHour { get; set; }
        /// <summary>
        /// نام مالک شعبه
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// کد ملی مالک شعبه
        /// </summary>
        public string OwnerNationalityCode { get; set; }
        /// <summary>
        /// شماره شناسنامه مالک 
        /// </summary>
        public string OwnerNationalityNo { get; set; }
        /// <summary>
        /// نام پدر مالک
        /// </summary>
        public string OwnerFatherName { get; set; }
        /// <summary>
        /// نام مستعار
        /// </summary>
        [MaxLength(5)]
        public string Alias { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// نام انگلیسی
        /// </summary>
        [MaxLength(100)]
        public string EnglishName { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        [MaxLength(1000)]
        public string Address { get; set; }
        /// <summary>
        /// آدرس انگلیسی
        /// </summary>
        [MaxLength(1000)]
        public string EnglishAddress { get; set; }
        /// <summary>
        /// رنگ
        /// </summary>
        [MaxLength(7)]
        public string Color { get; set; }
        /// <summary>
        /// تلفن
        /// </summary>
        [MaxLength(100)]
        public string Phone { get; set; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public int? CreditCard { get; set; }
        /// <summary>
        /// عرض جغرافیایی
        /// </summary>
        [MaxLength(38)]
        public string Latitude { get; set; }
        /// <summary>
        /// طول جغرافیایی
        /// </summary>
        [MaxLength(38)]
        public string Longitude { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// بدهی حساب طلا
        /// </summary>
        public float GoldDebt { get; set; }
        /// <summary>
        /// بدهی حساب ریال
        /// </summary>
        public long RialDebt { get; set; }
        /// <summary>
        /// اعتبار حساب طلا
        /// </summary>
        public float GoldCredit { get; set; }
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
        /// شهر
        /// </summary>
        public virtual Location City { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }

        /// <summary>
        /// لیست کاربران شعبه
        /// </summary>
        public virtual List<User> UserList { get; set; }
        /// <summary>
        /// لیست گزارش های روزانه شعبه
        /// </summary>
        public virtual List<DailyReport> DailyReportList { get; set; }
        /// <summary>
        /// سبد خرید شعبه
        /// </summary>
        public virtual List<Cart> CartList { get; set; }
        /// <summary>
        /// لیست سفارشات شعبه
        /// </summary>
        public virtual List<Order.Order> OrderList { get; set; }
        /// <summary>
        /// لیست سفارشات شعبه
        /// </summary>
        public virtual List<OrderUsableProduct> OrderUsableProductList { get; set; }
        /// <summary>
        /// لیست پیام های شعبه
        /// </summary>
        public virtual List<Message> MessageList { get; set; }
        /// <summary>
        /// لیست محصولات مورد علاقه شعبه
        /// </summary>
        public virtual List<FavouritesProduct> FavouritesProductList { get; set; }
        /// <summary>
        /// لیست حساب های بانکی شعبه
        /// </summary>
        public virtual List<Bank> BankList { get; set; }
        /// <summary>
        /// لیست حساب های بانکی شعبه
        /// </summary>
        public virtual List<BranchCalendar> BranchCalendarList { get; set; }

        /// <summary>
        /// لیست حساب های بانکی شعبه
        /// </summary>
        public virtual List<BranchNote> BranchNoteList { get; set; }
        /// <summary>
        /// لیست گیفت خریداری شده
        /// </summary>
        public virtual List<Gift.Gift> GiftShoppingList { get; set; }
        /// <summary>
        /// لیست گیفت دریافت شده از مشتری
        /// </summary>
        public virtual List<Gift.Gift> GiftReceiverList { get; set; }
        /// <summary>
        /// لیست پرداخت شعب
        /// </summary>
        public virtual List<BranchesPayments.BranchesPayments> BranchesPaymentsList { get; set; }
        /// <summary>
        /// لیست کاربران بات پرداخت شعب
        /// </summary>
        public virtual List<BranchesPayments.UserData> UserDataList { get; set; }
        /// <summary>
        /// لیست پرسنل
        /// </summary>
        public virtual List<Salary.Person> PersonList { get; set; }
        /// <summary>
        /// لیست استعلام محصول
        /// </summary>
        public virtual List<InquiryProduct> InquiryProductList { get; set; }
        /// <summary>
        /// لیست فاکتور های فروخته شده به مشتری
        /// </summary>
        public virtual List<CrmInvoice> InvoiceList { get; set; }
        /// <summary>
        /// لیست سفارشات داخلی شعب
        /// </summary>
        public virtual List<InternalOrder.InternalOrder> InternalOrderList { get; set; }
        /// <summary>
        /// خرید و فروش طلا
        /// </summary>
        public virtual List<BranchGold> BranchGoldList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده مشتریان وفادار
        /// </summary>
        public virtual List<CustomerFactor> CustomerFactorList { get; set; }
        /// <summary>
        /// لیست سبد خرید
        /// </summary>
        public virtual List<UsableProductCart> UsableProductCartList { get; set; }
        /// <summary>
        /// لیست فاکتورهای حقوقی یا شرکتی
        /// </summary>
        public virtual List<CompanyInvoice> CompanyInvoiceList { get; set; }
    }
}
