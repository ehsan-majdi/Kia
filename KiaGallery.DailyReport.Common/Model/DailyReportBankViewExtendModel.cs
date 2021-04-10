using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiaGallery.DailyReport.Common.Model
{
    public class DailyReportBankViewExtendModel : DailyReportBankViewModel, INotifyPropertyChanged
    {
        public DailyReportBankViewExtendModel(DailyReportViewExtendModel parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// مجموع 
        /// </summary>
        public long bankSum { get; set; }

        public DailyReportViewExtendModel Parent { get; set; }


        public override long entry
        {
            get => base.entry;
            set
            {
                base.entry = value; Parent.CalculateSum();
            }
        }
        public override long exit
        {
            get => base.exit;
            set
            {
                base.exit = value; Parent.CalculateSum();
            }
        }

        #region NotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public  void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
