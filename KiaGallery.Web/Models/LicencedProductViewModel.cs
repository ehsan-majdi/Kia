using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class LicencedProductViewModel
    {
        public int? id { get; set; }
        public int? productId { get; set; }
        public string productName { get; set; }
        public byte[] image { get; set; }
        public float weight { get; set; }
        public string color { get; set; }
        public string purity { get; set; }
        public string fileName { get; set; }
        public string code { get; set; }
        public string barcode { get; set; }
        public long wage { get; set; }
        public long? euro { get; set; }
        public long? leatherStonePrice { get; set; }
        public double price { get; set; }
        public long rialPrice { get; set; }
        public string stringPrice { get; set; }
        public string stringRialPrice { get; set; }
        public double euroPrice { get; set; }

    }
    public class LicencedProductSearchViewModel
    {
        public int count { get; set; }
        public int page { get; set; }
    }
}