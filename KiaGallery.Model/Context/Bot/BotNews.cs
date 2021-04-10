using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// اخبار
    /// </summary>
    [Table(name: "BotNews", Schema = "Bot")]
    public class BotNews
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// نوع
        /// </summary>
        public BotType? Type { get; set; }
        /// <summary>
        /// متن
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// متن انگلیسی
        /// </summary>
        public string TextFa { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// ردیف فایل
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// تاریخ منقضی
        /// </summary>
        public DateTime? ExpiredDate { get; set; }
        /// <summary>
        /// کاربر ارسال کننده
        /// </summary>
        public int? SubmitedUser { get; set; }
        /// <summary>
        /// کاربر
        /// </summary>
        public virtual User User { get; set; }
    }
}
