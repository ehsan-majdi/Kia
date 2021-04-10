using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر پایه که همه کنترلر ها از آن ارث بری میکنند
    /// </summary>
    [Authorize]
    public class BaseController : Controller
    {
        /// <summary>
        /// گرفتن اطلاعات کاربر لاگین شده
        /// </summary>
        /// <returns>اطلاعات کاربر لاگین شده</returns>
        public static GetAuthentiatedUserViewModel GetAuthenticatedUser()
        {

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName);
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                return JsonConvert.DeserializeObject<GetAuthentiatedUserViewModel>(ticket.Name);
            }
            else
            {
                var Request = System.Web.HttpContext.Current.Request;
                if (Request != null && Request.Headers["Authorization"] != null)
                {
                    var token = Request.Headers["Authorization"];
                    var userId = Auth.CheckToken(token);
                    if (userId != null)
                    {
                        using (var db = new KiaGalleryContext())
                        {
                            var user = db.UserToken.Where(x => x.AuthoritarianToken == token && x.ExpiredDateTime >= DateTime.Now).Select(x => x.User).Select(x => new GetAuthentiatedUserViewModel()
                            {
                                Id = x.Id,
                                BranchId = x.BranchId,
                                WorkshopId = x.WorkshopId,
                                PrintingHouseId=x.PrintingHouseId,
                                FirstName = x.FirstName,
                                LastName = x.LastName,
                                Color = x.Color,
                                FileName = x.FileName,
                                PhoneNumber = x.PhoneNumber,
                                Username = x.Username,
                                Salt = x.Salt,
                                Password = x.Password,
                                UserType = x.UserType,
                                Active = x.Active,
                                BranchType = x.Branch.BranchType,
                                BranchName = x.Branch.Name,
                                Ip = x.Ip,
                                UserPlace = x.UserPlace
                            }).Single();
                            return user;
                        }
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// گرفتن ردیف کاربر لاگین شده
        /// </summary>
        /// <returns>ردیف کاربر لاگین شده</returns>
        public int GetAuthenticatedUserId()
        {
            return GetAuthenticatedUser().Id;
        }

        /// <summary>
        /// آپلود فایل در سرور
        /// </summary>
        /// <param name="path">آدرسی که فایل آپلود شود</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        public JsonResult Upload(string path)
        {
            Response response;
            try
            {
                string serverPath = Server.MapPath("~/Upload/" + path);
                HttpPostedFileBase hpf = Request.Files[0];

                if (hpf.ContentLength == 0)
                    throw new Exception("File length can't be equal to zero");

                string fileName = Path.GetFileName(hpf.FileName);
                string savedFileName = Path.Combine(serverPath, fileName);

                if (System.IO.File.Exists(savedFileName))
                {
                    Random random = new Random();
                    string prefix = random.Next(1000, 9999).ToString() + "-";
                    fileName = prefix + fileName;
                }
                savedFileName = Path.Combine(serverPath, fileName);

                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }

                hpf.SaveAs(savedFileName);

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        name = fileName,
                        length = hpf.ContentLength,
                        type = hpf.ContentType
                    },
                    message = "بارگذاری فایل با موفقیت انجام شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}