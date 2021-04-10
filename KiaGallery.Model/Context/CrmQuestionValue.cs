using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// مقدار سوالات سیستم مدیریت ارتباط با مشتری
    /// </summary>
    public class CrmQuestionValue
    {
        /// <summary>
        /// کلاس سازنده
        /// </summary>
        public CrmQuestionValue()
        {
            CrmCustomerAnswerList = new List<CrmCustomerAnswer>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف سوالات
        /// </summary>
        public int CrmQuestionId { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// مقدار پیش فرض
        /// </summary>
        public bool DefaultSelected { get; set; }
        /// <summary>
        /// توضیحات مقدار
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }
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
        ///لیست مقادیر پاسخ به سوالات توسط مشتری
        /// </summary>
        public virtual List<CrmCustomerAnswer> CrmCustomerAnswerList { get; set; }
        /// <summary>
        /// سوال
        /// </summary>
        public virtual CrmQuestion CrmQuestion { get; set; }

    }
}
