//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KiaGallery.ScheduleJob
{
    using System;
    using System.Collections.Generic;
    
    public partial class BranchGolds
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public double Weight { get; set; }
        public long Price { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public int CreateUserId { get; set; }
        public int ModifyUserId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public string Ip { get; set; }
    
        public virtual Branches Branches { get; set; }
    }
}
