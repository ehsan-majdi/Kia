using KiaGallery.Model.Common;
using KiaGallery.Model.FileContext.Enum;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace KiaGallery.Model.FileContext
{
    /// <summary>
    /// کلاس کمکی برای کار کردن با دیتابیس فایل
    /// </summary>
    public class KiaGalleryFileContextHelper
    {
        /// <summary>
        /// خواندن یک فایل
        /// </summary>
        /// <param name="id">ردیف فایل</param>
        /// <param name="fileName">نام فایل</param>
        /// <returns>شی فایل</returns>
        public static Entity.File GetFile(string id, string fileName)
        {
            using (var db = new KiaGalleryFileContext())
            {
                return db.File.Single(x => x.Id == id && x.FileName == fileName && x.StatusId != FileStatus.Deleted.Id);
            }
        }

        /// <summary>
        /// تایید کردن فایل آپلود شده
        /// </summary>
        /// <param name="fileId">شناسه فایل</param>
        /// <param name="fileName">نام فایل</param>
        public static void VerifyFile(string fileId, string fileName)
        {
            using (var db = new KiaGalleryFileContext())
            {
                var entity = db.File.Single(x => x.Id == fileId && x.FileName == fileName);
                entity.StatusId = FileStatus.Verify.Id;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// تایید کردن فایل آپلود شده
        /// </summary>
        /// <param name="fileId">شناسه فایل</param>
        /// <param name="fileName">نام فایل</param>
        public static void NotVerifyFile(string fileId, string fileName)
        {
            using (var db = new KiaGalleryFileContext())
            {
                var entity = db.File.Single(x => x.Id == fileId && x.FileName == fileName);
                entity.StatusId = FileStatus.NotVerify.Id;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// حذف کردن فایل آپلود شده
        /// </summary>
        /// <param name="fileId">شناسه فایل</param>
        /// <param name="fileName">نام فایل</param>
        public static void DeleteFile(string fileId, string fileName, int? userId, params string[] roles)
        {
            using (var db = new KiaGalleryFileContext())
            {
                var entity = db.File.Single(x => x.Id == fileId && x.FileName == fileName);
                entity.StatusId = FileStatus.Deleted.Id;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// ذخیره فایل داخل دیتابیس
        /// </summary>
        /// <param name="file">فایل ارسالی در وب</param>
        /// <param name="system">سیستم ذخیره کننده فایل</param>
        /// <param name="userId">ردیف کاربر</param>
        /// <param name="ip">آی پی ارسال کننده درخواست</param>
        /// <returns>نتیجه ذخیره فایل</returns>
        public static SaveFile Save(HttpPostedFileBase file, int? userId, string ip, params string[] roles)
        {
            if (file != null && file.ContentLength > 0)
            {
                if (roles != null && roles.Length > 0)
                {
                    roles = roles.Select(x => string.Format("[{0}]", x)).ToArray();
                }

                using (var db = new KiaGalleryFileContext())
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fileBytes = ConvertToByte(file);

                    var entity = new Entity.File()
                    {
                        Id = Guid.NewGuid().ToString().Replace("-", ""),
                        FileName = fileName,
                        Extention = Path.GetExtension(fileName),
                        MimeType = file.ContentType,
                        Length = fileBytes.LongLength,
                        Data = fileBytes,
                        StatusId = FileStatus.NotVerify.Id,
                        Roles = string.Join(",", roles),
                        Download = 0,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now,
                        CreateIp = ip
                    };

                    db.File.Add(entity);
                    db.SaveChanges();

                    var data = new SaveFile
                    {
                        id = entity.Id,
                        length = entity.Length,
                        fileName = entity.FileName,
                        link = string.Format("/upload/file/{0}/{1}", entity.Id, entity.FileName)
                    };
                    return data;
                }
            }
            return null;
        }

        /// <summary>
        /// تبدیل فایل پست شده به سرور به آرایه بایت
        /// </summary>
        /// <param name="file">فایل پست شده</param>
        /// <returns>آرایه بایت</returns>
        private static byte[] ConvertToByte(HttpPostedFileBase file)
        {
            byte[] data;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return data;
        }

    }
}
