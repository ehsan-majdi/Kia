using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// جدول ثبت غذا
    /// </summary>
    public class FoodRegistration
    {
        /// <summary>
        /// ردیف 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف غذا
        /// </summary>
        public int FoodCensusId { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// وضعیت ثبت غذا توسط کاربران لاگین شده در سیستم
        /// </summary>
        public FoodStatus? FoodStatus { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// پیش غذا
        /// </summary>
        public bool Appertizer { get; set; }
        /// <summary>
        /// غذا با برنج
        /// </summary>
        public bool Food { get; set; }
        /// <summary>
        /// غذا بدون برنج 
        /// </summary>
        public bool FoodWithoutRice { get; set; }
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
        /// کاربر
        /// </summary>
        public virtual User User { get; set; }
        /// <summary>
        /// سرشماری غذا
        /// </summary>
        public virtual FoodCensus FoodCensus { get; set; }
    }
}
