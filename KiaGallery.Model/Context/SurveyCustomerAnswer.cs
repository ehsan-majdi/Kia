using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// جواب نظر سنجی مشتری
    /// </summary>
    public class SurveyCustomerAnswer
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف سوال
        /// </summary>
        public int SurveyQuestionId { get; set; }
        /// <summary>
        /// ردیف نظر سنجی کاربر
        /// </summary>
        public int CustomerSurveyId { get; set; }
        /// <summary>
        /// ردیف مقدار سوال
        /// </summary>
        public SurveyAnswerType SurveyAnswerType { get; set; }
        /// <summary>
        /// پیشنهادات
        /// </summary>
        public string Offer { get; set; }
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
        /// سوالات مدیریت ارتباط با مشتری
        /// </summary>
        public virtual SurveyQuestion SurveyQuestion { get; set; }
        /// <summary>
        /// نظر سنجی مشتری
        /// </summary>
        public virtual CustomerSurvey CustomerSurvey { get; set; }
    }
}
