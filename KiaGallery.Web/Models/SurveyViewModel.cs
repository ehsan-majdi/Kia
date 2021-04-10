using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// مدل نطر سنجی
    /// </summary>
    public class SurveyViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// نوع سوال
        /// </summary>
        public SurveyQuestionType questionType { get; set; }
        /// <summary>
        /// لیست جواب 
        /// </summary>
        public List<AnswerListViewModel> answerList { get; set; }
    }

    /// <summary>
    /// مدل جواب
    /// </summary>
    public class AnswerListViewModel
    {
        /// <summary>
        /// ردیف سوال
        /// </summary>
        public int questionId { get; set; }
        /// <summary>
        ///نوع جواب
        /// </summary>
        public SurveyAnswerType surveyAnswerType { get; set; }
        /// <summary>
        /// پیشنهادات و انتقادات
        /// </summary>
        public string offer { get; set; }
    }
}