using KiaGallery.Model;

namespace KiaGallery.Web.Models
{
    public class OrderDetailLogReasonViewModel
    {
        public int? id { get; set; }
        public OrderDetailStatus orderDetailStatus { get; set; }
        public string text { get; set; }
        public bool active { get; set; }
    }
}