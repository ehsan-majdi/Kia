using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// جزئیات فاکتور سی آر ام
    /// </summary>
    [Table("CrmInvoiceDetail", Schema = "crm")]
    public class CrmInvoiceDetail
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف فاکتور
        /// </summary>
        public int InvoiceId { get; set; }
        /// <summary>
        /// بارکد
        /// </summary>
        [MaxLength(100)]
        public string Barcode { get; set; }
        /// <summary>
        /// ابطال
        /// </summary>
        public bool Revocation { get; set; }

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
        /// فاکتور
        /// </summary>
        public virtual CrmInvoice Invoice { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
    }
}
