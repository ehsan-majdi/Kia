using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// مدل تنظیمات مربوط به غذا
    /// </summary>
    public class FoodSettingViewModel
    {
        /// <summary>
        /// تعداد غذا
        /// </summary>
        public string foodCount{ get; set; }
        /// <summary>
        /// تعداد پیش غذا
        /// </summary>
        public string appetizerCount{ get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description{ get; set; }
        public string value{ get; set; }
    }
}