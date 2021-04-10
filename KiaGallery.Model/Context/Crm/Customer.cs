using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// مشتری
    /// </summary>
    [Table("Custromer", Schema = "crm")]
    public class Customer
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public Customer()
        {

        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// بارکد کارت
        /// </summary>
        [MaxLength(100)]
        public string Barcode { get; set; }
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
        /// کد ملی
        /// </summary>
        [MaxLength(15)]
        public string NationalityCode { get; set; }
        /// <summary>
        /// تلفن همراه
        /// </summary>
        [MaxLength(20)]
        public string MobileNo { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
        public CrmSex Sex { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// تاریخ ازدواج
        /// </summary>
        public DateTime? WeddingDate { get; set; }
        /// <summary>
        /// ترازمالی
        /// </summary>
        public int Balance { get; set; }

        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }

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
        /// آی پی کاربر ایجاد کننده
        /// </summary>
        [MaxLength(45)]
        public string CreateIp { get; set; }
        /// <summary>
        /// آی پی کاربر ویرایش کننده
        /// </summary>
        [MaxLength(45)]
        public string ModifyIp { get; set; }

        /// <summary>
        /// نام کامل مشتری
        /// </summary>
        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }

        /// <summary>
        /// لیست فاکتور های ثبت شده برای کاربر
        /// </summary>
        public virtual List<CrmInvoice> InvoiceList { get; set; }
    }
}
