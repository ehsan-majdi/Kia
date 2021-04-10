using KiaGallery.Model;

namespace KiaGallery.Web.Models
{
    public class StoneViewModel
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
        public StoneType stoneType { get; set; }
        public int order { get; set; }
        public string fileName { get; set; }
        public bool active { get; set; }
    }

    public class StoneSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
    }
}