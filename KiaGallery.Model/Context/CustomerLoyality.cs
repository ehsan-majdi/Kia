using KiaGallery.Model.Context.Salary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    [Table(name: "CustomerLoyality", Schema = "customerLoyality")]
    public class CustomerLoyality
    {
        public CustomerLoyality()
        {
            CustomerFactorList = new List<CustomerFactor>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? LoyalityCardId { get; set; }
        /// <summary>
        /// ردیف آدرس
        /// </summary>
        public int? CustomerLocationId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? MariageDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CustomerCardLevel CustomerCardLevel { get; set; }
        /// <summary>
        /// اعتبار
        /// </summary>
        public long? Credit { get; set; }
        /// <summary>
        /// اعتبار خرج شده
        /// </summary>
        public long? UsedCredit { get; set; }
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
        /// نام کامل 
        /// </summary>
        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public DateTime Date { get; set; }
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
        /// تاریخ ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual LoyalityCard LoyalityCard { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده 
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        public virtual CustomerLocation CustomerLocation { get; set; }
        /// <summary>
        /// لیست مشتریان وفادار
        /// </summary>
        public virtual List<CustomerFactor> CustomerFactorList { get; set; }
    }
}
