using KiaGallery.Model;

namespace KiaGallery.Web.Models
{
    public class LeatherViewModel
    {
        public int? id { get; set; }
        public string name { get; set; }
        public LeatherType leatherType { get; set; }
        public int order { get; set; }
        public string fileName { get; set; }
        public bool active { get; set; }
    }

    public class LeatherSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
        public bool? active { get; set; }
        public LeatherType? leatherType { get; set; }
    }
}