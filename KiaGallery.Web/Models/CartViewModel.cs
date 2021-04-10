using KiaGallery.Model;
using System;
using System.Collections.Generic;

namespace KiaGallery.Web.Models
{
    public class CartViewModel
    {
        public bool edit { get; set; }
        public int? id { get; set; }
        public OrderType orderType { get; set; }
        public int productId { get; set; }
        public int count { get; set; }
        public string size { get; set; }
        public string size2 { get; set; }
        public int? setNumber { get; set; }
        public GoldType? goldType { get; set; }
        public ProductColor? productColor { get; set; }
        public OuterWerkType? outerWerkType { get; set; }
        public byte? leatherLoop { get; set; }
        public string customer { get; set; }
        public string phoneNumber { get; set; }
        public bool? forceOrder { get; set; }
        public DateTime? deliverDateRequest { get; set; }
        public string branchLabel { get; set; }
        public string description { get; set; }
        public List<CartStoneViewModel> stoneList { get; set; }
        public List<CartLeatherViewModel> leatherList { get; set; }
    }
    
    public class MakeOrderViewModel
    {
        public string cartIdList { get; set; }
        public int cartType { get; set; }
        public List<int> workshop { get; set; }
    }

    public class CartStoneViewModel
    {
        public int order { get; set; }
        public int? stoneId { get; set; }
    }

    public class CartLeatherViewModel
    {
        public int order { get; set; }
        public int? leatherId { get; set; }
    }

    public class CartSearchViewModel
    {
        public int? cartType { get; set; }
        public int? workshopId { get; set; }
        public List<int> workshopIdList { get; set; }
        public ProductType? productType { get; set; }
    }
    public class CartListViewModel
    {
        public int id { get; set; }
        public int branchId { get; set; }
        public string fileName { get; set; }
        public OrderType orderType { get; set; }
        public int productId { get; set; }
        public string orderTypeTitle { get; set; }
        public int workshopId { get; set; }
        public int? productSizeId { get; set; }
        public string workshopName { get; set; }
        public string workshopName2 { get; set; }
        public string size { get; set; }
        public string size2 { get; set; }
        public int? setNumber { get; set; }
        public GoldType? goldType { get; set; }
        public string goldTypeTitle { get; set; }
        public OuterWerkType? outerWerkType { get; set; }
        public ProductColor? productColor { get; set; }
        public DateTime? deliverDateRequest { get; set; }
        public string outerWerkTypeTitle { get; set; }
        public byte? leatherLoop { get; set; }
        public string customer { get; set; }
        public string phoneNumber { get; set; }
        public bool? forceOrder { get; set; }
        public string branchLabel { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string code { get; set; }
        public string bookCode { get; set; }
        public float? weight { get; set; }
        public int count { get; set; }
        public string createdUser { get; set; }
        public DateTime createDate { get; set; }
        public string joinedStoneList { get; set; }
        public string joinedLeatherList { get; set; }
        public List<CartStoneListViewModel> stoneList { get; set; }
        public List<CartLeatherListViewModel> leatherList { get; set; }
        //public List<pro> leatherList { get; set; }
    }

    public class CartStoneListViewModel
    {
        public int order { get; set; }
        public string stoneName { get; set; }
    }

    public class CartLeatherListViewModel
    {
        public int order { get; set; }
        public string leatherName { get; set; }
    }
}