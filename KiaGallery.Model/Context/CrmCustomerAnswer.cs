using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// جواب سوالات سیستم مدیریت ارتباط با مشتری
    /// </summary>
    public class CrmCustomerAnswer
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// ردیف مشتری
        /// </summary>
        public int CrmCustomerId { get; set; }
        /// <summary>
        /// ردیف سوال
        /// </summary>
        public int CrmQuestionId { get; set; }
        /// <summary>
        /// جواب بله خیر
        /// </summary>
        public bool? YesNoAnswer { get; set; }
        /// <summary>
        /// ردیف مقدار سوال
        /// </summary>
        public string CrmQuestionValueId { get; set; }
        /// <summary>
        /// جواب چند گزینه ای
        /// </summary>
        [MaxLength(200)]
        public string MultiAnswer { get; set; }
        /// <summary>
        /// مقدار جواب چند گزینه ای
        /// </summary>
        [MaxLength(200)]
        public string MultiAnswerValue { get; set; }
        /// <summary>
        /// جواب تک گزینه ای
        /// </summary>
        [MaxLength(100)]
        public string SingleAnswer { get; set; }
        /// <summary>
        /// مقدار جواب تک گزینه ای
        /// </summary>
        public bool SingleAnswerValue { get; set; }
        /// <summary>
        /// جواب تشریحی
        /// </summary>
        [MaxLength(1000)]
        public string DescriptiveAnswer { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int ModifyUserId { get; set; }
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
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// سوالات مدیریت ارتباط با مشتری
        /// </summary>
        public virtual CrmQuestion CrmQuestion { get; set; }
        /// <summary>
        ///  مقدار سوالات مدیریت ارتباط با مشتری
        /// </summary>
        public virtual CrmQuestionValue CrmQuestionValue { get; set; }
        /// <summary>
        /// مشتری
        /// </summary>
        public virtual CrmCustomer CrmCustomer { get; set; }
    }
}

