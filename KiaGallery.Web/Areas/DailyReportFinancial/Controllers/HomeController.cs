using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Areas.DailyReportFinancial.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Areas.DailyReportFinancial.Controllers
{
    /// <summary>
    /// سرویس اصلی گزارش روزانه
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// دریافت اطلاعات پایه برنامه گزارش روزانه
        /// </summary>
        /// <param name="token">توکن کاربر</param>
        /// <returns>مقدارهای پایه شعبه کاربر صاحب توکن</returns>
        public JsonResult GetBaseData(string token)
        {
            Response response;
            using (var db = new KiaGalleryContext())
            {
                var branch = db.Token.Where(x => x.Code == token && x.Voided == false).Select(x => x.User.Branch).SingleOrDefault();

                if (branch != null)
                {
                    var baseInformarion = new BaseInformationViewModel();

                    baseInformarion.bankList = db.Bank.Where(x => x.BranchId == branch.Id && x.Active == true).OrderBy(x => x.Order).Select(x => new BankViewModel
                    {
                        id = x.Id,
                        order = x.Order,
                        name = x.Name
                    }).ToList();

                    baseInformarion.currencyList = db.Currency.Where(x => x.Active == true).OrderBy(x => x.Order).Select(x => new CurrencyViewModel
                    {
                        id = x.Id,
                        order = x.Order,
                        name = x.Name
                    }).ToList();

                    var date = DateTime.Today;
                    var helper = new PersianCalendar();
                    var month = helper.GetMonth(date) == 1 ? 12 : helper.GetMonth(date) - 1;
                    var year = month == 12 ? helper.GetYear(date) - 1 : helper.GetYear(date);
                    var fromDate = helper.ToDateTime(year, month, 1, 0, 0, 0, 0);
                    var toDate = fromDate.AddMonths(2).AddDays(-1);

                    var calendarList = db.BranchCalendar.Where(x => x.BranchId == branch.Id && x.ReportDate >= fromDate && x.ReportDate <= toDate).Select(x => new
                    {
                        id = x.Id,
                        date = x.ReportDate,
                        status = x.DailyReportList.Count > 0 ? x.DailyReportList.FirstOrDefault().Status : CalendarStatus.None
                    }).ToList();

                    baseInformarion.calendarList = calendarList.Select(x => new CalendarViewModel
                    {
                        id = x.id,
                        date = DateUtility.GetPersianDate(x.date),
                        status = x.status,
                        canEdit = (x.date > date.AddDays(-2) && x.date < date.AddDays(1))
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = baseInformarion
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 403,
                        message = "شما دسترسی استفاده از برنامه را ندارید."
                    };
                }
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}