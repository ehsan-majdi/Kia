using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Bot;
using KiaGallery.Web.Areas.Bot.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Areas.Bot.Controllers
{
    public class SettingsController : Controller
    {
        [Authorize(Roles = "admin, botMain")]
        public ActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "admin, botMain")]
        public JsonResult GetAllSettings()
        {
            List<BotSettingsViewModel> _Settings;
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                _Settings = db.BotSettings.Select(x=> new BotSettingsViewModel() {
                    id = x.Id,
                    key = x.Key,
                    value = x.Value,
                    valueFa = x.ValueFa
                }).ToList();
            }
            Response response = new Response()
            {
                status = 200,
                data = _Settings
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "admin, botMain")]
        public JsonResult SaveSettings(string key, string value, string valueFa)
        {
            using (KiaGalleryContext db = new KiaGalleryContext())
            {
                var item = db.BotSettings.FirstOrDefault(x => x.Key == key);
                item.Value = value;
                item.ValueFa = valueFa;
                db.SaveChanges();
            }

            var response = new Response()
            {
                status = 200,
                message = "تنظیمات شما به روز شده است."
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}