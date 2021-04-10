using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    public class LicencedProduct
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف محصول
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public float Weight { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// بارکد
        /// </summary>
        public string Barcode { get; set; }
        /// <summary>
        /// متریال
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// عیار
        /// </summary>
        public string Purity { get; set; }
        /// <summary>
        /// عکس
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// یورو اضافی
        /// </summary>
        public long? Euro { get; set; }
        /// <summary>
        /// اجرت
        /// </summary>
        public long Wage { get; set; }
        /// <summary>
        /// قیمت سنگ یا چرم
        /// </summary>
        public long? LeatherStonePrice { get; set; }
        /// <summary>
        /// محصول
        /// </summary>
        public virtual Product Product { get; set; }
    }

}
