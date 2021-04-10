using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Areas.Order.Models;
using KiaGallery.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Areas.Order.Controllers
{
    public class ProductController : BaseController
    {
        /// <summary>
        /// جستجوی محصول
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست محصولات پیدا شده</returns>
        public JsonResult Search(ProductSearchViewModel model)
        {
            Response response;
            try
            {
                List<ResponseProductSearchViewModel> list;
                int dataCount;
                var branchId = GetAuthenticatedUser().BranchId;
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن

                    if(user != null)
                    {
                        var query = db.Product/*.Include(x => x.FavouritesProductList).Include(x => x.ProductFileList)*/.Select(x => x);

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
                        if (model.outerWerkType != null && model.outerWerkType.Count > 0)
                        {
                            query = query.Where(x => x.ProductOuterWerkList.Any(y => model.outerWerkType.Any(z => (OuterWerkType)z == y.OuterWerkType)));
                        }

                        if (model.active != null)
                        {
                            query = query.Where(x => x.Active == model.active);
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
                            productType = item.ProductType,
                            //productTypeTitle = Enums.GetTitle(item.ProductType),
                            title = item.Title,
                            weight = item.Weight,
                            active = item.Active,
                            productNew = item.New,
                            modifyDate = item.ModifyDate,
                            related = item.RelatedProduct.Count(),
                            set = item.SetProduct.Count(),
                            //isNew = item.ModifyDate.AddDays(item.New) >= DateTime.Now,
                            isFav = item.FavouritesProductList.Count(x => x.BranchId == branchId) > 0
                        }).ToList();
                        list.ForEach(x => {
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
                                page = model.page + 1
                            }
                        }; 
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
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