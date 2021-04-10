using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// فاکتور
    /// </summary>
    [Table("CrmInvoice", Schema = "crm")]
    public class CrmInvoice
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public CrmInvoice()
        {
            InvoiceDetailList = new List<CrmInvoiceDetail>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف مشتری
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// نوع فاکتور سیستم مشتری
        /// </summary>
        public CrmInvoiceType InvoiceType { get; set; }
        /// <summary>
        /// مبلغ خرید
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// مقدار تخفیف مصرف شده
        /// </summary>
        public int Discount { get; set; }
        /// <summary>
        /// درصد تخفیف
        /// </summary>
        public float DiscountPercent { get; set; }
        /// <summary>
        /// قیمت روز طلا
        /// </summary>
        public int GoldPrice { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int BranchId { get; set; }

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
        /// آی پی کاربر ایجاد کننده
        /// </summary>
        [MaxLength(45)]
        public string CreateIp { get; set; }
        /// <summary>
        /// آی پی کاربر ویرایش کننده
        /// </summary>
        [MaxLength(45)]
        public string ModifyIp { get; set; }

        /// <summary>
        /// مشتری
        /// </summary>
        public virtual Customer Customer { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }

        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }

        /// <summary>
        /// جزئیات فاکتور
        /// </summary>
        public virtual List<CrmInvoiceDetail> InvoiceDetailList { get; set; }
    }
}
