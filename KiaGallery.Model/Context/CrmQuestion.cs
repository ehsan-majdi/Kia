using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// سوالات سیستم مدیریت ارتباط با مشتری 
    /// </summary>
    public class CrmQuestion
    {
        /// <summary>
        /// کلاس مقادیر سوالات
        /// </summary>
        public CrmQuestion()
        {
            CrmQuestionValueList = new List<CrmQuestionValue>();
            CrmCustomerAnswerList = new List<CrmCustomerAnswer>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// ردیف دسته بندی سوالات
        /// </summary>
        public int CategoryQuestionId { get; set; }
        /// <summary>
        /// نوع پرسش
        /// </summary>
        public CrmQuestionType CrmQuestionType { get; set; }
        /// <summary>
        /// نوع خرید
        /// </summary>
        public BuyType BuyType { get; set; }
        /// <summary>
        /// زیرمجموعه نوع خرید برای شعب
        /// </summary>
        public BuyTypeSubset BuyTypeSubset { get; set; }
        /// <summary>
        ///زیر مجموعه خرید آنلاین برای دفتر مرکزی
        /// </summary>
        public BuyTypeOnline BuyTypeOnline { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        [MaxLength(500)]
        public string Title { get; set; }
        /// <summary>
        /// بله / خیر
        /// </summary>
        public bool? DefaultYesNo { get; set; }
        /// <summary>
        /// تشریحی
        /// </summary>
        public string DefaultDescriptive { get; set; }
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
        /// لیست مقادیر سوالات
        /// </summary>
        public virtual List<CrmQuestionValue> CrmQuestionValueList { get; set; }
        /// <summary>
        ///لیست مقادیر پاسخ به سوالات توسط مشتری
        /// </summary>
        public virtual List<CrmCustomerAnswer> CrmCustomerAnswerList { get; set; }
        /// <summary>
        /// دسته بندی سوالات
        /// </summary>
        public virtual CategoryQuestion CategoryQuestion { get; set; }
    }
}

