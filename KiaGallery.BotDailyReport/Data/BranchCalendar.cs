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
    
    public partial class BranchCalendar
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BranchCalendar()
        {
            this.DailyReport = new HashSet<DailyReport>();
        }
    
        public int Id { get; set; }
        public int BranchId { get; set; }
        public System.DateTime ReportDate { get; set; }
        public int CreateUserId { get; set; }
        public int ModifyUserId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public string Ip { get; set; }
    
        public virtual Branches Branches { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DailyReport> DailyReport { get; set; }
    }
}
