using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    public class GoldBalance
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// زمان خرید و فروش
        /// </summary>
        public string TradeTime { get; set; }
        /// <summary>
        /// نوع خرید و فروش 
        /// </summary>
        public TradeType TradeType { get; set; }
        /// <summary>
        /// نام خریدار یا فروشنده
        /// </summary>
        public string DealerName { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public double Weight { get; set; }
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
