using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    public class WorkShopGold
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف کارگاه
        /// </summary>
        public int WorkshopId { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public double? Weight { get; set; }
        /// <summary>
        /// قیمت طلای خریداری شده
        /// </summary>
        public double? BoughtGoldPrice { get; set; }
        /// <summary>
        /// مظنه
        /// </summary>
        public double? GoldRate { get; set; }
        /// <summary>
        /// نوع انتقال 
        /// </summary>
        public RemittanceType RemittanceType { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime? Date { get; set; }
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
        /// کارگاه
        /// </summary>
        public virtual Workshop Workshop { get; set; }
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
