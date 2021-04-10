using KiaGallery.Common;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر مربوط به کنترل فرآیند آپلود
    /// </summary>
    public class UploadController : BaseController
    {
        /// <summary>
        /// تغییر سایز تصویر و دخیره در کش
        /// </summary>
        /// <param name="type">نوع فایل درخواستی که در کدام پوشه به دنبال فایل بگردد</param>
        /// <param name="fileName">نام فایل</param>
        /// <param name="size">ابعاد خواسته شده</param>
        /// <returns>فایل تصویر تغییر سایز داده شده</returns>
        [AllowAnonymous]
        public ActionResult ResizeImage(string type, string fileName, string size)
        {
            try
            {
                string serverPath = Server.MapPath("~/upload/" + type);
                string serverTempPath = Server.MapPath("~/Temp/" + type);

                string filePath = Path.Combine(serverTempPath, string.Format("{0}-{1}", size, fileName));

                if (!System.IO.File.Exists(filePath))
                {
                    string savedThumbFileName = Path.Combine(serverTempPath, string.Format("{0}-{1}", size, fileName));
                    if (!Directory.Exists(serverTempPath))
                    {
                        Directory.CreateDirectory(serverTempPath);
                    }

                    var sizes = size.Split('x').Select(x => int.Parse(x)).ToArray();
                    if (sizes.Length == 2)
                    {
                        Image image = Image.FromFile(Path.Combine(serverPath, fileName));
                        int width = image.Width > sizes[0] ? sizes[0] : image.Width;
                        int height = image.Height > sizes[1] ? sizes[1] : image.Height;
                        if (width != height)
                        {
                            if (width > height) width = height;
                            else height = width;
                        }
                        Image croppedImage = BitmapUtility.FixedSize(image, sizes[0], sizes[1], true);

                        var byteImages = BitmapUtility.ImageToByteArray(croppedImage);
                        System.IO.File.WriteAllBytes(savedThumbFileName, byteImages);
                    }
                }

                if (!string.IsNullOrEmpty(Request.Headers["If-Modified-Since"]))
                {
                    Response.StatusCode = 304;
                    Response.StatusDescription = "Not Modified";
                    Response.AddHeader("Content-Length", "0");
                    return Content(String.Empty);
                }

                string lastModified = DateTime.Now.ToUniversalTime().ToString("R");
                Response.Cache.SetCacheability(HttpCacheability.Public);
                Response.Cache.SetLastModified(DateTime.Now.ToUniversalTime());
                Response.AddHeader("Last-Modified", lastModified);
                return File(filePath, MimeMapping.GetMimeMapping(fileName), Server.UrlEncode(Path.GetFileName(filePath)));
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return null;
            }
        }
    }
}