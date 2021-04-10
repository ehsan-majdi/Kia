using KiaGallery.Model.FileContext.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.FileContext.Entity
{
    /// <summary>
    /// مدل فایل
    /// </summary>
    public class File
    {
        /// <summary>
        /// ردیف
        /// </summary>
        [Key]
        [MaxLength(32)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        [MaxLength(255)]
        public string FileName { get; set; }
        /// <summary>
        /// پسوند
        /// </summary>
        [MaxLength(10)]
        public string Extention { get; set; }
        /// <summary>
        /// نوع فایل
        /// </summary>
        [MaxLength(50)]
        public string MimeType { get; set; }
        /// <summary>
        /// سایز
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// دیتای فایل
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// ردیف وضعیت
        /// </summary>
        public int StatusId { get; set; }
        /// <summary>
        /// اطلاعات اضافه
        /// </summary>
        public string Appendix { get; set; }
        /// <summary>
        /// دسترسی هایی که می توانند این فایل را مشاهده کنند
        /// </summary>
        [MaxLength(1000)]
        public string Roles { get; set; }
        /// <summary>
        /// تعداد خوانده شدن فایل
        /// </summary>
        public int Download { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int? CreateUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// آی پی ایجاد کننده
        /// </summary>
        [Required]
        [MaxLength(48)]
        public string CreateIp { get; set; }

        /// <summary>
        /// وضعیت
        /// </summary>
        public virtual FileStatus Status { get; set; }
    }
}
