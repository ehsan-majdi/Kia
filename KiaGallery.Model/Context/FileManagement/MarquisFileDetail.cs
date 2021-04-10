using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context.FileManagement
{
    /// <summary>
    /// جزئیات فایل های مارکیز
    /// </summary>
    public class MarquisFileDetail
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف فایل مارکیز
        /// </summary>
        public int MarquisFileId { get; set; }
        /// <summary>
        /// ردیف فایل
        /// </summary>
        [MaxLength(32)]
        public string FileId { get; set; }
        /// <summary>
        /// عنوان فایل
        /// </summary>
        [MaxLength(255)]
        public string FileName { get; set; }

        /// <summary>
        /// فایل مارکیز
        /// </summary>
        public virtual MarquisFile MarquisFile { get; set; }
    }
}
