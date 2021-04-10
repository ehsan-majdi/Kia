using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiaGallery.DailyReport.Common.Model;

namespace KiaGallery.DailyReport.Common
{
    public static class Cache
    {
        public static User CurrentUser { get; set; }
        public static Model.Calendarlist[] CalendarDays { get; set; }
        public static Model.Banklist [] BankList { get; set; }
        public static Model.Currencylist[] CurrencyList { get; set; }
    }



}
