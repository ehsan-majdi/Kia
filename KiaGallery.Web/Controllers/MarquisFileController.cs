using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.FileManagement;
using KiaGallery.Model.FileContext;
using KiaGallery.Model.FileContext.Enum;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر فایل مارکیز
    /// </summary>
    public class MarquisFileController : BaseController
    {
        /// <summary>
        /// صفحه بارگذاری فایل توسط اپراتور
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, marquisFileUpload")]
        public ActionResult Upload()
        {
            return View();
        }

        /// <summary>
        /// صفحه دریافت فایل توسط شعبه یا نمایندگی
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        public ActionResult Download()
        {
            return View();
        }

        /// <summary>
        /// ذخیره فایل های مارکیز
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات فایل که برای شعبه بایست ارسال شود</param>
        /// <returns>نتیجه ذخیره</returns>
        [Authorize(Roles = "admin, marquisFileUpload")]
        public JsonResult Save(MarquisFileViewModel model)
        {
            Response response;
            try
            {
                if (model.branchId == 0)
                {
                    response = new Response() { status = 500, message = "انتخاب شعبه مورد نظر اجباری است." };
                }
                //else if (string.IsNullOrEmpty(model.persianDate) || DateUtility.GetDateTime(model.persianDate) == null)
                //{
                //    response = new Response() { status = 500, message = "تاریخ وارد شده صحیح نیست." };
                //}
                else if (Request.Files.Count == 0 || Request.Files.Count % 2 != 0)
                {
                    response = new Response() { status = 500, message = "تعداد فایل های وارد شده صحیح نیست." };
                }
                else if (Request.Files.AllKeys.Count(x => !x.ToLower().StartsWith("dt") || !x.ToLower().StartsWith("img")) == 0)
                {
                    response = new Response() { status = 500, message = "نام فایل های وارد شده صحیح نیست." };
                }
                else
                {
                    var currentUser = GetAuthenticatedUser();

                    var file = new MarquisFile()
                    {
                        BranchId = model.branchId,
                        Date = DateTime.Today, //DateUtility.GetDateTime(model.persianDate).Value,
                        Description = model.description,
                        CreateUserId = currentUser.Id,
                        ModifyUserId = currentUser.Id,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };

                    List<MarquisFileDetail> detailList = new List<MarquisFileDetail>();

                    var files = Request.Files
                        .Cast<string>()
                        .Select(k => Request.Files[k])
                        .ToArray();

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase item = Request.Files[i];

                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(item.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(item.ContentLength);
                        }
                        using (var db = new KiaGalleryFileContext())
                        {

                            var fileEntity = new Model.FileContext.Entity.File()
                            {
                                Id = StringUtility.RandomString(32),
                                FileName = item.FileName,
                                Extention = Path.GetExtension(item.FileName),
                                MimeType = MimeMapping.GetMimeMapping(item.FileName),
                                Length = fileData.Length,
                                Data = fileData,
                                StatusId = FileStatus.Verify.Id,
                                CreateUserId = currentUser.Id,
                                CreateDate = DateTime.Now,
                                CreateIp = Request.UserHostAddress
                            };

                            db.File.Add(fileEntity);
                            db.SaveChanges();

                            detailList.Add(new MarquisFileDetail() { MarquisFile = file, FileId = fileEntity.Id, FileName = fileEntity.FileName });
                        };
                    }

                    using (var db = new KiaGalleryContext())
                    {
                        db.MarquisFile.Add(file);
                        db.MarquisFileDetail.AddRange(detailList);

                        db.SaveChanges();
                    }
                    response = new Response() { status = 200, message = "اطلاعات با موفقیت ذخیره شد." };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// لیست فایل های آپلود شده
        /// </summary>
        /// <param name="model">پارامتر های جستجو</param>
        /// <returns>نتیجه لیست فایل های یافت شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, marquisFileUpload")]
        public JsonResult GetFiles(MarquisFileSearchViewModel model)
        {
            Response response;
            try
            {
                List<MarquisFileViewModel> list;
                int dataCount;
                var date = DateUtility.GetDateTime(model.date);

                if (date != null)
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var query = db.MarquisFile.Select(x => x);

                        if (!string.IsNullOrEmpty(model.date))
                        {
                            query = query.Where(x => DbFunctions.TruncateTime(x.CreateDate) == DbFunctions.TruncateTime(date));
                        }

                        if (model.branchId != null && model.branchId > 0)
                        {
                            query = query.Where(x => x.BranchId == model.branchId);
                        }

                        dataCount = query.Count();
                        query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);
                        list = query.Select(item => new MarquisFileViewModel()
                        {
                            id = item.Id,
                            branchId = item.BranchId,
                            branchName = item.Branch.Name,
                            description = item.Description,
                            fileCount = item.MarquisFileDetailList.Count,
                            date = item.CreateDate
                        }).ToList();
                    }

                    list.ForEach(x => x.persianDate = DateUtility.GetPersianDateTime(x.date));

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
                else
                {
                    response = new Common.Response() { status = 500, message = "تاریخ وارد شده صحیح نیست." };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// لیست نام فایل های مارکیز
        /// </summary>
        /// <param name="id">ردیف فایل مارکیز</param>
        /// <returns>لیست فایل ها</returns>
        [HttpGet]
        [Authorize(Roles = "admin, marquisFileUpload")]
        public JsonResult GetDetailFiles(int id)
        {
            Response response;
            try
            {
                List<MarquisFileDetailViewModel> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.MarquisFileDetail.Where(x => x.MarquisFileId == id).Select(x => new MarquisFileDetailViewModel()
                    {
                        id = x.Id,
                        marquisFileId = x.MarquisFileId,
                        fileId = x.FileId,
                        fileName = x.FileName
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new { list }
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
        /// لیست فایل های آپلود شده شعبه لاگین در سیستم
        /// </summary>
        /// <param name="model">پارامتر های جستجو</param>
        /// <returns>نتیجه لیست فایل های یافت شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, marquisFileDownload")]
        public JsonResult GetBranchFiles(MarquisFileSearchViewModel model)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                List<MarquisFileViewModel> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.MarquisFile.Where(x => x.BranchId == user.BranchId);

                    if (!string.IsNullOrEmpty(model.date))
                    {
                        var date = DateUtility.GetDateTime(model.date);
                        query = query.Where(x => DbFunctions.TruncateTime(x.CreateDate) == DbFunctions.TruncateTime(date));
                    }

                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    list = query.Select(item => new MarquisFileViewModel()
                    {
                        id = item.Id,
                        branchId = item.BranchId,
                        branchName = item.Branch.Name,
                        description = item.Description,
                        fileCount = item.MarquisFileDetailList.Count,
                        date = item.CreateDate
                    }).ToList();
                }

                list.ForEach(x => x.persianDate = DateUtility.GetPersianDateTime(x.date));

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
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// لیست نام فایل های مارکیز شعبه
        /// </summary>
        /// <param name="id">ردیف فایل مارکیز</param>
        /// <returns>لیست فایل ها</returns>
        [HttpGet]
        [Authorize(Roles = "admin, marquisFileDownload")]
        public JsonResult GetBranchDetailFiles(int id)
        {
            Response response;
            try
            {
                var user = GetAuthenticatedUser();
                List<MarquisFileDetailViewModel> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.MarquisFileDetail.Where(x => x.MarquisFileId == id && x.MarquisFile.BranchId == user.BranchId).Select(x => new MarquisFileDetailViewModel()
                    {
                        id = x.Id,
                        marquisFileId = x.MarquisFileId,
                        fileId = x.FileId,
                        fileName = x.FileName
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new { list }
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
        /// دانلود فایل
        /// </summary>
        /// <param name="fileId">ردیف فایل</param>
        /// <param name="fileName">نام فایل</param>
        /// <returns>فایل درخواست شده برای دانلود</returns>
        [Authorize(Roles = "admin, marquisFileUpload, marquisFileDownload")]
        public ActionResult DownloadFile(string fileId, string fileName)
        {
            try
            {
                using (var db = new KiaGalleryFileContext())
                {
                    var file = db.File.Single(x => x.Id == fileId && x.FileName == fileName);
                    return File(file.Data, file.MimeType, file.FileName);
                }
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return null;
            }
        }

        /// <summary>
        /// حذف فایل
        /// </summary>
        /// <param name="id">ردیف فایل</param>
        /// <returns>نتیجه حذف</returns>
        [HttpPost]
        [Authorize(Roles = "admin, marquisFileUpload")]
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.MarquisFile.Single(x => x.Id == id);

                    db.MarquisFileDetail.RemoveRange(entity.MarquisFileDetailList);
                    db.MarquisFile.Remove(entity);
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200,
                    message = "فایل با موفقیت حذف شد."
                };

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ویرایش توضیحات
        /// </summary>
        /// <param name="id">ردیف فایل</param>
        /// <param name="description">توضیحات</param>
        /// <returns>نتیجه ویرایش توضیحات</returns>
        [HttpPost]
        [Authorize(Roles = "admin, marquisFileUpload")]
        public JsonResult ChangeDescription(int id, string description)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.MarquisFile.Single(x => x.Id == id);

                    entity.Description = description;
                    entity.ModifyUserId = GetAuthenticatedUserId();
                    entity.ModifyDate = DateTime.Now;
                    entity.Ip = Request.UserHostAddress;

                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200,
                    message = "توضیحات با موفقیت ویرایش شد."
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