using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class CustomerLoyalityViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? hiddenId { get; set; }
        /// <summary>
        /// ردیف مشتری
        /// </summary>
        public int customerId { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// مبلغ فاکتور
        /// </summary>
        public long factorPrice { get; set; }
        /// <summary>
        /// وزن فاکتور
        /// </summary>
        public decimal? factorWeight { get; set; }
        /// <summary>
        /// مبلغ فاکتور با جدا کننده سه رقمی
        /// </summary>
        public string separateFactorPrice { get; set; }
        /// <summary>
        /// تعداد فاکتور
        /// </summary>
        public int factorCount { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string factorNumber { get; set; }
        /// <summary>
        /// نام کامل
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// نوع خرید یا مرجوعی
        /// </summary>
        public PurchaseType purchaseType { get; set; }
        /// <summary>
        /// عنوان نوع خرید
        /// </summary>
        public string purchaseTypeTitle { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int branchId { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// تاریخ مرجوع
        /// </summary>
        public DateTime? returnDate { get; set; }
        /// <summary>
        /// تاریخ مرجوع
        /// </summary>
        public string persianReturnDate { get; set; }
        /// <summary>
        /// تاریخ شمسی
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string term { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string productCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string loyalityCardCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string addressLocation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime mariageDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string persianMariageDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string persianBirthDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? birthDate { get; set; }
        /// <summary>
        /// لیست فاکتور مشتریان
        /// </summary>
        public List<FactorProductCodeViewModel> factorProductCodeViewModelList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
   public class CustomerLoyalityFactorViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? hiddenId { get; set; }
        /// <summary>
        /// ردیف مشتری
        /// </summary>
        public int customerId { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// مبلغ فاکتور
        /// </summary>
        public long? factorPrice { get; set; }
        /// <summary>
        /// مبلغ فاکتور با جدا کننده سه رقمی
        /// </summary>
        public string separateFactorPrice { get; set; }
        /// <summary>
        /// تعداد فاکتور
        /// </summary>
        public int factorCount { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string factorNumber { get; set; }
        /// <summary>
        /// نام کامل
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// نوع خرید یا مرجوعی
        /// </summary>
        public PurchaseType purchaseType { get; set; }
        /// <summary>
        /// عنوان نوع خرید
        /// </summary>
        public string purchaseTypeTitle { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int branchId { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// تاریخ شمسی
        /// </summary>
        public string persianDate { get; set; }
        public string term { get; set; }
        public string productCode { get; set; }
    }
    public class FactorProductCodeViewModel
    {
        public int id { get; set; }
        public int code { get; set; }
        public int customerFactorId { get; set; }
    }
    public class CustomerFactorViewModel
    {
        public int? id { get; set; }
        /// <summary>
        /// صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// تعداد مرجوعی
        /// </summary>
        public int returnCount { get; set; }
        /// <summary>
        /// بخش
        /// </summary>
        public string term { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// نوع شعبه
        /// </summary>
        public BranchType branchType { get; set; }

    }
    public class CustomerLoyalitySearchViewModel
    {
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? branchId { get; set; }
        /// <summary>
        /// مبلغ فاکتور
        /// </summary>
        public long? factorPrice { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// نام کامل
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// تعداد فاکتور
        /// </summary>
        public int? factorCount { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// مبلغ فاکتور مرجوعی
        /// </summary>
        public long? returnFactorPrice { get; set; }
        /// <summary>
        /// صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? priceGoal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? countGoal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool goal { get; set; }

    }
    public class SearchAutoCompeleteViewModel
    {
        public int id { get; set; }
        public bool expired { get; set; }
        public string phoneNumber { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string productCode { get; set; }
        public string factorNumber { get; set; }
        public string branchName { get; set; }
        public string persianDate { get; set; }
        public DateTime date { get; set; }
        public string persianBirthDate { get; set; }
        public DateTime? birthDate { get; set; }
        public string persianMarriageDate { get; set; }
        public DateTime? mariageDate { get; set; }
    }

}