using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiaGallery.Web.Areas.DailyReportFinancial.Models;

namespace KiaGallery.DailyReport.Common.Model
{
    public class DailyReportViewExtendModel : Web.Areas.DailyReportFinancial.Models.DailyReportViewModel, INotifyPropertyChanged
    {

        #region SumFilds
        /// <summary>
        /// مجموع فروش
        /// </summary>
        public long saleSum { get; set; }

        /// <summary>
        /// مجموع مرجوعی
        /// </summary>
        public long returnedSum { get; set; }

        /// <summary>
        /// مجموع اسکناس
        /// </summary>
        public long cashSum { get; set; }

        /// <summary>
        /// مجموع گیفت
        /// </summary>
        public long giftSum { get; set; }

        /// <summary>
        /// مجموع بن خرید
        /// </summary>
        public long checkSum { get; set; }

        /// <summary>
        /// مجموع چرم و سنگ
        /// </summary>
        public long leatherStoneSum { get; set; }

        /// <summary>
        /// مجموع سکه
        /// </summary>
        public long coinSum { get; set; }

        /// <summary>
        /// مجموع متفرقه کیا
        /// </summary>
        public long otherKiaGoldSum { get; set; }

        /// <summary>
        /// مجموع ظلای متفرقه
        /// </summary>
        public long otherGoldSum { get; set; }

        /// <summary>
        /// مجموع مانده مشتری بستانکار
        /// </summary>
        public long creditorCustomerSum { get; set; }

        /// <summary>
        /// مجموع مانده مشتری بدهکار
        /// </summary>
        public long debtorCustomerSum { get; set; }

        /// <summary>
        /// مجموع بیعانه از قبل
        /// </summary>
        public long depositBeforeSum { get; set; }

        /// <summary>
        /// مجموع بیعانه جدید
        /// </summary>
        public long depositNewSum { get; set; }

        /// <summary>
        /// مجموع تخفیف
        /// </summary>
        public long discountSum { get; set; }

        /// <summary>
        /// مجموع هزینه پست و پیک
        /// </summary>
        public long costCourierPostSum { get; set; }

        /// <summary>
        /// مجموع هزینه 
        /// </summary>
        public long costSum { get; set; }
        #endregion

        #region OverrideFilds

        public override long saleEntry { get => base.saleEntry; set { base.saleEntry = value; CalculateSum(); } }

        public override long saleExit { get => base.saleExit; set { base.saleExit = value; CalculateSum(); } }

        public override long cashEntry { get => base.cashEntry; set { base.cashEntry = value; CalculateSum(); } }
        public override long cashExit { get => base.cashExit; set { base.cashExit = value; CalculateSum(); } }

        public override long coinEntry { get => base.coinEntry; set { base.coinEntry = value; CalculateSum(); } }
        public override long coinExit { get => base.coinExit; set { base.coinExit = value; CalculateSum(); } }

        public override long costCourierPostEntry { get => base.costCourierPostEntry; set { base.costCourierPostEntry = value; CalculateSum(); } }
        public override long costCourierPostExit { get => base.costCourierPostExit; set { base.costCourierPostExit = value; CalculateSum(); } }

        public override long creditorCustomerEntry { get => base.creditorCustomerEntry; set { base.creditorCustomerEntry = value; CalculateSum(); } }
        public override long creditorCustomerExit { get => base.creditorCustomerExit; set { base.creditorCustomerExit = value; CalculateSum(); } }

        public override long debtorCustomerEntry { get => base.debtorCustomerEntry; set { base.debtorCustomerEntry = value; CalculateSum(); } }
        public override long debtorCustomerExit { get => base.debtorCustomerExit; set { base.debtorCustomerExit = value; CalculateSum(); } }

        public override long depositBeforeEntry { get => base.depositBeforeEntry; set { base.depositBeforeEntry = value; CalculateSum(); } }
        public override long depositBeforeExit { get => base.depositBeforeExit; set { base.depositBeforeExit = value; CalculateSum(); } }

        public override long depositNewEntry { get => base.depositNewEntry; set { base.depositNewEntry = value; CalculateSum(); } }
        public override long depositNewExit { get => base.depositNewExit; set { base.depositNewExit = value; CalculateSum(); } }

        public override long discountEntry { get => base.discountEntry; set { base.discountEntry = value; CalculateSum(); } }
        public override long discountExit { get => base.discountExit; set { base.discountExit = value; CalculateSum(); } }

        public override long giftEntry { get => base.giftEntry; set { base.giftEntry = value; CalculateSum(); } }
        public override long giftExit { get => base.giftExit; set { base.giftExit = value; CalculateSum(); } }

