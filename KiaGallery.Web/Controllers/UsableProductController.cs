using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class UsableProductController : BaseController
    {
        // GET: UsableProduct
        [Authorize(Roles = "admin , createUsableProduct")]

        public ActionResult Index()
        {
            ViewBag.Title = "محصولات چاپخانه";
            return View();
        }
        /// <summary>
        /// ویرایش محصول
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , createUsableProduct")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// اضافه کردن محصول
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin , createUsableProduct")]
        public ActionResult Add()
        {
            return View("Edit");
        }
        /// <summary>
        /// ذخیره محصول
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult Save(UsableProductViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    List<string> excludeFileName = new List<string>();
                    var userId = GetAuthenticatedUserId();
                    if (model.id != null && model.id > 0)
                    {
                        var product = db.UsableProduct.Single(x => x.Id == model.id);
                        product.CategoryUsableProductId = model.categoryChild;
                        product.Name = model.name;
                        product.PrintingHouseId = model.printingHouseId;
                        product.Code = model.code;
                        product.Unit = model.unit;
                        product.Description = model.description;
                        //product.Available = model.available;
                        product.Order = model.order;
                        product.UsableProductStatus = model.usableProductStatus;
                        product.ModifyUserId = userId;
                        product.ModifyDate = DateTime.Now;
                        product.Ip = Request.UserHostAddress;
                    }
                    else
                    {
                        var item = new UsableProduct()
                        {
                            CategoryUsableProductId = model.categoryChild,
                            PrintingHouseId = model.printingHouseId,
                            Name = model.name,
                            Code = model.code,
                            Unit = model.unit,
                            Description = model.description,
                            //Available = model.available,
                            Order = model.order,
                            UsableProductStatus = model.usableProductStatus,
                            CreateUserId = userId,
                            ModifyUserId = userId,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress,
                        };
                        db.UsableProduct.Add(item);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "محصول جدید ثبت شد",
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveFile(UsableProductViewModel model)
        {
            Response response;
            List<string> excludeFileName = new List<string>();
            var user = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.UsableProduct.Single(x => x.Id == model.id);
                    if (model.fileList != null && model.fileList.Count > 0)
                    {
                        var fileNameList = model.fileList.Select(x => x.fileName).ToList(); // جدا کردن نام فایل های برای جستوجو در LINQ
                        var excludeFile = entity.UsableProductFileList.Where(x => !fileNameList.Any(y => y == x.FileName)).ToList(); // پیدا کردن فایل های حذف شده
                        excludeFileName = excludeFile.Select(x => x.FileName).ToList(); // جدا کردن نام فایل های حذف شده برای پاک کردن فایل فیزیکی
                        excludeFile.ForEach(x => db.UsableProductFile.Remove(x)); // حذف فایل های حذف شده از دیتابیس
                        model.fileList.ForEach(file =>
                        {
                            var item = entity.UsableProductFileList.SingleOrDefault(x => x.FileName == file.fileName); // بررسی عدم وجود تصویر در صورت اضافه شدن تصویر و ذخیره در دیتابیس
                            if (item == null)
                            {
                                var fileItem = new UsableProductFile()
                                {
                                    UsableProduct = entity,
                                    FileName = file.fileName,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };
                                db.UsableProductFile.Add(fileItem);
                            }
                        });
                    }
                    else
                    {
                        db.UsableProductFile.RemoveRange(entity.UsableProductFileList);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "عکس محصول با موفقیت ذخیره شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadFile(int id)
        {
            Response response;
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    var item = db.UsableProduct.SingleOrDefault(x => x.Id == id);
                    var fileList = item.UsableProductFileList.Select(x => new UsableProductFileViewModel()
                    {
                        id = x.Id,
                        fileName = x.FileName,
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            fileList = fileList
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
        public JsonResult UploadFile(string type, int id)
        {
            Response response;
            try
            {
                string fileName = string.Empty;
                string serverPath = Server.MapPath("~/Upload/UsableProductFile");
                HttpPostedFileBase hpf = Request.Files[0];
                string originalFileName = Path.GetFileName(hpf.FileName);

                string fileExtension = Path.GetExtension(originalFileName);

                if (hpf.ContentLength == 0)
                    throw new Exception("File length can't be equal to zero");


                string savedFileName = Path.Combine(serverPath, fileName);

                if (System.IO.File.Exists(savedFileName))
                {
                    Random random = new Random();
                    string prefix = random.Next(1000, 9999).ToString();
                    fileName = "(" + prefix + ")" + fileName;
                }
                savedFileName = Path.Combine(serverPath, fileName);

                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }

                hpf.SaveAs(savedFileName);

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        name = fileName,
                        length = hpf.ContentLength,
                        type = hpf.ContentType
                    },
                    message = "بارگذاری فایل با موفقیت انجام شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// خواندن اطلاعات محصول
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون اطلاعات لود شده پرسنل</returns>
        [HttpGet]
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                UsableProduct item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.UsableProduct.FirstOrDefault(x => x.Id == id);
                    var data = new UsableProductViewModel
                    {
                        id = item.Id,
                        categoryUsableProductId = item.CategoryUsableProduct.ParentId.Value,
                        categoryChild = item.CategoryUsableProductId,
                        printingHouseId = item.PrintingHouseId,
                        code = item.Code,
                        name = item.Name,
                        order = item.Order,
                        usableProductStatus = item.UsableProductStatus,
                        unit = item.Unit,
                        description = item.Description,
                        //available = item.Available,
                    };
                    response = new Response
                    {
                        status = 200,
                        data = data,
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
        /// لیست محصولات
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult Search(UsableProductSearchViewModel model)

        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.UsableProduct.OrderBy(x => x.CategoryUsableProduct.Parent.Order).ThenBy(x => x.CategoryUsableProduct.Order).Select(x => x);
                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.Name.ToString().Contains(model.term.Trim()) || x.Code.ToString().Contains(model.term.Trim()) || x.CategoryUsableProduct.Parent.Title.ToString().Contains(model.term.Trim()) || x.CategoryUsableProduct.Title.Contains(model.term.Trim()));
                    }
                    //if (model.printigHouseStatus == true)
                    //{
                    //    query = query.Where(x => x.PrintingHouse.Active == false);
                    //}

                    dataCount = query.Count();
                    //query = query.;
                    var list = query.Select(x => new UsableProductViewModel()
                    {
                        id = x.Id,
                        name = x.Name,
                        parentTitle = x.CategoryUsableProduct.Parent.Title,
                        childTitle = x.CategoryUsableProduct.Title,
                        code = x.Code,
                        image = x.UsableProductFileList.Select(y => y.FileName).FirstOrDefault(),
                        order = x.Order,
                        usableProductStatus = x.UsableProductStatus,

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.usableProductStatusTitle = Enums.GetTitle(x.usableProductStatus);
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
        /// <summary>
        /// حذف محصول
        /// </summary>
        /// <param name="id">ردیف پرسنل</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var item = db.UsableProduct.Find(Id);
                    if (item.UsableProductCartList.Count() == 0)
                    {
                        db.UsableProductFile.RemoveRange(item.UsableProductFileList);
                        db.UsableProduct.Remove(item);
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                            message = "محصول با موفقیت حذف شد."
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "برای این محصول سفارشی ثبت شده است ."
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
        /// دریافت همه دسته بندی سوالات
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult GetAllCategory()
        {
            Response response;
            try
            {
                List<CategoryUsableProduct> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.CategoryUsableProduct.Where(x => x.ParentId == null).OrderBy(x => x.Order).ToList();
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
        /// دریافت زیردسته بندی های چاپخانه
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult GetAllChild(int? id)
        {
            Response response;
            try
            {
                List<CategoryUsableProduct> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.CategoryUsableProduct.Where(x => x.ParentId == id).OrderBy(x => x.Order).ToList();
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
        /// دریافت همه چاپخانه
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin , createUsableProduct")]
        public JsonResult GetAllPrintingHouse()
        {
            Response response;
            try
            {
                List<PrintingHouse> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.PrintingHouse.Select(x => x).ToList();
                }
                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            name = item.Name,
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
    }
}