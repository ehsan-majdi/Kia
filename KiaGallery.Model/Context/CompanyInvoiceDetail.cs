using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// جزئیات فاکتور حقوقی یا شرکتی
    /// </summary>
    public class CompanyInvoiceDetail
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف جدوف فاکتور حقوقی یا شرکتی
        /// </summary>
        public int CompanyInvoiceId { get; set; }
        /// <summary>
        /// کد شناسایی کالا 
        /// </summary>
        public string IdentificationCode { get; set; }
        /// <summary>
        /// شرح کالا
        /// </summary>
        public string DescriptionProduct { get; set; }
        /// <summary>
        /// عیار
        /// </summary>
        public int Carat { get; set; }
        /// <summary>
        /// سوت
        /// </summary>
        public int Whistle { get; set; }
        /// <summary>
        /// گرم
        /// </summary>
        public int Gram { get; set; }
        /// <summary>
        /// وزن سنگ
        /// </summary>
        public int StoneWeight { get; set; }
        /// <summary>
        /// قیمت سنگ
        /// </summary>
        public long StonePrice { get; set; }
        /// <summary>
        /// اجرت ساخت
        /// </summary>
        public long Wage { get; set; }
        /// <summary>
        /// قیمت طلا
        /// </summary>
        public long GoldPrice { get; set; }
        /// <summary>
        /// فاکتور حقوقی یا شرکتی
        /// </summary>
        public virtual CompanyInvoice CompanyInvoice { get; set; }
    }
}
