using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.DailyReport.Common.Model
{
    public class Invoice
    {
        public Banklist[] bankList { get; set; }
        public Currencylist[] currencyList { get; set; }
        public Calendarlist[] calendarList { get; set; }
    }

    public class Banklist
    {
        public int id { get; set; }
        public int order { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class Currencylist
    {
        public int id { get; set; }
        public int order { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class Calendarlist : INotifyPropertyChanged
    {
        public int id { get; set; }

        private string _date;
        public string date
        {
            get { return _date; }
            set
            {
                _date = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Day"));
                    PropertyChanged(this, new PropertyChangedEventArgs("date"));
                }
            }
        }
        public bool canEdit { get; set; }
        public int status { get; set; }

        public int Day
        {
            get
            {
                return int.Parse(date.Split('/')[2]);
            }

        }
        public int Mount { get { return byte.Parse(date.Split('/')[1]); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return date;
        }

        public Common.Model.DailyReportViewExtendModel ReportData { get; set; }
    }

    public class CalendarColor
    {
        public ColorName Name { get; set; }
        public string NameString { get; set; }
        public string Color { get; set; }

        public enum ColorName
        {
            NoDate,
            Readonly,
            Editable,
        }
    }
}