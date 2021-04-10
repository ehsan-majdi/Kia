using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// اینستاگرام
    /// </summary>
    [Table(name: "Instagram", Schema = "Bot")]
    public class Instagram
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف اینستاگرام
        /// </summary>
        public string InstagramId { get; set; }
        /// <summary>
        /// نوع
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// ردیف فایل
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// ارسال شده
        /// </summary>
        public bool Sended { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
    }
}
