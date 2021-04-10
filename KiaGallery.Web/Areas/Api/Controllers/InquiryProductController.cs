using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using KiaGallery.Web.Controllers;
using KiaGallery.Model;

namespace KiaGallery.Web.Areas.Api.Controllers
{
    /// <summary>
    /// کنترلر استعلام محصول
    /// </summary>
    public class InquiryProductController : BaseController
    {
        /// <summary>
        /// اضافه کردن استعلام
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات جهت ذخیره استعلام</param>
        /// <returns>نتیجه ذخیره استعلام</returns>
        [HttpPost]
        public JsonResult AddInquiry(AddInquiryProductViewModel model)
        {
            Response response;
            var user = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (!string.IsNullOrEmpty(model.fileName))
                    {
                        model.fileName = SaveInquiryImage(model.base64, model.fileName);
                    }
                    InquiryProduct inquiryProduct = new InquiryProduct()
                    {
                        ProductId = model.productId,
                        BranchId = user.BranchId.GetValueOrDefault(),
                        FileName = model.fileName,
                        Comment = model.comment,
                        CreateUserId = user.Id,
                        ModifyUserId = user.Id,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };
                    db.InquiryProduct.Add(inquiryProduct);
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "درخواست استعلام محصول ذخیره شد "
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// حذف استعلام
        /// </summary>
        /// <param name="id">ردیف استعلام</param>
        /// <returns>نتیجه حذف استعلام</returns>
        [HttpGet]
        public JsonResult DeleteInquiry(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    InquiryProduct inquiryProduct = db.InquiryProduct.Where(x => x.Id == id).Single();
                    if (!string.IsNullOrEmpty(inquiryProduct.FileName))
                    {
                        string serverPath = Server.MapPath("~/Upload/InquiryProduct");
                        string fileName = Path.Combine(serverPath, inquiryProduct.FileName);
                        if (System.IO.File.Exists(fileName))
                        {
                            System.IO.File.Delete(fileName);
                        }
                    }
                    db.InquiryProduct.Remove(inquiryProduct);
                    response = new Response()
                    {
                        status = 200,
                        message = "استعلام محصول با موفقیت حذف شد"
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
        /// ذخیره پاسخ استعلام
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات جهت ذخیره پاسخ</param>
        /// <returns>نتیجه ذخیره پاسخ استعلام</returns>
        [HttpGet]
        public JsonResult SubmitInquiryReply(InquiryProductReplyViewModel model)
        {
            Response response;
            var user = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var inquiryProductReply = new InquiryProductReply()
                    {
                        BranchId = model.branchId,
                        InquiryProductId = model.inquiryProductId,
                        AnswerType = model.answerType,
                        Comment = model.comment,
                        CreateUserId = user.Id,
                        ModifyUserId = user.Id,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        CreateIp = Request.UserHostAddress,
                        ModifyIp = Request.UserHostAddress
                    };
                    db.InquiryProductReply.Add(inquiryProductReply);
                    db.SaveChanges();
                    response = new Response
                    {
                        status = 200,
                        message = " پاسخ استعلام محصول با موفقیت ثبت شد ",
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
        /// لیست پاسخ های ثبت شده شعب برای هر استعلام
        /// </summary>
        /// <returns>جیسون لیست پاسخ های ثبت شده برای استعلام</returns>
        [HttpPost]
        public JsonResult InquiryReplyList()
        {
            Response response;
            var user = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    List<InquiryProductReplyViewModel> replyList = db.InquiryProductReply.OrderByDescending(x => x.Id).Select(x => new InquiryProductReplyViewModel()
                    {
                        id = x.Id,
                        branchId = x.BranchId,
                        branchName = x.Branch.Name,
                        comment = x.Comment,
                        answerType = x.AnswerType,
                        inquiryProductId = x.InquiryProductId,
                        createdDate = x.CreateDate
                    }).ToList();

                    replyList.ForEach(x => x.persianCreatedDate = DateUtility.GetPersianDateTime(x.createdDate));
                    response = new Response
                    {
                        status = 200,
                        data = new
                        {
                            list = replyList
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
        /// لیست استعلام
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات برای جستجوی استعلام های ثبت شده</param>
        /// <returns>جیسون نتیجه جستجو</returns>
        [HttpPost]
        public JsonResult InquiryList(InquiryProductSearchViewModel model)
        {
            Response response;
            var user = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.InquiryProduct.Select(x => x);
                    var dataCount = query.Count();

                    List<InquiryProductViewModel> inquiryList = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count).Select(x => new InquiryProductViewModel()
                    {
                        id = x.Id,
                        imageLink = "/upload/user/" + user.FileName,
                        productCode = x.Product.Code,
                        productTitle = x.Product.Title,
                        branchId = x.BranchId,
                        branchName = x.Branch.Name,
                        comment = x.Comment,
                        createdDate = x.CreateDate,
                        commentCount = x.InquiryProductReplyList.Where(y => !string.IsNullOrEmpty(y.Comment)).Count(),
                    }).ToList();

                    inquiryList.ForEach(x =>
                    {
                        x.productTypeTitle = Enums.GetTitle(x.productType);
                        x.persianCreatedDate = DateUtility.GetPersianDateTime(x.createdDate);
                    });

                    response = new Response
                    {
                        status = 200,
                        data = new
                        {
                            list = inquiryList,
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
        /// تبدیل متن به عکس و ذخیره آن در پوشه فایل های استعلام
        /// </summary>
        /// <param name="base64String">تصویر که به صورت بیس 64 در آمده</param>
        /// <param name="fileName">نام فایل</param>
        /// <returns>در صورتی که نام فایل تغییر کند نام فایل جدید برگردانده می شود</returns>
        private string SaveInquiryImage(string base64String, string fileName)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            string serverPath = Server.MapPath("~/Upload/inquiry");

            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Upload/inquiry/"));
            }
            string savedFileName = Path.Combine(serverPath, fileName);
            if (System.IO.File.Exists(savedFileName))
            {
                Random random = new Random();
                string prefix = random.Next(1000, 9999).ToString() + "-";
                fileName = prefix + fileName;
                savedFileName = Path.Combine(serverPath, fileName);
            }
            image.Save(savedFileName);
            return fileName;
        }
    }
}
