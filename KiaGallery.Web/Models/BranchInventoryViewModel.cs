using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class BranchInventoryViewModel
    {
        public string title { get; set; }
        public string weight { get; set; }
        public string productCode { get; set; }
        public string word { get; set; }
        public string branchName { get; set; }
        public int page { get; set; }
        public int count { get; set; }
        public InventoryType inventoryType { get; set; }
        public string inventoryTypeTitle { get; set; }
        public DateTime date { get; set; }
        public string persianDate { get; set; }
    }
    public class LastUpdateListViewModel
    {
        public string branchName { get; set; }
        public DateTime date { get; set; }
        public string persianDate { get; set; }
    }
}