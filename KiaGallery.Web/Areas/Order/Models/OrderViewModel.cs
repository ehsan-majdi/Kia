using KiaGallery.Model;
using System;
using System.Collections.Generic;

namespace KiaGallery.Web.Areas.Order.Models
{
    public class OrderViewModel
    {
        public string token { get; set; }
        public int id { get; set; }
        public string orderSerial { get; set; }
        public int? sumCount { get; set; }
        public int? sumCountSet { get; set; }
        public double? sumWeight { get; set; }
        public float? sumWeightFloat { get; set; }
        public int? registered { get; set; }
        public double? registeredWeight { get; set; }
        public int? inWorkshop { get; set; }
        public double? inWorkshopWeight { get; set; }
        public int? underConstruction { get; set; }
        public double? underConstructionWeight { get; set; }
        public int? outOfConstruction { get; set; }
        public double? outOfConstructionWeight { get; set; }
        public int? inPreparation { get; set; }
        public double? inPreparationWeight { get; set; }
        public int? readyForDelivery { get; set; }
        public double? readyForDeliveryWeight { get; set; }
        public int? sent { get; set; }
        public double? sentWeight { get; set; }
        public int? shortage { get; set; }
        public double? shortageWeight { get; set; }
        public int? shortageOrder { get; set; }
        public double? shortageOrderWeight { get; set; }
        public int? cancel { get; set; }
        public double? cancelWeight { get; set; }
        public string bgColor { get; set; }
        public string createUser { get; set; }
        public string createBranch { get; set; }
        public string createDate { get; set; }
    }
    public class OrderViewModelFloat
    {
        public string token { get; set; }
        public int id { get; set; }
        public string orderSerial { get; set; }
        public int? sumCount { get; set; }
        public int? sumCountSet { get; set; }
        public float? sumWeight { get; set; }
        public int? registered { get; set; }
        public float? registeredWeight { get; set; }
        public int? inWorkshop { get; set; }
        public float? inWorkshopWeight { get; set; }
        public int? underConstruction { get; set; }
        public float? underConstructionWeight { get; set; }
        public int? outOfConstruction { get; set; }
        public float? outOfConstructionWeight { get; set; }
        public int? inPreparation { get; set; }
        public float? inPreparationWeight { get; set; }
        public int? readyForDelivery { get; set; }
        public float? readyForDeliveryWeight { get; set; }
        public int? sent { get; set; }
        public float? sentWeight { get; set; }
        public int? shortage { get; set; }
        public float? shortageWeight { get; set; }
        public int? shortageOrder { get; set; }
        public float? shortageOrderWeight { get; set; }
        public int? cancel { get; set; }
        public float? cancelWeight { get; set; }
        public string bgColor { get; set; }
        public string createUser { get; set; }
        public string createBranch { get; set; }
        public string createDate { get; set; }
        public DateTime createDateTime { get; set; }
    }

    public class OrderDetailViewModel
    {
        public string token { get; set; }
        public int? index { get; set; }
        public int id { get; set; }
        public string orderSerial { get; set; }
        public string fileName { get; set; }
        public OrderType orderType { get; set; }
        public string orderTypeTitle { get; set; }
        public int productId { get; set; }
        public int? productSizeId { get; set; }
        public string workshopName { get; set; }
        public string size { get; set; }
        public int? setNumber { get; set; }
        public GoldType? goldType { get; set; }
        public string goldTypeTitle { get; set; }
        public OuterWerkType? outerWerkType { get; set; }
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
        public OrderDetailStatus orderDetailStatus { get; set; }
        public string orderDetailStatusTitle { get; set; }
        public string createdUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string createDate { get; set; }
        public string relatedOrderDetailSerial { get; set; }
        public int? countUnderConstruction { get; set; }
        public List<OrderDetailStoneViewModel> stoneList { get; set; }
        public List<OrderDetailLeatherViewModel> leatherList { get; set; }
        public int? returned { get; set; }

    }
    public class OrderDetailStoneViewModel
    {
        public string token { get; set; }
        public int order { get; set; }
        public string stoneName { get; set; }
    }
    public class OrderDetailLeatherViewModel
    {
        public string token { get; set; }
        public int order { get; set; }
        public string leatherName { get; set; }
    }

    public class OrderSearchViewModel
    {
        public string token { get; set; }
        public int? orderId { get; set; }
        public int page { get; set; }
        public int count { get; set; }
        public List<OrderDetailStatus> statusList { get; set; }
        public string orderSerial { get; set; }
        public string term { get; set; }
        public int? workshopId { get; set; }
        public List<int> branchId { get; set; }
        public List<ProductType> typeList { get; set; }
        public string orderBy { get; set; }
        public string date { get; set; }
        public bool? archive { get; set; }
    }

    public class OrderDetailSearchViewModel
    {
        public string token { get; set; }
        public int page { get; set; }
        public int count { get; set; }
        public int? orderId { get; set; }
        public int? workshopOrderId { get; set; }
        public string term { get; set; }
        public int? setNumber { get; set; }
        public List<ProductType> typeList { get; set; }
        public OrderDetailStatus? status { get; set; }
        public List<int> workshopList { get; set; }
        public List<int> branchList { get; set; }
        public string order { get; set; }
        public string date { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }

    public class OrderDetailChangeStatusViewModel
    {
        public string token { get; set; }
        public int? orderId { get; set; }
        public List<int> id { get; set; }
        public OrderDetailStatus status { get; set; }
        public int? reasonId { get; set; }
        public string description { get; set; }
    }

    public class OrderDateViewModel
    {
        public string token { get; set; }
        public string Date { get; set; }
        public int Count { get; set; }
    }

    public class OrderPrintViewModel
    {
        public string token { get; set; }
        public int? orderType { get; set; }
        public List<int> workshopList { get; set; }
        public List<OrderDetailStatus> statusList { get; set; }
    }

    public class OrderPrintDataViewModel
    {
        public string token { get; set; }
        public byte[] Image { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Workshop { get; set; }
        public string Weight { get; set; }
        public string GoldType { get; set; }
        public string OuterWerkType { get; set; }
        public string OuterWerkTypeTitle { get; set; }
        public string Count { get; set; }
        public string Size { get; set; }
        public string Stone { get; set; }
        public string Leather { get; set; }
        public string Customer { get; set; }
        public string Description { get; set; }
        public string SetNumber { get; set; }
    }

    public class LogDetailViewModel
    {
        public string token { get; set; }
        public int id { get; set; }
        public OrderDetailStatus status { get; set; }
        public string statusText { get; set; }
        public string reasonText { get; set; }
        public string description { get; set; }
        public string createdDate { get; set; }
        public DateTime createdDateTime { get; set; }
        public string createUser { get; set; }
    }
}