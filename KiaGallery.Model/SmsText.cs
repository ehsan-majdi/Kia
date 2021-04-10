using KiaGallery.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model
{
    public class SmsText
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف دسته بندی اس ام اس
        /// </summary>
        public int SmsCategoryId { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// متن پیام
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// کدپیام
        /// </summary>
        public string UrlKey { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int Order { get; set; }
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
        /// دسته بندی اس ام اس
        /// </summary>
        public virtual SmsCategory SmsCategory { get; set; }
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
