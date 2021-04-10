using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// دسترسی کاربران
    /// </summary>
    public class Role
    {

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// عنوان دسترسی
        /// </summary>
        [MaxLength(100)]
        public string Title { get; set; }
        
        /// <summary>
        /// کاربر
        /// </summary>
        public virtual User User { get; set; }
    }
}
