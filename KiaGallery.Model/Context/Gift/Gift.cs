using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Gift
{
    /// <summary>
    /// گیفت
    /// </summary>
    [Table(name: "Gift", Schema = "gift")]
    public class Gift
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        [MaxLength(12)]
        public string Code { get; set; }
        /// <summary>
        /// نوع هدیه
        /// </summary>
        public GiftType GiftType { get; set; }
        /// <summary>
        /// وضعیت گیف
        /// </summary>
        public GiftStatus GiftStatus { get; set; }
        /// <summary>
        /// ردیف شرکت خریدار گیفت
        /// </summary>
        public int? CompanyIdShopping { get; set; }
        /// <summary>
        /// ردیف شعبه خریدار گیفت
        /// </summary>
        public int? BranchIdShopping { get; set; }
        /// <summary>
        /// ردیف شعبه دریافت کننده گیفت از مشتری
        /// </summary>
        public int? BranchIdReceiverCustomer { get; set; }
        /// <summary>
        /// مبلغ
        /// </summary>
        public long Value { get; set; }
        /// <summary>
        /// مدت زمان انقضا
        /// </summary>
        public long? ExpirationTime { get; set; }
        /// <summary>
        /// تاریخ انقضا به شمسی
        /// </summary>
        public DateTime? ExpiryDateToSolar { get; set; }
        /// <summary>
        /// نام، نام خانوادگی مشتری خریدکننده
        /// </summary>
        [MaxLength(50)]
        public string BuyerCustomerName { get; set; }
        /// <summary>
        /// شماره تلفن مشتری خرید کننده
        /// </summary>
        [MaxLength(14)]
        public string BuyerCustomerPhoneNumber { get; set; }
        /// <summary>
        /// نام، نام خانوادگی مشتری باطل کننده
        /// </summary>
        [MaxLength(50)]
        public string RevocationCustomerName { get; set; }
        /// <summary>
        /// شماره تلفن مشتری باطل کننده
        /// </summary>
        [MaxLength(14)]
        public string RevocationCustomerPhoneNumber { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string FactorNumber { get; set; }
        /// <summary>
        /// مبلغ فاکتور
        /// </summary>
        public long FactorPrice { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// سوابق گیفت
        /// </summary>
        public virtual List<GiftLog>  GiftLog { get; set; }
        /// <summary>
        /// شعبه خریدار گیفت
        /// </summary>
        public virtual Branch BranchShopping { get; set; }
        /// <summary>
        /// شرکت خریدار گیفت
        /// </summary>
        public virtual Company CompanyShopping { get; set; }
        /// <summary>
        /// شعبه دریافت کننده گیفت از مشتری
        /// </summary>
        public virtual Branch BranchReceiver { get; set; }
    }
}
