using System;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// پاسخ استعلام محصول
    /// </summary>
    public class InquiryProductReply
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// ردیف استعلام محصول
        /// </summary>
        public int InquiryProductId { get; set; }
        /// <summary>
        /// نوع پاسخ 
        /// </summary>
        public AnswerType AnswerType { get; set; }
        /// <summary>
        /// متن پاسخ
        /// </summary>
        [MaxLength(1024)]
        public string Comment { get; set; }
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
        ///  آی پی ایجاد کننده
        /// </summary>
        [MaxLength(45)]
        public string CreateIp { get; set; }
        /// <summary>
        /// آی پی ویرایش کننده
        /// </summary>
        [MaxLength(45)]
        public string ModifyIp { get; set; }
        /// <summary>
        /// استعلام محصول
        /// </summary>
        public virtual InquiryProduct InquiryProduct { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
       
    }
}
