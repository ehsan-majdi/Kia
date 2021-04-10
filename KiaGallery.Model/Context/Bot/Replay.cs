using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// بازپخش
    /// </summary>
    [Table(name: "Replay", Schema = "Bot")]
    public class Replay
    {
        public Replay()
        {
            MessageList = new List<BotMessage>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// متن
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// لیست پیام
        /// </summary>
        public virtual List<BotMessage> MessageList { get; set; }
    }
}
