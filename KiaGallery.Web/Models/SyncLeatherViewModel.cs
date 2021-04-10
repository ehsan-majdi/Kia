namespace KiaGallery.Web.Models
{
    /// <summary>
    /// مدل برای همگام سازی اطلاعات چرم از سیستم پورتال
    /// </summary>
    public class SyncLeatherViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// نوع سنگ
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// نام چرم
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// آدرس فایل تصویر
        /// </summary>
        public string imageUrl { get; set; }
        /// <summary>
        /// فعال بودن سنگ
        /// </summary>
        public bool active { get; set; }
    }
}
