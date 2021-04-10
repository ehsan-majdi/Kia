namespace KiaGallery.Common
{
    /// <summary>
    /// شی بازگشتی برای درخواست های اجکس
    /// </summary>
    public class Response
    {
        /// <summary>
        /// وضعیت
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// پیام سیستم
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// پاسخ بازگشتی
        /// </summary>
        public object data { get; set; }
    }

    /// <summary>
    /// شی بازگشتی برای درخواست های اجکس
    /// </summary>
    public class Response<T>
    {
        /// <summary>
        /// وضعیت
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// پیام سیستم
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// پاسخ بازگشتی
        /// </summary>
        public T data { get; set; }
    }

}
