using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class SiteApiController : BaseController
    {
        [AllowAnonymous]
        public JsonResult SyncStone()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.Stone.Select(x => new SyncStoneViewModel
                    {
                        id = x.Id,
                        type = (int)x.StoneType,
                        title = x.Name,
                        englishTitle = x.EnglishName,
                        order = x.Order,
                        imageUrl = "/image/stone/100x100/" + x.FileName,
                        active = x.Active
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = list
                    };
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult SyncLeather()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var list = db.Leather.Select(x => new SyncLeatherViewModel
                    {
                        id = x.Id,
                        type = (int)x.LeatherType,
                        title = x.Name,
                        order = x.Order,
                        imageUrl = "/image/leather/100x100/" + x.FileName,
                        active = x.Active
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = list
                    };
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetProductIdList(long? ticks)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Product.Select(x => x);
                    if (ticks != null)
                    {
                        DateTime date = new DateTime(ticks.Value);
                        query = query.Where(x => x.ModifyDate >= date);
                    }
                    var list = query.Select(x => x.Id).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = list
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetSizeList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Size.Select(x => x);

                    var list = query.Select(x => new ApiSizeViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        defaultValue = x.DefaultValue
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = list
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetSizeValueList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.SizeValue.Select(x => x);

                    var list = query.Select(x => new ApiSizeValueViewModel
                    {
                        id = x.Id,
                        sizeId = x.SizeId,
                        order = x.Order,
                        value = x.Value
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = list
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetWorkshopList()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Workshop.Select(x => x);

                    var list = query.Select(x => new ApiWorkshopViewModel
                    {
                        id = x.Id,
                        order = x.Order,
                        alias = x.Alias,
                        title = x.Name,
                        color = x.Color,
                        autoConfirm = x.AutoConfirm,
                        goldTrade = x.GoldTrade,
                        statusId = x.Active ? 1 : 2

                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = list
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetProductList(long? ticks)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Product.Select(x => x);
                    if (ticks != null)
                    {
                        DateTime date = new DateTime(ticks.Value);
                        query = query.Where(x => x.ModifyDate >= date);
                    }
                    var list = query.Select(x => new ApiProductViewModel
                    {
                        id = x.Id,
                        workshopId = x.WorkshopId,
                        workshopSecondaryId = x.WorkshopId2,
                        code = x.Code,
                        bookCode = x.BookCode,
                        title = x.Title,
                        productTypeId = (int)x.ProductType,
                        ringSizeType = x.RingSizeType,
                        goldTypeId = (int)x.GoldType,
                        productSexId = (int)x.Sex,
                        stonePrice = x.StonePrice,
                        leatherPrice = x.LeatherPrice,
                        sizeId = x.SizeId,
                        silverWeightFloat = x.SilverWeight,
                        weightFloat = x.Weight,
                        normalSizeValueId = x.NormalSizeValueId,
                        wage = x.Wage,
                        canLoop = x.CanLoop,
                        newDays = x.New,
                        outerWerkTypeId = (int)x.OuterWerkCategory,
                        outerWerkPlacement = x.OuterWerkPlacement,
                        specialProduct = x.SpecialProduct,
                        orderable = x.NotOrderable,
                        earringBack = x.EarringBack,
                        //orderableForBranch = x.NotOrderableForBranch,
                        statusId = x.Active ? 1 : 2,
                        stones = x.ProductStoneList.Select(y => new ApiProductStoneViewModel
                        {
                            stoneId = y.StoneId,
                            order = y.Order
                        }).ToList(),
                        leathers = x.ProductLeatherList.Select(y => new ApiProductLeatherViewModel
                        {
                            leatherId = y.LeatherId,
                            order = y.Order
                        }).ToList(),
                        images = x.ProductFileList.Select(y => new ApiProductImageViewModel
                        {
                            id = y.Id,
                            order = 1,
                            productImageTypeId = (int)y.FileType,
                            url = "/upload/product/" + y.FileName,
                            paddingImg = y.PaddingImg

                        }).ToList(),
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.weight = Convert.ToDecimal(x.weightFloat != null ? x.weightFloat : 0);
                        x.silverWeight = Convert.ToDecimal(x.silverWeightFloat != null ? x.silverWeightFloat : 0);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = list,
                    };

                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            var result = Json(response, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = 2147483647;
            return result;
        }
        [AllowAnonymous]
        public JsonResult GetProduct(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Product.Where(x => x.Id == id);

                    var product = entity.Select(x => new ApiProductViewModel
                    {
                        id = x.Id,
                        workshopId = x.WorkshopId,
                        workshopSecondaryId = x.WorkshopId2,
                        code = x.Code,
                        bookCode = x.BookCode,
                        title = x.Title,
                        productTypeId = (int)x.ProductType,
                        ringSizeType = x.RingSizeType,
                        goldTypeId = (int)x.GoldType,
                        productSexId = (int)x.Sex,
                        stonePrice = x.StonePrice,
                        leatherPrice = x.LeatherPrice,
                        sizeId = x.SizeId,
                        silverWeightFloat = x.SilverWeight,
                        weightFloat = x.Weight,
                        normalSizeValueId = x.NormalSizeValueId,
                        wage = x.Wage,
                        canLoop = x.CanLoop,
                        newDays = x.New,
                        outerWerkTypeId = (int)x.OuterWerkCategory,
                        outerWerkPlacement = x.OuterWerkPlacement,
                        specialProduct = x.SpecialProduct,
                        orderable = x.NotOrderable,
                        earringBack = x.EarringBack,
                        statusId = x.Active ? 1 : 2,
                        stones = x.ProductStoneList.Select(y => new ApiProductStoneViewModel
                        {
                            stoneId = y.StoneId,
                            order = y.Order
                        }).ToList(),
                        leathers = x.ProductLeatherList.Select(y => new ApiProductLeatherViewModel
                        {
                            leatherId = y.LeatherId,
                            order = y.Order
                        }).ToList(),
                        images = x.ProductFileList.Select(y => new ApiProductImageViewModel
                        {
                            id = y.Id,
                            order = 1,
                            productImageTypeId = (int)y.FileType,
                            url = "/upload/product/" + y.FileName,
                            paddingImg = y.PaddingImg

                        }).ToList(),

                    }).Single();

                    product.weight = Convert.ToDecimal(product.weightFloat != null ? product.weightFloat : 0);
                    product.silverWeight = Convert.ToDecimal(product.silverWeightFloat != null ? product.silverWeightFloat : 0);
                    response = new Response()
                    {
                        status = 200,
                        data = product,
                    };

                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            var result = Json(response, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = 2147483647;
            return result;
        }
    }
}