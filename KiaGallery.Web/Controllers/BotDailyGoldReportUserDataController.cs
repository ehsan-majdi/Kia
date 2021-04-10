using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class BotDailyGoldReportUserDataController : BaseController
    {
        /// <summary>
        /// کاربر گزارشات شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, dailyReportManage")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.branchList = db.Branch.Select(x => x).ToList();
            }
            return View();
        }
        /// <summary>
        /// گرفتن لیست کاربران گزارشات شعب ربات
        /// </summary>
        /// <param name="model"></param>
        /// <returns>لیست کاربران</returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult GetBotUserList(SearchBotUserListViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.BotGoldReportUserData.Select(x => x);
                    if (model.userName != null)
                    {
                        query = query.Where(x => x.Username == model.userName);
                    }
                    if (model.userType != null)
                    {
                        query = query.Where(x => x.UserType == model.userType);
                    }
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new BotUserListViewModel()
                    {
                        id = x.Id,
                        botUserType = x.BotUserType,
                        username = x.Username,
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        branchId = x.BranchId,
                        userType = x.UserType
                    }).ToList();

                    var branchList = db.Branch.ToList();

                    list.ForEach(x =>
                    {
                        x.botUserTypeTitle = Enums.GetTitle(x.botUserType);
                        if (!string.IsNullOrEmpty(x.branchId))
                        {
                            x.branchList = x.branchId.Split('-').Select(y => new BotUserListBranchViewModel()
                            {
                                id = int.Parse(y),
                                name = branchList.Single(z => z.Id.ToString() == y).Name
                            }).ToList();
                        }
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            pageCount = Math.Ceiling((double)dataCount / model.count),
                            count = dataCount,
                            page = model.page + 1
                        }
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
        /// تغییر نوع کاربران گزارشات شعب ربات
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult ChangeBotUserType(SaveBotUserTypeViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.BotGoldReportUserData.Single(x => x.Id == model.id);

                    entity.BotUserType = model.botUserType;
                    entity.UserType = model.userType;

                    if (model.branchId != null && model.branchId > 0)
                    {
                        var branchList = string.IsNullOrEmpty(entity.BranchId) ? new List<int>() { } : entity.BranchId.Split('-').Select(x => int.Parse(x)).ToList();
                        if (!branchList.Contains(model.branchId.Value))
                            branchList.Add(model.branchId.Value);
                        entity.BranchId = string.Join("-", branchList);
                    }

                    entity.ModifyUserId = currentUser.Id;

                    db.SaveChanges();

                }
                response = new Response()
                {
                    status = 200,
                    message = "اطلاعات با موفقیت ثبت شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// حذف یکی از شعبه های متصل شده به کاربر
        /// </summary>
        /// <param name="userId">ردیف کاربر</param>
        /// <param name="branchId">ردیف شعبه که می بایست حذف بشود.</param>
        /// <returns>نتیجه حذف شعبه کاربر</returns>
        [HttpPost]
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult RemoveBranch(int userId, int branchId)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var entity = db.BotGoldReportUserData.Single(x => x.Id == userId);
                    var branchList = entity.BranchId.Split('-').Select(x => int.Parse(x)).ToList();
                    branchList.Remove(branchId);

                    entity.BranchId = string.Join("-", branchList);

                    entity.ModifyUserId = currentUser.Id;
                    db.SaveChanges();

                }
                response = new Response()
                {
                    status = 200,
                    message = "شعبه با موفقیت حذف شد."
                };
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
        /// <param name="userId">ردیف کاربر</param>
        /// <returns>نتیجه حذف کاربر</returns>
        [HttpPost]
        [Authorize(Roles = "admin, dailyReportManage")]
        public JsonResult RemoveUser(int userId)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var entity = db.BotGoldReportUserData.Single(x => x.Id == userId);
                    db.BotGoldReportUserData.Remove(entity);

                    db.SaveChanges();

                }
                response = new Response()
                {
                    status = 200,
                    message = "کاربر با موفقیت حذف شد."
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