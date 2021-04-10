namespace KiaGallery.Web.Models
{
    public class BankViewModel
    {
        public int? id { get; set; }
        public int order { get; set; }
        public int branchId { get; set; }
        public string name { get; set; }
        public string branchName { get; set; }
        public bool active { get; set; }
    }
}