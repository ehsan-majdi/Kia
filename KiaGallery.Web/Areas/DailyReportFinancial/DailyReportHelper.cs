using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.DailyReportFinancial;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KiaGallery.Web.Areas.DailyReportFinancial
{
    public class DailyReportHelper
    {
        //public void CheckSubmit(DateTime date)
        //{
        //    bool check;
        //    var helper = new PersianCalendar();
        //    var fromDate = helper.ToDateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        //    var toDate = fromDate.AddDays(1);
        //    using (var db = new KiaGalleryContext())
        //    {
        //        List<int> BranchCalendarIdList = db.BranchCalendar.Where(x=> x.ReportDate >= fromDate && x.ReportDate <= toDate).Select(x=>x.Id).ToList();
        //        int count = db.DailyReport.Count(x => x.Status == CalendarStatus.Submit && BranchCalendarIdList.Any(y => y == x.BranchCalendarId));
        //        check = BranchCalendarIdList.Count() == count? true : false;
        //    }
        //    if(check == true)
        //    {
        //        SendReport(date);
        //    }
        //}

        //public void SendReport(DateTime date)
        //{
        //    List<DailyReport> DailyReportList;
        //    var helper = new PersianCalendar();
        //    var fromDate = helper.ToDateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        //    var toDate = fromDate.AddDays(1);
        //    using (var db = new KiaGalleryContext())
        //    {
        //        List<int> BranchCalendarIdList = db.BranchCalendar.Where(x => x.ReportDate >= fromDate && x.ReportDate <= toDate).Select(x => x.Id).ToList();
        //        DailyReportList = db.DailyReport.Where(x => x.Status == CalendarStatus.Submit && BranchCalendarIdList.Any(y => y == x.BranchCalendarId)).ToList();
        //    }
        //    DailyReport total = new DailyReport();
        //    DailyReportBank bank = new DailyReportBank();
        //    DailyReportCurrency currency = new DailyReportCurrency();
        //    DailyReportList.ForEach(x => {
        //        total.SaleWeight += x.SaleWeight;
        //        total.SaleEntry += x.SaleEntry;
        //        total.SaleExit += x.SaleExit;

        //        total.ReturnedWeight += x.ReturnedWeight;
        //        total.ReturnedEntry += x.ReturnedEntry;
        //        total.ReturnedExit += x.ReturnedExit;

        //        total.CashEntry += x.CashEntry;
        //        total.CashExit += x.CashExit;

        //        //total.GiftDescription += "\n" + x.GiftDescription;
        //        total.GiftEntry += x.GiftEntry;
        //        total.GiftExit += x.GiftExit;

        //        //total.LeatherStoneDescription += "\n" + x.LeatherStoneDescription;
        //        total.LeatherStoneEntry += x.LeatherStoneEntry;
        //        total.LeatherStoneExit += x.LeatherStoneExit;

        //        //total.CoinDescription += "\n" + x.CoinDescription;
        //        total.CoinEntry += x.CoinEntry;
        //        total.CoinExit += x.CoinExit;

        //        total.OtherKiaGoldWeight += x.OtherKiaGoldWeight;
        //        total.OtherKiaGoldEntry += x.OtherKiaGoldEntry;
        //        total.OtherKiaGoldExit += x.OtherKiaGoldExit;

        //        total.OtherGoldEntry += x.OtherGoldEntry;
        //        total.OtherGoldExit += x.OtherGoldExit;

        //        total.CreditorCustomerEntry += x.CreditorCustomerEntry;
        //        total.CreditorCustomerExit += x.CreditorCustomerExit;

        //        total.DebtorCustomerEntry += x.DebtorCustomerEntry;
        //        total.DebtorCustomerExit += x.DebtorCustomerExit;

        //        total.DepositBeforeCount += x.DepositBeforeCount;
        //        total.DepositBeforeEntry += x.DepositBeforeEntry;
        //        total.DepositBeforeExit += x.DepositBeforeExit;

        //        total.DepositNewCount += x.DepositNewCount;
        //        total.DepositNewEntry += x.DepositNewEntry;
        //        total.DepositNewExit += x.DepositNewExit;
                
        //        total.DiscountEntry += x.DiscountEntry;
        //        total.DiscountExit += x.DiscountExit;

        //        total.CostCourierPostEntry += x.CostCourierPostEntry;
        //        total.CostCourierPostExit += x.CostCourierPostExit;

        //        total.DepositNewEntry += x.DepositNewEntry;
        //        total.DepositNewEntry += x.DepositNewEntry;

        //        bank.Entry += x.DailyReportBankList.Sum(y => y.Entry);
        //        bank.Exit += x.DailyReportBankList.Sum(y => y.Exit);

        //        currency.Value += x.DailyReportCurrencyList.Sum(y => y.Value);
        //        currency.RialValue += x.DailyReportCurrencyList.Sum(y => y.RialValue);
        //        currency.RialEntry += x.DailyReportCurrencyList.Sum(y => y.RialEntry);
        //        currency.RialExit += x.DailyReportCurrencyList.Sum(y => y.RialExit);
        //    });

        //    string text = "وزن فروش: "+ total.SaleWeight;
        //    text += "\n مبلغ فروش ورودی: " + total.SaleEntry;
        //    text += "\n مبلغ فروش خروجی: " + total.SaleExit;

        //    text += "\n وزن مرجوعی: " + total.ReturnedWeight;
        //    text += "\n مبلغ مرجوعی ورودی: " + total.ReturnedEntry;
        //    text += "\n مبلغ مرجوعی خروجی: " + total.ReturnedExit;

        //    text += "\n مبلغ اسکناس ورودی: " + total.CashEntry;
        //    text += "\n مبلغ اسکناس خروجی: " + total.CashExit;

        //    text += "\n مبلغ گیفت ورودی: " + total.GiftEntry;
        //    text += "\n مبلغ گیفت خروجی: " + total.GiftExit;

        //    text += "\n مبلغ چرم و سنگ ورودی: " + total.LeatherStoneEntry;
        //    text += "\n مبلغ چرم و سنگ خروجی: " + total.LeatherStoneExit;

        //    text += "\n مبلغ سکه ورودی: " + total.CoinEntry;
        //    text += "\n مبلغ سکه خروجی: " + total.CoinExit;

        //    text += "\n وزن طلای کیا: " + total.OtherKiaGoldWeight;
        //    text += "\n مبلغ طلای کیا ورودی: " + total.OtherKiaGoldEntry;
        //    text += "\n مبلغ طلای کیا خروجی: " + total.OtherKiaGoldExit;

        //    text += "\n مبلغ طلای متفرقه ورودی: " + total.OtherGoldEntry;
        //    text += "\n مبلغ طلای متفرقه خروجی: " + total.OtherGoldExit;

        //    text += "\n مبلغ مانده مشتری بستانکار ورودی: " + total.CreditorCustomerEntry;
        //    text += "\n مبلغ مانده مشتری بستانکار خروجی: " + total.CreditorCustomerExit;

        //    text += "\n مبلغ مانده مشتری بدهکار ورودی: " + total.DebtorCustomerEntry;
        //    text += "\n مبلغ مانده مشتری بدهکار خروجی: " + total.DebtorCustomerExit;

        //    text += "\n تعداد بیعانه از قبل: " + total.DepositBeforeCount;
        //    text += "\n بلغ بیعانه از قبل ورودی: " + total.DepositBeforeEntry;
        //    text += "\n مبلغ بیعانه از قبل خروجی: " + total.DepositBeforeExit;

        //    text += "\n تعداد بیعانه جدید: " + total.DepositNewCount;
        //    text += "\n مبلغ بیعانه جدید ورودی: " + total.DepositNewEntry;
        //    text += "\n مبلغ بیعانه جدید خروجی: " + total.DepositNewExit;

        //    text += "\n مبلغ تخفیف ورودی: " + total.DiscountEntry;
        //    text += "\n مبلغ تخفیف خروجی: " + total.DiscountExit;

        //    text += "\n مبلغ هزینه پیک و پست ورودی: " + total.CostCourierPostEntry;
        //    text += "\n مبلغ هزینه پیک و پست خروجی: " + total.CostCourierPostExit;

        //    text += "\n مبلغ ورودی بانک: " + bank.Entry;
        //    text += "\n مبلغ خروجی بانک: " + bank.Exit;

        //    text += "\n مقدار ارز: " + currency.Value;
        //    text += "\n ارزش ریالی ارز: " + currency.RialValue;
        //    text += "\n ارزش ریالی وارد شده ارز: " + currency.RialEntry;
        //    text += "\n ارزش ریالی خارج شده ارز: " + currency.RialExit;


        //}
    }
}