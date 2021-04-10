using System;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// پیامک های ارسال شده
    /// </summary>
    public class SentSms
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// متن پیام
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// شماره مقصد پیام
        /// </summary>
        public string DestinationNumber { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int? CreateUserId { get; set; }
        /// <summary>
        /// تاریخ ارسال پیام
        /// </summary>
        public DateTime SendingDate { get; set; }
        /// <summary>
        /// آی پی
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
    }
}
