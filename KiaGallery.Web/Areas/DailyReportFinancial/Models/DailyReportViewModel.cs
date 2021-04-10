using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Areas.DailyReportFinancial.Models
{
    public class SaveDailyReportViewModel
    {
        public string token { get; set; }
        public string date { get; set; }
        public DailyReportViewModel report { get; set; }
    }

    public class DailyReportViewModel
    {
        public DailyReportViewModel()
        {
            dailyReportBankList = new List<DailyReportBankViewModel>();
            dailyReportCurrencyList = new List<DailyReportCurrencyViewModel>();
        }

        #region فروش
        /// <summary>
        /// وزن فروش
        /// </summary>
        public float saleWeight { get; set; }
        /// <summary>
        /// تعداد فاکتور فروش
        /// </summary>
        public int numberSaleFactor { get; set; }
        /// <summary>
        /// مبلغ فروش ورودی
        /// </summary>
        public virtual long saleEntry { get; set; }
        /// <summary>
        /// مبلغ فروش خروجی
        /// </summary>
        public virtual long saleExit { get; set; }
        #endregion

        #region مرجوعی
        /// <summary>
        /// وزن مرجوعی
        /// </summary>
        public float returnedWeight { get; set; }
        /// <summary>
        /// تعداد فاکتور مرجوعی
        /// </summary>
        public int numberReturnedFactor { get; set; }
        /// <summary>
        /// مبلغ مرجوعی ورودی
        /// </summary>
        public virtual long returnedEntry { get; set; }
        /// <summary>
        /// مبلغ مرجوعی خروجی
        /// </summary>
        public virtual long returnedExit { get; set; }
        #endregion

        #region اسکناس
        /// <summary>
        /// مبلغ اسکناس ورودی
        /// </summary>
        public virtual long cashEntry { get; set; }
        /// <summary>
        /// مبلغ اسکناس خروجی
        /// </summary>
        public virtual long cashExit { get; set; }
        #endregion

        #region گیفت
        /// <summary>
        /// تعداد گیفت 
        /// </summary>
        public int giftNumber { get; set; }
        /// <summary>
        /// توضیحات گیفت
        /// </summary>
        public string giftDescription { get; set; }
        /// <summary>
        /// مبلغ گیفت ورودی
        /// </summary>
        public virtual long giftEntry { get; set; }
        /// <summary>
        /// مبلغ گیفت خروجی
        /// </summary>
        public virtual long giftExit { get; set; }
        #endregion

        #region بن خرید
        /// <summary>
        /// تعداد بن خرید 
        /// </summary>
        public int checkNumber { get; set; }
        /// <summary>
        /// توضیحات بن خرید
        /// </summary>
        public string checkDescription { get; set; }
        /// <summary>
        /// مبلغ بن خرید ورودی
        /// </summary>
        public virtual long checkEntry { get; set; }

        /// <summary>
        /// مبلغ بن خرید خروجی
        /// </summary>
        public virtual long checkExit { get; set; }
        #endregion


        #region چرم و سنگ
        /// <summary>
        /// توضیحات چرم و سنگ
        /// </summary>
        public string leatherStoneDescription { get; set; }
        /// <summary>
        /// مبلغ چرم و سنگ ورودی
        /// </summary>
        public virtual long leatherStoneEntry { get; set; }
        /// <summary>
        /// مبلغ چرم و سنگ خروجی
        /// </summary>
        public virtual long leatherStoneExit { get; set; }
        #endregion

        #region سکه
        /// <summary>
        /// تعداد سکه 
        /// </summary>
        public int coinNumber { get; set; }
        /// <summary>
        /// توضیحات سکه
        /// </summary>
        public string coinDescription { get; set; }
        /// <summary>
        /// مبلغ سکه ورودی
        /// </summary>
        public virtual long coinEntry { get; set; }
        /// <summary>
        /// مبلغ سکه خروجی
        /// </summary>
        public virtual long coinExit { get; set; }
        #endregion

        #region متفرقه کیا
        /// <summary>
        /// وزن طلای کیا
        /// </summary>
        public float otherKiaGoldWeight { get; set; }
        /// <summary>
        /// مبلغ طلای کیا ورودی
        /// </summary>
        public virtual long otherKiaGoldEntry { get; set; }
        /// <summary>
        /// مبلغ طلای کیا خروجی
        /// </summary>
        public virtual long otherKiaGoldExit { get; set; }
        #endregion

        #region طلای متفرقه
        /// <summary>
        /// مبلغ طلای متفرقه ورودی
        /// </summary>
        public virtual long otherGoldEntry { get; set; }
        /// <summary>
        /// مبلغ طلای متفرقه خروجی
        /// </summary>
        public virtual long otherGoldExit { get; set; }
        #endregion

        #region مانده مشتری بستانکار
        /// <summary>
        /// مبلغ مانده مشتری بستانکار ورودی
        /// </summary>
        public virtual long creditorCustomerEntry { get; set; }
        /// <summary>
        /// مبلغ مانده مشتری بستانکار خروجی
        /// </summary>
        public virtual long creditorCustomerExit { get; set; }
        #endregion

        #region مانده مشتری بدهکار
        /// <summary>
        /// مبلغ مانده مشتری بدهکار ورودی
        /// </summary>
        public virtual long debtorCustomerEntry { get; set; }
        /// <summary>
        /// مبلغ مانده مشتری بدهکار خروجی
        /// </summary>
        public virtual  long debtorCustomerExit { get; set; }
        #endregion

        #region بیعانه از قبل
        /// <summary>
        /// تعداد بیعانه از قبل
        /// </summary>
        public int depositBeforeCount { get; set; }
        /// <summary>
        /// کبلغ بیعانه از قبل وروردی
        /// </summary>
        public virtual long depositBeforeEntry { get; set; }
        /// <summary>
        /// مبلغ بیعانه از قبل خروجی
        /// </summary>
        public virtual long depositBeforeExit { get; set; }
        #endregion

        #region بیعانه جدید
        /// <summary>
        /// تعداد بیعانه جدید
        /// </summary>
        public int depositNewCount { get; set; }
        /// <summary>
        /// مبلغ بیعانه جدید ورودی
        /// </summary>
        public virtual long depositNewEntry { get; set; }
        /// <summary>
        /// مبلغ بیعانه جدید خروجی
        /// </summary>
        public virtual long depositNewExit { get; set; }
        #endregion

        #region تخفیف
        /// <summary>
        /// مبلغ تخفیف ورودی
        /// </summary>
        public virtual long discountEntry { get; set; }
        /// <summary>
        /// مبلغ تخفیف خروجی
        /// </summary>
        public virtual long discountExit { get; set; }
        #endregion

        #region هزینه پیک و پست
        /// <summary>
        /// مبلغ هزینه پیک و پست ورودی
        /// </summary>
        public virtual long costCourierPostEntry { get; set; }
        /// <summary>
        /// مبلغ هزینه پیک و پست خروجی
        /// </summary>
        public virtual long costCourierPostExit { get; set; }
        #endregion

        #region هزینه
        /// <summary>
        /// مبلغ هزینه ورودی
        /// </summary>
        public virtual long costEntry { get; set; }
        /// <summary>
        /// مبلغ هزینه خروجی
        /// </summary>
        public virtual long costExit { get; set; }
        #endregion

        /// <summary>
        /// لیست بانک های گزارش
        /// </summary>
        public virtual List<DailyReportBankViewModel> dailyReportBankList { get; set; }
        /// <summary>
        /// لیست ارزهای گزارش
        /// </summary>
        public virtual List<DailyReportCurrencyViewModel> dailyReportCurrencyList { get; set; }
    }


    public class DailyReportBankViewModel
    {
        /// <summary>
        /// ردیف بانک
        /// </summary>
        public int bankId { get; set; }
        /// <summary>
        /// مبلغ ورودی
        /// </summary>
        public virtual long entry { get; set; }
        /// <summary>
        /// مبلغ خروجی
        /// </summary>
        public virtual long exit { get; set; }
    }

    public class DailyReportCurrencyViewModel
    {
        /// <summary>
        /// ردیف ارز
        /// </summary>
        public int currencyId { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public float value { get; set; }
        /// <summary>
        /// ارزش ریالی
        /// </summary>
        public long rialValue { get; set; }
        /// <summary>
        /// ارزش ریالی وارد شده
        /// </summary>
        public virtual long rialEntry { get; set; }
        /// <summary>
        /// ارزش ریالی خارج شده
        /// </summary>
        public virtual long rialExit { get; set; }
    }
}