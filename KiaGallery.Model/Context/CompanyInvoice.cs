using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// فاکتور حقوقی یا شرکتی
    /// </summary>
    public class CompanyInvoice
    {
        public CompanyInvoice()
        {
            CompanyInvoiceDetailList = new List<CompanyInvoiceDetail>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// نام شخص حقیقی
        /// </summary>
        public string BuyerName { get; set; }
        /// <summary>
        /// شماره اقتصادی
        /// </summary>
        public string BuyerEconomicalNumber { get; set; }
        /// <summary>
        /// شناسه ملی
        /// </summary>
        public string BuyerNationalId { get; set; }
        /// <summary>
        /// کد پستی
        /// </summary>
        public string BuyerPostalCode { get; set; }
        /// <summary>
        /// تخفیفات و کسورات
        /// </summary>
        public long Reduction { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string BuyerAddress { get; set; }
        /// <summary>
        /// تلفن
        /// </summary>
        public string BuyerPhone { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// فایل اصلی
        /// </summary>
        public string AttachmentFile { get; set; }
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
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// لیست جزئیات فاکتور حقوقی یا شرکتی
        /// </summary>
        public virtual List<CompanyInvoiceDetail> CompanyInvoiceDetailList { get; set; }
    }
}
