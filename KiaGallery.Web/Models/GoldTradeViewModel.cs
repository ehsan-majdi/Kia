using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class BranchGoldViewModel
    {
        public double weight { get; set; }
        public string date { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int second { get; set; }
        public long price { get; set; }
    }
    public class BranchGoldSearchViewModel
    {
        public double weight { get; set; }
        public long price { get; set; }
        public string stringPrice { get; set; }
        public int id { get; set; }
        public DateTime? date { get; set; }
        public string stringDate { get; set; }
        public int count { get; set; }
        public double lastWeight { get; set; }

    }
    public class GoldBalanceViewModel
    {
        public string tradeTime { get; set; }
        public string dealerName { get; set; }
        public double weight { get; set; }
        public string description { get; set; }
        public TradeType tradeType { get; set; }
        public string date { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int second { get; set; }
    }
    public class GoldBalanceSearchViewModel
    {
        public int id { get; set; }
        public string tradeTime { get; set; }
        public string dealerName { get; set; }
        public double weight { get; set; }
        public string description { get; set; }
        public TradeType tradeType { get; set; }
        public string tradeTypeTitle { get; set; }
        public DateTime? date { get; set; }
        public string stringDate { get; set; }

    }
    public class GetBranchGoldViewModel
    {
        public string branchName { get; set; }
        public double weight { get; set; }
        public long price { get; set; }
        public DateTime? date { get; set; }
        public string stringDate { get; set; }
        public List<DetailSearchViewModel> detail { get; set; }
    }

    public class DetailSearchViewModel
    {
        public double? weight { get; set; }
        public string stringDate { get; set; }
        public DateTime? date { get; set; }

    }
    public class WorkShopGoldViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int workshopId { get; set; }
        public double weight { get; set; }
        public double? workShopBoughtGoldPrice { get; set; }
        public double? workShopGoldRate { get; set; }
        public string date { get; set; }
        public RemittanceType remittanceType { get; set; }

    }
    public class WorkShopDetailViewModel
    {
        public double? weight { get; set; }
        public double? goldRate { get; set; }
        public double? boughtGoldPrice { get; set; }
        public DateTime? date { get; set; }
        public string stringDate { get; set; }
        public string name { get; set; }
        
    }

}