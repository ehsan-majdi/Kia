using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context.BranchesPayments
{
    /// <summary>
    /// پیام ارسالی ربات پرداخت شعب
    /// </summary>
    public class BranchesPaymentsSendMessage
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// ردیف چت
        /// </summary>
        public long ChatId { get; set; }
        /// <summary>
        /// ردیف پیام
        /// </summary>
        public int MessageId { get; set; }
        /// <summary>
        /// ردیف پرداخت شعب
        /// </summary>
        public int BranchesPaymentsId { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// پرداخت شعب
        /// </summary>
        public BranchesPayments BranchesPayments { get; set; }
        /// <summary>
        /// کاربر ربات فاکتور پرداخت شعب 
        /// </summary>
        public UserData UserData { get; set; }
    }
}
