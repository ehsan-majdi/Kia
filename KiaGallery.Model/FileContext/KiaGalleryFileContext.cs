using KiaGallery.Model.FileContext.Entity;
using KiaGallery.Model.FileContext.Enum;
using System.Data.Entity;

namespace KiaGallery.Model.FileContext
{
    /// <summary>
    /// مدیریت اتصال به پایگاه داده فایل 
    /// </summary>
    public class KiaGalleryFileContext : DbContext
    {
        public KiaGalleryFileContext() : base("KiaGalleryFileContext")
        {
            Database.SetInitializer(new KiaGalleryFileContextInitializer());
        }

        /// <summary>
        /// جدول فایل
        /// </summary>
        public DbSet<File> File { get; set; }

        /// <summary>
        /// جدول وضعیت فایل
        /// </summary>
        public DbSet<FileStatus> FileStatus { get; set; }

        /// <summary>
        /// ساختار رابطه بین موجودیت های دیتابیس
        /// </summary>
        /// <param name="model">سازنده مدل دیتابیس</param>
        protected override void OnModelCreating(DbModelBuilder model)
        {
            model.Entity<FileStatus>().HasMany(x => x.FileList).WithRequired(x => x.Status).HasForeignKey(x => x.StatusId).WillCascadeOnDelete(false);

            base.OnModelCreating(model);
        }

    }
}
