using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class InternalOrderSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string text { get; set; }
        public List<InternalOrderStatus?> status { get; set; }
        public bool? remind { get; set; }
        public UserType? userType { get; set; }
        public int? id { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public int? productId { get; set; }
        public int? deposit { get; set; }
        public string barcode { get; set; }
        public int? userId { get; set; }
        public string process { get; set; }
        public string title { get; set; }
        public bool notCustomer { get; set; }
        public int? branchId { get; set; }
        public string term { get; set; }
    }
    public class SendToBranchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
    }
    public class SendToOfficeViewModel
    {
        public int? page { get; set; }
        public int? count { get; set; }
        public int? branchId { get; set; }
    }
    public class AcceptFromBranchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string firstName { get; set; }
        public bool notInsurance { get; set; }
        public string lastName { get; set; }
        public int? personNumber { get; set; }
        public int? branchId { get; set; }
    }
    public class EditViewModel
    {
        public ProductType? productType { get; set; }
        public string term { get; set; }
    }

    public class GetDataRecordViewModel
    {
        public string phoneNumber { get; set; }
        public string telephone { get; set; }
        public string name { get; set; }
        public int? gender { get; set; }
        public string userName { get; set; }
        public ProductType productType { get; set; }
        public string productTypeTitle { get; set; }
        public string productTitle { get; set; }
        public string bookCode { get; set; }
        public string siteCode { get; set; }
        public string description { get; set; }
        public string[] stoneIdList { get; set; }
        public string stoneId { get; set; }
        public string leatherId { get; set; }
        public string size { get; set; }
        public GoldType? goldType { get; set; }
        public string goldTypeTitle { get; set; }
        public string fileName { get; set; }
        public string image { get; set; }
    }

    public class ProductInquiryViewModel
    {
        public ProductType productType { get; set; }
        public string productTypeTitle { get; set; }
        public string productTitle { get; set; }
        public string bookCode { get; set; }
        public string siteCode { get; set; }
        public string image { get; set; }
        public GoldType? goldType { get; set; }
        public string goldTypeTitle { get; set; }

    }
}