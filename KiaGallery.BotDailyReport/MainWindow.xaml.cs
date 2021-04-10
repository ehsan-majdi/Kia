using KiaGallery.BotDailyReport.Data;
using KiaGallery.Common;
using KiaGallery.Model;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace KiaGallery.BotDailyReport
{
    public partial class MainWindow : Window
    {
        #region Timer
        private string _currenttime;
        private TimeZoneInfo _selectedTimeZone;
        public List<TimeZoneInfo> TimeZones
        {
            get { return TimeZoneInfo.GetSystemTimeZones().ToList(); }
        }

        public string CurrentTime
        {
            get { return _currenttime; }
            set { _currenttime = value; OnPropertyChanged("CurrentTime"); }
        }

        public TimeZoneInfo SelectedTimeZone
        {
            get { return _selectedTimeZone; }
            set
            {
                _selectedTimeZone = value;
                OnPropertyChanged("SelectedTimeZone");
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            CurrentTime = SelectedTimeZone == null
                   ? DateTime.Now.ToLongTimeString()
                   : DateTime.UtcNow.AddHours(SelectedTimeZone.BaseUtcOffset.TotalHours).ToLongTimeString();

            txtTime.Text = CurrentTime + " / " + dailyTime;
            if (CurrentTime == dailyTime)
            {
                Task.Factory.StartNew(() =>
                {
                    makeFinalReport();
                });
            }

            //if (CurrentTime == goldReport1 || CurrentTime == goldReport2 || CurrentTime == goldReport3)
            //{
            //    Task.Factory.StartNew(() =>
            //    {
            //        sendGoldReport();
            //    });
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion

        string dailyTime = "11:45:00 PM";
        string goldReport1 = "12:30:00 PM";
        string goldReport2 = "3:30:00 PM";
        string goldReport3 = "6:30:00 PM";

        private bool Running;
        private TelegramBotClient Bot;
        private int Offset;

        List<DailyReportSettings> Settings;

        public MainWindow()
        {
            InitializeComponent();
            GetSettings();
            TxtApiCode.Text = Settings.Single(x => x.Key == "token").Value;

            #region Timer
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.IsEnabled = true;
            timer.Tick += (s, e) =>
            {
                UpdateTime();
            };
            #endregion
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            Bot = new TelegramBotClient(TxtApiCode.Text.Trim());
            BtnConnect.IsEnabled = false;
            BtnDisconnect.IsEnabled = true;
            LblStatus.Text = "Connecting...";
            Running = true;
            GetMe();
            Task.Factory.StartNew(() =>
            {
                GetUpdates();
            });
        }

        private async void GetMe()
        {
            var me = await Bot.GetMeAsync();
            LblStatus.Text = me.FirstName + " (" + me.Username + ")";
        }

        private async void GetUpdates()
        {
            Offset = int.Parse(Settings.Single(x => x.Key == "last-offset").Value);
            long i = 0;

            //makeFinalReportTask();

            while (Running)
            {
                try
                {
                    if (i % 10 == 0)
                    {
                        sendCompleteReport();
                    }
                    var updates = await Bot.GetUpdatesAsync(Offset);
                    foreach (var item in updates)
                    {
                        try
                        {
                            if (item.Type == UpdateType.Message)
                                ResponseMessage(item);
                            if (item.Type == UpdateType.CallbackQuery)
                                ResponseCallback(item);
                        }
                        catch (Exception ex)
                        { }
                        Offset = item.Id + 1;
                    }
                    UpdateOffset(Offset);
                    await Task.Delay(1000);
                    i++;
                }
                catch (Exception)
                { }
            }
        }

        private void sendCompleteReport()
        {
            using (var db = new KiaGalleryMainEntities())
            {
                var report = db.DailyReport.Where(x => x.Sent == false && x.Status == (int)CalendarStatus.Submit).ToList();


                report.ForEach(x =>
                {
                    var date = DateUtility.GetPersianDate(x.BranchCalendar.ReportDate);
                    var userList = db.BotDailyReportUserData.Where(y => y.BotUserType == (int)BotDailyReportUserType.Admin || y.BotUserType == (int)BotDailyReportUserType.Branch).ToList();
                    userList.ForEach(user =>
                    {
                        if (user.BotUserType == (int)BotDailyReportUserType.Admin || user.BranchId.Split('-').Select(z => int.Parse(z)).ToArray().Contains(x.BranchId))
                        {
                            var file = MakeReport(x, date);
                            InputOnlineFile document = new InputOnlineFile(new MemoryStream(file), (x.Branches.Alias + " " + date.Replace("/", "-") + ".pdf").Trim());
                            var caption = "🏠 شعبه " + x.Branches.Name;
                            caption += "\n" + (x.Status == 2 ? "✅" : "❌") + " " + Enums.GetTitle((CalendarStatus)x.Status);
                            caption += "\n📆 تاریخ: " + date;

                            caption += "\n💵 فروش: " + (x.SaleExit - x.ReturnedEntry - x.OtherKiaGoldEntry).ToString("N0") + " ریال";
                            caption += "\n⚖️ وزن: " + (x.SaleWeight - x.ReturnedWeight - x.OtherKiaGoldWeight) + " گرم";

                            if (x.DailyReportLog.Count(y => y.Status == (int)CalendarStatus.Submit) > 1)
                            {
                                caption += "\n📝 ویرایش شده";
                            }

                            Bot.SendDocumentAsync(user.ChatId, document, caption);
                        }
                    });

                    x.Sent = true;
                });

                db.SaveChanges();
            }
        }

        private void makeFinalReportTask()
        {
            //Task.Factory.StartNew(() =>
            //{
            //    var timeParts = dailyTime.Split(new char[1] { ':' }).Select(x => int.Parse(x)).ToArray();
            //    while (true)
            //    {
            //        var dateNow = DateTime.Now;
            //        var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, timeParts[0], timeParts[1], timeParts[2]);
            //        TimeSpan ts;
            //        if (date > dateNow)
            //        {
            //            ts = date - dateNow;
            //        }
            //        else
            //        {
            //            date = date.AddDays(1);
            //            ts = date - dateNow;
            //        }
            //        Task.Delay(ts).Wait();
            //        makeFinalReport();
            //    }
            //});
        }

        private void makeFinalReport()
        {
            using (var db = new KiaGalleryMainEntities())
            {
                var calendar = db.BranchCalendar.Where(x => x.ReportDate == DateTime.Today).Select(x => new
                {
                    x.BranchId,
                    x.Branches.Name
                }).ToList();

                var branchList = calendar.Select(x => x.BranchId).ToList();

                var reportList = db.DailyReport.Where(x => x.BranchCalendar.ReportDate == DateTime.Today && branchList.Any(y => y == x.BranchId)).ToList();

                var user = db.BotDailyReportUserData.Where(x => x.BotUserType == (int)BotDailyReportUserType.Admin).ToList();
                var date = DateUtility.GetPersianDate(DateTime.Today);
                var file = MakeTotalReport(DateTime.Today);
                user.ForEach(x =>
                {
                    InputOnlineFile document = new InputOnlineFile(new MemoryStream(file.data), ("Total " + date.Replace("/", "-") + ".pdf").Trim());
                    var caption = "گزارش خودکار:\n";
                    caption += "🏠 شعب:\n";
                    var branchNameList = reportList.Select(y => "✅ " + y.Branches.Name).ToList();
                    caption += string.Join("\n", branchNameList);

                    var sentBranchList = reportList.Select(y => y.BranchId).ToList();
                    var notSentBranchList = calendar.Where(y => !sentBranchList.Any(z => y.BranchId == z)).Select(y => y.Name).ToList();
                    var notSentBranchNameList = notSentBranchList.Select(y => "⚠️ " + y).ToList();
                    if (notSentBranchNameList.Count > 0)
                    {
                        caption += "\n";
                        caption += string.Join("\n", notSentBranchNameList);
                    }

                    caption += "\n📆 تاریخ: " + date;

                    caption += "\n💵 فروش: " + file.totalPrice.ToString("N0") + " ریال";
                    caption += "\n⚖️ وزن: " + file.totalWeight + " گرم";

                    var task = Bot.SendDocumentAsync(x.ChatId, document, caption);
                    task.Wait();
                });
            }
        }

        private void sendGoldReport()
        {
            using (var db = new KiaGalleryMainEntities())
            {
                var data = db.BranchGolds.Where(x => x.CreateDate >= DateTime.Today).GroupBy(x => x.Branches).Select(y => new
                {
                    weight = y.OrderByDescending(x => x.Id).Select(x => x.Weight).FirstOrDefault(),
                    price = y.OrderByDescending(x => x.Id).Select(x => x.Price).FirstOrDefault(),
                    branch = y.Key.Name
                }).ToList();

                var persianDate = DateUtility.GetPersianDate(DateTime.Now);
                double sum = data.Sum(x => x.weight);
                long priceSum = data.Sum(x => x.price);
                var rate = priceSum / sum * 4.3318;
                string message = persianDate + " \n جمع کل: " + sum + " گرم " + " \n جمع قیمت: " + Core.ToSeparator(priceSum) + " ریال " + " \n  میانگین مظنه: " + Core.ToSeparator(rate) + " ریال " + "\n";

                foreach (var item in data)
                {
                    message += "شعبه " + item.branch + ": " + item.weight + " گرم " + Core.ToSeparator(item.price) + " ریال \n";
                }

                var user = db.BotDailyReportUserData.Where(x => x.BotUserType == (int)BotDailyReportUserType.Admin).ToList();
                user.ForEach(x =>
                {
                    var task = Bot.SendTextMessageAsync(x.ChatId, message.Trim());
                    task.Wait();
                });
            }
        }

        private void ResponseMessage(Update item)
        {
            var Chat = item.Message.Chat;
            BotDailyReportUserData user = new BotDailyReportUserData()
            {
                UserType = (int)item.Message.Chat.Type,
                UserId = item.Message.From.Id,
                ChatId = Chat.Id,
                FirstName = Chat.FirstName,
                LastName = Chat.LastName,
                Username = Chat.Username,
                ModifyUserId = 1,
                Stoped = false
            };
            user = AddUser(user);

            if (user.BotUserType == (int)BotDailyReportUserType.None)
            {
                Bot.SendTextMessageAsync(Chat.Id, "در حال حاضر شما نمی توانید از سیستم استفاده کنید");
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, "در حال حاضر شما نمی توانید از سیستم استفاده کنید", item.Message.Date, false, null);
                return;
            }

            var TextMessage = item.Message.Text?.Trim();
            if (TextMessage == "/start" || TextMessage == "/keyboard" || TextMessage == "🔚 بازگشت")
            {
                var mainKeyboard = new ReplyKeyboardMarkup();
                mainKeyboard.ResizeKeyboard = true;
                if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                {
                    mainKeyboard.Keyboard = new KeyboardButton[][] {
                        new KeyboardButton[] { "📋 گزارش تجمیعی", "📆 گزارش تفصیلی" }
                    };
                }
                else
                {
                    mainKeyboard.Keyboard = new KeyboardButton[][] {
                        new KeyboardButton[] { "📆 گزارش تفصیلی" }
                    };
                }

                Bot.SendTextMessageAsync(Chat.Id, "منوی اصلی", ParseMode.Html, false, false, 0, mainKeyboard);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
            }
            else if (TextMessage == "📋 گزارش تجمیعی")
            {
                List<string> dateList = new List<string>();
                for (int i = 0; i < 4; i++)
                {
                    dateList.Add(DateUtility.GetPersianDate(DateTime.Now.AddDays(-1 * i)));
                }

                var keyboard = GenerateKeyBoard(dateList, 2, true, true);

                Bot.SendTextMessageAsync(Chat.Id, "تاریخ مورد نظر را انتخاب کنید:", ParseMode.Html, false, false, 0, keyboard);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
            }
            else if (TextMessage == "📆 گزارش تفصیلی")
            {
                List<string> dateList = new List<string>();
                for (int i = 0; i < 4; i++)
                {
                    dateList.Add(DateUtility.GetPersianDate(DateTime.Now.AddDays(-1 * i)));
                }

                var keyboard = GenerateKeyBoard(dateList, 2, true, true);

                Bot.SendTextMessageAsync(Chat.Id, "تاریخ مورد نظر را انتخاب کنید:", ParseMode.Html, false, false, 0, keyboard);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
            }
            else if (TextMessage == "بازه تاریخ")
            {
                Bot.SendTextMessageAsync(Chat.Id, "تاریخ شروع را وارد کنید:", ParseMode.Html);
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
            }
            else if (TextMessage == "همه")
            {
                using (var db = new KiaGalleryMainEntities())
                {
                    var typeMessage = db.DailyReportMessage.Where(x => x.ChatId == user.ChatId && (x.Text == "📆 گزارش تفصیلی" || x.Text == "📋 گزارش تجمیعی")).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                    if (typeMessage.Text == "📋 گزارش تجمیعی")
                    {
                        var messageItem = db.DailyReportMessage.Where(x => x.ChatId == user.ChatId && DbFunctions.Like(x.Text, "____/__/__")).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                        if (messageItem != null)
                        {
                            var date = DateUtility.GetDateTime(messageItem.Text);

                            List<DailyReport> data = null;
                            if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                            {
                                data = db.DailyReport.Where(x => x.BranchCalendar.ReportDate == date).ToList();
                            }
                            else
                            {
                                var branchList = user.BranchId.Split('-').Select(x => int.Parse(x)).ToList();
                                data = db.DailyReport.Where(x => x.BranchCalendar.ReportDate == date && branchList.Any(y => y == x.BranchId)).ToList();
                            }

                            if (data.Count > 0)
                            {
                                foreach (var itemDailyReport in data)
                                {
                                    var file = MakeReport(itemDailyReport, messageItem.Text);
                                    InputOnlineFile document = new InputOnlineFile(new MemoryStream(file), (itemDailyReport.Branches.Alias + " " + messageItem.Text.Replace("/", "-") + ".pdf").Trim());
                                    var caption = "🏠 شعبه " + itemDailyReport.Branches.Name;
                                    caption += "\n" + (itemDailyReport.Status == 2 ? "✅" : "❌") + " " + Enums.GetTitle((CalendarStatus)itemDailyReport.Status);
                                    caption += "\n📆 تاریخ: " + messageItem.Text;

                                    caption += "\n💵 فروش: " + (itemDailyReport.SaleExit - itemDailyReport.ReturnedEntry - itemDailyReport.OtherKiaGoldEntry).ToString("N0") + " ریال"; ;
                                    caption += "\n⚖️ وزن: " + (itemDailyReport.SaleWeight - itemDailyReport.ReturnedWeight - itemDailyReport.OtherKiaGoldWeight) + " گرم";

                                    Bot.SendDocumentAsync(user.ChatId, document, caption);
                                }
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(user.ChatId, "در تاریخ انتخاب شده گزارشی ثبت نشده است");
                            }
                        }

                    }
                    else if (typeMessage.Text == "📆 گزارش تفصیلی")
                    {
                        var messageList = db.DailyReportMessage.Where(x => x.ChatId == user.ChatId && x.Id > typeMessage.Id).OrderBy(x => x.CreateDate).ToList();

                        var fromDate = DateUtility.GetDateTime(messageList[messageList.FindIndex(x => x.Text == "تاریخ پایان را وارد کنید:") - 1].Text).Value;
                        var toDate = DateUtility.GetDateTime(messageList[messageList.FindIndex(x => x.Text == "شعبه مورد نظر را انتخاب کنید:") - 1].Text).Value;

                        var mainKeyboard = new ReplyKeyboardMarkup();
                        mainKeyboard.ResizeKeyboard = true;
                        if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                        {
                            mainKeyboard.Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "📋 گزارش تجمیعی", "📆 گزارش تفصیلی" } };
                        }
                        else
                        {
                            mainKeyboard.Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "📆 گزارش تفصیلی" } };
                        }

                        var query = db.DailyReport.Where(x => x.BranchCalendar.ReportDate >= fromDate && x.BranchCalendar.ReportDate <= toDate);
                        var data = query.GroupBy(x => x.Branches).ToList();

                        if (data != null && data.Count > 0)
                        {
                            foreach (var report in data)
                            {
                                var file = MakeReport(report.ToList(), DateUtility.GetPersianDate(fromDate) + " - " + DateUtility.GetPersianDate(toDate));
                                InputOnlineFile document = new InputOnlineFile(new MemoryStream(file), report.Key.Alias + " " + DateUtility.GetPersianDate(fromDate).Replace("/", "-") + ".pdf");
                                string caption = "📉 گزارش تفصیلی\n";
                                caption += "🏠 شعبه: " + report.Key.Name + "\n";
                                caption += "📆 از " + DateUtility.GetPersianDate(fromDate) + "\n";
                                caption += "📆 تا " + DateUtility.GetPersianDate(toDate);


                                caption += "\n💵 فروش: " + (report.Sum(x => x.SaleExit) - report.Sum(x => x.ReturnedEntry) - report.Sum(x => x.OtherKiaGoldEntry)).ToString("N0") + " ریال"; ;
                                caption += "\n⚖️ وزن: " + (report.Sum(x => x.SaleWeight) - report.Sum(x => x.ReturnedWeight) - report.Sum(x => x.OtherKiaGoldWeight)) + " گرم";


                                Bot.SendDocumentAsync(user.ChatId, document, caption, replyMarkup: mainKeyboard);
                            }

                        }
                        else
                        {
                            Bot.SendTextMessageAsync(user.ChatId, "در تاریخ انتخاب شده گزارشی ثبت نشده است");
                        }
                    }
                }
                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
            }
            else
            {
                if (DateUtility.GetDateTime(TextMessage) != null) // گزارش روزانه
                {
                    using (var db = new KiaGalleryMainEntities())
                    {
                        var messageItem = db.DailyReportMessage.Where(x => x.ChatId == user.ChatId && (x.Text == "📆 گزارش تفصیلی" || x.Text == "📋 گزارش تجمیعی")).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                        if (messageItem != null)
                        {
                            if (messageItem.Text == "📆 گزارش تفصیلی")
                            {
                                var messageList = db.DailyReportMessage.Where(x => x.ChatId == user.ChatId && x.Id > messageItem.Id).OrderBy(x => x.CreateDate).ToList();
                                if (messageList.Count(x => x.Text == "بازه تاریخ") > 0)
                                {
                                    if (messageList.Count(x => x.Text == "تاریخ پایان را وارد کنید:") == 0)
                                    {
                                        Bot.SendTextMessageAsync(Chat.Id, "تاریخ پایان را وارد کنید:", ParseMode.Html);
                                        AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
                                        AddMessage(item.Message.Chat.Id, item.Message.MessageId, "تاریخ پایان را وارد کنید:", item.Message.Date, false, null);
                                    }
                                    else if (messageList.Count(x => x.Text == "شعبه مورد نظر را انتخاب کنید:") == 0)
                                    {
                                        var branches = db.Branches.Where(x => x.BranchType == (int)BranchType.Branch).Select(x => x.Name).ToList();
                                        branches.Insert(0, "همه");
                                        var keyboard = GenerateKeyBoard(branches, 3, true);

                                        Bot.SendTextMessageAsync(Chat.Id, "شعبه مورد نظر را انتخاب کنید:", ParseMode.Html, false, false, 0, keyboard);
                                        AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
                                        AddMessage(item.Message.Chat.Id, item.Message.MessageId, "شعبه مورد نظر را انتخاب کنید:", item.Message.Date, false, null);
                                    }
                                    else
                                    {
                                        //////////////
                                        var fromDate = DateUtility.GetDateTime(messageList[messageList.FindIndex(x => x.Text == "تاریخ پایان را وارد کنید:") - 1].Text).Value;
                                        var toDate = DateUtility.GetDateTime(TextMessage).Value;

                                        var mainKeyboard = new ReplyKeyboardMarkup();
                                        mainKeyboard.ResizeKeyboard = true;
                                        if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                                        {
                                            mainKeyboard.Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "📋 گزارش تجمیعی", "📆 گزارش تفصیلی" } };
                                        }
                                        else
                                        {
                                            mainKeyboard.Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "📆 گزارش تفصیلی" } };
                                        }

                                        var query = db.DailyReport.Where(x => x.BranchCalendar.ReportDate >= fromDate && x.BranchCalendar.ReportDate <= toDate);
                                        var data = query.GroupBy(x => x.Branches).ToList();

                                        if (data != null && data.Count > 0)
                                        {
                                            foreach (var report in data)
                                            {
                                                var file = MakeReport(report.ToList(), DateUtility.GetPersianDate(fromDate) + " - " + DateUtility.GetPersianDate(toDate));
                                                InputOnlineFile document = new InputOnlineFile(new MemoryStream(file), report.Key.Alias + " " + DateUtility.GetPersianDate(fromDate).Replace("/", "-") + ".pdf");
                                                string caption = "📉 گزارش تفصیلی\n";
                                                caption += "🏠 شعبه: " + report.Key.Name + "\n";
                                                caption += "📆 از " + DateUtility.GetPersianDate(fromDate) + "\n";
                                                caption += "📆 تا " + DateUtility.GetPersianDate(toDate);


                                                caption += "\n💵 فروش: " + (report.Sum(x => x.SaleExit) - report.Sum(x => x.ReturnedEntry) - report.Sum(x => x.OtherKiaGoldEntry)).ToString("N0") + " ریال"; ;
                                                caption += "\n⚖️ وزن: " + (report.Sum(x => x.SaleWeight) - report.Sum(x => x.ReturnedWeight) - report.Sum(x => x.OtherKiaGoldWeight)) + " گرم";


                                                Bot.SendDocumentAsync(user.ChatId, document, caption, replyMarkup: mainKeyboard);
                                            }

                                        }
                                        else
                                        {
                                            Bot.SendTextMessageAsync(user.ChatId, "در تاریخ انتخاب شده گزارشی ثبت نشده است");
                                        }
                                    }
                                }
                                else
                                {
                                    if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                                    {
                                        var branches = db.Branches.Where(x => x.BranchType == (int)BranchType.Branch).Select(x => x.Name).ToList();
                                        branches.Insert(0, "همه");
                                        var keyboard = GenerateKeyBoard(branches, 3, true);

                                        Bot.SendTextMessageAsync(Chat.Id, "شعبه مورد نظر را انتخاب کنید:", ParseMode.Html, false, false, 0, keyboard);
                                        AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
                                    }
                                    else
                                    {
                                        if (!user.BranchId.Contains("-"))
                                        {
                                            var date = DateUtility.GetDateTime(TextMessage);
                                            var branchList = user.BranchId.Split('-').Select(x => int.Parse(x)).ToList();

                                            var data = db.DailyReport.Where(x => x.BranchCalendar.ReportDate == date && branchList.Any(y => y == x.BranchId)).ToList();
                                            if (data.Count > 0)
                                            {
                                                foreach (var itemDailyReport in data)
                                                {
                                                    var file = MakeReport(itemDailyReport, TextMessage);
                                                    InputOnlineFile document = new InputOnlineFile(new MemoryStream(file), (itemDailyReport.Branches.Alias + " " + TextMessage.Replace("/", "-") + ".pdf").Trim());
                                                    var caption = "🏠 شعبه " + itemDailyReport.Branches.Name;
                                                    caption += "\n" + (itemDailyReport.Status == 2 ? "✅" : "❌") + " " + Enums.GetTitle((CalendarStatus)itemDailyReport.Status);
                                                    caption += "\n📆 تاریخ: " + TextMessage;

                                                    caption += "\n💵 فروش: " + (itemDailyReport.SaleExit - itemDailyReport.ReturnedEntry - itemDailyReport.OtherKiaGoldEntry).ToString("N0") + " ریال"; ;
                                                    caption += "\n⚖️ وزن: " + (itemDailyReport.SaleWeight - itemDailyReport.ReturnedWeight - itemDailyReport.OtherKiaGoldWeight) + " گرم";

                                                    Bot.SendDocumentAsync(user.ChatId, document, caption);
                                                }
                                            }
                                            else
                                            {
                                                Bot.SendTextMessageAsync(user.ChatId, "در تاریخ انتخاب شده گزارشی ثبت نشده است");
                                            }
                                            AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
                                        }
                                        else
                                        {
                                            var branchList = user.BranchId.Split('-').Select(x => int.Parse(x)).ToList();

                                            var branches = db.Branches.Where(x => x.BranchType == (int)BranchType.Branch && branchList.Any(y => y == x.Id)).Select(x => x.Name).ToList();
                                            branches.Insert(0, "همه");
                                            var keyboard = GenerateKeyBoard(branches, 3, true);

                                            Bot.SendTextMessageAsync(Chat.Id, "شعبه مورد نظر را انتخاب کنید:", ParseMode.Html, false, false, 0, keyboard);
                                            AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
                                        }
                                    }
                                }
                            }
                            else if (messageItem.Text == "📋 گزارش تجمیعی")
                            {
                                var messageList = db.DailyReportMessage.Where(x => x.ChatId == user.ChatId && x.Id > messageItem.Id).OrderBy(x => x.CreateDate).ToList();
                                if (messageList.Count(x => x.Text == "بازه تاریخ") > 0)
                                {
                                    if (messageList.Count(x => x.Text == "تاریخ پایان را وارد کنید:") == 0)
                                    {
                                        Bot.SendTextMessageAsync(Chat.Id, "تاریخ پایان را وارد کنید:", ParseMode.Html);
                                        AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
                                        AddMessage(item.Message.Chat.Id, item.Message.MessageId, "تاریخ پایان را وارد کنید:", item.Message.Date, false, null);
                                    }
                                    else
                                    {
                                        var fromDate = DateUtility.GetDateTime(messageList[messageList.FindIndex(x => x.Text == "تاریخ پایان را وارد کنید:") - 1].Text).Value;
                                        var toDate = DateUtility.GetDateTime(TextMessage).Value;

                                        var mainKeyboard = new ReplyKeyboardMarkup();
                                        mainKeyboard.ResizeKeyboard = true;
                                        if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                                        {
                                            mainKeyboard.Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "📋 گزارش تجمیعی", "📆 گزارش تفصیلی" } };
                                        }
                                        else
                                        {
                                            mainKeyboard.Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "📆 گزارش تفصیلی" } };
                                        }


                                        var file = MakeTotalReport(fromDate, toDate);
                                        if (file.data != null)
                                        {
                                            InputOnlineFile document = new InputOnlineFile(new MemoryStream(file.data), "TotalReport.pdf");
                                            string caption = "📊 گزارش تجمیعی\n";
                                            caption += "📆 از " + DateUtility.GetPersianDate(fromDate) + "\n";
                                            caption += "📆 تا " + DateUtility.GetPersianDate(toDate);

                                            caption += "\n💵 فروش: " + file.totalPrice.ToString("N0") + " ریال"; ;
                                            caption += "\n⚖️ وزن: " + file.totalWeight + " گرم";


                                            Bot.SendDocumentAsync(user.ChatId, document, caption, replyMarkup: mainKeyboard);
                                        }
                                        else
                                        {
                                            Bot.SendTextMessageAsync(user.ChatId, "در تاریخ انتخاب شده گزارشی ثبت نشده است");
                                        }
                                    }
                                }
                                else
                                {
                                    var mainKeyboard = new ReplyKeyboardMarkup();
                                    mainKeyboard.ResizeKeyboard = true;
                                    if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                                    {
                                        mainKeyboard.Keyboard = new KeyboardButton[][] {
                                            new KeyboardButton[] { "📋 گزارش تجمیعی", "📆 گزارش تفصیلی" }
                                        };
                                    }
                                    else
                                    {
                                        mainKeyboard.Keyboard = new KeyboardButton[][] {
                                            new KeyboardButton[] { "📆 گزارش تفصیلی" }
                                        };
                                    }

                                    var file = MakeTotalReport(DateUtility.GetDateTime(TextMessage).Value);
                                    if (file.data != null)
                                    {
                                        InputOnlineFile document = new InputOnlineFile(new MemoryStream(file.data), TextMessage.Replace("/", "-") + ".pdf");
                                        string caption = "📊 گزارش تجمیعی\n";
                                        caption += "📆 تاریخ: " + TextMessage + "\n";

                                        caption += "\n💵 فروش: " + file.totalPrice.ToString("N0") + " ریال"; ;
                                        caption += "\n⚖️ وزن: " + file.totalWeight + " گرم";

                                        Bot.SendDocumentAsync(user.ChatId, document, caption, replyMarkup: mainKeyboard);
                                    }
                                    else
                                    {
                                        Bot.SendTextMessageAsync(user.ChatId, "در تاریخ انتخاب شده گزارشی ثبت نشده است");
                                    }
                                }
                            }
                            else
                            {
                                var mainKeyboard = new ReplyKeyboardMarkup();
                                mainKeyboard.ResizeKeyboard = true;
                                if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                                {
                                    mainKeyboard.Keyboard = new KeyboardButton[][] {
                                        new KeyboardButton[] { "📋 گزارش تجمیعی", "📆 گزارش تفصیلی" }
                                    };
                                }
                                else
                                {
                                    mainKeyboard.Keyboard = new KeyboardButton[][] {
                                        new KeyboardButton[] { "📆 گزارش تفصیلی" }
                                    };
                                }

                                Bot.SendTextMessageAsync(user.ChatId, "پیام مورد نظر یافت نشد", replyMarkup: mainKeyboard);
                                AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
                            }
                        }
                    }
                }
                else if (user.BotUserType == (int)BotDailyReportUserType.Admin && IsBranch(TextMessage))
                {
                    using (var db = new KiaGalleryMainEntities())
                    {
                        var typeMessage = db.DailyReportMessage.Where(x => x.ChatId == user.ChatId && (x.Text == "📆 گزارش تفصیلی" || x.Text == "📋 گزارش تجمیعی")).OrderByDescending(x => x.CreateDate).FirstOrDefault();

                        var messageList = db.DailyReportMessage.Where(x => x.ChatId == user.ChatId && x.Id > typeMessage.Id).OrderBy(x => x.CreateDate).ToList();
                        if (messageList.Count(x => x.Text == "بازه تاریخ") > 0)
                        {
                            var fromDate = DateUtility.GetDateTime(messageList[messageList.FindIndex(x => x.Text == "تاریخ پایان را وارد کنید:") - 1].Text).Value;
                            var toDate = DateUtility.GetDateTime(messageList[messageList.FindIndex(x => x.Text == "شعبه مورد نظر را انتخاب کنید:") - 1].Text).Value;

                            var mainKeyboard = new ReplyKeyboardMarkup();
                            mainKeyboard.ResizeKeyboard = true;
                            if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                            {
                                mainKeyboard.Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "📋 گزارش تجمیعی", "📆 گزارش تفصیلی" } };
                            }
                            else
                            {
                                mainKeyboard.Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "📆 گزارش تفصیلی" } };
                            }

                            var query = db.DailyReport.Where(x => x.BranchCalendar.ReportDate >= fromDate && x.BranchCalendar.ReportDate <= toDate && x.Branches.Name == TextMessage);
                            var data = query.GroupBy(x => x.Branches).ToList();

                            if (data != null && data.Count > 0)
                            {
                                foreach (var report in data)
                                {
                                    var file = MakeReport(report.ToList(), DateUtility.GetPersianDate(fromDate) + " - " + DateUtility.GetPersianDate(toDate));
                                    InputOnlineFile document = new InputOnlineFile(new MemoryStream(file), report.Key.Alias + " " + DateUtility.GetPersianDate(fromDate).Replace("/", "-") + ".pdf");
                                    string caption = "📉 گزارش تفصیلی\n";
                                    caption += "🏠 شعبه: " + report.Key.Name + "\n";
                                    caption += "📆 از " + DateUtility.GetPersianDate(fromDate) + "\n";
                                    caption += "📆 تا " + DateUtility.GetPersianDate(toDate);


                                    caption += "\n💵 فروش: " + (report.Sum(x => x.SaleExit) - report.Sum(x => x.ReturnedEntry) - report.Sum(x => x.OtherKiaGoldEntry)).ToString("N0") + " ریال"; ;
                                    caption += "\n⚖️ وزن: " + (report.Sum(x => x.SaleWeight) - report.Sum(x => x.ReturnedWeight) - report.Sum(x => x.OtherKiaGoldWeight)) + " گرم";


                                    Bot.SendDocumentAsync(user.ChatId, document, caption, replyMarkup: mainKeyboard);
                                }
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(user.ChatId, "در تاریخ انتخاب شده گزارشی ثبت نشده است");
                            }
                        }
                        else
                        {
                            var messageItem = db.DailyReportMessage.Where(x => x.ChatId == user.ChatId && DbFunctions.Like(x.Text, "____/__/__")).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                            if (messageItem != null)
                            {
                                var date = DateUtility.GetDateTime(messageItem.Text);

                                List<DailyReport> data = db.DailyReport.Where(x => x.BranchCalendar.ReportDate == date && x.Branches.Name == TextMessage).ToList();

                                if (data.Count > 0)
                                {
                                    foreach (var itemDailyReport in data)
                                    {
                                        var file = MakeReport(itemDailyReport, messageItem.Text);
                                        InputOnlineFile document = new InputOnlineFile(new MemoryStream(file), (itemDailyReport.Branches.Alias + " " + messageItem.Text.Replace("/", "-") + ".pdf").Trim());
                                        var caption = "🏠 شعبه " + itemDailyReport.Branches.Name;
                                        caption += "\n" + (itemDailyReport.Status == 2 ? "✅" : "❌") + " " + Enums.GetTitle((CalendarStatus)itemDailyReport.Status);
                                        caption += "\n📆 تاریخ: " + messageItem.Text;

                                        caption += "\n💵 فروش: " + (itemDailyReport.SaleExit - itemDailyReport.ReturnedEntry - itemDailyReport.OtherKiaGoldEntry).ToString("N0") + " ریال"; ;
                                        caption += "\n⚖️ وزن: " + (itemDailyReport.SaleWeight - itemDailyReport.ReturnedWeight - itemDailyReport.OtherKiaGoldWeight) + " گرم";

                                        Bot.SendDocumentAsync(user.ChatId, document, caption);
                                    }
                                }
                                else
                                {
                                    Bot.SendTextMessageAsync(user.ChatId, "در تاریخ انتخاب شده گزارشی ثبت نشده است");
                                }
                            }
                        }
                    }
                    AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
                }
                else
                {
                    var mainKeyboard = new ReplyKeyboardMarkup();
                    mainKeyboard.ResizeKeyboard = true;
                    if (user.BotUserType == (int)BotDailyReportUserType.Admin)
                    {
                        mainKeyboard.Keyboard = new KeyboardButton[][] {
                            new KeyboardButton[] { "📋 گزارش تجمیعی", "📆 گزارش تفصیلی" }
                        };
                    }
                    else
                    {
                        mainKeyboard.Keyboard = new KeyboardButton[][] {
                            new KeyboardButton[] { "📆 گزارش تفصیلی" }
                        };
                    }

                    Bot.SendTextMessageAsync(user.ChatId, "پیام مورد نظر یافت نشد", replyMarkup: mainKeyboard);
                    AddMessage(item.Message.Chat.Id, item.Message.MessageId, TextMessage, item.Message.Date, false, null);
                }
            }
        }

        private void ResponseCallback(Update item)
        {

        }

        private ReplyKeyboardMarkup GenerateKeyBoard(List<string> Titles, int ColumnSize, bool MenuButton, bool dateRange = false)
        {
            ReplyKeyboardMarkup Keyboard = new ReplyKeyboardMarkup();
            Keyboard.ResizeKeyboard = true;

            int RowSize = Titles.Count / ColumnSize;

            if (Titles.Count % ColumnSize > 0)
                RowSize++;

            if (dateRange)
                RowSize++;

            if (MenuButton)
                RowSize++;

            KeyboardButton[][] keys = new KeyboardButton[RowSize][];

            for (int i = 0; i < Titles.Count; i = i + ColumnSize)
            {
                List<string> key = new List<string>();
                for (int j = 0; j < ColumnSize; j++)
                {
                    if (i + j >= Titles.Count)
                        continue;
                    key.Add(Titles[i + j]);
                }
                var tempKey = key.ToArray();
                for (int x = 0; x < tempKey.Length; x++)
                {
                    if (keys[i / ColumnSize] == null)
                        keys[i / ColumnSize] = new KeyboardButton[tempKey.Length];
                    keys[i / ColumnSize][x] = tempKey[x];
                }
            }

            if (dateRange)
            {
                keys[RowSize - 2] = new KeyboardButton[] { "بازه تاریخ" };
            }

            if (MenuButton)
            {
                keys[RowSize - 1] = new KeyboardButton[] { "🔚 بازگشت" };
            }

            Keyboard.Keyboard = keys;

            return Keyboard;
        }

        private BotDailyReportUserData AddUser(BotDailyReportUserData user)
        {
            using (var db = new KiaGalleryMainEntities())
            {
                var userEntity = db.BotDailyReportUserData.FirstOrDefault(x => x.ChatId == user.ChatId);
                if (userEntity == null)
                {
                    user.CreatedDate = DateTime.Now;
                    user.BotUserType = (int)BotDailyReportUserType.None;
                    db.BotDailyReportUserData.Add(user);
                }
                else
                {
                    userEntity.UserType = user.UserType;
                    userEntity.UserId = user.UserId;
                    userEntity.ChatId = user.ChatId;
                    userEntity.FirstName = user.FirstName;
                    userEntity.LastName = user.LastName;
                    userEntity.Username = user.Username;
                }
                db.SaveChanges();
                user.BranchId = userEntity.BranchId;
                return userEntity;
            }
        }

        public static void AddMessage(long chatId, int messageId, string text, DateTime date, bool unknown, int? branchId)
        {
            using (var db = new KiaGalleryMainEntities())
            {
                var _Message = new DailyReportMessage()
                {
                    ChatId = chatId,
                    MessageId = messageId,
                    Text = text,
                    CreateDate = date.ToUniversalTime(),
                    Unknown = unknown,
                    BranchId = branchId
                };
                db.DailyReportMessage.Add(_Message);
                db.SaveChanges();
            }
        }

        private void UpdateOffset(int offset)
        {
            using (var db = new KiaGalleryMainEntities())
            {
                var entity = db.DailyReportSettings.SingleOrDefault(x => x.Key == "last-offset");
                entity.Value = offset.ToString();
                db.SaveChanges();
            }
        }

        private void GetSettings()
        {
            using (var db = new KiaGalleryMainEntities())
            {
                Settings = db.DailyReportSettings.ToList();
            }
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Running = false;
            BtnConnect.IsEnabled = true;
            BtnDisconnect.IsEnabled = false;
            LblStatus.Text = "Disconnect.";
        }

        public bool IsBranch(string TextMessage)
        {
            using (var db = new KiaGalleryMainEntities())
            {
                return db.Branches.Count(x => x.Name == TextMessage && x.BranchType == (int)BranchType.Branch) > 0;
            }
        }

        public byte[] MakeReport(DailyReport entity, string date)
        {
            StiReport report = new StiReport();
            report.Load(System.AppDomain.CurrentDomain.BaseDirectory + "/Report/BranchDailyReport.mrt");
            report.Dictionary.Databases.Clear();
            report.ScriptLanguage = StiReportLanguageType.CSharp;

            #region Variable
            report.Dictionary.Variables["Date"].Value = date;
            report.Dictionary.Variables["Branch"].Value = entity.Branches.Name;

            var totalSale = entity.SaleExit - entity.SaleEntry;
            report.Dictionary.Variables["NumberSaleFactor"].Value = entity.NumberSaleFactor.ToString().ToPersianNumber() + " عدد";
            report.Dictionary.Variables["SaleWeight"].Value = entity.SaleWeight.ToString().ToPersianNumber() + " گرم";
            report.Dictionary.Variables["SaleWeightPercent"].Value = entity.SaleWeightPercent.ToString().ToPersianNumber() + " گرم";
            report.Dictionary.Variables["SaleExit"].Value = entity.SaleExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["SaleEntry"].Value = entity.SaleEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["SaleRemaining"].Value = totalSale.ToString("N0").ToPersianNumber();

            var totalReturned = totalSale + (entity.ReturnedExit - entity.ReturnedEntry);
            report.Dictionary.Variables["ReturnedWeight"].Value = entity.ReturnedWeight.ToString().ToPersianNumber() + " گرم";
            report.Dictionary.Variables["ReturnWeightPercent"].Value = entity.ReturnWeightPercent.ToString().ToPersianNumber() + " گرم";
            report.Dictionary.Variables["ReturnedExit"].Value = entity.ReturnedExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["ReturnedEntry"].Value = entity.ReturnedEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["ReturnedRemaining"].Value = totalReturned.ToString("N0").ToPersianNumber();

            var totalBank = totalReturned + (entity.DailyReportBank.Sum(x => x.Exit) - entity.DailyReportBank.Sum(x => x.Entry));

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Exit");
            dataTable.Columns.Add("Entry");
            dataTable.Columns.Add("Remaining");
            var bank = totalReturned;
            var bankList = entity.DailyReportBank.ToList();
            for (int j = 0; j < entity.DailyReportBank.Count; j++)
            {
                bank = bank + (bankList[j].Exit - bankList[j].Entry);
                DataRow row = dataTable.NewRow();
                row["Name"] = bankList[j].Bank.Name;
                row["Exit"] = bankList[j].Exit.ToString("N0").ToPersianNumber();
                row["Entry"] = bankList[j].Entry.ToString("N0").ToPersianNumber();
                row["Remaining"] = bank.ToString("N0").ToPersianNumber();

                dataTable.Rows.Add(row);
            }

            var totalCash = totalBank + (entity.CashExit - entity.CashEntry);
            report.Dictionary.Variables["OtherCash"].Value = entity.OtherCash.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CashExit"].Value = entity.CashExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CashEntry"].Value = entity.CashEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CashRemaining"].Value = totalCash.ToString("N0").ToPersianNumber();

            var totalCurrency = totalCash + (entity.DailyReportCurrency.Sum(x => x.RialExit) - entity.DailyReportCurrency.Sum(x => x.RialEntry));
            //report.Dictionary.Variables["CurrencyExit"].Value = entity.CashExit.ToString("N0");
            //report.Dictionary.Variables["CurrencyEntry"].Value = entity.CashEntry.ToString("N0");
            //report.Dictionary.Variables["CurrencyRemaining"].Value = totalCurrency.ToString("N0");

            DataTable currencyDataTable = new DataTable();
            currencyDataTable.Columns.Add("Name");
            currencyDataTable.Columns.Add("Exit");
            currencyDataTable.Columns.Add("Entry");
            currencyDataTable.Columns.Add("Remaining");
            var currency = totalCurrency;
            for (int j = 0; j < entity.DailyReportCurrency.Count; j++)
            {
                currency = currency + (entity.DailyReportCurrency.ToList()[j].RialExit - entity.DailyReportCurrency.ToList()[j].RialEntry);
                DataRow row = currencyDataTable.NewRow();
                row["Name"] = entity.DailyReportCurrency.ToList()[j].Currency.Name;
                row["Exit"] = entity.DailyReportCurrency.ToList()[j].RialExit.ToString("N0").ToPersianNumber();
                row["Entry"] = entity.DailyReportCurrency.ToList()[j].RialEntry.ToString("N0").ToPersianNumber();
                row["Remaining"] = currency.ToString("N0").ToPersianNumber();

                currencyDataTable.Rows.Add(row);
            }

            var totalOtherCurrency = totalCurrency + (entity.OtherCurrencyRialExit - entity.OtherCurrencyRialEntry);
            report.Dictionary.Variables["OtherCurrencyExit"].Value = entity.OtherCurrencyRialExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherCurrencyEntry"].Value = entity.OtherCurrencyRialEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherCurrencyRemaining"].Value = totalOtherCurrency.ToString("N0").ToPersianNumber();

            report.Dictionary.Variables["InventoryCash"].Value = entity.InventoryCash.ToString("N0").ToPersianNumber();

            var totalGoldDeficit = totalOtherCurrency + (entity.GoldDeficitExit - entity.GoldDeficitEntry);
            report.Dictionary.Variables["GoldDeficitWeight"].Value = entity.GoldDeficitWeight.ToString().ToPersianNumber();
            report.Dictionary.Variables["GoldDeficitExit"].Value = entity.GoldDeficitExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["GoldDeficitEntry"].Value = entity.GoldDeficitEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["GoldDeficitRemaining"].Value = totalGoldDeficit.ToString("N0").ToPersianNumber();

            var totalGift = totalGoldDeficit + (entity.GiftExit - entity.GiftEntry);
            report.Dictionary.Variables["GiftNumberEntry"].Value = entity.GiftNumberEntry.ToString().ToPersianNumber();
            report.Dictionary.Variables["GiftNumberExit"].Value = entity.GiftNumberExit.ToString().ToPersianNumber();
            report.Dictionary.Variables["GiftExit"].Value = entity.GiftExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["GiftEntry"].Value = entity.GiftEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["GiftRemaining"].Value = totalGift.ToString("N0").ToPersianNumber();

           

            var totalCheck = totalGift + (entity.CheckExit - entity.CheckEntry);
            report.Dictionary.Variables["CheckNumber"].Value = entity.CheckNumber.ToString().ToPersianNumber();
            report.Dictionary.Variables["CheckExit"].Value = entity.CheckExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CheckEntry"].Value = entity.CheckEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CheckRemaining"].Value = totalCheck.ToString("N0").ToPersianNumber();

            var totalLeatherStone = totalCheck + (entity.LeatherStoneExit - entity.LeatherStoneEntry);
            report.Dictionary.Variables["LeatherStoneDescEntry"].Value = entity.LeatherStoneDescriptionEntry?.ToPersianNumber();
            report.Dictionary.Variables["LeatherStoneDescExit"].Value = entity.LeatherStoneDescriptionExit?.ToPersianNumber();
            report.Dictionary.Variables["LeatherStoneExit"].Value = entity.LeatherStoneExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["LeatherStoneEntry"].Value = entity.LeatherStoneEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["LeatherStoneRemaining"].Value = totalLeatherStone.ToString("N0").ToPersianNumber();

            var totalCoin = totalLeatherStone + (entity.CoinExit - entity.CoinEntry);
            report.Dictionary.Variables["CoinNumber"].Value = entity.CoinNumber.ToString().ToPersianNumber();
            report.Dictionary.Variables["CoinExit"].Value = entity.CoinExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CoinEntry"].Value = entity.CoinEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CoinRemaining"].Value = totalCoin.ToString("N0").ToPersianNumber();

            var totalOtherKiaGold = totalCoin + (entity.OtherKiaGoldExit - entity.OtherKiaGoldEntry);
            report.Dictionary.Variables["OtherKiaGoldWeight"].Value = entity.OtherKiaGoldWeight.ToString().ToPersianNumber();
            report.Dictionary.Variables["OtherKiaGoldExit"].Value = entity.OtherKiaGoldExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherKiaGoldEntry"].Value = entity.OtherKiaGoldEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherKiaGoldRemaining"].Value = totalOtherKiaGold.ToString("N0").ToPersianNumber();

            var totalOtherGold = totalOtherKiaGold + (entity.OtherGoldExit - entity.OtherGoldEntry);
            report.Dictionary.Variables["OtherGoldWeight"].Value = entity.OtherGoldWeight.ToString().ToPersianNumber();
            report.Dictionary.Variables["OtherGoldExit"].Value = entity.OtherGoldExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherGoldEntry"].Value = entity.OtherGoldEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherGoldRemaining"].Value = totalOtherGold.ToString("N0").ToPersianNumber();

            var totalCreditorCustomer = totalOtherGold + (entity.CreditorCustomerExit - entity.CreditorCustomerEntry);
            report.Dictionary.Variables["CreditorCustomerExit"].Value = entity.CreditorCustomerExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CreditorCustomerEntry"].Value = entity.CreditorCustomerEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CreditorCustomerRemaining"].Value = totalCreditorCustomer.ToString("N0").ToPersianNumber();

            var totalDebtorCustomer = totalCreditorCustomer + (entity.DebtorCustomerExit - entity.DebtorCustomerEntry);
            report.Dictionary.Variables["DebtorCustomerExit"].Value = entity.DebtorCustomerExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DebtorCustomerEntry"].Value = entity.DebtorCustomerEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DebtorCustomerRemaining"].Value = totalDebtorCustomer.ToString("N0").ToPersianNumber();

            var totalDepositBefore = totalDebtorCustomer + (entity.DepositBeforeExit - entity.DepositBeforeEntry);
            report.Dictionary.Variables["DepositBeforeCount"].Value = entity.DepositBeforeCount.ToString().ToPersianNumber();
            report.Dictionary.Variables["DepositBeforeExit"].Value = entity.DepositBeforeExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DepositBeforeEntry"].Value = entity.DepositBeforeEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DepositBeforeRemaining"].Value = totalDepositBefore.ToString("N0").ToPersianNumber();

            var totalDepositNew = totalDepositBefore + (entity.DepositNewExit - entity.DepositNewEntry);
            report.Dictionary.Variables["DepositNewCount"].Value = entity.DepositNewCount.ToString().ToPersianNumber();
            report.Dictionary.Variables["DepositNewExit"].Value = entity.DepositNewExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DepositNewEntry"].Value = entity.DepositNewEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DepositNewRemaining"].Value = totalDepositNew.ToString("N0").ToPersianNumber();

            var totalLoyality = totalDepositNew + (entity.LoyalityExit - entity.LoyalityEntry);
            report.Dictionary.Variables["LoyalityExit"].Value = entity.LoyalityExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["LoyalityEntry"].Value = entity.LoyalityEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["LoyalityRemaining"].Value = totalLoyality.ToString("N0").ToPersianNumber();

            var totalDiscount = totalLoyality + (entity.DiscountExit - entity.DiscountEntry);
            report.Dictionary.Variables["DiscountExit"].Value = entity.DiscountExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DiscountEntry"].Value = entity.DiscountEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DiscountRemaining"].Value = totalDiscount.ToString("N0").ToPersianNumber();

            var totalCost = totalDiscount + (entity.CostExit - entity.CostEntry);
            report.Dictionary.Variables["CostCourierPostExit"].Value = entity.CostExit.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CostCourierPostEntry"].Value = entity.CostEntry.ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CostCourierPostRemaining"].Value = totalCost.ToString("N0").ToPersianNumber();

            report.Dictionary.Variables["TotalProfit"].Value = (entity.SaleExit * 0.059).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["TotalValue"].Value = (entity.SaleExit * 0.0818).ToString("N0").ToPersianNumber();
            #endregion

            report.Dictionary.Databases.Clear();
            report.RegData("BankDataSource", dataTable);
            report.RegData("CurrencyDataSource", currencyDataTable);
            report.Compile();
            report.Render(false);

            MemoryStream stream = new MemoryStream();

            StiPdfExportSettings settings = new StiPdfExportSettings();

            StiPdfExportService service = new StiPdfExportService();
            service.ExportPdf(report, stream, settings);

            return stream.ToArray();
        }

        public byte[] MakeReport(List<DailyReport> data, string date)
        {
            StiReport report = new StiReport();
            report.Load(System.AppDomain.CurrentDomain.BaseDirectory + "/Report/BranchDailyReport.mrt");
            report.Dictionary.Databases.Clear();
            report.ScriptLanguage = StiReportLanguageType.CSharp;

            #region Variable
            report.Dictionary.Variables["Date"].Value = date;
            var branchList = data.Select(x => x.Branches.Name).Distinct();
            report.Dictionary.Variables["Branch"].Value = "تجمیعی " + string.Join(" , ", branchList);

            var totalSale = data.Sum(x => x.SaleExit) - data.Sum(x => x.SaleEntry);
            report.Dictionary.Variables["NumberSaleFactor"].Value = data.Sum(x => x.NumberSaleFactor).ToString().ToPersianNumber() + " عدد";
            report.Dictionary.Variables["SaleWeight"].Value = data.Sum(x => x.SaleWeight).ToString().ToPersianNumber() + " گرم";
            report.Dictionary.Variables["SaleWeightPercent"].Value = data.Sum(x => x.SaleWeightPercent).ToString().ToPersianNumber() + " گرم";
            report.Dictionary.Variables["SaleExit"].Value = data.Sum(x => x.SaleExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["SaleEntry"].Value = data.Sum(x => x.SaleEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["SaleRemaining"].Value = totalSale.ToString("N0").ToPersianNumber();

            var totalReturned = totalSale + (data.Sum(x => x.ReturnedExit) - data.Sum(x => x.ReturnedEntry));
            report.Dictionary.Variables["ReturnedWeight"].Value = data.Sum(x => x.ReturnedWeight).ToString().ToPersianNumber() + " گرم";
            report.Dictionary.Variables["ReturnWeightPercent"].Value = data.Sum(x => x.ReturnWeightPercent).ToString().ToPersianNumber() + " گرم";
            report.Dictionary.Variables["ReturnedExit"].Value = data.Sum(x => x.ReturnedExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["ReturnedEntry"].Value = data.Sum(x => x.ReturnedEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["ReturnedRemaining"].Value = totalReturned.ToString("N0").ToPersianNumber();

            var totalBank = totalReturned + (data.Sum(x => x.DailyReportBank.Sum(y => y.Exit)) - data.Sum(x => x.DailyReportBank.Sum(y => y.Entry)));

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Exit");
            dataTable.Columns.Add("Entry");
            dataTable.Columns.Add("Remaining");
            var bank = totalReturned;

            var bankList = data.Select(x =>
                x.DailyReportBank.Select(y => new HelperData
                {
                    Id = y.BankId,
                    Exit = y.Exit,
                    Entry = y.Entry,
                    Name = y.Bank.Name
                }).ToList()
            ).ToList();

            List<HelperData> bankData = new List<HelperData>();
            foreach (var item in bankList)
            {
                foreach (var itemReport in item)
                {
                    if (bankData.Count(x => x.Id == itemReport.Id) == 0)
                    {
                        bankData.Add(new HelperData()
                        {
                            Id = itemReport.Id,
                            Name = itemReport.Name,
                            Entry = itemReport.Entry,
                            Exit = itemReport.Exit
                        });
                    }
                    else
                    {
                        var itemData = bankData.Single(x => x.Id == itemReport.Id);
                        itemData.Exit += itemReport.Exit;
                        itemData.Entry += itemReport.Entry;
                    }
                }
            }

            for (int j = 0; j < bankData.Count; j++)
            {
                bank = bank + (bankData[j].Exit - bankData[j].Entry);
                DataRow row = dataTable.NewRow();
                row["Name"] = bankData[j].Name;
                row["Exit"] = bankData[j].Exit.ToString("N0").ToPersianNumber();
                row["Entry"] = bankData[j].Entry.ToString("N0").ToPersianNumber();
                row["Remaining"] = bank.ToString("N0").ToPersianNumber();

                dataTable.Rows.Add(row);
            }

            var totalCash = totalBank + (data.Sum(x => x.CashExit) - data.Sum(x => x.CashEntry));
            report.Dictionary.Variables["OtherCash"].Value = data.Sum(x => x.OtherCash).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CashExit"].Value = data.Sum(x => x.CashExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CashEntry"].Value = data.Sum(x => x.CashEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CashRemaining"].Value = totalCash.ToString("N0").ToPersianNumber();

            var totalCurrency = totalCash + (data.Sum(x => x.DailyReportCurrency.Sum(y => y.RialExit)) - data.Sum(x => x.DailyReportCurrency.Sum(y => y.RialEntry)));
            //report.Dictionary.Variables["CurrencyExit"].Value = data.Sum(x => x.CashExit).ToString("N0").ToPersianNumber();
            //report.Dictionary.Variables["CurrencyEntry"].Value = data.Sum(x => x.CashEntry).ToString("N0").ToPersianNumber();
            //report.Dictionary.Variables["CurrencyRemaining"].Value = totalCurrency.ToString("N0").ToPersianNumber();

            DataTable currencyDataTable = new DataTable();
            currencyDataTable.Columns.Add("Name");
            currencyDataTable.Columns.Add("Exit");
            currencyDataTable.Columns.Add("Entry");
            currencyDataTable.Columns.Add("Remaining");
            var currency = totalCurrency;

            var currencyList = data.Select(x =>
                x.DailyReportCurrency.Select(y => new HelperData
                {
                    Id = y.CurrencyId,
                    Exit = y.RialExit,
                    Entry = y.RialEntry,
                    Name = y.Currency.Name
                }).ToList()
            ).ToList();

            List<HelperData> currencyData = new List<HelperData>();
            foreach (var item in currencyList)
            {
                foreach (var itemReport in item)
                {
                    if (currencyData.Count(x => x.Id == itemReport.Id) == 0)
                    {
                        currencyData.Add(new HelperData()
                        {
                            Id = itemReport.Id,
                            Name = itemReport.Name,
                            Entry = itemReport.Entry,
                            Exit = itemReport.Exit
                        });
                    }
                    else
                    {
                        var itemData = currencyData.Single(x => x.Id == itemReport.Id);
                        itemData.Exit += itemReport.Exit;
                        itemData.Entry += itemReport.Entry;
                    }
                }
            }

            for (int j = 0; j < currencyData.Count; j++)
            {
                currency = currency + (currencyData[j].Exit - currencyData[j].Entry);
                DataRow row = currencyDataTable.NewRow();
                row["Name"] = currencyData[j].Name;
                row["Exit"] = currencyData[j].Exit.ToString("N0").ToPersianNumber();
                row["Entry"] = currencyData[j].Entry.ToString("N0").ToPersianNumber();
                row["Remaining"] = currency.ToString("N0").ToPersianNumber();

                currencyDataTable.Rows.Add(row);
            }

            var totalOtherCurrency = totalCurrency + (data.Sum(x => x.OtherCurrencyRialExit) - data.Sum(x => x.OtherCurrencyRialEntry));
            report.Dictionary.Variables["OtherCurrencyExit"].Value = data.Sum(x => x.OtherCurrencyRialExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherCurrencyEntry"].Value = data.Sum(x => x.OtherCurrencyRialEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherCurrencyRemaining"].Value = totalOtherCurrency.ToString("N0").ToPersianNumber();

            report.Dictionary.Variables["InventoryCash"].Value = data.Sum(x => x.InventoryCash).ToString("N0").ToPersianNumber();

            var totalGoldDeficit = totalOtherCurrency + (data.Sum(x => x.GoldDeficitExit) - data.Sum(x => x.GoldDeficitEntry));
            report.Dictionary.Variables["GoldDeficitWeight"].Value = data.Sum(x => x.GoldDeficitWeight).ToString().ToPersianNumber();
            report.Dictionary.Variables["GoldDeficitExit"].Value = data.Sum(x => x.GoldDeficitExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["GoldDeficitEntry"].Value = data.Sum(x => x.GoldDeficitEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["GoldDeficitRemaining"].Value = totalGoldDeficit.ToString("N0").ToPersianNumber();

            var totalGift = totalGoldDeficit + (data.Sum(x => x.GiftExit) - data.Sum(x => x.GiftEntry));
            report.Dictionary.Variables["GiftNumberEntry"].Value = data.Sum(x => x.GiftNumberEntry).ToString().ToPersianNumber();
            report.Dictionary.Variables["GiftNumberExit"].Value = data.Sum(x => x.GiftNumberExit).ToString().ToPersianNumber();
            report.Dictionary.Variables["GiftExit"].Value = data.Sum(x => x.GiftExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["GiftEntry"].Value = data.Sum(x => x.GiftEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["GiftRemaining"].Value = totalGift.ToString("N0").ToPersianNumber();

            var totalCheck = totalGift + (data.Sum(x => x.CheckExit) - data.Sum(x => x.CheckEntry));
            report.Dictionary.Variables["CheckNumber"].Value = data.Sum(x => x.CheckNumber).ToString().ToPersianNumber();
            report.Dictionary.Variables["CheckExit"].Value = data.Sum(x => x.CheckExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CheckEntry"].Value = data.Sum(x => x.CheckEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CheckRemaining"].Value = totalCheck.ToString("N0").ToPersianNumber();

            var totalLeatherStone = totalCheck + (data.Sum(x => x.LeatherStoneExit) - data.Sum(x => x.LeatherStoneEntry));
            report.Dictionary.Variables["LeatherStoneDescEntry"].Value = "";
            report.Dictionary.Variables["LeatherStoneDescExit"].Value = "";
            report.Dictionary.Variables["LeatherStoneExit"].Value = data.Sum(x => x.LeatherStoneExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["LeatherStoneEntry"].Value = data.Sum(x => x.LeatherStoneEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["LeatherStoneRemaining"].Value = totalLeatherStone.ToString("N0").ToPersianNumber();

            var totalCoin = totalLeatherStone + (data.Sum(x => x.CoinExit) - data.Sum(x => x.CoinEntry));
            report.Dictionary.Variables["CoinNumber"].Value = data.Sum(x => x.CoinNumber).ToString().ToPersianNumber();
            report.Dictionary.Variables["CoinExit"].Value = data.Sum(x => x.CoinExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CoinEntry"].Value = data.Sum(x => x.CoinEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CoinRemaining"].Value = totalCoin.ToString("N0").ToPersianNumber();

            var totalOtherKiaGold = totalCoin + (data.Sum(x => x.OtherKiaGoldExit) - data.Sum(x => x.OtherKiaGoldEntry));
            report.Dictionary.Variables["OtherKiaGoldWeight"].Value = data.Sum(x => x.OtherKiaGoldWeight).ToString().ToPersianNumber();
            report.Dictionary.Variables["OtherKiaGoldExit"].Value = data.Sum(x => x.OtherKiaGoldExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherKiaGoldEntry"].Value = data.Sum(x => x.OtherKiaGoldEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherKiaGoldRemaining"].Value = totalOtherKiaGold.ToString("N0").ToPersianNumber();

            var totalOtherGold = totalOtherKiaGold + (data.Sum(x => x.OtherGoldExit) - data.Sum(x => x.OtherGoldEntry));
            report.Dictionary.Variables["OtherGoldWeight"].Value = data.Sum(x => x.OtherGoldWeight).ToString().ToPersianNumber();
            report.Dictionary.Variables["OtherGoldExit"].Value = data.Sum(x => x.OtherGoldExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherGoldEntry"].Value = data.Sum(x => x.OtherGoldEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["OtherGoldRemaining"].Value = totalOtherGold.ToString("N0").ToPersianNumber();

            var totalCreditorCustomer = totalOtherGold + (data.Sum(x => x.CreditorCustomerExit) - data.Sum(x => x.CreditorCustomerEntry));
            report.Dictionary.Variables["CreditorCustomerExit"].Value = data.Sum(x => x.CreditorCustomerExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CreditorCustomerEntry"].Value = data.Sum(x => x.CreditorCustomerEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CreditorCustomerRemaining"].Value = totalCreditorCustomer.ToString("N0").ToPersianNumber();

            var totalDebtorCustomer = totalCreditorCustomer + (data.Sum(x => x.DebtorCustomerExit) - data.Sum(x => x.DebtorCustomerEntry));
            report.Dictionary.Variables["DebtorCustomerExit"].Value = data.Sum(x => x.DebtorCustomerExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DebtorCustomerEntry"].Value = data.Sum(x => x.DebtorCustomerEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DebtorCustomerRemaining"].Value = totalDebtorCustomer.ToString("N0").ToPersianNumber();

            var totalDepositBefore = totalDebtorCustomer + (data.Sum(x => x.DepositBeforeExit) - data.Sum(x => x.DepositBeforeEntry));
            report.Dictionary.Variables["DepositBeforeCount"].Value = data.Sum(x => x.DepositBeforeCount).ToString().ToPersianNumber();
            report.Dictionary.Variables["DepositBeforeExit"].Value = data.Sum(x => x.DepositBeforeExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DepositBeforeEntry"].Value = data.Sum(x => x.DepositBeforeEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DepositBeforeRemaining"].Value = totalDepositBefore.ToString("N0").ToPersianNumber();

            var totalDepositNew = totalDepositBefore + (data.Sum(x => x.DepositNewExit) - data.Sum(x => x.DepositNewEntry));
            report.Dictionary.Variables["DepositNewCount"].Value = data.Sum(x => x.DepositNewCount).ToString().ToPersianNumber();
            report.Dictionary.Variables["DepositNewExit"].Value = data.Sum(x => x.DepositNewExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DepositNewEntry"].Value = data.Sum(x => x.DepositNewEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DepositNewRemaining"].Value = totalDepositNew.ToString("N0").ToPersianNumber();

            var totalDiscount = totalDepositNew + (data.Sum(x => x.DiscountExit) - data.Sum(x => x.DiscountEntry));
            report.Dictionary.Variables["DiscountExit"].Value = data.Sum(x => x.DiscountExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DiscountEntry"].Value = data.Sum(x => x.DiscountEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["DiscountRemaining"].Value = totalDiscount.ToString("N0").ToPersianNumber();

            var totalCost = totalDiscount + (data.Sum(x => x.CostExit) - data.Sum(x => x.CostEntry));
            report.Dictionary.Variables["CostCourierPostExit"].Value = data.Sum(x => x.CostExit).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CostCourierPostEntry"].Value = data.Sum(x => x.CostEntry).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["CostCourierPostRemaining"].Value = totalCost.ToString("N0").ToPersianNumber();

            report.Dictionary.Variables["TotalProfit"].Value = (data.Sum(x => x.SaleExit) * 0.059).ToString("N0").ToPersianNumber();
            report.Dictionary.Variables["TotalValue"].Value = (data.Sum(x => x.SaleExit) * 0.0818).ToString("N0").ToPersianNumber();
            #endregion

            report.Dictionary.Databases.Clear();
            report.RegData("BankDataSource", dataTable);
            report.RegData("CurrencyDataSource", currencyDataTable);
            report.Compile();
            report.Render(false);

            MemoryStream stream = new MemoryStream();

            StiPdfExportSettings settings = new StiPdfExportSettings();

            StiPdfExportService service = new StiPdfExportService();
            service.ExportPdf(report, stream, settings);

            return stream.ToArray();
        }

        public TotalData MakeTotalReport(DateTime fromDate, DateTime? toDate = null)
        {
            using (var db = new KiaGalleryMainEntities())
            {
                bool dateRange = true;
                if (toDate == null)
                {
                    toDate = fromDate.AddDays(1);
                    dateRange = false;
                }

                var query = db.DailyReport.Where(x => x.BranchCalendar.ReportDate >= fromDate && x.BranchCalendar.ReportDate <= toDate);

                var data = query.GroupBy(x => x.Branches).Select(x => new BranchesSummary()
                {
                    branchId = x.Key.Id,
                    branchName = x.Key.Name,
                    date = x.Count().ToString(),

                    saleExit = x.Sum(y => y.SaleExit),
                    returnedEntry = x.Sum(y => y.ReturnedEntry),
                    otherKiaGoldEntry = x.Sum(y => y.OtherKiaGoldEntry),
                    otherGoldEntry = x.Sum(y => y.OtherGoldEntry),
                    totalPrice = (x.Sum(y => y.SaleExit) - x.Sum(y => y.ReturnedEntry) - x.Sum(y => y.OtherKiaGoldEntry)) - x.Sum(y => y.OtherGoldEntry),

                    saleWeight = x.Sum(y => y.SaleWeight),
                    saleWeightPercent = x.Sum(y => y.SaleWeightPercent),
                    returnedWeight = x.Sum(y => y.ReturnedWeight),
                    returnedWeightPercent = x.Sum(y => y.ReturnWeightPercent),
                    otherKiaGoldWeight = x.Sum(y => y.OtherKiaGoldWeight),
                    otherGoldWeight = x.Sum(y => y.OtherGoldWeight),
                    totalWeight = (x.Sum(y => y.SaleWeight) + x.Sum(y => y.SaleWeightPercent)) - (x.Sum(y => y.ReturnedWeight) + x.Sum(y => y.ReturnWeightPercent) + x.Sum(y => y.OtherKiaGoldWeight)) - x.Sum(y => y.OtherGoldWeight)
                }).ToList();

                if (data.Count == 0) return null;

                StiReport report = new StiReport();
                report.Load(System.AppDomain.CurrentDomain.BaseDirectory + "/Report/DailyReport.mrt");
                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("BranchName");
                dataTable.Columns.Add("SaleExit");
                dataTable.Columns.Add("ReturnedEntry");
                dataTable.Columns.Add("OtherKiaGoldEntry");
                dataTable.Columns.Add("OtherGoldEntry");
                dataTable.Columns.Add("TotalPrice");
                dataTable.Columns.Add("SaleWeight");
                dataTable.Columns.Add("SaleWeightPercent");
                dataTable.Columns.Add("ReturnedWeight");
                dataTable.Columns.Add("ReturnWeightPercent");
                dataTable.Columns.Add("OtherKiaGoldWeight");
                dataTable.Columns.Add("OtherGoldWeight");
                dataTable.Columns.Add("TotalWeight");

                foreach (var item in data)
                {
                    DataRow row = dataTable.NewRow();
                    row["BranchName"] = item.branchName;
                    row["SaleExit"] = item.saleExit.GetValueOrDefault().ToString("N0").ToPersianNumber();
                    row["ReturnedEntry"] = item.returnedEntry.GetValueOrDefault().ToString("N0").ToPersianNumber();
                    row["OtherKiaGoldEntry"] = item.otherKiaGoldEntry.GetValueOrDefault().ToString("N0").ToPersianNumber();
                    row["OtherGoldEntry"] = item.otherGoldEntry.GetValueOrDefault().ToString("N0").ToPersianNumber();
                    row["TotalPrice"] = item.totalPrice.GetValueOrDefault().ToString("N0").ToPersianNumber();
                    row["SaleWeight"] = item.saleWeight.GetValueOrDefault().ToString().ToPersianNumber();
                    row["SaleWeightPercent"] = item.saleWeightPercent.GetValueOrDefault().ToString().ToPersianNumber();
                    row["ReturnedWeight"] = item.returnedWeight.GetValueOrDefault().ToString().ToPersianNumber();
                    row["ReturnWeightPercent"] = item.returnedWeightPercent.GetValueOrDefault().ToString().ToPersianNumber();
                    row["OtherKiaGoldWeight"] = item.otherKiaGoldWeight.GetValueOrDefault().ToString().ToPersianNumber();
                    row["OtherGoldWeight"] = item.otherGoldWeight.GetValueOrDefault().ToString().ToPersianNumber();
                    row["TotalWeight"] = item.totalWeight.GetValueOrDefault().ToString().ToPersianNumber();
                    dataTable.Rows.Add(row);
                }

                report.Dictionary.Variables["OrderDate"].Value = DateUtility.GetPersianDate(fromDate) + (dateRange ? " - " + DateUtility.GetPersianDate(toDate) : ".");
                report.Dictionary.Variables["SumSaleExit"].Value = data.Sum(x => x.saleExit).GetValueOrDefault().ToString("N0").ToPersianNumber();
                report.Dictionary.Variables["SumReturnedEntry"].Value = data.Sum(x => x.returnedEntry).GetValueOrDefault().ToString("N0").ToPersianNumber();
                report.Dictionary.Variables["SumOtherKiaGoldEntry"].Value = data.Sum(x => x.otherKiaGoldEntry).GetValueOrDefault().ToString("N0").ToPersianNumber();
                report.Dictionary.Variables["SumOtherGoldEntry"].Value = data.Sum(x => x.otherGoldEntry).GetValueOrDefault().ToString("N0").ToPersianNumber();
                report.Dictionary.Variables["SumTotalPrice"].Value = data.Sum(x => x.totalPrice).GetValueOrDefault().ToString("N0").ToPersianNumber();
                report.Dictionary.Variables["SumSaleWeight"].Value = data.Sum(x => x.saleWeight).GetValueOrDefault().ToString().ToPersianNumber();
                report.Dictionary.Variables["SumSaleWeightPercent"].Value = data.Sum(x => x.saleWeightPercent).GetValueOrDefault().ToString().ToPersianNumber();
                report.Dictionary.Variables["SumReturnedWeight"].Value = data.Sum(x => x.returnedWeight).GetValueOrDefault().ToString().ToPersianNumber();
                report.Dictionary.Variables["SumReturnWeightPercent"].Value = data.Sum(x => x.returnedWeightPercent).GetValueOrDefault().ToString().ToPersianNumber();
                report.Dictionary.Variables["SumOtherKiaGoldWeight"].Value = data.Sum(x => x.otherKiaGoldWeight).GetValueOrDefault().ToString().ToPersianNumber();
                report.Dictionary.Variables["SumOtherGoldWeight"].Value = data.Sum(x => x.otherGoldWeight).GetValueOrDefault().ToString().ToPersianNumber();
                report.Dictionary.Variables["SumTotalWeight"].Value = data.Sum(x => x.totalWeight).GetValueOrDefault().ToString().ToPersianNumber();

                var totalPrice = data.Sum(x => x.totalPrice).GetValueOrDefault();
                var totalWeight = data.Sum(x => x.saleWeight).GetValueOrDefault();

                report.Dictionary.Databases.Clear();
                report.ScriptLanguage = StiReportLanguageType.CSharp;
                report.RegData("DataSource", dataTable.DefaultView);
                report.Compile();
                report.Render(false);


                MemoryStream stream = new MemoryStream();
                StiPdfExportSettings settings = new StiPdfExportSettings();
                StiPdfExportService service = new StiPdfExportService();
                service.ExportPdf(report, stream, settings);
                return new TotalData
                {
                    data = stream.ToArray(),
                    totalPrice = totalPrice,
                    totalWeight = totalWeight
                };
            }
        }

        public void saveToFile(Exception ex)
        {
            string filePath = @"D:\Error.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }
    }

    public class HelperData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Exit { get; set; }
        public long Entry { get; set; }
    }

    public class TotalData
    {
        public byte[] data { get; set; }
        public long totalPrice { get; set; }
        public decimal totalWeight { get; set; }
    }

}
