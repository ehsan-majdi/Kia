using KiaGallery.Model.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model
{
    public class LoyalityCardLog
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// وضعیت کارت
        /// </summary>
        public LoyalityCardStatus CardStatus { get; set; }
        /// <summary>
        /// ردیف کارت
        /// </summary>
        public int LoyalityCardId { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        [MaxLength(45)]
        public string Ip { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کارت
        /// </summary>
        public virtual LoyalityCard LoyalityCard { get; set; }
    }
}
