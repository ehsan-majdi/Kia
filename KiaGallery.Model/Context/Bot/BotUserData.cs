using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context.Bot
{
    /// <summary>
    /// کاربران ربات
    /// </summary>
    [Table(name: "BotUserData", Schema = "Bot")]
    public class BotUserData
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public BotUserData()
        {
            BotOrderList = new List<BotOrder>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// نوع کاربری
        /// </summary>
        public int? UserType { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// ردیف چت
        /// </summary>
        public long ChatId { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(100)]
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [MaxLength(100)]
        public string LastName { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        [MaxLength(100)]
        public string Username { get; set; }
        /// <summary>
        /// توقف
        /// </summary>
        public bool? Stoped { get; set; }
        /// <summary>
        /// زبان
        /// </summary>
        public byte? Language { get; set; }
        /// <summary>
        /// تاریخ عضویت
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// لیست سفارشات
        /// </summary>
        public virtual List<BotOrder> BotOrderList { get; set; }
    }
}
