using KiaGallery.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model
{
    /// <summary>
    /// جدول سرشماری غذا
    /// </summary>
    public class FoodCensus
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// نام غذای ثبت شده توسط مدیر این بخش
        /// </summary>
        public string FoodName { get; set; }
        /// <summary>
        /// تعداد غذاهای ثبت شده توسط کاربران سیستم
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// تعیین روز تعطیل توسط مدیر این بخش
        /// </summary>
        public bool Holiday { get; set; }
        /// <summary>
        /// تعیین غذا با برنج یا بدون برج توسط ادمین
        /// </summary>
        public bool TypeFood { get; set; }
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
        /// لیست افراد ثبت کننده و انتخاب کننده غذاهای ثبت شده
        /// </summary>
        public virtual List<FoodRegistration> FoodRegistrationList { get; set; }

    }
}
