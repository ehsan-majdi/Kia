using KiaGallery.Model;
using System;
using System.Collections.Generic;

namespace KiaGallery.Web.Models
{
    public class GiftViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// وضعیت گیف
        /// </summary>
        public GiftType giftType { get; set; }
        /// <summary>
        /// وضعیت گیف
        /// </summary>
        public GiftStatus giftStatus { get; set; }
        /// <summary>
        /// ردیف شعبه خریدار گیفت
        /// </summary>
        public int? branchIdShopping { get; set; }
        /// <summary>
        /// ردیف شرکت خریدار گیفت
        /// </summary>
        public int? companyIdShopping { get; set; }
        /// <summary>
        /// ردیف شعبه دریافت کننده گیفت از مشتری
        /// </summary>
        public int? branchIdReceiverCustomer { get; set; }
        /// <summary>
        /// مبلغ
        /// </summary>
        public long value { get; set; }
        /// <summary>
        /// مدت زمان انقضا
        /// </summary>
        public long expirationTime { get; set; }
        /// <summary>
        /// تاریخ انقضا به شمسی
        /// </summary>
        public string expiryDateToSolar { get; set; }
        /// <summary>
        /// نام، نام خانوادگی مشتری خریدکننده
        /// </summary>
        public string buyerCustomerName { get; set; }
        /// <summary>
        /// شماره تلفن مشتری خرید کننده
        /// </summary>
        public string buyerCustomerPhoneNumber { get; set; }
        /// <summary>
        /// نام، نام خانوادگی مشتری باطل کننده
        /// </summary>
        public string revocationCustomerName { get; set; }
        /// <summary>
        /// شماره تلفن مشتری باطل کننده
        /// </summary>
        public string revocationCustomerPhoneNumber { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string branchReceiver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string giftTypeTitle { get; set; }
    }

