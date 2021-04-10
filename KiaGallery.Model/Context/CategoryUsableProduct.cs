using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// دسته بندی برای محصول مصرفی
    /// </summary>
    public class CategoryUsableProduct
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public CategoryUsableProduct()
        {
            ChildList = new List<CategoryUsableProduct>();
            UsableProductList = new List<UsableProduct>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف والد
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// ردیف فرزند
        /// </summary>
        public int? OrderChild { get; set; }
        /// <summary>
        /// عنوان دسته بندی
        /// </summary>
        [MaxLength(500)]
        public string Title { get; set; }
        /// <summary>
        /// ردیف
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
        /// آی پی کاربر 
        /// </summary>
        [MaxLength(45)]
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
        /// والد
        /// </summary>
        public virtual CategoryUsableProduct Parent { get; set; }
        /// <summary>
        /// لیست محصولات
        /// </summary>
        public virtual List<UsableProduct> UsableProductList { get; set; }
        /// <summary>
        /// لیست دسته بندی های فرزند
        /// </summary>
        public virtual List<CategoryUsableProduct> ChildList { get; set; }
    }
}
