namespace KiaGallery.Model.Common
{
    /// <summary>
    /// کلاس برای مقدار بازگشتی فایل هایی که ذخیره می شوند
    /// </summary>
    public class SaveFile
    {
        /// <summary>
        /// ردیف فایل
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// حجم فایل (به بایت)
        /// </summary>
        public long length { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// لینک
        /// </summary>
        public string link { get; set; }
    }
}
