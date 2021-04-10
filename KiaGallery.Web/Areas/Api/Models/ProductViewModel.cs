using KiaGallery.Model;
using System;

namespace KiaGallery.Web.Areas.Api.Models
{
    public class ProductViewModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public string bookCode { get; set; }
        public string fileName { get; set; }
        public ProductType productType { get; set; }
        public string productTypeTitle { get; set; }
        public string title { get; set; }
        public float? weight { get; set; }
        public int productNew { get; set; }
        public bool isNew { get; set; }
        public bool isFav { get; set; }
        public DateTime modifyDate { get; set; }
    }

    public class ProductSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public ProductType? productType { get; set; }
        public int? workShopId { get; set; }
        public string word { get; set; }

    }
}