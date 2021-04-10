using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using RestSharp;
using System;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class BankAccountController : BaseController
    {
        // GET: BankAccount
        [Authorize(Roles = "admin ,bankAccount")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "admin ,bankAccount")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [Authorize(Roles = "admin ,bankAccount")]
        public ActionResult Add()
        {
            return View("Edit");
        }
        [Authorize(Roles = "admin ,bankAccount")]
        public JsonResult Save(BankAccountViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.BankAccount.Single(x => x.Id == model.id);
                        entity.Title = model.title;
                        entity.Iban = model.iban;
                        entity.AccountNumber = model.accountNumber;
                        entity.CardNumber = model.cardNumber;
                        entity.Bank = model.bank;
                        entity.FirstName = model.firstName;
                        entity.LastName = model.lastName;
                        entity.PhoneNumber = model.phoneNumber;
                        entity.Telephone = model.telephone;
                        entity.Description = model.description;
                        entity.Explanation = model.explanation;
                        entity.Organ = model.organ;
                        entity.ModifyUserId = GetAuthenticatedUserId();
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;
                        if (model.bankAccountDetailViewModelList != null && model.bankAccountDetailViewModelList.Count() > 0)
                        {
                            if (entity.BankAccountDetailList.Count > 0)
                                db.BankAccountDetail.RemoveRange(entity.BankAccountDetailList);

                            entity.BankAccountDetailList = model.bankAccountDetailViewModelList.Select(x => new BankAccountDetail
                            {
                                Bank = x.bankList,
                                AccountNumber = x.accountNumberList,
                                Title = x.title,
                                CardNumber = x.cardNumberList,
                                Iban = x.ibanList,
                                Explanation = x.explanationList,
                            }).ToList();
                        }
                        else
                        {
                            if (entity.BankAccountDetailList.Count > 0)
                                db.BankAccountDetail.RemoveRange(entity.BankAccountDetailList);
                        }
                    }
                    else
                    {
                        var item = new BankAccount()
                        {
                            FirstName = model.firstName,
                            LastName = model.lastName,
                            Bank = model.bank,
                            AccountNumber = model.accountNumber,
                            CardNumber = model.cardNumber,
                            Iban = model.iban,
                            Title = model.title,
                            PhoneNumber = model.phoneNumber,
                            Telephone = model.telephone,
                            Organ = model.organ,
                            Description = model.description,
                            Explanation = model.explanation,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,

                        };
                        if (model.bankAccountDetailViewModelList != null && model.bankAccountDetailViewModelList.Count() > 0)
                        {
                            item.BankAccountDetailList = model.bankAccountDetailViewModelList.Select(x => new BankAccountDetail
                            {
                                Bank = x.bankList,
                                AccountNumber = x.accountNumberList,
                                Title = x.title,
                                CardNumber = x.cardNumberList,
                                Iban = x.ibanList,
                                Explanation = x.explanationList,
                            }).ToList();
                        };
                        db.BankAccount.Add(item);
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
        
        [Authorize(Roles = "admin ,bankAccount")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.BankAccount.Single(x => x.Id == id);
                    response = new Response()
                    {
                        status = 200,
                        data = new BankAccountViewModel()
                        {
                            id = entity.Id,
                            firstName = entity.FirstName,
                            lastName = entity.LastName,
                            bank = entity.Bank,
                            accountNumber = entity.AccountNumber,
                            cardNumber = entity.CardNumber,
                            iban = entity.Iban,
                            title = entity.Title,
                            explanation = entity.Explanation,
                            phoneNumber = entity.PhoneNumber,
                            telephone = entity.Telephone,
                            organ = entity.Organ,
                            description = entity.Description,
                            bankAccountDetailViewModelList = entity.BankAccountDetailList.Select(x => new BankAccountDetailViewModel()
                            {
                                title = x.Title,
                                bankList = x.Bank,
                                explanationList = x.Explanation,
                                accountNumberList = x.AccountNumber,
                                cardNumberList = x.CardNumber,
                                ibanList = x.Iban,
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
        [HttpPost]
        [Authorize(Roles = "admin ,bankAccount")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.BankAccount.Find(Id);
                   
                    response = new Response()
                    {
                        status = 200,
                        message = "حساب با موفقیت حذف شد."
                    };
                    

                    db.BankAccount.Remove(item);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin ,bankAccount")]
        public JsonResult Search(BankAccountSearchViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var query = db.BankAccount.Select(x => x);
                    if (model.organ)
                    {
                        query = query.Where(x => x.Organ == model.organ);
                    }
                    var list = query.Select(x => new
                    {
                        id = x.Id,
                        title = x.Title,
                        bank = x.Bank,
                        explanation = x.Explanation,
                        fullName = x.FirstName + " " + x.LastName,
                        phoneNumber = x.PhoneNumber,
                        telephone = x.Telephone,
                        cardNumber = x.CardNumber,
                        accountNumber = x.AccountNumber,
                        iban = x.Iban,
                        organ = x.Organ,
                        description = x.Description,
                        detail = x.BankAccountDetailList.Select(y => new
                        {
                            fullNameList = x.FirstName+ " " + x.LastName,
                            bankList = y.Bank,
                            ibanList = y.Iban,
                            accountNumberList = y.AccountNumber,
                            cardNumberList = y.CardNumber,
                            explanationList = y.Explanation,
                        }).ToList(),
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
                                bank = x.bank,
                                fullName = x.fullName,
                                explanation = x.explanation,
                                phoneNumber = x.phoneNumber,
                                telephone = x.telephone,
                                cardNumber = Core.CardToSeparator(x.cardNumber),
                                accountNumber = x.accountNumber,
                                iban = x.iban,
                                organ = x.organ,
                                description = x.description,
                                detail = x.detail.Select(y => new
                                {
                                    fullNameList = y.fullNameList,
                                    bankList = y.bankList,
                                    ibanList = y.ibanList,
                                    accountNumberList = y.accountNumberList,
                                    cardNumberList = y.cardNumberList,
                                    explanationList = y.explanationList
                                }).ToList()
                            }).ToList()
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
        //[Authorize(Roles = "admin ,bankAccount")]
        //public JsonResult GetDetailList(int id)
        //{
        //    Response response;
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {

        //            var query = db.BankAccount.Where(x => x.Id == id);
        //            var list = query.Select(x => new
        //            {
        //                detail = x.BankAccountDetailList.Select(y => new
        //                {
        //                    bankList = y.Bank,
        //                    ibanlist = y.Iban,
        //                    accounNumberList = y.AccountNumber,
        //                    cardNumberList = y.CardNumber,
        //                    explanationList = y.Explanation,
        //                }).ToList(),
        //            }).ToList();

        //            response = new Response()
        //            {
        //                status = 200,
        //                data = new
        //                {
        //                    list = list.Select(x => new
        //                    {
        //                        detail = x.detail.Select(y => new
        //                        {
        //                            bankList = y.bankList,
        //                            ibanlist = y.ibanlist,
        //                            accounNumberList = y.accounNumberList,
        //                            cardNumberList = y.cardNumberList,
        //                            explanationList = y.explanationList
        //                        }).ToList()
        //                    }).ToList()
        //                }

        //            };
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }

        //    return Json(response, JsonRequestBehavior.AllowGet);

        //}
        private string Sms(string text, string number)
        {
            string data = "username=" + "kiagallery" + "&password=" + "880866252" + "&to=" + number + "&from=" + "50004000040005" + "&text=" + text + "&isflash=false";

            var client = new RestClient("http://rest.payamak-panel.com/api/SendSMS/SendSMS");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/x-www-form-urlencoded", data, ParameterType.RequestBody);

            IRestResponse responsed = client.Execute(request);
            return null;
        }
        [Authorize(Roles = "admin ,bankAccount")]
        public JsonResult SendSms(SendSmsViewModel model)
        {
            Response response;
            try
            {
                Sms("نام صاحب حساب : "+ model.firstName + "\n" + "بانک : " + model.bank + "\n" + "شماره کارت:" + "\n" + model.cardNumber + "\n" + "شماره حساب:" + "\n" + model.accountNumber + "\n" + "شبا:" + "\n" + model.iban + "\n", model.phoneNumber);
                response = new Response()
                {
                    status = 200,
                    message = "پیام با موفقیت ارسال شد."

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