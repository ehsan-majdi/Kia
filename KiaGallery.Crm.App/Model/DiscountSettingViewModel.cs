namespace KiaGallery.Crm.App.Model
{
    /// <summary>
    /// مدل نمایشی تنظیمات تخفیفات فاکتور مشتری
    /// </summary>
    public class DiscountSettingViewModel
    {
        /// <summary>
        /// از قیمت
        /// </summary>
        public int fromPrice { get; set; }
        /// <summary>
        /// تا قیمت
        /// </summary>
        public int toPrice { get; set; }
        /// <summary>
        /// میزان تخفیف
        /// </summary>
        public float discount { get; set; }
    }
}
