

namespace KiaGallery.Web.Areas.Bot.Models
{
    public class MessageView
    {
        public int Id { get; set; }
        public long? ChatId { get; set; }
        public int? MessageId { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public bool Unknown { get; set; }
        public int? ReplayId { get; set; }
        public string ReplayText { get; set; }
        public string ReplayDate { get; set; }
    }
}