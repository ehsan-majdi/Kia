using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class GiftReportViewModel
    {
        public List<GiftReportDetailViewModel> data { get; set; }
        public string branchName { get; set; }
     
    }
    public class GiftReportDetailViewModel
    {
        public int count { get; set; }
        public GiftStatus status { get; set; }
        public string statusTitle { get; set; }
    }

    public class GiftReportSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public int? branchId { get; set; }
    }
}