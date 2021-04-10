using KiaGallery.Model;
using System;
using System.Collections.Generic;

namespace KiaGallery.Web.Areas.Order.Models
{
    public class ProductSearchViewModel
    {
        public string token { get; set; }
        public int page { get; set; }
        public int count { get; set; }
        public bool fav { get; set; }
        public int? workshopId { get; set; }
        public string term { get; set; }
        public bool? leatherBracelet { get; set; }
        public bool? isNew { get; set; }
        public bool? active { get; set; }
        public List<int> type { get; set; }
        public List<int> sex { get; set; }
        public List<int> outerWerkType { get; set; }
        public bool? related { get; set; }
        public bool? set { get; set; }
    }

    public class ResponseProductSearchViewModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public string bookCode { get; set; }
        public string fileName { get; set; }
        public int paddingImg { get; set; }
        public ProductType productType { get; set; }
        public string productTypeTitle { get; set; }
        public string title { get; set; }
        public float? weight { get; set; }
        public bool active { get; set; }
        public int productNew { get; set; }
        public DateTime modifyDate { get; set; }
        public bool isNew { get; set; }
        public bool isFav { get; set; }
        public int related { get; set; }
        public int set { get; set; }
    }
}