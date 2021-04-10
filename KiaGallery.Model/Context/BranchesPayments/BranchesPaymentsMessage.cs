using System;

namespace KiaGallery.Model.Context.BranchesPayments
{
    /// <summary>
    /// پیام ربات پرداخت شعب
    /// </summary>
    public class BranchesPaymentsMessage
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعیه
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// ردیف چت
        /// </summary>
        public long? ChatId { get; set; }
        /// <summary>
        /// ردیف پیام
        /// </summary>
        public int? MessageId { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// ناشناخته
        /// </summary>
        public bool Unknown { get; set; }
    }
}
