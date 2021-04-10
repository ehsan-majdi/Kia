using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class PhotographyManageViewModel
    {
        public int productId { get; set; }
        public int photography { get; set; }
        public bool status { get; set; }
        public string whiteBack { get; set; }
        public string modelImage { get; set; }
        public string siteImage { get; set; }

    }
}