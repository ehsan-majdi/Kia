using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// کارت وفاداری مشتریان
    /// </summary>
    public class LoyalityCard
    {
        public LoyalityCard()
        {
            CustomerList = new List<CustomerLoyality>();
            LogList = new List<LoyalityCardLog>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// کد کارت
        /// </summary>
        [MaxLength(20)]
        public string Code { get; set; }
        /// <summary>
        /// نوع کارت
        /// </summary>
        public LoyalityCardType CardType { get; set; }
        /// <summary>
        /// وضعیت کارت
        /// </summary>
        public LoyalityCardStatus CardStatus { get; set; }
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
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual List<CustomerLoyality> CustomerList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual List<LoyalityCardLog> LogList { get; set; }
    }
}
