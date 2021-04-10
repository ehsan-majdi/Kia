using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class SearchOperationViewModel
    {
        public int? orderType { get; set; }
        public OrderDetailStatus? status { get; set; }
        public BranchType? branchType { get; set; }
        public List<ProductType> productTypeList { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public List<int> workshopList { get; set; }
        public List<int> branchList  { get; set; }

    }
}