using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class TutorialController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View("Edit");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Video()
        {
            return View();
        }

        public JsonResult Save(TutorialViewModel model)
        {
            Response response;
            try
            {

                var currentUser = GetAuthenticatedUser();
                string oldFileName = "";
                using (var db = new KiaGalleryContext())
                {
                    if (model.id > 0)
                    {

                        var entity = db.Tutorial.SingleOrDefault(x => x.Id == model.id);
                        entity.Title = model.title;
                        entity.Description = model.description;
                        entity.TutorialType = model.tutorialType;
                        entity.Active = model.active;
                        entity.FileName = model.fileName;
                        entity.VideoCoverFileName = model.videoCoverFileName;
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                        if (!string.IsNullOrEmpty(entity.FileName) && entity.FileName != model.fileName)
                            oldFileName = entity.FileName;
                    }
                    else
                    {
                        var entity = new Tutorial()
                        {
                            Title = model.title,
                            Description = model.description,
                            Active = model.active,
                            TutorialType = model.tutorialType,
                            FileName = model.fileName,
                            VideoCoverFileName = model.videoCoverFileName,
                            CreateUserId = currentUser.Id,
                            ModifyUserId = currentUser.Id,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        };
                        db.Tutorial.Add(entity);
                    }
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(oldFileName) && System.IO.File.Exists(Server.MapPath("~/Upload/tutorial/" + oldFileName)))
                        System.IO.File.Delete(Server.MapPath("~/Upload/tutorial/" + oldFileName));
                    response = new Response()
                    {
                        status = 200,
                        message = "اطلاعات ثبت شد"
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
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public JsonResult GetVideoList(TutorialSearchViewModel options)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Tutorial.Where(x => x.Active == true && x.TutorialType == TutorialType.Video);

                    if (!string.IsNullOrEmpty(options.word))
                    {
                        query = query.Where(x => x.Title.Contains(options.word));
                    }
                    var dataCount = query.Count();
                    var list = query.OrderByDescending(x => x.CreateDate).Skip(options.page * options.count).Take(options.count).Select(x => new TutorialViewModel
                    {
                        title = x.Title,
                        fileName = x.FileName,
                        tutorialType = x.TutorialType,
                        link = "/upload/tutorial/" + x.FileName,
                        coverLink = "/upload/tutorialCover/" + x.VideoCoverFileName,
                        description = x.Description
                    }).ToList();
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            pageCount = Math.Ceiling((double)dataCount / options.count),
                            count = dataCount,
                            page = options.page + 1
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
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public JsonResult Search(TutorialSearchViewModel options)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Tutorial.Select(x => x);

                    if (!string.IsNullOrEmpty(options.word))
                    {
                        query = query.Where(x => x.Title.Contains(options.word));
                    }
                    if (options.tutorialType != null && options.tutorialType >= 0)
                    {
                        query = query.Where(x => x.TutorialType == options.tutorialType);
                    }
                    var dataCount = query.Count();
                    var list = query.OrderByDescending(x => x.Id).Skip(options.page * options.count).Take(options.count).Select(x => new TutorialViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        fileName = x.FileName,
                        tutorialType = x.TutorialType,
                        active = x.Active,
                        description = x.Description,
                    }).ToList();
                    list.ForEach(x =>
                    {
                        x.tutorialTypeTitle = Enums.GetTitle(x.tutorialType);
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,
                            pageCount = Math.Ceiling((double)dataCount / options.count),
                            count = dataCount,
                            page = options.page + 1
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
        public JsonResult Load(int id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var data = db.Tutorial.Where(x => x.Id == id).Select(x => new TutorialViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        fileName = x.FileName,
                        videoCoverFileName = x.VideoCoverFileName,
                        tutorialType = x.TutorialType,
                        active = x.Active,
                        description = x.Description
                    }).SingleOrDefault();
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                var currentUser = GetAuthenticatedUser();
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Tutorial.Find(id);
                    db.Tutorial.Remove(entity);
                    db.SaveChanges();
                    response = new Response()
                    {
                        status = 200,
                        message = "آموزش با موفقیت حذف شد."
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