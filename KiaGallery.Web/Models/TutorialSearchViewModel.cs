using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class TutorialSearchViewModel
    {
        public string word { get; set; }
        public TutorialType? tutorialType { get; set; }
        public int count { get; set; }
        public int page { get; set; }
    }
    public class TutorialViewModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string fileName { get; set; }
        public string videoCoverFileName { get; set; }
        public TutorialType tutorialType { get; set; }
        public string tutorialTypeTitle { get; set; }
        public string coverLink { get; set; }
        public bool active { get; set; }
    }
}