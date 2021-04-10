using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// دسته بندی پیام ها
    /// </summary>
    public class SmsCategory
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public SmsCategory()
        {
            SmsTextList = new List<SmsText>();
            CreateMessageList = new List<CreateMessage>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// رنگ
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// نوع پیام
        /// </summary>
        public SmsCategoryType CategoryType { get; set; }
        /// <summary>
        /// مقدار تشریحی
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// پیام آزاد
        /// </summary>
        public string FreeMessage { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int Order { get; set; }
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
        /// لیست اس ام اس
        /// </summary>
        public virtual List<SmsText> SmsTextList { get; set; }
        /// <summary>
        /// لیست ایجاد پیام
        /// </summary>
        public virtual List<CreateMessage> CreateMessageList { get; set; }
    }
}
