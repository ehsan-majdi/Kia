using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using KiaGallery.Web.SmsHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiaGallery.Web.Controllers
{
    public class TicketController : BaseController
    {

        public ActionResult Index()
        {
            var currentUser = GetAuthenticatedUser();
            List<UserListViewModel> userList = new List<UserListViewModel>();
            using (var db = new KiaGalleryContext())
            {
                userList = db.User.Where(x => x.Active == true && x.Id != currentUser.Id).Select(x => new UserListViewModel
                {
                    id = x.Id,
                    firstName = x.FirstName,
                    lastName = x.LastName
                }).ToList();
            }
            return View(userList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public ActionResult Message(int? id)
        //{
        //    using (var db = new KiaGalleryContext())
        //    {
        //        ViewBag.UserList = db.User.Where(x => x.Branch.BranchType == BranchType.Branch || x.Branch.BranchType == BranchType.Other).ToList();

        //    }
        //    ViewBag.Id = id;
        //    return View();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public ActionResult List()
        //{

        //    return View();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Save(TicketViewModel model)
        {
            Response response;

            var currentUser = GetAuthenticatedUser();
            try
            {
                if (model.toUserId == currentUser.Id)
                {
                    response = new Response()
                    {
                        status = 500,
                        message = "شما نمیتوانید به این کاربر پیام بدهید;"
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                using (var db = new KiaGalleryContext())
                {
                    if (model.id != null && model.id > 0)
                    {
                        var entity = db.TicketMessage.Where(x => x.Id == model.id).Single();
                        entity.Text = model.text;
                        entity.ModifyUserId = currentUser.Id;
                        entity.ModifyDate = DateTime.Now;
                        entity.Ip = Request.UserHostAddress;
                        db.SaveChanges();
                        response = new Response()
                        {
                            status = 200,
                        };
                    }
                    else
                    {

                        if (model.ticketId != null && model.ticketId > 0)
                        {
                            var ticket = db.Ticket.Single(x => x.Id == model.ticketId);
                            if (currentUser.Id == ticket.ToUserId)
                            {
                                ticket.TicketStatus = TicketStatus.Opened;
                            }
                            var ticketMessage = new TicketMessage()
                            {
                                TicketId = model.ticketId.Value,
                                FileName = model.fileName,
                                Text = model.text,
                                CreateUserId = currentUser.Id,
                                ModifyUserId = currentUser.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                            };
                            var notification = new Notification()
                            {
                                Seen = false,
                                TicketId = model.ticketId.Value,
                                UserId = ticket.ToUserId.Value,
                                CreateUserId = currentUser.Id,
                                CreateDate = DateTime.Now,
                            };
                            //Task.Factory.StartNew(() =>
                            //{
                            //    NikSmsWebServiceClient.SendSmsNik("همکار گرامی شما یک تیکت جدید دارید لطفا جهت پاسخ،به پورتال کیا گالری مراجعه  فرمایید." + "\n" + model.title, ticket.ToUser.PhoneNumber);
                            //});
                            db.Notification.Add(notification);
                            db.TicketMessage.Add(ticketMessage);
                            db.SaveChanges();
                            response = new Response()
                            {
                                status = 200,
                                data = ticketMessage.TicketId
                            };
                        }
                        else
                        {
                            var ticketItem = new Ticket()
                            {
                                Title = model.title,
                                FromUserId = currentUser.Id,
                                DepartmentId = model.departmentId,
                                ToUserId = model.toUserId,
                                TicketStatus = TicketStatus.New,
                                CreateUserId = currentUser.Id,
                                ModifyUserId = currentUser.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                            };
                            var ticketMessage = new TicketMessage()
                            {
                                Ticket = ticketItem,
                                Text = model.text,
                                FileName = model.fileName,
                                CreateUserId = currentUser.Id,
                                ModifyUserId = currentUser.Id,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                            };
                            var notification = new Notification()
                            {
                                Seen = false,
                                Ticket = ticketItem,
                                UserId = ticketItem.ToUserId.Value,
                                CreateUserId = currentUser.Id,
                                CreateDate = DateTime.Now,
                            };
                          
                            db.Notification.Add(notification);
                            db.Ticket.Add(ticketItem);
                            db.TicketMessage.Add(ticketMessage);
                            db.SaveChanges();
                            var ToUserPhoneNumber = db.User.Single(x=>x.Id == model.toUserId).PhoneNumber;
                            Task.Factory.StartNew(() =>
                            {
                                NikSmsWebServiceClient.SendSmsNik("همکار گرامی شما یک تیکت جدید دارید لطفا جهت پاسخ،به پورتال کیا گالری مراجعه  فرمایید." + "\n" + model.title, ToUserPhoneNumber);
                            });

                            response = new Response()
                            {
                                status = 200,
                                message = "تیکت شما با موفقیت ارسال شد."
                            };
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchTicket()
        {
            Response response;
            try
            {
                response = new Response()
                {
                    status = 200,
                    data = new
                    {

                    }
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchMessage(SearchTicketViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    string serverPath = Server.MapPath("~/Upload/ticketFile");
                    string aa = Path.Combine(serverPath, "5403-carrot-apple.png");
                    if (System.IO.File.Exists(aa))
                    {

                    }
                    var query = db.TicketMessage.Where(x => x.TicketId == model.ticketId).Select(x => x);
                    var list = query.Select(x => new TicketViewModel
                    {
                        id = x.Id,
                        title = x.Ticket.Title,
                        fileName = x.FileName,
                        ticketId = x.TicketId,
                        text = x.Text,
                        sender = x.CreateUserId == currentUser.Id,
                        createDate = x.CreateDate,
                        userName = x.CreateUser.FirstName + " " + x.CreateUser.LastName,
                    }).ToList();
                    list.ForEach(x =>
                    {
                        if (!string.IsNullOrEmpty(x.fileName))
                        {
                            x.fileNameList = x.fileName.Split('*').Select(y => new TicketFileViewModel { fileName = y , fileExtension = Path.GetExtension(Path.Combine(serverPath, y)) }).ToArray();
                        }
                        x.createTime = x.createDate.ToString("HH:mm");
                    });
                    response = new Response()
                    {
                        status = 200,
                        data = new
                        {
                            list = list,

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

        public JsonResult Search(SearchTicketViewModel model)
        {
            Response response;
            var currentUser = GetAuthenticatedUser();
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var query = db.Ticket.Where(x => x.FromUserId == currentUser.Id || x.ToUserId == currentUser.Id).Select(x => x);
                    var list = query.Select(x => new TicketViewModel
                    {
                        id = x.Id,
                        title = x.Title,
                        text = x.TicketMessagesList.OrderByDescending(y => y.Id).Select(y => y.Text).FirstOrDefault(),
                        ticketStatus = x.TicketStatus,
                        createDate = x.CreateDate,
                        toUserId = x.ToUserId,
                        fromUserName = x.FromUser.FirstName + " " + x.FromUser.LastName,
                        toUserName = x.ToUser.FirstName + " " + x.ToUser.LastName,
                    }).ToList();
                    var dataCount = query.Count();
                    list.ForEach(x =>
                    {
                        x.ticketStatusTitle = Enums.GetTitle(x.ticketStatus);
                        x.persianDate = DateUtility.GetPersianDateTime(x.createDate);
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
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Ticket.Find(id);

                    db.TicketMessage.RemoveRange(entity.TicketMessagesList);
                    db.Notification.RemoveRange(entity.NotificationList);
                    db.Ticket.Remove(entity);
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "تیکت با موفقیت حذف شد."
                };
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult changeStatus(ChangeTicketStatusViewModel model)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var entity = db.Ticket.Find(model.id);
                    entity.TicketStatus = model.status;
                    db.SaveChanges();
                }
                response = new Response()
                {
                    status = 200,
                    message = "تیکت با موفقیت حذف شد."
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
