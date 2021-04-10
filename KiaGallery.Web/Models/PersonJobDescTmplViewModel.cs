using System.Web.Mvc;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// مدل اطلاعات شرح وظایف پرسنل
    /// </summary>
    public class PersonJobDescTmplViewModel
    {
        /// <summary>
        /// رذیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// شرح وظایف
        /// </summary>
        [AllowHtml]
        public string text { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool status { get; set; }
    }
}