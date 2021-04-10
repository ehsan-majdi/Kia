using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// مشتری
    /// </summary>
    public class CrmCustomer
    {
        /// <summary>
        /// کلاس سازنده
        /// </summary>
        public CrmCustomer()
        {
            CrmCustomerAnswerList = new List<CrmCustomerAnswer>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// نام کامل
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// شماره تماس
        /// </summary>
        [MaxLength(18)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// نوع خرید مشتری
        /// </summary>
        public BuyType BuyType { get; set; }
        /// <summary>
        /// زیرمجموعه نوع خرید برای شعب
        /// </summary>
        public BuyTypeSubset BuyTypeSubset { get; set; }
        /// <summary>
        ///زیر مجموعه خرید آنلاین برای دفتر مرکزی
        /// </summary>
        public BuyTypeOnline BuyTypeOnline { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public int FactorNumber { get; set; }
        /// <summary>
        /// امتیاز
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// تاریخ 
        /// </summary>
        public DateTime? Date { get; set; }
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
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// لیست جواب های مشتری
        /// </summary>
        public virtual List<CrmCustomerAnswer> CrmCustomerAnswerList { get; set; }
        /// <summary>
        /// دسته بندی سوالات
        /// </summary>
        public virtual Branch Branch { get; set; }
    }
}
