using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class ProductCollectionViewModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string fileName { get; set; }
        public bool active { get; set; }
    }
}