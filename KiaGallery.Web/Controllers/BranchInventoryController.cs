using ClosedXML.Excel;
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
    public class BranchInventoryController : BaseController
    {
        [Authorize(Roles = "admin, branchInventory")]
        public ActionResult Index()
        {
            using (var db = new KiaGalleryContext())
            {
                ViewBag.BranchList = db.Branch.Where(x => x.Active == true && x.BranchType == BranchType.Branch).ToList();
            }
            return View();
        }
        [Authorize(Roles = "admin, branchInventory")]
        public JsonResult Import(int? id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                string serverPath = Server.MapPath("~/Temp/");
                HttpPostedFileBase hpf = Request.Files[0];
                if (hpf.ContentLength == 0)
                    throw new Exception("File length can't be equal to zero");

                string fileName = Path.GetFileName(hpf.FileName);
                string savedFileName = Path.Combine(serverPath, fileName);

                if (System.IO.File.Exists(savedFileName))
                {
                    Random random = new Random();
                    string prefix = random.Next(1000, 9999).ToString() + "-";
                    fileName = prefix + fileName;
                }
                savedFileName = Path.Combine(serverPath, fileName);
                if (Path.GetExtension(savedFileName) != ".xlsx")
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "فرمت فایل اشتباه است."
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }

                hpf.SaveAs(savedFileName);
                // Open the Excel file using ClosedXML.
                // Keep in mind the Excel file cannot be open when trying to read it
                IXLRange range;
                using (var db = new KiaGalleryContext())
                {
                    var branchId = id != null && id > 0 ? id : currentUser.BranchId;
                    var oldData = db.BranchInventory.Where(x => x.BranchId == branchId);
                    db.BranchInventory.RemoveRange(oldData);
                    using (XLWorkbook wb = new XLWorkbook(savedFileName))
                    {
                        var ws = wb.Worksheets.First();
                        range = ws.RangeUsed();

                        var firstCell = ws.Cell(1, 1).Value.ToString();
                        if (string.IsNullOrEmpty(firstCell) || string.IsNullOrWhiteSpace(firstCell) || firstCell.Trim() != "نام")
                        {
                            response = new Response()
                            {
                                status = 500,
                                message = "فرمت فایل اشتباه است."
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                        ws.FirstRow().Delete();
                        for (int i = 1; i < range.RowCount() + 1; i++)
                        {

                            var item = new BranchInventory()
                            {
                                Title = ws.Cell(i, 1).Value.ToString(),
                                ProductCode = ws.Cell(i, 2).Value.ToString(),
                                Weight = ws.Cell(i, 3).Value.ToString(),
                                BranchId = branchId.Value,
                                InventoryType = ws.Cell(i, 1).Value.ToString().Contains("*") ? InventoryType.Customer : InventoryType.Branch,
                                CreateUserId = currentUser.Id,
                                ModifyUserId = currentUser.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };
                            db.BranchInventory.Add(item);

                        }
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "فایل با موفقیت بارگزاری شد." + " " + range.RowCount()
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin, branchInventory")]
        public JsonResult Search(BranchInventoryViewModel model)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var query = db.BranchInventory.Where(x => !x.Title.Contains("*"));
                    var lastUpdate = db.BranchInventory.Where(x => !x.Title.Contains("*"));

                    if (!string.IsNullOrEmpty(model.word))
                    {
                        query = query.Where(x => x.Title.Contains(model.word) || x.ProductCode.Contains(model.word));
                    }
                    var dataCount = query.Count();
                    query = query.OrderBy(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new BranchInventoryViewModel
                    {
                        title = x.Title,
                        branchName = x.Branch.Name,
                        productCode = x.ProductCode,
                        weight = x.Weight,
                        inventoryType = x.InventoryType,
                        date = x.CreateDate
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.inventoryTypeTitle = Enums.GetTitle(x.inventoryType);
                        x.persianDate = DateUtility.GetPersianDateTime(x.date);
                    });
                    var lastUpdateList = lastUpdate.GroupBy(x => x.Branch).Select(x => new LastUpdateListViewModel
                    {
                        branchName = x.Key.Name,
                        date = x.OrderByDescending(y => y.CreateDate).FirstOrDefault().CreateDate
                    }).ToList();
                    lastUpdateList.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDateTime(x.date);
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
                            lastUpdateList
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