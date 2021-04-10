using ClosedXML.Excel;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class ProductController : BaseController
    {
        /// <summary>
        /// مدیریت محصولات
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, product")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.CollectionList = db.ProductCollection.Select(x => new SelectOptionViewModel { id = x.Id, title = x.Title }).ToList();
            }
            return View();
        }

        /// <summary>
        /// ویرایش محصول
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, product")]
        public ActionResult Edit(int id)
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.StoneList = db.Stone.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.LeatherList = db.Leather.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.CollectionList = db.ProductCollection.Where(x => x.Active == true).OrderBy(x => x.Id).ToList();
            }
            ViewBag.Id = id;
            ViewBag.Title = "ویرایش محصول";
            return View();
        }

        /// <summary>
        /// محصول جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, product")]
        public ActionResult Add()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.StoneList = db.Stone.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.LeatherList = db.Leather.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                ViewBag.CollectionList = db.ProductCollection.Where(x => x.Active == true).OrderBy(x => x.Id).ToList();

            }
            ViewBag.Title = "محصول جدید";
            return View("Edit");
        }

        /// <summary>
        /// محصولات مرتبط
        /// </summary>
        /// <param name="id">شناسه مورد نظر</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, product")]
        public ActionResult Related(int id)
        {
            ViewBag.Id = id;
            using (var db = new KiaGalleryContext())
            {
                var product = db.Product.Where(x => x.Id == id).Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Code,
                    x.BookCode,
                    FileName = x.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName
                }).Single();

                ViewBag.ProductFileName = product.FileName;
                ViewBag.ProductTitle = product.Title;
                ViewBag.ProductBookCode = product.BookCode;
                ViewBag.ProductCode = product.Code;

                var relatedProduct = db.RelatedProduct.Where(x => x.ProductId == id).Select(x => new RelatedProductViewModel()
                {
                    Id = x.Id,
                    ProductId = x.RelatedTo.Id,
                    ProductTitle = x.RelatedTo.Title,
                    ProductCode = x.RelatedTo.Code,
                    ProductBookCode = x.RelatedTo.BookCode,
                    ProductFileName = x.RelatedTo.ProductFileList.Where(y => y.FileType == FileType.WhiteBack).FirstOrDefault().FileName
                }).ToList();
                ViewBag.RelatedProduct = relatedProduct;
            }
            return View();
        }

        /// <summary>
        /// محصولات ست
        /// </summary>
        /// <param name="id">شناسه مورد نظر</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, product")]
        public ActionResult Set(int id)
        {
            ViewBag.Id = id;
            using (var db = new KiaGalleryContext())
            {
                var product = db.Product.Where(x => x.Id == id).Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Code,
                    x.BookCode,
                    FileName = x.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName
                }).Single();

                ViewBag.ProductFileName = product.FileName;
                ViewBag.ProductTitle = product.Title;
                ViewBag.ProductBookCode = product.BookCode;
                ViewBag.ProductCode = product.Code;

                var setProduct = db.SetProduct.Where(x => x.ProductId == id).Select(x => new RelatedProductViewModel()
                {
                    Id = x.Id,
                    ProductId = x.SetTo.Id,
                    ProductTitle = x.SetTo.Title,
                    ProductCode = x.SetTo.Code,
                    ProductBookCode = x.SetTo.BookCode,
                    ProductFileName = x.SetTo.ProductFileList.Where(y => y.FileType == FileType.WhiteBack).FirstOrDefault().FileName
                }).ToList();

                ViewBag.SetProduct = setProduct;
            }
            return View();
        }

        /// <summary>
        /// صفحه شخصی سازی برای سفارش محصول
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin, order, orderOuterWerk")]
        public ActionResult Customize(int Id)
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.StoneList = db.Stone.Include(x => x.StoneOutOfStockList).OrderBy(x => x.Order).ToList();
                ViewBag.LeatherList = db.Leather.OrderBy(x => x.Order).ToList();
                var product = db.Product.Include(x => x.ProductOuterWerkList).Include(x => x.NormalSizeValue).Include(x => x.ProductFileList).Include(x => x.ProductStoneList).Include(x => x.ProductLeatherList).Include(x => x.Size).Include(x => x.Size.SizeValueList).SingleOrDefault(x => x.Id == Id);
                ViewBag.Product = product;
                var productId = product.Id;
                ViewBag.RelatedProduct = db.RelatedProduct.Include(x => x.RelatedTo.ProductOuterWerkList).Include(x => x.RelatedTo.ProductFileList).Include(x => x.RelatedTo.ProductStoneList).Include(x => x.RelatedTo.ProductLeatherList).Include(x => x.RelatedTo.Size).Include(x => x.RelatedTo.Size.SizeValueList).Where(x => x.ProductId == productId).ToList();
                ViewBag.SetProduct = db.SetProduct.Include(x => x.SetTo.ProductOuterWerkList).Include(x => x.SetTo.ProductFileList).Include(x => x.SetTo.ProductStoneList).Include(x => x.SetTo.ProductLeatherList).Include(x => x.SetTo.Size).Include(x => x.SetTo.Size.SizeValueList).Where(x => x.ProductId == productId).ToList();
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
            }
            return View("_Customize");
        }

        /// <summary>
        /// ذخیره محصول
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات محصول</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, product")]
        public JsonResult Save(ProductViewModel model)
        {
            Response response;
            try
            {
                List<string> excludeFileName = new List<string>();
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    if (string.IsNullOrEmpty(model.bookCode))
                    {
                        status = 500;
                        message = "وارد کردن کد کتاب کد محصول اجباری است.";
                    }
                    else if (string.IsNullOrEmpty(model.title))
                    {
                        status = 500;
                        message = "وارد کردن نام محصول اجباری است.";
                    }
                    else
                    {
                        var user = GetAuthenticatedUser();
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.Product.Single(x => x.Id == model.id);
                            if (model.bookCode != entity.BookCode && db.Product.Count(x => x.BookCode == model.bookCode.Trim() && x.Id != model.id) > 0)
                            {
                                status = 500;
                                message = "کد کتاب کد وارد شده تکراری است.";
                            }
                            else
                            {
                                entity.Code = model.code?.Trim();
                                entity.WorkshopTagId = model.workshopTagId;
                                entity.OnlyForWorkshop = model.onlyForWorkshop;
                                entity.EarringBack = model.earringBack;
                                entity.OrderableBranchType = model.orderableBranchType;
                                entity.OrderableCoWorkerType = model.orderableCoWorkerType;
                                entity.OrderableSolicitorshipType = model.orderableSolicitorshipType;
                                entity.OrderableOtherType = model.orderableOtherType;
                                entity.GoldColor = model.goldColor;
                                entity.RosegoldColor = model.rosegoldColor;
                                entity.WhiteColor = model.whiteColor;
                                entity.SpecialProduct = model.specialProduct;
                                entity.NotOrderable = model.notOrderable;
                                //entity.NotOrderableForBranch = model.notOrderableForBranch;
                                entity.ProductCollectionId = model.productCollectionId;
                                entity.BookCode = model.bookCode.Trim();
                                entity.Title = model.title.Trim();
                                entity.ProductType = model.productType;
                                entity.RingSizeType = model.ringSizeType;
                                entity.OuterWerkCategory = model.outerWerkCategory;
                                entity.GoldType = model.goldType;
                                entity.Sex = model.sex;
                                entity.WorkshopId = model.workshopId;
                                entity.WorkshopId2 = model.workshopId2;
                                entity.Weight = model.weight;
                                entity.SilverWeight = model.silverWeight;
                                entity.StoneCount = model.stoneCount;
                                entity.StonePrice = model.stonePrice;
                                entity.LeatherCount = model.leatherCount;
                                entity.LeatherPrice = model.leatherPrice;
                                entity.SizeId = model.sizeId;
                                entity.NormalSizeValueId = model.normalSizeValueId;
                                entity.Wage = model.wage;
                                entity.OuterWerkPlacement = model.outerWerkPlacement;
                                entity.CanLoop = model.canLoop;
                                entity.Active = model.active;
                                entity.New = model.newDay;
                                entity.ModifyUserId = user.Id;
                                entity.ModifyDate = DateTime.Now;
                                entity.Ip = Request.UserHostAddress;

                                if (model.fileList != null && model.fileList.Count > 0)
                                {
                                    var fileNameList = model.fileList.Select(x => x.fileName).ToList(); // جدا کردن نام فایل های برای جستوجو در LINQ
                                    var excludeFile = entity.ProductFileList.Where(x => !fileNameList.Any(y => y == x.FileName)).ToList(); // پیدا کردن فایل های حذف شده
                                    excludeFileName = excludeFile.Select(x => x.FileName).ToList(); // جدا کردن نام فایل های حذف شده برای پاک کردن فایل فیزیکی
                                    excludeFile.ForEach(x => db.ProductFile.Remove(x)); // حذف فایل های حذف شده از دیتابیس
                                    model.fileList.ForEach(file =>
                                    {
                                        var item = entity.ProductFileList.SingleOrDefault(x => x.FileName == file.fileName && x.FileType == file.fileType); // بررسی عدم وجود تصویر در صورت اضافه شدن تصویر و ذخیره در دیتابیس
                                        if (item == null)
                                        {
                                            var fileItem = new ProductFile()
                                            {
                                                Product = entity,
                                                FileName = file.fileName,
                                                FileType = file.fileType,
                                                PaddingImg = file.paddingImg,
                                                CreateUserId = user.Id,
                                                ModifyUserId = user.Id,
                                                CreateDate = DateTime.Now,
                                                ModifyDate = DateTime.Now,
                                                Ip = Request.UserHostAddress
                                            };
                                            db.ProductFile.Add(fileItem);
                                        }
                                        else
                                            item.PaddingImg = file.paddingImg;
                                    });
                                }
                                else
                                {
                                    db.ProductFile.RemoveRange(entity.ProductFileList);
                                }

                                if (model.stoneList != null && model.stoneList.Count > 0)
                                {
                                    model.stoneList.GroupBy(x => x.order).Select(x => new
                                    {
                                        order = x.Key,
                                        stoneList = x.ToList()
                                    }).ToList().ForEach(groupedItem =>
                                    {
                                        var stoneIdList = groupedItem.stoneList.Select(x => x.stoneId).ToList();
                                        var excludeStone = entity.ProductStoneList.Where(x => x.Order == groupedItem.order && !stoneIdList.Any(y => y == x.StoneId)).ToList(); // پیدا کردن سنگ های حذف شده
                                        excludeStone.ForEach(x => db.ProductStone.Remove(x)); // حذف سنگ های حذف شده از دیتابیس

                                        groupedItem.stoneList.ForEach(stone =>
                                        {
                                            var item = entity.ProductStoneList.SingleOrDefault(x => x.StoneId == stone.stoneId && x.Order == stone.order); // بررسی عدم وجود سنگ، و اضافه شدن سنگ و ذخیره در دیتابیس
                                            if (item == null)
                                            {
                                                var stoneItem = new ProductStone()
                                                {
                                                    Product = entity,
                                                    Order = stone.order,
                                                    StoneId = stone.stoneId,
                                                    DefaultStoneId = stone.defaultStoneId,
                                                    StoneShape = stone.stoneShape,
                                                    ShapeSizeId = stone.shapeSizeId
                                                };
                                                db.ProductStone.Add(stoneItem);
                                            }
                                            else
                                            {
                                                item.DefaultStoneId = stone.defaultStoneId;
                                                item.StoneShape = stone.stoneShape;
                                                item.ShapeSizeId = stone.shapeSizeId;
                                            }
                                        });
                                    });

                                    var orderList = model.stoneList.Select(x => x.order).Distinct();
                                    var excludeOrderItem = entity.ProductStoneList.Where(x => !orderList.Any(y => y == x.Order)).ToList();
                                    db.ProductStone.RemoveRange(excludeOrderItem);
                                }
                                else
                                {
                                    db.ProductStone.RemoveRange(entity.ProductStoneList);
                                }

                                if (model.leatherList != null && model.leatherList.Count > 0)
                                {
                                    model.leatherList.GroupBy(x => x.order).Select(x => new
                                    {
                                        order = x.Key,
                                        leatherList = x.ToList()
                                    }).ToList().ForEach(groupedItem =>
                                    {
                                        var leatherIdList = groupedItem.leatherList.Select(x => x.leatherId).ToList();
                                        var excludeLeather = entity.ProductLeatherList.Where(x => x.Order == groupedItem.order && !leatherIdList.Any(y => y == x.LeatherId)).ToList(); // پیدا کردن چرم های حذف شده
                                        excludeLeather.ForEach(x => db.ProductLeather.Remove(x)); // حذف چرم های حذف شده از دیتابیس

                                        groupedItem.leatherList.ForEach(leather =>
                                        {
                                            var item = entity.ProductLeatherList.SingleOrDefault(x => x.LeatherId == leather.leatherId && x.Order == leather.order); // بررسی عدم وجود چرم، و اضافه شدن چرم و ذخیره در دیتابیس
                                            if (item == null)
                                            {
                                                var leatherItem = new ProductLeather()
                                                {
                                                    Product = entity,
                                                    Order = leather.order,
                                                    LeatherId = leather.leatherId
                                                };
                                                db.ProductLeather.Add(leatherItem);
                                            }
                                        });
                                    });

                                    var orderList = model.leatherList.Select(x => x.order).Distinct();
                                    var excludeOrderItem = entity.ProductLeatherList.Where(x => !orderList.Any(y => y == x.Order)).ToList();
                                    db.ProductLeather.RemoveRange(excludeOrderItem);
                                }
                                else
                                {
                                    db.ProductLeather.RemoveRange(entity.ProductLeatherList);
                                }

                                if (model.outerWerkTypeList != null && model.outerWerkTypeList.Count > 0)
                                {
                                    model.outerWerkTypeList.ForEach(x =>
                                    {
                                        var item = entity.ProductOuterWerkList.SingleOrDefault(y => y.OuterWerkType == x);
                                        if (item == null)
                                        {
                                            var productOuterWerk = new ProductOuterWerk()
                                            {
                                                Product = entity,
                                                OuterWerkType = x
                                            };
                                            db.ProductOuterWerk.Add(productOuterWerk);
                                        }
                                    });
                                    var excludeProductOuterWerkItem = entity.ProductOuterWerkList.Where(x => !model.outerWerkTypeList.Any(y => y == x.OuterWerkType)).ToList();
                                    db.ProductOuterWerk.RemoveRange(excludeProductOuterWerkItem);
                                }
                                else
                                {
                                    db.ProductOuterWerk.RemoveRange(entity.ProductOuterWerkList);
                                }

                                status = 200;
                                message = "محصول با موفقیت ویرایش شد.";
                            }
                        }
                        else
                        {
                            if (db.Product.Count(x => x.BookCode == model.bookCode.Trim()) > 0)
                            {
                                status = 500;
                                message = "کد کتاب کد وارد شده تکراری است.";
                            }
                            else
                            {
                                var entity = new Product()
                                {
                                    Code = model.code?.Trim(),
                                    WorkshopTagId = model.workshopTagId,
                                    OnlyForWorkshop = model.onlyForWorkshop,
                                    EarringBack = model.earringBack,
                                    OrderableBranchType = model.orderableBranchType,
                                    OrderableCoWorkerType = model.orderableCoWorkerType,
                                    OrderableSolicitorshipType = model.orderableSolicitorshipType,
                                    OrderableOtherType = model.orderableOtherType,
                                    GoldColor = model.goldColor,
                                    RosegoldColor = model.rosegoldColor,
                                    WhiteColor = model.whiteColor,
                                    SpecialProduct = model.specialProduct,
                                    NotOrderable = model.notOrderable,
                                    //NotOrderableForBranch = model.notOrderableForBranch,
                                    BookCode = model.bookCode.Trim(),
                                    Title = model.title.Trim(),
                                    ProductCollectionId = model.productCollectionId,
                                    ProductType = model.productType,
                                    RingSizeType = model.ringSizeType,
                                    OuterWerkCategory = model.outerWerkCategory,
                                    GoldType = model.goldType,
                                    Sex = model.sex,
                                    WorkshopId = model.workshopId,
                                    WorkshopId2 = model.workshopId2,
                                    Weight = model.weight,
                                    SilverWeight = model.silverWeight,
                                    StoneCount = model.stoneCount,
                                    StonePrice = model.stonePrice,
                                    LeatherCount = model.leatherCount,
                                    LeatherPrice = model.leatherPrice,
                                    SizeId = model.sizeId,
                                    NormalSizeValueId = model.normalSizeValueId,
                                    Wage = model.wage,
                                    OuterWerkPlacement = model.outerWerkPlacement,
                                    CanLoop = model.canLoop,
                                    Active = model.active,
                                    New = model.newDay,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };


                                var fileList = model.fileList?.Select(x => new ProductFile()
                                {

                                    Product = entity,
                                    FileName = x.fileName,
                                    FileType = x.fileType,
                                    PaddingImg = x.paddingImg,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                }).ToList();

                                var stoneList = model.stoneList?.Select(x => new ProductStone()
                                {
                                    Product = entity,
                                    Order = x.order,
                                    StoneId = x.stoneId,
                                    DefaultStoneId = x.defaultStoneId,
                                    StoneShape = x.stoneShape,
                                    ShapeSizeId = x.shapeSizeId
                                }).ToList();

                                var leatherList = model.leatherList?.Select(x => new ProductLeather()
                                {
                                    Product = entity,
                                    Order = x.order,
                                    LeatherId = x.leatherId
                                }).ToList();

                                var productOuterWerkList = model.outerWerkTypeList?.Select(x => new ProductOuterWerk()
                                {
                                    Product = entity,
                                    OuterWerkType = x
                                }).ToList();
                                db.Product.Add(entity);
                                if (fileList != null && fileList.Count > 0)
                                    db.ProductFile.AddRange(fileList);
                                if (stoneList != null && stoneList.Count > 0)
                                    db.ProductStone.AddRange(stoneList);
                                if (leatherList != null && leatherList.Count > 0)
                                    db.ProductLeather.AddRange(leatherList);
                                if (productOuterWerkList != null && productOuterWerkList.Count > 0)
                                    db.ProductOuterWerk.AddRange(productOuterWerkList);
                                message = "محصول با موفقیت ایجاد شد.";
                            }
                        }
                        db.SaveChanges();


                        if (excludeFileName != null && excludeFileName.Count > 0)
                        {
                            excludeFileName.ForEach(item =>
                            {
                                string serverPath = Server.MapPath("~/Upload/Product");
                                if (System.IO.File.Exists(Path.Combine(serverPath, item)))
                                    System.IO.File.Delete(Path.Combine(serverPath, item));
                            });
                        }
                    }
                }

                response = new Response()
                {
                    status = status,
                    message = message
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveFile(ProductViewModel model)
        {
            Response response;
            List<string> excludeFileName = new List<string>();
            var user = GetAuthenticatedUser();
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Product.Single(x => x.Id == model.id);
                    entity.ModifyDate = DateTime.Now;
                    if (model.fileList != null && model.fileList.Count > 0)
                    {
                        var fileNameList = model.fileList.Select(x => x.fileName).ToList(); // جدا کردن نام فایل های برای جستوجو در LINQ
                        var excludeFile = entity.ProductFileList.Where(x => !fileNameList.Any(y => y == x.FileName)).ToList(); // پیدا کردن فایل های حذف شده
                        excludeFileName = excludeFile.Select(x => x.FileName).ToList(); // جدا کردن نام فایل های حذف شده برای پاک کردن فایل فیزیکی
                        excludeFile.ForEach(x => db.ProductFile.Remove(x)); // حذف فایل های حذف شده از دیتابیس
                        model.fileList.ForEach(file =>
                        {
                            var item = entity.ProductFileList.SingleOrDefault(x => x.FileName == file.fileName && x.FileType == file.fileType); // بررسی عدم وجود تصویر در صورت اضافه شدن تصویر و ذخیره در دیتابیس
                            if (item == null)
                            {
                                var fileItem = new ProductFile()
                                {
                                    Product = entity,
                                    FileName = file.fileName,
                                    FileType = file.fileType,
                                    PaddingImg = file.paddingImg,
                                    CreateUserId = user.Id,
                                    ModifyUserId = user.Id,
                                    CreateDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    Ip = Request.UserHostAddress
                                };
                                db.ProductFile.Add(fileItem);
                            }
                            else
                                item.PaddingImg = file.paddingImg;
                        });
                    }
                    else
                    {
                        db.ProductFile.RemoveRange(entity.ProductFileList);
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

        /// <summary>
        /// خواندن اطلاعات 
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns>جیسون اطلاعات لود شده محصول</returns>
        [HttpGet]
        [Authorize(Roles = "admin, product")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Product item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Product.Include(x => x.ProductFileList).Include(x => x.ProductStoneList).Include(x => x.ProductLeatherList).Include(x => x.ProductOuterWerkList).First(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new ProductViewModel
                        {
                            id = item.Id,
                            notOrderable = item.NotOrderable,
                            goldColor = item.GoldColor,
                            workshopTagId = item.WorkshopTagId,
                            onlyForWorkshop = item.OnlyForWorkshop,
                            earringBack = item.EarringBack,
                            orderableBranchType = item.OrderableBranchType,
                            orderableCoWorkerType = item.OrderableCoWorkerType,
                            orderableSolicitorshipType = item.OrderableSolicitorshipType,
                            orderableOtherType = item.OrderableOtherType,
                            rosegoldColor = item.RosegoldColor,
                            whiteColor = item.WhiteColor,
                            //notOrderableForBranch = item.NotOrderableForBranch,
                            productCollectionId = item.ProductCollectionId,
                            code = item.Code,
                            specialProduct = item.SpecialProduct,
                            bookCode = item.BookCode,
                            title = item.Title,
                            productType = item.ProductType,
                            ringSizeType = item.RingSizeType,
                            outerWerkCategory = item.OuterWerkCategory,
                            goldType = item.GoldType,
                            sex = item.Sex,
                            workshopId = item.WorkshopId,
                            workshopId2 = item.WorkshopId2,
                            weight = item.Weight,
                            silverWeight = item.SilverWeight,
                            stoneCount = item.StoneCount,
                            stonePrice = item.StonePrice,
                            leatherCount = item.LeatherCount,
                            leatherPrice = item.LeatherPrice,
                            sizeId = item.SizeId,
                            normalSizeValueId = item.NormalSizeValueId,
                            wage = item.Wage,
                            outerWerkPlacement = item.OuterWerkPlacement,
                            canLoop = item.CanLoop,
                            active = item.Active,
                            newDay = item.New,

                            fileList = item.ProductFileList.Select(x => new ProductFileViewModel()
                            {
                                id = x.Id,
                                fileName = x.FileName,
                                fileType = x.FileType,
                                paddingImg = x.PaddingImg
                            }).ToList(),
                            stoneList = item.ProductStoneList.Select(x => new ProductStoneViewModel()
                            {
                                id = x.Id,
                                stoneId = x.StoneId,
                                defaultStoneId = x.DefaultStoneId,
                                order = x.Order,
                                stoneShape = x.StoneShape,
                                shapeSizeId = x.ShapeSizeId
                            }).ToList(),
                            leatherList = item.ProductLeatherList.Select(x => new ProductLeatherViewModel()
                            {
                                id = x.Id,
                                leatherId = x.LeatherId,
                                order = x.Order
                            }).ToList(),
                            outerWerkTypeList = item.ProductOuterWerkList.Select(x => x.OuterWerkType).ToList()
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "محصول مورد نظر یافت نشد."
                    };
                }

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
                    var item = db.Product.SingleOrDefault(x => x.Id == id);
                    var fileList = item.ProductFileList.Select(x => new ProductFileViewModel()
                    {
                        id = x.Id,
                        fileName = x.FileName,
                        fileType = x.FileType,
                        paddingImg = x.PaddingImg
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

        /// <summary>
        /// جستجوی محصول
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست محصولات پیدا شده</returns>
        [Authorize(Roles = "admin, product, order, orderOuterWerk, order-workshop, OuterWerkPlacement ,manageImage ,allOrder")]
        public JsonResult Search(ProductSearchViewModel model)
        {
            Response response;
            try
            {
                List<ResponseProductSearchViewModel> list;
                int dataCount;
                var branchId = GetAuthenticatedUser().BranchId;
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Product.Select(x => x);

                    if (User.IsInRole("admin") || User.IsInRole("product"))
                    {
                        query = query;
                    }
                    else if (currentUser.WorkshopId != null)
                    {
                        query = query.Where(x => x.NotOrderable != false);
                    }
                    else if (currentUser.BranchId != null)
                    {
                        query = query.Where(x => x.NotOrderable != true);
                    }
                    if (model.outerwerkPage == true)
                    {
                        query = query.Where(x => x.ProductType == ProductType.OuterWerk);
                    }
                    if (model.outerwerkPage == false)
                    {
                        query = query.Where(x => x.ProductType != ProductType.OuterWerk);
                    }

                    //if (!User.IsInRole("showNotOrderableProductForBranch") && !User.IsInRole("admin"))
                    //{
                    //    query = query.Where(x => x.NotOrderableForBranch != true);
                    //}
                    if (User.IsInRole("stoneOuterwerk") && !User.IsInRole("leatherOuterwerk"))
                    {
                        query = query.Where(x => x.OuterWerkCategory == OuterWerkCategory.Stone || x.OuterWerkCategory == OuterWerkCategory.Both || x.OuterWerkCategory == null);
                    }
                    if (User.IsInRole("leatherOuterwerk") && !User.IsInRole("stoneOuterwerk"))
                    {
                        query = query.Where(x => x.OuterWerkCategory == OuterWerkCategory.Leather || x.OuterWerkCategory == OuterWerkCategory.Both || x.OuterWerkCategory == null);
                    }
                    if (User.IsInRole("leatherOuterwerk") && User.IsInRole("stoneOuterwerk"))
                    {
                        query = query.Where(x => x.OuterWerkCategory == OuterWerkCategory.Leather || x.OuterWerkCategory == OuterWerkCategory.Both || x.OuterWerkCategory == OuterWerkCategory.Stone || x.OuterWerkCategory == null);
                    }
                    if (!User.IsInRole("plaque") && !User.IsInRole("admin") && !User.IsInRole("allOrder"))
                    {
                        query = query.Where(x => x.ProductType != ProductType.Plaque);
                    }
                    if (!User.IsInRole("admin") && !User.IsInRole("onlyForWorkshop"))
                    {
                        query = query.Where(x => x.OnlyForWorkshop != true);
                    }
                    //if (User.IsInRole("leatherProductUser"))
                    //{
                    //    query = query.Where(x => x.WorkshopId2 != null && x.WorkshopId2 == 5);
                    //}

                    if (model.isOuterWerk == true)
                    {
                        query = query.Where(x => !string.IsNullOrEmpty(x.OuterWerkPlacement));
                    }
                    if (model.collectionId != null && model.collectionId > 0)
                    {
                        query = query.Where(x => x.ProductCollectionId == model.collectionId);
                    }

                    if (model.fav)
                    {
                        //var branchId = GetAuthenticatedUser().BranchId;
                        query = query.Where(x => x.FavouritesProductList.Count(y => y.BranchId == branchId) > 0);
                    }
                    else if (model.workshopId != null && model.workshopId > 0)
                    {
                        query = query.Where(x => x.WorkshopId == model.workshopId);
                    }

                    if (!string.IsNullOrEmpty(model.term?.Trim()))
                    {
                        query = query.Where(x => x.Code.Contains(model.term.Trim()) || x.BookCode.Contains(model.term.Trim()) || x.Title.Contains(model.term.Trim()) || x.Title.Contains(model.term.Trim().Replace("ی", "ي").Replace("ک", "ك")));
                    }

                    if (model.type != null && model.type.Count > 0)
                    {
                        query = query.Where(x => model.type.Any(y => (ProductType)y == x.ProductType));
                    }

                    if (model.leatherBracelet == true)
                    {
                        query = query.Where(x => x.LeatherPrice > 0);
                    }

                    if (model.isNew == true)
                    {
                        query = query.Where(x => DbFunctions.AddDays(x.ModifyDate, x.New) >= DateTime.Now);
                    }

                    if (model.related == true)
                    {
                        query = query.Where(x => x.RelatedProduct.Count() > 0);
                    }

                    if (model.set == true)
                    {
                        query = query.Where(x => x.SetProduct.Count() > 0);
                    }

                    if (model.sex != null && model.sex.Count > 0)
                    {
                        if (model.sex.Any(y => (Sex)y == Sex.Man) || model.sex.Any(y => (Sex)y == Sex.Woman))
                            query = query.Where(x => model.sex.Any(y => (Sex)y == x.Sex) || x.Sex == Sex.Both);
                        else
                            query = query.Where(x => model.sex.Any(y => (Sex)y == x.Sex));
                    }
                    if (model.outerWerkCategory != null && model.outerWerkCategory.Count > 0)
                    {
                        if (model.outerWerkCategory.Any(y => (OuterWerkCategory)y == OuterWerkCategory.Leather) || model.outerWerkCategory.Any(y => (OuterWerkCategory)y == OuterWerkCategory.Stone))
                            query = query.Where(x => model.outerWerkCategory.Any(y => (OuterWerkCategory)y == x.OuterWerkCategory) || x.OuterWerkCategory == OuterWerkCategory.Both);
                        else
                            query = query.Where(x => model.outerWerkCategory.Any(y => (OuterWerkCategory)y == x.OuterWerkCategory));
                    }
                    if (model.outerWerkType != null && model.outerWerkType.Count > 0)
                    {
                        query = query.Where(x => x.ProductOuterWerkList.Any(y => model.outerWerkType.Any(z => (OuterWerkType)z == y.OuterWerkType)));
                    }

                    if (model.active != null)
                    {
                        query = query.Where(x => x.Active == model.active);
                    }
                    if (model.noModelImage == true)
                    {
                        query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.ModelImage) == 0);
                    }
                    if (model.modelImage == true)
                    {
                        query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.ModelImage) == 0);
                    }
                    if (model.whiteBack == true)
                    {
                        query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.WhiteBack) == 0);
                    }
                    if (model.siteImage == true)
                    {
                        query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.Bot) == 0);
                    }
                    if (model.firstPhoto == true)
                    {
                        query = query.Where(x => x.PhotographyManageList.Where(y => y.FirstPhotography == true).Count() > 0);
                    }
                    if (model.secondPhoto == true)
                    {
                        query = query.Where(x => x.PhotographyManageList.Where(y => y.SecondPhotography == true).Count() > 0);
                    }
                    if (model.notOrderable == true)
                    {
                        query = query.Where(x => x.NotOrderable == true);
                    }
                    if (model.collectionId > 0)
                    {
                        query = query.Where(x => x.ProductCollectionId == model.collectionId);
                    }
                    if (model.specialProduct != null)
                    {
                        query = query.Where(x => x.SpecialProduct == model.specialProduct);
                    }
                    if (model.goldType != null && model.goldType.Count > 0)
                    {
                        query = query.Where(x => model.goldType.Any(y => (GoldType)y == x.GoldType));
                    }

                    if (model.goldColor == true && model.rosegoldColor == true && model.whiteColor == true)
                    {
                        query = query.Where(x => x.GoldColor == true || x.RosegoldColor == true || x.WhiteColor == true);
                    }
                    if (model.goldColor == true && model.rosegoldColor == true && model.whiteColor != true)
                    {
                        query = query.Where(x => x.GoldColor == true || x.RosegoldColor == true);
                    }
                    if (model.goldColor == true && model.rosegoldColor != true && model.whiteColor != true)
                    {
                        query = query.Where(x => x.GoldColor == true);
                    }
                    if (model.goldColor != true && model.rosegoldColor != true && model.whiteColor == true)
                    {
                        query = query.Where(x => x.WhiteColor == true);
                    }
                    if (model.goldColor == true && model.rosegoldColor != true && model.whiteColor == true)
                    {
                        query = query.Where(x => x.GoldColor == true || x.WhiteColor == true);
                    }
                    if (model.goldColor != true && model.rosegoldColor == true && model.whiteColor == true)
                    {
                        query = query.Where(x => x.RosegoldColor == true || x.WhiteColor == true);
                    }
                    if (model.goldColor != true && model.rosegoldColor == true && model.whiteColor != true)
                    {
                        query = query.Where(x => x.RosegoldColor == true);
                    }
                    if (model.lower != null && model.upper != null)
                    {
                        query = query.Where(x => x.Weight >= model.lower && x.Weight <= model.upper);
                    }

                    if (model.orderableBranchType == true && model.orderableSolicitorshipType == true && model.orderableCoWorkerType == true && model.orderableOtherType == true)
                    {
                        query = query.Where(x => x.OrderableBranchType == true || x.OrderableSolicitorshipType == true || x.OrderableCoWorkerType == true || x.OrderableOtherType == true);
                    }
                    if (model.orderableBranchType == true && model.orderableSolicitorshipType == true && model.orderableCoWorkerType == true && model.orderableOtherType != true)
                    {
                        query = query.Where(x => x.OrderableBranchType == true || x.OrderableSolicitorshipType == true || x.OrderableCoWorkerType == true);
                    }
                    if (model.orderableBranchType == true && model.orderableSolicitorshipType == true && model.orderableCoWorkerType != true && model.orderableOtherType == true)
                    {
                        query = query.Where(x => x.OrderableBranchType == true || x.OrderableSolicitorshipType == true || x.OrderableOtherType == true );
                    }
                    if (model.orderableBranchType == true && model.orderableSolicitorshipType != true && model.orderableCoWorkerType == true && model.orderableOtherType == true)
                    {
                        query = query.Where(x => x.OrderableBranchType == true || x.OrderableCoWorkerType == true || x.OrderableOtherType == true);
                    }
                    if (model.orderableBranchType != true && model.orderableSolicitorshipType == true && model.orderableCoWorkerType == true && model.orderableOtherType == true)
                    {
                        query = query.Where(x => x.OrderableSolicitorshipType == true || x.OrderableCoWorkerType == true || x.OrderableOtherType == true);
                    }
                    if (model.orderableBranchType == true && model.orderableSolicitorshipType == true && model.orderableCoWorkerType != true && model.orderableOtherType != true)
                    {
                        query = query.Where(x => x.OrderableBranchType == true || x.OrderableSolicitorshipType == true);
                    }
                    if (model.orderableBranchType == true && model.orderableSolicitorshipType != true && model.orderableCoWorkerType != true && model.orderableOtherType != true)
                    {
                        query = query.Where(x => x.OrderableBranchType == true);
                    }
                    if (model.orderableBranchType != true && model.orderableSolicitorshipType != true && model.orderableCoWorkerType == true && model.orderableOtherType == true)
                    {
                        query = query.Where(x => x.OrderableOtherType == true || x.OrderableCoWorkerType == true);
                    }
                    if (model.orderableBranchType != true && model.orderableSolicitorshipType != true && model.orderableCoWorkerType != true && model.orderableOtherType == true)
                    {
                        query = query.Where(x => x.OrderableOtherType == true);
                    }
                    if (model.orderableBranchType != true && model.orderableSolicitorshipType != true && model.orderableCoWorkerType == true && model.orderableOtherType != true)
                    {
                        query = query.Where(x => x.OrderableCoWorkerType == true);
                    }
                    if (model.orderableBranchType != true && model.orderableSolicitorshipType == true && model.orderableCoWorkerType != true && model.orderableOtherType != true)
                    {
                        query = query.Where(x => x.OrderableSolicitorshipType == true);
                    }
                    //if (model.showProduct != null) 
                    //{
                    //    query = query.Where(x => x.ShowProduct == model.showProduct);
                    //}
                    //if (!User.IsInRole("admin") && !User.IsInRole("allOrder") && !User.IsInRole("product"))
                    //{
                    //    if (currentUser.BranchType == BranchType.Branch)
                    //    {
                    //        query = query.Where(x => x.ShowProduct == ShowProduct.Branch || x.ShowProduct == ShowProduct.Both);
                    //    }
                    //    else if (currentUser.BranchType == BranchType.Solicitorship)
                    //    {
                    //        query = query.Where(x => x.ShowProduct == ShowProduct.SolicitorShip || x.ShowProduct == ShowProduct.Both);
                    //    }
                    //    else
                    //    {
                    //        query = query.Where(x => x.ShowProduct == ShowProduct.Both || x.ShowProduct == ShowProduct.Branch || x.ShowProduct == ShowProduct.SolicitorShip);
                    //    }
                    //}
                    if (User.IsInRole("specialProducts") || User.IsInRole("admin") || User.IsInRole("allOrder") || User.IsInRole("product"))
                    {
                        query = query.Where(x => x.SpecialProduct == true || x.SpecialProduct == false);
                    }
                    else
                    {
                        query = query.Where(x => x.SpecialProduct == false);
                    }
                    if (!User.IsInRole("admin") && !User.IsInRole("product"))
                    {
                        if (currentUser.BranchType == BranchType.Branch)
                        {
                            query = query.Where(x => x.OrderableBranchType == true);
                        }
                        if (currentUser.BranchType == BranchType.Solicitorship)
                        {
                            query = query.Where(x => x.OrderableSolicitorshipType == true);
                        }
                        if (currentUser.BranchType == BranchType.CoWorker)
                        {
                            query = query.Where(x => x.OrderableCoWorkerType == true);
                        }
                        if (currentUser.BranchType == BranchType.Other)
                        {
                            query = query.Where(x => x.OrderableOtherType == true);
                        }

                    }
                    dataCount = query.Count();

                    query = query.OrderByDescending(x => x.BookCode).Skip(model.page * model.count).Take(model.count);

                    list = query.Select(item => new ResponseProductSearchViewModel()
                    {
                        id = item.Id,
                        productCollectionTitle = item.ProductCollection.Title,
                        code = item.Code,
                        specialProduct = item.SpecialProduct,
                        bookCode = item.BookCode,
                        fileName = item.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        paddingImg = item.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).PaddingImg,
                        modelImage = item.ProductFileList.FirstOrDefault(x => x.FileType == FileType.ModelImage).FileName,
                        orderImage = item.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Order).FileName,
                        siteImage = item.ProductFileList.FirstOrDefault(x => x.FileType == FileType.Bot).FileName,
                        productType = item.ProductType,
                        //productTypeTitle = Enums.GetTitle(item.ProductType),
                        title = item.Title,
                        weight = item.Weight,
                        silverWeight = item.SilverWeight,
                        outerWerkPlacement = item.OuterWerkPlacement,
                        active = item.Active,
                        productNew = item.New,
                        modifyDate = item.ModifyDate,
                        related = item.RelatedProduct.Count(),
                        set = item.SetProduct.Count(),
                        //isNew = item.ModifyDate.AddDays(item.New) >= DateTime.Now,
                        isFav = item.FavouritesProductList.Count(x => x.BranchId == branchId) > 0,
                        secondPhoto = item.PhotographyManageList.OrderByDescending(x => x.Id).Select(x => x.SecondPhotography).FirstOrDefault(),
                        firstPhoto = item.PhotographyManageList.OrderByDescending(x => x.Id).Select(x => x.FirstPhotography).FirstOrDefault(),
                        productCollectionFileName = item.ProductCollection.FileName,
                        //notOrderableForBranch = item.NotOrderableForBranch

                    }).ToList();
                }
                list.ForEach(x =>
                {
                    x.productTypeTitle = Enums.GetTitle(x.productType);
                    x.isNew = x.modifyDate.AddDays(x.productNew) >= DateTime.Now;
                });
                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list,
                        pageCount = Math.Ceiling((double)dataCount / model.count),
                        count = dataCount,
                        page = model.page + 1,
                        workshopId = model.workshopId
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
        /// غیر فعال کردن محصول
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, product")]
        public JsonResult Inactive(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Product.Find(id);
                    item.Active = false;
                    response = new Response()
                    {
                        status = 200,
                        message = "محصول با موفقیت غیرفعال شد."
                    };
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
        /// فعال کردن محصول
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, product")]
        public JsonResult Active(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Product.Find(id);
                    item.Active = true;
                    response = new Response()
                    {
                        status = 200,
                        message = "محصول با موفقیت فعال شد."
                    };
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
        /// حذف محصول
        /// </summary>
        /// <param name="id">ردیف محصول</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, product")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    var item = db.Product.Find(Id);
                    var fileList = item.ProductFileList.Select(x => x.FileName).ToList();
                    if (item.CartList.Count() == 0)
                    {
                        db.RelatedProduct.RemoveRange(item.RelatedProduct);
                        db.RelatedProduct.RemoveRange(item.SourceRelatedProduct);
                        db.SetProduct.RemoveRange(item.SetProduct);
                        db.SetProduct.RemoveRange(item.SourceSetProduct);
                        db.ProductFile.RemoveRange(item.ProductFileList);
                        db.ProductStone.RemoveRange(item.ProductStoneList);
                        db.ProductLeather.RemoveRange(item.ProductLeatherList);
                        db.ProductOuterWerk.RemoveRange(item.ProductOuterWerkList);
                        db.Product.Remove(item);
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
                            status = 200,
                            message = "سفارشی برای این محصول ثبت شده است,شما نمیتوانیداین محصول را حذف کنید."
                        };
                    }


                    //fileList.ForEach(fileName =>
                    //{
                    //    string serverPath = Server.MapPath("~/Upload/Product");
                    //    if (System.IO.File.Exists(Path.Combine(serverPath, fileName)))
                    //    {
                    //        System.IO.File.SetAttributes(serverPath, FileAttributes.Normal);
                    //        System.IO.File.Delete(Path.Combine(serverPath, fileName));
                    //    }
                    //});


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
        /// <param name="id">ردیف محصول</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, product")]
        public ActionResult DeleteOuterWerk()
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    List<Product> productList = db.Product.Include(x => x.RelatedProduct).Include(x => x.SourceRelatedProduct).Include(x => x.SetProduct).Include(x => x.SourceSetProduct).Include(x => x.ProductFileList).Include(x => x.ProductStoneList).Include(x => x.ProductLeatherList).Where(x => x.Title.Contains("KH") && x.Active == false).ToList();
                    productList.ForEach(item =>
                    {
                        db.RelatedProduct.RemoveRange(item.RelatedProduct);
                        db.RelatedProduct.RemoveRange(item.SourceRelatedProduct);
                        db.SetProduct.RemoveRange(item.SetProduct);
                        db.SetProduct.RemoveRange(item.SourceSetProduct);
                        db.ProductFile.RemoveRange(item.ProductFileList);
                        db.ProductStone.RemoveRange(item.ProductStoneList);
                        db.ProductLeather.RemoveRange(item.ProductLeatherList);
                        db.Product.Remove(item);
                    });

                    db.SaveChanges();
                }
                ViewBag.Message = "محصولات خرج کار غیر فعال با موفقیت حذف شد.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        /// <summary>
        /// برگرداندن لیست محصولات برای autocomplete
        /// </summary>
        /// <param name="term">واژه ای که باید بر اساس آن فیلتر شود</param>
        /// <returns>لیست جیسون محصولات</returns>
        [HttpGet]
        [Authorize(Roles = "admin, product")]
        public JsonResult GetProductAutoComplete(string term)
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.Product.Where(x => x.Title.Contains(term) || x.Code.Contains(term) || x.BookCode.Contains(term) || x.Title.Contains(term.Trim().Replace("ی", "ي").Replace("ک", "ك"))).Select(x => new
                    {
                        id = x.Id,
                        title = x.Title,
                        bookCode = x.BookCode,
                        code = x.Code,
                        fileName = x.ProductFileList.FirstOrDefault(y => y.FileType == FileType.WhiteBack).FileName
                    }).OrderBy(x => x.title).Take(20).ToList();

                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return Json("[]", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// اضافه کردن کالای مرتبط
        /// </summary>
        /// <param name="id">محصول اصلی</param>
        /// <param name="relatedId">محصول مرتبط</param>
        /// <returns>نتیجه عملیات</returns>
        [HttpPost]
        [Authorize(Roles = "admin, product")]
        public JsonResult AddRelated(int id, int relatedId)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var count = db.RelatedProduct.Count(x => (x.ProductId == id && x.RelatedToId == relatedId) || (x.ProductId == relatedId && x.RelatedToId == id));
                    if (count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این محصول جزو موارد مرتبط این کالا هست."
                        };
                    }
                    else
                    {
                        var user = GetAuthenticatedUser();

                        db.RelatedProduct.Add(new RelatedProduct()
                        {
                            ProductId = id,
                            RelatedToId = relatedId,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        });

                        db.RelatedProduct.Add(new RelatedProduct()
                        {
                            ProductId = relatedId,
                            RelatedToId = id,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        });
                        db.SaveChanges();

                        response = new Response()
                        {
                            status = 200,
                            message = "محصول مرتبط با موفقیت اضافه شد."
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
        /// حذف محصول مرتبط
        /// </summary>
        /// <param name="id">شناسه محصول مرتبط</param>
        /// <returns>نتیجه عملیات حذف محصول</returns>
        [HttpPost]
        [Authorize(Roles = "admin, product")]
        public JsonResult RemoveRelated(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var related = db.RelatedProduct.Where(x => x.Id == id).Single();
                    var list = db.RelatedProduct.Where(x => (x.ProductId == related.ProductId && x.RelatedToId == related.RelatedToId) || (x.ProductId == related.RelatedToId && x.RelatedToId == related.ProductId)).ToList();

                    db.RelatedProduct.RemoveRange(list);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "محصول مرتبط با موفقیت اضافه شد."
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
        /// اضافه کردن کالای ست
        /// </summary>
        /// <param name="id">محصول اصلی</param>
        /// <param name="setId">محصول ست</param>
        /// <returns>نتیجه عملیات</returns>
        [HttpPost]
        [Authorize(Roles = "admin, product")]
        public JsonResult AddSet(int id, int setId)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var productSet = db.Product.Where(x => x.Id == setId).Single();
                    var product = db.Product.Where(x => x.Id == id).Single();
                    var count = db.SetProduct.Count(x => (x.ProductId == id && x.SetToId == setId) || (x.ProductId == setId && x.SetToId == id));
                    if (count > 0)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این محصول جزو موارد ست این کالا هست."
                        };
                    }
                    else if(productSet.WorkshopId != product.WorkshopId)
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "کارگاه این محصول تفاوت دارد."
                        };
                    }
                    else
                    {
                        var user = GetAuthenticatedUser();

                        db.SetProduct.Add(new SetProduct()
                        {
                            ProductId = id,
                            SetToId = setId,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        });

                        db.SetProduct.Add(new SetProduct()
                        {
                            ProductId = setId,
                            SetToId = id,
                            CreateUserId = user.Id,
                            ModifyUserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        });
                        db.SaveChanges();

                        response = new Response()
                        {
                            status = 200,
                            message = "محصول ست با موفقیت اضافه شد."
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
        /// حذف محصول ست
        /// </summary>
        /// <param name="id">شناسه محصول ست</param>
        /// <returns>نتیجه عملیات حذف محصول</returns>
        [HttpPost]
        [Authorize(Roles = "admin, product")]
        public JsonResult RemoveSet(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var set = db.SetProduct.Where(x => x.Id == id).Single();
                    var list = db.SetProduct.Where(x => (x.ProductId == set.ProductId && x.SetToId == set.SetToId) || (x.ProductId == set.SetToId && x.SetToId == set.ProductId)).ToList();

                    db.SetProduct.RemoveRange(list);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "محصول ست با موفقیت اضافه شد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UploadCameraImage(string path, string fileName, string base64)
        {
            Response response;
            try
            {
                string serverPath = Server.MapPath("~/Upload/" + path);

                fileName = Imagefile(base64, fileName);
                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        name = fileName,
                        length = fileName.Length,
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
        private string Imagefile(string base64String, string fileName)
        {
            if (base64String != null)
            {
                base64String = base64String.Replace("data:image/png;base64,", "");

            }
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            string serverPath = Server.MapPath("~/Upload/product");

            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(Server.MapPath("~/Upload/product/"));
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
        [HttpPost]
        public JsonResult UploadFile(string type, int id)
        {
            Response response;
            try
            {
                string fileName = string.Empty;
                string serverPath = Server.MapPath("~/Upload/product");
                HttpPostedFileBase hpf = Request.Files[0];
                string originalFileName = Path.GetFileName(hpf.FileName);

                string fileExtension = Path.GetExtension(originalFileName);
                using (var db = new KiaGalleryContext())
                {
                    var product = db.Product.Single(x => x.Id == id);
                    fileName = product.Code + "_" + product.BookCode + "_" + type + Path.GetExtension(originalFileName);
                }

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

        public JsonResult GetWorkshopTag(int id)
        {
            Response response;
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    var list = db.WorkshopTag.Where(x => x.WorkshopId == id).Select(x => new
                    {
                        id = x.Id,
                        value = x.Title,
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

    }
}