using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Areas.Api.Models;
using KiaGallery.Web.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Areas.Api.Controllers
{
    /// <summary>
    /// کنترلر مشتری
    /// </summary>
    public class CustomerController : BaseController
    {
        /// <summary>
        /// خواندن اطلاعات یک مشتری با بارکد
        /// </summary>
        /// <param name="barcode">بارکد کارت که توسط کاتخوان خوانده شده</param>
        /// <returns>اطلاعات مشتری یا در صورت جدید بودن کارت خطای 404</returns>
        [HttpGet]
        public JsonResult GetCustomer(string barcode)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Customer.Where(x => x.Barcode == barcode).Select(x => new 
                    {
                        id = x.Id,
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        nationalityCode = x.NationalityCode,
                        mobileNo = x.MobileNo,
                        balance = x.Balance,
                        birthDate = x.BirthDate,
                        weddingDate = x.WeddingDate,
                        sex = x.Sex
                    }).SingleOrDefault();

                    if (entity != null)
                    {
                        var customer = new CustomerViewModel()
                        {
                            id = entity.id,
                            firstName= entity.firstName,
                            lastName = entity.lastName,
                            nationalityCode = entity.nationalityCode,
                            mobileNo = entity.mobileNo,
                            birthDate = DateUtility.GetPersianDate(entity.birthDate),
                            weddingDate = DateUtility.GetPersianDate(entity.weddingDate),
                            balance = entity.balance,
                            sex = entity.sex,
                            sexTitle = Enums.GetTitle(entity.sex)
                        };

                        response = new Response()
                        {
                            status = 200,
                            data = customer
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "مشتری مورد نظر یافت نشد"
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
        /// ایجاد و ویرایش اطلاعات مشتری
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات مشتری و کاربر</param>
        /// <returns>نتیجه ذخیره یا ایجاد کاربر به همراه اطلاعات کاربر</returns>
        [HttpPost]
        public JsonResult Save(CustomerViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    Customer entity = null;
                    if (model.id != null && model.id > 0)
                    {
                        entity = db.Customer.Single(x => x.Id == model.id);

                        entity.FirstName = model.firstName;
                        entity.LastName = model.lastName;
                        entity.NationalityCode = model.nationalityCode;
                        entity.MobileNo = model.mobileNo;
                        entity.BirthDate = DateUtility.GetDateTime(model.birthDate);
                        entity.WeddingDate = DateUtility.GetDateTime(model.weddingDate);
                        entity.Sex = model.sex;
                        entity.ModifyUserId = user.Id;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyIp = Request.UserHostAddress;
                    }
                    else
                    {
                        entity = new Customer()
                        {
                            Barcode = model.barcode,
                            FirstName = model.firstName,
                            LastName = model.lastName,
                            NationalityCode = model.nationalityCode,
                            MobileNo = model.mobileNo,
                            BirthDate = DateUtility.GetDateTime(model.birthDate),
                            WeddingDate = DateUtility.GetDateTime(model.weddingDate),
                            Sex = model.sex,
                            Balance = 0,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreateIp = Request.UserHostAddress,
                            ModifyIp = Request.UserHostAddress
                        };

                        db.Customer.Add(entity);
                    }

                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "مشتری با موفقیت ذخیره شد.",
                        data = new CustomerViewModel()
                        {
                            id = entity.Id,
                            barcode = entity.Barcode,
                            firstName = entity.FirstName,
                            lastName = entity.LastName,
                            nationalityCode = entity.NationalityCode,
                            mobileNo = entity.MobileNo,
                            sex = entity.Sex,
                            sexTitle = Enums.GetTitle(entity.Sex),
                            balance = entity.Balance
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