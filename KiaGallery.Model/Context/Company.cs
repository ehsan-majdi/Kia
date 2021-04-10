using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// شرکت
    /// </summary>
    public class Company
    {
        public Company()
        {
            GiftShoppingList = new List<Gift.Gift>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        [Column("OrderNo")]
        public int Order { get; set; }
        /// <summary>
        /// نام مستعار
        /// </summary>
        [MaxLength(5)]
        public string Alias { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// نام انگلیسی
        /// </summary>
        [MaxLength(100)]
        public string EnglishName { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        [MaxLength(1000)]
        public string Address { get; set; }
        /// <summary>
        /// آدرس انگلیسی
        /// </summary>
        [MaxLength(1000)]
        public string EnglishAddress { get; set; }
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
        /// آی پی کاربر
        /// </summary>
        [MaxLength(45)]
        public string Ip { get; set; }
        /// <summary>
        /// لیست گیفت خریداری شده
        /// </summary>
        public virtual List<Gift.Gift> GiftShoppingList { get; set; }
    }
}
