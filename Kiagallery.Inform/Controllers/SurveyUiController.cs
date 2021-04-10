using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kiagallery.Inform.Controllers
{
    /// <summary>
    /// کنترلر نظر سنجی
    /// </summary>
    public class SurveyUiController : Controller
    {
        /// <summary>
        /// صفحه نظر سنجی
        /// </summary>
        /// <param name="id">کد ساخته شده مخصوص هر مشتری</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index(string id)
        {
            ViewBag.Code = id;
            return View();
        }

        /// <summary>
        /// صفحه تشکر از مشتری
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Confirm()
        {
            return View();
        }

        /// <summary>
        /// نمایش لیست سوالات در بخش ایجاد مشتری
        /// </summary>
        /// <returns>لیست سوالات</returns>
        [AllowAnonymous]
        public JsonResult GetQuestionList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.SurveyQuestion.Select(x => x);
                    var list = query.OrderBy(x => x.Order).Select(x => new SurveyViewModel()
                    {
                        id = x.Id,
                        title = x.Title,
                        questionType = x.QuestionType,


                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list
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
        /// ذخیره جواب های مشتری در دیتا بیس
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات سوال و جواب</param>
        /// <returns>نتیجه ذخیره در دیتابیس</returns>
        public JsonResult SaveAnswer(SurveyViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var customerSurveyId = db.CustomerSurvey.Where(x => x.Code == model.code).Select(x => x.Id).Single();
                    model.answerList.ForEach(x =>
                    {
                        var entity = new SurveyCustomerAnswer()
                        {
                            SurveyQuestionId = x.questionId,
                            CustomerSurveyId = customerSurveyId,
                            SurveyAnswerType = x.surveyAnswerType,
                            Offer = x.offer,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.SurveyCustomerAnswer.Add(entity);
                    });
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "با تشکر از حضور شما در نظر سنجی،لطفا ما را در شبکه های اجتماعی دنبال کنید"
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