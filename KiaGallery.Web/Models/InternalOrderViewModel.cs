using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class InternalOrderViewModel
    {
        public int? id { get; set; }
        public int? createPersonId { get; set; }
        public int? count { get; set; }
        public int? newCount { get; set; }
        public string date { get; set; }
        public string persianDate { get; set; }
        public string barcodeDate { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public int deposit { get; set; }
        public int detailCount { get; set; }
        public string barcode { get; set; }
        public UserType userType { get; set; }
        public string description { get; set; }
        public OrderType orderType { get; set; }
        public OrderTypeForm orderTypeForm { get; set; }
        public InternalOrderStatus? roleBackStatus { get; set; }
        public Gender gender { get; set; }
        public string trackCode { get; set; }
        public string telephone { get; set; }
        public List<AddProductViewModel> addProductViewModelList { get; set; }

    }
    public class SentViewModel
    {
        public int orderId { get; set; }
        public bool ponyUp { get; set; }
        public DeliveryType deliveryType { get; set; }
        public int? deliveredBranchId { get; set; }
    }
    public class AddProductViewModel
    {
        //public int? id { get; set; }
        public int? detailId { get; set; }
        public int? count { get; set; }
        public int? newCount { get; set; }
        public ProductType productType { get; set; }
        public ProductType? newProductType { get; set; }
        public GoldType goldType { get; set; }
        public ProductColor productColor { get; set; }
        public OrderType orderType { get; set; }
        public string stoneName { get; set; }
        public string barcodeDate { get; set; }
        public string barcode { get; set; }
        public int? leatherLoop { get; set; }
        public string size { get; set; }
        public string image { get; set; }
        public string siteCode { get; set; }
        public int deposit { get; set; }
        public string bookCode { get; set; }
        public string trackCode { get; set; }
        public int? productId { get; set; }
        public string fileName { get; set; }
        public string title { get; set; }
        public string productTitle { get; set; }
        public string newProductTitle { get; set; }
        public int? leatherCount { get; set; }
        public int? stoneCount { get; set; }
        public string description { get; set; }
        public string newStone { get; set; }
        public string newLeather { get; set; }
        public string newSize { get; set; }
        public string newLeatherLoop { get; set; }
        public bool? goldOwnership { get; set; }
        public ProductColor? newProductColor { get; set; }
        public GoldType? newGoldType { get; set; }
        public string newDescription { get; set; }
        public List<InternalOrderStoneDetailViewModel> internalOrderDetailStoneViewModelList { get; set; }
        public List<InternalOrderLeatherDetailViewModel> internalOrderDetailLeatherViewModelList { get; set; }
    }
    //public class InternalOrderDetailStoneViewModel
    //{
    //    /// <summary>
    //    /// ردیف
    //    /// </summary>
    //    public int id { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public int stoneId { get; set; }
    //}

    public class InternalOrderStoneDetailViewModel
    {
        public int internalOrderDetailId { get; set; }
        public int stoneId { get; set; }
        public int order { get; set; }
    }
    public class InternalOrderLeatherDetailViewModel
    {
        public int internalOrderDetailId { get; set; }
        public int leatherId { get; set; }
        public int order { get; set; }
    }
}