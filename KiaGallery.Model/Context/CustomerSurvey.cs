using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// نظر سنجی مشتری
    /// </summary>
    public class CustomerSurvey
    {
        /// <summary>
        /// متد سازنده
        /// </summary>
        public CustomerSurvey()
        {
            SurveyCustomerAnswerList = new List<SurveyCustomerAnswer>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف فاکتور مشتری وفادار
        /// </summary>
        public int? CustomerFactorId { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// آی پی
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// فاکتور مشتری وفادار
        /// </summary>
        public virtual CustomerFactor CustomerFactor { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// لیست جواب های مشتریان
        /// </summary>
        public virtual List<SurveyCustomerAnswer> SurveyCustomerAnswerList { get; set; }
    }
}
