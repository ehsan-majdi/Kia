using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    public class CardChargeAggreement
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
            public CardChargeAggreementStatus Status { get; set; }
        public string Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
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
