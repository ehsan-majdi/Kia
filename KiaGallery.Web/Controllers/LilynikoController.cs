using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class LilynikoController : BaseController
    {
        [AllowAnonymous]
        public JsonResult GetData()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var data = db.OrderDetail.Where(x => x.Product.WorkshopId == 3 && x.OrderDetailStatus == OrderDetailStatus.InWorkshop);
                    var order = db.Order.Select(x => x.OrderDetailList.Where(y => y.Product.WorkshopId == 3 && y.OrderDetailStatus == OrderDetailStatus.Registered));
                    var list = data.GroupBy(x => x.Order).Select(x => new OViewModel
                    {
                        id = x.Key.Id,
                        detailList = x.Key.OrderDetailList.Where(y => y.Product.WorkshopId == 3 && y.OrderDetailStatus == OrderDetailStatus.InWorkshop).Select(y => new LilynikoViewModel
                        {
                            customer_code = y.Order.Branch.EnglishName,
                            customerId = y.Order.BranchId,
                            order_entry_date = y.CreateDate,
                            order_karat = "18",
                            product_code = y.Product.BookCode,
                            quantity = y.Count,
                            length = y.Size,
                            colorId = y.ProductColor,
                            orderNumber = y.Order.OrderNumber,
                            orderSerial = y.Order.OrderSerial
                        }).ToList(),
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.detailList.ForEach(y =>
                        {
                            if (y.colorId == ProductColor.Gold)
                            {
                                y.color = "green";
                            }
                            if (y.colorId == ProductColor.White)
                            {
                                y.color = "White";
                            }
                            if (y.colorId == ProductColor.Rosegold)
                            {
                                y.color = "red";
                            }
                        });
                    });
                    response = new Response()
                    {
                        data = list
                    };
                    if (data != null)
                    {
                        data.ForEach(x =>
                        {
                            //x.OrderDetailStatus = OrderDetailStatus.UnderConstruction;
                            x.ModifyDate = DateTime.Now;
                            x.ModifyUserId = 1196;
                        });
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public class OViewModel
        {
            public int id { get; set; }
            public List<LilynikoViewModel> detailList { get; set; }
        }
        public class LilynikoViewModel
        {
            public int customerId { get; set; }
            public string customer_code { get; set; }
            public DateTime order_entry_date { get; set; }
            public string order_karat { get; set; }
            public string product_code { get; set; }
            public int quantity { get; set; }
            public string length { get; set; }
            public ProductColor? colorId { get; set; }
            public string color { get; set; }
            public string orderNumber { get; set; }
            public string orderSerial { get; set; }
        }
    }
}