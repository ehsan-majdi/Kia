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
    
    public partial class DailyReportMessage
    {
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<long> ChatId { get; set; }
        public Nullable<int> MessageId { get; set; }
        public string Text { get; set; }
        public System.DateTime CreateDate { get; set; }
        public bool Unknown { get; set; }
    }
}
