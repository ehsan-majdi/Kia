using KiaGallery.Model;

namespace KiaGallery.Web.Models
{
    public class WorkshopViewModel
    {
        public int? id { get; set; }
        public int order { get; set; }
        public string alias { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public bool autoConfirm { get; set; }
        public bool active { get; set; }
        public bool? goldTrade { get; set; }
    }

    public class WorkshopSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
    }

    public class WorkshopDropDwonViewModel
    {
        public int id { get; set; }
        public string val { get; set; }
    }
    public class WorkShopGoldEditViewModel
    {
        public int workShopId { get; set; }
        public double? weight { get; set; }
    }
}