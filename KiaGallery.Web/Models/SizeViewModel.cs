namespace KiaGallery.Web.Models
{
    public class SizeViewModel
    {
        public int? id { get; set; }
        public string title { get; set; }
        public string defaultValue { get; set; }
    }

    public class SizeValueViewModel
    {
        public int? id { get; set; }
        public int sizeId { get; set; }
        public int order { get; set; }
        public string value { get; set; }
    }

    public class SizeSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
    }
}