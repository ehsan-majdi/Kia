using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
  public class BranchGold
  {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// قیمت
        /// </summary>
        public long Price { get; set; }
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
        /// آیپی
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
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
