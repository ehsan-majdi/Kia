using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.Post
{
    /// <summary>
    /// بسته های ارسال شده توسط پست
    /// </summary>
    [Table(name: "PostItem", Schema = "post")]
    public class PostItem
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شهر
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// شماره فاکتور مارکیز
        /// </summary>
        [MaxLength(50)]
        public string InvoiceNo { get; set; }
        /// <summary>
        /// تعداد قطعه طلا
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public float Weight { get; set; }
        /// <summary>
        /// ثبت نام شده توسط کاربر 
        /// </summary>
        public string SubmitUser { get; set; }
        /// <summary>
        /// قیمت
        /// </summary>
        public long Price { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime? SubmitDate { get; set; }
        /// <summary>
        /// تاریخ تحویل به پست
        /// </summary>
        public DateTime? PostDate { get; set; }
        /// <summary>
        /// نام مشتری
        /// </summary>
        [MaxLength(100)]
        public string Customer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? Sex { get; set; }
        /// <summary>
        /// تلفن ثابت
        /// </summary>
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// تلفن همراه
        /// </summary>
        [MaxLength(15)]
        public string MobileNumber { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        [MaxLength(1500)]
        public string Address { get; set; }
        /// <summary>
        /// کد پستی
        /// </summary>
        [MaxLength(10)]
        public string PostalCode { get; set; }
        /// <summary>
        /// کد رهگیری
        /// </summary>
        public string TrackingCode { get; set; }
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
        /// شهر
        /// </summary>
        public virtual Location City { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
    }
}
