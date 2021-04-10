using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Model.Context.BotDailyReport;
using KiaGallery.Model.Context.DailyReportFinancial;
using KiaGallery.Web.Areas.DailyReportFinancial.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Data.Entity;
using System.Data;

namespace KiaGallery.Web.Areas.DailyReportFinancial.Controllers
{
    /// <summary>
    /// سرویس ذخیره گزارش روزانه
    /// </summary>
    public class DailyReporttController : Controller
    {
        private TelegramBotClient Bot;
        /// <summary>
        /// دخیره پیش نویس
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات جهت ذخیره</param>
        /// <returns>نتیجه ذخیره سازی</returns>
        public JsonResult SaveDraft(SaveDailyReportViewModel model)
        {
            return Json(DoSave(model, CalendarStatus.Draft), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// دخیره نهایی
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات جهت ذخیره</param>
        /// <returns>نتیجه ذخیره سازی</returns>
        public JsonResult Submit(SaveDailyReportViewModel model)
        {
            return Json(DoSave(model, CalendarStatus.Submit), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// متد ذخیره اطلاعات ارسال شده توسط نرم افزار کاربر
        /// بعد از بررسی اطلاعات ارسال نتیجه را ذخیره می کند.
        /// </summary>
        /// <param name="model">مدل حاوی اطلاعات جهت ذخیره</param>
        /// <param name="status">وضعیت ذخیره</param>
        /// <returns>نتیجه عمل ذخیره اطلاعات</returns>
        private Response DoSave(SaveDailyReportViewModel model, CalendarStatus status)
        {
            Response response;
            try
            {
                DailyReport entity = null;
                List<Bank> bankList = null;
                List<Currency> currencyList = null;
                List<BotDailyReportUserData> listBotDailyReportUserData = null;
                string token = "";
                string branchName = "";
                List<int> listBranchCalendarId = null;
                List<DailyReport> listDailyReport = null;
                if (!string.IsNullOrEmpty(model.token))
                {
                    using (var db = new KiaGalleryContext())
                    {

                        var user = db.Token.Where(x => x.Voided == false && x.Code == model.token).Select(x => new { x.User.Branch.Name, x.User }).SingleOrDefault(); // بررسی اعتبار توکن
                        branchName = user.Name;
                        if (user != null)
                        {
                            if (model.report.dailyReportBankList.Count() > 0)
                                bankList = db.Bank.Where(x => model.report.dailyReportBankList.Any(y => y.bankId == x.Id)).ToList();
                            if (model.report.dailyReportCurrencyList.Count() > 0)
                                currencyList = db.Currency.Where(x => model.report.dailyReportCurrencyList.Any(y => y.currencyId == x.Id)).ToList();
                            var date = DateUtility.GetDateTime(model.date);
                            var calendar = db.BranchCalendar.Where(x => x.BranchId == user.User.BranchId && x.ReportDate == date).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده

                            if (calendar != null)
                            {
                                entity = db.DailyReport.Where(x => x.BranchId == user.User.BranchId && x.BranchCalendarId == calendar.Id).SingleOrDefault();

                                if (entity == null)
                                {
                                    entity = new DailyReport();
                                }

                                entity.BranchId = user.User.BranchId.GetValueOrDefault();
                                entity.BranchCalendarId = calendar.Id;
                                entity.Status = status;

                                var report = model.report;

                                entity.NumberSaleFactor = report.numberSaleFactor;
                                entity.SaleWeight = report.saleWeight;
                                entity.SaleEntry = report.saleEntry;
                                entity.SaleExit = report.saleExit;

                                entity.NumberReturnedFactor = report.numberSaleFactor;
                                entity.ReturnedWeight = report.returnedWeight;
                                entity.ReturnedEntry = report.returnedEntry;
                                entity.ReturnedExit = report.returnedExit;

                                entity.CashEntry = report.cashEntry;
                                entity.CashExit = report.cashExit;

                                entity.GiftNumber = report.giftNumber;
                                entity.GiftEntry = report.giftEntry;
                                entity.GiftExit = report.giftExit;

                                entity.CheckNumber = report.checkNumber;
                                entity.CheckEntry = report.checkEntry;
                                entity.CheckExit = report.checkExit;


                                entity.LeatherStoneDescription = report.leatherStoneDescription;
                                entity.LeatherStoneEntry = report.leatherStoneEntry;
                                entity.LeatherStoneExit = report.leatherStoneExit;

                                entity.CoinNumber = report.coinNumber;
                                entity.CoinDescription = report.coinDescription;
                                entity.CoinEntry = report.coinEntry;
                                entity.CoinExit = report.coinExit;

                                entity.OtherKiaGoldWeight = report.otherKiaGoldWeight;
                                entity.OtherKiaGoldEntry = report.otherKiaGoldEntry;
                                entity.OtherKiaGoldExit = report.otherKiaGoldExit;

                                entity.OtherGoldEntry = report.otherGoldEntry;
                                entity.OtherGoldExit = report.otherGoldExit;

                                entity.CreditorCustomerEntry = report.creditorCustomerEntry;
                                entity.CreditorCustomerExit = report.creditorCustomerExit;

                                entity.DebtorCustomerEntry = report.debtorCustomerEntry;
                                entity.DebtorCustomerExit = report.debtorCustomerExit;

                                entity.DepositBeforeCount = report.depositBeforeCount;
                                entity.DepositBeforeEntry = report.depositBeforeEntry;
                                entity.DepositBeforeExit = report.depositBeforeExit;

                                entity.DepositNewCount = report.depositNewCount;
                                entity.DepositNewEntry = report.depositNewEntry;
                                entity.DepositNewExit = report.depositNewExit;

                                entity.DiscountEntry = report.discountEntry;
                                entity.DiscountExit = report.discountExit;

                                entity.CostCourierPostEntry = report.costCourierPostEntry;
                                entity.CostCourierPostExit = report.costCourierPostExit;

                                entity.CostEntry = report.costEntry;
                                entity.CostExit = report.costExit;


                                report.dailyReportBankList.ForEach(x =>
                                {
                                    var bank = entity.DailyReportBankList.SingleOrDefault(y => y.BankId == x.bankId);
                                    if (bank != null)
                                    {
                                        bank.Entry = x.entry;
                                        bank.Exit = x.exit;
                                    }
                                    else
                                    {
                                        bank = new DailyReportBank
                                        {
                                            BankId = x.bankId,
                                            DailyReport = entity,
                                            Entry = x.entry,
                                            Exit = x.exit
                                        };
                                        entity.DailyReportBankList.Add(bank);
                                    }
                                });
                                report.dailyReportCurrencyList.ForEach(x =>
                                {
                                    var currency = entity.DailyReportCurrencyList.SingleOrDefault(y => y.CurrencyId == x.currencyId);
                                    if (currency != null)
                                    {
                                        currency.Value = x.value;
                                        currency.RialValue = x.rialValue;
                                        currency.RialEntry = x.rialEntry;
                                        currency.RialExit = x.rialExit;
                                    }
                                    else
                                    {
                                        currency = new DailyReportCurrency
                                        {
                                            CurrencyId = x.currencyId,
                                            DailyReport = entity,
                                            Value = x.value,
                                            RialValue = x.rialValue,
                                            RialEntry = x.rialEntry,
                                            RialExit = x.rialExit
                                        };
                                        entity.DailyReportCurrencyList.Add(currency);
                                    }
                                });

                                if (entity.Id == 0)
                                {
                                    db.DailyReport.Add(entity);
                                }

                                DailyReportLog log = new DailyReportLog
                                {
                                    DailyReport = entity,
                                    UserId = user.User.Id,
                                    Date = DateTime.Now,
                                    Status = status,
                                    Ip = Request.UserHostAddress
                                };
                                token = db.DailyReportSettings.Single(x => x.Key == "token").Value;
                                listBotDailyReportUserData = db.BotDailyReportUserData.Where(x => x.BranchId == user.User.BranchId || x.BranchId == 19).ToList();
                                db.SaveChanges();
                                listBranchCalendarId = db.BranchCalendar.Where(x => x.ReportDate == date).Select(x=> x.Id).ToList(); //  گزارشتات که باید وارد شود
                                listDailyReport = db.DailyReport.Include(x=> x.Branch).Where(x =>x.Status == CalendarStatus.Submit && listBranchCalendarId.Any(y => x.BranchCalendarId == y)).ToList();
                                response = new Response()
                                {
                                    status = 200,
                                    message = "اطلاعات با موفقیت ذخیره شد."
                                };
                            }
                            else
                            {
                                response = new Response()
                                {
                                    status = 500,
                                    message = "شما مجاز به ثبت گزارش روزانه در این تاریخ نیستید."
                                };
                            }
                        }
                        else
                        {
                            response = new Response
                            {
                                status = 403,
                                message = "توکن ارسال شده معتبر نمی باشد."
                            };
                        }
                    }
                }
                else
                {
                    response = new Response
                    {
                        status = 403,
                        message = "توکن ارسال شده معتبر نمی باشد."
                    };

                }

                if (response.status == 200 && status == CalendarStatus.Submit)
                {
                    long SaleRemaining = entity.SaleExit - entity.SaleEntry;
                    long ReturnedRemaining = SaleRemaining + entity.ReturnedExit - entity.ReturnedEntry;
                    long DailyReportBankRemaining0 = 0, DailyReportBankRemaining1 = 0, DailyReportBankRemaining2 = 0;
                    long DailyReportCurrencyRemaining0 = 0, DailyReportCurrencyRemaining1 = 0;
                    if (model.report.dailyReportBankList?.Count() > 0)
                        DailyReportBankRemaining0 = ReturnedRemaining + entity.DailyReportBankList[0].Exit - entity.DailyReportBankList[0].Entry;
                    if (model.report.dailyReportBankList?.Count() > 1)
                        DailyReportBankRemaining1 = DailyReportBankRemaining0 + entity.DailyReportBankList[1].Exit - entity.DailyReportBankList[1].Entry;
                    if (model.report.dailyReportBankList?.Count() > 2)
                        DailyReportBankRemaining2 = DailyReportBankRemaining1 + entity.DailyReportBankList[2].Exit - entity.DailyReportBankList[2].Entry;

                    if (model.report.dailyReportCurrencyList.Count() > 0)
                        DailyReportCurrencyRemaining0 = DailyReportBankRemaining2 + entity.DailyReportCurrencyList[0].RialExit - entity.DailyReportCurrencyList[0].RialEntry;
                    if (model.report.dailyReportCurrencyList.Count() > 1)
                        DailyReportCurrencyRemaining1 = DailyReportCurrencyRemaining0 + entity.DailyReportCurrencyList[1].RialExit - entity.DailyReportCurrencyList[1].RialEntry;

                    long CashRemaining = DailyReportCurrencyRemaining1 + entity.CashExit - entity.CashEntry;
                    long GiftRemaining = CashRemaining + entity.GiftExit - entity.GiftEntry;
                    long CheckRemaining = GiftRemaining + entity.CheckExit - entity.CheckEntry;
                    long LeatherStoneRemaining = CheckRemaining + entity.LeatherStoneExit - entity.LeatherStoneEntry;
                    long CoinRemaining = LeatherStoneRemaining + entity.CoinExit - entity.CoinEntry;
                    long OtherKiaGoldRemaining = CoinRemaining + entity.OtherKiaGoldExit - entity.OtherKiaGoldEntry;
                    long OtherGoldRemaining = OtherKiaGoldRemaining + entity.OtherGoldExit - entity.OtherGoldEntry;
                    long CreditorCustomerRemaining = OtherGoldRemaining + entity.CreditorCustomerExit - entity.CreditorCustomerEntry;
                    long DebtorCustomerRemaining = CreditorCustomerRemaining + entity.DebtorCustomerExit - entity.DebtorCustomerEntry;
                    long DepositBeforeRemaining = DebtorCustomerRemaining + entity.DepositBeforeExit - entity.DepositBeforeEntry;
                    long DepositNewRemaining = DepositBeforeRemaining + entity.DepositNewExit - entity.DepositNewEntry;
                    long DiscountRemaining = DepositNewRemaining + entity.DiscountExit - entity.DiscountEntry;
                    long CostCourierPostRemaining = DiscountRemaining + entity.CostCourierPostExit - entity.CostCourierPostEntry;
                    long CostRemaining = CostCourierPostRemaining + entity.CostExit - entity.CostEntry;

                    StiReport report = new StiReport();
                    report.Load(Server.MapPath("~/Report/DailyReport/DailyReport.mrt"));
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.Dictionary.Variables["NumberSaleFactor"].Value = Core.ToSeparator(entity.NumberReturnedFactor);
                    report.Dictionary.Variables["SaleWeight"].Value = entity.SaleWeight.ToString();
                    report.Dictionary.Variables["SaleEntry"].Value = Core.ToSeparator(entity.SaleEntry);
                    report.Dictionary.Variables["SaleExit"].Value = Core.ToSeparator(entity.SaleExit);
                    report.Dictionary.Variables["SaleRemaining"].Value = Core.ToSeparator(SaleRemaining);

                    report.Dictionary.Variables["NumberReturnedFactor"].Value = Core.ToSeparator(entity.NumberReturnedFactor);
                    report.Dictionary.Variables["ReturnedWeight"].Value = entity.ReturnedWeight.ToString();
                    report.Dictionary.Variables["ReturnedEntry"].Value = Core.ToSeparator(entity.ReturnedEntry);
                    report.Dictionary.Variables["ReturnedExit"].Value = Core.ToSeparator(entity.ReturnedExit);
                    report.Dictionary.Variables["ReturnedRemaining"].Value = Core.ToSeparator(ReturnedRemaining);

                    report.Dictionary.Variables["CashEntry"].Value = Core.ToSeparator(entity.CashEntry);
                    report.Dictionary.Variables["CashExit"].Value = Core.ToSeparator(entity.CashExit);
                    report.Dictionary.Variables["CashRemaining"].Value = Core.ToSeparator(CashRemaining);

                    report.Dictionary.Variables["BankName0"].Value = model.report.dailyReportBankList.Count() > 0 ? bankList.FirstOrDefault(x => x.Id == model.report.dailyReportBankList[0].bankId).Name : "";
                    report.Dictionary.Variables["BankEntry0"].Value = model.report.dailyReportBankList.Count() > 0 ? Core.ToSeparator(model.report.dailyReportBankList[0].entry) : "";
                    report.Dictionary.Variables["BankExit0"].Value = model.report.dailyReportBankList.Count() > 0 ? Core.ToSeparator(model.report.dailyReportBankList[0].exit) : "";
                    report.Dictionary.Variables["BankRemaining0"].Value = model.report.dailyReportBankList.Count() > 0 ? Core.ToSeparator(DailyReportBankRemaining0) : "";

                    report.Dictionary.Variables["BankName1"].Value = model.report.dailyReportBankList.Count() > 1 ? bankList.FirstOrDefault(x => x.Id == model.report.dailyReportBankList[1].bankId).Name : "";
                    report.Dictionary.Variables["BankEntry1"].Value = model.report.dailyReportBankList.Count() > 1 ? Core.ToSeparator(model.report.dailyReportBankList[1].entry) : "";
                    report.Dictionary.Variables["BankExit1"].Value = model.report.dailyReportBankList.Count() > 1 ? Core.ToSeparator(model.report.dailyReportBankList[1].exit) : "";
                    report.Dictionary.Variables["BankRemaining1"].Value = model.report.dailyReportBankList.Count() > 1 ? Core.ToSeparator(DailyReportBankRemaining1) : "";

                    report.Dictionary.Variables["BankName2"].Value = model.report.dailyReportBankList.Count() > 2 ? bankList.FirstOrDefault(x => x.Id == model.report.dailyReportBankList[2].bankId).Name : "";
                    report.Dictionary.Variables["BankEntry2"].Value = model.report.dailyReportBankList.Count() > 2 ? Core.ToSeparator(model.report.dailyReportBankList[2].entry) : "";
                    report.Dictionary.Variables["BankExit2"].Value = model.report.dailyReportBankList.Count() > 2 ? Core.ToSeparator(model.report.dailyReportBankList[2].exit) : "";
                    report.Dictionary.Variables["BankRemaining2"].Value = model.report.dailyReportBankList.Count() > 2 ? Core.ToSeparator(DailyReportBankRemaining2) : "";

                    report.Dictionary.Variables["CurrencyName0"].Value = model.report.dailyReportCurrencyList.Count() > 0 ? currencyList.FirstOrDefault(x => x.Id == model.report.dailyReportCurrencyList[0].currencyId).Name : "";
                    report.Dictionary.Variables["CurrencyValue0"].Value = model.report.dailyReportCurrencyList.Count() > 0 ? model.report.dailyReportCurrencyList[0].value.ToString() : "";
                    report.Dictionary.Variables["CurrencyEntry0"].Value = model.report.dailyReportCurrencyList.Count() > 0 ? Core.ToSeparator(model.report.dailyReportCurrencyList[0].rialExit) : "";
                    report.Dictionary.Variables["CurrencyExit0"].Value = model.report.dailyReportCurrencyList.Count() > 0 ? Core.ToSeparator(model.report.dailyReportCurrencyList[0].rialEntry) : "";
                    report.Dictionary.Variables["CurrencyRemaining0"].Value = model.report.dailyReportCurrencyList.Count() > 0 ? Core.ToSeparator(DailyReportCurrencyRemaining0) : "";

                    report.Dictionary.Variables["CurrencyName1"].Value = model.report.dailyReportCurrencyList.Count() > 1 ? currencyList.FirstOrDefault(x => x.Id == model.report.dailyReportCurrencyList[1].currencyId).Name : "";
                    report.Dictionary.Variables["CurrencyValue1"].Value = model.report.dailyReportCurrencyList.Count() > 1 ? model.report.dailyReportCurrencyList[1].value.ToString() : "";
                    report.Dictionary.Variables["CurrencyEntry1"].Value = model.report.dailyReportCurrencyList.Count() > 1 ? Core.ToSeparator(model.report.dailyReportCurrencyList[1].rialExit) : "";
                    report.Dictionary.Variables["CurrencyExit1"].Value = model.report.dailyReportCurrencyList.Count() > 1 ? Core.ToSeparator(model.report.dailyReportCurrencyList[1].rialEntry) : "";
                    report.Dictionary.Variables["CurrencyRemaining1"].Value = model.report.dailyReportCurrencyList.Count() > 1 ? Core.ToSeparator(DailyReportCurrencyRemaining1) : "";

                    report.Dictionary.Variables["GiftNumber"].Value = Core.ToSeparator(entity.GiftNumber);
                    report.Dictionary.Variables["GiftEntry"].Value = Core.ToSeparator(entity.GiftEntry);
                    report.Dictionary.Variables["GiftExit"].Value = Core.ToSeparator(entity.GiftExit);
                    report.Dictionary.Variables["GiftRemaining"].Value = Core.ToSeparator(GiftRemaining);

                    report.Dictionary.Variables["CheckNumber"].Value = Core.ToSeparator(entity.CheckNumber);
                    report.Dictionary.Variables["CheckEntry"].Value = Core.ToSeparator(entity.CheckEntry);
                    report.Dictionary.Variables["CheckExit"].Value = Core.ToSeparator(entity.CheckExit);
                    report.Dictionary.Variables["CheckRemaining"].Value = Core.ToSeparator(CheckRemaining);

                    //report.Dictionary.Variables["LeatherStoneDescription"].Value = entity.LeatherStoneDescription.ToString();
                    report.Dictionary.Variables["LeatherStoneEntry"].Value = Core.ToSeparator(entity.LeatherStoneEntry);
                    report.Dictionary.Variables["LeatherStoneExit"].Value = Core.ToSeparator(entity.LeatherStoneExit);
                    report.Dictionary.Variables["LeatherStoneRemaining"].Value = Core.ToSeparator(LeatherStoneRemaining);

                    //report.Dictionary.Variables["CoinDescription"].Value = entity.CoinDescription;
                    report.Dictionary.Variables["CoinNumber"].Value = Core.ToSeparator(entity.CoinNumber);
                    report.Dictionary.Variables["CoinEntry"].Value = Core.ToSeparator(entity.CoinEntry);
                    report.Dictionary.Variables["CoinExit"].Value = Core.ToSeparator(entity.CoinExit);
                    report.Dictionary.Variables["CoinRemaining"].Value = Core.ToSeparator(CoinRemaining);

                    report.Dictionary.Variables["OtherKiaGoldWeight"].Value = entity.OtherKiaGoldWeight.ToString();
                    report.Dictionary.Variables["OtherKiaGoldEntry"].Value = Core.ToSeparator(entity.OtherKiaGoldEntry);
                    report.Dictionary.Variables["OtherKiaGoldExit"].Value = Core.ToSeparator(entity.OtherKiaGoldExit);
                    report.Dictionary.Variables["OtherKiaGoldRemaining"].Value = Core.ToSeparator(OtherKiaGoldRemaining);

                    report.Dictionary.Variables["OtherGoldWeight"].Value = entity.OtherGoldWeight.ToString();
                    report.Dictionary.Variables["OtherGoldEntry"].Value = Core.ToSeparator(entity.OtherGoldEntry);
                    report.Dictionary.Variables["OtherGoldExit"].Value = Core.ToSeparator(entity.OtherGoldExit);
                    report.Dictionary.Variables["OtherGoldRemaining"].Value = Core.ToSeparator(OtherGoldRemaining);

                    report.Dictionary.Variables["CreditorCustomerEntry"].Value = Core.ToSeparator(entity.CreditorCustomerEntry);
                    report.Dictionary.Variables["CreditorCustomerExit"].Value = Core.ToSeparator(entity.CreditorCustomerExit);
                    report.Dictionary.Variables["CreditorCustomerRemaining"].Value = Core.ToSeparator(CreditorCustomerRemaining);


                    report.Dictionary.Variables["DebtorCustomerEntry"].Value = Core.ToSeparator(entity.DebtorCustomerEntry);
                    report.Dictionary.Variables["DebtorCustomerExit"].Value = Core.ToSeparator(entity.DebtorCustomerExit);
                    report.Dictionary.Variables["DebtorCustomerRemaining"].Value = Core.ToSeparator(DebtorCustomerRemaining);

                    report.Dictionary.Variables["DepositBeforeCount"].Value = Core.ToSeparator(entity.DepositBeforeCount);
                    report.Dictionary.Variables["DepositBeforeEntry"].Value = Core.ToSeparator(entity.DepositBeforeEntry);
                    report.Dictionary.Variables["DepositBeforeExit"].Value = Core.ToSeparator(entity.DepositBeforeExit);
                    report.Dictionary.Variables["DepositBeforeRemaining"].Value = Core.ToSeparator(DepositBeforeRemaining);

                    report.Dictionary.Variables["DepositNewCount"].Value = Core.ToSeparator(entity.DepositNewCount);
                    report.Dictionary.Variables["DepositNewEntry"].Value = Core.ToSeparator(entity.DepositNewEntry);
                    report.Dictionary.Variables["DepositNewExit"].Value = Core.ToSeparator(entity.DepositNewExit);
                    report.Dictionary.Variables["DepositNewRemaining"].Value = Core.ToSeparator(DepositNewRemaining);

                    report.Dictionary.Variables["DiscountEntry"].Value = Core.ToSeparator(entity.DiscountEntry);
                    report.Dictionary.Variables["DiscountExit"].Value = Core.ToSeparator(entity.DepositNewEntry);
                    report.Dictionary.Variables["DiscountRemaining"].Value = Core.ToSeparator(DiscountRemaining);

                    report.Dictionary.Variables["DiscountEntry"].Value = Core.ToSeparator(entity.DiscountEntry);
                    report.Dictionary.Variables["DiscountExit"].Value = Core.ToSeparator(entity.DepositNewEntry);
                    report.Dictionary.Variables["DiscountRemaining"].Value = Core.ToSeparator(DiscountRemaining);

                    report.Dictionary.Variables["CostCourierPostEntry"].Value = Core.ToSeparator(entity.CostCourierPostEntry);
                    report.Dictionary.Variables["CostCourierPostExit"].Value = Core.ToSeparator(entity.CostCourierPostExit);
                    report.Dictionary.Variables["CostCourierPostRemaining"].Value = Core.ToSeparator(CostCourierPostRemaining);

                    report.Dictionary.Variables["CostEntry"].Value = Core.ToSeparator(entity.CostEntry);
                    report.Dictionary.Variables["CostExit"].Value = Core.ToSeparator(entity.CostExit);
                    report.Dictionary.Variables["CostRemaining"].Value = Core.ToSeparator(CostRemaining);


                    report.Dictionary.Variables["Branch"].Value = branchName;
                    report.Dictionary.Variables["Date"].Value = model.date;
                    report.Compile();
                    report.Render(false);

                    StiReport joinedReport = new StiReport();
                    joinedReport.NeedsCompiling = false;
                    joinedReport.IsRendered = true;
                    joinedReport.RenderedPages.Clear();

                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                    MemoryStream stream = new MemoryStream();
                    StiPdfExportSettings settings = new StiPdfExportSettings();
                    StiPdfExportService service = new StiPdfExportService();
                    service.ExportPdf(joinedReport, stream, settings);
                    FileToSend document = new MemoryStream(stream.ToArray()).ToFileToSend("KIA-DailyReport.pdf");
                    Bot = new TelegramBotClient(token);
                    listBotDailyReportUserData.ForEach(x =>
                    {
                        Bot.SendDocumentAsync(x.ChatId, document);
                    });
                }
                if(response.status == 200 && listBranchCalendarId.Count() == listDailyReport.Count())
                {
                    long SaleRial = listDailyReport.Sum(x => x.SaleExit);//فروش
                    int NumberSaleFactor = listDailyReport.Sum(x => x.NumberSaleFactor);// تعداد فاکتور فروش
                    float SaleWeight = listDailyReport.Sum(x => x.SaleWeight);//وزن فروش
                    float ReturnedWeight = listDailyReport.Sum(x => x.ReturnedWeight);//وزن مرجوعی
                    long Discount = listDailyReport.Sum(x => x.DiscountEntry);//تخفیف
                    long Cost = listDailyReport.Sum(x => x.CostEntry);//هزینه
                    long DepositNew = listDailyReport.Sum(x => x.DepositNewExit);//بیعانه جدید
                    long DepositBefore = listDailyReport.Sum(x => x.DepositBeforeEntry);//بیعانه از قبل
                    
                    StiReport report = new StiReport();
                    report.Load(Server.MapPath("~/Report/DailyReport/DailyReportFinal.mrt"));
                    report.Dictionary.Databases.Clear();
                    report.ScriptLanguage = StiReportLanguageType.CSharp;
                    report.Dictionary.Variables["Date"].Value = model.date;
                    report.Dictionary.Variables["SaleRial"].Value = Core.ToSeparator(SaleRial);
                    report.Dictionary.Variables["NumberSaleFactor"].Value = Core.ToSeparator(NumberSaleFactor);
                    report.Dictionary.Variables["SaleWeight"].Value = SaleWeight.ToString();
                    report.Dictionary.Variables["SaleWeightAverage"].Value = (SaleWeight / NumberSaleFactor).ToString(); 
                    report.Dictionary.Variables["ReturnedWeight"].Value = ReturnedWeight.ToString();
                    report.Dictionary.Variables["ReturnedPercent"].Value = (ReturnedWeight * 100 / SaleWeight).ToString();
                    report.Dictionary.Variables["Discount"].Value = Core.ToSeparator(Discount);
                    report.Dictionary.Variables["DiscountPercent"].Value = Core.ToSeparator(Discount * 100 / SaleRial);
                    report.Dictionary.Variables["Cost"].Value = Core.ToSeparator(Cost);
                    report.Dictionary.Variables["CostAverage"].Value = Core.ToSeparator(Cost/ listDailyReport.Count());
                    report.Dictionary.Variables["DepositNew"].Value = Core.ToSeparator(DepositNew);
                    report.Dictionary.Variables["DepositBefore"].Value = Core.ToSeparator(DepositBefore);

                    report.Dictionary.Variables["branch0"].Value = listDailyReport[0].Branch.Name;
                    report.Dictionary.Variables["SaleRial0"].Value = Core.ToSeparator(listDailyReport[0].SaleExit);
                    report.Dictionary.Variables["NumberSaleFactor0"].Value = Core.ToSeparator(listDailyReport[0].NumberSaleFactor);
                    report.Dictionary.Variables["NumberSaleFactorAverage0"].Value = (listDailyReport[0].SaleWeight / listDailyReport[0].NumberSaleFactor).ToString();
                    report.Dictionary.Variables["NumberSaleFactorPercent0"].Value = Core.ToSeparator(listDailyReport[0].NumberSaleFactor * 100 / NumberSaleFactor);
                    report.Dictionary.Variables["SaleWeight0"].Value = listDailyReport[0].SaleWeight.ToString();
                    report.Dictionary.Variables["SaleWeightPercent0"].Value = (listDailyReport[0].SaleWeight * 100 / SaleWeight).ToString();
                    report.Dictionary.Variables["ReturnedWeight0"].Value = listDailyReport[0].ReturnedWeight.ToString();
                    report.Dictionary.Variables["ReturnedPercent0"].Value = (listDailyReport[0].ReturnedWeight * 100 / listDailyReport[0].SaleWeight).ToString();
                    report.Dictionary.Variables["Discount0"].Value = Core.ToSeparator(listDailyReport[0].DiscountEntry);
                    report.Dictionary.Variables["DiscountPercent0"].Value = Core.ToSeparator(listDailyReport[0].DiscountEntry * 100 / listDailyReport[0].SaleExit);
                    report.Dictionary.Variables["Cost0"].Value = Core.ToSeparator(listDailyReport[0].CostEntry);
                    //report.Dictionary.Variables["CostPercent0"].Value = Core.ToSeparator(listDailyReport[0].CostEntry / );
                    report.Dictionary.Variables["DepositNew0"].Value = Core.ToSeparator(listDailyReport[0].DepositNewExit);
                    report.Dictionary.Variables["DepositNewPercent0"].Value = DepositNew!= 0 ? Core.ToSeparator(listDailyReport[0].DepositNewExit * 100 / DepositNew) : "";
                    report.Dictionary.Variables["DepositBefore0"].Value = Core.ToSeparator(listDailyReport[0].DepositBeforeEntry);
                    report.Dictionary.Variables["DepositBeforePercent0"].Value = DepositBefore != 0 ? Core.ToSeparator(listDailyReport[0].DepositBeforeEntry * 100 / DepositBefore) : "";

                    if (listDailyReport.Count() > 1)
                    {
                        report.Dictionary.Variables["branch1"].Value = listDailyReport[1].Branch.Name;
                        report.Dictionary.Variables["SaleRial1"].Value = Core.ToSeparator(listDailyReport[1].SaleExit);
                        report.Dictionary.Variables["NumberSaleFactor1"].Value = Core.ToSeparator(listDailyReport[1].NumberSaleFactor);
                        report.Dictionary.Variables["NumberSaleFactorAverage1"].Value = (listDailyReport[1].SaleWeight / listDailyReport[1].NumberSaleFactor).ToString();
                        report.Dictionary.Variables["NumberSaleFactorPercent1"].Value = Core.ToSeparator(listDailyReport[1].NumberSaleFactor * 100 / NumberSaleFactor);
                        report.Dictionary.Variables["SaleWeight1"].Value = listDailyReport[1].SaleWeight.ToString();
                        report.Dictionary.Variables["SaleWeightPercent1"].Value = (listDailyReport[1].SaleWeight * 100 / SaleWeight).ToString();
                        report.Dictionary.Variables["ReturnedWeight1"].Value = listDailyReport[1].ReturnedWeight.ToString();
                        report.Dictionary.Variables["ReturnedPercent1"].Value = (listDailyReport[1].ReturnedWeight * 100 / listDailyReport[1].SaleWeight).ToString();
                        report.Dictionary.Variables["Discount1"].Value = Core.ToSeparator(listDailyReport[1].DiscountEntry);
                        report.Dictionary.Variables["DiscountPercent1"].Value = Core.ToSeparator(listDailyReport[1].DiscountEntry * 100 / listDailyReport[1].SaleExit);
                        report.Dictionary.Variables["Cost1"].Value = Core.ToSeparator(listDailyReport[1].CostEntry);
                        //report.Dictionary.Variables["CostPercent0"].Value = Core.ToSeparator(listDailyReport[1].CostEntry / );
                        report.Dictionary.Variables["DepositNew1"].Value = Core.ToSeparator(listDailyReport[1].DepositNewExit);
                        report.Dictionary.Variables["DepositNewPercent1"].Value = DepositNew != 0 ? Core.ToSeparator(listDailyReport[1].DepositNewExit * 100 / DepositNew) : "";
                        report.Dictionary.Variables["DepositBefore1"].Value = Core.ToSeparator(listDailyReport[1].DepositBeforeEntry);
                        report.Dictionary.Variables["DepositBeforePercent1"].Value = DepositBefore != 0 ? Core.ToSeparator(listDailyReport[1].DepositBeforeEntry * 100 / DepositBefore) : "";
                    }
                    else
                    {
                        report.Dictionary.Variables["branch1"].Value = "";
                        report.Dictionary.Variables["SaleRial1"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactor1"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorAverage1"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorPercent1"].Value = "";
                        report.Dictionary.Variables["SaleWeight1"].Value = "";
                        report.Dictionary.Variables["SaleWeightPercent1"].Value = "";
                        report.Dictionary.Variables["ReturnedWeight1"].Value = "";
                        report.Dictionary.Variables["ReturnedPercent1"].Value = "";
                        report.Dictionary.Variables["Discount1"].Value = "";
                        report.Dictionary.Variables["DiscountPercent1"].Value = "";
                        report.Dictionary.Variables["Cost1"].Value = "";
                        //report.Dictionary.Variables["CostPercent0"].Value = "";
                        report.Dictionary.Variables["DepositNew1"].Value = "";
                        report.Dictionary.Variables["DepositNewPercent1"].Value = "";
                        report.Dictionary.Variables["DepositBefore1"].Value = "";
                        report.Dictionary.Variables["DepositBeforePercent1"].Value = "";
                    }

                    if (listDailyReport.Count() > 2)
                    {
                        report.Dictionary.Variables["branch2"].Value = listDailyReport[2].Branch.Name;
                        report.Dictionary.Variables["SaleRial2"].Value = Core.ToSeparator(listDailyReport[2].SaleExit);
                        report.Dictionary.Variables["NumberSaleFactor2"].Value = Core.ToSeparator(listDailyReport[2].NumberSaleFactor);
                        report.Dictionary.Variables["NumberSaleFactorAverage2"].Value = (listDailyReport[2].SaleWeight / listDailyReport[2].NumberSaleFactor).ToString();
                        report.Dictionary.Variables["NumberSaleFactorPercent2"].Value = Core.ToSeparator(listDailyReport[2].NumberSaleFactor * 100 / NumberSaleFactor);
                        report.Dictionary.Variables["SaleWeight2"].Value = listDailyReport[2].SaleWeight.ToString();
                        report.Dictionary.Variables["SaleWeightPercent2"].Value = (listDailyReport[2].SaleWeight * 100 / SaleWeight).ToString();
                        report.Dictionary.Variables["ReturnedWeight2"].Value = listDailyReport[2].ReturnedWeight.ToString();
                        report.Dictionary.Variables["ReturnedPercent2"].Value = (listDailyReport[2].ReturnedWeight * 100 / listDailyReport[2].SaleWeight).ToString();
                        report.Dictionary.Variables["Discount2"].Value = Core.ToSeparator(listDailyReport[2].DiscountEntry);
                        report.Dictionary.Variables["DiscountPercent2"].Value = Core.ToSeparator(listDailyReport[2].DiscountEntry * 100 / listDailyReport[2].SaleExit);
                        report.Dictionary.Variables["Cost2"].Value = Core.ToSeparator(listDailyReport[2].CostEntry);
                        //report.Dictionary.Variables["CostPercent2"].Value = Core.ToSeparator(listDailyReport[2].CostEntry / );
                        report.Dictionary.Variables["DepositNew2"].Value = Core.ToSeparator(listDailyReport[2].DepositNewExit);
                        report.Dictionary.Variables["DepositNewPercent2"].Value = DepositNew != 0 ? Core.ToSeparator(listDailyReport[2].DepositNewExit * 100 / DepositNew) : "";
                        report.Dictionary.Variables["DepositBefore2"].Value = Core.ToSeparator(listDailyReport[2].DepositBeforeEntry);
                        report.Dictionary.Variables["DepositBeforePercent2"].Value = DepositBefore != 0 ? Core.ToSeparator(listDailyReport[2].DepositBeforeEntry * 100 / DepositBefore) : "";
                    }
                    else
                    {
                        report.Dictionary.Variables["branch2"].Value = "";
                        report.Dictionary.Variables["SaleRial2"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactor2"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorAverage2"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorPercent2"].Value = "";
                        report.Dictionary.Variables["SaleWeight2"].Value = "";
                        report.Dictionary.Variables["SaleWeightPercent2"].Value = "";
                        report.Dictionary.Variables["ReturnedWeight2"].Value = "";
                        report.Dictionary.Variables["ReturnedPercent2"].Value = "";
                        report.Dictionary.Variables["Discount2"].Value = "";
                        report.Dictionary.Variables["DiscountPercent2"].Value = "";
                        report.Dictionary.Variables["Cost2"].Value = "";
                        //report.Dictionary.Variables["CostPercent2"].Value = "";
                        report.Dictionary.Variables["DepositNew2"].Value = "";
                        report.Dictionary.Variables["DepositNewPercent2"].Value = "";
                        report.Dictionary.Variables["DepositBefore2"].Value = "";
                        report.Dictionary.Variables["DepositBeforePercent2"].Value = "";
                    }

                    if (listDailyReport.Count() > 3)
                    {
                        report.Dictionary.Variables["branch3"].Value = listDailyReport[3].Branch.Name;
                        report.Dictionary.Variables["SaleRial3"].Value = Core.ToSeparator(listDailyReport[3].SaleExit);
                        report.Dictionary.Variables["NumberSaleFactor3"].Value = Core.ToSeparator(listDailyReport[3].NumberSaleFactor);
                        report.Dictionary.Variables["NumberSaleFactorAverage3"].Value = (listDailyReport[3].SaleWeight / listDailyReport[3].NumberSaleFactor).ToString();
                        report.Dictionary.Variables["NumberSaleFactorPercent3"].Value = Core.ToSeparator(listDailyReport[3].NumberSaleFactor * 100 / NumberSaleFactor);
                        report.Dictionary.Variables["SaleWeight3"].Value = listDailyReport[3].SaleWeight.ToString();
                        report.Dictionary.Variables["SaleWeightPercent3"].Value = (listDailyReport[3].SaleWeight * 100 / SaleWeight).ToString();
                        report.Dictionary.Variables["ReturnedWeight3"].Value = listDailyReport[3].ReturnedWeight.ToString();
                        report.Dictionary.Variables["ReturnedPercent3"].Value = (listDailyReport[3].ReturnedWeight * 100 / listDailyReport[3].SaleWeight).ToString();
                        report.Dictionary.Variables["Discount3"].Value = Core.ToSeparator(listDailyReport[3].DiscountEntry);
                        report.Dictionary.Variables["DiscountPercent3"].Value = Core.ToSeparator(listDailyReport[3].DiscountEntry * 100 / listDailyReport[3].SaleExit);
                        report.Dictionary.Variables["Cost3"].Value = Core.ToSeparator(listDailyReport[3].CostEntry);
                        //report.Dictionary.Variables["CostPercent3"].Value = Core.ToSeparator(listDailyReport[3].CostEntry / );
                        report.Dictionary.Variables["DepositNew3"].Value = Core.ToSeparator(listDailyReport[3].DepositNewExit);
                        report.Dictionary.Variables["DepositNewPercent3"].Value = DepositNew != 0 ? Core.ToSeparator(listDailyReport[3].DepositNewExit * 100 / DepositNew) : "";
                        report.Dictionary.Variables["DepositBefore3"].Value = Core.ToSeparator(listDailyReport[3].DepositBeforeEntry);
                        report.Dictionary.Variables["DepositBeforePercent3"].Value = DepositBefore != 0 ? Core.ToSeparator(listDailyReport[3].DepositBeforeEntry * 100 / DepositBefore) : "";
                    }
                    else
                    {
                        report.Dictionary.Variables["branch3"].Value = "";
                        report.Dictionary.Variables["SaleRial3"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactor3"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorAverage3"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorPercent3"].Value = "";
                        report.Dictionary.Variables["SaleWeight3"].Value = "";
                        report.Dictionary.Variables["SaleWeightPercent3"].Value = "";
                        report.Dictionary.Variables["ReturnedWeight3"].Value = "";
                        report.Dictionary.Variables["ReturnedPercent3"].Value = "";
                        report.Dictionary.Variables["Discount3"].Value = "";
                        report.Dictionary.Variables["DiscountPercent3"].Value = "";
                        report.Dictionary.Variables["Cost3"].Value = "";
                        //report.Dictionary.Variables["CostPercent3"].Value = "";
                        report.Dictionary.Variables["DepositNew3"].Value = "";
                        report.Dictionary.Variables["DepositNewPercent3"].Value = "";
                        report.Dictionary.Variables["DepositBefore3"].Value = "";
                        report.Dictionary.Variables["DepositBeforePercent3"].Value = "";
                    }

                    if (listDailyReport.Count() > 4)
                    {
                        report.Dictionary.Variables["branch4"].Value = listDailyReport[4].Branch.Name;
                        report.Dictionary.Variables["SaleRial4"].Value = Core.ToSeparator(listDailyReport[4].SaleExit);
                        report.Dictionary.Variables["NumberSaleFactor4"].Value = Core.ToSeparator(listDailyReport[4].NumberSaleFactor);
                        report.Dictionary.Variables["NumberSaleFactorAverage4"].Value = (listDailyReport[4].SaleWeight / listDailyReport[4].NumberSaleFactor).ToString();
                        report.Dictionary.Variables["NumberSaleFactorPercent4"].Value = Core.ToSeparator(listDailyReport[4].NumberSaleFactor * 100 / NumberSaleFactor);
                        report.Dictionary.Variables["SaleWeight4"].Value = listDailyReport[4].SaleWeight.ToString();
                        report.Dictionary.Variables["SaleWeightPercent4"].Value = (listDailyReport[4].SaleWeight * 100 / SaleWeight).ToString();
                        report.Dictionary.Variables["ReturnedWeight4"].Value = listDailyReport[4].ReturnedWeight.ToString();
                        report.Dictionary.Variables["ReturnedPercent4"].Value = (listDailyReport[4].ReturnedWeight * 100 / listDailyReport[4].SaleWeight).ToString();
                        report.Dictionary.Variables["Discount4"].Value = Core.ToSeparator(listDailyReport[4].DiscountEntry);
                        report.Dictionary.Variables["DiscountPercent4"].Value = Core.ToSeparator(listDailyReport[4].DiscountEntry * 100 / listDailyReport[4].SaleExit);
                        report.Dictionary.Variables["Cost4"].Value = Core.ToSeparator(listDailyReport[4].CostEntry);
                        //report.Dictionary.Variables["CostPercent4"].Value = Core.ToSeparator(listDailyReport[4].CostEntry / );
                        report.Dictionary.Variables["DepositNew4"].Value = Core.ToSeparator(listDailyReport[4].DepositNewExit);
                        report.Dictionary.Variables["DepositNewPercent4"].Value = DepositNew != 0 ? Core.ToSeparator(listDailyReport[4].DepositNewExit * 100 / DepositNew) : "";
                        report.Dictionary.Variables["DepositBefore4"].Value = Core.ToSeparator(listDailyReport[4].DepositBeforeEntry);
                        report.Dictionary.Variables["DepositBeforePercent4"].Value = DepositBefore != 0 ? Core.ToSeparator(listDailyReport[4].DepositBeforeEntry * 100 / DepositBefore) : "";
                    }
                    else
                    {
                        report.Dictionary.Variables["branch4"].Value = "";
                        report.Dictionary.Variables["branch4"].Value = "";
                        report.Dictionary.Variables["SaleRial4"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactor4"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorAverage4"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorPercent4"].Value = "";
                        report.Dictionary.Variables["SaleWeight4"].Value = "";
                        report.Dictionary.Variables["SaleWeightPercent4"].Value = "";
                        report.Dictionary.Variables["ReturnedWeight4"].Value = "";
                        report.Dictionary.Variables["ReturnedPercent4"].Value = "";
                        report.Dictionary.Variables["Discount4"].Value = "";
                        report.Dictionary.Variables["DiscountPercent4"].Value = "";
                        report.Dictionary.Variables["Cost4"].Value = "";
                        //report.Dictionary.Variables["CostPercent4"].Value =  "";
                        report.Dictionary.Variables["DepositNew4"].Value = "";
                        report.Dictionary.Variables["DepositNewPercent4"].Value = "";
                        report.Dictionary.Variables["DepositBefore4"].Value = "";
                        report.Dictionary.Variables["DepositBeforePercent4"].Value = "";
                    }

                    if (listDailyReport.Count() > 5)
                    {
                        report.Dictionary.Variables["branch5"].Value = listDailyReport[5].Branch.Name;
                        report.Dictionary.Variables["SaleRial5"].Value = Core.ToSeparator(listDailyReport[5].SaleExit);
                        report.Dictionary.Variables["NumberSaleFactor5"].Value = Core.ToSeparator(listDailyReport[5].NumberSaleFactor);
                        report.Dictionary.Variables["NumberSaleFactorAverage5"].Value = (listDailyReport[5].SaleWeight / listDailyReport[5].NumberSaleFactor).ToString();
                        report.Dictionary.Variables["NumberSaleFactorPercent5"].Value = Core.ToSeparator(listDailyReport[5].NumberSaleFactor * 100 / NumberSaleFactor);
                        report.Dictionary.Variables["SaleWeight5"].Value = listDailyReport[5].SaleWeight.ToString();
                        report.Dictionary.Variables["SaleWeightPercent5"].Value = (listDailyReport[5].SaleWeight * 100 / SaleWeight).ToString();
                        report.Dictionary.Variables["ReturnedWeight5"].Value = listDailyReport[5].ReturnedWeight.ToString();
                        report.Dictionary.Variables["ReturnedPercent5"].Value = (listDailyReport[5].ReturnedWeight * 100 / listDailyReport[4].SaleWeight).ToString();
                        report.Dictionary.Variables["Discount5"].Value = Core.ToSeparator(listDailyReport[5].DiscountEntry);
                        report.Dictionary.Variables["DiscountPercent5"].Value = Core.ToSeparator(listDailyReport[5].DiscountEntry * 100 / listDailyReport[4].SaleExit);
                        report.Dictionary.Variables["Cost5"].Value = Core.ToSeparator(listDailyReport[5].CostEntry);
                        //report.Dictionary.Variables["CostPercent4"].Value = Core.ToSeparator(listDailyReport[5].CostEntry / );
                        report.Dictionary.Variables["DepositNew5"].Value = Core.ToSeparator(listDailyReport[5].DepositNewExit);
                        report.Dictionary.Variables["DepositNewPercent5"].Value = DepositNew != 0 ? Core.ToSeparator(listDailyReport[5].DepositNewExit * 100 / DepositNew) : "";
                        report.Dictionary.Variables["DepositBefore5"].Value = Core.ToSeparator(listDailyReport[5].DepositBeforeEntry);
                        report.Dictionary.Variables["DepositBeforePercent5"].Value = DepositBefore != 0 ? Core.ToSeparator(listDailyReport[5].DepositBeforeEntry * 100 / DepositBefore) : "";
                    }
                    else
                    {
                        report.Dictionary.Variables["branch5"].Value = "";
                        report.Dictionary.Variables["SaleRial5"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactor5"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorAverage5"].Value = "";
                        report.Dictionary.Variables["NumberSaleFactorPercent5"].Value = "";
                        report.Dictionary.Variables["SaleWeight5"].Value = "";
                        report.Dictionary.Variables["SaleWeightPercent5"].Value = "";
                        report.Dictionary.Variables["ReturnedWeight5"].Value = "";
                        report.Dictionary.Variables["ReturnedPercent5"].Value = "";
                        report.Dictionary.Variables["Discount5"].Value = "";
                        report.Dictionary.Variables["DiscountPercent5"].Value = "";
                        report.Dictionary.Variables["Cost5"].Value = "";
                        //report.Dictionary.Variables["CostPercent4"].Value = "";
                        report.Dictionary.Variables["DepositNew5"].Value = "";
                        report.Dictionary.Variables["DepositNewPercent5"].Value = "";
                        report.Dictionary.Variables["DepositBefore5"].Value = "";
                        report.Dictionary.Variables["DepositBeforePercent5"].Value = "";
                    }

                    report.Compile();
                    report.Render(false);

                    StiReport joinedReport = new StiReport();
                    joinedReport.NeedsCompiling = false;
                    joinedReport.IsRendered = true;
                    joinedReport.RenderedPages.Clear();

                    foreach (StiPage page in report.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport;
                        page.NewGuid();
                        joinedReport.RenderedPages.Add(page);
                    }
                    MemoryStream stream = new MemoryStream();
                    StiPdfExportSettings settings = new StiPdfExportSettings();
                    StiPdfExportService service = new StiPdfExportService();
                    service.ExportPdf(joinedReport, stream, settings);
                    FileToSend document = new MemoryStream(stream.ToArray()).ToFileToSend("KIA-DailyReportFinal.pdf");
                    Bot = new TelegramBotClient(token);
                    listBotDailyReportUserData.Where(x=> x.BranchId == 19).ToList().ForEach(x =>
                    {
                        Bot.SendDocumentAsync(x.ChatId, document);
                    });
                    //chart
                    StiReport report2 = new StiReport();
                    DataSet dataset = new DataSet("DataSource");
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Row");
                    dataTable.Columns.Add("Sale");
                    dataTable.Columns.Add("Branch");
                    int counter = 0;
                    foreach (var item in listDailyReport)
                    {
                        DataRow row = dataTable.NewRow();
                        row["Row"] = ++counter;
                        row["Sale"] = item.SaleExit;
                        row["Branch"] = item.Branch.Name;
                        dataTable.Rows.Add(row);
                    }
                    dataset.Tables.Add(dataTable);
                    report2.Load(Server.MapPath("~/Report/DailyReport/Chart.mrt"));
                    report2.Dictionary.Databases.Clear();
                    report2.Dictionary.Variables["Date"].Value = model.date;
                    report2.ScriptLanguage = StiReportLanguageType.CSharp;
                    report2.RegData("DataSource", dataset.Tables[0].DefaultView);
                    report2.Compile();
                    report2.Render(false);

                    StiReport joinedReport2 = new StiReport();
                    joinedReport2.NeedsCompiling = false;
                    joinedReport2.IsRendered = true;
                    joinedReport2.RenderedPages.Clear();

                    foreach (StiPage page in report2.CompiledReport.RenderedPages)
                    {
                        page.Report = joinedReport2;
                        page.NewGuid();
                        joinedReport2.RenderedPages.Add(page);
                    }
                    MemoryStream stream2 = new MemoryStream();
                    StiPdfExportSettings settings2 = new StiPdfExportSettings();
                    StiPdfExportService service2 = new StiPdfExportService();
                    service.ExportPdf(joinedReport2, stream2, settings2);
                    FileToSend document2 = new MemoryStream(stream2.ToArray()).ToFileToSend("KIA-Chart.pdf");
                    Bot = new TelegramBotClient(token);
                    listBotDailyReportUserData.Where(x => x.BranchId == 19).ToList().ForEach(x =>
                    {
                        Bot.SendDocumentAsync(x.ChatId, document2);
                    });
                }
            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }
            return response;
        }

        /// <summary>
        /// خواندن اطلاعات گزارش روزانه
        /// </summary>
        /// <param name="date">تاریخ اطلاعات جهت خواندن اطلاعات</param>
        /// <param name="token">توکن اطلاعات جهت خواندن اطلاعات</param>
        /// <returns>جیسون اطلاعات لود شده گزارش روزانه</returns>
        private Response Load(string date, string token)
        {
            Response response;
            try
            {
                using (var db = new KiaGalleryContext())
                {
                    var user = db.Token.Where(x => x.Voided == false && x.Code == token).Select(x => x.User).SingleOrDefault(); // بررسی اعتبار توکن

                    if (user != null)
                    {
                        var dateTime = DateUtility.GetDateTime(date);
                        var calendar = db.BranchCalendar.Where(x => x.BranchId == user.BranchId && x.ReportDate == dateTime).SingleOrDefault(); // بررسی اعتبار تاریخ وارد شده
                        if (calendar != null)
                        {
                            var entity = db.DailyReport.Where(x => x.BranchId == user.BranchId && x.BranchCalendarId == calendar.Id).SingleOrDefault();
                            if (entity != null)
                            {
                                DailyReportViewModel model = new DailyReportViewModel()
                                {
                                    numberSaleFactor = entity.NumberSaleFactor,
                                    saleWeight = entity.SaleWeight,
                                    saleEntry = entity.SaleEntry,
                                    saleExit = entity.SaleExit,

                                    numberReturnedFactor = entity.NumberReturnedFactor,
                                    returnedWeight = entity.ReturnedWeight,
                                    returnedEntry = entity.ReturnedEntry,
                                    returnedExit = entity.ReturnedExit,

                                    cashEntry = entity.CashEntry,
                                    cashExit = entity.CashExit,

                                    giftNumber = entity.GiftNumber,
                                    giftEntry = entity.GiftEntry,
                                    giftExit = entity.GiftExit,

                                    checkNumber = entity.CheckNumber,
                                    checkEntry = entity.CheckEntry,
                                    checkExit = entity.CheckExit,


                                    leatherStoneDescription = entity.LeatherStoneDescription,
                                    leatherStoneEntry = entity.LeatherStoneEntry,
                                    leatherStoneExit = entity.LeatherStoneExit,

                                    coinNumber = entity.CoinNumber,
                                    coinDescription = entity.CoinDescription,
                                    coinEntry = entity.CoinEntry,
                                    coinExit = entity.CoinExit,

                                    otherKiaGoldWeight = entity.OtherKiaGoldWeight,
                                    otherKiaGoldEntry = entity.OtherKiaGoldEntry,
                                    otherKiaGoldExit = entity.OtherKiaGoldExit,

                                    otherGoldEntry = entity.OtherGoldEntry,
                                    otherGoldExit = entity.OtherGoldExit,

                                    creditorCustomerEntry = entity.CreditorCustomerEntry,
                                    creditorCustomerExit = entity.CreditorCustomerExit,

                                    debtorCustomerEntry = entity.DebtorCustomerEntry,
                                    debtorCustomerExit = entity.DebtorCustomerExit,

                                    depositBeforeCount = entity.DepositBeforeCount,
                                    depositBeforeEntry = entity.DepositBeforeEntry,
                                    depositBeforeExit = entity.DepositBeforeExit,

                                    depositNewCount = entity.DepositNewCount,
                                    depositNewEntry = entity.DepositNewEntry,
                                    depositNewExit = entity.DepositNewExit,

                                    discountEntry = entity.DiscountEntry,
                                    discountExit = entity.DiscountExit,

                                    costCourierPostEntry = entity.CostCourierPostEntry,
                                    costCourierPostExit = entity.CostCourierPostExit,

                                    costEntry = entity.CostEntry,
                                    costExit = entity.CostExit,

                                    dailyReportBankList = entity.DailyReportBankList.Select(x => new DailyReportBankViewModel()
                                    {
                                        bankId = x.BankId,
                                        entry = x.Entry,
                                        exit = x.Exit
                                    }).ToList(),

                                    dailyReportCurrencyList = entity.DailyReportCurrencyList.Select(x => new DailyReportCurrencyViewModel()
                                    {
                                        currencyId = x.CurrencyId,
                                        value = x.Value,
                                        rialValue = x.RialValue,
                                        rialEntry = x.RialEntry,
                                        rialExit = x.RialExit
                                    }).ToList(),
                                };
                                response = new Response
                                {
                                    status = 200,
                                    data = model
                                };
                            }
                            else
                            {
                                response = new Response()
                                {
                                    status = 500,
                                    message = "گزارش روزانه در این تاریخ ثبت نشده."
                                };
                            }
                        }
                        else
                        {
                            response = new Response()
                            {
                                status = 500,
                                message = "گزارش روزانه در این تاریخ ثبت نشده."
                            };
                        }
                    }
                    else
                    {
                        response = new Response
                        {
                            status = 403,
                            message = "توکن ارسال شده معتبر نمی باشد."
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                response = Core.GetExceptionResponse(ex);
            }

            return response;
        }
        
    }
}