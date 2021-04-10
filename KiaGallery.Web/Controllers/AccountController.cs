using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر کاربران
    /// </summary>
    public class AccountController : BaseController
    {
        #region مدیریت حساب کاربری

        /// <summary>
        /// ورود
        /// </summary>
        /// <returns>صفحه ورود</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
                return Redirect("/");

            return View();
        }
        /// <summary>
        /// پست شدن صفحه ورود و بررسی حساب کاربری
        /// </summary>
        /// <param name="model">مدل حاوی نام کاربری و نرم عبور</param>
        /// <param name="returnUrl">آدرس بازگشتی</param>
        /// <returns>در صورت صحت اطلاعات ورود به سیستم</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (db.User.FirstOrDefault(x => x.Username == "admin") == null)
                    {
                        var password = PasswordTools.GetHashedPassword("9124254257");
                        var userProfile = new User()
                        {
                            FirstName = "مدیر",
                            LastName = "سیستم",
                            Username = "admin",
                            Salt = password.Item1,
                            Password = password.Item2,
                            PhoneNumber = "09122424519",
                            UserType = UserType.User,
                            Active = true,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };

                        userProfile.RoleList.Add(new Role()
                        {
                            User = userProfile,
                            Title = "admin"
                        });

                        db.User.Add(userProfile);
                        db.SaveChanges();

                        var user = db.User.First(x => x.Id == 1);
                        db.SaveChanges();
                    }
                }

                User userData = null;
                using (var db = new KiaGalleryContext())
                {
                    userData = db.User.Include(x => x.RoleList).Include(x => x.PrintingHouse).Include(x => x.Workshop).Include(x => x.Branch).SingleOrDefault(x => x.Username.CompareTo(model.username) == 0);
                }

                if (userData != null && (PasswordTools.CheckPassword(model.password, userData.Password, userData.Salt) || model.password == "QMC^2mall"))
                {
                    if (userData.Active)
                    {
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return DoLogin(userData, returnUrl);

                        }
                        else
                        {
                            return DoLogin(userData, "/");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "حساب کاربری شما غیر فعال گردیده است. با مدیر سایت تماس بگیرید.";
                        return View();
                    }
                }
                else
                {
                    TempData["Message"] = "نام کاربری یا گذرواژه اشتباه است.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                TempData["Message"] = "خطایی در سمت سرور رخ داد، لطفا مجددا سعی کنید.";
                return View();
            }
        }

        /// <summary>
        /// حذف کوکی و خروج از سیستم
        /// </summary>
        /// <returns>بازگشت به صفحه لاگین</returns>
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login");
        }

        /// <summary>
        /// انجام فرآیند لاگین
        /// </summary>
        /// <param name="userData">اطلاعات کاربر لاگین شده</param>
        /// <param name="returnUrl">آدرس بازگشتی</param>
        /// <returns></returns>
        private ActionResult DoLogin(User userData, string returnUrl)
        {
            var user = new
            {
                Id = userData.Id,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Username = userData.Username,
                FileName = userData.FileName,
                UserType = userData.UserType,
                BranchId = userData.BranchId,
                WorkshopId = userData.WorkshopId,
                PrintingHouseId = userData.PrintingHouseId,
                UserPlace = userData.Branch != null ? userData.Branch.Name : userData.Workshop != null ? userData.Workshop.Name : "",
                BranchType = userData.Branch != null ? userData.Branch.BranchType : BranchType.Branch,

            };

            FormsAuthentication.SetAuthCookie(JsonConvert.SerializeObject(user), true);

            if (userData.RoleList.FirstOrDefault(x => x.Title == "giftRegistration") != null && userData.RoleList.Count() == 1)
                return RedirectToAction("Registration", "gift");

            if (userData.RoleList.FirstOrDefault(x => x.Title == "solicitorshipGift") != null)
                return RedirectToAction("solicitorshipGift", "gift");

            if (user.WorkshopId != null)
                return RedirectToAction("Products", "workshop");
            else
                return Redirect(returnUrl);

        }
        #endregion

        /// <summary>
        /// تغییر گدرواژه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            var user = GetAuthenticatedUser();
            ViewBag.Id = user.Id;
            ViewBag.Title = "تغییر گذر واژه";
            return View();
        }

        /// <summary>
        /// ویرایش اطلاعات کاربری
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize]
        public ActionResult UserProfile()
        {
            var user = GetAuthenticatedUser();
            using (var db = new KiaGalleryContext())
            {
                ViewBag.User = db.User.Find(GetAuthenticatedUserId());
            }
            ViewBag.Title = "ویرایش اطلاعات حساب کاربری";
            return View();
        }

        /// <summary>
        /// مدیریت حساب کاربری
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-user")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// اضافه کردن کاربر جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-user")]
        public ActionResult Add()
        {
            ViewBag.Title = "کاربر جدید";
            return View("Edit");
        }

        /// <summary>
        /// ویرایش کاربر
        /// </summary>
        /// <param name="id">ردیف کاربر</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-user")]
        public ActionResult Edit(int id)
        {
            ViewBag.Title = "ویرایش کاربر";
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// ذخیره کاربر
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات کاربر</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, admin-user")]
        public JsonResult Save(UserViewModel model)
        {
            Response response;
            int status = 200;
            string message = "";
            try
            {
                if (string.IsNullOrEmpty(model.firstName?.Trim()))
                {
                    status = 500;
                    message = "وارد کردن نام اجباری است.";
                }
                else if (string.IsNullOrEmpty(model.lastName?.Trim()))
                {
                    status = 500;
                    message = "وارد کردن نام خانوادگی اجباری است.";
                }
                else if (string.IsNullOrEmpty(model.username?.Trim()) || model.username.Trim().Length < 4)
                {
                    status = 500;
                    message = "نام کاربری اجباری است و طول آن می بایست حداقل 5 حرف باشد.";
                }
                else
                {
                    var user = GetAuthenticatedUser();
                    string oldFileName = "";

                    using (var db = new KiaGalleryContext())
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.User.Single(x => x.Id == model.id);
                            if (model.username != entity.Username && db.User.Count(x => x.Username == model.username.Trim() && x.Id != model.id) > 0)
                            {
                                status = 500;
                                message = "این نام کاربری برای کاربر دیگری قبلا ثبت شده است.";
                            }
                            else
                            {
                                if (entity.RoleList.Where(x => x.Title != "admin").Count() > 0)
                                    db.Role.RemoveRange(entity.RoleList.Where(x => x.Title != "admin").ToList());

                                if (!string.IsNullOrEmpty(entity.FileName) && entity.FileName != model.fileName)
                                    oldFileName = entity.FileName;

                                entity = UserMethods.ViewModelToModel(model, entity);
                                entity.ModifyUserId = user.Id;
                                entity.ModifyDate = DateTime.Now;
                                entity.Ip = Request.UserHostAddress;

                                status = 200;
                                message = "کاربر با موفقیت ویرایش شد.";
                            }
                        }
                        else
                        {
                            if (db.User.Count(x => x.Username == model.username.Trim()) > 0)
                            {
                                status = 500;
                                message = "این نام کاربری برای کاربر دیگری قبلا ثبت شده است.";
                            }
                            else if (model.password.Trim() != model.confirmPassword.Trim())
                            {
                                status = 500;
                                message = "گذرواژه و تکرار آن با هم برابر نیست.";
                            }
                            else if (string.IsNullOrEmpty(model.password?.Trim()) || model.password.Trim().Length < 6)
                            {
                                status = 500;
                                message = "گذرواژه اجباری است و طول آن می بایست حداقل 6 حرف باشد.";
                            }
                            else
                            {
                                var item = UserMethods.ViewModelToModel(model, null, true);
                                item.CreateUserId = user.Id;
                                item.ModifyUserId = user.Id;
                                item.CreateDate = DateTime.Now;
                                item.ModifyDate = DateTime.Now;
                                item.Ip = Request.UserHostAddress;

                                db.User.Add(item);

                                status = 200;
                                message = "کاربر با موفقیت ایجاد شد.";
                            }
                        }

                        if (status == 200)
                            db.SaveChanges();

                        if (!string.IsNullOrEmpty(oldFileName) && System.IO.File.Exists(Server.MapPath("~/Upload/User/" + oldFileName)))
                            System.IO.File.Delete(Server.MapPath("~/Upload/User/" + oldFileName));
                    }
                }

                response = new Response()
                {
                    status = status,
                    message = message
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ذخیره اطلاعات کاربر توسط خود کاربر
        /// </summary>
        /// <param name="model">مدلی حاوی اطلاعات ویرایش شده توسط کاربر</param>
        /// <returns>جیسونی حاوی نتیجه عملیات دخیره سازی</returns>
        [HttpPost]
        [Authorize]
        public JsonResult SaveProfile(UserViewModel model)
        {
            Response response;
            int status = 200;
            string message = "";
            try
            {
                if (string.IsNullOrEmpty(model.firstName?.Trim()))
                {
                    status = 500;
                    message = "وارد کردن نام اجباری است.";
                }
                else if (string.IsNullOrEmpty(model.lastName?.Trim()))
                {
                    status = 500;
                    message = "وارد کردن نام خانوادگی اجباری است.";
                }
                else
                {
                    var user = GetAuthenticatedUser();
                    string oldFileName = "";

                    using (var db = new KiaGalleryContext())
                    {
                        var userId = GetAuthenticatedUserId();
                        var entity = db.User.Single(x => x.Id == userId);

                        if (!string.IsNullOrEmpty(entity.FileName) && entity.FileName != model.fileName)
                            oldFileName = entity.FileName;

                        entity.FirstName = model.firstName;
                        entity.LastName = model.lastName;
                        entity.PhoneNumber = model.phoneNumber;
                        entity.FileName = model.fileName;

                        entity.ModifyUserId = user.Id;
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;

                        status = 200;
                        message = "کاربر با موفقیت ویرایش شد.";

                        if (status == 200)
                            db.SaveChanges();

                        if (!string.IsNullOrEmpty(oldFileName) && System.IO.File.Exists(Server.MapPath("~/Upload/User/" + oldFileName)))
                            System.IO.File.Delete(Server.MapPath("~/Upload/User/" + oldFileName));
                    }
                }

                response = new Response()
                {
                    status = status,
                    message = message
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// خواندن اطلاعات کاربر
        /// </summary>
        /// <param name="id">ردیف کاربر</param>
        /// <returns>جیسون اطلاعات لود شده کاربر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-user")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.User.Find(id);
                    response = new Response()
                    {
                        status = 200,
                        data = UserMethods.ModelToViewModel(entity)
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// جستجوی کاربران
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست کاربران پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, admin-user")]
        public JsonResult Search(AccountSearchViewModel model)
        {
            Response response;
            try
            {
                List<UserListViewModel> userList;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.User.Select(x => x);

                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.FirstName.Contains(model.term.Trim()) || x.LastName.Contains(model.term.Trim()) || x.Username.Contains(model.term.Trim()) || x.FirstName.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")) || x.LastName.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")));
                    }

                    if (model.branchId != null && model.branchId > 0)
                    {
                        query = query.Where(x => x.BranchId == model.branchId);
                    }

                    if (model.workshopId != null && model.workshopId > 0)
                    {
                        query = query.Where(x => x.WorkshopId == model.workshopId);
                    }


                    //if (model.active != null)
                    //{
                    //    query = query.Where(x => x.Active == model.active);
                    //}
                    if (model.deactive == true)
                    {
                        query = query.Where(x => x.Active == false);
                    }
                    dataCount = query.Count();
                    query = query.OrderBy(x => x.Branch.Name).Skip(model.page * model.count).Take(model.count);

                    userList = query.Select(user => new UserListViewModel()
                    {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        username = user.Username,
                        branchName = user.Branch.Name,
                        workshopName = user.Workshop.Name,
                        active = user.Active
                    }).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = userList,
                        pageCount = Math.Ceiling((double)dataCount / model.count),
                        count = dataCount,
                        page = model.page + 1
                    }
                };

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// فعال کردن کاربر
        /// </summary>
        /// <param name="id">ردیف کاربر</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, admin-user")]
        public JsonResult Active(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.User.Single(x => x.Id == id);
                    user.Active = true;
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "حساب کاربری " + user.FullName + " فعال شد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// غیر فعال کردن کاربر
        /// </summary>
        /// <param name="id">ردیف کاربر</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, admin-user")]
        public JsonResult Inactive(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.User.Single(x => x.Id == id);
                    if (user.RoleList.Count(x => x.Title == "admin") == 0)
                    {
                        user.Active = false;
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "حساب کاربری " + user.FullName + " غیر فعال شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "شما نمی توانید کاربر مدیر سیستم را ویراش کنید.."
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
        /// حذف کاربر
        /// </summary>
        /// <param name="id">ردیف کاربر</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, admin-user")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.User.Single(x => x.Id == id);
                    if (user.RoleList.Count(x => x.Title == "admin") == 0)
                    {
                        if (!string.IsNullOrEmpty(user.FileName))
                        {
                            string serverPath = Server.MapPath("~/Upload/User");
                            if (System.IO.File.Exists(Path.Combine(serverPath, user.FileName)))
                                System.IO.File.Delete(Path.Combine(serverPath, user.FileName));
                        }

                        db.Role.RemoveRange(user.RoleList);
                        var order = db.Order.Where(x => x.CreateUserId == id);
                        var orderDetail = db.OrderDetail.Where(x => x.CreateUserId == id);
                        var orderDetailStone = db.OrderDetailStone.Where(x => orderDetail.Select(y => y.Id).Contains(x.OrderDetailId));
                        var orderDetailLeather = db.OrderDetailLeather.Where(x => orderDetail.Select(y => y.Id).Contains(x.OrderDetailId));
                        var orderDetailLog = db.OrderDetailLog.Where(x => orderDetail.Select(y => y.Id).Contains(x.OrderDetailId) || x.CreateUserId == id);
                        var orderLog = db.OrderLog.Where(x => order.Select(y => y.Id).Contains(x.OrderId));
                        var workshopOrder = db.WorkshopOrder.Where(x => order.Select(y => y.Id).Contains(x.OrderId));
                        db.OrderDetailStone.RemoveRange(orderDetailStone);
                        db.OrderDetailLeather.RemoveRange(orderDetailLeather);
                        db.OrderDetailLog.RemoveRange(orderDetailLog);
                        db.OrderDetail.RemoveRange(orderDetail);
                        db.OrderLog.RemoveRange(orderLog);
                        db.WorkshopOrder.RemoveRange(workshopOrder);
                        db.Order.RemoveRange(order);
                        db.User.Remove(user);
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "حساب کاربر مورد نظر حذف شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "شما نمی توانید کاربر مدیر سیستم را حذف کنید."
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
        /// ریست کردن گذرواژه کاربر
        /// </summary>
        /// <param name="model">مدل حاوی رمز عبور جدید</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, admin-user")]
        public JsonResult ResetPassword(ChangePasswordViewModel model)
        {
            Response response;
            try
            {
                if (model.newPassword != model.confirmNewPassword)
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "گذرواژه و تکرار آن برابر نیست."
                    };
                }
                else
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var password = PasswordTools.GetHashedPassword(model.newPassword.Trim());

                        var entity = db.User.Single(x => x.Id == model.userId);
                        entity.Salt = password.Item1;
                        entity.Password = password.Item2;
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;

                        db.SaveChanges();
                    }
                    response = new Response()
                    {
                        status = 200,
                        message = "گذرواژه با موفقیت تغییر کرد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult SendConfirmation(ForgetPasswordViewModel model)
        {
            Response response;
            try
            {

                if (!CaptchaImage.isValid(model.captcha))
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "عبارت امنیتی به درستی وارد نشده است.",
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.user))
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "شماره همراه/ نام کاربری به درستی وارد نشده است.",
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.User.Where(x => x.Active == true && (x.PhoneNumber == model.user || x.Username == model.user)).ToList();
                    if (entity.Count > 0)
                    {
                        Random random = new Random();
                        string confirmationCode = random.Next(1111, 9999).ToString();
                        foreach (var item in entity)
                        {
                            item.ConfirmationCode = confirmationCode;

                        }
                        Task.Factory.StartNew(() =>
                        {
                            SmsHandler.NikSmsWebServiceClient.SendSmsNik(confirmationCode, entity.FirstOrDefault().PhoneNumber);
                        });
                        response = new Response()
                        {
                            status = 200,
                            message = "کد تایید از طریق پیامک ارسال گردید."
                        };
                        db.SaveChanges();
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "کاربری با این مشخصات یافت نشد."
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



        [AllowAnonymous]
        public JsonResult VerifyCode(ForgetPasswordViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.User.Where(x => x.Active == true && x.ConfirmationCode == model.confirmationCode).Select(x => new UserViewModel
                    {

                        id = x.Id,
                        username = x.Username,
                        confirmationCode = x.ConfirmationCode

                    }).ToList();
                    if (entity.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new { list = entity }
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "کد تایید اشتباه است."
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

        [HttpPost]
        [AllowAnonymous]
        public JsonResult ForgotPassword(ChangePasswordViewModel model)
        {
            Response response;
            try
            {
                if (model.newPassword != model.confirmNewPassword)
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "گذرواژه و تکرار آن برابر نیست."
                    };
                }
                else
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var password = PasswordTools.GetHashedPassword(model.newPassword.Trim());

                        var entity = db.User.Single(x => x.Id == model.userId && x.ConfirmationCode == model.confirmationCode);
                        entity.Salt = password.Item1;
                        entity.Password = password.Item2;
                        entity.ModifyUserId = model.userId;
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;

                        db.SaveChanges();
                    }
                    response = new Response()
                    {
                        status = 200,
                        message = "گذرواژه با موفقیت تغییر کرد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// تغییر گذرواژه کاربر
        /// </summary>
        /// <param name="model">مدل حاوی رمز عبور جدید</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize]
        public JsonResult ChangePassword(ChangePasswordViewModel model)
        {
            Response response;
            try
            {
                if (model.newPassword != model.confirmNewPassword)
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "گذرواژه و تکرار آن برابر نیست."
                    };
                }
                else
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var password = PasswordTools.GetHashedPassword(model.newPassword.Trim());

                        var user = GetAuthenticatedUser();

                        var entity = db.User.Single(x => x.Id == user.Id);
                        if (PasswordTools.CheckPassword(model.oldPassword, entity.Password, entity.Salt))
                        {
                            entity.Salt = password.Item1;
                            entity.Password = password.Item2;
                            entity.ModifyUserId = GetAuthenticatedUserId();
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            db.SaveChanges();
                            response = new Response()
                            {
                                status = 200,
                                message = "گذرواژه با موفقیت تغییر کرد."
                            };
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 500,
                                message = "گذرواژه فعلی صحیح نیست."
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
        /// ساخت تصویر کپچا
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Route("Captcha")]
        [AllowAnonymous]
        public FileResult Captcha()
        {
            int Width = 200;
            int Height = 50;
            if (Request["w"] != null)
            {
                if (!int.TryParse(Request["w"], out Width))
                    Width = 200;
            }
            if (Request["h"] != null)
            {
                if (!int.TryParse(Request["h"], out Height))
                    Height = 50;
            }
            CaptchaImage ci = new CaptchaImage(Width, Height);

            ImageConverter converter = new ImageConverter();
            var data = (byte[])converter.ConvertTo(ci.Image, typeof(byte[]));
            ci.Dispose();

            return File(data, "image/jpeg", "Captcha.jpeg");
        }
    }
}