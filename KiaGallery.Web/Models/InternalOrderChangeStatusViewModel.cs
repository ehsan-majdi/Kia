using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class InternalOrderChangeStatusViewModel
    {
        public List<int> id { get; set; }
        public InternalOrderStatus status { get; set; }
        public int orderId { get; set; }
        public DateTime createDate { get; set; }
        public string persianDate { get; set; }
        public string customerName { get; set; }
        /// <summary>
        /// ردیف شعبه خریدار گیفت
        /// </summary>
        public int? deliveredBranchId { get; set; }
        public string deliveredBranchName { get; set; }
        public string branchName { get; set; }
        public string userName { get; set; }
        public string barcode { get; set; }
        public string phoneNumber { get; set; }
    }
}