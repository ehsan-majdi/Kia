using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class TicketViewModel
    {
        public int? id { get; set; }
        public int? ticketId { get; set; }
        public string title { get; set; }
        public int? departmentId { get; set; }
        public string text { get; set; }
        public int? toUserId { get; set; }
        public TicketStatus ticketStatus { get; set; }
        public string ticketStatusTitle { get; set; }
        public string color { get; set; }
        public DateTime createDate { get; set; }
        public string createTime { get; set; }
        public string userName { get; set; }
        public string position { get; set; }
        public string persianDate { get; set; }
        public string toUserName { get; set; }
        public string fromUserName { get; set; }
        public string fileName { get; set; }
        public TicketFileViewModel[] fileNameList { get; set; }
        public string fileExtension { get; set; }
        public string link { get; set; }
        public bool sender { get; set; }
    }
    public class ChangeTicketStatusViewModel
    {
        public int id { get; set; }
        public TicketStatus status { get; set; }
    }
    public class TicketFileViewModel
    {
        public string fileName { get; set; }
        public string fileExtension { get; set; }
    }
    public class SearchTicketViewModel
    {
        public int ticketId { get; set; }
        public int toUserId { get; set; }
        public int page { get; set; }
        public int count { get; set; }


    }
}