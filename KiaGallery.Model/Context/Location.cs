using KiaGallery.Model.Context.Post;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// موقعیت جغرافیایی
    /// </summary>
    public class Location
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public Location()
        {
            BranchList = new List<Branch>();
            PostItemList = new List<PostItem>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف پدر
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// نوع موقعیت جغرافیایی
        /// </summary>
        public LocationType LocationType { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        [Column("OrderNo")]
        public int Order { get; set; }
        [MaxLength(100)]
        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نام انگلیسی
        /// </summary>
        [MaxLength(100)]
        public string EnglishName { get; set; }

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
        /// پدر
        /// </summary>
        public Location Parent { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// لیست شعبه های ثبت شده برای شهر
        /// </summary>
        public virtual List<Branch> BranchList { get; set; }
        /// <summary>
        /// لیست محصولاتی که به این شهر ارسال شده است
        /// </summary>
        public virtual List<PostItem> PostItemList { get; set; }
        /// <summary>
        /// لیست فرزندان یک موقعیت جغرافیایی
        /// </summary>
        public virtual List<Location> ChildList { get; set; }
    }
}
