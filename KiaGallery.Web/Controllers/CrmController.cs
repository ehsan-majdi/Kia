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
    public class CrmController : BaseController
    {
        public ActionResult Test()
        {
            return View();
        }

        /// <summary>
        /// مشاهده جزئیات
        /// </summary>
        /// <returns>مشاهده بخش های ایجاد مشتری ، ایجاد سوال ، ایجاد و مشاهده دسته بندی</returns>
        [Authorize(Roles = "admin ,officeCrm ,branchCrm")]
        public ActionResult DetailView()
        {
            return View();
        }
        /// <summary>
        /// افزودن مشخصات مشتری و مشاهده سوالات ایجاد شده با کلیک بر نوع سفارش مشتری
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin ,officeCrm ,branchCrm")]
        public ActionResult CustomerAppend()
        {
            return View();
        }
        /// <summary>
        /// صفحه اصلی
        /// </summary>
        /// <returns>لیست صفحه اصلی ایجاد مشتری</returns>
        [Authorize(Roles = "admin ,officeCrm ,branchCrm")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// لیست سوالات ایجاد شده
        /// </summary>
        /// <returns>مشاهده لیست ایجاد سوال برای کاربر</returns>
        [Authorize(Roles = "admin")]
        public ActionResult CreateQuestion()
        {
            return View();
        }
        /// <summary>
        /// لیست صفحه اصلی ایجاد دسته بندی
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public ActionResult QuestionCategory()
        {
            return View();
        }
        /// <summary>
        /// ویرایش دسته بندی سوالات
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public ActionResult EditCategory(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// افزودن دسته بندی سوالات
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public ActionResult AddCategory()
        {
            return View("EditCategory");
        }
        /// <summary>
        /// ذخیره دسته بندی سوالات
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult SaveCategory(CategoryQuestionViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {
                        var category = db.CategoryQuestion.Single(x => x.Id == model.id);
                        category.Id = model.id;
                        category.Order = model.order;
                        category.Title = model.title;
                        category.ModifyUserId = userId;
                        category.ModifyDate = DateTime.Now;
                        category.CreateIp = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new CategoryQuestion()
                        {
                            Id = model.id,
                            Order = model.order,
                            Title = model.title,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreateIp = Request.UserHostAddress,
                        };
                        db.CategoryQuestion.Add(item);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "دسته بندی جدید ثبت شد",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// نمایش لیست مشتری
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, branchCrm, officeCrm")]
        public JsonResult SearchCustomer(CrmQuestionSearchViewModel model)
        {
            var currentUser = GetAuthenticatedUser();
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CrmCustomer.Where(x => x.BuyTypeSubset == model.buyTypeSubset && x.BuyTypeOnline == model.buyTypeOnline);
                    if (User.IsInRole("branchCrm"))
                    {
                        query = query.Where(x => x.BuyType == BuyType.BuyAttendance);
                    }

                    if (User.IsInRole("officeCrm"))
                    {
                        query = query.Where(x => x.BuyType == BuyType.BuyOnline);
                    }

                    if (User.IsInRole("admin") && model.branchId > 0 && model.branchId != null)
                    {
                        query = query.Where(x => x.BranchId == model.branchId);
                    }

                    if (!User.IsInRole("admin"))
                    {
                        query = query.Where(x => x.BranchId == currentUser.BranchId);
                    }


                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        fullName = x.FullName,
                        phoneNumber = x.PhoneNumber,
                        factorNumber = x.FactorNumber,
                        branchId = x.BranchId,
                        branchName = x.Branch.Name,
                        score = x.Score,
                        date = x.Date,

                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list.Select(x => new
                            {
                                id = x.id,
                                fullName = x.fullName,
                                phoneNumber = x.phoneNumber,
                                factorNumber = x.factorNumber,
                                branchId = x.branchId,
                                branchName = x.branchName,
                                score = x.score,
                                date = DateUtility.GetPersianDate(x.date),
                            }).ToList(),
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
        /// لیست دسته بندی سوالات
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public JsonResult SearchCategory(CategorySearchViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CategoryQuestion.Select(x => x);
                    dataCount = query.Count();
                    query = query.OrderBy(x => x.Order).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        title = x.Title,
                        order = x.Order,
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list.Select(x => new
                            {
                                id = x.id,
                                title = x.title,
                                order = x.order,

                            }).ToList(),
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
        /// دریافت همه دسته بندی سوالات
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public JsonResult GetAllCategory()
        {
            Response response;
            try
            {
                List<CategoryQuestion> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.CategoryQuestion.Select(x => x).OrderBy(x => x.Order).ToList();
                }
                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            title = item.Title,
                        })
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
        /// خواندن مقادیر سوالات
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون اطلاعات لود شده سوالات</returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public JsonResult LoadCategory(int id)
        {
            Response response;
            try
            {
                CategoryQuestion item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.CategoryQuestion.FirstOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = new CategoryQuestionViewModel
                            {
                                id = item.Id,
                                order = item.Order,
                                title = item.Title,
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
        /// حذف دسته بندی
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public JsonResult DeleteCategory(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var item = db.CategoryQuestion.Find(Id);

                    if (item.CrmQuestionList.Count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این دسته بندی شامل یک یا چند سوال است و شما نمی توانید آن را حذف کنید."
                        };
                    }

                    else
                    {
                        response = new Response()
                        {
                            status = 200,
                            message = "سوال با موفقیت حذف شد."
                        };
                        db.CategoryQuestion.Remove(item);
                        db.SaveChanges();
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
        /// ویرایش سوالات
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public ActionResult EditQuestion(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// صفحه ایجاد سوال
        /// </summary>
        /// <returns> صفحه ایجاد سوالات برای کاربر</returns>
        [Authorize(Roles = "admin")]
        public ActionResult AddQuestion()
        {
            return View("EditQuestion");
        }
        /// <summary>
        /// ویرایش بخش ایجاد مشتری
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, branchCrm, officeCrm")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// ایجاد مشتری
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin, branchCrm, officeCrm")]
        public ActionResult Add()
        {
            using (var db = new KiaGalleryContext())
            {
                var question = db.CrmQuestion.Select(x => x.Id).FirstOrDefault();
            }
            return View("Edit");
        }
        /// <summary>
        /// نمایش لیست سوالات در بخش ایجاد مشتری
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,branchCrm,officeCrm")]
        public JsonResult GetQuestionList(CrmBuyTypeViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.CrmQuestion.Where(x => x.BuyTypeSubset == model.buyTypeSubset && x.BuyTypeOnline == model.buyTypeOnline).ToList();
                    if (User.IsInRole("branchCrm"))
                    {
                        list = list.Where(x => x.BuyType == BuyType.BuyAttendance).ToList();
                    }
                    if (User.IsInRole("officeCrm"))
                    {
                        list = list.Where(x => x.BuyType == BuyType.BuyOnline).ToList();
                    }
                    list.Select(x => new QuestionViewModel()
                    {
                        id = x.Id,
                        crmQuestionType = x.CrmQuestionType,
                        buyType = x.BuyType,
                        buyTypeSubset = x.BuyTypeSubset,
                        buyTypeOnline = x.BuyTypeOnline,
                        defaultDescriptive = x.DefaultDescriptive,
                        defaultYesNo = x.DefaultYesNo,
                        title = x.Title,
                        crmQuestionValueViewModelList = x.CrmQuestionValueList.Select(y => new CrmQuestionValueViewModel()
                        {
                            id = y.Id,
                            crmQuestionId = y.CrmQuestionId,
                            defaultSelected = y.DefaultSelected,
                            description = y.Description,
                            value = y.Value,
                        }).ToList(),
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list.Select(x => new
                            {
                                id = x.Id,
                                crmQuestionType = x.CrmQuestionType,
                                buyType = x.BuyType,
                                buyTypeSubset = x.BuyTypeSubset,
                                buyTypeOnline = x.BuyTypeOnline,
                                defaultDescriptive = x.DefaultDescriptive,
                                defaultYesNo = x.DefaultYesNo,
                                title = x.Title,
                                crmQuestionValueViewModelList = x.CrmQuestionValueList.Select(y => new CrmQuestionValueViewModel()
                                {
                                    id = y.Id,
                                    crmQuestionId = y.CrmQuestionId,
                                    defaultSelected = y.DefaultSelected,
                                    description = y.Description,
                                    value = y.Value,
                                }).ToList(),
                            }).ToList(),
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
        /// نمایش لیست سوالات در بخش ایجاد مشتری
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,branchCrm,officeCrm")]
        public JsonResult GetQuestionListEditByUser(CrmBuyTypeViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var customer = db.CrmCustomer.Single(x => x.Id == model.id);
                    var list = db.CrmQuestion.Where(x => x.BuyTypeOnline == customer.BuyTypeOnline && x.BuyTypeSubset == customer.BuyTypeSubset);
                    var data = list.Select(x => new QuestionViewModel()
                    {
                        id = x.Id,
                        crmQuestionType = x.CrmQuestionType,
                        buyType = x.BuyType,
                        buyTypeSubset = x.BuyTypeSubset,
                        buyTypeOnline = x.BuyTypeOnline,
                        defaultDescriptive = x.DefaultDescriptive,
                        defaultYesNo = x.DefaultYesNo,
                        title = x.Title,
                        crmQuestionValueViewModelList = x.CrmQuestionValueList.Select(y => new CrmQuestionValueViewModel()
                        {
                            id = y.Id,
                            crmQuestionId = y.CrmQuestionId,
                            defaultSelected = y.DefaultSelected,
                            description = y.Description,
                            value = y.Value,
                        }).ToList(),
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new { list = data }
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
        /// مشاهده اطلاعات مشتری 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin,branchCrm,officeCrm")]
        public JsonResult LoadCustomer(int id)
        {
            Response response;
            try
            {
                CrmCustomer item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.CrmCustomer.FirstOrDefault(x => x.Id == id);
                    var list = new CrmCustomerViewModel
                    {
                        id = item.Id,
                        fullName = item.FullName,
                        phoneNumber = item.PhoneNumber,
                        factorNumber = item.FactorNumber,
                        buyType = item.BuyType,
                        buyTypeOnline = item.BuyTypeOnline,
                        buyTypeSubset = item.BuyTypeSubset,
                        score = item.Score,
                        date = DateUtility.GetPersianDate(item.Date),
                        answerList = item.CrmCustomerAnswerList.Select(x => new CrmCustomerAnswerViewModel()
                        {
                            crmQuestionId = x.CrmQuestionId,
                            crmQuestionType = x.CrmQuestion.CrmQuestionType,
                            buyType = x.CrmQuestion.BuyType,
                            buyTypeSubset = x.CrmQuestion.BuyTypeSubset,
                            buyTypeOnline = x.CrmQuestion.BuyTypeOnline,
                            crmQuestionValueId = x.CrmQuestionValueId,
                            descriptiveAnswer = x.DescriptiveAnswer,
                            yesNoAnswer = x.YesNoAnswer,

                        }).ToList()
                    };
                    if (list != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = list,
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
        /// ذخیره مشتری
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin,branchCrm,officeCrm")]
        public JsonResult SaveCustomer(CrmCustomerViewModel model)
        {
            var currentUser = GetAuthenticatedUser();
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var customer = db.CrmCustomer.Single(x => x.Id == model.id);
                        customer.Id = model.id;
                        customer.FullName = model.fullName;
                        customer.FactorNumber = model.factorNumber;
                        customer.PhoneNumber = model.phoneNumber;
                        if (User.IsInRole("branchCrm"))
                        {
                            customer.BuyType = BuyType.BuyAttendance;

                        }
                        if (User.IsInRole("officeCrm"))
                        {
                            customer.BuyType = BuyType.BuyOnline;

                        }
                        customer.BuyTypeOnline = model.buyTypeOnline;
                        customer.BuyTypeSubset = model.buyTypeSubset;
                        customer.Date = DateUtility.GetDateTime(model.date);
                        customer.Score = model.score;
                        customer.BranchId = currentUser.BranchId;
                        customer.ModifyUserId = GetAuthenticatedUserId();
                        customer.ModifyDate = DateTime.Now;
                        customer.Ip = Request.UserHostAddress;

                        var idList = model.answerList.Where(x => model.id != null).Select(x => model.id).ToList();
                        var deletedItems = customer.CrmCustomerAnswerList.Where(x => !idList.Any(y => y == x.Id)).ToList();
                        if (deletedItems.Count > 0)
                            db.CrmCustomerAnswer.RemoveRange(deletedItems);
                        model.answerList.ForEach(x =>
                        {
                            var value = customer.CrmCustomerAnswerList.SingleOrDefault(y => y.CrmQuestionId == x.crmQuestionId);

                            if (value != null)
                            {
                                value.CrmQuestionId = x.crmQuestionId;
                                value.CrmQuestionValueId = x.crmQuestionValueId;
                                value.DescriptiveAnswer = x.descriptiveAnswer;
                                value.YesNoAnswer = x.yesNoAnswer;
                                value.ModifyDate = DateTime.Now;
                                value.ModifyUserId = GetAuthenticatedUserId();
                                value.Ip = Request.UserHostAddress;
                            }
                            else
                            {
                                customer.CrmCustomerAnswerList.Add(new CrmCustomerAnswer()
                                {
                                    CrmQuestionId = x.crmQuestionId,
                                    CrmQuestionValueId = x.crmQuestionValueId,
                                    DescriptiveAnswer = x.descriptiveAnswer,
                                    YesNoAnswer = x.yesNoAnswer,
                                    CreateUserId = GetAuthenticatedUserId(),
                                    ModifyUserId = GetAuthenticatedUserId(),
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress,
                                });
                            }

                        });
                    }
                    else
                    {
                        var item = new CrmCustomer()
                        {
                            FullName = model.fullName,
                            FactorNumber = model.factorNumber,
                            PhoneNumber = model.phoneNumber,
                            BuyType = model.buyType,
                            BuyTypeOnline = model.buyTypeOnline,
                            BuyTypeSubset = model.buyTypeSubset,
                            BranchId = currentUser.BranchId,
                            Date = DateUtility.GetDateTime(model.date),
                            Score = model.score,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,

                        };
                        if (User.IsInRole("branchCrm"))
                        {
                            item.BuyType = BuyType.BuyAttendance;

                        }
                        if (User.IsInRole("officeCrm"))
                        {
                            item.BuyType = BuyType.BuyOnline;

                        }
                        model.answerList.ForEach(x =>
                        {
                            item.CrmCustomerAnswerList.Add(new CrmCustomerAnswer()
                            {
                                CrmQuestionId = x.crmQuestionId,
                                CrmQuestionValueId = x.crmQuestionValueId,
                                DescriptiveAnswer = x.descriptiveAnswer,
                                YesNoAnswer = x.yesNoAnswer,
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress,
                            });
                        });
                        db.CrmCustomer.Add(item);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "ثبت اطلاعات با موفقیت انجام شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ذخیره سوالات 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public JsonResult SaveQuestion(QuestionViewModel model)
        {
            Response response;
            try
            {
                if (model.crmQuestionType == CrmQuestionType.MultiChoice || model.crmQuestionType == CrmQuestionType.SingleChoice)
                {
                    if (model.crmQuestionValueViewModelList == null || model.crmQuestionValueViewModelList.Count == 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "ثبت یک پاسخ برای نوع چند انتخابی و تک انتخابی الزامی است."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                var userId = GetAuthenticatedUserId();

                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var question = db.CrmQuestion.Single(x => x.Id == model.id);
                        question.Title = model.title;
                        question.CrmQuestionType = model.crmQuestionType;
                        question.BuyType = model.buyType;
                        question.Order = model.order;
                        question.BuyTypeSubset = model.buyTypeSubset;
                        question.BuyTypeOnline = model.buyTypeOnline;
                        question.DefaultDescriptive = model.defaultDescriptive;
                        question.DefaultYesNo = model.defaultYesNo;
                        question.CategoryQuestionId = model.categoryQuestionId;
                        question.ModifyUserId = userId;
                        question.ModifyDate = DateTime.Now;
                        question.Ip = Request.UserHostAddress;

                        if (model.crmQuestionType == CrmQuestionType.MultiChoice || model.crmQuestionType == CrmQuestionType.SingleChoice)
                        {
                            var idList = model.crmQuestionValueViewModelList.Where(x => x.id != null).Select(x => x.id).ToList();
                            var deletedItems = question.CrmQuestionValueList.Where(x => !idList.Any(y => y == x.Id)).ToList();
                            if (deletedItems.Count > 0)
                                db.CrmQuestionValue.RemoveRange(deletedItems);
                            model.crmQuestionValueViewModelList.ForEach(x =>
                            {
                                if (x.id != null && x.id > 0)
                                {
                                    var value = question.CrmQuestionValueList.Single(y => y.Id == x.id);
                                    value.Value = x.value;
                                    value.DefaultSelected = x.defaultSelected;
                                    value.Description = x.description;
                                    value.ModifyUserId = userId;
                                    value.ModifyDate = DateTime.Now;
                                    value.Ip = Request.UserHostAddress;
                                }
                                else
                                {
                                    question.CrmQuestionValueList.Add(new CrmQuestionValue()
                                    {
                                        CrmQuestion = question,
                                        Value = x.value,
                                        DefaultSelected = x.defaultSelected,
                                        Description = x.description,
                                        CreateUserId = userId,
                                        ModifyUserId = userId,
                                        CreateDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        Ip = Request.UserHostAddress,
                                    });
                                }
                            });
                        }
                        else
                        {
                            db.CrmQuestionValue.RemoveRange(question.CrmQuestionValueList);
                        }
                    }
                    else
                    {

                        var item = new CrmQuestion()
                        {
                            Title = model.title,
                            CrmQuestionType = model.crmQuestionType,
                            BuyType = model.buyType,
                            Order = model.order,
                            BuyTypeSubset = model.buyTypeSubset,
                            BuyTypeOnline = model.buyTypeOnline,
                            DefaultDescriptive = model.defaultDescriptive,
                            DefaultYesNo = model.defaultYesNo,
                            CategoryQuestionId = model.categoryQuestionId,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };

                        if (model.crmQuestionValueViewModelList != null)
                        {
                            for (int i = 0; i < model.crmQuestionValueViewModelList.Count; i++)
                            {
                                item.CrmQuestionValueList.Add(new CrmQuestionValue()
                                {
                                    CrmQuestion = item,
                                    Value = model.crmQuestionValueViewModelList[i].value,
                                    DefaultSelected = model.crmQuestionValueViewModelList[i].defaultSelected,
                                    Description = model.crmQuestionValueViewModelList[i].description,
                                    CreateUserId = GetAuthenticatedUserId(),
                                    ModifyUserId = GetAuthenticatedUserId(),
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress,
                                });
                            }
                        }
                        db.CrmQuestion.Add(item);

                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "ثبت اطلاعات با موفقیت انجام شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// خواندن مقادیر سوالات
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون اطلاعات لود شده سوالات</returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                CrmQuestion item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.CrmQuestion.FirstOrDefault(x => x.Id == id);
                    var list = new QuestionViewModel
                    {
                        id = item.Id,
                        defaultDescriptive = item.DefaultDescriptive,
                        defaultYesNo = item.DefaultYesNo,
                        title = item.Title,
                        crmQuestionType = item.CrmQuestionType,
                        buyType = item.BuyType,
                        order = item.Order,
                        buyTypeSubset = item.BuyTypeSubset,
                        buyTypeOnline = item.BuyTypeOnline,
                        categoryQuestionId = item.CategoryQuestionId,
                        crmQuestionValueViewModelList = item.CrmQuestionValueList.Select(x => new CrmQuestionValueViewModel()
                        {
                            id = x.Id,
                            crmQuestionId = x.CrmQuestionId,
                            defaultSelected = x.DefaultSelected,
                            description = x.Description,
                            value = x.Value,
                        }).ToList()
                    };
                    if (list != null)
                    {
                        response = new Response()
                        {
                            status = 200,
                            data = list,
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
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public JsonResult DeleteQuestion(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.CrmQuestion.Find(Id);
                    response = new Response()
                    {
                        status = 200,
                        message = "سوال با موفقیت حذف شد."
                    };
                    db.CrmQuestionValue.RemoveRange(item.CrmQuestionValueList);
                    db.CrmQuestion.Remove(item);
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
        /// مشاهده مقادیر مربوط به سوالات بر حسب نوع سوالات
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,branchCrm,officeCrm")]
        public JsonResult Search(CrmQuestionSearchViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CrmQuestion.Select(x => x);
                    dataCount = query.Count();
                    query = query.OrderBy(x => x.Order).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        title = x.Title,
                        questionType = x.CrmQuestionType,
                        buyType = x.BuyType,
                        order = x.Order,
                        buyTypeSubset = x.BuyTypeSubset,
                        BuyTypeOnline = x.BuyTypeOnline,
                        defaultYesNo = x.DefaultYesNo,
                        categoryQuestionTitle = x.CategoryQuestion.Title,
                        defaultDescriptive = x.DefaultDescriptive,

                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list.Select(x => new
                            {
                                id = x.id,
                                title = x.title,
                                defaultYesNo = x.defaultYesNo,
                                categoryQuestionTitle = x.categoryQuestionTitle,
                                defaultDescriptive = x.defaultDescriptive,
                                questionType = Enums.GetTitle(x.questionType),
                                buyType = Enums.GetTitle(x.buyType),
                                buyTypeSubset = Enums.GetTitle(x.buyTypeSubset),
                                order = x.order,


                            }).ToList(),
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
        /// حذف مشتری
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,branchCrm,officeCrm")]
        public JsonResult DeleteCustomer(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.CrmCustomer.Find(Id);
                    response = new Response()
                    {
                        status = 200,
                        message = "مشتری با موفقیت حذف شد."
                    };
                    db.CrmCustomerAnswer.RemoveRange(item.CrmCustomerAnswerList);
                    db.CrmCustomer.Remove(item);
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
        /// مشاهده اطلاعات مشتریان ثبت شده توسط شعب گالری کیا
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin,branchCrm,officeCrm")]
        public JsonResult GetBranchList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var branchList = db.Branch.Where(x => x.BranchType == BranchType.Branch).Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = branchList,
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