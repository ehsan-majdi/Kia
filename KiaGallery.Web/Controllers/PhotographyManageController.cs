using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class PhotographyManageController : BaseController
    {
        [Authorize(Roles = "admin, manageImage")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.Workshop = db.Workshop.Where(x => x.Active == true && x.ProductList.Count(y => y.Active == true) > 0 && x.Name != "خرج کار").OrderBy(x => x.Order).Select(x => x).ToList();
            }

            return View();
        }
        /// <summary>
        /// تغییر وضعیت سفارشات
        /// </summary>
        /// <param name="model">مدلی شامل ردیف های گیفت های می باشد</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, manageImage")]
        public JsonResult ChangeStatus(PhotographyManageViewModel model)
        {
            Response response;
            try
            {
                int userid = GetAuthenticatedUserId();
                using (var db = new KiaGalleryContext())
                {
                    var photographyManage = db.PhotographyManage.Where(x => x.ProductId == model.productId).SingleOrDefault();

                    if (photographyManage == null && model.status)
                    {

                        var photo = new PhotographyManage()
                        {
                            ProductId = model.productId,
                            FirstPhotography = model.photography == 0,
                            SecondPhotography = model.photography == 1,
                            CreateUserId = GetAuthenticatedUserId(),
                            ModifyUserId = GetAuthenticatedUserId(),
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        };
                        db.PhotographyManage.Add(photo);
                    }
                    if (photographyManage != null)
                    {
                        if (model.status)
                        {
                            switch (model.photography)
                            {
                                case 0:
                                    photographyManage.FirstPhotography = true;
                                    break;
                                case 1:
                                    photographyManage.SecondPhotography = true;
                                    break;
                            }
                        }
                        else
                        {
                            switch (model.photography)
                            {
                                case 0:
                                    if (photographyManage.SecondPhotography)
                                        photographyManage.FirstPhotography = false;
                                    else
                                        db.PhotographyManage.Remove(photographyManage);
                                    break;
                                case 1:

                                    if (photographyManage.FirstPhotography)
                                        photographyManage.SecondPhotography = false;
                                    else
                                        db.PhotographyManage.Remove(photographyManage);
                                    break;
                            }
                        }

                    }


                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,

                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, manageImage")]
        public JsonResult GetImage(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.Product.Where(x => x.Id == id).Select(x => x.ProductFileList).Select(item => new PhotographyManageViewModel()
                    {
                        whiteBack = item.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        modelImage = item.FirstOrDefault(x => x.FileType == FileType.ModelImage).FileName,
                        siteImage = item.FirstOrDefault(x => x.FileType == FileType.Bot).FileName
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

        public JsonResult SaveProductImage(SaveProductImageViewModel model)
        {
            var user = GetAuthenticatedUser();
            Response response;
            int status = 200;
            string message = string.Empty;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Product.Single(x => x.Id == model.productId);
                    entity.CreateDate = DateTime.Now;
                    entity.ModifyDate = DateTime.Now;
                    var imageFile = new ProductFile()
                    {

                        ProductId = model.productId,
                        FileName = model.fileName,
                        FileType = model.fileType,
                        PaddingImg = 0,
                        CreateUserId = user.Id,
                        ModifyUserId = user.Id,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Ip = Request.UserHostAddress
                    };

                    db.ProductFile.Add(imageFile);
                    db.SaveChanges();
                }


                response = new Response()
                {
                    status = status,
                    message = "عکس محصول با موفقیت اضافه شد."
                };
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
        [Authorize(Roles = "admin,manageImage")]
        public JsonResult Search(ProductSearchViewModel model)
        {
            Response response;
            try
            {
                List<ResponseProductSearchViewModel> list;
                int dataCount;
                var imageCount = 0;
                var branchId = GetAuthenticatedUser().BranchId;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Product.Where(x => x.Active && x.ProductType != ProductType.OuterWerk && x.ProductType != ProductType.Plaque && x.OrderableBranchType == true || x.OrderableSolicitorshipType == true || x.OrderableOtherType == true);

                    if (model.fav)
                    {
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
                    if (model.hasNotCode != null)
                    {
                        if (model.hasNotCode == true)
                        {
                            query = query.Where(x => x.Code != null);

                        }
                        else
                        {
                            query = query.Where(x => x.Code == null);
                        }
                        imageCount = query.Count();

                    }
                    if (model.type != null && model.type.Count > 0)
                    {
                        query = query.Where(x => model.type.Any(y => (ProductType)y == x.ProductType));
                    }

                    if (model.leatherBracelet == true)
                    {
                        query = query.Where(x => x.LeatherPrice > 0);
                    }

                    if (model.set == true)
                    {
                        query = query.Where(x => x.SetProduct.Count() > 0);
                    }

                    if (model.noModelImage == true)
                    {
                        query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.ModelImage) == 0);
                    }

                    if (model.modelImage != null)
                    {
                        if (model.modelImage == true)
                        {
                            query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.ModelImage) > 0);
                        }
                        else
                        {
                            query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.ModelImage) == 0);
                        }
                        imageCount = query.Count();
                    }
                    if (model.whiteBack != null)
                    {
                        if (model.whiteBack == true)
                        {
                            query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.WhiteBack) > 0);
                        }
                        else
                        {
                            query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.WhiteBack) == 0);
                        }
                        imageCount = query.Count();
                    }

                    if (model.siteImage != null)
                    {
                        if (model.siteImage == true)
                        {
                            query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.Bot) > 0);
                        }
                        else
                        {
                            query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.Bot) == 0);
                        }
                        imageCount = query.Count();
                    }
                    if (model.orderImage != null)
                    {
                        if (model.orderImage == true)
                        {
                            query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.Order) > 0);
                        }
                        else
                        {
                            query = query.Where(x => x.ProductFileList.Count(y => y.FileType == FileType.Order) == 0);
                        }
                        imageCount = query.Count();
                    }

                    if (model.sex != null && model.sex.Count > 0)
                    {
                        if (model.sex.Any(y => (Sex)y == Sex.Man) || model.sex.Any(y => (Sex)y == Sex.Woman))
                            query = query.Where(x => model.sex.Any(y => (Sex)y == x.Sex) || x.Sex == Sex.Both);
                        else
                            query = query.Where(x => model.sex.Any(y => (Sex)y == x.Sex));
                    }

                    if (model.firstPhoto == true)
                    {
                        query = query.Where(x => x.PhotographyManageList.Where(y => y.FirstPhotography == true).Count() > 0);
                    }
                    if (model.secondPhoto == true)
                    {
                        query = query.Where(x => x.PhotographyManageList.Where(y => y.SecondPhotography == true).Count() > 0);
                    }
                    dataCount = query.Count();
                    query = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);

                    list = query.Select(item => new ResponseProductSearchViewModel()
                    {
                        id = item.Id,
                        code = item.Code,
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
                        firstPhoto = item.PhotographyManageList.OrderByDescending(x => x.Id).Select(x => x.FirstPhotography).FirstOrDefault()

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
                        imageCount = imageCount
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