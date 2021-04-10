using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Areas.Api.Models
{
    /// <summary>
    /// مدل کارت هدیه
    /// </summary>
    public class GiftViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// عنوان وضعیت
        /// </summary>
        public string statusTitle { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// پیام
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public long ammount { get; set; }
    }

    /// <summary>
    /// مدل سوزاندن گیفت
    /// </summary>
    public class BurnGiftViewModel
    {
        /// <summary>
        /// کد کارت هدیه
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// نام خریدار
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// تلفن خریدار
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string facotrNumber { get; set; }
        /// <summary>
        /// مبلغ فاکتور
        /// </summary>
        public long price { get; set; }
    }

}