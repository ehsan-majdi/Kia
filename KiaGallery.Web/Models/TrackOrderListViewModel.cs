using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class TrackLoginViewModel
    {
        public string phoneNumber { get; set; }
        public string trackCode { get; set; }
    }

    public class TrackOrderListViewModel
    {
        public string name { get; set; }
        public bool? ponyUp { get; set; }
        public InternalOrderStatus? status { get; set; }
        public string productCode { get; set; }
        public string trackCode { get; set; }
        public DateTime? date { get; set; }
        public string persianDate { get; set; }
        public string description { get; set; }
        public string phoneNumber { get; set; }
        public int deposit { get; set; }
        public string branchName { get; set; }
        public DeliveryType? deliverType { get; set; }
        public string depositToSeparator { get; set; }




    }
}