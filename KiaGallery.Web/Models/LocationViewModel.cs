using KiaGallery.Model;

namespace KiaGallery.Web.Models
{
    public class LocationViewModel
    {
        public int? id { get; set; }
        public int? parentId { get; set; }
        public LocationType locationType { get; set; }
        public int order { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
    }

    public class LocationSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
    }
}