using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// دفترچه تلفن
    /// </summary>
    public class PhoneBookViewModel
    {
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string branchName { get; set; }
        /// <summary>
        /// نام پرسنل شعبه
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی پرسنل شعبه
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// تلفن شعبه
        /// </summary>
        public string branchPhone { get; set; }
        /// <summary>
        /// نوع شعبه
        /// </summary>
        public BranchType branchType { get; set; }
        /// <summary>
        /// تلفن پرسنل
        /// </summary>
        public string personPhone { get; set; }

    }
}