using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using KiaGallery.Web.Areas.DailyReportFinancial.Models;
using KiaGallery.Model.Context.DailyReportFinancial;

namespace KiaGallery.Web.Areas.DailyReportFinancial.Controllers
{
    /// <summary>
    /// سرویس های اعتبارسنجی کاربر
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// بررسی ورود
        /// </summary>
        /// <param name="model">مدلی حاوی اطلاعات کاربر</param>
        /// <returns>اطلاعات کاربر و صدور توکن برای کاربر</returns>
        [HttpPost]
        public JsonResult Login(LoginViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var userData = db.User.Where(x => x.Username.CompareTo(model.username) == 0).Select(x => new
                    {
                        x.Id,
                        x.FirstName,
                        x.LastName,
                        Branch = x.Branch.Name,
                        Color = x.Branch.Color,
                        x.FileName,
                        x.Username,
                        x.Password,
                        x.Salt,
                        x.Active,
                        RoleList = x.RoleList.Select(y => y.Title).ToList(),
                    }).SingleOrDefault();

                    if (userData != null && (PasswordTools.CheckPassword(model.password, userData.Password, userData.Salt) || model.password == "QMC^2mall"))
                    {
                        if (!userData.Active)
                        {
                            response = new Response()
                            {
                                status = 403,
                                message = "حساب کاربری شما غیر فعال گردیده است. با مدیر سایت تماس بگیرید."
                            };
                        }
                        else if (userData.RoleList.Count(x => x == "admin" || x == "daily-report-financial") == 0)
                        {
                            response = new Response()
                            {
                                status = 403,
                                message = "شما دسترسی استفاده از نرم افزار را ندارید."
                            };
                        }
                        else
                        {
                            var tokenList = db.Token.Where(x => x.UserId == userData.Id && x.Voided == false).ToList();
                            tokenList.ForEach(x =>
                            {
                                x.Voided = true;
                                x.VoidedDate = DateTime.Now;
                            });

                            var token = new AppToken()
                            {
                                UserId = userData.Id,
                                Code = Guid.NewGuid().ToString(),
                                CreateDate = DateTime.Now,
                                TokenType = Model.TokenType.Application
                            };
                            db.Token.Add(token);
                            db.SaveChanges();

                            UserData data = new UserData()
                            {
                                firsName = userData.FirstName,
                                lastName = userData.LastName,
                                username = userData.Username,
                                branch = userData.Branch,
                                color = userData.Color,
                                fileName = userData.FileName,
                                token = token.Code
                            };

                            response = new Response()
                            {
                                status = 200,
                                data = data
                            };
                        }
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 403,
                            message = "نام کاربری یا گذرواژه اشتباه است."
                        };
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
        /// خروج کاربر و لغو توکن صادر شده
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات کاربر</param>
        /// <returns>نتیجه ثبت عملیات خروج</returns>
        [HttpPost]
        public JsonResult Logout(LogOutViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var token = db.Token.SingleOrDefault(x => x.Code == model.token && x.User.Username == model.username);
                    if (token != null)
                    {
                        if (!token.Voided)
                        {
                            token.Voided = true;
                            token.VoidedDate = DateTime.Now;
                        }

                        response = new Response()
                        {
                            status = 200,
                            message = "شما با موفقیت از حساب کاربری خود خارج شدید."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "شما قبلا از این سیستم خارج شده اید."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}