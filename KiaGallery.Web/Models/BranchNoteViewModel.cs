using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class BranchNoteViewModel
    {
        public int? id { get; set; }
        public string text { get; set; }
        public DateTime? CreateDate { get; set; }
        public string createDate { get; set; }
        public string createUserName { get; set; }
    }
    public class BranchNoteSaveViewModel
    {
        public int? id { get; set; }
        public string text { get; set; }
    }

    public class SearchBranchNoteViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
    }
}