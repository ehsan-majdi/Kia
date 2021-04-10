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
    /// کنترلر محصولات
    /// </summary>
    public class ProductController : BaseController
    {
        /// <summary>
        /// دریافت لیست محصولات
        /// </summary>
        /// <param name="model">مدل حاوی پارامترهای جستجو</param>
        /// <returns>نتیجه جستجو محصولات به صورت جیسون</returns>
        public JsonResult ProductList(ProductSearchViewModel model)
        {
            Response response;
            int dataCount;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var branchId = GetAuthenticatedUser().BranchId;

                    IQueryable<Product> query = db.Product.Where(x => x.Active == true);

                    if (model.workShopId != null)
                    {
                        query = query.Where(x => x.WorkshopId == model.workShopId);
                    }

                    if (model.productType != null)
                    {
                        query = query.Where(x => x.ProductType == model.productType);
                    }

                    if (!string.IsNullOrEmpty(model.word?.Trim()))
                    {
                        query = query.Where(x => x.Code.Contains(model.word.Trim()) || x.BookCode.Contains(model.word.Trim()) || x.Title.Contains(model.word.Trim()) || x.Title.Contains(model.word.Trim().Replace("ی", "ي").Replace("ک", "ك")));
                    }

                    dataCount = query.Count();
                    var productList = query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count).Select(item => new ProductViewModel
                    {
                        id = item.Id,
                        code = item.Code,
                        bookCode = item.BookCode,
                        fileName = item.ProductFileList.FirstOrDefault(x => x.FileType == FileType.WhiteBack).FileName,
                        productType = item.ProductType,
                        title = item.Title,
                        weight = item.Weight,
                        productNew = item.New,
                        modifyDate = item.ModifyDate,
                        isFav = item.FavouritesProductList.Count(x => x.BranchId == branchId) > 0
                    }).ToList();

                    productList.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.fileName))
                        {
                            x.fileName = "/upload/product/" + x.fileName;
                        }

                        x.productTypeTitle = Enums.GetTitle(x.productType);
                        x.isNew = x.modifyDate.AddDays(x.productNew) >= DateTime.Now;
                    });

                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = productList,
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
    }
}