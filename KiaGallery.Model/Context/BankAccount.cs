using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
   public class BankAccount
    {
        public BankAccount()
        {
            BankAccountDetailList = new List<BankAccountDetail>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// عنوان حساب
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// نام بانک
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// تلفن ثابت
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// انتخاب ارگان
        /// </summary>
        public bool Organ { get; set; }
        /// <summary>
        /// شماره کارت
        /// </summary>
        public double CardNumber { get; set; }
        /// <summary>
        /// شماره حساب
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// شبا
        /// </summary>
        public string  Iban { get; set; }
        /// <summary>
        /// شرح در مورد نوع حساب
        /// </summary>
        public string Explanation { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int? CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int? ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime? ModifyDate { get; set; }
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
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual List<BankAccountDetail> BankAccountDetailList { get; set; }
    }
}
