using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class SendSmsController : BaseController
    {
        // GET: SendSms
        [Authorize(Roles = "admin,sendSms")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                List<BranchListViewModel> branchList = db.Branch.Where(x => x.Active == true && x.BranchType == Model.BranchType.Branch).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                List<BranchListViewModel> solicitorshipList = db.Branch.Where(x => x.Active == true && x.BranchType == Model.BranchType.Solicitorship).Select(x => new BranchListViewModel()
                {
                    id = x.Id,
                    name = x.Name
                }).ToList();
                ViewBag.CategoryList = db.SmsCategory.OrderBy(x => x.Order).Where(x => x.Active == true).ToList();
                ViewBag.BranchList = branchList;
                ViewBag.SolicitorshipList = solicitorshipList;
            }
            return View();
        }
        [Authorize(Roles = "admin,sendSms")]
        public ActionResult Send()
        {
            return View();
        }
        [Authorize(Roles = "admin,sendSms")]
        public JsonResult SendSms(SendSmsViewModel model)
        {
            Response response;
            try
            {
                Task.Factory.StartNew(() =>
                {
                    NikSmsWebServiceClient.SendSmsNik(model.text, model.phoneNumber);
                });
            }
            catch(Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            response = new Response()
            {
                status = 200,
                message = "Done!"
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin, createSms,sendSms")]
        public JsonResult GetSmsText(int? id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.SmsText.Select(x => x);
                    if (id != null && id > 0)
                    {
                        query = query.Where(x => x.SmsCategoryId == id);
                    }
                    var list = query.OrderBy(x => x.Order).Where(x => x.Active == true).Select(x => new GetSmsTextViewModel()
                    {
                        text = x.Text,
                        title = x.Title,
                        order = x.Order
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
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
        /// ارسال پیامک
        /// </summary>
        /// <param name="text"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,sendSms")]
        private string Sms(string text, string number)
        {
            string data = "username=" + "kiagallery" + "&password=" + "880866252" + "&to=" + number + "&from=" + "50004000040005" + "&text=" + text + "&isflash=false";
            var client = new RestClient("http://rest.payamak-panel.com/api/SendSMS/SendSMS");
            var request = new RestRequest(Method.POST);
            request.AddParameter("application/x-www-form-urlencoded", data, ParameterType.RequestBody);
            IRestResponse responsed = client.Execute(request);
            return null;
        }
    }
}