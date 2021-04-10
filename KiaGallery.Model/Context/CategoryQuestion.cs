using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// دسته بندی سوالات
    /// </summary>
    public class CategoryQuestion
    {
        /// <summary>
        /// کلاس سازنده
        /// </summary>
        public CategoryQuestion()
        {
            CrmQuestionList = new List<CrmQuestion>();
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
        /// آی پی کاربر ایجاد کننده
        /// </summary>
        [MaxLength(45)]
        public string CreateIp { get; set; }
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
        /// لیست سوالات
        /// </summary>
        public virtual List<CrmQuestion> CrmQuestionList { get; set; }
    }
}
