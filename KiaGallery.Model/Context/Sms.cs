using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// پیامک های ارسال شده
    /// </summary>
    public class Sms
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
        /// 
        /// </summary>
        public string DayOfMonth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DayOfWeek { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SendingTimeMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BranchId { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ارسال پیام
        /// </summary>
        public DateTime SendingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TimeTotalMinutes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Sent { get; set; }
        /// <summary>
        /// آی پی
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual User ModifyUser { get; set; }
    }
}
