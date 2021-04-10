using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Common
{
    /// <summary>
    /// مقدار پایه ای برای داده های شمارشی
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseEnum<T> where T : BaseEnum<T>
    {
        public BaseEnum(int id, string title, string persianTitle)
        {
            Id = id;
            Title = title;
            PersianTitle = persianTitle;
        }

        /// <summary>
        /// ردیف
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        /// <summary>
        /// عنوان فارسی
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string PersianTitle { get; set; }

        /// <summary>
        /// دریافت تمام مقدارها به صورت لیست
        /// </summary>
        /// <returns>لیست مقدارها</returns>
        public static IEnumerable<T> GetList()
        {
            throw new Exception("not override method GetList() from BaseEnum");
        }

    }
}
