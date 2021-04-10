using System;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// کارت مشتری
    /// </summary>
    public class CustomerCard
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// شناسه شعبه
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// کد مشتری
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// مبلغ
        /// </summary>
        public long Price { get; set; }
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
        /// شعبه
        /// </summary>
        public Branch Branch { get; set; }
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
