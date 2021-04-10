using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    /// <summary>
    /// کنترلر شعبه
    /// </summary>
    public class BranchController : BaseController
    {
        /// <summary>
        /// مدیریت شعب
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ویرایش
        /// </summary>
        /// <param name="Id">ردیف شعبه</param>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public ActionResult Edit(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Title = "ویرایش شعبه";
            return View();
        }
        /// <summary>
        /// شعبه جدید
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public ActionResult Add()
        {
            ViewBag.Title = "شعبه جدید";
            return View("Edit");
        }

        /// <summary>
        /// ذخیره شعبه
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات شعبه</param>
        /// <returns>جیسون حاوی اطلاعات نتیجه فرایند ثبت </returns>
        [HttpPost]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Save(BranchViewModel model)
        {
            Response response;
            try
            {
                int status = 200;
                string message = string.Empty;
                using (var db = new KiaGalleryContext())
                {
                    if (string.IsNullOrEmpty(model.name))
                    {
                        status = 500;
                        message = "وارد کردن نام شعبه اجباری است.";
                    }
                    else
                    {
                        if (model.id != null && model.id > 0)
                        {
                            var entity = db.Branch.Single(x => x.Id == model.id);
                            entity.WorkingHour = model.workingHour;
                            entity.OwnerName = model.ownerName;
                            entity.OwnerFatherName = model.ownerFatherName;
                            entity.OwnerNationalityCode = model.ownerNationalityCode;
                            entity.OwnerNationalityNo = model.ownerNationalityNo;
                            entity.CityId = model.cityId;
                            entity.Order = model.order;
                            entity.Alias = model.alias.Trim();
                            entity.Name = model.name.Trim();
                            entity.EnglishName = model.englishName?.Trim();
                            entity.Address = model.address?.Trim();
                            entity.Phone = model.phone;
                            entity.MobileNumber = model.mobileNumber;
                            entity.EnglishAddress = model.englishAddress?.Trim();
                            entity.Color = model.color?.Trim();
                            entity.Latitude = model.latitude?.Trim();
                            entity.Longitude = model.longitude?.Trim();
                            entity.Active = model.active;
                            entity.BranchType = model.branchType;
                            //entity.GoldCredit = model.goldCredit;
                            //entity.GoldDebt = model.goldDebt;
                            //entity.RialDebt = model.rialDebt;
                            entity.ModifyUserId = GetAuthenticatedUserId();
                            entity.ModifyDate = DateTime.Now;
                            entity.Ip = Request.UserHostAddress;

                            message = "شعبه با موفقیت ویرایش شد.";
                        }
                        else
                        {
                            var entity = new Branch()
                            {
                                WorkingHour = model.workingHour,
                                OwnerName = model.ownerName,
                                OwnerFatherName = model.ownerFatherName,
                                OwnerNationalityCode = model.ownerNationalityCode,
                                OwnerNationalityNo = model.ownerNationalityNo,
                                CityId = model.cityId,
                                Order = model.order,
                                Alias = model.alias.Trim(),
                                Name = model.name.Trim(),
                                EnglishName = model.englishName?.Trim(),
                                Address = model.address?.Trim(),
                                EnglishAddress = model.englishAddress?.Trim(),
                                Color = model.color?.Trim(),
                                Latitude = model.latitude?.Trim(),
                                Longitude = model.longitude?.Trim(),
                                Active = model.active,
                                Phone = model.phone,
                                BranchType = model.branchType,
                                //GoldDebt = model.goldDebt,
                                //GoldCredit = model.goldCredit,
                                //RialDebt = model.rialDebt,
                                GoldDebt = 0,
                                GoldCredit = 0,
                                RialDebt = 0,
                                CreateUserId = GetAuthenticatedUserId(),
                                ModifyUserId = GetAuthenticatedUserId(),
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Ip = Request.UserHostAddress
                            };

                            db.Branch.Add(entity);
                            message = "شعبه با موفقیت ایجاد شد.";
                        }
                        db.SaveChanges();
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

        /// <summary>
        /// خواندن اطلاعات شعبه
        /// </summary>
        /// <param name="id">ردیف شعبه</param>
        /// <returns>جیسون اطلاعات لود شده شعبه</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                Branch item;
                using (var db = new KiaGalleryContext())
                {
                    item = db.Branch.Include(x => x.City).FirstOrDefault(x => x.Id == id);
                }

                if (item != null)
                {
                    response = new Response()
                    {
                        status = 200,
                        data = new BranchViewModel
                        {
                            workingHour = item.WorkingHour,
                            ownerName = item.OwnerName,
                            ownerFatherName = item.OwnerFatherName,
                            ownerNationalityCode = item.OwnerNationalityCode,
                            ownerNationalityNo = item.OwnerNationalityNo,
                            id = item.Id,
                            provinceId = item.City.ParentId,
                            cityId = item.CityId,
                            order = item.Order,
                            alias = item.Alias,
                            name = item.Name,
                            englishName = item.EnglishName,
                            address = item.Address,
                            englishAddress = item.EnglishAddress,
                            color = item.Color,
                            latitude = item.Latitude,
                            longitude = item.Longitude,
                            active = item.Active,
                            phone = item.Phone,
                            mobileNumber = item.MobileNumber,
                            branchType = item.BranchType
                            //goldCredit = item.GoldCredit,
                            //goldDebt = item.GoldDebt,
                            //rialDebt = item.RialDebt
                        }
                    };
                }
                else
                {
                    response = new Response()
                    {
                        status = 404,
                        message = "شعبه مورد نظر یافت نشد."
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
        /// جستجوی شعب
        /// </summary>
        /// <param name="model">مدلی حاوی پارامترهای تاثیر گذار در جستجو</param>
        /// <returns>لیست شعب پیدا شده</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Search(BranchSearchViewModel model)
        {
            Response response;
            try
            {
                List<Branch> list;
                int dataCount;
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Branch.Select(x => x);

                    dataCount = query.Count();
                    query = query.OrderBy(x => x.Order).Skip(model.page * model.count).Take(model.count);

                    list = query.ToList();
                }

                response = new Response()
                {
                    status = 200,

                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            alias = item.Alias,
                            name = item.Name,
                            order = item.Order,
                            color = item.Color,
                            active = item.Active
                        }),
                        pageCount = Math.Ceiling((double)dataCount / model.count),
                        count = dataCount,
                        page = model.page + 1
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
        /// حذف شعبه
        /// </summary>
        /// <param name="id">ردیف شعبه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Delete(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Branch.Find(Id);

                    response = new Response()
                    {
                        status = 200,
                        message = "شعبه با موفقیت حذف شد."
                    };
                    db.Branch.Remove(item);
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
        /// فعال کردن شعبه
        /// </summary>
        /// <param name="id">ردیف شعبه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Inactive(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Branch.Find(Id);
                    item.Active = false;
                    response = new Response()
                    {
                        status = 200,
                        message = "شعبه با موفقیت غیرفعال شد."
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
        /// غیر فعال کردن شعبه
        /// </summary>
        /// <param name="id">ردیف شعبه</param>
        /// <returns>جیسون نتیجه عملیات خواسته شده</returns>
        [HttpPost]
        [Authorize(Roles = "admin, branch")]
        public JsonResult Active(int Id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var item = db.Branch.Find(Id);
                    item.Active = true;
                    response = new Response()
                    {
                        status = 200,
                        message = "شعبه با موفقیت فعال شد."
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
        /// فقط دریافت شعب و عدم نمایش نمایندگی
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBranch()
        {
            Response response;
            try
            {
                List<Branch> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Branch.Where(x => x.BranchType == BranchType.Branch).OrderBy(x => x.Order).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            alias = item.Alias,
                            name = item.Name
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
        /// دریافت همه شعبه های فعال
        /// </summary>
        /// <returns>جیسون حاوی لیست تمام شعبه ها</returns>
        [HttpGet]
        public JsonResult GetAll()
        {
            Response response;
            try
            {
                List<Branch> list;
                using (var db = new KiaGalleryContext())
                {
                    list = db.Branch.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                }

                response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = list.Select(item => new
                        {
                            id = item.Id,
                            alias = item.Alias,
                            name = item.Name
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
        /// مالی شعبه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpGet]
        [Authorize(Roles = "admin, branch")]
        public ActionResult Financial()
        {
            List<BranchFinancial> listBranchFinancial;
            using (var db = new KiaGalleryContext())
            {
                listBranchFinancial = db.Branch.Select(x => new BranchFinancial()
                {
                    id = x.Id,
                    name = x.Name,
                    goldCredit = x.GoldCredit,
                    goldDebt = x.GoldDebt,
                    rialDebt = x.RialDebt
                }).ToList();
            }
            ViewBag.ListBranchFinancial = listBranchFinancial;
            return View();
        }

        /// <summary>
        /// مالی شعبه
        /// </summary>
        /// <returns>صفحه مورد نظر</returns>
        [HttpPost]
        [Authorize(Roles = "admin, branch")]
        public JsonResult SaveFinancial()
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var listBranch = db.Branch.ToList();
                    response = new Response()
                    {
                        status = 200,
                        message = "شعبه با موفقیت فعال شد."
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

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetList()
        {
            Response response;
            try
            {

                using (var db = new KiaGalleryContext())
                {
                    var query = db.Branch.Select(x => x);
                    var list = query.Select(item => new BranchApiViewModel
                    {
                        id = item.Id,
                        alias = item.Alias,
                        name = item.Name,
                        address = item.Address,
                        englishName = item.EnglishName,
                        englishAddress = item.EnglishAddress,
                        order = item.Order,
                        color = item.Color,
                        active = item.Active,
                        branchType = item.BranchType,
                        cityId = item.CityId,
                        provinceId = item.City.ParentId,
                        
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.branchTypeTitle = Enums.GetTitle(x.branchType);
                    });
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
    }
}