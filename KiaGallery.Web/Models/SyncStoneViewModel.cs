namespace KiaGallery.Web.Models
{
    /// <summary>
    /// مدل برای همگام سازی اطلاعات سنگ از سیستم پورتال
    /// </summary>
    public class SyncStoneViewModel
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
        /// نام سنگ
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// نام انگلیسی سنگ
        /// </summary>
        public string englishTitle { get; set; }
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
