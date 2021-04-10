using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.DailyReportFinancial
{
    /// <summary>
    /// گزارش روزانه
    /// </summary>
    [Table(name: "DailyReport", Schema = "drf")]
    public class DailyReport
    {
        public DailyReport()
        {
            DailyReportBankList = new List<DailyReportBank>();
            DailyReportCurrencyList = new List<DailyReportCurrency>();
            DailyReportLogList = new List<DailyReportLog>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// ردیف تاریخ گزارش شعبه
        /// </summary>
        public int BranchCalendarId { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public CalendarStatus Status { get; set; }

        #region فروش
        /// <summary>
        /// تعداد فاکتور فروش
        /// </summary>
        public int NumberSaleFactor { get; set; }
        /// <summary>
        /// وزن فروش
        /// </summary>
        public decimal SaleWeight { get; set; }
        /// <summary>
        /// وزن درصد فروش
        /// </summary>
        public decimal SaleWeightPercent { get; set; }
        /// <summary>  
        /// مبلغ فروش ورودی
        /// </summary>
        public long SaleEntry { get; set; }
        /// <summary>
        /// مبلغ فروش خروجی
        /// </summary>
        public long SaleExit { get; set; }
        #endregion

        #region مرجوعی
        /// <summary>
        /// تعداد فاکتور مرجوعی
        /// </summary>
        public int NumberReturnedFactor { get; set; }
        /// <summary>
        /// وزن مرجوعی
        /// </summary>
        public decimal ReturnedWeight { get; set; }
        /// <summary>
        /// وزن درصد مرجوعی
        /// </summary>
        public decimal ReturnWeightPercent { get; set; }
        /// <summary>
        /// مبلغ مرجوعی ورودی
        /// </summary>
        public long ReturnedEntry { get; set; }
        /// <summary>
        /// مبلغ مرجوعی خروجی
        /// </summary>
        public long ReturnedExit { get; set; }
        #endregion

        #region اسکناس
        /// <summary>
        /// سایر اسکناس
        /// </summary>
        public long OtherCash { get; set; }
        /// <summary>
        /// مبلغ اسکناس ورودی
        /// </summary>
        public long CashEntry { get; set; }
        /// <summary>
        /// مبلغ اسکناس خروجی
        /// </summary>
        public long CashExit { get; set; }
        #endregion

        #region سایر ارزها
        /// <summary>
        /// سایر ارز ها
        /// </summary>
        public string OtherCurrency { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public decimal OtherCurrencyValue { get; set; }
        /// <summary>
        /// ارزش ریالی
        /// </summary>
        public long OtherCurrencyRialValue { get; set; }
        /// <summary>
        /// ارزش ریالی وارد شده
        /// </summary>
        public long OtherCurrencyRialEntry { get; set; }
        /// <summary>
        /// ارزش ریالی خارج شده
        /// </summary>
        public long OtherCurrencyRialExit { get; set; }
        #endregion

        #region موجودی نقد صندوق
        public long InventoryCash { get; set; }
        #endregion

        #region کسری طلا
        /// <summary>
        /// وزن کسری طلا
        /// </summary>
        public decimal GoldDeficitWeight { get; set; }
        /// <summary>
        /// مبلغ کسری طلا ورودی
        /// </summary>
        public long GoldDeficitEntry { get; set; }
        /// <summary>
        /// مبلغ کسری طلا خروجی
        /// </summary>
        public long GoldDeficitExit { get; set; }
        #endregion

        #region گیفت
        /// <summary>
        /// تعداد گیفت 
        /// </summary>
        public int GiftNumberEntry { get; set; }
        /// <summary>
        /// تعداد گیفت 
        /// </summary>
        public int GiftNumberExit { get; set; }
        /// <summary>
        /// مبلغ گیفت ورودی
        /// </summary>
        public long GiftEntry { get; set; }

        /// <summary>
        /// مبلغ گیفت خروجی
        /// </summary>
        public long GiftExit { get; set; }
        #endregion

        #region بن خرید
        /// <summary>
        /// تعداد بن خرید 
        /// </summary>
        public int CheckNumber { get; set; }
        /// <summary>
        /// مبلغ بن خرید ورودی
        /// </summary>
        public long CheckEntry { get; set; }
        /// <summary>
        /// مبلغ بن خرید خروجی
        /// </summary>
        public long CheckExit { get; set; }
        #endregion


        #region چرم و سنگ
        /// <summary>
        /// توضیحات چرم و سنگ
        /// </summary>
        [MaxLength(255)]
        public string LeatherStoneDescriptionEntry { get; set; }
        /// <summary>
        /// توضیحات چرم و سنگ
        /// </summary>
        [MaxLength(255)]
        public string LeatherStoneDescriptionExit { get; set; }
        /// <summary>
        /// مبلغ چرم و سنگ ورودی
        /// </summary>
        public long LeatherStoneEntry { get; set; }
        /// <summary>
        /// مبلغ چرم و سنگ خروجی
        /// </summary>
        public long LeatherStoneExit { get; set; }
        #endregion

        #region سکه
        /// <summary>
        /// تعداد سکه 
        /// </summary>
        public int CoinNumber { get; set; }
        /// <summary>
        /// توضیحات سکه
        /// </summary>
        [MaxLength(255)]
        public string CoinDescription { get; set; }
        /// <summary>
        /// مبلغ سکه ورودی
        /// </summary>
        public long CoinEntry { get; set; }
        /// <summary>
        /// مبلغ سکه خروجی
        /// </summary>
        public long CoinExit { get; set; }
        #endregion

        #region متفرقه کیا
        /// <summary>
        /// وزن طلای کیا
        /// </summary>
        public decimal OtherKiaGoldWeight { get; set; }
        /// <summary>
        /// مبلغ طلای کیا ورودی
        /// </summary>
        public long OtherKiaGoldEntry { get; set; }
        /// <summary>
        /// مبلغ طلای کیا خروجی
        /// </summary>
        public long OtherKiaGoldExit { get; set; }
        #endregion

        #region طلای متفرقه
        /// <summary>
        /// وزن طلای متفرقه
        /// </summary>
        public decimal OtherGoldWeight { get; set; }
        /// <summary>
        /// مبلغ طلای متفرقه ورودی
        /// </summary>
        public long OtherGoldEntry { get; set; }
        /// <summary>
        /// مبلغ طلای متفرقه خروجی
        /// </summary>
        public long OtherGoldExit { get; set; }
        #endregion

        #region مانده مشتری بستانکار
        /// <summary>
        /// مبلغ مانده مشتری بستانکار ورودی
        /// </summary>
        public long CreditorCustomerEntry { get; set; }
        /// <summary>
        /// مبلغ مانده مشتری بستانکار خروجی
        /// </summary>
        public long CreditorCustomerExit { get; set; }
        #endregion

        #region مانده مشتری بدهکار
        /// <summary>
        /// مبلغ مانده مشتری بدهکار ورودی
        /// </summary>
        public long DebtorCustomerEntry { get; set; }
        /// <summary>
        /// مبلغ مانده مشتری بدهکار خروجی
        /// </summary>
        public long DebtorCustomerExit { get; set; }
        #endregion

        #region بیعانه از قبل
        /// <summary>
        /// تعداد بیعانه از قبل
        /// </summary>
        public int DepositBeforeCount { get; set; }
        /// <summary>
        /// مبلغ بیعانه از قبل وروردی
        /// </summary>
        public long DepositBeforeEntry { get; set; }
        /// <summary>
        /// مبلغ بیعانه از قبل خروجی
        /// </summary>
        public long DepositBeforeExit { get; set; }
        #endregion

        #region بیعانه جدید
        /// <summary>
        /// تعداد بیعانه جدید
        /// </summary>
        public int DepositNewCount { get; set; }
        /// <summary>
        /// مبلغ بیعانه جدید ورودی
        /// </summary>
        public long DepositNewEntry { get; set; }
        /// <summary>
        /// مبلغ بیعانه جدید خروجی
        /// </summary>
        public long DepositNewExit { get; set; }
        #endregion

        #region تخفیف
        /// <summary>
        /// مبلغ تخفیف ورودی
        /// </summary>
        public long DiscountEntry { get; set; }
        /// <summary>
        /// مبلغ تخفیف خروجی
        /// </summary>
        public long DiscountExit { get; set; }
        /// <summary>
        /// مبلغ  کارت باشگاهی ورودی
        /// </summary>
        public long LoyalityEntry { get; set; }

        /// <summary>
        /// مبلغ  کارت باشگاهی ورودی
        /// </summary>
        public long LoyalityExit { get; set; }
        #endregion

        #region هزینه پیک و پست
        /// <summary>
        /// مبلغ هزینه پیک و پست ورودی
        /// </summary>
        public long CostCourierPostEntry { get; set; }
        /// <summary>
        /// مبلغ هزینه پیک و پست خروجی
        /// </summary>
        public long CostCourierPostExit { get; set; }
        #endregion
        
        #region هزینه
        /// <summary>
        /// مبلغ هزینه ورودی
        /// </summary>
        public long CostEntry { get; set; }
        /// <summary>
        /// مبلغ هزینه خروجی
        /// </summary>
        public long CostExit { get; set; }
        #endregion

        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// آیا این گزارش در ربات ارسال شده است؟
        /// </summary>
        public bool Sent { get; set; }

        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// تاریخ گزارش شعبه
        /// </summary>
        public virtual BranchCalendar BranchCalendar { get; set; }

        /// <summary>
        /// لیست بانک های گزارش
        /// </summary>
        public virtual List<DailyReportBank> DailyReportBankList { get; set; }
        /// <summary>
        /// لیست ارزهای گزارش
        /// </summary>
        public virtual List<DailyReportCurrency> DailyReportCurrencyList { get; set; }
        /// <summary>
        /// لیست تغییر وضعیت های گزارش
        /// </summary>
        public virtual List<DailyReportLog> DailyReportLogList { get; set; }
    }
}