using KiaGallery.Model;

namespace KiaGallery.Web.Models
{
    public class ShapeSizeViewModel
    {
        public int? id { get; set; }
        public int order { get; set; }
        public StoneShape stoneShape { get; set; }
        public float sizeLength { get; set; }
        public float sizeWidth { get; set; }
        public bool active { get; set; }
    }

    public class ShapeSizeSearchViewModel
    {
        public StoneShape stoneShape { get; set; }
    }

}