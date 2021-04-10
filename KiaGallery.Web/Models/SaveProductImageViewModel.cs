using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class SaveProductImageViewModel
    {
        public int productId { get; set; }
        public string fileName { get; set; }
        public FileType fileType { get; set; }
    }
}