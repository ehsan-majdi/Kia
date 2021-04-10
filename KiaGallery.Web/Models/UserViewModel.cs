using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KiaGallery.Web.Models
{
    public class UserViewModel
    {
        public int? id { get; set; }
        public int? branchId { get; set; }
        public int? workshopId { get; set; }
        public int? printingHouseId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fileName { get; set; }
        public string phoneNumber { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
        public string confirmationCode { get; set; }
        public UserType userType { get; set; }
        public bool active { get; set; }
        public List<string> roleList { get; set; }
    }

    public class UserListViewModel
    {
        public int? id { get; set; }
        public string branchName { get; set; }
        public string workshopName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public bool active { get; set; }
    }

    public class LoginViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class AccountSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
        public int? branchId { get; set; }
        public int? workshopId { get; set; }
        public bool? active { get; set; }
        public bool? deactive { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public int? userId { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmNewPassword { get; set; }
        public string confirmationCode { get; set; }
    }
    public class GetAuthentiatedUserViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// ردیف کارگاه
        /// </summary>
        public int? WorkshopId { get; set; }
        /// <summary>
        /// ردیف چاپخانه
        /// </summary>
        public int? PrintingHouseId { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
       
        public string LastName { get; set; }
        /// <summary>
        /// رنگ
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// نوع شعب
        /// </summary>
        public BranchType BranchType { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string BranchName { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// کلید ساخت گذرواژه
        /// </summary>
        public string Salt { get; set; }
        /// <summary>
        /// گذرواژه
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// نوع کاربر
        /// </summary>
        public UserType UserType { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int? CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int? ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        /// <summary>
        /// محل کاربر
        /// </summary>
        public string UserPlace { get; set; }

    }

    public class UserMethods
    {
        public static User ViewModelToModel(UserViewModel model, User item, bool withPassword = false)
        {
            if (item == null)
                item = new User();

            if (model.id != null && model.id > 0) item.Id = model.id.GetValueOrDefault();

            item.BranchId = model.branchId;
            item.WorkshopId = model.workshopId;
            item.PrintingHouseId = model.printingHouseId;
            item.FirstName = model.firstName;
            item.LastName = model.lastName;
            item.FileName = model.fileName;
            item.PhoneNumber = model.phoneNumber;
            item.Username = model.username;
            item.UserType = model.userType;
            item.Active = model.active;

            if (withPassword)
            {
                var password = PasswordTools.GetHashedPassword(model.password);
                item.Salt = password.Item1;
                item.Password = password.Item2;
            }

            if (model.roleList != null && model.roleList.Count > 0)
            {
                item.RoleList = model.roleList?.Select(x => new Role()
                {
                    User = item,
                    Title = x
                }).ToList();
            }

            return item;
        }

        public static UserViewModel ModelToViewModel(User model)
        {
            var item = new UserViewModel()
            {
                id = model.Id,
                branchId = model.BranchId,
                workshopId = model.WorkshopId,
                printingHouseId = model.PrintingHouseId,
                firstName = model.FirstName,
                lastName = model.LastName,
                fileName = model.FileName,
                phoneNumber = model.PhoneNumber,
                username = model.Username,
                userType = model.UserType,
                active = model.Active,

                roleList = model.RoleList?.Select(x => x.Title).ToList()
            };
            return item;
        }

    }

}