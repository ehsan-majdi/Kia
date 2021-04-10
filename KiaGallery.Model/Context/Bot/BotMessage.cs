using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// پیام
    /// </summary>
    [Table(name: "BotMessage", Schema = "Bot")]
    public class BotMessage
    {
        public BotMessage()
        {
            MessageList = new List<BotMessage>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف چت
        /// </summary>
        public long? ChatId { get; set; }
        /// <summary>
        /// ردیف پیام
        /// </summary>
        public int MessageId { get; set; }
        /// <summary>
        /// متن
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// نا مشخص
        /// </summary>
        public bool? Unknown { get; set; }
        /// <summary>
        /// ردیف باز پخش
        /// </summary>
        public int? ReplayId { get; set; }
        /// <summary>
        /// ردیف پیام باز پخش
        /// </summary>
        public int? ReplayMessageId { get; set; }
        /// <summary>
        /// نوع فایل
        /// </summary>
        public byte? FileType { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// ردیف فایل
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// کاربر ارسال کننده
        /// </summary>
        public int? SubmitedUser { get; set; }
        /// <summary>
        /// لیست پیام ها
        /// </summary>
        public virtual List<BotMessage> MessageList { get; set; }
        /// <summary>
        /// پیام
        /// </summary>
        public virtual BotMessage ReplayMessage { get; set; }
        /// <summary>
        /// باز پخش
        /// </summary>
        public virtual Replay Replay { get; set; }
        /// <summary>
        /// کاربر
        /// </summary>
        public virtual User User { get; set; }
    }
}
