using ClosedXML.Excel;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class CustomerCardController : BaseController
    {
        string adminusername = "1091147";
        string adminpassword = Sha1Hash("880866252");

        [Authorize(Roles = "admin, customerCardCharge,customerCardChargeAggrement, customerCard")]
        public ActionResult Index()
        {
            var currentUser = GetAuthenticatedUser();
            ViewBag.UserList = new KiaGalleryContext().Person.Where(x => x.BranchId == currentUser.BranchId && x.Active == true).ToList();

            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult Add()
        {
            return View("Edit");
        }
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult UserInfo()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult UserInfoUpdate()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult UserDebit()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult UserReport()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult UserBalance()
        {
            return View();
        }
        [Authorize(Roles = "admin,customerCardChargeAggrement,customerCardCharge")]
        public ActionResult ChargeAccount()
        {
            ViewBag.Branch = new KiaGalleryContext().Branch.Where(x => x.BranchType == BranchType.Branch || x.BranchType == BranchType.Other).ToList();
            return View();
        }
        [Authorize(Roles = "admin,customerCardChargeAggrement,customerCardCharge")]
        public ActionResult BranchChargeAccountList()
        {
            ViewBag.Branch = new KiaGalleryContext().Branch.Where(x => x.BranchType == BranchType.Branch || x.BranchType == BranchType.Other).ToList();
            return View();
        }
        [Authorize(Roles = "admin,customerCard")]
        public ActionResult Gift()
        {
            return View();
        }
        [Authorize(Roles = "admin,customerCard")]
        public ActionResult CustomerFactor()
        {
            return View();
        }

        public JsonResult SaveProductCode(CustomerFactorProductCodeViewModel model)
        {
            Response response;

            try
            {
                var currentUser = GetAuthenticatedUser();

                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.CustomerFactorProductCode.Find(model.id);
                        entity.Mobile = model.mobile;
                        entity.ProductCode = model.productCode;
                        entity.FactorNumber = model.factorNumber;
                        entity.Date = DateUtility.GetDateTime(model.persianDate).Value;
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                    }
                    else
                    {
                        var entity = new CustomerFactorProductCode
                        {
                            Mobile = model.mobile,
                            UserInfoId = model.userInfoId,
                            PersonId = 113,
                            ProductCode = model.productCode,
                            FactorNumber = model.factorNumber,
                            Date = DateUtility.GetDateTime(model.persianDate).Value,
                            CreateUserId = currentUser.Id,
                            ModifyUserId = currentUser.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        };
                        db.CustomerFactorProductCode.Add(entity);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "اطلاعات با موفقیت ذخیره شد."
                };

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin")]
        public JsonResult Save(CustomerCardViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                XLWorkbook Workbook = new XLWorkbook(@"d:\discharge.xlsx");
                IXLWorksheet Worksheet = Workbook.Worksheet("shablondt");
                Worksheet.Rows(2, 999).Delete();
                int NumberOfLastRow = Worksheet.LastRowUsed().RowNumber();
                IXLCell CellForNewData1 = Worksheet.Cell(NumberOfLastRow + 1, 1);
                IXLCell CellForNewData2 = Worksheet.Cell(NumberOfLastRow + 1, 2);
                IXLCell CellForNewData3 = Worksheet.Cell(NumberOfLastRow + 1, 3);
                IXLCell CellForNewData4 = Worksheet.Cell(NumberOfLastRow + 1, 4);
                CellForNewData1.SetValue("1003_00");
                CellForNewData2.SetValue(model.customerCode);
                CellForNewData3.SetValue("1098000");
                CellForNewData4.SetValue(model.price.ToString());
                Workbook.SaveAs(@"d:\discharge.xlsx");

                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.CustomerCard.Find(model.id);
                        entity.CustomerCode = model.customerCode;
                        entity.Price = model.price;
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                    }
                    else
                    {
                        var entity = new CustomerCard
                        {
                            CustomerCode = model.customerCode,
                            Price = model.price,
                            BranchId = currentUser.BranchId.Value,
                            CreateUserId = currentUser.Id,
                            ModifyUserId = currentUser.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Ip = Request.UserHostAddress
                        };
                        db.CustomerCard.Add(entity);
                    }
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "اطلاعات با موفقیت ذخیره شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin")]
        public JsonResult Load(int id)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.CustomerCard.Where(x => x.Id == id).Select(x => new
                    {
                        customerCode = x.CustomerCode,
                        price = x.Price
                    }).Single();
                    response = new Response()
                    {
                        status = 200,
                        data = entity
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CardChargeLoad(int id)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.CardTransaction.Where(x => x.Id == id).Select(x => new LoadCardChargeViewModel
                    {
                        id = x.Id,
                        amount = x.Amount,
                        descriptionType = x.DescriptionType,
                        description = x.Description,
                        factorNumber = x.FactorNumber,
                        userInfoId = x.UserInfoId,
                        cariorCode = x.UserInfo.CarriorCode,
                    }).Single();
                    response = new Response()
                    {
                        status = 200,
                        data = entity
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin")]
        public JsonResult Search(CustomerCardSearchViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                var dataCount = 0;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CustomerCard.Select(x => x);
                    if (!string.IsNullOrEmpty(model.word))
                    {
                        query = query.Where(x => x.CustomerCode.Contains(model.word));
                    }
                    dataCount = query.Count();
                    query.OrderByDescending(x => x.Id).Skip(model.page * model.count).Take(model.count);
                    var list = query.Select(x => new CustomerCardViewModel
                    {
                        customerCode = x.CustomerCode,
                        price = x.Price,
                        branch = x.Branch.Name

                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list,
                            pageCount = Math.Ceiling((double)dataCount / model.count),
                            count = dataCount,
                            page = model.page + 1
                        },

                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin")]
        public JsonResult ClearFile(string persianDate)
        {
            Response response;
            var date = DateUtility.GetDateTime(persianDate);
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CustomerCard.Select(x => x).ToList();
                    DataTable dt = new DataTable();
                    dt.Columns.AddRange(new DataColumn[6]
                    {  new DataColumn("projectcode", typeof(string)),
                       new DataColumn("fromCard", typeof(string)),
                       new DataColumn("toCard",typeof(string)),
                       new DataColumn("amount",typeof(string)),
                       new DataColumn("UserName",typeof(string)),
                       new DataColumn("Branch",typeof(string)),
                    });
                    foreach (var item in query)
                    {
                        dt.Rows.Add("1003_00", item.CustomerCode, "1098000", item.Price, item.CreateUser.FirstName + " " + item.CreateUser.LastName, item.Branch.Name);
                    }
                    string folderPath = "d:\\Excel\\";
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Customers");

                        wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
                        string myName = Server.UrlEncode("Test" + "_" +
                        DateTime.Now.ToShortDateString() + ".xlsx");
                        MemoryStream stream = GetStream(wb);// The method is defined below
                        Response.Clear();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition",
                        "attachment; filename=" + myName);
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.BinaryWrite(stream.ToArray());
                        Response.End();
                    }
                }
                response = new Response()
                {
                    status = 200,
                    message = "اطلاعات با موفقیت حذف شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin,customerCardCharge")]
        public JsonResult CardChargeTransaction(CardChargeAggreementViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    if (model.id > 0)
                    {
                        var entity = db.CardTransaction.Single(x => x.Id == model.id);
                        entity.Amount = Convert.ToInt64(model.amount);
                        entity.Description = model.description;
                        entity.FactorNumber = model.factorNumber;
                        entity.Status = CardTransactionStatus.Waiting;
                        entity.Type = CardTransactionType.Charge;
                        entity.DescriptionType = model.descriptionType;
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyIp = Request.UserHostAddress;
                    }
                    else
                    {
                        var entity = new CardTransaction()
                        {
                            Amount = Convert.ToInt64(model.amount),
                            UserId = model.userId,
                            Description = model.description,
                            FactorNumber = model.factorNumber,
                            Status = CardTransactionStatus.Waiting,
                            Type = CardTransactionType.Charge,
                            DescriptionType = model.descriptionType,
                            UserInfoId = model.userInfoId,
                            CreateUserId = currentUser.Id,
                            ModifyUserId = currentUser.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreateIp = Request.UserHostAddress,
                            ModifyIp = Request.UserHostAddress
                        };
                        db.CardTransaction.Add(entity);
                    }
                    db.SaveChanges();
                }

                response = new Response()
                {
                    status = 200,
                    message = "اطلاعات با موفقیت ارسال شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin,customerCardCharge")]
        public JsonResult CardChargeDelete(int id)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.CardTransaction.Find(id);
                    db.CardTransaction.Remove(entity);
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "اطلاعات با موفقیت ارسال شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin,customerCardCharge,customerCardChargeAggrement")]
        public JsonResult CardChargeDeny(int id)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.CardTransaction.Find(id);

                    entity.Status = CardTransactionStatus.Deny;
                    entity.CreateUserId = currentUser.Id;
                    entity.ModifyUserId = currentUser.Id;
                    entity.CreateDate = DateTime.Now;
                    entity.ModifyDate = DateTime.Now;
                    entity.CreateIp = Request.UserHostAddress;
                    entity.ModifyIp = Request.UserHostAddress;
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
        [Authorize(Roles = "admin,customerCardCharge,customerCardChargeAggrement")]
        public JsonResult Checkout(CheckoutViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.CardTransaction.Find(model.id);

                    entity.Status = CardTransactionStatus.Checkout;
                    entity.CheckoutDescription = model.description;
                    entity.ModifyUserId = currentUser.Id;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyIp = Request.UserHostAddress;
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
        [Authorize(Roles = "admin,customerCardCharge,customerCardChargeAggrement")]
        public JsonResult CardChargeAggreementList(CardChargeAggreementViewModel model)
        {
            Response response;
            int dataCount;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CardTransaction.Where(x => x.Amount > 0 && x.Type == CardTransactionType.Charge);
                    if (model.status > 0)
                    {
                        query = query.Where(x => x.Status == model.status);
                    }
                    if (model.descriptionType >= 0)
                    {
                        query = query.Where(x => x.DescriptionType == model.descriptionType);
                    }
                    if (!User.IsInRole("admin"))
                    {
                        if (User.IsInRole("customerCardChargeCrm"))
                        {
                            query = query.Where(x => x.DescriptionType == CardTransactionDescription.Crm);
                        }
                        if (User.IsInRole("customerCardChargeAccounting"))
                        {
                            query = query.Where(x => x.DescriptionType == CardTransactionDescription.Accounting || x.DescriptionType == CardTransactionDescription.Other);
                        }
                    }
                    if (model.branch > 0)
                    {
                        query = query.Where(x => x.CreateUser.Branch.Id == model.branch);
                    }
                    if (!string.IsNullOrEmpty(model.word))
                    {
                        query = query.Where(x => x.CreateUser.Branch.Name.Contains(model.word) || x.UserInfo.FullName.Contains(model.word) || x.UserInfo.Mobile.Contains(model.word) || x.UserInfo.CarriorCode.Contains(model.word));
                    }

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.CreateUser.Branch.Id).ThenByDescending(x => x.CreateDate).Skip(model.page * model.count).Take(model.count);
                    var data = query.Select(x => new CardChargeAggreementViewModel
                    {
                        id = x.Id,
                        description = x.Description,
                        checkoutDescription = x.CheckoutDescription,
                        userInfoId = x.UserInfoId,
                        amount = x.DescriptionType == CardTransactionDescription.Other ? x.Amount / 100 * x.UserInfo.PointPercent : x.Amount,
                        requestAmount = x.Amount,
                        fullName = x.UserInfo.FullName,
                        mobile = x.UserInfo.Mobile,
                        createUser = x.CreateUser.FirstName + " " + x.CreateUser.LastName,
                        createPerson = x.Person.FirstName + " " + x.Person.LastName,
                        createDate = x.CreateDate,
                        cariorCode = x.UserInfo.CarriorCode,
                        descriptionType = x.DescriptionType,
                        factorNumber = x.FactorNumber,
                        pointPercent = x.UserInfo.PointPercent,
                        status = x.Status

                    }).ToList();
                    data.ForEach(x =>
                    {
                        x.persianCreateDate = DateUtility.GetPersianDate(x.createDate);
                        x.amountSeparator = Core.ToSeparator(x.amount);
                        x.requestAmountSeparator = Core.ToSeparator(x.requestAmount);
                        x.descriptionTypeTitle = Enums.GetTitle(x.descriptionType);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
                            sumRequestAmount = Core.ToSeparator(data.Sum(x => x.requestAmount)),
                            sumChargeAmount = Core.ToSeparator(data.Sum(x => x.amount)),
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

        [Authorize(Roles = "admin,customerCardCharge,customerCardChargeAggrement")]
        public JsonResult BranchCardChargeList(CardChargeAggreementViewModel model)
        {
            Response response;
            int dataCount;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.CardTransaction.Where(x => x.Amount > 0 && x.Type == CardTransactionType.Charge && x.CreateUser.BranchId == currentUser.BranchId);
                    if (model.status > 0)
                    {
                        query = query.Where(x => x.Status == model.status);
                    }
                    if (model.descriptionType >= 0)
                    {
                        query = query.Where(x => x.DescriptionType == model.descriptionType);
                    }
                    if (!string.IsNullOrEmpty(model.word))
                    {
                        query = query.Where(x => x.CreateUser.Branch.Name.Contains(model.word) || x.UserInfo.FullName.Contains(model.word) || x.UserInfo.Mobile.Contains(model.word) || x.UserInfo.CarriorCode.Contains(model.word));
                    }
                    if (!string.IsNullOrEmpty(model.persianCreateDate))
                    {
                        var date = DateUtility.GetDateTime(model.persianCreateDate);
                        query = query.Where(x => DbFunctions.TruncateTime(x.CreateDate) == DbFunctions.TruncateTime(date));
                    }
                    dataCount = query.Count();
                    query = query.OrderBy(x => x.CreateUser.Branch.Id).ThenByDescending(x => x.CreateDate).Skip(model.page * model.count).Take(model.count);
                    var data = query.Select(x => new CardChargeAggreementViewModel
                    {
                        id = x.Id,
                        description = x.Description,
                        checkoutDescription = x.CheckoutDescription,
                        userInfoId = x.UserInfoId,
                        amount = x.DescriptionType == CardTransactionDescription.Other ? x.Amount / 100 * x.UserInfo.PointPercent : x.Amount,
                        requestAmount = x.Amount,
                        fullName = x.UserInfo.FullName,
                        mobile = x.UserInfo.Mobile,
                        createUser = x.CreateUser.FirstName + " " + x.CreateUser.LastName,
                        createPerson = x.Person.FirstName + " " + x.Person.LastName,
                        createDate = x.CreateDate,
                        cariorCode = x.UserInfo.CarriorCode,
                        descriptionType = x.DescriptionType,
                        factorNumber = x.FactorNumber,
                        pointPercent = x.UserInfo.PointPercent,

                    }).ToList();
                    data.ForEach(x =>
                    {
                        x.persianCreateDate = DateUtility.GetPersianDate(x.createDate);
                        x.amountSeparator = Core.ToSeparator(x.amount);
                        x.requestAmountSeparator = Core.ToSeparator(x.requestAmount);
                        x.descriptionTypeTitle = Enums.GetTitle(x.descriptionType);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
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

        public JsonResult UserInfoAutoCompelete(string term)
        {
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var list = db.UserInfo.Where(x => x.Mobile.Contains(term) || x.FullName.Contains(term) || x.FirstName.Contains(term) || x.LastName.Contains(term)).Select(x => new LoadUserInfoViewModel
                    {
                        id = x.Id,
                        mobile = x.Mobile,
                        fullName = x.FullName,

                    }).Take(5).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return Json("[]", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetLogList(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {

                    var list = db.CustomerFactorProductCode.Where(x => x.UserInfoId == id).Select(x => new CustomerFactorProductCodeViewModel
                    {
                        productCode = x.ProductCode,
                        date = x.Date,
                        factorNumber = x.FactorNumber,
                        mobile = x.Mobile

                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.persianDate = DateUtility.GetPersianDate(x.date);
                        x.productCode = x.productCode.Replace('-', ',');
                    });
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


        //[Authorize(Roles = "admin,customerCardCharge,customerCardChargeAggrement")]
        //public JsonResult SearchPersonelCard(CardChargeAggreementViewModel model)
        //{
        //    Response response;
        //    int dataCount;
        //    var currentUser = GetAuthenticatedUser();
        //    try
        //    {
        //        using (var db = new KiaGalleryContext())
        //        {
        //            var query = db.UserInfo.Where(x => x.CarriorCode >= 1094940);
        //            if (model.status > 0)
        //            {
        //                query = query.Where(x => x.Status == model.status);
        //            }
        //            if (model.descriptionType >= 0)
        //            {
        //                query = query.Where(x => x.DescriptionType == model.descriptionType);
        //            }
        //            if (!string.IsNullOrEmpty(model.word))
        //            {
        //                query = query.Where(x => x.CreateUser.Branch.Name.Contains(model.word) || x.UserInfo.FullName.Contains(model.word) || x.UserInfo.Mobile.Contains(model.word) || x.UserInfo.CarriorCode.Contains(model.word));
        //            }
        //            dataCount = query.Count();
        //            query = query.OrderBy(x => x.CreateUser.Branch.Id).ThenBy(x => x.CreateDate).Skip(model.page * model.count).Take(model.count);
        //            var data = query.Select(x => new CardChargeAggreementViewModel
        //            {
        //                id = x.Id,
        //                description = x.Description,
        //                userInfoId = x.UserInfoId,
        //                amount = x.DescriptionType == CardTransactionDescription.Other ? x.Amount / 100 * x.UserInfo.PointPercent : x.Amount,
        //                requestAmount = x.Amount,
        //                fullName = x.UserInfo.FullName,
        //                mobile = x.UserInfo.Mobile,
        //                createUser = x.CreateUser.FirstName + " " + x.CreateUser.LastName,
        //                createDate = x.CreateDate,
        //                cariorCode = x.UserInfo.CarriorCode,
        //                descriptionType = x.DescriptionType,
        //                factorNumber = x.FactorNumber,
        //                pointPercent = x.UserInfo.PointPercent,

        //            }).ToList();
        //            data.ForEach(x =>
        //            {
        //                x.persianCreateDate = DateUtility.GetPersianDate(x.createDate);
        //                x.amountSeparator = Core.ToSeparator(x.amount);
        //                x.requestAmountSeparator = Core.ToSeparator(x.requestAmount);
        //                x.descriptionTypeTitle = Enums.GetTitle(x.descriptionType);
        //            });
        //            response = new Response()
        //            {
        //                status = 200,
        //                data = new
        //                {
        //                    list = data,
        //                    pageCount = Math.Ceiling((double)dataCount / model.count),
        //                    count = dataCount,
        //                    page = model.page + 1
        //                }
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = Core.GetExceptionResponse(ex);
        //    }
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        #region Api

        [Authorize(Roles = "admin, customerCardCharge, customerCard")]
        public JsonResult LoadUserInfo(string id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                var model = new UserInfoRequestViewModel();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.UserInfo.Where(x => /*x.Active == true &&*/ (x.CarriorCode == id || x.Mobile == id || x.CardNumber == id) && x.PointPercent != 0);
                    var data = entity.Select(x => new LoadUserInfoViewModel
                    {
                        id = x.Id,
                        replica = x.Replica ? "المثنی" : "",
                        cariorcard = x.CarriorCode,
                        cardNo = x.CardNumber,
                        fullName = x.FullName,
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        email = x.Email,
                        mobile = x.Mobile,
                        ntCode = x.NationalCode,
                        telHome = x.Telephone,
                        homeAddr = x.Address,
                        area = x.Area,
                        gBirthDate = x.BirthDate,
                        marriedDate = x.MarriedDate,
                        pointPercent = x.PointPercent,
                        branchName = x.ModifyUser.Branch.Name
                    }).SingleOrDefault();
                    if (data != null)
                    {
                        data.birthDate = DateUtility.GetPersianDate(data.gBirthDate);
                        data.persianMarriedDate = DateUtility.GetPersianDate(data.marriedDate);
                        response = new Response()
                        {
                            status = 200,
                            data = data
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "موردی یافت نشد."
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
        [Authorize(Roles = "admin, customerCardCharge, customerCard")]
        public JsonResult LoadUserInfoGift(string id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                var model = new UserInfoRequestViewModel();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.UserInfo.Where(x => /*x.Active == true &&*/ (x.CarriorCode == id || x.Mobile == id || x.CardNumber == id) && x.PointPercent == 0);
                    var data = entity.Select(x => new LoadUserInfoViewModel
                    {
                        id = x.Id,
                        replica = x.Replica ? "المثنی" : "",
                        cariorcard = x.CarriorCode,
                        cardNo = x.CardNumber,
                        fullName = x.FullName,
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        email = x.Email,
                        mobile = x.Mobile,
                        ntCode = x.NationalCode,
                        telHome = x.Telephone,
                        homeAddr = x.Address,
                        area = x.Area,
                        gBirthDate = x.BirthDate,
                        marriedDate = x.MarriedDate,
                        pointPercent = x.PointPercent,
                    }).SingleOrDefault();
                    if (data != null)
                    {
                        data.birthDate = DateUtility.GetPersianDate(data.gBirthDate);
                        data.persianMarriedDate = DateUtility.GetPersianDate(data.marriedDate);
                        response = new Response()
                        {
                            status = 200,
                            data = data
                        };
                    }
                    else
                    {
                        response = new Response()
                        {
                            status = 404,
                            message = "موردی یافت نشد."
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
        [Authorize(Roles = "admin, customerCardCharge, customerCard")]
        public JsonResult Report(string id)
        {
            Response response;
            long kiaPoint = 0;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    kiaPoint = db.UserInfo.Single(x => x.CarriorCode == id || x.Mobile == id || x.CardNumber == id).KiaPoint;
                }
                var model = new CardReportRequestViewModel();
                var currentUser = GetAuthenticatedUser();
                model.adminusername = adminusername;
                model.adminpassword = adminpassword;
                model.dcCode = "0";
                model.merchantName = "0";
                model.pageSize = "99999999";
                model.mobile = id;
                if (!string.IsNullOrEmpty(model.fDate))
                    model.fDate = model.fDate.Replace("/", "-");
                if (!string.IsNullOrEmpty(model.tDate))
                    model.tDate = model.tDate.Replace("/", "-");
                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/accountreport");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<CardReportResponseViewModel>(responsed.Content);
                if (data.responseStatus == "notconfirmed")
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "وضعیت این کارت غیر فعال یا سوخته می باشد، لطفا کارت دیگری انتخاب کنید."
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                if (data.pointPercent == 1.0)
                {
                    data.level = "پایه";
                }
                if (data.pointPercent == 1.5)
                {
                    data.level = "نقره ای";
                }
                if (data.pointPercent == 2.0)
                {
                    data.level = "وی آی پی";
                }
                if (data.lstTransaction != null)
                {
                    data.debitCount = data.lstTransaction.Where(x => x.debitAmount > 0).Count();
                    data.creditCount = data.lstTransaction.Where(x => x.creditAmount > 0).Count();
                    data.debitSum = data.lstTransaction.Where(x => x.debitAmount > 0).Sum(x => x.debitAmount);
                    data.creditSum = data.lstTransaction.Where(x => x.creditAmount > 0).Sum(x => x.creditAmount);
                    data.localBalanceSeparator = Core.ToSeparator(data.localBalance);
                    data.debitCountSeparator = Core.ToSeparator(data.debitCount);
                    data.creditCountSeparator = Core.ToSeparator(data.creditCount);
                    data.debitSumSeparator = Core.ToSeparator(data.debitSum);
                    data.creditSumSeparator = Core.ToSeparator(data.creditSum);
                    using (var db = new KiaGalleryContext())
                    {
                        long charge = db.CardTransaction.Where(x => x.UserInfo.CarriorCode == id && x.Type == CardTransactionType.Charge && x.Status == CardTransactionStatus.Agreed).Sum(x => (long?)x.Amount) ?? 0;

                        var chargeCount = db.CardTransaction.Where(x => x.UserInfo.CarriorCode == id && x.Type == CardTransactionType.Charge && x.Status == CardTransactionStatus.Agreed).Count();


                        var debit = db.CardTransaction.Where(x => x.UserInfo.CarriorCode == id && x.Type == CardTransactionType.Debit).Sum(x => (long?)x.Amount) ?? 0;

                        var debitCount = db.CardTransaction.Where(x => x.UserInfo.CarriorCode == id && x.Type == CardTransactionType.Debit && x.Status == CardTransactionStatus.Agreed).Count();

                        data.kiaPoint = Math.Round(((data.creditSum - charge) / 10000) + (data.creditCount - chargeCount) * 2 - (debitCount * 2));
                    }

                    if (data.kiaPoint > 0 && data.kiaPoint <= 500)
                    {
                        data.kiaLevel = "1";
                    }
                    if (data.kiaPoint > 500 && data.kiaPoint <= 1500)
                    {
                        data.kiaLevel = "2";
                    }
                    if (data.kiaPoint > 1500)
                    {
                        data.kiaLevel = "VIP";
                    }
                    data.lstTransaction.ForEach(x =>
                    {
                        x.creditAmountSeparator = Core.ToSeparator(x.creditAmount);
                        x.debitAmountSeparator = Core.ToSeparator(x.debitAmount);

                        var date = UnixTimeToDateTime(long.Parse(x.trnDate));
                        x.persianTrnDate = DateUtility.GetPersianDate(date);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = data,
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 200,
                        message = "سوابقی یافت نشد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin, customerCardCharge, customerCard")]
        public JsonResult Debit(DebitRequestViewModel model)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/debit");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<DebitResponseViewModel>(responsed.Content);

                response = new Response()
                {
                    status = 200,
                    data = data
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin, customerCardCharge, customerCard,customerCardChargeAggrement")]
        public JsonResult AdminDebit(DebitRequestViewModel model)
        {
            Response response;
            try
            {
                if (string.IsNullOrEmpty(model.cariorcard))
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "موردی یافت نشد."
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                var currentUser = GetAuthenticatedUser();

                model.adminusername = adminusername;
                model.adminpassword = adminpassword;
                model.card = model.cariorcard;
                model.stan = "0";
                model.rrn = "0";
                model.channelvalue = "0";
                model.amount = Math.Round((double.Parse(model.amount) * model.pointPercent / 100)).ToString();
                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/debit");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<DebitResponseViewModel>(responsed.Content);
                using (var db = new KiaGalleryContext())
                {
                    var entity = new CardTransaction()
                    {
                        Amount = long.Parse(model.amount),
                        UserInfoId = model.userInfoId,
                        UserId = model.userId,
                        Type = CardTransactionType.Debit,
                        Status = CardTransactionStatus.Agreed,
                        CreateUserId = currentUser.Id,
                        ModifyUserId = currentUser.Id,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        CreateIp = Request.UserHostAddress,
                        ModifyIp = Request.UserHostAddress,
                    };
                    db.CardTransaction.Add(entity);
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "مبلغ مورد نظر با موفقیت کسر شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin, customerCardCharge, customerCard,customerCardChargeAggrement")]
        public JsonResult DisableCard(DebitRequestViewModel model)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();

                model.adminusername = adminusername;
                model.adminpassword = adminpassword;
                model.card = model.cariorcard;
                model.stan = "0";
                model.rrn = "0";
                model.channelvalue = "0";
                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/debit");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<DebitResponseViewModel>(responsed.Content);
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.UserInfo.Where(x => x.CarriorCode == model.cariorcard).SingleOrDefault();
                    entity.Active = false;
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "مبلغ مورد نظر با موفقیت کسر شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [Authorize(Roles = "admin, customerCardCharge, customerCard,customerCardChargeAggrement")]
        public JsonResult Balance(BalanceRequestViewModel model)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                model.adminusername = adminusername;
                model.adminpassword = adminpassword;
                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/balance");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                request.Timeout = 999999999;
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<BalanceResponseViewModel>(responsed.Content);
                data.balanceAmount = Core.ToSeparator(long.Parse(data.balanceAmount));
                response = new Response()
                {
                    status = 200,
                    data = data
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin, customerCardCharge, customerCard,customerCardChargeAggrement")]
        public JsonResult UpdateUserInfo(UpdateUserInfoRequestViewModel model)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                model.adminusername = adminusername;
                model.adminpassword = adminpassword;
                model.fullName = model.fullName;
                if (string.IsNullOrEmpty(model.jbirthDate) || string.IsNullOrWhiteSpace(model.jbirthDate))
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "تاریخ تولد به درستی وارد نشده است."
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.fullName) || model.fullName.Trim() == "بي نام" || model.fullName.Trim() == "بی نام" || model.fullName.Trim() == "بینام")
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "نام مشتری به درستی وارد نشده است."
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.fullName) || model.fullName.Trim() == "بي نام" || model.fullName.Trim() == "بی نام" || model.fullName.Trim() == "بینام")
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "نام مشتری به درستی وارد نشده است."
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/updateuserinfo");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<UpdateUserInfoResponseViewModel>(responsed.Content);
                if (data.responseStatus == "success")
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var entity = db.UserInfo.Where(x => x.CarriorCode == model.cariorcard).SingleOrDefault();

                        if (entity.Mobile != model.mobile)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                NikSmsWebServiceClient.SendSmsNik(string.Format("{0} به باشگاه مشتریان گالری کیا خوش آمدید", model.fullName), model.mobile);
                            });
                        }
                        entity.CardNumber = model.cardNo;
                        entity.Gift = false;
                        entity.FirstName = model.firstName;
                        entity.LastName = model.lastName;
                        entity.FullName = model.fullName;
                        entity.Email = model.email;
                        entity.Mobile = model.mobile;
                        entity.NationalCode = model.ntCode;
                        entity.Telephone = model.telHome;
                        entity.Address = model.homeAddr;
                        entity.Area = model.area;
                        entity.BirthDate = DateUtility.GetDateTime(model.jbirthDate).Value;
                        entity.MarriedDate = DateUtility.GetDateTime(model.persianMarriedDate);
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyIp = Request.UserHostAddress;
                        db.SaveChanges();
                    }
                    response = new Response()
                    {
                        status = 200,
                        message = "اطلاعات کاربر با موفقیت به روز رسانی شد."
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 200,
                        message = " اطلاعات ورودی نادرست است و یا شماره همراه تکراری می باشد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, customerCardCharge, customerCard,customerCardChargeAggrement")]
        public JsonResult UpdateUserInfoGift(UpdateUserInfoRequestViewModel model)
        {
            Response response;
            try
            {
                Random random = new Random();
                const string chars = "0123456789";
                var randomNum = new string(Enumerable.Repeat(chars, 11).Select(s => s[random.Next(s.Length)]).ToArray());
                var currentUser = GetAuthenticatedUser();
                model.adminusername = adminusername;
                model.adminpassword = adminpassword;
                model.fullName = model.fullName;
                model.jbirthDate = DateUtility.GetPersianDate(DateTime.Now);
                model.mobile = randomNum;

                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/updateuserinfo");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<UpdateUserInfoResponseViewModel>(responsed.Content);
                if (data.responseStatus == "success")
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var entity = db.UserInfo.Where(x => x.CarriorCode == model.cariorcard).SingleOrDefault();
                        entity.CardNumber = model.cardNo;
                        entity.Gift = true;
                        entity.FirstName = model.firstName;
                        entity.LastName = model.lastName;
                        entity.FullName = model.fullName;
                        entity.Email = model.email;
                        entity.Mobile = model.mobileNumber;
                        entity.NationalCode = model.ntCode;
                        entity.Telephone = model.telHome;
                        entity.Address = model.homeAddr;
                        entity.Area = model.area;
                        entity.BirthDate = DateTime.Now;
                        entity.MarriedDate = DateUtility.GetDateTime(model.persianMarriedDate);
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyIp = Request.UserHostAddress;
                        db.SaveChanges();
                    }
                    response = new Response()
                    {
                        status = 200,
                        message = "اطلاعات کاربر با موفقیت به روز رسانی شد."
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 200,
                        message = " اطلاعات ورودی نادرست است و یا شماره همراه تکراری می باشد."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin, customerCardCharge, customerCard,customerCardChargeAggrement")]
        public JsonResult ChargeAccountMerchant(ChargeAccountMerchantRequestViewModel model)
        {
            Response response;
            try
            {
                if (string.IsNullOrEmpty(model.cariorcard))
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "موردی یافت نشد."
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                using (var db = new KiaGalleryContext())
                {
                    var userName = db.UserInfo.Where(x => x.CarriorCode == model.cariorcard).SingleOrDefault().FullName;
                    if (userName.Trim().Equals("بي نام"))
                    {
                        response = new Response()
                        {
                            status = 500,
                            message = "این کارت برای مشتری ثبت نشده."
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                var refNum = StringUtility.RandomNumber();
                var transactionnumber = StringUtility.RandomNumber();
                var currentUser = GetAuthenticatedUser();
                model.merchantpassword = adminpassword;
                model.merchantId = "820000000008198";
                model.ipgcode = 11120;
                model.referencenumber = refNum;
                model.transactionnumber = transactionnumber;
                var amount = model.descriptionType != CardTransactionDescription.Other ? Math.Round(int.Parse(model.amount) * 100 / model.pointPercent) : int.Parse(model.amount);
                model.amount = amount.ToString();
                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/chargeaccountmerchant");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<ChargeAccountMerchantResponseViewModel>(responsed.Content);
                if (data.ResponseStatus == "Succeed")
                {
                    using (var db = new KiaGalleryContext())
                    {
                        var entity = db.CardTransaction.Single(x => x.Id == model.id);
                        entity.Status = CardTransactionStatus.Agreed;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyUserId = currentUser.Id;
                        db.SaveChanges();
                    }
                }
                else
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "خطایی از سمت پذیرنده رخ داد." + " " + data.ResponseStatus
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
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
        [Authorize(Roles = "admin, customerCardCharge, customerCard,customerCardChargeAggrement")]
        public JsonResult ForgotPassword(ForgotPasswordRequestViewModel model)
        {
            Response response;
            try
            {
                var param = JsonConvert.SerializeObject(model);
                var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/forgetportalpassword");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", param, ParameterType.RequestBody);
                IRestResponse responsed = client.Execute(request);
                var data = JsonConvert.DeserializeObject<ForgotPasswordResponseViewModel>(responsed.Content);
                if (!string.IsNullOrEmpty(data.newPassword))
                {

                    Task.Factory.StartNew(() =>
                    {
                        NikSmsWebServiceClient.SendSmsNik("گالری کیا\n رمز عبور جدید کارت شما :" + data.newPassword, model.mobile);
                    });
                    response = new Response()
                    {
                        status = 200,
                        message = "رمز عبور جدید برای مشتری ارسال شد."
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 200,
                        message = "شماره تلفن اشتباه است."
                    };
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin, customerCardCharge, customerCard,customerCardChargeAggrement")]
        public JsonResult ChangeCustomerCard(ChangeCardViewModel model)
        {
            Response response;
            try
            {
                model.adminusername = adminusername;
                model.adminpassword = adminpassword;
                using (var db = new KiaGalleryContext())
                {
                    var oldCard = db.UserInfo.Where(x => x.CarriorCode == model.cariorcard).Single();
                    oldCard.Active = false;
                    var newCard = db.UserInfo.Where(x => x.CarriorCode == model.newCariorCard).Single();
                    if (model.cardType == false)
                    {
                        if (newCard.PointPercent < oldCard.PointPercent)
                        {
                            response = new Response()
                            {
                                status = 200,
                                message = "سطح کارت مورد نظر پایین تر از کارت فعلی می باشد."
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                    }
                    newCard.Replica = model.cardType;
                    newCard.FirstName = oldCard.FirstName;
                    newCard.LastName = oldCard.LastName;
                    newCard.FullName = oldCard.FullName;
                    newCard.Address = oldCard.Address;
                    newCard.BirthDate = oldCard.BirthDate;
                    newCard.MarriedDate = oldCard.MarriedDate;
                    newCard.Sex = oldCard.Sex;
                    newCard.Email = oldCard.Email;
                    newCard.Mobile = oldCard.Mobile;
                    newCard.Telephone = oldCard.Telephone;
                    newCard.NationalCode = oldCard.NationalCode;
                    newCard.Active = true;
                    newCard.CardNumber = oldCard.CardNumber;
                    newCard.Point = oldCard.Point;
                    newCard.DiscountPercent = oldCard.DiscountPercent;
                    newCard.Area = oldCard.Area;
                    newCard.ModifyDate = DateTime.Now;
                    newCard.KiaPoint = model.kiaPoint.Value;

                    model.fullName = oldCard.FullName;
                    model.jbirthDate = DateUtility.GetPersianDate(oldCard.BirthDate);
                    model.ntCode = oldCard.NationalCode;
                    model.telHome = oldCard.Telephone;
                    model.homeAddr = oldCard.Address;
                    model.email = oldCard.Email;
                    model.cariorcard = model.newCariorCard;
                    model.mobile = oldCard.Mobile;

                    double chargeAmount = model.chargeAmount * 100;
                    double debitAmount = model.debitAmount;
                    if (chargeAmount > 0 || debitAmount > 0)
                    {
                        ChangeCardCharge(model.cariorcard, chargeAmount.ToString());
                        ChangeCardDebit(model.cariorcard, debitAmount.ToString());
                    }

                    ChangeOldCardMobile(oldCard.CarriorCode);
                    ChangeCardUpdateUserInfo(model);
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "تغییر کارت با موفقیت انجام شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        #endregion

        protected static string Sha1Hash(string password)
        {
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hashedPassword;
            }
        }
        protected DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
        protected MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }
        protected void ChangeCardDebit(string cariorCard, string amount)
        {
            var currentUser = GetAuthenticatedUser();
            var model = new DebitRequestViewModel();
            model.cariorcard = cariorCard;
            model.amount = amount;
            model.adminusername = adminusername;
            model.adminpassword = adminpassword;
            model.card = model.cariorcard;
            model.stan = "0";
            model.rrn = "0";
            model.channelvalue = "0";
            var param = JsonConvert.SerializeObject(model);
            var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/debit");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/json", param, ParameterType.RequestBody);
            IRestResponse responsed = client.Execute(request);
        }
        protected void ChangeCardCharge(string cariorCard, string amount)
        {
            var model = new ChargeAccountMerchantRequestViewModel();
            var refNum = StringUtility.RandomNumber();
            var transactionnumber = StringUtility.RandomNumber();
            var currentUser = GetAuthenticatedUser();
            model.merchantId = "820000000008198";
            model.merchantpassword = adminpassword;
            model.ipgcode = 11120;
            model.referencenumber = refNum;
            model.transactionnumber = transactionnumber;
            model.cariorcard = cariorCard;
            model.amount = amount;
            var param = JsonConvert.SerializeObject(model);
            var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/chargeaccountmerchant");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/json", param, ParameterType.RequestBody);
            IRestResponse responsed = client.Execute(request);
        }

        protected void ChangeCardUpdateUserInfo(ChangeCardViewModel model)
        {
            var param = JsonConvert.SerializeObject(model);
            var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/updateuserinfo");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/json", param, ParameterType.RequestBody);
            IRestResponse responsed = client.Execute(request);
        }

        protected void ChangeOldCardMobile(string cariCode)
        {
            Random random = new Random();
            const string chars = "0123456789";
            var randomNum = new string(Enumerable.Repeat(chars, 11).Select(s => s[random.Next(s.Length)]).ToArray());

            var model = new UpdateUserInfoRequestViewModel();
            model.cariorcard = cariCode;
            model.adminusername = adminusername;
            model.adminpassword = adminpassword;
            model.fullName = "سوخته";
            model.jbirthDate = DateUtility.GetPersianDate(DateTime.Now);
            var param = JsonConvert.SerializeObject(model);
            var client = new RestClient("https://restcore.irpointcenter.com/loyalbank/updateuserinfo");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/json", param, ParameterType.RequestBody);
            IRestResponse responsed = client.Execute(request);
        }
    }
}