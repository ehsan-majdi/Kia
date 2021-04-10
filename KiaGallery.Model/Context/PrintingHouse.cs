using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{

    /// <summary>
    /// ایجاد چاپخانه برای محصولات مصرفی 
    /// </summary>
    public class PrintingHouse
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public PrintingHouse()
        {
            UsableProductList = new List<UsableProduct>();
            UserList = new List<User>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ترتیب چاپخانه
        /// </summary>
        public string Rate { get; set; }
        /// <summary>
        /// کد چاپخانه
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// نام چاپخانه
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// مدیریت چاپخانه
        /// </summary>
        public string Management { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// تلفن ثابت
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string Address { get; set; }
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
        /// تاریخ ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی
        /// </summary>
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
        /// لیست محصولات
        /// </summary>
        public virtual List<UsableProduct> UsableProductList { get; set; }
        /// <summary>
        /// لیست کاربران
        /// </summary>
        public virtual List<User> UserList { get; set; }
    }
}
