using KiaGallery.Model.Context.Salary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// مشتریان وفادار
    /// </summary>
    [Table(name: "CustomerFactor", Schema = "customerLoyality")]
    public class CustomerFactor
    {
       
        public CustomerFactor()
        {
            CustomerSurveyList = new List<CustomerSurvey>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// ردیف وفاداری مشتریان
        /// </summary>
        public int CustomerLoyalityId { get; set; }
        /// <summary>
        /// مبلغ فاکتور
        /// </summary>
        public long FactorPrice { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string FactorNumber { get; set; }
        /// <summary>
        /// وزن فاکتور
        /// </summary>
        public decimal? FactorWeight { get; set; }
        /// <summary>
        /// کد کالا
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// نوع خرید یا مرجوعی
        /// </summary>
        public PurchaseType PurchaseType { get; set; }
        /// <summary>
        /// تاریخ ثبت
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// تاریخ مرجوع
        /// </summary>
        public DateTime? ReturnDate { get; set; }
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
        /// تاریخ ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// وفاداری مشتریان
        /// </summary>
        public virtual CustomerLoyality CustomerLoyality { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده 
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// لیست نظر سنجی
        /// </summary>
        public virtual List<CustomerSurvey> CustomerSurveyList { get; set; }
        
    }
}
