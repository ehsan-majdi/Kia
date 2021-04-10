using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class SettingsController : BaseController
    {
        // GET: Settings
        public ActionResult Index()
        {
            List<string> keyList = new List<string> {
                Settings.KeyContractYear,
                Settings.KeyBaseSalary,
                Settings.KeyYearAddedRate,
                Settings.KeyBranchPersonelHourlyPayment,
                Settings.KeyBranchPersonelOverTime,
                Settings.KeyBranchPersonelMission,
                Settings.KeyBranchPersonelMonthlyWorkingHour,
                Settings.KeySuperVisorHourlyPayment,
                Settings.KeySuperVisorOverTime,
                Settings.KeySuperVisorMission,
                Settings.KeySuperVisorMonthlyWorkingHour,
                Settings.KeyOfficeHourlyPayment,
                Settings.KeyOfficeOverTime,
                Settings.KeyOfficeMission,
                Settings.KeyOfficeMonthlyWorkingHour
            };

            using (var db = new KiaGalleryContext())
            {
                List<Settings> settingList = db.Settings.Where(x => keyList.Any(y => y == x.Key)).ToList();
                Settings year = settingList.SingleOrDefault(x => x.Key == Settings.KeyContractYear);
                if (year == null)
                {
                    PersianCalendar cal = new PersianCalendar();
                    int Shamsi = cal.GetYear(DateTime.Now);
                    year = new Settings()
                    {
                        Key = Settings.KeyContractYear,
                        Value = Shamsi.ToString(),
                        ModifyUserId = GetAuthenticatedUserId(),
                        ModifyDate = DateTime.Now,
                    };
                    settingList.Add(year);

                }
                ViewBag.SettingsList = settingList;
            }
            ViewBag.Title = "تنظیمات";
            return View();

        }

        [HttpPost]
        [Authorize(Roles = "admin, settings")]
        public JsonResult SettingSave(SettingsViewModel model)
        {
            Response response;
            try
            {
                List<string> keyList = new List<string> {
                Settings.KeyContractYear,
                Settings.KeyBaseSalary,
                Settings.KeyYearAddedRate,
                Settings.KeyBranchPersonelHourlyPayment,
                Settings.KeyBranchPersonelOverTime,
                Settings.KeyBranchPersonelMission,
                Settings.KeyBranchPersonelMonthlyWorkingHour,
                Settings.KeySuperVisorHourlyPayment,
                Settings.KeySuperVisorOverTime,
                Settings.KeySuperVisorMission,
                Settings.KeySuperVisorMonthlyWorkingHour,
                Settings.KeyOfficeHourlyPayment,
                Settings.KeyOfficeOverTime,
                Settings.KeyOfficeMission,
                Settings.KeyOfficeMonthlyWorkingHour
                };
                using (var db = new KiaGalleryContext())
                {
                    List<Settings> settings = db.Settings.Where(x => keyList.Any(y => y == x.Key)).ToList();
                    settings.Single(x => x.Key == Settings.KeyContractYear).Value = model.contractYear;
                    settings.Single(x => x.Key == Settings.KeyBaseSalary).Value = model.baseSalary;
                    settings.Single(x => x.Key == Settings.KeyYearAddedRate).Value = model.yearAddedRate;

                    settings.Single(x => x.Key == Settings.KeyBranchPersonelHourlyPayment).Value = model.branchPersonelHourlyPayment;
                    settings.Single(x => x.Key == Settings.KeyBranchPersonelOverTime).Value = model.branchPersonelOverTime;
                    settings.Single(x => x.Key == Settings.KeyBranchPersonelMission).Value = model.branchPersonelMission;
                    settings.Single(x => x.Key == Settings.KeyBranchPersonelMonthlyWorkingHour).Value = model.branchPersonelMonthlyWorkingHour;

                    settings.Single(x => x.Key == Settings.KeySuperVisorHourlyPayment).Value = model.superVisorHourlyPayment;
                    settings.Single(x => x.Key == Settings.KeySuperVisorOverTime).Value = model.superVisorOverTime;
                    settings.Single(x => x.Key == Settings.KeySuperVisorMission).Value = model.superVisorMission;
                    settings.Single(x => x.Key == Settings.KeySuperVisorMonthlyWorkingHour).Value = model.superVisorMonthlyWorkingHour;

                    settings.Single(x => x.Key == Settings.KeyOfficeHourlyPayment).Value = model.officeHourlyPayment;
                    settings.Single(x => x.Key == Settings.KeyOfficeOverTime).Value = model.officeOverTime;
                    settings.Single(x => x.Key == Settings.KeyOfficeMission).Value = model.officeMission;
                    settings.Single(x => x.Key == Settings.KeyOfficeMonthlyWorkingHour).Value = model.officeMonthlyWorkingHour;



                    Settings year = settings.SingleOrDefault(x => x.Key == Settings.KeyContractYear);
                    if (year != null)
                    {
                        year.Value = model.contractYear;
                        year.CreateUserId = GetAuthenticatedUserId();
                        year.ModifyUserId = GetAuthenticatedUserId();
                        year.CreateDate = DateTime.Now;
                        year.ModifyDate = DateTime.Now;
                        year.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        db.Settings.Add(new Model.Context.Settings()
                        {
                            Key = Model.Context.Settings.KeyContractYear,
                            Value = model.contractYear,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        });

                    }
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
    }
}