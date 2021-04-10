using KiaGallery.Model.Context.Salary;
using System;

namespace KiaGallery.Model.Context
{
    public class CardTransaction
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UserInfoId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CardTransactionType Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CardTransactionStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CardTransactionDescription? DescriptionType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CheckoutDescription { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string FactorNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public virtual UserInfo UserInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Person Person { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateIp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ModifyIp { get; set; }
    }
}
