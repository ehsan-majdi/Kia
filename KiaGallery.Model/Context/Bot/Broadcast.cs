using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// پیام عمومی
    /// </summary>
    [Table(name: "Broadcast", Schema = "Bot")]
    public class Broadcast
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// نوع
        /// </summary>
        public BotType? BroadcastType { get; set; }
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
        /// ردیف فایل
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// ارسال شده
        /// </summary>
        public bool? Sended { get; set; }
        /// <summary>
        /// کاربر ارسال کننده
        /// </summary>
        public int? SubmitedUser { get; set; }
        /// <summary>
        /// کاربری 
        /// </summary>
        public virtual User User { get; set; }
        /// <summary>
        /// محصول 
        /// </summary>
        public virtual Product Product { get; set; }
    }
}
