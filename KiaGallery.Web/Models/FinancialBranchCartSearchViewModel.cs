using KiaGallery.Model;
using System.Collections.Generic;

namespace KiaGallery.Web.Models
{
    public class FinancialBranchCartSearchViewModel
    {
        public int branchId { get; set; }
        public int? cartType { get; set; }
        public int? workshopId { get; set; }
        public List<int> workshopIdList { get; set; }
        public ProductType? productType { get; set; }
    }
}