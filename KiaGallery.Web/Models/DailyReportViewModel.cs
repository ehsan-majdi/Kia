using KiaGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Models
{
    public class SaveDailyReportViewModel
    {
        public string token { get; set; }
        public string date { get; set; }
        public int branchId { get; set; }
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
        public decimal saleWeight { get; set; }
        /// <summary>
        /// تعداد فاکتور فروش
        /// </summary>
        public int numberSaleFactor { get; set; }
        /// <summary>
        /// مبلغ فروش ورودی
        /// </summary>
        public long saleEntry { get; set; }
        /// <summary>
        /// وزن درصد فروش
        /// </summary>
        public decimal saleWeightPercent { get; set; }
        /// <summary>
        /// مبلغ فروش خروجی
        /// </summary>
        public long saleExit { get; set; }
        #endregion

        #region مرجوعی
        /// <summary>
        /// وزن مرجوعی
        /// </summary>
        public decimal returnedWeight { get; set; }
        /// <summary>
        /// تعداد فاکتور مرجوعی
        /// </summary>
        public int numberReturnedFactor { get; set; }
        /// <summary>
        /// مبلغ مرجوعی ورودی
        /// </summary>
        public long returnedEntry { get; set; }
        /// <summary>
        /// وزن درصد مرجوعی
        /// </summary>
        public decimal returnWeightPercent { get; set; }
        /// <summary>
        /// مبلغ مرجوعی خروجی
        /// </summary>
        public long returnedExit { get; set; }
        #endregion

        #region اسکناس
        /// <summary>
        /// سایر اسکناس
        /// </summary>
        public long otherCash { get; set; }
        /// <summary>
        /// مبلغ اسکناس ورودی
        /// </summary>
        public long cashEntry { get; set; }
        /// <summary>
        /// مبلغ اسکناس خروجی
        /// </summary>
        public long cashExit { get; set; }
        #endregion

        #region سایر ارزها
        /// <summary>
        /// سایر ارز ها
        /// </summary>
        public string otherCurrency { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public decimal otherCurrencyValue { get; set; }
        /// <summary>
        /// ارزش ریالی
        /// </summary>
        public long otherCurrencyRialValue { get; set; }
        /// <summary>
        /// ارزش ریالی وارد شده
        /// </summary>
        public long otherCurrencyRialEntry { get; set; }
        /// <summary>
        /// ارزش ریالی خارج شده
        /// </summary>
        public long otherCurrencyRialExit { get; set; }
        #endregion

        #region موجودی نقد صندوق
        public long inventoryCash { get; set; }
        #endregion

        #region کسری طلا
        /// <summary>
        /// وزن کسری طلا
        /// </summary>
        public decimal goldDeficitWeight { get; set; }
        /// <summary>
        /// مبلغ کسری طلا ورودی
        /// </summary>
        public long goldDeficitEntry { get; set; }
        /// <summary>
        /// مبلغ کسری طلا خروجی
        /// </summary>
        public long goldDeficitExit { get; set; }
        #endregion

        #region گیفت
        /// <summary>
        /// تعداد گیفت 
        /// </summary>
        public int giftNumberEntry { get; set; }
        /// <summary>
        /// تعداد گیفت 
        /// </summary>
        public int giftNumberExit { get; set; }
        /// <summary>
        /// توضیحات گیفت
        /// </summary>
        public string giftDescription { get; set; }
        /// <summary>
        /// مبلغ گیفت ورودی
        /// </summary>
        public long giftEntry { get; set; }
        /// <summary>
        /// مبلغ گیفت خروجی
        /// </summary>
        public long giftExit { get; set; }

        /// <summary>
        /// تعداد کارت باشگاهی ورودی 
        /// </summary>
        public int loyalityNumberEntry { get; set; }
        /// <summary>
        /// تعداد  کارت باشگاهی ورودی 
        /// </summary>
        public int loyalityNumberExit { get; set; }
        /// <summary>
        /// مبلغ  کارت باشگاهی ورودی
        /// </summary>
        public long loyalityEntry { get; set; }

        /// <summary>
        /// مبلغ  کارت باشگاهی ورودی
        /// </summary>
        public long loyalityExit { get; set; }
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
        public long checkEntry { get; set; }
        /// <summary>
        /// مبلغ بن خرید خروجی
        /// </summary>
        public long checkExit { get; set; }
        #endregion


        #region چرم و سنگ
        /// <summary>
        /// توضیحات چرم و سنگ
        /// </summary>
        public string leatherStoneDescriptionEntry { get; set; }
        /// <summary>
        /// توضیحات چرم و سنگ
        /// </summary>
        public string leatherStoneDescriptionExit { get; set; }
        /// <summary>
        /// مبلغ چرم و سنگ ورودی
        /// </summary>
        public long leatherStoneEntry { get; set; }
        /// <summary>
        /// مبلغ چرم و سنگ خروجی
        /// </summary>
        public long leatherStoneExit { get; set; }
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
        public long coinEntry { get; set; }
        /// <summary>
        /// مبلغ سکه خروجی
        /// </summary>
        public long coinExit { get; set; }
        #endregion

        #region متفرقه کیا
        /// <summary>
        /// وزن طلای کیا
        /// </summary>
        public decimal otherKiaGoldWeight { get; set; }
        /// <summary>
        /// مبلغ طلای کیا ورودی
        /// </summary>
        public long otherKiaGoldEntry { get; set; }
        /// <summary>
        /// مبلغ طلای کیا خروجی
        /// </summary>
        public long otherKiaGoldExit { get; set; }
        #endregion

        #region طلای متفرقه
        /// <summary>
        /// وزن طلای کیا
        /// </متفرقه>
        public decimal otherGoldWeight { get; set; }
        /// <summary>
        /// مبلغ طلای متفرقه ورودی
        /// </summary>
        public long otherGoldEntry { get; set; }
        /// <summary>
        /// مبلغ طلای متفرقه خروجی
        /// </summary>
        public long otherGoldExit { get; set; }
        #endregion

        #region مانده مشتری بستانکار
        /// <summary>
        /// مبلغ مانده مشتری بستانکار ورودی
        /// </summary>
        public long creditorCustomerEntry { get; set; }
        /// <summary>
        /// مبلغ مانده مشتری بستانکار خروجی
        /// </summary>
        public long creditorCustomerExit { get; set; }
        #endregion

        #region مانده مشتری بدهکار
        /// <summary>
        /// مبلغ مانده مشتری بدهکار ورودی
        /// </summary>
        public long debtorCustomerEntry { get; set; }
        /// <summary>
        /// مبلغ مانده مشتری بدهکار خروجی
        /// </summary>
        public long debtorCustomerExit { get; set; }
        #endregion

        #region بیعانه از قبل
        /// <summary>
        /// تعداد بیعانه از قبل
        /// </summary>
        public int depositBeforeCount { get; set; }
        /// <summary>
        /// کبلغ بیعانه از قبل وروردی
        /// </summary>
        public long depositBeforeEntry { get; set; }
        /// <summary>
        /// مبلغ بیعانه از قبل خروجی
        /// </summary>
        public long depositBeforeExit { get; set; }
        #endregion

        #region بیعانه جدید
        /// <summary>
        /// تعداد بیعانه جدید
        /// </summary>
        public int depositNewCount { get; set; }
        /// <summary>
        /// مبلغ بیعانه جدید ورودی
        /// </summary>
        public long depositNewEntry { get; set; }
        /// <summary>
        /// مبلغ بیعانه جدید خروجی
        /// </summary>
        public long depositNewExit { get; set; }
        #endregion

        #region تخفیف
        /// <summary>
        /// مبلغ تخفیف ورودی
        /// </summary>
        public long discountEntry { get; set; }
        /// <summary>
        /// مبلغ تخفیف خروجی
        /// </summary>
        public long discountExit { get; set; }
        #endregion

        #region هزینه پیک و پست
        /// <summary>
        /// مبلغ هزینه پیک و پست ورودی
        /// </summary>
        public long costCourierPostEntry { get; set; }
        /// <summary>
        /// مبلغ هزینه پیک و پست خروجی
        /// </summary>
        public long costCourierPostExit { get; set; }
        #endregion

        #region هزینه
        /// <summary>
        /// مبلغ هزینه ورودی
        /// </summary>
        public long costEntry { get; set; }
        /// <summary>
        /// مبلغ هزینه خروجی
        /// </summary>
        public long costExit { get; set; }
        #endregion

        /// <summary>
        /// لیست بانک های گزارش
        /// </summary>
        public virtual List<DailyReportBankViewModel> dailyReportBankList { get; set; }
        /// <summary>
        /// لیست ارزهای گزارش
        /// </summary>
        public virtual List<DailyReportCurrencyViewModel> dailyReportCurrencyList { get; set; }
        /// <summary>
        /// لیست تغییرات اعمال شده روی سفارش
        /// </summary>
        public virtual List<DailyReportLogViewModel> logListViewModel { get; set; }
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
        public long entry { get; set; }
        /// <summary>
        /// مبلغ خروجی
        /// </summary>
        public long exit { get; set; }
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
        public decimal value { get; set; }
        /// <summary>
        /// ارزش ریالی
        /// </summary>
        public long rialValue { get; set; }
        /// <summary>
        /// ارزش ریالی وارد شده
        /// </summary>
        public long rialEntry { get; set; }
        /// <summary>
        /// ارزش ریالی خارج شده
        /// </summary>
        public long rialExit { get; set; }
    }

    public class DailyReportSummaryViewModel
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public CalendarStatus? status { get; set; }
        /// <summary>
        /// عنوان وضعیت
        /// </summary>
        public string statusTitle { get; set; }
        /// <summary>
        /// وزن فروش
        /// </summary>
        public decimal? saleWeight { get; set; }
        /// <summary>
        /// مبلغ فروش خروجی
        /// </summary>
        public long? saleExit { get; set; }
    }

    public class BranchesSummary
    {
        public long? saleExit { get; set; }
        public long? returnedEntry { get; set; }
        public long? otherKiaGoldEntry { get; set; }
        public long? otherGoldEntry { get; set; }
        public long? totalPrice { get; set; }
        public decimal? saleWeight { get; set; }
        public decimal? saleWeightPercent { get; set; }
        public decimal? returnedWeight { get; set; }
        public decimal? returnedWeightPercent { get; set; }
        public decimal? otherKiaGoldWeight { get; set; }
        public decimal? otherGoldWeight { get; set; }
        public decimal? totalWeight { get; set; }
        public int branchId { get; set; }
        public string branchName { get; set; }
        public CalendarStatus? status { get; set; }
        public string statusTitle { get; set; }
        public string date { get; set; }
    }

    public class DailyReportLogViewModel
    {
        public int id { get; set; }
        public CalendarStatus status { get; set; }
        public string statusTitle { get; set; }
        public int userId { get; set; }
        public string userFullName { get; set; }
        public DateTime date { get; set; }
        public string persianDate { get; set; }
    }

    public class TotalReportViewModel
    {
        public List<int> branchList { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
}