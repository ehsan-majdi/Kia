using KiaGallery.Model;
using System;

namespace KiaGallery.Web.Models
{
    public class PostViewModel
    {
        public int? id { get; set; }
        public int? provinceId { get; set; }
        public int cityId { get; set; }
        public string invoiceNo { get; set; }
        public int count { get; set; }
        public float weight { get; set; }
        public string submitUser { get; set; }
        public long price { get; set; }
        public string customer { get; set; }
        public bool? sex { get; set; }
        public string address { get; set; }
        public string submitDate { get; set; }
        public string postDate { get; set; }
        public string phoneNumber { get; set; }
        public string mobileNumber { get; set; }
        public string postalCode { get; set; }
    }

    public class PostSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
    }

    public class PostItemSettings
    {
        public string senderAddress { get; set; }
    }
}