        public override long leatherStoneEntry { get => base.leatherStoneEntry; set { base.leatherStoneEntry = value; CalculateSum(); } }
        public override long leatherStoneExit { get => base.leatherStoneExit; set { base.leatherStoneExit = value; CalculateSum(); } }

        public override long otherGoldEntry { get => base.otherGoldEntry; set { base.otherGoldEntry = value; CalculateSum(); } }
        public override long otherGoldExit { get => base.otherGoldExit; set { base.otherGoldExit = value; CalculateSum(); } }

        public override long otherKiaGoldEntry { get => base.otherKiaGoldEntry; set { base.otherKiaGoldEntry = value; CalculateSum(); } }
        public override long otherKiaGoldExit { get => base.otherKiaGoldExit; set { base.otherKiaGoldExit = value; CalculateSum(); } }

        public override long returnedEntry { get => base.returnedEntry; set { base.returnedEntry = value; CalculateSum(); } }
        public override long returnedExit { get => base.returnedExit; set { base.returnedExit = value; CalculateSum(); } }


        public override long checkEntry { get => base.checkEntry; set { base.checkEntry = value; CalculateSum(); } }
        public override long checkExit { get => base.checkExit; set { base.checkExit = value; CalculateSum(); } }
        public override long costEntry { get => base.costEntry; set { base.costEntry = value; CalculateSum(); } }
        public override long costExit { get => base.costExit; set { base.costExit = value; CalculateSum(); } }
        #endregion

        public void CalculateSum()
        {
            saleSum = saleExit - saleEntry;
            returnedSum = returnedExit - returnedEntry - saleSum;

            // بانک
            DailyReportBankViewExtendModel lastBank = null;
            foreach (var item in dailyReportBankList.OrderBy(x => x.bankId))
            {
                ((DailyReportBankViewExtendModel)item).bankSum = item.exit - item.entry - (lastBank == null ? returnedSum : lastBank.bankSum);
                ((DailyReportBankViewExtendModel)item).NotifyPropertyChanged("bankSum");
                lastBank = (DailyReportBankViewExtendModel)item;
            }

            cashSum = cashExit - cashEntry - (lastBank != null ? lastBank.bankSum : 0);

            // ارز
            DailyReportCurrencyViewExtendModel lastCurrency = null;
            foreach (var item in dailyReportCurrencyList.OrderBy(x => x.currencyId))
            {
                ((DailyReportCurrencyViewExtendModel)item).CurrencySum = item.rialExit - item.rialEntry - (lastCurrency == null ? cashSum : lastCurrency.CurrencySum);
                ((DailyReportCurrencyViewExtendModel)item).NotifyPropertyChanged("CurrencySum");
                lastCurrency = (DailyReportCurrencyViewExtendModel)item;
            }

            giftSum = giftExit - giftEntry - (lastCurrency == null ? 0 : lastCurrency.CurrencySum);
            checkSum = checkExit - checkEntry - giftSum;
            leatherStoneSum = leatherStoneExit - leatherStoneEntry - checkSum;
            coinSum = coinExit - coinEntry - leatherStoneSum;
            otherKiaGoldSum = otherKiaGoldExit - otherKiaGoldEntry - coinSum;
            otherGoldSum = otherGoldExit - otherGoldEntry - otherKiaGoldSum;
            creditorCustomerSum = creditorCustomerExit - creditorCustomerEntry - otherGoldSum;
            debtorCustomerSum = debtorCustomerExit - debtorCustomerEntry - creditorCustomerSum;
            depositBeforeSum = depositBeforeExit - depositBeforeEntry - debtorCustomerSum;
            depositNewSum = depositNewExit - depositNewEntry - depositBeforeSum;
            discountSum = discountExit - discountEntry - depositNewSum;
            costCourierPostSum = costCourierPostExit - costCourierPostEntry - discountSum;
            costSum = costExit = costEntry - costCourierPostSum;

            NotifyPropertyChanged("saleSum");
            NotifyPropertyChanged("returnedSum");
            NotifyPropertyChanged("cashSum");
            NotifyPropertyChanged("giftSum");
            NotifyPropertyChanged("leatherStoneSum");
            NotifyPropertyChanged("coinSum");
            NotifyPropertyChanged("otherKiaGoldSum");
            NotifyPropertyChanged("otherGoldSum");
            NotifyPropertyChanged("creditorCustomerSum");
            NotifyPropertyChanged("debtorCustomerSum");
            NotifyPropertyChanged("depositBeforeSum");
            NotifyPropertyChanged("depositNewSum");
            NotifyPropertyChanged("discountSum");
            NotifyPropertyChanged("costCourierPostSum");

            NotifyPropertyChanged("costSum");
            NotifyPropertyChanged("checkSum");
        }


        #region NotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
