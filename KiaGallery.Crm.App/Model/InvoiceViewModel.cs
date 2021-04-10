using System;
using System.Collections.Generic;

namespace KiaGallery.Crm.App.Model
{
    /// <summary>
    /// مدل نمایشی فاکتور
    /// </summary>
    public class InvoiceViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// ردیف مشتری
        /// </summary>
        public int customerId { get; set; }
        /// <summary>
        /// نوع فاکتور سیستم مشتری
        /// </summary>
        public int invoiceType { get; set; }
        /// <summary>
        /// مبلغ خرید
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// مقدار تخفیف مصرف شده
        /// </summary>
        public int discount { get; set; }
        /// <summary>
        /// درصد تخفیف
        /// </summary>
        public float discountPercent { get; set; }
        /// <summary>
        /// لیست بارکد جزئیات فاکتور
        /// </summary>
        public List<InvoiceDetailViewModel> detailList { get; set; }
    }

    /// <summary>
    /// مدل نمایشی جزئیات فاکتور
    /// </summary>
    public class InvoiceDetailViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// بارکد
        /// </summary>
        public string barcode { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// تاریخ ایجاد شمسی
        /// </summary>
        public string persianDate { get; set; }
    }

}
