using KiaGallery.Model.Common;
using KiaGallery.Model.FileContext.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.FileContext.Enum
{
    /// <summary>
    /// وضعیت فایل
    /// </summary>
    [Table(name: "FileStatus", Schema = "enum")]
    public class FileStatus : BaseEnum<FileStatus>
    {
        protected FileStatus(int id, string title, string perKiaGalleryTitle) : base(id, title, perKiaGalleryTitle)
        {
            FileList = new List<File>();
        }

        /// <summary>
        /// هنوز تایید نشده
        /// </summary>
        public readonly static FileStatus NotVerify = new FileStatus(1, "NotVerify", "هنوز تایید نشده");
        /// <summary>
        /// تایید شده
        /// </summary>
        public readonly static FileStatus Verify = new FileStatus(2, "Verify", "تایید شده");
        /// <summary>
        /// حذف شده
        /// </summary>
        public readonly static FileStatus Deleted = new FileStatus(3, "Deleted", "حذف شده");

        /// <summary>
        /// لیست فایل
        /// </summary>
        public virtual List<File> FileList { get; set; }

        /// <summary>
        /// دریافت تمام مقدارهای وضعیت فایل به صورت لیست
        /// </summary>
        /// <returns>لیست وضعیت فایل</returns>
        public new static IEnumerable<FileStatus> GetList()
        {
            return new FileStatus[] {
                NotVerify, Verify, Deleted
            };
        }
    }
}
