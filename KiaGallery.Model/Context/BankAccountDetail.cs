using System;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// جزییات حساب های بانکی
    /// </summary>
    public class BankAccountDetail
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// ردیف حساب بانکی
        /// </summary>
        public int? BankAccountId { get; set; }
        /// <summary>
        /// عنوان حساب
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// نام بانک
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// شماره کارت
        /// </summary>
        public double? CardNumber { get; set; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// شبا
        /// </summary>
        public string Iban { get; set; }
        /// <summary>
        /// شرح در مورد نوع حساب
        /// </summary>
        public string Explanation { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int? CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int? ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime? ModifyDate { get; set; }
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
        public BankAccount BankAccount { get; set; }
    }

}
