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
    
    public partial class UserProfile
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserProfile()
        {
            this.UserInfoes = new HashSet<UserInfoes>();
            this.UserInfoes1 = new HashSet<UserInfoes>();
            this.UserProfile1 = new HashSet<UserProfile>();
            this.UserProfile11 = new HashSet<UserProfile>();
        }
    
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> WorkshopId { get; set; }
        public Nullable<int> PrintingHouseId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Color { get; set; }
        public string FileName { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public bool Active { get; set; }
        public string ConfirmationCode { get; set; }
        public Nullable<int> CreateUserId { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public string Ip { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserInfoes> UserInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserInfoes> UserInfoes1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserProfile> UserProfile1 { get; set; }
        public virtual UserProfile UserProfile2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserProfile> UserProfile11 { get; set; }
        public virtual UserProfile UserProfile3 { get; set; }
    }
}
