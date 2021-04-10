
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kiagallery.Inform.Controllers
{
    public class AllMessageController : BaseController
    {
        // GET: AllMessage
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Advertisment(string key)
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Text = db.SmsText.Where(x => x.UrlKey == key).Select(x => new SmsTextViewModel
                {
                    text = x.Text,
                    title = x.Title,
                }).ToList();
            };

            return View();
        }

        [AllowAnonymous]
        public JsonResult Search()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.SmsText.Where(x => x.Active == false).Select(x => new SmsTextViewModel
                    {
                        id = x.Id,
                        text = x.Text,
                        title = x.Title,
                        urlKey = x.UrlKey,
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new { list = list },
                    };
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