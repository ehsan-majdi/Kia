using KiaGallery.Model;
using System;

namespace KiaGallery.Web.Models
{
    public class GiftLogViewModel
    {
        public int id { get; set; }
        public DateTime createdDateTime { get; set; }
        public string createdDate { get; set; }
        public GiftStatus giftStatus { get; set; }
        public string giftTypeTitle { get; set; }
        public GiftType giftType { get; set; }
        public string status { get; set; }
        public string createUser { get; set; }
        public string branch { get; set; }
        public string branchReceiverCustomer { get; set; }
        public string factorNumber { get; set; }
        public long factorPrice { get; set; }
        public string customerName { get; set; }
        public string customerPhoneNumber { get; set; }
    }
}