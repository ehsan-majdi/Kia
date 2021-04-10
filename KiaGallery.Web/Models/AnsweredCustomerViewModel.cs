using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// مدل مشتریان شرکت کرده در نظر سنجی
    /// </summary>
    public class AnsweredCustomerViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// تلفن همراه
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// تاریخ شمسی
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// سوال
        /// </summary>
        public string question { get; set; }
        /// <summary>
        /// عنوان نوع سوال
        /// </summary>
        public string typeTitle { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// نوع جواب
        /// </summary>
        public SurveyAnswerType type { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// صفحه
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// تعداد
        /// </summary>
        public int count { get; set; }
    }
}