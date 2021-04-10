using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// 
    /// </summary>
    public class UserInfo
    {
        //public UserInfo()
        //{
        //    UserCardTransactionList = new List<UserCardTransaction>();
        //}
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// المثنی
        /// </summary>
        public bool Replica { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Gift { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? Sex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NationalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CarriorCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double DiscountPercent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double PointPercent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double? Point { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long KiaPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? MarriedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? BirthDate { get; set; }
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
        //public virtual List<UserCardTransaction> UserCardTransactionList { get; set; }
    }
}
