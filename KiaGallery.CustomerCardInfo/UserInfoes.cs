//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KiaGallery.CustomerCardInfo
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserInfoes
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public Nullable<bool> Sex { get; set; }
        public string Email { get; set; }
        public string NationalCode { get; set; }
        public string Address { get; set; }
        public string Area { get; set; }
        public string CarriorCode { get; set; }
        public string CardNumber { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public double DiscountPercent { get; set; }
        public double PointPercent { get; set; }
        public Nullable<double> Point { get; set; }
        public long KiaPoint { get; set; }
        public string Province { get; set; }
        public Nullable<System.DateTime> MarriedDate { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public int CreateUserId { get; set; }
        public int ModifyUserId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public string CreateIp { get; set; }
        public string ModifyIp { get; set; }
    
        public virtual UserProfile UserProfile { get; set; }
        public virtual UserProfile UserProfile1 { get; set; }
    }
}
