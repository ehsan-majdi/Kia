using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class InquiryProductReplyViewModel
    {
        public int id { get; set; }
        public int? branchId { get; set; }
        public AnswerType answerType { get; set; }
        public string comment { get; set; }
    }
}