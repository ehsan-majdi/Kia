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
    /// <summary>
    /// کنترلر نظر سنجی
    /// </summary>
    public class SurveyController : BaseController
    {
        /// <summary>
        /// صفحه لیست سوالات نظر سنجی
        /// </summary>
        /// <returns>صفحه سوالات</returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// گزارش جواب های نظر سنجی
        /// </summary>
        /// <returns>صفحه گزارش</returns>
        public ActionResult AnswerReportList()
        {
            return View();
        }
        /// <summary>
        /// لیست مشتریان دعوت شده به نظر سنجی
        /// </summary>
        /// <returns>صفحه مشتریان</returns>
        public ActionResult AnsweredCustomerList()
        {

            return View();
        }
        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="id">ردیف سوال</param>
        /// <returns>صفحه ویرایش</returns>
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// ایجاد سوال
        /// </summary>
        /// <returns>صفحه ایجاد سوال</returns>
        public ActionResult Add()
        {
            return View("Edit");
        }
        /// <summary>
        /// ذخیره سوال در دیتابیس
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات سوال</param>
        /// <returns>نتیجه ذخیره</returns>
        public JsonResult Save(SurveyViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var userId = GetAuthenticatedUserId();

                    if (model.id != null && model.id > 0)
                    {
                        var question = db.SurveyQuestion.Single(x => x.Id == model.id);

                        question.Title = model.title;
                        question.Order = model.order;
                        question.QuestionType = model.questionType;
                        question.ModifyUserId = userId;
                        question.ModifyDate = DateTime.Now;
                        question.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new SurveyQuestion()
                        {
                            Title = model.title,
                            Order = model.order,
                            QuestionType = model.questionType,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };


                        db.SurveyQuestion.Add(item);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "سوال با موفقیت ایجاد شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
       
        /// <summary>
        /// خواندن اطلاعات سوال
        /// </summary>
        /// <param name="id">ردیف سوال</param>
        /// <returns>اطلاعات سوال</returns>
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                SurveyQuestion item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.SurveyQuestion.FirstOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new SurveyViewModel
                            {
                                id = item.Id,
                                title = item.Title,
                                order = item.Order,
                                questionType = item.QuestionType
                            }

                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "سوال مورد نظر یافت نشد."
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
        /// حذف کردن سوالات
        /// </summary>
        /// <param name="Id">ردیف سوال</param>
        /// <returns>نتیجه حذف سوال</returns>
        [Authorize(Roles = "admin")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.SurveyQuestion.Find(Id);
                    response = new Response()
                    {
                        status = 200,
                        message = "سوال با موفقیت حذف شد."
                    };

                    db.SurveyQuestion.Remove(item);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// گرفتن لیست سوالات ایجاد شده
        /// </summary>
        /// <returns>لیست سوالات</returns>
        public JsonResult Search()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.SurveyQuestion.Select(x => x);
                    var list = query.Select(x => new SurveyViewModel
                    {
                        id = x.Id,
                        order = x.Order,
                        title = x.Title

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
        /// گرفتن گزارش جواب ها 
        /// </summary>
        /// <returns>لیست جوابها</returns>
        [Authorize(Roles = "admin")]
        public JsonResult GetAnswerList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.SurveyCustomerAnswer.Select(x => x);


                    var list = query.GroupBy(x => x.SurveyQuestion).Select(x => new
                    {
                        title = x.Key.Title,
                        low = x.Key.SurveyCustomerAnswerList.Where(y => y.SurveyAnswerType == SurveyAnswerType.Low).Count(),
                        bad = x.Key.SurveyCustomerAnswerList.Where(y => y.SurveyAnswerType == SurveyAnswerType.Bad).Count(),
                        weak = x.Key.SurveyCustomerAnswerList.Where(y => y.SurveyAnswerType == SurveyAnswerType.Weak).Count(),
                        normal = x.Key.SurveyCustomerAnswerList.Where(y => y.SurveyAnswerType == SurveyAnswerType.Normal).Count(),
                        high = x.Key.SurveyCustomerAnswerList.Where(y => y.SurveyAnswerType == SurveyAnswerType.High).Count(),
                        good = x.Key.SurveyCustomerAnswerList.Where(y => y.SurveyAnswerType == SurveyAnswerType.Good).Count(),
                        perfect = x.Key.SurveyCustomerAnswerList.Where(y => y.SurveyAnswerType == SurveyAnswerType.Perfect).Count(),
                        yes = x.Key.SurveyCustomerAnswerList.Where(y => y.SurveyAnswerType == SurveyAnswerType.Yes).Count(),
                        no = x.Key.SurveyCustomerAnswerList.Where(y => y.SurveyAnswerType == SurveyAnswerType.No).Count(),

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
        /// گرفتن لیست جواب های مشتری
        /// </summary>
        /// <returns>جواب های مشتری</returns>
        [AllowAnonymous]
        public JsonResult GetCustomerAnswer(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.SurveyCustomerAnswer.Where(x => x.CustomerSurveyId == id);

                    var list = query.Select(y => new AnsweredCustomerViewModel
                    {
                        type = y.SurveyAnswerType,
                        question = y.SurveyQuestion.Title
                    }).ToList();
                    list.ForEach(y =>
                    {
                        y.typeTitle = Enums.GetTitle(y.type);
                    });
                    
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
        /// گرفتن لیست مشتریان دعوت شده به نظر سنجی
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult GetAnsweredCustomerList(AnsweredCustomerViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {


                    var query = db.CustomerSurvey.Select(x => x);
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new AnsweredCustomerViewModel
                    {
                        id = x.Id,
                        firstName = x.CustomerFactor.CustomerLoyality.FirstName,
                        lastName = x.CustomerFactor.CustomerLoyality.LastName,
                        phoneNumber = x.CustomerFactor.CustomerLoyality.PhoneNumber,
                        date = x.CreateDate,
                        code = x.Code

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.persianDate = Common.DateUtility.GetPersianDate(x.date);
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

    }
}