using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// استعلام محصولات
    /// </summary>
    public class InquiryProduct
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public InquiryProduct()
        {
            InquiryProductReplyList = new List<InquiryProductReply>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه درخواست کننده استعلام محصول
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// متن
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// ردیف مصحول
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// فایل
        /// </summary>
        public string FileName { get; set; }
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
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// محصولات
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }

        /// <summary>
        /// لیست پاسخ استعلام محضول
        /// </summary>
        public virtual List<InquiryProductReply> InquiryProductReplyList { get; set; }

    }
}
