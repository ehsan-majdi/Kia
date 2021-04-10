using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.Model.Context.Salary
{
    /// <summary>
    /// فایل پرسنل
    /// </summary>
    public class PersonFile
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// دسته بندی
        /// </summary>
        public PersonFileCategory Category { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        [MaxLength(250)]
        public string Title { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        [MaxLength(250)]
        public string FileName { get; set; }
        /// <summary>
        /// نوع فایل
        /// </summary>
        public PersonFileType FileType { get; set; }
        /// <summary>
        /// پرسنل
        /// </summary>
        public virtual Person Person { get; set; }
    }
}
