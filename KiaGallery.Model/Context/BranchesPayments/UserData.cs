using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context.BranchesPayments
{
    /// <summary>
    /// کاربران ربات فاکتور پرداخت شعب 
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public UserData()
        {
            SendMessageList = new List<BranchesPaymentsSendMessage>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// نوع کاربری
        /// </summary>
        public int UserType { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int? ModifyUserId { get; set; }
        /// <summary>
        /// ردیف شعب
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// ردیف چت
        /// </summary>
        public long ChatId { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(200)]
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [MaxLength(200)]
        public string LastName { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        [MaxLength(200)]
        public string Username { get; set; }
        /// <summary>
        /// متوقف شده 
        /// </summary>
        public bool Stoped { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }

        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual List<BranchesPaymentsSendMessage> SendMessageList { get; set; }
    }
}
