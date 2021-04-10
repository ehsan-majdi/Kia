using KiaGallery.Model;
using System.Collections.Generic;

namespace KiaGallery.Web.Areas.DailyReportFinancial.Models
{
    public class BaseInformationViewModel
    {
        public List<BankViewModel> bankList { get; set; }
        public List<CurrencyViewModel> currencyList { get; set; }
        public List<CalendarViewModel> calendarList { get; set; }
    }

    public class BankViewModel
    {
        public int id { get; set; }
        public int order { get; set; }
        public string name { get; set; }
    }

    public class CurrencyViewModel
    {
        public int id { get; set; }
        public int order { get; set; }
        public string name { get; set; }
    }

    public class CalendarViewModel
    {
        public int id { get; set; }
        public string date { get; set; }
        public bool canEdit { get; set; }
        public CalendarStatus status { get; set; }
    }



}