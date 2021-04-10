using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    public class Ticket
    {

        public Ticket()
        {
            TicketMessagesList = new List<TicketMessage>();
            NotificationList = new List<Notification>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int? FromUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? ToUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TicketStatus TicketStatus { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        [MaxLength(45)]
        public string Ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Department Department { get; set; }
        /// <summary>
        /// کاربر
        /// </summary>
        public virtual User FromUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual User ToUser { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }

        public virtual List<TicketMessage> TicketMessagesList { get; set; }
        public virtual List<Notification> NotificationList { get; set; }

    }
}
