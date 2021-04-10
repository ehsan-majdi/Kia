using KiaGallery.Model;
using System;
using System.Collections.Generic;

namespace KiaGallery.Web.Models
{
    public class UserDataViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// ردیف شعب
        /// </summary>
        public int? branchId { get; set; }
        /// <summary>
        /// نام شعب
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// متوقف شده 
        /// </summary>
        public bool stoped { get; set; }
    }

    public class BotUserListViewModel
    {
        public int id { get; set; }
        public int userType { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public BotDailyReportUserType botUserType { get; set; }
        public string botUserTypeTitle { get; set; }
        public List<BotUserListBranchViewModel> branchList { get; set; }
        public string branchName { get; set; }
        public string branchId { get; set; }
    }

    public class BotUserListBranchViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class SearchBotUserListViewModel
    {
        public string userName { get; set; }
        public int? userType { get; set; }
        public BotDailyReportUserType? botUserType { get; set; }
        public int page { get; set; }
        public int count { get; set; }
    }
    public class UserDataSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
    }
    public class SaveBotUserTypeViewModel
    {
        public int id { get; set; }
        public int? branchId { get; set; }
        public BotDailyReportUserType botUserType { get; set; }
        public int userType { get; set; }


    }
}