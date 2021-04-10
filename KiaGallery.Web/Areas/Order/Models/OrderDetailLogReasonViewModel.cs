using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Areas.Order.Models
{
    public class OrderDetailLogReasonViewModel
    {
        public string token { get; set; }
        public int? id { get; set; }
        public OrderDetailStatus orderDetailStatus { get; set; }
        public string text { get; set; }
        public bool active { get; set; }
    }
}