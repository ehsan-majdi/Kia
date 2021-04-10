namespace KiaGallery.Web.Areas.Api.Models
{
    public class UserDetailViewModel
    {
        public int id { get; set; }
        public string token { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string fileName { get; set; }
        public string branchName { get; set; }
        public string imageLink { get; set; }
    }
}