using KiaGallery.Model.FileContext.Enum;
using System.Data.Entity;

namespace KiaGallery.Model.FileContext
{
    /// <summary>
    /// سازنده دیتابیس فایل
    /// از این کلاس برای درج مقادیر پیش فرض به جدول های مورد نظر استفاده می شود.
    /// </summary>
    public class KiaGalleryFileContextInitializer : DropCreateDatabaseIfModelChanges<KiaGalleryFileContext>
    {
        /// <summary>
        /// درج مقادیر مورد نیاز در دیتابیس به هنگام ساخت دیتابیس
        /// </summary>
        /// <param name="context">شی دیتابیس</param>
        protected override void Seed(KiaGalleryFileContext context)
        {
            foreach (var item in FileStatus.GetList())
            {
                context.FileStatus.Add(item);
            }

            base.Seed(context);
        }
    }
}
