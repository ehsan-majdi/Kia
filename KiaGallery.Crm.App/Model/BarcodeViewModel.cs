using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Crm.App.Model
{
    /// <summary>
    /// مدل نمایشی بارکد
    /// </summary>
    public class BarcodeViewModel
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
        /// <summary>
        /// ابطال
        /// </summary>
        public bool revocation { get; set; }
        /// <summary>
        /// قیمت طلا
        /// </summary>
        public string goldPrice { get; set; }
    }
}
