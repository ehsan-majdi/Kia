using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// جدول کمکی برای ثبت دریافت غذا توسط کاربر لاگین شده
    /// </summary>
    public class FoodRegistrationViewModel
    {
        /// <summary>
        /// ردیف 
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف غذا
        /// </summary>
        public int foodCensusId { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// وضعیت ثبت غذا توسط کاربران لاگین شده در سیستم
        /// </summary>
        public FoodStatus foodStatus { get; set; }
        /// <summary>
        /// پیش غذا
        /// </summary>
        public bool appertizer { get; set; }
        /// <summary>
        /// غذا با برنج
        /// </summary>
        public bool food { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// غذای بدون برنج
        /// </summary>
        public bool foodWithoutRice { get; set; }
        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// تعداد غذا
        /// </summary>
        public int foodCount { get; set; }
        /// <summary>
        /// تعداد سالاد
        /// </summary>
        public int appertizerCount { get; set; }
    }
}