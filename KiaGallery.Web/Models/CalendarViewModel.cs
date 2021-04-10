using System.Collections.Generic;

namespace KiaGallery.Web.Models
{
    public class CalendarViewModel
    {
        public CalendarViewModel()
        {
            branchList = new List<int>();
        }
        public string date { get; set; }
        public List<int> branchList { get; set; }
    }


}