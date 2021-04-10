using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context.BranchesPayments
{
    /// <summary>
    /// تاریخچه وضعیت ثبت فاکتورها
    /// </summary>
    public class BranchesPaymentsLog
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف گزارش روزانه
        /// </summary>
        public int BranchPaymentsId { get; set; }
        /// <summary>
        /// ردیف کاربر ثبت کننده
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// تاریخ ثبت
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public BranchesPaymentsStatus Status { get; set; }
        /// <summary>
        /// اطلاعات ثبت شده و تغییر داده شده
        /// </summary>
        public string PrevData { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        [MaxLength(48)]
        public string Ip { get; set; }

        /// <summary>
        /// گزارش روزانه
        /// </summary>
        public virtual BranchesPayments BranchesPayments { get; set; }
        /// <summary>
        /// کاربر ثبت گننده
        /// </summary>
        public virtual User User { get; set; }
    }
}
