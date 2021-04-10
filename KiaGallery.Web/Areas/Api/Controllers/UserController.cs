using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Areas.Api.Models;
using KiaGallery.Web.Controllers;
using KiaGallery.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Areas.Api.Controllers
{
    /// <summary>
    /// کنترلر کاربران
    /// </summary>
    public class UserController : BaseController
    {
        /// <summary>
        /// ورود کاربر به سیستم
        /// </summary>
        /// <param name="username">نام کاربری</param>
        /// <param name="password">گذرواژه</param>
        /// <returns>نتیجه ورود کاربر</returns>
        [AllowAnonymous]
        public JsonResult AppLogin(string username, string password)
        {
            return InternalLogin(username, password);
        }

        /// <summary>
        /// ورود کاربر به سیستم
        /// </summary>
        /// <param name="username">نام کاربری</param>
        /// <param name="password">گذرواژه</param>
        /// <returns>نتیجه ورود کاربر</returns>
        [AllowAnonymous]
        public JsonResult Login(string username, string password)
        {
            return InternalLogin(username, password, false);
        }

        /// <summary>
        /// متد ورود به برنامه که از اکشن های دیگر صدا زده می شود.
        /// </summary>
        /// <param name="username">نام کاربری</param>
        /// <param name="password">گذرواژه</param>
        /// <param name="needToken">نیاز به صدور توکن هست یا خیر</param>
        /// <returns>نتیجه ورود کاربر</returns>
        private JsonResult InternalLogin(string username, string password, bool needToken = true)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var userdata = db.User.Where(x => x.Username == username).SingleOrDefault();
                    if (userdata == null)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "نام کاربری یا گذرواژه اشتباه است"
                        };
                    }
                    else
                    {
                        if (PasswordTools.CheckPassword(password, userdata.Password, userdata.Salt))
                        {
                            if (userdata.Active)
                            {
                                string token = "";
                                if (needToken)
                                {
                                    token = Auth.GenerateToken(userdata.Id, Request.UserHostAddress);
                                    db.UserToken.Add(new UserToken()
                                    {
                                        UserId = userdata.Id,
                                        AuthoritarianToken = token,
                                        CreatedDateTime = DateTime.Now,
                                        ExpiredDateTime = DateTime.Now.AddHours(3),
                                        CreatedIp = Request.UserHostAddress
                                    });
                                }

                                db.SaveChanges();
                                response = new Response()
                                {
                                    status = 200,
                                    data = new
                                    {
                                        id = userdata.Id,
                                        username = userdata.Username,
                                        firstName = userdata.FirstName,
                                        lastName = userdata.LastName,
                                        imageLink = string.IsNullOrEmpty(userdata.FileName) ? "" : "/upload/user/",
                                        branchName = userdata.Branch.Name,
                                        token = token
                                    }
                                };
                            }
                            else
                            {
                                response = new Response()
                                {
                                    status = 500,
                                    message = "حساب کاربری شما غیرفعال است."
                                };
                            }
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 500,
                                message = "نام کاربری یا گذرواژه اشتباه است"
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// مشخصات کاربر
        /// </summary>
        /// <returns>اطلاعات کاربر</returns>
        public JsonResult UserDetail()
        {
            Response response;
            GetAuthentiatedUserViewModel user = GetAuthenticatedUser();
            try
            {
                response = new Response()
                {
                    status = 200,
                    data = new UserDetailViewModel
                    {
                        id = user.Id,
                        username = user.Username,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        imageLink = string.IsNullOrEmpty(user.FileName) ? "" : "/upload/user/",
                        branchName = user.BranchName,
                    }
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// خارج شدن کاربر از سیستم
        /// </summary>
        /// <param name="token">توکن اعتبار سنجی کاربر</param>
        /// <returns>نتیجه خروج کاربر</returns>
        [Authorize]
        public JsonResult Logout(string token)
        {
            Response response;
            UserToken usertoken;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    usertoken = db.UserToken.Single(x => x.AuthoritarianToken == token);
                    usertoken.ExpiredDateTime = DateTime.Now;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
    }
}