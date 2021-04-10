using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// جدول کمکی برای استعلام غذا
    /// </summary>
    public class FoodCensusViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool boolDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool boolCheckingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int foodRegisterList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int foodCensusId { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// نام کاربر
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// نام غذای ثبت شده توسط مدیر این بخش
        /// </summary>
        public string foodName { get; set; }
        /// <summary>
        /// نوع غذا
        /// </summary>
        public bool typeFood { get; set; }
        /// <summary>
        /// پیش غذا
        /// </summary>
        public bool? appertizer { get; set; }
        /// <summary>
        ///  غذا
        /// </summary>
        public bool? food { get; set; }
        /// <summary>
        /// تعداد غذاهای ثبت شده توسط کاربران سیستم
        /// </summary>
        public int foodCount { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string persianDate { get; set; }
        public DateTime day { get; set; }
        public DateTime? dateTime { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// تعیین روز تعطیل توسط مدیر این بخش
        /// </summary>
        public bool? holiday { get; set; }
        /// <summary>
        /// وضعیت ثبت غذا توسط کاربران لاگین شده در سیستم
        /// </summary>
        public FoodStatus? foodStatus { get; set; }
        public bool? foodWithoutRice { get; set; }
        /// <summary>
        /// ردیف ثبت غذا
        /// </summary>
        public int? foodRegistrationId { get; set; }
        public List<string> foodCensusList { get; set; }
    }
    public class FoodCensusValueViewModel
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// برگرداندن اولین روز ماه
        /// </summary>
        public string persianFirstDayOfMonth { get; set; }
        /// <summary>
        /// برگرداندن آخرین روز ماه
        /// </summary>
        public string persianLastDayOfMonth { get; set; }
        public int pageType { get; set; }
        /// <summary>
        /// ردیف کاربران
        /// </summary>
        public List<int> userIdList { get; set; }
        /// <summary>
        /// غذا
        /// </summary>
        public List<bool> foodList { get; set; }
        /// <summary>
        /// پیش غذا
        /// </summary>
        public List<bool> appertizerList { get; set; }
    }


    public class FoodDateViewModel
    {
        public int? id { get; set; }
        public bool food { get; set; }
        public bool appertizer { get; set; }
        /// <summary>
        /// وضعیت ثبت غذا توسط کاربران لاگین شده در سیستم
        /// </summary>
        public FoodStatus? foodStatus { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int? userId { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// لیست
        /// </summary>
        public FoodListViewModel list { get; set; }
    }
    public class FoodListViewModel
    {
        /// <summary>
        /// ردیف کاربران
        /// </summary>
        public List<int> userIdList { get; set; }
        /// <summary>
        /// غذا
        /// </summary>
        public List<bool> foodList { get; set; }
        /// <summary>
        /// پیش غذا
        /// </summary>
        public List<bool> appertizerList { get; set; }
    }
}