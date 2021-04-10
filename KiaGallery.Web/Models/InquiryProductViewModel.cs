using KiaGallery.Model;
using System;

namespace KiaGallery.Web.Models
{
    public class InquiryProductViewModel
    {
        public int id { get; set; }
        public int branchId { get; set; }
        public string branchName { get; set; }
        public string imageLink { get; set; }
        public ProductType productType { get; set; }
        public string productCode { get; set; }
        public string productTitle { get; set; }
        public string comment { get; set; }
        public int commentCount { get; set; }
        public DateTime createdDate { get; set; }
        public string persianCreatedDate { get; set; }
        public string productTypeTitle { get; set; }
    }

    public class AddInquiryProductViewModel
    {
        public int id { get; set; }
        public string comment { get; set; }
        public int productId { get; set; }
        public string fileName { get; set; }
        public string base64 { get; set; }

    }

    public class InquiryProductSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
    }

    public class InquiryProductReplyViewModel
    {
        public int id { get; set; }
        public int branchId { get; set; }
        public string branchName { get; set; }
        public DateTime createdDate { get; set; }
        public string persianCreatedDate { get; set; }
        public int inquiryProductId { get; set; }
        public AnswerType answerType { get; set; }
        public string comment { get; set; }
    }
}