//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KiaGallery.BotDailyReport.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class BotDailyReportUserData
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public int UserId { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public string BranchId { get; set; }
        public long ChatId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public bool Stoped { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int BotUserType { get; set; }
    }
}
