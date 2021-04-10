using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// کلاس دسته بندی گزارش موجودی طلا مثل کارگاه که کارگاه شامل افرادی می باشند که طلا نزد آنها نیز موجود می باشد
    /// </summary>
    public class CategoryInventoryReportMember
    {
        /// <summary>
        /// کلاس سازنده
        /// </summary>
        public CategoryInventoryReportMember()
        {
            InventoryDetailList = new List<InventoryDetail>();
        }
        /// <summary>
        /// ردیف دسته بندی
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// عنوان دسته بندی
        /// </summary>
        [MaxLength(500)]
        public string Title { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }
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
        /// آی پی
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// آی پی کاربر ویرایش کننده
        /// </summary>
        [MaxLength(45)]
        public string ModifyIp { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// لیست افرادی که طلا نزد آنها موجود می باشد جهت استعلام موجود طلا 
        /// </summary>
        public virtual List<InventoryDetail> InventoryDetailList { get; set; }
    }
}
