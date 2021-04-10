using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.Gift;
using KiaGallery.Web.Areas.Api.Models;
using KiaGallery.Web.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Areas.Api.Controllers
{
    public class GiftController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public JsonResult Check(string id)
        {
            using (var db = new KiaGalleryContext())
            {
                var entity = db.Gift.Where(x => x.Code == id).SingleOrDefault();

                if (entity != null)
                {
                    var gift = new GiftViewModel()
                    {
                        id = entity.Id,
                        code = entity.Code,
                        ammount = entity.Value
                    };

                    switch (entity.GiftStatus)
                    {
                        case GiftStatus.Registered:
                        case GiftStatus.PrintingHouse:
                        case GiftStatus.DeliveryFromPrintingHouse:
                        case GiftStatus.SoldToTheBranch:
                        case GiftStatus.Cancel:
                        case GiftStatus.Burn:
                            gift.status = 0;
                            gift.statusTitle = "این کارت هدیه قابل استفاده نمی باشد.";
                            gift.ammount = 0;
                            break;
                        case GiftStatus.SoldToTheCustomer:
                            gift.status = 1;
                            gift.statusTitle = "قابل استفاده";
                            break;
                        case GiftStatus.Used:
                            gift.status = 2;
                            gift.statusTitle = "این کارت هدیه معتبر نمی باشد";
                            gift.ammount = 0;
                            break;
                    }

                    return Json(gift, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Burn(BurnGiftViewModel model)
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Gift.Where(x => x.Code == model.code && x.GiftStatus == GiftStatus.SoldToTheCustomer).SingleOrDefault();

                    if (entity != null)
                    {
                        entity.GiftStatus = GiftStatus.Used;

                        entity.RevocationCustomerName = model.fullName;
                        entity.RevocationCustomerPhoneNumber = model.phoneNumber;
                        entity.FactorNumber = model.facotrNumber;
                        entity.FactorPrice = model.price;

                        var log = new GiftLog()
                        {
                            GiftId = entity.Id,
                            GiftStatus = GiftStatus.Used,
                            CreateUserId = 1,
                            CreateDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.GiftLog.Add(log);
                        return Json(new
                        {
                            status = 200,
                            message = "کارت هدیه " + model.code + " نظر مصرف شد."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            status = 404,
                            message = "کارت هدیه با این مشخصات یافت نشد."
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 500,
                    message = "در هنگام اجرای عملیات خطایی رخ داد."
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}