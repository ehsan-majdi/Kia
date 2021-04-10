using KiaGallery.Web.Areas.DailyReportFinancial.Models;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.DailyReport.Common.Model
{
    public class DailyReportCurrencyViewExtendModel : DailyReportCurrencyViewModel, INotifyPropertyChanged
    {

        public DailyReportCurrencyViewExtendModel(DailyReportViewExtendModel parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// مجموع 
        /// </summary>
        public long CurrencySum { get; set; }

        public DailyReportViewExtendModel Parent { get; set; }


        public override long rialEntry { get => base.rialEntry; set { base.rialEntry = value; Parent.CalculateSum(); } }
        public override long rialExit { get => base.rialExit; set { base.rialExit = value; Parent.CalculateSum(); } }


        #region NotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
