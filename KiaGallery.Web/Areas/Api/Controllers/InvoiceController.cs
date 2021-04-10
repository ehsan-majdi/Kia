using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Areas.Api.Models;
using KiaGallery.Web.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Areas.Api.Controllers
{
    /// <summary>
    /// کنترلر فاکتور های مشتریان
    /// </summary>
    public class InvoiceController : BaseController
    {
        /// <summary>
        /// دریافت تنظیمات برنامه
        /// </summary>
        /// <returns>لیست تنظیمات و قیمت طلای روز</returns>
        [HttpGet]
        public JsonResult GetSettings()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var discountList = db.CrmDiscountSetting.Select(x => new
                    {
                        fromPrice = x.FromPrice,
                        toPrice = x.ToPrice,
                        discount = x.Discount
                    }).ToList();

                    var goldPriceValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice)?.Value;
                    var goldPrice = !string.IsNullOrEmpty(goldPriceValue) ? long.Parse(goldPriceValue) : 0;

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            goldPrice = goldPrice,
                            discountList = discountList
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
        /// ذخیره فاکتور سیستم سفارش مشتریان
        /// </summary>
        /// <param name="model">مدل ذخیره</param>
        /// <returns>نتیجه دخیره فاکتور</returns>
        [HttpPost]
        public JsonResult SaveInvoice(InvoiceViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var discountList = db.CrmDiscountSetting.ToList();
                    var goldPriceValue = db.Settings.SingleOrDefault(x => x.Key == Settings.KeyGoldPrice)?.Value;
                    var goldPrice = !string.IsNullOrEmpty(goldPriceValue) ? int.Parse(goldPriceValue) : 0;

                    var price = model.amount - model.discount;
                    var persentDiscount = discountList.SingleOrDefault(x => x.FromPrice <= price && x.ToPrice >= price)?.Discount;
                    var customer = db.Customer.Single(x => x.Id == model.customerId);
                    var barcodeList = model.detailList.Select(x => x.barcode).ToList();

                    if (price <= 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "قیمت وارد شده صحیح نمی باشد."
                        };
                    }
                    else if (model.discount > customer.Balance)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "میزان اعتبار مورد استفاده برای فاکتور نمی تواند بیشتر از اعتبار مشتری باشد."
                        };
                    }
                    else if (model.invoiceType == Model.CrmInvoiceType.Purchase && db.CrmInvoiceDetail.Where(x => x.Revocation == false && barcodeList.Any(y => x.Barcode == y)).Count() > 0)
                    {
                        var duplicateBarcode = db.CrmInvoiceDetail.Where(x => x.Revocation == false && barcodeList.Any(y => x.Barcode == y)).Select(x => x.Barcode).ToList();
                        var duplicateBarcodeString = string.Join(", ", duplicateBarcode);

                        response = new Response()
                        {
                            status = 500,
                            message = string.Format("بارکدهای {0} قبلا خریداری شده اند.", duplicateBarcodeString)
                        };
                    }
                    else
                    {
                        if (model.invoiceType == Model.CrmInvoiceType.Purchase)
                        {
                            customer.Balance += (int)(((price * persentDiscount.GetValueOrDefault())) / 100);
                        }
                        else
                        {
                            customer.Balance -= (int)(((price * persentDiscount.GetValueOrDefault())) / 100);
                        }

                        var entity = new CrmInvoice()
                        {
                            CustomerId = model.customerId,
                            InvoiceType = model.invoiceType,
                            Amount = model.amount,
                            Discount = model.discount,
                            DiscountPercent = persentDiscount.GetValueOrDefault(),
                            GoldPrice = goldPrice,
                            BranchId = user.BranchId.GetValueOrDefault(),
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreateIp = Request.UserHostAddress,
                            ModifyIp = Request.UserHostAddress
                        };

                        var details = model.detailList?.Select(x => new CrmInvoiceDetail()
                        {
                            Invoice = entity,
                            Barcode = x.barcode,
                            Revocation = model.invoiceType == Model.CrmInvoiceType.Revocation,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreateIp = Request.UserHostAddress,
                            ModifyIp = Request.UserHostAddress
                        }).ToList();

                        db.CrmInvoice.Add(entity);

                        if (model.invoiceType == Model.CrmInvoiceType.Revocation)
                        {
                            var duplicateBarcode = db.CrmInvoiceDetail.Where(x => x.Revocation == false && barcodeList.Any(y => x.Barcode == y)).ToList();
                            duplicateBarcode.ForEach(x =>
                            {
                                x.Revocation = true;
                                x.ModifyUserId = user.Id;
                                x.ModifyDate = DateTime.Now;
                                x.ModifyIp = Request.UserHostAddress;
                            });
                        }

                        if (details != null)
                            db.CrmInvoiceDetail.AddRange(details);

                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "فاکتور با موفقیت ذخیره شد."
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
        /// لیست فاکتور های ثبت شده برای مشتری در سیستم مدیریت مشتری
        /// </summary>
        /// <param name="model">مدل اطلاعات جستجو</param>
        /// <returns>نتیجه جستجو</returns>
        [HttpGet]
        public JsonResult InvoiceList(InvoiceSearchViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CrmInvoice.Where(x => x.CustomerId == model.customerId).Select(x => new InvoiceViewModel()
                    {
                        id = x.Id,
                        customerId = x.CustomerId,
                        invoiceType = x.InvoiceType,
                        amount = x.Amount,
                        discount = x.Discount,
                        discountPercent = x.DiscountPercent
                    });

                    var dataCount = query.Count();

                    var invoiceList = query.OrderByDescending(x => x.id).Skip(model.page * model.count).Take(model.count).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = invoiceList,
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
        /// خواندن اطلاعات یک بارکد
        /// </summary>
        /// <param name="barcode">بارکد ثبت شده</param>
        /// <returns>اطلاعات بارکد، که نام شعبه و تاریخ را در خود دارد</returns>
        [HttpGet]
        public JsonResult GetBarcode(string barcode)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.CrmInvoiceDetail.Where(x => x.Invoice.InvoiceType == Model.CrmInvoiceType.Purchase && x.Barcode == barcode).OrderByDescending(x => x.CreateDate).Select(x => new InvoiceDetailViewModel
                    {
                        id = x.Id,
                        barcode = x.Barcode,
                        branchName = x.Invoice.Branch.Name,
                        date = x.CreateDate,
                        revocation = x.Revocation,
                        goldPrice = x.Invoice.GoldPrice.ToString()
                    }).FirstOrDefault();

                    if (item != null)
                    {
                        item.persianDate = DateUtility.GetPersianDateTime(item.date);
                        item.goldPrice = Core.ToSeparator(int.Parse(item.goldPrice));

                        response = new Response()
                        {
                            status = 200,
                            data = item
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "بارکد مورد نظر یافت نشد."
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
    }
}