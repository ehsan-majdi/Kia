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
    
    public partial class AppToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; }
        public bool Voided { get; set; }
        public int TokenType { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> VoidedDate { get; set; }
    }
}