using KiaGallery.Model;
using KiaGallery.Model.Context;
using System.Linq;
using System.Web.Mvc;
using System;
using KiaGallery.Common;
using KiaGallery.Web.Models;
using KiaGallery.Model.Context.StoneTable;
using System.Collections.Generic;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر میز سنک
    /// </summary>
    public class StoneTableController : BaseController
    {
        /// <summary>
        /// مشاهده میز سنگ
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, stoneTable")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                var stoneList = db.Stone.ToList();
                ViewBag.Transparent = stoneList.Where(x => x.StoneType == StoneType.Transparent).OrderBy(x => x.Order).ToList();
                ViewBag.Sedimentary = stoneList.Where(x => x.StoneType == StoneType.Sedimentary).OrderBy(x => x.Order).ToList();
                ViewBag.Pearl = stoneList.Where(x => x.StoneType == StoneType.Pearl).OrderBy(x => x.Order).ToList();
                //ViewBag.Atomic = stoneList.Where(x => x.StoneType == StoneType.Atomic).OrderBy(x => x.Order).ToList();
                ViewBag.LeatherBraceletStone = stoneList.Where(x => x.StoneType == StoneType.LeatherBraceletStone).OrderBy(x => x.Order).ToList();
                ViewBag.BraceletStone = stoneList.Where(x => x.StoneType == StoneType.BraceletStone).OrderBy(x => x.Order).ToList();

                var shapeSize = db.ShapeSize.ToList();
                ViewBag.CircleSize = shapeSize.Where(x => x.StoneShape == StoneShape.Circle).OrderBy(x => x.Order).ToList();
                ViewBag.MarquiseSize = shapeSize.Where(x => x.StoneShape == StoneShape.Marquise).OrderBy(x => x.Order).ToList();
                ViewBag.TearSize = shapeSize.Where(x => x.StoneShape == StoneShape.Tear).OrderBy(x => x.Order).ToList();
                ViewBag.TriangleSize = shapeSize.Where(x => x.StoneShape == StoneShape.Triangle).OrderBy(x => x.Order).ToList();
                ViewBag.SquareSize = shapeSize.Where(x => x.StoneShape == StoneShape.Square).OrderBy(x => x.Order).ToList();
                ViewBag.RectangleSize = shapeSize.Where(x => x.StoneShape == StoneShape.Rectangle).OrderBy(x => x.Order).ToList();
                ViewBag.OvalSize = shapeSize.Where(x => x.StoneShape == StoneShape.Oval).OrderBy(x => x.Order).ToList();
                ViewBag.OtherSize = shapeSize.Where(x => x.StoneShape == StoneShape.Other).OrderBy(x => x.Order).ToList();

            }
            return View();
        }

        /// <summary>
        /// گرفتن لیست سنگ های حذف شده از میز سنگ
        /// </summary>
        /// <param name="stoneId">ردیف سنگ</param>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, stoneTable")]
        [HttpGet]
        public JsonResult GetExcludeStone(int stoneId)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var data = db.StoneOutOfStock.Where(x => x.StoneId == stoneId).Select(x => new
                    {
                        id = x.Id,
                        stoneId = x.StoneId,
                        shapeSizeId = x.ShapeSizeId
                    }).ToList();

                    response = new Response()
                    {
                        status = 200,
                        data = data
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
        /// ذخیره لیست سنگ های حذف شده از میز سنگ
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات سایز های حذف شده</param>
        /// <returns>صفحه مورد نظر</returns>
        [Authorize(Roles = "admin, stoneTable")]
        [HttpPost]
        public JsonResult SetExcludeStone(StoneTableViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = GetAuthenticatedUser();

                    var stoneItemList = db.StoneOutOfStock.Where(y => y.StoneId == model.stoneId).ToList();
                    if (model.shapeSizeIdList != null && model.shapeSizeIdList.Count > 0)
                    {

                        var addedItemList = model.shapeSizeIdList.Where(x => !stoneItemList.Any(y => y.ShapeSizeId == x)).ToList();
                        addedItemList.ForEach(x =>
                        {
                            db.StoneOutOfStock.Add(new StoneOutOfStock()
                            {
                                StoneId = model.stoneId,
                                ShapeSizeId = x,
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            });
                        });
                    }

                    List<StoneOutOfStock> removedItemList;
                    if (model.shapeSizeIdList != null && model.shapeSizeIdList.Count > 0)
                    {
                        removedItemList = stoneItemList.Where(x => x.StoneId == model.stoneId && !model.shapeSizeIdList.Any(y => y == x.ShapeSizeId)).ToList();
                    }
                    else
                    {
                        removedItemList = stoneItemList.Where(x => x.StoneId == model.stoneId).ToList();
                    }
                    db.StoneOutOfStock.RemoveRange(removedItemList);

                    db.SaveChanges();

                    response = new Response()
                    {
                        status = 200,
                        message = "اطلاعات با موفقیت ذخیره شد."
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