    public class GiftSearchViewModel
    {
        public int? branchId { get; set; }
        public int page { get; set; }
        public int count { get; set; }
        public GiftStatus? status { get; set; }
        public GiftType? giftType { get; set; }
        public string term { get; set; }
        public string order { get; set; }
        public long? filter { get; set; }
        public int? branchIdShopping { get; set; }
        public int? branchReceiverCustomer { get; set; }
        public int? companyIdShopping { get; set; }
        public string buyerCustomerName { get; set; }
        public string buyerCustomerPhoneNumber { get; set; }
        public string dateSort { get; set; }
    }
    public class GiftCheckViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// وضعیت گیف
        /// </summary>
        public GiftStatus giftStatus { get; set; }
        /// <summary>
        /// نوع گیف
        /// </summary>
        public GiftType giftType { get; set; }
        /// <summary>
        /// نوع گیف
        /// </summary>
        public string giftTypeTitle { get; set; }
        /// <summary>
        /// ردیف شرکت خریدار گیفت
        /// </summary>
        public int? companyIdShopping { get; set; }
        /// <summary>
        /// شعبه خریدار گیفت
        /// </summary>
        public int? branchShoppingId { get; set; }
        /// <summary>
        /// شعبه خریدار گیفت
        /// </summary>
        public string branchShopping { get; set; }
        /// <summary>
        /// شعبه فروشنده گیفت به مشتری
        /// </summary>
        public string branchBuyerCustomer { get; set; }
        /// <summary>
        /// تاریخ فروش گیفت به مشتری
        /// </summary>
        public DateTime? dateSaleCustomer { get; set; }
        /// <summary>
        /// تاریخ فروش گیفت به شعبه
        /// </summary>
        public DateTime? dateSaleBranch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? expirationDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? expirationDateToSolar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string persianExpirationDate { get; set; }
        /// <summary>
        /// شعبه دریافت کننده گیفت از مشتری
        /// </summary>
        public string branchReceiverCustomer { get; set; }
        /// <summary>
        /// نام مشتری خریدار
        /// </summary>
        public string buyerCustomerName { get; set; }
        /// <summary>
        /// شماره تماس مشتری خریدار
        /// </summary>
        public string buyerCustomerPhoneNumber { get; set; }
        /// <summary>
        /// نام، نام خانوادگی مشتری باطل کننده
        /// </summary>
        public string revocationCustomerName { get; set; }
        /// <summary>
        /// وضعیت گیف
        /// </summary>
        public string giftStatusTitle { get; set; }
        /// <summary>
        /// مبلغ
        /// </summary>
        public long value { get; set; }
        /// <summary>
        /// مدت زمان انقضا
        /// </summary>
        public long? expirationTime { get; set; }
        /// <summary>
        /// مبلغ ,
        /// </summary>
        public string valueToSeparator { get; set; }
        /// <summary>
        /// نام شرکت خریدار گیفت
        /// </summary>
        public string owner { get; set; }
    }
    public class GiftTitleViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// وضعیت گیف
        /// </summary>
        public GiftStatus giftStatus { get; set; }
        /// <summary>
        /// نوع گیف
        /// </summary>
        public GiftType giftType { get; set; }
        /// <summary>
        /// نوع گیف
        /// </summary>
        public string giftTypeTitle { get; set; }
        /// <summary>
        ///  شرکت خریدار گیفت
        /// </summary>
        public string companyShopping { get; set; }
        /// <summary>
        /// شعبه خریدار گیفت
        /// </summary>
        public string branchShopping { get; set; }
        /// <summary>
        /// شعبه فروشنده گیفت به مشتری
        /// </summary>
        public string branchBuyerCustomer { get; set; }
        /// <summary>
        /// شعبه دریافت کننده گیفت از مشتری
        /// </summary>
        public string branchReceiverCustomer { get; set; }
        /// <summary>
        /// وضعیت گیف
        /// </summary>
        public string giftStatusTitle { get; set; }
        /// <summary>
        /// مبلغ
        /// </summary>
        public long value { get; set; }
        /// <summary>
        /// مبلغ ,
        /// </summary>
        public string valueToSeparator { get; set; }

        /// <summary>
        /// نام، نام خانوادگی مشتری خریدکننده
        /// </summary>
        public string buyerCustomerName { get; set; }
        /// <summary>
        ///  تاریخ فروخته شده به مشتری
        /// </summary>
        public DateTime? dateTimeSoldToTheCustomer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? dateTimeSoldToTheBranch { get; set; }
        /// <summary>
        ///  تاریخ فروخته شده به مشتری
        /// </summary>
        public string dateSoldToTheCustomer { get; set; }
        /// <summary>
        /// شماره تلفن مشتری خرید کننده
        /// </summary>
        public string buyerCustomerPhoneNumber { get; set; }
        /// <summary>
        /// نام، نام خانوادگی مشتری باطل کننده
        /// </summary>
        public string revocationCustomerName { get; set; }
        /// <summary>
        /// شماره تلفن مشتری باطل کننده
        /// </summary>
        public string revocationCustomerPhoneNumber { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string factorNumber { get; set; }
        /// <summary>
        /// مبلغ فاکتور
        /// </summary>
        public long factorPriceToSeparator { get; set; }
        /// <summary>
        /// مبلغ فاکتور
        /// </summary>
        public string factorPrice { get; set; }
        public DateTime? expireDate { get; set; }
        public long? expireTime { get; set; }
        public string persianExpireDate { get; set; }
        public string persianExpireTime { get; set; }
        public string canceledUserName { get; set; }
    }
    
    public class GiftChangeStatusViewModel
    {
        /// <summary>
        /// لیست ردیف
        /// </summary>
        public List<int> id { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public GiftStatus status { get; set; }
        /// <summary>
        /// نام مشتری
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// شماره همراه مشتری
        /// </summary>
        public string customerPhoneNumber { get; set; }
        /// <summary>
        /// تاریخ انقضا
        /// </summary>
        public long expirationTime { get; set; }
        /// <summary>
        /// ردیف شعبه خریدار گیفت
        /// </summary>
        public int? branchIdShopping { get; set; }
        /// <summary>
        /// ردیف شرکت خریدار گیفت
        /// </summary>
        public int? companyIdShopping { get; set; }

    }

    /// <summary>
    /// مدل تعویض زمان انقضای گیفت ها
    /// </summary>
    public class ChangeExpirationTimeViewModel
    {
        /// <summary>
        /// لیست ردیف
        /// </summary>
        public List<int> id { get; set; }
        /// <summary>
        /// تاریخ انقضا
        /// </summary>
        public long? expirationTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expirationDate { get; set; }
    }

    public class BranchListViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        public string phone { get; set; }

    }

    public class CompanyListViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class GiftRegistrationViewModel
    {
        public List<string> codeList { get; set; }
        public string customerName { get; set; }
        public string customerPhoneNumber { get; set; }
        public string factorNumber { get; set; }
        public GiftStatus status { get; set; }
        public long factorPrice { get; set; }
        public string factorPricesep { get; set; }
        public string owner { get; set; }
        public GiftType giftType { get; set; }
    }
    public class GiftPrintViewModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public long value { get; set; }
        public GiftType giftType { get; set; }
    }

    public class UsedGiftFactorSearchViewModel
    {
        public int id { get; set; }
        public int page { get; set; }
        public int count { get; set; }
        public string revocationCustomerName { get; set; }
        public string revocationCustomerPhoneNumber { get; set; }
        public int? companyIdShopping { get; set; }
        public int? branchReceiverCustomer { get; set; }
        public DateTime fromDateToAc { get; set; }
        public string fromDate { get; set; }
        public DateTime toDateToAc { get; set; }
        public string toDate { get; set; }
        public GiftType? giftType { get; set; }
        public GiftStatus? giftStatus { get; set; }

    }

    public class UsedGiftFactorViewModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public List<string> codeList { get; set; }
        public List<string> branchShoppingList { get; set; }
        public List<string> buyerCustomerNameList { get; set; }
        public List<string> buyerCustomerPhoneNumberList { get; set; }
        public DateTime? createDate { get; set; }
        public string usedDate { get; set; }
        
        public string persianCreateDate { get; set; }
        public string branchShopping { get; set; }
        public int giftCount { get; set; }
        public string factorNumber { get; set; }
        public long factorPrice { get; set; }
        public string factorPriceSeparator { get; set; }
        public string buyerCustomerName { get; set; }
        public string buyerCustomerPhoneNumber { get; set; }
        public string branchReceiverName { get; set; }
        public string revocationCustomerName { get; set; }
        public string revocationCustomerPhoneNumber { get; set; }
    }

}